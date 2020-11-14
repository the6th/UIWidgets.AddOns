using Unity.UIWidgets.animation;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.material;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace UIWidgets.AddOns
{
    public class SplitViewExample : UIWidgetsPanel
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override Widget createWidget()
        {
            return new WidgetsApp(
                home: new SplitViewExample.SplitViewExampleApp(),
                pageRouteBuilder: (RouteSettings settings, WidgetBuilder builder) => new PageRouteBuilder(
                    settings: settings,
                    pageBuilder: (BuildContext context, Animation<float> animation,
                        Animation<float> secondaryAnimation) => builder(context)
                )
            );
        }

        public class SplitViewExampleApp : StatelessWidget
        {
            public override Widget build(BuildContext context)
            {
                return new MaterialApp(
                    title: "SplitViewExample",
                    theme: new ThemeData(primarySwatch: Colors.blue),
                    home: new SplitViewHome("SplitViewExampleHome")
                );
            }
        }

        public class SplitViewHome : StatefulWidget
        {
            public string title;
            public SplitViewHome(string title)
            {
                this.title = title;
            }

            public override State createState()
            {
                return new SplitViewHomeState();
            }
        }

        public class SplitViewHomeState : State<SplitViewHome>
        {
            public override Widget build(BuildContext context)
            {
                return new Scaffold(
                    appBar: new AppBar(title: new Text(widget.title)),
                    body: new SplitView(
                        initialWeight: 0.7f,
                        view1: new SplitView(
                            viewMode: SplitViewMode.Horizontal,
                            view1: new Container(
                                child: new Center(child: new Text("View-1")),
                                color: Colors.red
                            ),//container
                            view2: new Container(
                                child: new Center(child: new Text("View-2")),
                                color: Colors.blue
                            ),//container
                            onWeightChanged: (w) =>
                            {
                                Debug.Log($"Horizontal:{w}");
                            }
                        ),//split view
                        view2: new Container(
                            child: new Center(child: new Text("View-3"))
                        ),
                        viewMode: SplitViewMode.Vertical,
                        onWeightChanged: (w) =>
                        {
                            Debug.Log($"Vertical:{w}");
                        }
                    )//split view
                );
            }
        }
    }

}