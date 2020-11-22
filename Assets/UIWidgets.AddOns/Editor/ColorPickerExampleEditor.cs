using Unity.UIWidgets.editor;
using Unity.UIWidgets.material;
using Unity.UIWidgets.widgets;
using UnityEditor;

namespace UIWidgets.AddOns
{
    public class ColorPickerExampleEditor : UIWidgetsEditorWindow
    {
        [MenuItem("UIWidgetsTests/AddOns/ColorPickerExample")]
        public static void SplitViewExample()
        {
            EditorWindow.GetWindow<ColorPickerExampleEditor>();
        }

        protected override Widget createWidget()
        {
            return new MaterialApp(
                 home: new ColorPickerExampleWidget()
                 //pageRouteBuilder: (RouteSettings settings, WidgetBuilder builder) => new PageRouteBuilder(
                 //    settings: settings,
                 //    pageBuilder: (BuildContext context, Animation<float> animation,
                 //        Animation<float> secondaryAnimation) => builder(context)
                 //)
             );
        }
    }
}
