using System.Collections.Generic;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.material;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;

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
                                new Container(height:100)
                            }//list
                        )//Column
                    )//SingleChildScrollView
                );
            }
        }
    }
}