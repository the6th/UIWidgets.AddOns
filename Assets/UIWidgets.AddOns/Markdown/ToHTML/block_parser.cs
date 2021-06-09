﻿using markdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.UIWidgets.foundation;
using UnityEngine;

namespace markdown
{
    public class BlockParser
    {
        /// The line contains only whitespace or is empty.
        public static Regex _emptyPattern = new Regex(@"^(?:[ \t]*)$");

        /// A series of `=` or `-` (on the next line) define setext-style headers.
        public static Regex _setextPattern = new Regex(@"^[ ]{0,3}(=+|-+)\s*$");

        /// Leading (and trailing) `#` define atx-style headers.
        ///
        /// Starts with 1-6 unescaped `#` characters which must not be followed by a
        /// non-space character. Line may end with any number of `#` characters,.
        public static Regex _headerPattern = new Regex(@"^ {0,3}(#{1,6})[ \x09\x0b\x0c](.*?)#*$");

        /// The line starts with `>` with one optional space after.
        public static Regex _blockquotePattern = new Regex(@"^[ ]{0,3}>[ ]?(.*)$");

        /// A line indented four spaces. Used for code blocks and lists.
        public static Regex _indentPattern = new Regex(@"^(?:    | {0,3}\t)(.*)$");

        /// Fenced code block.
        public static Regex _codePattern = new Regex(@"^[ ]{0,3}(`{3,}|~{3,})(.*)$");

        /// Three or more hyphens, asterisks or underscores by themselves. Note that
        /// a line like `----` is valid as both HR and SETEXT. In case of a tie,
        /// SETEXT should win.
        public static Regex _hrPattern = new Regex(@"^ {0,3}([-*_])[ \t]*\1[ \t]*\1(?:\1|[ \t])*$");

        /// One or more whitespace, for compressing.
        public static Regex _oneOrMoreWhitespacePattern = new Regex(@"[ \n\r\t]+");

        /// A line starting with one of these markers: `-`, `*`, `+`. May have up to
        /// three leading spaces before the marker and any number of spaces or tabs
        /// after.
        ///
        /// Contains a dummy group at [2], so that the groups in [_ulPattern] and
        /// [_olPattern] match up; in both, [2] is the length of the number that begins
        /// the list marker.
        public static Regex _ulPattern = new Regex(@"^([ ]{0,3})()([*+-])(([ \t])([ \t]*)(.*))?$");

        /// A line starting with a number like `123.`. May have up to three leading
        /// spaces before the marker and any number of spaces or tabs after.
        public static Regex _olPattern = new Regex(@"^([ ]{0,3})(\d{1,9})([\.)])(([ \t])([ \t]*)(.*))?$");

        /// A line of hyphens separated by at least one pipe.
        public static Regex _tablePattern = new Regex(@"^[ ]{0,3}\|?( *:?\-+:? *\|)+( *:?\-+:? *)?$");


        private List<string> lines;
        public Document document;

        /// The enabled block syntaxes.
        ///
        /// To turn a series of lines into blocks, each of these will be tried in
        /// turn. Order matters here.
        public List<BlockSyntax> blockSyntaxes = new List<BlockSyntax>();

        /// Index of the current line.
        private int _pos = 0;

        /// Whether the parser has encountered a blank line between two block-level
        /// elements.
        public bool encounteredBlankLine = false;

        List<BlockSyntax> standardBlockSyntaxes = new List<BlockSyntax>()
        {
            new EmptyBlockSyntax(),
            new BlockTagBlockHtmlSyntax(),
            new LongBlockHtmlSyntax(@"^ {0,3}<pre(?:\s|>|$)", "</pre>"),
            new LongBlockHtmlSyntax(@"^ {0,3}<script(?:\s|>|$)", "</script>"),
            new LongBlockHtmlSyntax(@"^ {0,3}<style(?:\s|>|$)", "</style>"),
            new LongBlockHtmlSyntax(@"^ {0,3}<!--", "-->"),
            new LongBlockHtmlSyntax(@"^ {0,3}<\?", "\\?>"),
            new LongBlockHtmlSyntax(@"^ {0,3}<![A-Z]", ">"),
            new LongBlockHtmlSyntax(@"^ {0,3}<!\[CDATA\[", "\\]\\]>"),
            new OtherTagBlockHtmlSyntax(),
            new SetextHeaderSyntax(),
            new HeaderSyntax(),
            new CodeBlockSyntax(),
            new BlockquoteSyntax(),
            new HorizontalRuleSyntax(),
            new UnorderedListSyntax(),
            new OrderedListSyntax(),
            new ParagraphSyntax()
        };

        public BlockParser(List<string> lines, Document document)
        {
            this.lines = lines;
            this.document = document;

            blockSyntaxes.AddRange(document.blockSyntaxes);
            blockSyntaxes.AddRange(standardBlockSyntaxes);
        }

        public string current
        {
            get
            {
//                Debug.Log(_pos + " -> " + lines.Count);
                if (_pos < lines.Count && _pos >= 0)
                    return lines[_pos];
                else
                {
                    return string.Empty;
                }
            }
        }

        public string next
        {
            get
            {
                if (_pos >= lines.Count - 1) return null;

                return lines[_pos + 1];
            }
        }

        public string peek(int linesAhead)
        {
            if (linesAhead < 0)
            {
                throw new ArgumentException(string.Format("Invalid linesAhead: {0}; must be >= 0.", linesAhead));
            }

            if (_pos >= lines.Count - linesAhead) return null;

            return lines[_pos + linesAhead];
        }

        public void advance()
        {
            _pos++;
        }

        public bool isDone
        {
            get { return _pos >= lines.Count; }
        }

        public bool matches(Regex regex)
        {
            if (isDone) return false;

            return regex.Match(current).Success;
        }

        public bool matchesNext(Regex regex)
        {
            if (next == null) return false;
            return regex.Match(next).Success;
        }

        public List<Node> parseLines()
        {
            var blocks = new List<Node>();
            while (!isDone)
            {
                foreach (var syntax in blockSyntaxes)
                {
                    if (syntax.canParse(this))
                    {
                        var block = syntax.parse(this);
                        if (block != null)
                        {
                            blocks.Add(block);
                            break;
                        }
                    }
                }
            }

            return blocks;
        }
    }

    public abstract class BlockSyntax
    {
        public virtual Regex pattern
        {
            get { return null; }
        }

        public virtual bool canEndBlock
        {
            get { return true; }
        }

        public virtual bool canParse(BlockParser parser)
        {
       
            return pattern.hasMatch(parser.current);
        }

        public abstract Node parse(BlockParser parser);


        public virtual List<string> parseChildLines(BlockParser parser)
        {
            var childLines = new List<string>();

            while (!parser.isDone)
            {
                var match = pattern.Match(parser.current);
                if (!match.Success) break;

                childLines.Add(match.Groups[1].Value);

                parser.advance();
            }

            return childLines;
        }

        protected static bool isAtBlockEnd(BlockParser parser)
        {
            if (parser.isDone) return true;

            return parser.blockSyntaxes.Any(s => s.canParse(parser) && s.canEndBlock);
        }

        public static string generateAnchorHash(Element elment)
        {
            var str = elment.children.first().textContent.ToLower().Trim();
            var regStr = @"[^a-z0-9 _-]";
            return Regex.Replace(Regex.Replace(str, regStr, ""), @"\s", "-");
        }
    }

    class EmptyBlockSyntax : BlockSyntax
    {
        public override Regex pattern
        {
            get { return BlockParser._emptyPattern; }
        }


        public override Node parse(BlockParser parser)
        {
            parser.encounteredBlankLine = true;
            parser.advance();

            return null;
        }
    }

    class SetextHeaderSyntax : BlockSyntax
    {
        public override bool canParse(BlockParser parser)
        {
            if (!_interperableAsParagraph(parser.current))
            {
                return false;
            }

            var i = 1;
            while (true)
            {
                var nextLine = parser.peek(i);
                if (nextLine == null)
                {
                    // We never reached an underline.
                    return false;
                }

                if (BlockParser._setextPattern.hasMatch(nextLine))
                {
                    return true;
                }

                if (!_interperableAsParagraph(nextLine))
                {
                    return false;
                }

                i++;
            }
        }

        public override Node parse(BlockParser parser)
        {
            var lines = new List<string>();
            string tag = string.Empty;
            while (!parser.isDone)
            {
                var match = BlockParser._setextPattern.Match(parser.current);
                if (!match.Success)
                {
                    lines.Add(parser.current);
                    parser.advance();
                    continue;
                }
                else
                {
                    tag = match.Groups[1].Value[0] == '=' ? "h1" : "h2";
                    parser.advance();
                    break;
                }
            }


            var contents = new UnparsedContent(lines.join('\n'));
            return new Element(tag, new List<Node>() {contents});
        }

        public bool _interperableAsParagraph(string line)
        {
            return !(BlockParser._indentPattern.hasMatch(line) ||
                     BlockParser._codePattern.hasMatch(line) ||
                     BlockParser._headerPattern.hasMatch(line) ||
                     BlockParser._blockquotePattern.hasMatch(line) ||
                     BlockParser._hrPattern.hasMatch(line) ||
                     BlockParser._ulPattern.hasMatch(line) ||
                     BlockParser._olPattern.hasMatch(line) ||
                     BlockParser._emptyPattern.hasMatch(line));
        }
    }


    class SetextHeaderWithIdSyntax : SetextHeaderSyntax
    {
        public override Node parse(BlockParser parser)
        {
            var element = base.parse(parser) as Element;
            element.generatedId = BlockSyntax.generateAnchorHash(element);
            return element;
        }
    }


    /// Parses atx-style headers: `## Header ##`.
    class HeaderSyntax : BlockSyntax
    {
        public override Regex pattern
        {
            get { return BlockParser._headerPattern; }
        }


        public override Node parse(BlockParser parser)
        {
            var match = pattern.Match(parser.current);
            parser.advance();
            var level = match.Groups[1].Length;
            var contents = new UnparsedContent(match.Groups[2].Value.Trim());
            return new Element("h" + level, new List<Node>() {contents});
        }
    }


    class HeaderWithIdSyntax : HeaderSyntax
    {
        public override Node parse(BlockParser parser)
        {
            var element = base.parse(parser) as Element;
            element.generatedId = BlockSyntax.generateAnchorHash(element);
            return element;
        }
    }

    /// Parses email-style blockquotes: `> quote`.
    class BlockquoteSyntax : BlockSyntax
    {
        public override Regex pattern
        {
            get { return BlockParser._blockquotePattern; }
        }

        public override List<string> parseChildLines(BlockParser parser)
        {
            // Grab all of the lines that form the blockquote, stripping off the ">".
            var childLines = new List<string>();

            while (!parser.isDone)
            {
                var match = pattern.Match(parser.current);
                if (match.Success)
                {
                    childLines.Add(match.Groups[1].Value);
                    parser.advance();
                    continue;
                }

                // A paragraph continuation is OK. This is content that cannot be parsed
                // as any other syntax except Paragraph, and it doesn't match the bar in
                // a Setext header.
                if (parser.blockSyntaxes.First(s => s.canParse(parser)) is ParagraphSyntax)
                {
                    childLines.Add(parser.current);
                    parser.advance();
                }
                else
                {
                    break;
                }
            }

            return childLines;
        }

        public override Node parse(BlockParser parser)
        {
            var childLines = parseChildLines(parser);

            // Recursively parse the contents of the blockquote.
            var children = new BlockParser(childLines, parser.document).parseLines();

            return new Element("blockquote", children);
        }
    }

    class CodeBlockSyntax : BlockSyntax
    {
        public override Regex pattern
        {
            get { return BlockParser._indentPattern; }
        }

        public override bool canEndBlock
        {
            get { return false; }
        }

        public override List<string> parseChildLines(BlockParser parser)
        {
            var childLines = new List<string>();

            while (!parser.isDone)
            {
                var match = pattern.Match(parser.current);
                if (match.Success)
                {
                    childLines.Add(match.Groups[1].Value);
                    parser.advance();
                }
                else
                {
                    // If there's a codeblock, then a newline, then a codeblock, keep the
                    // code blocks together.
                    var nextMatch =
                        parser.next != null ? pattern.Match(parser.next) : null;
                    if (parser.current.Trim() == "" && nextMatch != null)
                    {
                        childLines.Add("");
                        childLines.Add(nextMatch.Groups[1].Value);
                        parser.advance();
                        parser.advance();
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return childLines;
        }

        public override Node parse(BlockParser parser)
        {
            var childLines = parseChildLines(parser);

            // The Markdown tests expect a trailing newline.
            childLines.Add("");

            var escaped = Utils.escapeHtml(childLines.@join("\n"));

            return new Element("pre", new List<Node>() {Element.text("code", escaped)});
        }
    }

    /// Parses preformatted code blocks between two ~~~ or ``` sequences.
    ///
    /// See [Pandoc's documentation](http://pandoc.org/README.html#fenced-code-blocks).
    class FencedCodeBlockSyntax : BlockSyntax
    {
        public override Regex pattern
        {
            get { return BlockParser._codePattern; }
        }


        List<String> parseChildLines(BlockParser parser, String endBlock = "")
        {
            if (endBlock == null) endBlock = "";

            var childLines = new List<string>();
            parser.advance();

            while (!parser.isDone)
            {
                var match = pattern.Match(parser.current);
                if (!match.Success || !match.Groups[1].Value.StartsWith(endBlock))
                {
                    childLines.Add(parser.current);
                    parser.advance();
                }
                else
                {
                    parser.advance();
                    break;
                }
            }

            return childLines;
        }

        public override Node parse(BlockParser parser)
        {
            // Get the syntax identifier, if there is one.
            var match = pattern.Match(parser.current);
            var endBlock = match.Groups[1].Value;
            var infoString = match.Groups[2].Value;

            var childLines = parseChildLines(parser, endBlock);

            // The Markdown tests expect a trailing newline.
            childLines.Add("");

            var text = childLines.join('\n');
            if (parser.document.encodeHtml)
            {
                // Escape the code.
                text = Utils.escapeHtml(text);
            }

            var code = Element.text("code", text);

            // the info-string should be trimmed
            // http://spec.commonmark.org/0.22/#example-100
            infoString = infoString.Trim();
            if (infoString.isNotEmpty())
            {
                // only use the first word in the syntax
                // http://spec.commonmark.org/0.22/#example-100
                infoString = infoString.Split(' ').first();
                code.attributes["class"] = "language-" + infoString;
            }

            var element = new Element("pre", new List<Node>() {code});

            return element;
        }
    }


    /// Parses horizontal rules like `---`, `_ _ _`, `*  *  *`, etc.
    class HorizontalRuleSyntax : BlockSyntax
    {
        public override Regex pattern
        {
            get { return BlockParser._hrPattern; }
        }


        public override Node parse(BlockParser parser)
        {
            parser.advance();
            return Element.empty("hr");
        }
    }

    /// Parses inline HTML at the block level. This differs from other Markdown
    /// implementations in several ways:
    ///
    /// 1.  This one is way way WAY simpler.
    /// 2.  Essentially no HTML parsing or validation is done. We're a Markdown
    ///     parser, not an HTML parser!
    abstract class BlockHtmlSyntax : BlockSyntax
    {
        public override bool canEndBlock
        {
            get { return true; }
        }
    }

    class BlockTagBlockHtmlSyntax : BlockHtmlSyntax
    {
        static Regex _pattern = new Regex(
            @"^ {0,3}</?(?:address|article|aside|base|basefont|blockquote|body|caption|center|col|colgroup|dd|details|dialog|dir|div|dl|dt|fieldset|figcaption|figure|footer|form|frame|frameset|h1|head|header|hr|html|iframe|legend|li|link|main|menu|menuitem|meta|nav|noframes|ol|optgroup|option|p|param|section|source|summary|table|tbody|td|tfoot|th|thead|title|tr|track|ul)(?:\s|>|/>|$)");

        public override Regex pattern
        {
            get { return _pattern; }
        }

        public override Node parse(BlockParser parser)
        {
            var childLines = new List<string>();

            // Eat until we hit a blank line.
            while (!parser.isDone && !parser.matches(BlockParser._emptyPattern))
            {
                childLines.Add(parser.current);
                parser.advance();
            }

            return new Text(childLines.join("\n"));
        }
    }

    class OtherTagBlockHtmlSyntax : BlockTagBlockHtmlSyntax
    {
        public override Boolean canEndBlock
        {
            get { return true; }
        }

        // Really hacky way to detect "other" HTML. This matches:
        //
        // * any opening spaces
        // * open bracket and maybe a slash ("<" or "</")
        // * some word characters
        // * either:
        //   * a close bracket, or
        //   * whitespace followed by not-brackets followed by a close bracket
        // * possible whitespace and the end of the line.
        public override Regex pattern
        {
            get { return new Regex(@"^ {0,3}</?\w+(?:>|\s+[^>]*>)\s*$"); }
        }
    }

    /// A BlockHtmlSyntax that has a specific `endPattern`.
    ///
    /// In practice this means that the syntax dominates; it is allowed to eat
    /// many lines, including blank lines, before matching its `endPattern`.
    class LongBlockHtmlSyntax : BlockHtmlSyntax
    {
        private Regex _pattern;

        public override Regex pattern
        {
            get { return _pattern; }
        }

        public Regex _endPattern;

        public LongBlockHtmlSyntax(String patternStr, String endPatternStr)
        {
            _pattern = new Regex(patternStr);
            _endPattern = new Regex(endPatternStr);
        }


        public override Node parse(BlockParser parser)
        {
            var childLines = new List<string>();
            // Eat until we hit [endPattern].
            while (!parser.isDone)
            {
                childLines.Add(parser.current);
                if (parser.matches(_endPattern)) break;
                parser.advance();
            }

            parser.advance();
            return new Text(childLines.join("\n"));
        }
    }

    class ListItem
    {
        internal bool forceBlock = false;
        public List<string> lines;

        public ListItem(List<string> lines)
        {
            this.lines = lines;
        }
    }

    /// Base class for both ordered and unordered lists.
    abstract class ListSyntax : BlockSyntax
    {
        public override bool canEndBlock
        {
            get { return true; }
        }

        public abstract string listTag { get; }


        /// A list of patterns that can start a valid block within a list item.
        static List<Regex> blocksInList = new List<Regex>()
        {
            BlockParser._blockquotePattern,
            BlockParser._headerPattern,
            BlockParser._hrPattern,
            BlockParser._indentPattern,
            BlockParser._ulPattern,
            BlockParser._olPattern
        };

        static Regex _whitespaceRe = new Regex("[ \t]*");

        void endItem(ref List<string> childLines, List<ListItem> items)
        {
            if (childLines.isNotEmpty())
            {
                items.Add(new ListItem(childLines));
                childLines = new List<string>();
            }
        }

        bool tryMatch(Regex pattern, BlockParser parser, ref Match match)
        {
            match = pattern.Match(parser.current);
            return match.Success;
        }

        public override Node parse(BlockParser parser)
        {
            var items = new List<ListItem>();
            var childLines = new List<string>();


            Match match = null;


            string listMarker = null;
            string indent = null;
            // In case the first number in an ordered list is not 1, use it as the
            // "start".
            int startNumber = 0;

            while (!parser.isDone)
            {
                var leadingSpace = _whitespaceRe.matchAsPrefix(parser.current).Groups[0].Value;
                var leadingExpandedTabLength = _expandedTabLength(leadingSpace);
                if (tryMatch(BlockParser._emptyPattern, parser, ref match))
                {
                    if (BlockParser._emptyPattern.Match(parser.next ?? "").Success)
                    {
                        // Two blank lines ends a list.
                        break;
                    }

                    // Add a blank line to the current list item.
                    childLines.Add("");
                }
                else if (!string.IsNullOrEmpty(indent) && indent.Length <= leadingExpandedTabLength)
                {
                    // Strip off indent and add to current item.
                    var line = parser.current
                        .replaceFirst(leadingSpace, new string(' ', leadingExpandedTabLength))
                        .replaceFirst(indent, "");
                    childLines.Add(line);
                }
                else if (tryMatch(BlockParser._hrPattern, parser, ref match))
                {
                    // Horizontal rule takes precedence to a new list item.
                    break;
                }
                else if (tryMatch(BlockParser._ulPattern, parser, ref match) ||
                         tryMatch(BlockParser._olPattern, parser, ref match))
                {
                    var precedingWhitespace = match.Groups[1].Value;
                    var digits = match.Groups[2].Value ?? "";
                    if (startNumber == 0 && digits.isNotEmpty())
                    {
                        startNumber = int.Parse(digits);
                    }

                    var marker = match.Groups[3];
                    var firstWhitespace = match.Groups[5].Value ?? "";
                    var restWhitespace = match.Groups[6].Value ?? "";
                    var content = match.Groups[7].Value ?? "";
                    var isBlank = content.isEmpty();
                    if (listMarker != null && listMarker != marker.Value)
                    {
                        // Changing the bullet or ordered list delimiter starts a new list.
                        break;
                    }

                    listMarker = marker.Value;
                    var markerAsSpaces = new string(' ', digits.Length + marker.Length);
                    if (isBlank)
                    {
                        // See http://spec.commonmark.org/0.28/#list-items under "3. Item
                        // starting with a blank line."
                        //
                        // If the list item starts with a blank line, the final piece of the
                        // indentation is just a single space.
                        indent = precedingWhitespace + markerAsSpaces + ' ';
                    }
                    else if (restWhitespace.Length >= 4)
                    {
                        // See http://spec.commonmark.org/0.28/#list-items under "2. Item
                        // starting with indented code."
                        //
                        // If the list item starts with indented code, we need to _not_ count
                        // any indentation past the required whitespace character.
                        indent = precedingWhitespace + markerAsSpaces + firstWhitespace;
                    }
                    else
                    {
                        indent = precedingWhitespace +
                                 markerAsSpaces +
                                 firstWhitespace +
                                 restWhitespace;
                    }

                    // End the current list item and start a new one.
                    endItem(ref childLines, items);
                    childLines.Add(restWhitespace + content);
                }
                else if (BlockSyntax.isAtBlockEnd(parser))
                {
                    // Done with the list.
                    break;
                }
                else
                {
                    // If the previous item is a blank line, this means we're done with the
                    // list and are starting a new top-level paragraph.
                    if ((childLines.isNotEmpty()) && (childLines.last() == ""))
                    {
                        parser.encounteredBlankLine = true;
                        break;
                    }

                    // Anything else is paragraph continuation text.
                    childLines.Add(parser.current);
                }

                parser.advance();
            }

            endItem(ref childLines, items);
            var itemNodes = new List<Element>();

            items.ForEach(removeLeadingEmptyLine);
            var anyEmptyLines = removeTrailingEmptyLines(items);
            var anyEmptyLinesBetweenBlocks = false;

            foreach (var item in items)
            {
                var itemParser = new BlockParser(item.lines, parser.document);
                var children = itemParser.parseLines();
                itemNodes.Add(new Element("li", children));
                anyEmptyLinesBetweenBlocks =
                    anyEmptyLinesBetweenBlocks || itemParser.encounteredBlankLine;
            }

            // Must strip paragraph tags if the list is "tight".
            // http://spec.commonmark.org/0.28/#lists
            var listIsTight = !anyEmptyLines && !anyEmptyLinesBetweenBlocks;

            if (listIsTight)
            {
                // We must post-process the list items, converting any top-level paragraph
                // elements to just text elements.
                foreach (var item in itemNodes)
                {
                    for (var i = 0; i < item.children.Count; i++)
                    {
                        var child = item.children[i];
                        var ele = child as Element;
                        if (ele != null && ele.tag == "p")
                        {
                            item.children.RemoveAt(i);
                            item.children.InsertRange(i, ele.children);
                        }
                    }
                }
            }

            if (listTag == "ol" && startNumber != 1)
            {
                var element = new Element(listTag, itemNodes);
                element.attributes["start"] = startNumber.ToString();
                return element;
            }
            else
            {
                return new Element(listTag, itemNodes);
            }
        }

        void removeLeadingEmptyLine(ListItem item)
        {
            if (item.lines.isNotEmpty() && BlockParser._emptyPattern.hasMatch(item.lines.first()))
            {
                item.lines.RemoveAt(0);
            }
        }

        /// Removes any trailing empty lines and notes whether any items are separated
        /// by such lines.
        bool removeTrailingEmptyLines(List<ListItem> items)
        {
            var anyEmpty = false;
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].lines.Count == 1) continue;
                while (items[i].lines.isNotEmpty() &&
                       BlockParser._emptyPattern.hasMatch(items[i].lines.last()))
                {
                    if (i < items.Count - 1)
                    {
                        anyEmpty = true;
                    }

                    items[i].lines.removeLast();
                }
            }

            return anyEmpty;
        }

        static int _expandedTabLength(String input)
        {
            var length = 0;
            foreach (var cha in input)
            {
                length += cha == 0x9 ? 4 - (length % 4) : 1;
            }

            return length;
        }
    }


    /// Parses unordered lists.
    class UnorderedListSyntax : ListSyntax
    {
        public override Regex pattern
        {
            get { return BlockParser._ulPattern; }
        }

        public override string listTag
        {
            get { return "ul"; }
        }
    }

    /// Parses ordered lists.
    class OrderedListSyntax : ListSyntax
    {
        public override Regex pattern
        {
            get { return BlockParser._olPattern; }
        }

        public override string listTag
        {
            get { return "ol"; }
        }
    }

    /// Parses tables.
    class TableSyntax : BlockSyntax
    {
        static Regex _pipePattern = new Regex(@"^\s*\|\s*");
        static Regex _openingPipe = new Regex(@"^\|\s*");
        static Regex _closingPipe = new Regex(@"\s*\|$");

        public override bool canEndBlock
        {
            get { return false; }
        }


        public override bool canParse(BlockParser parser)
        {
            // Note: matches *next* line, not the current one. We're looking for the
            // bar separating the head row from the body rows.
            return parser.matchesNext(BlockParser._tablePattern);
        }

        /// Parses a table into its three parts:
        ///
        /// * a head row of head cells (`<th>` cells)
        /// * a divider of hyphens and pipes (not rendered)
        /// * many body rows of body cells (`<td>` cells)
        public override Node parse(BlockParser parser)
        {
            var alignments = parseAlignments(parser.next);
            var columnCount = alignments.Count;
            var headRow = parseRow(parser, alignments, "th");
            if (headRow.children.Count != columnCount)
            {
                return null;
            }

            var head = new Element("thead", new List<Node>() {headRow});

            // Advance past the divider of hyphens.
            parser.advance();

            var rows = new List<Element>() { };
            while (!parser.isDone && !BlockSyntax.isAtBlockEnd(parser))
            {
                var row = parseRow(parser, alignments, "td");
                while (row.children.Count < columnCount)
                {
                    // Insert synthetic empty cells.
                    row.children.Add(Element.empty("td"));
                }

                while (row.children.Count > columnCount)
                {
                    row.children.removeLast();
                }

                rows.Add(row);
            }

            if (rows.isEmpty())
            {
                return new Element("table", new List<Node>() {head});
            }
            else
            {
                var body = new Element("tbody", rows);

                return new Element("table", new List<Node>() {head, body});
            }
        }

        List<String> parseAlignments(String line)
        {
            line = line.replaceFirst(_openingPipe, "").replaceFirst(_closingPipe, "");
            return line.Split('|').Select(column =>
            {
                column = column.Trim();
                if (column.StartsWith(":") && column.EndsWith(":")) return "center";
                if (column.StartsWith(":")) return "left";
                if (column.EndsWith(":")) return "right";
                return null;
            }).ToList();
        }

        Element parseRow(
            BlockParser parser, List<String> alignments, String cellType)
        {
            var line = parser.current
                .replaceFirst(_openingPipe, "")
                .replaceFirst(_closingPipe, "");
            var cells = line.split(_pipePattern);
            parser.advance();
            var row = new List<Element>();
            string preCell = string.Empty;

            for (var index = 0; index < cells.Length; index++)
            {
                var cell = cells[index];
                if (preCell != null)
                {
                    cell = preCell + cell;
                    preCell = null;
                }

                if (cell.EndsWith("\\"))
                {
                    preCell = cell.Substring(0, cell.Length - 1) + '|';
                    continue;
                }

                var contents = new UnparsedContent(cell);
                row.Add(new Element(cellType, new List<Node>() {contents}));
            }

            for (var i = 0; i < row.Count && i < alignments.Count; i++)
            {
                if (alignments[i] == null) continue;
                row[i].attributes["style"] = "text-align: " + alignments[i] + ';';
            }

            return new Element("tr", row);
        }
    }


    /// Parses paragraphs of regular text.
    class ParagraphSyntax : BlockSyntax
    {
        static Regex _reflinkDefinitionStart = new Regex(@"[ ]{0,3}\[");
        static Regex _whitespacePattern = new Regex(@"^\s*$");

        public override bool canEndBlock
        {
            get { return false; }
        }

        public override bool canParse(BlockParser parser) => true;

        public override Node parse(BlockParser parser)
        {
            var childLines = new List<string>();

            // Eat until we hit something that ends a paragraph.
            while (!BlockSyntax.isAtBlockEnd(parser))
            {
                childLines.Add(parser.current);
                parser.advance();
            }

            var paragraphLines = _extractReflinkDefinitions(parser, childLines);
            if (paragraphLines == null)
            {
                // Paragraph consisted solely of reference link definitions.
                return new Text("");
            }
            else
            {
                var contents = new UnparsedContent(paragraphLines.join('\n'));
                return new Element("p", new List<Node>() {contents});
            }
        }

        bool lineStartsReflinkDefinition(List<string> lines, int i)
        {
            if (i < lines.Count && i >= 0)
                return lines[i].startsWith(_reflinkDefinitionStart);
            else
            {
                return false;
            }
        }

        /// Extract reference link definitions from the front of the paragraph, and
        /// return the remaining paragraph lines.
        List<String> _extractReflinkDefinitions(
            BlockParser parser, List<String> lines)
        {
            var i = 0;
            loopOverDefinitions:
            while (true)
            {
//                Debug.LogWarning("--->" + i);
                // Check for reflink definitions.
                if (!lineStartsReflinkDefinition(lines, i))
                {
                    // It's paragraph content from here on out.
                    break;
                }

                var contents = lines[i];
                var j = i + 1;
                while (j < lines.Count)
                {
                    // Check to see if the _next_ line might start a new reflink definition.
                    // Even if it turns out not to be, but it started with a '[', then it
                    // is not a part of _this_ possible reflink definition.
                    if (lineStartsReflinkDefinition(lines, j))
                    {
                        // Try to parse [contents] as a reflink definition.
                        if (_parseReflinkDefinition(parser, contents))
                        {
                            // Loop again, starting at the next possible reflink definition.
                            i = j;
                            Debug.Log("--->" + i);
                            goto loopOverDefinitions;
                        }
                        else
                        {
                            // Could not parse [contents] as a reflink definition.
                            break;
                        }
                    }
                    else
                    {
                        contents = contents + '\n' + lines[j];
                        j++;
                    }
                }

                // End of the block.
                if (_parseReflinkDefinition(parser, contents))
                {
                    i = j;
                    break;
                }

                // It may be that there is a reflink definition starting at [i], but it
                // does not extend all the way to [j], such as:
                //
                //     [link]: url     // line i
                //     "title"
                //     garbage
                //     [link2]: url   // line j
                //
                // In this case, [i, i+1] is a reflink definition, and the rest is
                // paragraph content.
                while (j >= i)
                {
                    // This isn't the most efficient loop, what with this big ole'
                    // Iterable allocation (`getRange`) followed by a big 'ole String
                    // allocation, but we
                    // must walk backwards, checking each range.
                    contents = lines.getRange(i, j).join('\n');
                    if (_parseReflinkDefinition(parser, contents))
                    {
                        // That is the last reflink definition. The rest is paragraph
                        // content.
                        i = j;
                        break;
                    }

                    j--;
                }
                // The ending was not a reflink definition at all. Just paragraph
                // content.

                break;
            }

            if (i == lines.Count)
            {
                // No paragraph content.
                return null;
            }
            else
            {
                // Ends with paragraph content.
                return lines.sublist(i);
            }
        }

        // Parse [contents] as a reference link definition.
        //
        // Also adds the reference link definition to the document.
        //
        // Returns whether [contents] could be parsed as a reference link definition.
        bool _parseReflinkDefinition(BlockParser parser, string contents)
        {
            var pattern =
                new Regex(
                    @"^[ ]{0,3}\[((?:\\\]|[^\]])+)\]:\s*(?:<(\S+)>|(\S+))\s*(""[^""]+""|'[^']+'|\([^)]+\)|)\s*$",
                    RegexOptions.Multiline);

            var match = pattern.Match(contents);
            if (!match.Success)
            {
                // Not a reference link definition.
                return false;
            }

            if (match.Groups[0].Length < contents.Length)
            {
                // Trailing text. No good.
                return false;
            }

            var label = match.Groups[1].Value;
            var destination = string.IsNullOrEmpty(match.Groups[2].Value)
                ? match.Groups[3].Value
                : match.Groups[2].Value;
            var title = match.Groups[4].Value;

            // The label must contain at least one non-whitespace character.
            if (_whitespacePattern.hasMatch(label))
            {
                return false;
            }

            if (string.IsNullOrEmpty(title))
            {
                // No title.
                title = null;
            }
            else
            {
                // Remove "", '', or ().
                title = title.substring(1, title.Length - 1);
            }

            // References are case-insensitive, and internal whitespace is compressed.
            label =
                label.ToLower().Trim().replaceAll(BlockParser._oneOrMoreWhitespacePattern, ' ');

            parser.document.linkReferences
                .putIfAbsent(label, () => new LinkReference(label, destination, title));
            return true;
        }
    }
}