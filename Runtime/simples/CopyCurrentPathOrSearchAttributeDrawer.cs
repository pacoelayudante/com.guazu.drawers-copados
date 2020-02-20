using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

namespace Guazu.DrawersCopados
{
    public class CopyCurrentFolderAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(CopyCurrentFolderAttribute))]
        public class CopyCurrentFolderAttributeDrawer : PropertyDrawer
        {
            static float tamBoton = EditorGUIUtility.singleLineHeight * 2f;

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (property.propertyType == SerializedPropertyType.String)
                {
                    position.width -= tamBoton;
                }
                EditorGUI.PropertyField(position, property, label);
                if (property.propertyType == SerializedPropertyType.String)
                {
                    position.x += position.width;
                    position.width = tamBoton;
                    if (GUI.Button(position, "\u00A9"))
                    {
                        var primerFolderObj = Selection.GetFiltered<Object>(SelectionMode.Assets | SelectionMode.TopLevel).FirstOrDefault(e => ProjectWindowUtil.IsFolder(e.GetInstanceID()));
                        var primerFolderPath = primerFolderObj ? AssetDatabase.GetAssetPath(primerFolderObj) : null;
                        if (!string.IsNullOrEmpty(primerFolderPath)) property.stringValue = primerFolderPath;
                        else {
                            EditorUtility.DisplayDialog("Un temita...","Para que esto funcione tenes que haber seleccionado una carpeta en la ventana de navegacion (la parte que te muestra el arbol, a la izquierda)","¡Que loco!");
                        }
                    }
                }
            }
        }
#endif
    }
}