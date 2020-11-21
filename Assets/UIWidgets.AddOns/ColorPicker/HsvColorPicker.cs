using System.Collections.Generic;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace UIWidgets.AddOns
{
    public class HsvColorPicker : StatefulWidget
    {
        public HSVColor pickerColor;
        public ValueChanged<HSVColor> onColorChanged;
        public PaletteType paletteType;
        public bool enableAlpha;
        public bool showLabel;
        public TextStyle labelTextStyle;
        public bool displayThumbColor;
        public float colorPickerWidth;
        public float pickerAreaHeightPercent;
        public BorderRadius pickerAreaBorderRadius;

        public HsvColorPicker(
            HSVColor pickerColor,
            TextStyle labelTextStyle,
            PaletteType paletteType = PaletteType.hsv,
            bool enableAlpha = true,
            bool showLabel = true,
            bool displayThumbColor = false,
            float colorPickerWidth = 300f,
            float pickerAreaHeightPercent = 1f,
            BorderRadius pickerAreaBorderRadius = null,
            ValueChanged<HSVColor> onColorChanged = null
            )
        {
            this.pickerColor = pickerColor;
            this.labelTextStyle = labelTextStyle;
            this.paletteType = paletteType;
            this.enableAlpha = enableAlpha;
            this.showLabel = showLabel;
            this.displayThumbColor = displayThumbColor;
            this.colorPickerWidth = colorPickerWidth;
            this.pickerAreaHeightPercent = pickerAreaHeightPercent;
            if (pickerAreaBorderRadius != null)
                this.pickerAreaBorderRadius = pickerAreaBorderRadius;
            else
                this.pickerAreaBorderRadius = BorderRadius.all(Radius.zero);
            this.onColorChanged = onColorChanged;
        }


        public override State createState()
        {
            throw new System.NotImplementedException();
        }
    }

    public class _HsvColorPickerState : State<HsvColorPicker>
    {
        HSVColor currentHsvColor = HSVColor.fromAHSV(0f, 0f, 0f, 0f);
        public override void initState()
        {
            base.initState();
            currentHsvColor = widget.pickerColor;
        }

        public override void didUpdateWidget(StatefulWidget oldWidget)
        {
            base.didUpdateWidget(oldWidget);
            currentHsvColor = widget.pickerColor;
        }

        public Widget colorPickerSlider(TrackType trackType)
        {
            return new ColorPickerSlider(
                trackType,
                currentHsvColor,
                (HSVColor color) => {
                    setState(()=> { currentHsvColor = color; });
                    widget.onColorChanged(currentHsvColor);
                },
                displayThumbColor: widget.displayThumbColor
            );
        }

        public Widget colorPickerArea()
        {
            return new ClipRRect(
                borderRadius: widget.pickerAreaBorderRadius,
                child: new ColorPickerArea(
                    currentHsvColor,
                    (HSVColor color) =>{
                        setState(()=> { currentHsvColor = color; });
                        widget.onColorChanged(currentHsvColor);
                    },
                    widget.paletteType
                )//ColorPickerArea
            );//ClipRRect
        }

        public override Widget build(BuildContext context)
        {
            if (MediaQuery.of(context).orientation == Orientation.portrait)
            {
                var list = new List<Widget> {
                    new SizedBox(
                        height: 40f,
                        width: widget.colorPickerWidth - 75f,
                        child: colorPickerSlider(TrackType.alpha)
                    )
                };
                if (widget.enableAlpha)
                {
                    list.Add(
                        new SizedBox(
                            height: 40f,
                            width: widget.colorPickerWidth - 75f,
                            child: colorPickerSlider(TrackType.alpha)
                        )
                    );
                }

                return new Column(
                    children: new List<Widget> {
                        new SizedBox(
                            width: widget.colorPickerWidth,
                            height: widget.colorPickerWidth * widget.pickerAreaHeightPercent,
                            child:  colorPickerArea()
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
                                            children: list
                                        )//Column
                                    )//Expanded
                                }
                            )//Row
                        ),//Padding
                        (!widget.showLabel ? null :new ColorPickerLabel(
                            currentHsvColor,
                            enableAlpha: widget.enableAlpha,
                            textStyle: widget.labelTextStyle
                            )//ColorPickerLabel
                        ),
                        new SizedBox(height:20f)
                    }
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
                                    children: new List<Widget>
                                    {
                                        new SizedBox(width:20f),
                                        new ColorIndicator(currentHsvColor),
                                        new Column(
                                            children: new List<Widget>
                                            {
                                                new SizedBox(
                                                    width:40f,
                                                    height: 260f,
                                                    child: colorPickerSlider(TrackType.hue)
                                                ),//SizedBox
                                                (!widget.enableAlpha ? null : new SizedBox(
                                                    height:40f,
                                                    width:260f,
                                                    child: colorPickerSlider(TrackType.alpha)
                                                    )//SizedBox
                                                )
                                            }
                                        ),
                                        new SizedBox(width:10f)
                                    }
                                )//Row
                            }
                        ),//Column
                        new SizedBox(height: 20f),
                        (!widget.showLabel ? null : new ColorPickerLabel(
                                currentHsvColor,
                                enableAlpha: widget.enableAlpha,
                                textStyle: widget.labelTextStyle
                            )//ColorPickerLabel
                        )
                    }
                );
            }
        }
    }
}