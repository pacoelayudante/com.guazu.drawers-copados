using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Guazu.DrawersCopados
{
    public class TextAssetEsJSONAttribute : PropertyAttribute
    {
        public TextAssetEsJSONAttribute() { }
        [System.Serializable]
        private class DummyClassSerializable { }
#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(TextAssetEsJSONAttribute))]
        public class ValidarJSONAttributeDrawer : PropertyDrawer
        {
            const string errorSoloTextAsset = "Solo funciona con TextAsset";

            bool esTextAsset;
            bool EsTextAsset()
            {
                return fieldInfo.MemberType == System.Reflection.MemberTypes.Field && fieldInfo.FieldType == typeof(TextAsset);
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (!esTextAsset)
                {
                    position.height /= 2f;
                    EditorGUI.HelpBox(position, errorSoloTextAsset, MessageType.Warning);
                    position.y += position.height;
                }
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(position, property, label);
                if (EditorGUI.EndChangeCheck() && property.objectReferenceValue != null && esTextAsset)
                {
                    if (esTextAsset)
                    {
                        var textAsset = property.objectReferenceValue as TextAsset;
                        try
                        {
                            var obj = JsonUtility.FromJson(textAsset.text, typeof(DummyClassSerializable));
                            JsonUtility.FromJsonOverwrite(textAsset.text, obj);
                        }
                        catch// (System.Exception error)
                        {
                            //Debug.LogError(error);
                            EditorUtility.DisplayDialog("No parsea", "Este documento no parsea bien como JSON.\nVofi...", "Bajon");
                        }
                    }
                }
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                esTextAsset = EsTextAsset();
                return base.GetPropertyHeight(property, label) * (esTextAsset ? 1 : 2);
            }
        }
#endif
    }
}