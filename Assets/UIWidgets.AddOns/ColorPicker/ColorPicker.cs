﻿using System.Collections.Generic;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using Color = Unity.UIWidgets.ui.Color;

namespace UIWidgets.AddOns
{
    public class ColorPicker : StatefulWidget
    {
        public Color pickerColor;
        public ValueChanged<Color> onColorChanged;
        public PaletteType paletteType;
        public bool enableAlpha;
        public bool showLabel;
        public TextStyle labelTextStyle;
        public bool displayThumbColor;
        public bool portraitOnly;
        public float colorPickerWidth;
        public float pickerAreaHeightPercent;
        public BorderRadius pickerAreaBorderRadius;

        public ColorPicker(
            Color pickerColor,
            ValueChanged<Color> onColorChanged,
            TextStyle labelTextStyle = null,
            PaletteType paletteType = PaletteType.hsv,
            BorderRadius pickerAreaBorderRadius = null,
            bool enableAlpha = true,
            bool displayThumbColor = false,
            bool portraitOnly = false,
            bool showLabel = true,
            float colorPickerWidth = 300f,
            float pickerAreaHeightPercent = 1f
            )
        {
            this.pickerColor = pickerColor;
            this.onColorChanged = onColorChanged;
            this.labelTextStyle = labelTextStyle;
            this.paletteType = paletteType;

            if (pickerAreaBorderRadius == null)
                this.pickerAreaBorderRadius = BorderRadius.all(Radius.zero);
            else
                this.pickerAreaBorderRadius = pickerAreaBorderRadius;

            this.enableAlpha = enableAlpha;
            this.displayThumbColor = displayThumbColor;
            this.portraitOnly = portraitOnly;
            this.showLabel = showLabel;
            this.colorPickerWidth = colorPickerWidth;
            this.pickerAreaHeightPercent = pickerAreaHeightPercent;
        }

        public override State createState()
        {
            return new _ColorPickerState();
        }

        public class _ColorPickerState : State<ColorPicker>
        {
            HSVColor currentHsvColor = HSVColor.fromAHSV(0f, 0f, 0f, 0f);

            public override void initState()
            {
                base.initState();
                currentHsvColor = HSVColorExtention.fromColor(widget.pickerColor);
            }

            public override void didChangeDependencies()
            {
                base.didChangeDependencies();
                currentHsvColor = HSVColorExtention.fromColor(widget.pickerColor);
            }
            Widget colorPickerSlider(TrackType trackType)
            {
                return new ColorPickerSlider(
                    trackType,
                    currentHsvColor,
                    (HSVColor color) =>
                    {
                        setState(() =>
                        {
                            currentHsvColor = color;
                        });
                        widget.onColorChanged(currentHsvColor.toColor());
                    },
                    displayThumbColor: widget.displayThumbColor
                );
            }

            Widget colorPickerArea()
            {
                return new ClipRRect(
                    borderRadius: widget.pickerAreaBorderRadius,
                    child: new ColorPickerArea(
                        currentHsvColor,
                        (HSVColor color) =>
                        {
                            setState(() => { currentHsvColor = color; });
                            widget.onColorChanged(currentHsvColor.toColor());
                        },
                        widget.paletteType
                    )
                );
            }
            public override Widget build(BuildContext context)
            {
                if (MediaQuery.of(context).orientation == Orientation.portrait || widget.portraitOnly)
                {
                    return new Column(
                        children: new List<Widget>
                        {
                            new SizedBox(
                                width: widget.colorPickerWidth,
                                height: widget.colorPickerWidth * widget.pickerAreaHeightPercent,
                                child: colorPickerArea()
                            ),//SizedBox
                            new Padding(
                                padding: EdgeInsets.fromLTRB(15f,5f,10f,5f),
                                child: new Row(
                                    mainAxisAlignment: MainAxisAlignment.center,
                                    children: new List<Widget>
                                    {
                                        new ColorIndicator(currentHsvColor),
                                        new Expanded(
                                            child: new Column(
                                                children: new List<Widget>{
                                                    new SizedBox(
                                                        height:40f,
                                                        width: widget.colorPickerWidth - 75,
                                                        child: colorPickerSlider(TrackType.hue)
                                                    ),
                                                    new Visibility(
                                                        visible: widget.enableAlpha,
                                                        child: new SizedBox(
                                                            height: 40f,
                                                            width: widget.colorPickerWidth - 75,
                                                            child: colorPickerSlider(TrackType.alpha)
                                                        )//SizedBox
                                                    )//Visibility
                                                }
                                            )//Column
                                        )//Expanded
                                    }
                                )//Row
                            ),//Padding
                            new Visibility(
                                visible: widget.showLabel,
                                child: new ColorPickerLabel(
                                    currentHsvColor,
                                    enableAlpha: widget.enableAlpha,
                                    textStyle: widget.labelTextStyle
                                )
                            ),
                            new SizedBox(height:20f)
                        }//List
                    );//Column
                }
                else
                {
                    return new Row(
                        children: new List<Widget>
                        {
                            new Expanded(
                                child: new SizedBox(
                                    width: 300f,
                                    height: 200f,
                                    child: colorPickerArea()
                                )//SizedBox
                            ),//Expanded
                            new Column(
                                children: new List<Widget>
                                {
                                    new Row(
                                        children: new List<Widget>{
                                            new SizedBox(width:20f),
                                            new ColorIndicator(currentHsvColor),
                                            new Column(
                                                children: new List<Widget>{
                                                    new SizedBox(
                                                        height:40f,
                                                        width:260f,
                                                        child: colorPickerSlider(TrackType.hue)
                                                    ),
                                                    new Visibility(
                                                        visible: widget.enableAlpha,
                                                        child: new SizedBox(
                                                            height: 40f,
                                                            width: 260f,
                                                            child: colorPickerSlider(TrackType.alpha)
                                                        )
                                                    )
                                                }
                                            ),//Column
                                            new SizedBox(width:10f)
                                        }
                                    ),//Row
                                    new SizedBox(width:10f),
                                    new Visibility(
                                        visible: widget.showLabel,
                                        child: new ColorPickerLabel(
                                            currentHsvColor,
                                            enableAlpha: widget.enableAlpha,
                                            textStyle: widget.labelTextStyle
                                        )//ColorPickerLabel
                                    )//Visibility
                                }
                            )//Column
                        }
                    );//Row
                }
            }
        }
    }

    public class SlidePicker : StatefulWidget
    {
        public Color pickerColor;
        public ValueChanged<Color> onColorChanged;
        public PaletteType paletteType;
        public bool enableAlpha;
        public Size sliderSize;
        public bool showSliderText;
        public TextStyle sliderTextStyle;
        public bool showLabel;
        public TextStyle labelTextStyle;
        public bool showIndicator;
        public Size indicatorSize;
        //public AlignmentGeometry indicatorAlignmentBegin;
        //public AlignmentGeometry indicatorAlignmentEnd;
        public bool displayThumbColor;
        public BorderRadius indicatorBorderRadius;

        public SlidePicker(
            Color pickerColor,
            TextStyle sliderTextStyle = null,
            TextStyle labelTextStyle = null,
            ValueChanged<Color> onColorChanged = null,
            PaletteType paletteType = PaletteType.hsv,
            Size sliderSize = null,
            bool showSliderText = true,
            bool enableAlpha = true,
            bool showLabel = true,
            bool showIndicator = true,
            Size indicatorSize = null,
            //Alignment indicatorAlignmentBegin = null,
            //Alignment indicatorAlignmentEnd = null,
            bool displayThumbColor = false,
            BorderRadius indicatorBorderRadius = null
            )
        {
            D.assert(pickerColor != null);
            D.assert(onColorChanged != null);

            this.pickerColor = pickerColor;
            this.sliderTextStyle = sliderTextStyle;
            this.labelTextStyle = labelTextStyle;

            this.onColorChanged = onColorChanged;
            this.paletteType = paletteType;

            if (sliderSize == null)
                this.sliderSize = new Size(260, 40);
            else
                this.sliderSize = sliderSize;

            this.showSliderText = showSliderText;
            this.enableAlpha = enableAlpha;
            this.showLabel = showLabel;
            this.showIndicator = showIndicator;

            if (indicatorSize == null)
                this.indicatorSize = new Size(280, 50);
            else
                this.indicatorSize = indicatorSize;

            //if (indicatorAlignmentBegin == null)
            //    this.indicatorAlignmentBegin = new Alignment(1f, 3f);
            //else
            //    this.indicatorAlignmentBegin = indicatorAlignmentBegin;

            //if (indicatorAlignmentEnd == null)
            //    this.indicatorAlignmentEnd = new Alignment(1f, 3f);
            //else
            //    this.indicatorAlignmentEnd = indicatorAlignmentEnd;

            this.displayThumbColor = displayThumbColor;

            if (indicatorBorderRadius == null)
                this.indicatorBorderRadius = BorderRadius.all(Radius.zero);
            else
                this.indicatorBorderRadius = indicatorBorderRadius;

        }

        public override State createState()
        {
            return new _SlidePickerState();
        }
    }

    public class _SlidePickerState : State<SlidePicker>
    {
        HSVColor currentHsvColor = HSVColor.fromAHSV(0f, 0f, 0f, 0f);

        public override void initState()
        {
            base.initState();
            currentHsvColor = HSVColorExtention.fromColor(widget.pickerColor);
        }

        public override void didUpdateWidget(StatefulWidget oldWidget)
        {
            base.didUpdateWidget(oldWidget);
            currentHsvColor = HSVColorExtention.fromColor(widget.pickerColor);

        }

        public Widget colorPickerSlider(TrackType trackType)
        {
            return new ColorPickerSlider(
                trackType,
                currentHsvColor,
                (HSVColor color) =>
                {
                    //UnityEngine.Debug.Log(color.toColor());
                    setState(() => { currentHsvColor = color; });
                    widget.onColorChanged(currentHsvColor.toColor());
                },
                displayThumbColor: widget.displayThumbColor,
                fullThumbColor: true
            );
        }

        public Widget indicator()
        {
            return new ClipRRect(
                borderRadius: widget.indicatorBorderRadius,
                clipBehavior: Clip.antiAliasWithSaveLayer,
                child: new Container(
                    width: widget.indicatorSize.width,
                    height: widget.indicatorSize.height,
                    margin: EdgeInsets.only(bottom: 15f),
                    forgroundDecoration: new BoxDecoration(
                        gradient: new LinearGradient(
                            colors: new List<Color>
                            {
                                widget.pickerColor,
                                widget.pickerColor,
                                currentHsvColor.toColor(),
                                currentHsvColor.toColor()
                            },
                            //begin: widget.indicatorAlignmentBegin,
                            //end: widget.indicatorAlignmentEnd,
                            begin: FractionalOffset.topLeft,
                            end: FractionalOffset.bottomRight,
                            stops: new List<float> { 0f, 0.5f, 0.5f, 1f }
                        )//LinearGradient
                    ),//BoxDecoration
                    child: new CustomPaint(painter: new CheckerPainter())
                )//Container
            );//ClipRRect
        }

        public override Widget build(BuildContext context)
        {
            List<TrackType> types = new List<TrackType>();

            switch (widget.paletteType)
            {
                case PaletteType.hsv:
                    types = new List<TrackType> { TrackType.hue, TrackType.saturation, TrackType.value, TrackType.alpha };
                    break;
                case PaletteType.hsl:
                    types = new List<TrackType> { TrackType.hue, TrackType.saturationForHSL, TrackType.lightness, TrackType.alpha };
                    break;
                case PaletteType.rgb:
                    types = new List<TrackType> { TrackType.red, TrackType.green, TrackType.blue, TrackType.alpha };
                    break;
            }

            var sliders = new List<Widget>();
            if (widget.showIndicator)
                sliders.Add(indicator());

            foreach (var palette in types)
            {
                if (!widget.enableAlpha && palette == TrackType.alpha) continue;

                sliders.Add(
                    new SizedBox(
                        width: widget.sliderSize.width,
                        height: widget.sliderSize.height,
                        child: new Row(
                            children: new List<Widget>
                            {
                                new Visibility(
                                    visible: widget.showSliderText,
                                    child:
                                        new Padding(
                                            padding: EdgeInsets.only(left:10f),
                                            child: new Text(
                                                palette.ToString().Split('.').last<string>().Substring(0,1).ToUpper(),
                                                style: widget.sliderTextStyle ?? Theme.of(context).textTheme.body1.copyWith(fontWeight:FontWeight.bold,fontSize: 16)
                                            )//Text
                                        )//Padding
                                ),//Visibility
                                new Expanded(child: colorPickerSlider(palette))
                            }
                        )//Row
                    )//SizedBox
                );
            }
            //if (widget.enableAlpha)
            //{
            //    sliders.Add(
            //        new SizedBox(
            //            height: 40,
            //            width: 260,
            //            child: colorPickerSlider(TrackType.alpha)
            //        )
            //    );
            //}

            return new Column(
                mainAxisAlignment: MainAxisAlignment.center,
                crossAxisAlignment: CrossAxisAlignment.center,
                children: sliders
            );
        }
    }
}