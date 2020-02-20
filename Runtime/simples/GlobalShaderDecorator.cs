using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Guazu.DrawersCopados
{
    // Por Ahora solo un float sobre un Vector4.X
    public class GlobalShaderAttribute : PropertyAttribute
    {
        GUIContent label;
        string variable;
        float min, max;

        public GlobalShaderAttribute(string variable, string label, float min, float max) : this(variable, new GUIContent(label), min, max) { }
        public GlobalShaderAttribute(string variable, string label, string tooltip, float min, float max) : this(variable, new GUIContent(label,tooltip), min, max) { }
        public GlobalShaderAttribute(string variable, GUIContent label, float min, float max)
        {
            this.label = label;
            this.variable = variable;
            this.min = min;
            this.max = max;
        }
#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(GlobalShaderAttribute))]
        class GlobalShaderDecoratorDrawer : DecoratorDrawer
        {
            public override void OnGUI(Rect position)
            {
                var decorator = (GlobalShaderAttribute)attribute;
                EditorGUI.BeginChangeCheck();
                var valor = EditorGUI.Slider(position, decorator.label, Shader.GetGlobalVector(decorator.variable).x, decorator.min, decorator.max);
                if (EditorGUI.EndChangeCheck()){
                    Shader.SetGlobalVector(decorator.variable, new Vector4(valor, 0f, 0f, 0f));
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                }
            }
            public override float GetHeight()
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }
#endif
    }
}