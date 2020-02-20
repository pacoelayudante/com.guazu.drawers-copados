using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Guazu.DrawersCopados
{
    public class CreameScriptableAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(CreameScriptableAttribute))]
        public class CreameScriptableAttributeDrawer : PropertyDrawer
        {
            const string errorSoloScriptableObject = "Solo funciona con derivados de ScriptableObject";
            static readonly Vector2 proporcionBoton = new Vector2(.25f, 1f);
            static readonly Vector2 proporcionField = new Vector2(1f - proporcionBoton.x, 1f);
            static System.Type tipoScriptableObject = typeof(ScriptableObject);
            static System.Type tipoScriptableObjectArray = typeof(ScriptableObject[]);
            static System.Type tipoScriptableObjectList = typeof(List<ScriptableObject>);

            System.Type tipoDetectado = null;
            bool esScriptableObject;
            bool EsScriptableObject()
            {
                tipoDetectado = fieldInfo.FieldType;
                if (tipoDetectado.IsGenericType) tipoDetectado = fieldInfo.FieldType.GetGenericArguments()[0];
                return fieldInfo.MemberType == System.Reflection.MemberTypes.Field &&
                (tipoScriptableObject.IsAssignableFrom(tipoDetectado) ||
                tipoScriptableObjectArray.IsAssignableFrom(fieldInfo.FieldType));
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (!esScriptableObject)
                {
                    position.height /= 2f;
                    EditorGUI.HelpBox(position, errorSoloScriptableObject, MessageType.Warning);
                    position.y += position.height;
                }
                var propFieldPosition = position;
                if (esScriptableObject && property.objectReferenceValue == null) propFieldPosition = new Rect(position.position, position.size * proporcionField);
                EditorGUI.PropertyField(propFieldPosition, property, label);
                if (esScriptableObject && property.objectReferenceValue == null)
                {
                    position.x += position.size.x * proporcionField.x;
                    position.size *= proporcionBoton;
                    if (GUI.Button(position, new GUIContent("Crear",$"Crear {tipoDetectado.Name}")))
                    {
                        var creado = CrearAsset();
                        if (creado != null) property.objectReferenceValue = creado;
                    }
                }
            }

            ScriptableObject CrearAsset()
            {
                var destino = EditorUtility.SaveFilePanelInProject(string.Format("Crear {0}", tipoDetectado.Name), tipoDetectado.Name, "asset", "Elegir donde generar asset dentro del proyecto");
                if (!string.IsNullOrEmpty(destino))
                {
                    var creado = ScriptableObject.CreateInstance(tipoDetectado);
                    AssetDatabase.CreateAsset(creado, destino);
                    creado = AssetDatabase.LoadAssetAtPath<ScriptableObject>(destino);
                    EditorGUIUtility.PingObject(creado);
                    return creado;
                }
                return null;
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                esScriptableObject = EsScriptableObject();
                return base.GetPropertyHeight(property, label) * (esScriptableObject ? 1 : 2);
            }
        }
#endif
    }
}