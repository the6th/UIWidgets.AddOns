using System.Collections.Generic;
using System.Linq;
using Unity.UIWidgets.cupertino;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace UIWidgets.AddOns
{
    public class CommonWidget
    {
        public static Widget SettingTitle(string text = "title")
        {
            return new Container(
                padding: EdgeInsets.symmetric(vertical: 8f, horizontal: 6f),
                alignment: Alignment.topLeft,
                child: new Text(text),
                decoration: new BoxDecoration(
                    color: Colors.black12,
                    border: new Border(
                        top: new BorderSide(
                            width: 0.2f,
                            color: Colors.grey
                        )
                    )
                )
            );
        }

        public static Widget SettingDropDown(string title, List<string> itemList, int selectedIndex = 0, ValueChanged<int> callback = null)
        {
            return new Container(
                padding: EdgeInsets.symmetric(horizontal: 6f),
                child: new Row(
                    children: new List<Widget>
                    {
                        new Container(child: new Text(title)),
                        new Expanded(
                        child: new Container(
                            //color: Colors.red,
                            alignment: Alignment.bottomRight,
                            child:  new DropdownButton<string>(
                                        style: new TextStyle(fontSize:14),
                                        value: itemList[selectedIndex],
                                        onChanged: (string newValue) => {
                                            int index = itemList.FindLastIndex(s=>s.Equals(newValue));
                                            //Debug.Log($"onChanged:{newValue} at {index}");
                                            callback?.Invoke(index);
                                        },
                                        items: itemList.Select<string, DropdownMenuItem<string>>(value => {
                                            return new DropdownMenuItem<string>(
                                                value: value,
                                                child: new Text(value)
                                            );
                                        }).ToList()
                                    )//dropdown
                            )//container
                        )//expanded
                    }
            )
            );
        }
        public static Widget SettingToggle(string title, bool b)
        {
            return new Container(
                //color: Colors.red,
                padding: EdgeInsets.symmetric(horizontal: 6f),
                child: new Row(
                    children: new List<Widget>
                    {
                        new Container(width:100, child: new Text(title)),
                        new CupertinoSwitch(
                            value: b,
                            onChanged:(bool value)=>{
                                Debug.Log(value);
                            }
                        )//CupertinoSwitch
                    }//list
                )//row
            );
        }
        public static Widget SettingCheckbox(string title, bool? b, ValueChanged<bool?> callback = null)
        {
            return new Container(

                padding: EdgeInsets.symmetric(horizontal: 6f),
                child: new Row(
                    children: new List<Widget>
                    {
                        new Container( child: new Text(title)),
                        new Expanded(
                            child: new Container(
                                    //color: Colors.red,
                                    alignment: Alignment.centerRight,
                                    child:  new Checkbox(
                                            value: b,
                                            onChanged:(bool? value)=>{
                                                //Debug.Log($"SettingCheckbox:{value}");
                                                callback?.Invoke(value);
                                            }
                                        )//Checkbox
                                )//Container
                        )//Expanded
                    }//list
                )//row
            );
        }

        public static Widget SettingValueUpDown(string title, string value, VoidCallback upPressed = null, VoidCallback downPressed = null)
        {
            return new Container(
                padding: EdgeInsets.symmetric(horizontal: 6f),
                child: new Row(
                    children: new List<Widget>
                    {
                        new Container(
                            child: new Text(title)
                        ),
                        new Expanded(
                            child: new Row(
                                mainAxisAlignment:Unity.UIWidgets.rendering.MainAxisAlignment.end,
                                children:    new List<Widget>
                                {
                                    new Container(
                                        padding: EdgeInsets.symmetric(horizontal: 16f),
                                        child: new Text(value.ToString())
                                    ),
                                    new Column(
                                                
                                        children: new List<Widget>{
                                            new Container(
                                                decoration: new BoxDecoration(
                                                    border: new Border(bottom: new BorderSide(width:0.1f,color: Colors.black26))
                                                ),
                                                child:new InkWell(
                                                    child: new Icon(Icons.arrow_drop_up,size:18),
                                                    onTap: ()=>{upPressed?.Invoke();}
                                                )//InkWell
                                            ),//Container
                                            new Container(
                                                decoration: new BoxDecoration(
                                                    border: new Border(bottom: new BorderSide(width:0.5f,color:Colors.white))
                                                ),
                                                child:new InkWell(
                                                    child: new Icon(Icons.arrow_drop_down,size:18),
                                                    onTap: ()=>{downPressed?.Invoke();}
                                                )//InkWell
                                            )//Container
                                        }
                                    ),//column
                                }//end children
                            )//row
                        )//expanded
                    }//list
                )//row
            );
        }

        public static Widget SettingValueUpDownButton(string title, string value, VoidCallback upPressed = null, VoidCallback downPressed = null, string upButtonText = "+", string downButtontext = "-")
        {
            return new Container(
                padding: EdgeInsets.symmetric(horizontal: 6f),
                child: new Row(
                    children: new List<Widget>
                    {
                        new Container(
                            child: new Text(title)
                        ),
                        new Expanded(
                            child: new Row(
                                mainAxisAlignment:Unity.UIWidgets.rendering.MainAxisAlignment.end,
                                children:new List<Widget>
                                {
                                    new Container(
                                        width:40,
                                        child:new RaisedButton(
                                            padding: EdgeInsets.zero,
                                            child:  new Text(downButtontext),
                                            color: Colors.white,
                                            shape: new CircleBorder(
                                                side: new BorderSide(
                                                    color:  Colors.grey,
                                                    width:  1,
                                                    style:  BorderStyle.solid
                                                )//BorderSide
                                            ),//CircleBorder
                                            onPressed:  ()=>{
                                                downPressed?.Invoke();
                                            }
                                        )
                                    ),//RaisedButton
                                    new Container(
                                        padding: EdgeInsets.symmetric(horizontal: 16f),
                                        alignment: Alignment.center,
                                        child: new Text(value.ToString())
                                    ),
                                    new Container(
                                        width:40,
                                        child:new RaisedButton(
                                            padding: EdgeInsets.zero,
                                            child:  new Text(upButtonText),
                                            color: Colors.white,
                                            shape: new CircleBorder(
                                                side: new BorderSide(
                                                    color:  Colors.grey,
                                                    width:  1,
                                                    style:  BorderStyle.solid
                                                )//BorderSide
                                            ),//CircleBorder
                                            onPressed:  ()=>{
                                                upPressed?.Invoke();
                                            }
                                        )//RaisedButton
                                    )//Container
                                }
                            )//Container
                        )//Expand
                    }//list
                )//row
            );
        }
    }
}
