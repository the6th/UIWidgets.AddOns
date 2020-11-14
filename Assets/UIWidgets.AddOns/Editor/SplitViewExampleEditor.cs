using Unity.UIWidgets.animation;
using Unity.UIWidgets.editor;
using Unity.UIWidgets.widgets;
using UnityEditor;

namespace UIWidgets.AddOns
{
    public class SplitViewExampleEditor : UIWidgetsEditorWindow
    {
        [MenuItem("UIWidgetsTests/AddOns/SplitViewExample")]
        public static void SplitViewExample()
        {
            EditorWindow.GetWindow<SplitViewExampleEditor>();
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

    }
}