using System;
using System.Collections.Generic;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.gestures;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Color = Unity.UIWidgets.ui.Color;
using Material = Unity.UIWidgets.material.Material;

namespace UIWidgets.AddOns
{
    public delegate Widget PickerLayoutBuilder(BuildContext context, List<Color> colors, PickerItem child);
    public delegate Widget PickerItem(Color color);
    public delegate Widget PickerItemBuilder(Color color, bool isCurrentColor, GestureTapCallback changeColor);

    public class BlockPicker : StatefulWidget
    {
        public Color pickerColor;
        public ValueChanged<Color> onColorChanged;
        public List<Color> availableColors;
        public PickerLayoutBuilder layoutBuilder;
        public PickerItemBuilder itemBuilder;

        public BlockPicker(
            Color pickerColor,
            PickerItemBuilder itemBuilder = null,
            PickerLayoutBuilder layoutBuilder = null,
            ValueChanged<Color> onColorChanged = null,
            List<Color> availableColors = null
            )
        {
            D.assert(pickerColor != null);
            D.assert(onColorChanged != null);

            this.pickerColor = pickerColor;
            this.onColorChanged = onColorChanged;

            if (availableColors == null)
                this.availableColors = Utils._defaultColors;
            else
                this.availableColors = availableColors;

            if (layoutBuilder == null)
                this.layoutBuilder = defaultLayoutBuilder;
            else
                this.layoutBuilder = layoutBuilder;

            if (itemBuilder == null)
                this.itemBuilder = defaultItemBuilder;
            else
                this.itemBuilder = itemBuilder;
        }

        public static Widget defaultLayoutBuilder(
            BuildContext context,
            List<Color> colors,
            PickerItem child
            )
        {
            Orientation orientation = MediaQuery.of(context).orientation;

            var list = new List<Widget>();
            foreach (var color in colors)
            {
                list.Add(child(color));
            }
            return new Container(
                width: orientation == Orientation.portrait ? 300f : 300f,
                height: orientation == Orientation.portrait ? Mathf.FloorToInt(list.Count / 4) * 60f : 200f,
                child: GridView.count(
                    crossAxisCount: orientation == Orientation.portrait ? 4 : 6,
                    crossAxisSpacing: 5f,
                    mainAxisSpacing: 5f,
                    children: list
                )//GridView.count
            ); //Container
        }

        public static Widget defaultItemBuilder(
            Color color,
            bool isCurrentColor,
            GestureTapCallback changeColor
            )
        {
            return new Container(
                margin: EdgeInsets.all(5f),
                decoration: new BoxDecoration(
                    borderRadius: BorderRadius.circular(50f),
                    color: color,
                    boxShadow: new List<BoxShadow>
                    {
                        new BoxShadow(
                            color: color.withOpacity(0.8f),
                            offset: new Offset(1f,2f),
                            blurRadius: 3f
                        )//BoxShadow
                    }
                ),//BoxDecoration
                child: new Material(
                    color: Colors.transparent,
                    child: new InkWell(
                        onTap: changeColor,
                        borderRadius: BorderRadius.circular(50f),
                        child: new AnimatedOpacity(
                            duration: new TimeSpan(0, 0, 0, 0, 210),
                            opacity: isCurrentColor ? 1f : 0f,
                            child: new Icon(
                                Icons.done,
                                color: Utils.useWhiteForeground(color) ? Colors.white : Colors.black
                            )//Icon
                        )//AnimatedOpacity
                    )//InkWell
                )// Material 
            );
        }


        public override State createState()
        {
            return new _BlockPickerState();
        }

        public class _BlockPickerState : State<BlockPicker>
        {
            Color _currentColor;

            public override void initState()
            {
                _currentColor = widget.pickerColor;
                base.initState();
            }

            void changeColor(Color color)
            {
                setState(() =>
                {
                    _currentColor = color;
                    widget.onColorChanged(color);
                });
            }

            public override Widget build(BuildContext context)
            {
                return widget.layoutBuilder(
                    context,
                    widget.availableColors ?? Utils._defaultColors,
                    (Color col) => widget.itemBuilder(
                        col,
                        _currentColor.value == col.value,
                        () => changeColor(col)
                    )
                );
            }
        }
    }


    public class MultipleChoiceBlockPicker : StatefulWidget
    {
        public List<Color> pickerColors;
        public ValueChanged<List<Color>> onColorsChanged;
        public List<Color> availableColors;
        public PickerLayoutBuilder layoutBuilder;
        public PickerItemBuilder itemBuilder;

        public MultipleChoiceBlockPicker(
            List<Color> pickerColors,
            ValueChanged<List<Color>> onColorsChanged,
            PickerLayoutBuilder layoutBuilder = null,
            PickerItemBuilder itemBuilder = null,
            List<Color> availableColors = null
            )
        {
            this.pickerColors = pickerColors;
            this.onColorsChanged = onColorsChanged;

            if (availableColors == null)
                this.availableColors = Utils._defaultColors;
            else
                this.availableColors = availableColors;
            if (layoutBuilder == null)
                this.layoutBuilder = BlockPicker.defaultLayoutBuilder;
            else
                this.layoutBuilder = layoutBuilder;
            if (itemBuilder == null)
                this.itemBuilder = BlockPicker.defaultItemBuilder;
            else
                this.itemBuilder = itemBuilder;
        }

        public override State createState()
        {
            return new _MultipleChoiceBlockPickerState();
        }
    }
    public class _MultipleChoiceBlockPickerState : State<MultipleChoiceBlockPicker>
    {
        List<Color> _currentColors;

        public override void initState()
        {
            _currentColors = widget.pickerColors;
            base.initState();
        }

        void toggleColor(Color color)
        {
            setState(() =>
            {
                if (_currentColors.Contains(color))
                    _currentColors.Remove(color);
                else
                    _currentColors.Add(color);
            });
        }

        public override Widget build(BuildContext context)
        {
            return widget.layoutBuilder(
                context,
                widget.availableColors,
                (Color color) => widget.itemBuilder(
                    color,
                    _currentColors.Contains(color),
                    () => toggleColor(color)
                )//itemBuilder
            );
        }
    }
}