using System.Collections.Generic;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.material;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Color = Unity.UIWidgets.ui.Color;

namespace UIWidgets.AddOns
{
    public class UMateriaiWidgetExample : UIWidgetsPanel
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
                home: new UMateriaiWidgetExample.CommonWidgetExampleApp()
            );
        }

        public class CommonWidgetExampleApp : StatefulWidget
        {
            public List<string> dropdownList = new List<string>() { "Item1", "Item2", "Item3" };
            public int dropdownIndex = 0;
            public bool isChecked = false;
            public float floatvalue1 = 0.5f;
            public float floatvalue2 = 0.5f;
            public float sliderValue = 0.5f;
            public string dummyText = "dummy text";
            public Color color = Colors.yellowAccent;

            public List<Color> availableColors = new List<Color>
            {
                Colors.red,
                Colors.pink,
                Colors.purple,
                Colors.deepPurple,
                Colors.indigo,
                //Colors.blue,
                //Colors.lightBlue,
                //Colors.cyan,
                //Colors.teal,
                //Colors.green,
                //Colors.lightGreen,
                //Colors.lime,
                //Colors.yellow,
                //Colors.amber,
                //Colors.orange,
                //Colors.deepOrange,
                Colors.brown,
                Colors.grey,
                Colors.blueGrey,
                Colors.black
            };

            public override State createState()
            {
                return new CommonWidgetExampleAppState();
            }
        }


        public class CommonWidgetExampleAppState : State<CommonWidgetExampleApp>
        {
            public override Widget build(BuildContext context)
            {
                return new Scaffold(
                    appBar: new AppBar(title:new Text("UMateriaiWidgetExample")),
                    body: new SingleChildScrollView(

                        child: new Column(
                            children: new List<Widget>
                            {
                                UMateriaiWidget.SettingTitle("SettingTitle"),
                                UMateriaiWidget.SettingDropDown(
                                    title:"SettingDropDown",
                                    itemList:widget.dropdownList,
                                    selectedIndex: widget.dropdownIndex,
                                    onValueChanged:(int value)=>{
                                        setState(()=> widget.dropdownIndex = value);
                                    }
                                ),
                                UMateriaiWidget.SettingTitle("Header2"),

                                UMateriaiWidget.SettingCheckbox(
                                    value:  widget.isChecked,
                                    title:  "SettingCheckbox",
                                    onValueChanged:(bool? value)=>{
                                        setState(()=>{widget.isChecked = value ?? false;});
                                    }
                                ),

                                UMateriaiWidget.SettingValueUpDown(
                                    title: "SettingValueUpDown",
                                    value: $"{widget.floatvalue1:N1}",
                                    upPressed:()=>{
                                        setState(()=> widget.floatvalue1 +=0.1f);
                                    },
                                    downPressed:()=>{
                                        setState(()=> widget.floatvalue1 +=0.1f);
                                    }
                                ),


                                UMateriaiWidget.SettingSlider(
                                    title: "SettingSlider",
                                    value:widget.sliderValue,
                                    divisions:4,
                                    valueChanged: (float value) =>{
                                        setState(()=>{
                                            widget.sliderValue = value;
                                        });
                                    }
                                ),
                                UMateriaiWidget.SettingInputField(
                                    title: "SettingInputField",
                                    value:widget.dummyText,
                                    valueChanged:(string value)=>{
                                        widget.dummyText = value;
                                    }
                                ),
                                UMateriaiWidget.SettingButton(
                                    title:"SettingButton",
                                    buttonName:"ok",
                                    onPressed: ()=>{
                                        Debug.Log("Press");
                                    }
                                ),
                                UMateriaiWidget.SettingValueUpDownButton(
                                    title:"UpDownButton",
                                    value: $"{widget.floatvalue2:N1}",
                                    downPressed:()=>{
                                        setState(()=>{
                                            if(widget.floatvalue2 >0f)
                                                widget.floatvalue2 -=0.1f;
                                        });
                                    },
                                    upPressed:()=>{
                                        setState(()=>{
                                            if(widget.floatvalue2 < 1f)
                                                widget.floatvalue2 +=0.1f;
                                        });
                                    }
                                ),
                                UMateriaiWidget.SettingHSVColorPicker(
                                    context: context,
                                    title:"ColorPicker1",
                                    color: widget.color,
                                    enableAlpha: false,
                                    showPreviousColor: false,
                                    onColorChanged:(Color color)=>{
                                        setState(()=>{
                                            widget.color = color;
                                        });
                                        Debug.Log("onColorChanged:" + color.ToHexString());
                                    }
                                ),
                                UMateriaiWidget.SettingHSVColorPicker(
                                    context: context,
                                    title:"ColorPicker2",
                                    color: widget.color,
                                    enableAlpha: false,
                                    showPreviousColor: true,
                                    onColorChanged:(Color color)=>{
                                        setState(()=>{
                                            widget.color = color;
                                        });
                                        Debug.Log("onColorChanged:" + color.ToHexString());
                                    }
                                ),

                                UMateriaiWidget.SettingBlockColorPicker(
                                    context: context,
                                    title:"ColorBlock",
                                    color: widget.color,
                                    availableColors: widget.availableColors,
                                    onColorChanged:(Color color)=>{
                                        setState(()=>{
                                            widget.color = color;
                                        });
                                        Debug.Log("onColorChanged:" + color.ToHexString());
                                    }
                                ),
                                new Container(height:100)
                            }//list
                        )//Column
                    )//SingleChildScrollView
                );
            }
        }
    }
}