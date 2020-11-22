using System.Collections.Generic;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.gestures;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Color = Unity.UIWidgets.ui.Color;
using DialogUtils = Unity.UIWidgets.material.DialogUtils;

namespace UIWidgets.AddOns
{
    public class ColorPickerExample : UIWidgetsPanel
    {
        protected override void OnEnable()
        {
            FontManager.instance.addFont(Resources.Load<Font>("fonts/MaterialIcons-Regular"), "Material Icons");
            Window.onFrameRateCoolDown = () => { };
            Window.onFrameRateSpeedUp = () => { };
            base.OnEnable();
        }
        protected override Widget createWidget()
        {
            return new MaterialApp(
                home: new ColorPickerExampleWidget()
            );
        }

    }
    public class ColorPickerExampleWidget : StatefulWidget
    {
        public override State createState()
        {
            return new ColorPickerExampleState();
        }
    }

    public class ColorPickerExampleState : State<ColorPickerExampleWidget>
    {
        public bool lightTheme = true;
        public Color currentColor = Colors.limeAccent;
        public List<Color> currentColors = new List<Color> { Colors.limeAccent, Colors.green };

        void changeColor(Color color) => setState(() => currentColor = color);
        void changeColors(List<Color> colors) => setState(() => currentColors = colors);

        public override Widget build(BuildContext context)
        {
            return new Theme(
                data: lightTheme ? ThemeData.light() : ThemeData.dark(),
                child: new DefaultTabController(
                    length: 3,
                    child: new Scaffold(
                        appBar: new AppBar(
                            title: new GestureDetector(
                                child: new Text("Color Picker Example"),
                                onDoubleTap: (DoubleTapDetails details) => setState(() => lightTheme = !lightTheme)
                            ),//GestureDetector
                            bottom: new TabBar(
                                tabs: new List<Widget>
                                {
                                    new Tab(text:"HSV"),
                                    new Tab(text:"Material"),
                                    new Tab(text:"Block")
                                }
                            )//TabBar
                        ),//AppBar
                        body: new TabBarView(
                            physics: new NeverScrollableScrollPhysics(),
                            children: new List<Widget>
                            {
                                new Column(
                                    mainAxisAlignment: MainAxisAlignment.center,
                                    children: new List<Widget>{
                                        new RaisedButton(
                                            elevation: 3f,
                                            onPressed: () =>
                                            {
                                                DialogUtils.showDialog(
                                                    context: context,
                                                    builder:(BuildContext _)=>{
                                                        return new AlertDialog(
                                                            titlePadding: EdgeInsets.all(0f),
                                                            contentPadding: EdgeInsets.all(0f),
                                                            content: new SingleChildScrollView(
                                                                child: new ColorPicker(
                                                                    pickerColor: currentColor,
                                                                    onColorChanged: changeColor,
                                                                    colorPickerWidth:300f,
                                                                    pickerAreaHeightPercent:0.7f,
                                                                    enableAlpha: true,
                                                                    displayThumbColor: true,
                                                                    showLabel: true,
                                                                    paletteType: PaletteType.hsv,
                                                                    pickerAreaBorderRadius:BorderRadius.only(
                                                                        topLeft: Radius.circular(2f),
                                                                        topRight: Radius.circular(2f)
                                                                    )//BorderRadius
                                                                    
                                                                )//ColorPicker
                                                            )//SingleChildScrollView
                                                        );//AlertDialog
                                                    }
                                                );
                                            },
                                            child: new Text("Change me"),
                                            color: currentColor,
                                            textColor: Utils.useWhiteForeground(currentColor)
                                            ? new Color(0xffffffff)
                                            : new Color(0xff000000)
                                        ),//RaisedButton
                                        new RaisedButton(
                                            elevation: 3f,
                                            onPressed: ()=>{
                                                DialogUtils.showDialog(
                                                    context: context,
                                                    builder:(BuildContext _)=>{
                                                        return new AlertDialog(
                                                            titlePadding: EdgeInsets.all(0),
                                                            contentPadding: EdgeInsets.all(0f),
                                                            shape: new RoundedRectangleBorder(
                                                                borderRadius: BorderRadius.circular(25)
                                                            ),//RoundedRectangleBorder
                                                            content: new SingleChildScrollView(
                                                                child: new SlidePicker(
                                                                    pickerColor: currentColor,
                                                                    onColorChanged: changeColor,
                                                                    paletteType: PaletteType.rgb,
                                                                    enableAlpha: false,
                                                                    displayThumbColor: true,
                                                                    showLabel: false,
                                                                    showIndicator: true,
                                                                    indicatorBorderRadius: BorderRadius.vertical(top: Radius.circular(25))
                                                                )//SlidePicker
                                                            )//SingleChildScrollView
                                                        );//AlertDialog
                                                    }
                                                );
                                            },
                                            child: new Text("Change me again"),
                                            color: currentColor,
                                            textColor: Utils.useWhiteForeground(currentColor)
                                            ? new Color(0xffffffff)
                                            : new Color(0xff000000)
                                        )
                                    }
                                ),//Column
                                new Center(
                                    child: new RaisedButton(
                                        elevation:3f,
                                        onPressed:()=>{
                                            DialogUtils.showDialog(
                                                context:context,
                                                builder:(BuildContext _) => {
                                                    return new AlertDialog(
                                                        titlePadding: EdgeInsets.all(0f),
                                                        contentPadding: EdgeInsets.all(0f),
                                                        content: new SingleChildScrollView(
                                                            child: new MaterialPicker(
                                                                pickerColor: currentColor,
                                                                onColorChanged:changeColor,
                                                                enableLabel: true
                                                            )//MaterialPicker
                                                        )//SingleChildScrollView
                                                    );//AlertDialog
                                                }
                                            );//showDialog
                                        },//onPresed
                                        child: new Text("Change me"),
                                        color: currentColor,
                                        textColor: Utils.useWhiteForeground(currentColor)
                                        ? new Color(0xffffffff)
                                        : new Color(0xff000000)
                                    )//RaisedButton
                                ),//Center
                                new Center(
                                    child: new Column(
                                        mainAxisAlignment: MainAxisAlignment.center,
                                        children: new List<Widget>{
                                            new RaisedButton(
                                                elevation:3f,
                                                onPressed:()=>{
                                                    DialogUtils.showDialog(
                                                        context: context,
                                                        builder:(BuildContext _) =>{
                                                            return new AlertDialog(
                                                                title: new Text("Select color"),
                                                                content: new SingleChildScrollView(
                                                                    child: new BlockPicker(
                                                                        pickerColor: currentColor,
                                                                        onColorChanged: changeColor
                                                                    )//BlockPicker
                                                                )//SingleChildScrollView
                                                            );//AlertDialog
                                                        }
                                                    );//showDialog
                                                },//onPressed
                                                child: new Text("Change me again"),
                                                color:currentColor,
                                                textColor: Utils.useWhiteForeground(currentColor)
                                                ? new Color(0xffffffff)
                                                : new Color(0xff000000)
                                            )//RaisedButton
                                        }
                                    )//Column
                                )//center
                            }
                        )//TabBarView
                    )//Scaffold
                )//DefaultTabController
            );//Theme
        }//function build
    }//class ColorPickerExampleState
}