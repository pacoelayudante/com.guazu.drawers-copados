using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace Guazu.DrawersCopados
{
    public class SingleLayerAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(SingleLayerAttribute))]
        public class SingleLayerAttributeDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (property.propertyType == SerializedPropertyType.Integer)
                {
                    property.intValue = EditorGUI.LayerField(position, label, property.intValue);
                }
                else
                {
                    EditorGUI.LabelField(position, label, "LayerAttribute solo con Ints");
                }
            }
        }
#endif
    }
}