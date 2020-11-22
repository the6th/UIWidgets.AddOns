using System;
using System.Collections.Generic;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace UIWidgets.AddOns
{
    public class MaterialPicker : StatefulWidget
    {
        public Color pickerColor;
        public ValueChanged<Color> onColorChanged;
        public bool enableLabel;

        public MaterialPicker(
            Color pickerColor,
            ValueChanged<Color> onColorChanged,
            bool enableLabel = false
            )
        {
            D.assert(pickerColor != null);
            D.assert(onColorChanged != null);
            this.pickerColor = pickerColor;
            this.onColorChanged = onColorChanged;
            this.enableLabel = enableLabel;
        }

        public override State createState()
        {
            return new _MaterialPickerState();
        }
    }
    public class _MaterialPickerState : State<MaterialPicker>
    {
        public readonly List<List<Color>> _colorTypes = new List<List<Color>> {
            new List<Color>{Colors.red, Colors.redAccent},
            new List<Color>{Colors.pink, Colors.pinkAccent},
            new List<Color>{Colors.purple, Colors.purpleAccent},
            new List<Color>{Colors.deepPurple, Colors.deepPurpleAccent},
            new List<Color>{Colors.indigo, Colors.indigoAccent},
            new List<Color>{Colors.blue, Colors.blueAccent},
            new List<Color>{Colors.lightBlue, Colors.lightBlueAccent},
            new List<Color>{Colors.cyan, Colors.cyanAccent},
            new List<Color>{Colors.teal, Colors.tealAccent},
            new List<Color>{Colors.green, Colors.greenAccent},
            new List<Color>{Colors.lightGreen, Colors.lightGreenAccent},
            new List<Color>{Colors.lime, Colors.limeAccent},
            new List<Color>{Colors.yellow, Colors.yellowAccent},
            new List<Color>{Colors.amber, Colors.amberAccent},
            new List<Color>{Colors.orange, Colors.orangeAccent},
            new List<Color>{Colors.deepOrange, Colors.deepOrangeAccent},
            new List<Color>{Colors.brown},
            new List<Color>{Colors.grey},
            new List<Color>{Colors.blueGrey},
            new List<Color>{Colors.black }
        };

        public List<Color> _currentColor = new List<Color> { Colors.red, Colors.redAccent };
        public Color _currentShading;

        public List<Color> _shadingTypes(List<Color> colors)
        {
            var result = new List<Color>();
            var shadeList1 = new List<int> { 50, 100, 200, 300, 400, 500, 600, 700, 800, 900 };
            var shadeList2 = new List<int> { 100, 200, 400, 700 };

            foreach (var colorType in colors)
            {
                if (colorType == Colors.grey)
                {
                    foreach (var shadeValue in shadeList1)
                    {
                        result.Add(Colors.grey[shadeValue]);
                    }
                }
                else if (colorType == Colors.black || colorType == Colors.white)
                {
                    result.Add(Colors.black);
                    result.Add(Colors.white);
                }
                else if (colorType is MaterialAccentColor)
                {
                    var _tmp = colorType as MaterialAccentColor;
                    foreach (var shadeValue in shadeList2)
                    {
                        result.Add(_tmp[shadeValue]);
                    }
                }
                else if (colorType is MaterialColor)
                {
                    var _tmp = colorType as MaterialColor;

                    foreach (var shadeValue in shadeList1)
                    {
                        result.Add(_tmp[shadeValue]);
                    }
                }
                else
                {
                    result.Add(new Color(0));
                }
            }
            return result;
        }

        public override void initState()
        {
            foreach (var _colors in _colorTypes)
            {
                foreach (var color in _shadingTypes(_colors))
                {
                    if (widget.pickerColor.value == color.value)
                    {
                        setState(
                            () =>
                            {
                                _currentColor = _colors;
                                _currentShading = color;
                            }
                        );
                        break;
                    }
                }
            }
            base.initState();
        }

        public override Widget build(BuildContext context)
        {
            Orientation _orientation = MediaQuery.of(context).orientation;
            bool _isPortrait = _orientation == Orientation.portrait;

            Widget _colorList()
            {
                var list = new List<Widget>();
                if (_isPortrait)
                    list.Add(new Padding(padding: EdgeInsets.only(top: 7f)));
                else
                    list.Add(new Padding(padding: EdgeInsets.only(left: 7f)));
                foreach (var _colors in _colorTypes)
                {
                    Color _colorType = _colors[0];
                    list.Add(
                        new GestureDetector(
                            onTap: () =>
                            {
                                setState(() =>
                                {
                                    _currentColor = _colors;
                                });
                            },
                            child: new Container(
                                color: new Color(0),
                                padding: (_isPortrait ? EdgeInsets.fromLTRB(0f, 7f, 0f, 7f) : EdgeInsets.fromLTRB(7f, 0f, 7f, 0f)),
                                child: new Align(
                                    child: new AnimatedContainer(
                                        duration: new TimeSpan(0, 0, 0, 300),
                                        width: 25f,
                                        height: 25f,
                                        decoration: new BoxDecoration(
                                            color: _colorType,
                                            borderRadius: BorderRadius.circular(60f),
                                            //boxShadow: new List<BoxShadow>{
                                            //    (_currentColor != _colors ? null :
                                            //        (_colorType == Theme.of(context).cardColor
                                            //            ?  new BoxShadow(color: Colors.grey[300], blurRadius: 5f)
                                            //            : new BoxShadow(color: _colorType, blurRadius: 5f)
                                            //        )
                                            //    )
                                            //},
                                            border: (_colorType == Theme.of(context).cardColor
                                            ? Border.all(color: Colors.grey[300], width: 1f)
                                            : null
                                            )
                                        )//BoxDecoration
                                    )//AnimatedContainer
                                )//Align
                            )//Container
                        )//GestureDetector
                    );
                }
                if (_isPortrait)
                    list.Add(new Padding(padding: EdgeInsets.only(top: 5f)));
                else
                    list.Add(new Padding(padding: EdgeInsets.only(left: 5f)));


                return new Container(
                    width: (_isPortrait ? (float?)60f : null),
                    height: (_isPortrait ? null : (float?)60f),
                    decoration: new BoxDecoration(
                        border: _isPortrait
                        ? new Border(right: new BorderSide(color: Colors.grey[300], width: 1f))
                        : new Border(top: new BorderSide(color: Colors.grey[300], width: 1f))
                    ),//BoxDecoration
                    child: new ListView(
                        scrollDirection: _isPortrait ? Axis.vertical : Axis.horizontal,
                        children: list
                    )//ListView
                );//container

            }//function _colorList

            Widget _shadingList()
            {
                var list = new List<Widget>();
                if (_isPortrait)
                    list.Add(new Padding(padding: EdgeInsets.only(top: 15f)));
                else
                    list.Add(new Padding(padding: EdgeInsets.only(left: 15f)));

                foreach (Color _color in _shadingTypes(_currentColor))
                {
                    list.Add(
                        new GestureDetector(
                            onTap: () =>
                            {
                                setState(() =>
                                {
                                    _currentShading = _color;
                                });
                                widget.onColorChanged(_currentShading);
                            },
                            child: new Container(
                                color: new Color(0),
                                padding: (_isPortrait
                                    ? EdgeInsets.fromLTRB(0f, 7f, 0f, 7f)
                                    : EdgeInsets.fromLTRB(7f, 0f, 7f, 0)
                                ),
                                child: new Align(
                                    child: new AnimatedContainer(
                                        duration: new TimeSpan(0, 0, 0, 300),
                                        width: _isPortrait ? 250f : 50f,
                                        height: _isPortrait ? 50f : 220f,
                                        decoration: new BoxDecoration(
                                            color: _color,
                                            //boxShadow: new List<BoxShadow>{
                                            //    (_currentShading  != _color ? null :
                                            //        (_color == Theme.of(context).cardColor
                                            //            ?  new BoxShadow(color: Colors.grey[300], blurRadius: 5f)
                                            //            : new BoxShadow(color: _currentShading, blurRadius: 5f)
                                            //        )
                                            //    )
                                            //},//List<BoxShadow>
                                            border: (_color == Theme.of(context).cardColor
                                                ? Border.all(color: Colors.grey[300], width: 1f)
                                                : Border.all(color: Colors.grey[300], width: 1f)
                                            )
                                        ),//BoxDecoration
                                        child: (_isPortrait && widget.enableLabel
                                            ? new Container(
                                                color: _color,
                                                child:
                                                  new Align(
                                                    alignment: Alignment.centerRight,
                                                    child: new Text(
                                                        "#" + _color.ToString().Replace("Color(0xFF", "").Replace(")", ""),
                                                        style: new TextStyle(
                                                            color: Utils.useWhiteForeground(_color) ? Colors.white : Colors.black,
                                                            fontWeight: FontWeight.w100
                                                        )//TextStyle
                                                    )//Text
                                                ) //Align
                                            ) 
                                            : new Container(color: _color) as Widget
                                        )//child
                                    )//AnimatedContainer
                                )//Align
                            )//Container
                        )//GestureDetector
                    );
                }//end foreach


                return new ListView(
                    scrollDirection: _isPortrait ? Axis.vertical : Axis.horizontal,
                    children: list
                );//ListView
            }//_shadingList

            switch (_orientation)
            {
                case Orientation.portrait:
                    return new SizedBox(
                        height: 500f,
                        width: 300f,
                        child: new Row(
                            children: new List<Widget>
                            {
                                _colorList(),
                                new Expanded(
                                    child: new Padding(
                                        padding: EdgeInsets.symmetric(horizontal:12f),
                                        child: _shadingList()
                                    )
                                )
                            }
                        )
                    );
                case Orientation.landscape:
                    return new SizedBox(
                        width: 500f,
                        height: 300f,
                        child: new Column(
                            children: new List<Widget>
                            {
                                new Expanded(
                                    child: new Padding(
                                        padding: EdgeInsets.symmetric(vertical: 12f),
                                        child: _shadingList()
                                    )//Padding
                                ),//Expanded
                                _colorList()
                            }
                        )//Column
                    );//SizedBox
                default:
                    return new Container();
            }
        }//build
    }
}