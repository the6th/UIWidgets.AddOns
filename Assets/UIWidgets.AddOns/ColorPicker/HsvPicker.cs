using System;
using System.Collections.Generic;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.gestures;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

//https://github.com/mchome/flutter_colorpicker/blob/master/lib/src/hsv_picker.dart

namespace UIWidgets.AddOns
{
    public enum PaletteType { hsv, hsl, rgb };

    public enum TrackType
    {
        hue,
        saturation,
        saturationForHSL,
        value,
        lightness,
        red,
        green,
        blue,
        alpha,
    }

    public enum ColorModel { hex, rgb, hsv, hsl }

    public class HSVColorPainter : AbstractCustomPainter
    {
        public HSVColor hsvColor;
        public Color pointerColor;

        public HSVColorPainter(HSVColor hsvColor, Color pointerColor = null)
        {
            this.hsvColor = hsvColor;
            this.pointerColor = pointerColor;
        }
        public override void paint(Canvas canvas, Size size)
        {
            Rect rect = Offset.zero & size;
            var gradientV = new LinearGradient(
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
                colors: new List<Color> { Colors.white, Colors.black }
            );
            var gradientH = new LinearGradient(
                colors: new List<Color> {
                    Colors.white,
                    HSVColor.fromAHSV(1f,hsvColor.hue,1f,1f).toColor()
                }
            );

            var paint = new Paint() { strokeWidth = 1.5f, style = PaintingStyle.stroke };
            if (pointerColor != null)
                paint.color = pointerColor;
            else if (Utils.useWhiteForeground(hsvColor.toColor()))
                paint.color = Colors.white;
            else
                paint.color = Colors.black;


            canvas.drawRect(rect, new Paint() { shader = gradientV.createShader(rect) });
            canvas.drawRect(rect, new Paint() { blendMode = BlendMode.multiply, shader = gradientH.createShader(rect) });
            canvas.drawCircle(
                new Offset(size.width * hsvColor.saturation, size.height * (1 - hsvColor.value)),
                size.height * 0.04f,
                paint
            );
        }

        public override bool shouldRepaint(CustomPainter oldDelegate)
        {
            return false;
        }
    }

    public class HSLColorPainter : AbstractCustomPainter
    {
        public HSLColor hslColor;
        public Color pointerColor;

        public HSLColorPainter(HSLColor hslColor, Color pointerColor = null)
        {
            this.hslColor = hslColor;
            this.pointerColor = pointerColor;
        }

        public override void paint(Canvas canvas, Size size)
        {
            Rect rect = Offset.zero & size;

            var gradientH = new LinearGradient(
                colors: new List<Color> {
                    new Color(0xff808080),
                    HSLColor.fromAHSL(1f,hslColor.hue,1f,0.5f).toColor()
                }
            );
            var gradientV = new LinearGradient(
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
                stops: new List<float> { 0.0f, 0.5f, 0.5f, 1f },
                colors: new List<Color> {
                                Colors.white,
                                new Color(0x00ffffff),
                                Colors.transparent,
                                Colors.black
                }
            );
            var paint = new Paint() { strokeWidth = 1.5f, style = PaintingStyle.stroke };
            if (pointerColor != null)
                paint.color = pointerColor;
            else if (Utils.useWhiteForeground(hslColor.toColor()))
                paint.color = Colors.white;
            else
                paint.color = Colors.black;


            canvas.drawRect(rect, new Paint() { shader = gradientV.createShader(rect) });
            canvas.drawRect(rect, new Paint() { shader = gradientH.createShader(rect) });
            canvas.drawCircle(
                new Offset(size.width * hslColor.saturation, size.height * (1 - hslColor.lightness)),
                size.height * 0.04f,
                paint
            );
        }

        public override bool shouldRepaint(CustomPainter oldDelegate) => false;
    }

    public class _SliderLayout : MultiChildLayoutDelegate
    {
        public static readonly string track = "track";
        public static readonly string thumb = "thumb";
        public static readonly string gestureContainer = "gesturecontainer";
        public override void performLayout(Size size)
        {
            layoutChild(
                track,
                BoxConstraints.tightFor(
                    width: size.width - 30f,
                    height: size.height / 5f
                )
            );
            positionChild(track, new Offset(15.0f, size.height * 0.4f));
            layoutChild(
                thumb,
                BoxConstraints.tightFor(width: 5f, height: size.height / 4)
            );
            positionChild(thumb, new Offset(0f, size.height * 0.4f));
            layoutChild(
                gestureContainer,
                BoxConstraints.tightFor(width: size.width, height: size.height)
            );
            positionChild(gestureContainer, Offset.zero);
        }

        public override bool shouldRelayout(MultiChildLayoutDelegate oldDelegate)
        {
            return false;
        }
    }

    public class TrackPainter : AbstractCustomPainter
    {
        TrackType trackType;
        HSVColor hsvColor;
        public TrackPainter(TrackType trackType, HSVColor hsvColor)
        {
            this.trackType = trackType;
            this.hsvColor = hsvColor;
        }

        public override void paint(Canvas canvas, Size size)
        {
            Rect rect = Offset.zero & size;
            if (trackType == TrackType.alpha)
            {
                Size chessSize = new Size(size.height / 2, size.height / 2);
                Paint chessPaintB = new Paint() { color = new Color(0xffcccccc) };
                Paint chessPaintW = new Paint() { color = Colors.white };

                int height = (size.height / chessSize.height).round();
                int width = (size.width / chessSize.width).round();
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        canvas.drawRect(
                            new Offset(chessSize.width * x, chessSize.width * y) & chessSize,
                            (x + y) % 2 != 0 ? chessPaintW : chessPaintB
                        );
                    }
                }
            }

            List<Color> colors;
            LinearGradient gradient;

            switch (trackType)
            {
                case TrackType.hue:
                    colors = new List<Color>
                        {
                            HSVColor.fromAHSV(1f,0f,1f,1f).toColor(),
                            HSVColor.fromAHSV(1f,60f,1f,1f).toColor(),
                            HSVColor.fromAHSV(1f,120f,1f,1f).toColor(),
                            HSVColor.fromAHSV(1f,180f,1f,1f).toColor(),
                            HSVColor.fromAHSV(1f,240f,1f,1f).toColor(),
                            HSVColor.fromAHSV(1f,240f,1f,1f).toColor(),
                            HSVColor.fromAHSV(1f,300f,1f,1f).toColor(),
                            HSVColor.fromAHSV(1f,360f,1f,1f).toColor()
                        };
                    gradient = new LinearGradient(colors: colors);
                    canvas.drawRect(rect, new Paint() { shader = gradient.createShader(rect) });
                    break;

                case TrackType.saturation:
                    colors = new List<Color> {
                            HSVColor.fromAHSV(1f,hsvColor.hue,0f,1f).toColor(),
                            HSVColor.fromAHSV(1f,hsvColor.hue,1f,1f).toColor()
                        };
                    gradient = new LinearGradient(colors: colors);
                    canvas.drawRect(rect, new Paint() { shader = gradient.createShader(rect) });
                    break;

                case TrackType.saturationForHSL:
                    colors = new List<Color> {
                            HSVColor.fromAHSV(1f,hsvColor.hue,0f,.5f).toColor(),
                            HSVColor.fromAHSV(1f,hsvColor.hue,1f,.5f).toColor()
                        };
                    gradient = new LinearGradient(colors: colors);
                    canvas.drawRect(rect, new Paint() { shader = gradient.createShader(rect) });
                    break;
                case TrackType.value:
                    colors = new List<Color> {
                            HSVColor.fromAHSV(1f,hsvColor.hue,1f,0f).toColor(),
                            HSVColor.fromAHSV(1f,hsvColor.hue,1f,1f).toColor()
                        };
                    gradient = new LinearGradient(colors: colors);
                    canvas.drawRect(rect, new Paint() { shader = gradient.createShader(rect) });
                    break;
                case TrackType.lightness:
                    colors = new List<Color> {
                            HSVColor.fromAHSV(1f,hsvColor.hue,1f,0f).toColor(),
                            HSVColor.fromAHSV(1f,hsvColor.hue,1f,0.5f).toColor(),
                            HSVColor.fromAHSV(1f,hsvColor.hue,1f,1f).toColor()
                        };
                    gradient = new LinearGradient(colors: colors);
                    canvas.drawRect(rect, new Paint() { shader = gradient.createShader(rect) });
                    break;
                case TrackType.red:
                    colors = new List<Color> {
                            hsvColor.toColor().withRed(0).withOpacity(1f),
                            hsvColor.toColor().withRed(255).withOpacity(1f)

                        };
                    gradient = new LinearGradient(colors: colors);
                    canvas.drawRect(rect, new Paint() { shader = gradient.createShader(rect) });
                    break;
                case TrackType.green:
                    colors = new List<Color> {
                            hsvColor.toColor().withGreen(0).withOpacity(1f),
                            hsvColor.toColor().withGreen(255).withOpacity(1f)
                        };
                    gradient = new LinearGradient(colors: colors);
                    canvas.drawRect(rect, new Paint() { shader = gradient.createShader(rect) });
                    break;
                case TrackType.blue:
                    colors = new List<Color> {
                            hsvColor.toColor().withBlue(0).withOpacity(1f),
                            hsvColor.toColor().withBlue(255).withOpacity(1f)
                        };
                    gradient = new LinearGradient(colors: colors);
                    canvas.drawRect(rect, new Paint() { shader = gradient.createShader(rect) });
                    break;
                case TrackType.alpha:
                    colors = new List<Color> {
                            Colors.black.withOpacity(0f),
                            Colors.black.withOpacity(1f)
                        };
                    gradient = new LinearGradient(colors: colors);
                    canvas.drawRect(rect, new Paint() { shader = gradient.createShader(rect) });
                    break;
            }
        }

        public override bool shouldRepaint(CustomPainter oldDelegate)
        {
            return false;
        }
    }

    public class ThumbPainter : AbstractCustomPainter
    {
        Color thumbColor;
        bool fullThumbColor;

        public ThumbPainter(Color thumbColor, bool fullThumbColor = false)
        {
            this.thumbColor = thumbColor;
            this.fullThumbColor = fullThumbColor;
        }

        public override void paint(Canvas canvas, Size size)
        {
            var path = new Path();
            path.addOval(
                Rect.fromCircle(
                    center: new Offset(0.5f, 2f),
                    radius: size.width * 1.8f)
            );

            canvas.drawShadow(path, Colors.black, 3f, true);
            canvas.drawCircle(
                new Offset(0f, size.height * 0.4f),
                size.height,
                new Paint() { color = Colors.white, style = PaintingStyle.fill }
            );

            if (thumbColor != null)
            {
                canvas.drawCircle(
                    new Offset(0f, size.height * 0.4f),
                    size.height * (fullThumbColor ? 1f : 0.65f),
                    new Paint() { color = thumbColor, style = PaintingStyle.fill }
                );
            }
        }

        public override bool shouldRepaint(CustomPainter oldDelegate)
        {
            return false;
        }
    }
    public class IndicatorPainter : AbstractCustomPainter
    {
        Color color;
        public IndicatorPainter(Color color)
        {
            this.color = color;
        }

        public override void paint(Canvas canvas, Size size)
        {
            var chessSize = new Size(size.width / 10f, size.height / 10);
            var chessPaintB = new Paint() { color = new Color(0xFFCCCCCC) };
            var chessPaintW = new Paint() { color = Colors.white };

            int height = (size.height / chessSize.height).round();
            int width = (size.width / chessSize.width).round();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    canvas.drawRect(
                        new Offset(chessSize.width * x, chessSize.width * y) & chessSize,
                        (x + y) % 2 != 0 ? chessPaintW : chessPaintB
                    );
                }
            }

            canvas.drawCircle(
                new Offset(size.width / 2, size.height / 2),
                size.height / 2,
                new Paint() { color = color, style = PaintingStyle.fill }
            );
        }

        public override bool shouldRepaint(CustomPainter oldDelegate)
        {
            return false;
        }
    }

    public class CheckerPainter : AbstractCustomPainter
    {
        public CheckerPainter() { }

        public override void paint(Canvas canvas, Size size)
        {
            var chessSize = new Size(size.height / 6, size.height / 6);
            var chessPaintB = new Paint() { color = new Color(0xffcccccc) };
            var chessPaintW = new Paint() { color = Colors.white };

            int height = (size.height / chessSize.height).round();
            int width = (size.width / chessSize.width).round();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    canvas.drawRect(
                        new Offset(chessSize.width * x, chessSize.width * y) & chessSize,
                        (x + y) % 2 != 0 ? chessPaintW : chessPaintB
                    );
                }
            }
        }

        public override bool shouldRepaint(CustomPainter oldDelegate)
        {
            return false;
        }
    }

    public class ColorPickerLabel : StatefulWidget
    {
        public HSVColor hsvColor;
        public bool enableAlpha;
        public TextStyle textStyle;
        public bool editable;
        public ValueChanged<HSVColor> onColorChanged;

        public ColorPickerLabel(
            HSVColor hsvColor,
            TextStyle textStyle,
            bool enableAlpha = true,
            bool editable = false,
            ValueChanged<HSVColor> onColorChanged = null
            )
        {
            D.assert(enableAlpha != null);
            D.assert(editable != null);

            this.hsvColor = hsvColor;
            this.textStyle = textStyle;
            this.enableAlpha = enableAlpha;
            this.editable = editable;
            this.onColorChanged = onColorChanged;
        }

        public override State createState()
        {
            return new _ColorPickerLabelState();
        }
    }

    public class _ColorPickerLabelState : State<ColorPickerLabel>
    {
        Dictionary<ColorModel, List<string>> _colorTypes = new Dictionary<ColorModel, List<string>>() {

            {  ColorModel.hex, new List<string>(){"R","G","B","A" }},
            {  ColorModel.rgb, new List<string>(){"R","G","B","A" }},
            {  ColorModel.hsv, new List<string>(){"H","S","V","A" }},
            {  ColorModel.hsl, new List<string>(){"H","S","L","A" }}
        };

        ColorModel _colorType = ColorModel.hex;

        List<string> colorValue(HSVColor hsvColor, ColorModel colorModel)
        {
            Color color = hsvColor.toColor();
            if (colorModel == ColorModel.hex)
            {
                return new List<string>()
                {
                    color.red.ToString("X2"),
                    color.green.ToString("X2"),
                    color.blue.ToString("X2"),
                    $"{(color.opacity * 100).round()}%"
                };
            }
            else if (colorModel == ColorModel.rgb)
            {
                return new List<string>()
                {
                    color.red.ToString(),
                    color.green.ToString(),
                    color.blue.ToString(),
                    $"{(color.opacity * 100).round()}%"
                };
            }
            else if (colorModel == ColorModel.hsv)
            {
                return new List<string>()
                {
                    $"{hsvColor.hue.round()}°",
                    $"{(hsvColor.saturation * 100).round()}%",
                    $"{(hsvColor.value * 100).round()}%",
                    $"{(hsvColor.alpha * 100).round()}%",
                };
            }
            else if (colorModel == ColorModel.hsl)
            {
                HSLColor hslColor = Utils.hsvToHsl(hsvColor);
                return new List<string>()
                {
                    $"{hslColor.hue.round()}°",
                    $"{(hslColor.saturation * 100).round()}%",
                    $"{(hslColor.lightness * 100).round()}%",
                    $"{(hslColor.alpha * 100).round()}%",
                };
            }
            else
            {
                return new List<string>() { "??", "??", "??", "??" };
            }
        }

        List<Widget> colorValueLabels()
        {
            var list = new List<Widget>();
            foreach (string item in _colorTypes[_colorType])
            {
                if (widget.enableAlpha || item != "A")
                {
                    list.Add(
                        new Padding(
                            padding: EdgeInsets.symmetric(horizontal: 7f),
                            child: new IntrinsicHeight(
                                child: new Column(
                                    children: new List<Widget> {
                                        new Text(
                                            item,
                                            style: widget.textStyle ?? Theme.of(context).textTheme.body2.copyWith(fontWeight: FontWeight.bold,fontSize:16f)
                                        ),//Text
                                        new SizedBox(height:10f),
                                        new Expanded(
                                            child: new Text(
                                                colorValue(widget.hsvColor,_colorType)[_colorTypes[_colorType].IndexOf(item)],
                                                overflow: TextOverflow.ellipsis
                                            )//Text
                                        ),//Expanded
                                    }//List
                                )//Column
                            )//IntrinsicHeight
                        )//Padding
                    );//Add
                }//endif
            }
            return list;
        }

        public override Widget build(BuildContext context)
        {
            var list = new List<Unity.UIWidgets.material.DropdownMenuItem<string>>();
            foreach (ColorModel type in _colorTypes.Keys)
            {
                list.Add(
                    new Unity.UIWidgets.material.DropdownMenuItem<string>(
                        value: type.ToString(),
                        child: new Text(type.ToString().Split('.').last<string>().ToUpper())
                    )
                );
            }

            var list2 = new List<Widget> {
                    new DropdownButton<string>(
                        value: _colorType.ToString(),
                        onChanged: (string type) =>{
                            Enum.TryParse<ColorModel>(type,out ColorModel _type);
                            setState(()=>{
                                _colorType = _type;
                            });
                        },
                        items:list

                    ),
                    new SizedBox(width:10f)
                };
            list2.AddRange(colorValueLabels());

            //return null;
            return new Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: list2
            );//Row;



        }
    }

    public class ColorPickerSlider : StatelessWidget
    {
        public TrackType trackType;
        public HSVColor hsvColor;
        public ValueChanged<HSVColor> onColorChanged;
        public bool displayThumbColor;
        public bool fullThumbColor;

        public ColorPickerSlider(
            TrackType trackType,
            HSVColor hsvColor,
            ValueChanged<HSVColor> onColorChanged = null,
            bool displayThumbColor = false,
            bool fullThumbColor = false
        )
        {
            this.trackType = trackType;
            this.hsvColor = hsvColor;
            this.onColorChanged = onColorChanged;
            this.displayThumbColor = displayThumbColor;
            this.fullThumbColor = fullThumbColor;
        }

        void slideEvent(RenderBox getBox, BoxConstraints box, Offset globalPosition)
        {
            float localDx = getBox.globalToLocal(globalPosition).dx - 15f;
            float progress = localDx.clamp(0f, box.maxWidth - 30f) / (box.maxWidth - 30f);

            switch (trackType)
            {
                case TrackType.hue:
                    // 360 is the same as zero
                    // if set to 360, sliding to end goes to zero
                    onColorChanged(hsvColor.withHue(progress * 359));

                    break;
                case TrackType.saturation:
                    onColorChanged(hsvColor.withSaturation(progress));
                    break;
                case TrackType.saturationForHSL:
                    onColorChanged(Utils.hslToHsv(Utils.hsvToHsl(hsvColor).withSaturation(progress)));
                    break;
                case TrackType.value:
                    onColorChanged(hsvColor.withValue(progress));
                    break;
                case TrackType.lightness:
                    //TODO: 未実装
                    onColorChanged(Utils.hslToHsv(Utils.hsvToHsl(hsvColor).withLightness(progress)));
                    break;
                //case TrackType.red:
                //    onColorChanged(HSVColor.fromColor(
                //        hsvColor.toColor().withRed((progress * 0xff).round())));
                //    break;
                //case TrackType.green:
                //    onColorChanged(HSVColor.fromColor(
                //        hsvColor.toColor().withGreen((progress * 0xff).round())));
                //    break;
                //case TrackType.blue:
                //    onColorChanged(HSVColor.fromColor(
                //        hsvColor.toColor().withBlue((progress * 0xff).round())));
                //    break;
                case TrackType.alpha:
                    onColorChanged(hsvColor.withAlpha(
                        localDx.clamp(0f, box.maxWidth - 30f) / (box.maxWidth - 30f)));
                    break;
            }
        }
        public override Widget build(BuildContext context)
        {
            float thumbOffset = 15f;
            Color _thumbColor = Colors.black;
            return new LayoutBuilder(builder: (BuildContext _, BoxConstraints box) =>
            {
                switch (trackType)
                {
                    case TrackType.hue:
                        thumbOffset += (box.maxWidth - 30f) * hsvColor.hue / 360f;
                        _thumbColor = HSVColor.fromAHSV(1f, hsvColor.hue, 1f, 1f).toColor();
                        break;
                    case TrackType.saturation:
                        thumbOffset += (box.maxWidth - 30f) * hsvColor.saturation;
                        _thumbColor =
                            HSVColor.fromAHSV(1f, hsvColor.hue, hsvColor.saturation, 1f)
                                .toColor();
                        break;
                    case TrackType.saturationForHSL:
                        thumbOffset += (box.maxWidth - 30f) * Utils.hsvToHsl(hsvColor).saturation;
                        _thumbColor = HSLColor.fromAHSL(
                                1f, hsvColor.hue, Utils.hsvToHsl(hsvColor).saturation, 0.5f)
                            .toColor();
                        break;
                    case TrackType.value:
                        thumbOffset += (box.maxWidth - 30f) * hsvColor.value;
                        _thumbColor = HSVColor.fromAHSV(1f, hsvColor.hue, 1f, hsvColor.value)
                            .toColor();
                        break;
                    case TrackType.lightness:
                        thumbOffset += (box.maxWidth - 30f) * Utils.hsvToHsl(hsvColor).lightness;
                        _thumbColor = HSLColor.fromAHSL(
                                1f, hsvColor.hue, 1f, Utils.hsvToHsl(hsvColor).lightness)
                            .toColor();
                        break;
                    case TrackType.red:
                        thumbOffset += (box.maxWidth - 30f) * hsvColor.toColor().red / 0xff;
                        _thumbColor = hsvColor.toColor().withOpacity(1f);
                        break;
                    case TrackType.green:
                        thumbOffset +=
                            (box.maxWidth - 30f) * hsvColor.toColor().green / 0xff;
                        _thumbColor = hsvColor.toColor().withOpacity(1f);
                        break;
                    case TrackType.blue:
                        thumbOffset += (box.maxWidth - 30f) * hsvColor.toColor().blue / 0xff;
                        _thumbColor = hsvColor.toColor().withOpacity(1f);
                        break;
                    case TrackType.alpha:
                        thumbOffset += (box.maxWidth - 30f) * hsvColor.toColor().opacity;
                        _thumbColor = Colors.black.withOpacity(hsvColor.alpha);
                        break;
                }//switch 

                return new CustomMultiChildLayout(
                    layoutDelegate: new _SliderLayout(),
                    children: new List<Widget> {
                        new LayoutId(
                            id: _SliderLayout.track,
                            child: new ClipRRect(
                               borderRadius: BorderRadius.all(Radius.circular(50f)),
                               child: new CustomPaint(
                                   painter: new TrackPainter(this.trackType,this.hsvColor)
                                )//CustomPaint
                            )
                        ),//LayoutId
                        new LayoutId(
                            id: _SliderLayout.thumb,
                            child: Transform.translate(
                                offset: new Offset(thumbOffset,0f),
                                child: new CustomPaint(
                                    painter: new ThumbPainter(
                                        thumbColor: !displayThumbColor ? null : _thumbColor,
                                        fullThumbColor: fullThumbColor
                                    )//ThumbPainter
                                )//CustomPaint
                            )//Transform
                        ),//LayoutId
                        new LayoutId(
                            id:_SliderLayout.gestureContainer,
                            child: new LayoutBuilder(
                                builder:(BuildContext __, BoxConstraints _box)=>{
                                    RenderBox getBox = context.findRenderObject() as RenderBox;
                                    return new GestureDetector(
                                        onPanDown:(DragDownDetails details)=> { slideEvent(getBox,box,details.globalPosition); },
                                        onPanUpdate:(DragUpdateDetails details )=>{  slideEvent(getBox,box,details.globalPosition); }
                                     );
                                }
                            )//LayoutBuilder
                        )//LayoutId
                    }//list Widget
                );//CustomMultiChildLayout
            });//LayoutBuilder
        }
    }

    public class ColorIndicator : StatelessWidget
    {
        public HSVColor hsvColor;
        public float width;
        public float height;

        public ColorIndicator(HSVColor hsvColor, float width = 50f, float height = 50f)
        {
            this.hsvColor = hsvColor;
            this.width = width;
            this.height = height;
        }

        public override Widget build(BuildContext context)
        {
            return new Container(
                width: width,
                height: height,
                decoration: new BoxDecoration(
                    borderRadius: BorderRadius.all(Radius.circular(1000f)),
                    border: Border.all(color: new Color(0xffdddddd))
                ),//BoxDecoration
                child: new ClipRRect(
                    borderRadius: BorderRadius.all(Radius.circular(1000)),
                    child: new CustomPaint(painter: new IndicatorPainter(hsvColor.toColor()))
                )//ClipRRect
            );//Container
        }
    }


    public class ColorPickerArea : StatelessWidget
    {
        public HSVColor hsvColor;
        public ValueChanged<HSVColor> onColorChanged;
        public PaletteType paletteType;


        public ColorPickerArea(HSVColor hsvColor, ValueChanged<HSVColor> onColorChanged, PaletteType paletteType)
        {
            this.hsvColor = hsvColor;
            this.onColorChanged = onColorChanged;
            this.paletteType = paletteType;
        }

        void _handleColorChange(float horizontal, float vertical)
        {
            switch (paletteType)
            {
                case PaletteType.hsv:
                    onColorChanged(hsvColor.withSaturation(horizontal).withValue(vertical));
                    break;
                case PaletteType.hsl:
                    onColorChanged(Utils.hslToHsv(Utils.hsvToHsl(hsvColor)
                        .withSaturation(horizontal)
                        .withLightness(vertical)));
                    break;
                default:
                    break;
            }
        }

        void _handleGesture(Offset position, BuildContext context, float height, float width)
        {
            RenderBox getBox = context.findRenderObject() as RenderBox;
            Offset localOffset = getBox.globalToLocal(position);
            float horizontal = localOffset.dx.clamp(0f, width) / width;
            float vertical = 1 - localOffset.dy.clamp(0f, height) / height;
            _handleColorChange(horizontal, vertical);
        }

        public override Widget build(BuildContext context)
        {
            return new LayoutBuilder(
                builder: (BuildContext _, BoxConstraints constraints) =>
                {
                    float width = constraints.maxWidth;
                    float height = constraints.maxHeight;

                    Dictionary<Type, GestureRecognizerFactory> gestures = new Dictionary<Type, GestureRecognizerFactory>();
                    gestures.Add(
                        typeof(AlwaysWinPanGestureRecognizer),
                        new GestureRecognizerFactoryWithHandlers<AlwaysWinPanGestureRecognizer>(
                            () => new AlwaysWinPanGestureRecognizer(),
                            instance =>
                            {
                                instance.onDown = (details) => _handleGesture(
                                    details.globalPosition,
                                    context,
                                    height,
                                    width
                                );
                                instance.onUpdate = (details) => _handleGesture(
                                    details.globalPosition,
                                    context,
                                    height,
                                    width
                                );
                            }
                        )//GestureRecognizerFactoryWithHandlers
                    );//Add

                    return new RawGestureDetector(
                        gestures: gestures,
                        child: new Builder(
                            builder: (BuildContext __) =>
                            {
                                switch (paletteType)
                                {
                                    case PaletteType.hsv:
                                        return new CustomPaint(painter: new HSVColorPainter(hsvColor));

                                    case PaletteType.hsl:
                                        return new CustomPaint(painter: new HSLColorPainter(Utils.hsvToHsl(hsvColor)));
                                    default:
                                        return new CustomPaint();
                                }
                            }
                        )//Builder
                    ); //RawGestureDetector
                }
            );
        }
    }

    public class AlwaysWinPanGestureRecognizer : PanGestureRecognizer
    {
        public override void addAllowedPointer(PointerDownEvent evt)
        {
            base.addAllowedPointer(evt);
            resolve(GestureDisposition.accepted);
        }
        public override string debugDescription => "alwaysWin";

    }

}