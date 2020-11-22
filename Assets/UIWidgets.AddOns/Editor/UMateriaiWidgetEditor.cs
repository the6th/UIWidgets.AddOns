using Unity.UIWidgets.editor;
using Unity.UIWidgets.material;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEditor;
using UnityEngine;

namespace UIWidgets.AddOns
{
    public class UMateriaiWidgetEditor : UIWidgetsEditorWindow
    {
        protected override void OnEnable()
        {
            FontManager.instance.addFont(Resources.Load<Font>("fonts/MaterialIcons-Regular"), "Material Icons");

            base.OnEnable();
        }

        [MenuItem("UIWidgetsTests/AddOns/UMateriaiWidgetExample")]
        public static void CommonWidgetExample()
        {
            EditorWindow.GetWindow<UMateriaiWidgetEditor>();
        }

        protected override Widget createWidget()
        {
            return new MaterialApp(
                home: new UMateriaiWidgetExample.CommonWidgetExampleApp()
            );
        }
    }
}
