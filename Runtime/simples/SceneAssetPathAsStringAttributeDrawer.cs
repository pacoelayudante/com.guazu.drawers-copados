using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Guazu.DrawersCopados
{

    public class SceneAssetPathAsStringAttribute : PropertyAttribute
    {
        readonly bool incluirTodoElPath;

        public SceneAssetPathAsStringAttribute() : this(false) { }
        public SceneAssetPathAsStringAttribute(bool incluirTodoElPath)
        {
            this.incluirTodoElPath = incluirTodoElPath;
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(SceneAssetPathAsStringAttribute))]
        public class SceneAssetPathAsStringAttributeDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (property.propertyType != SerializedPropertyType.String)
                {
                    EditorGUI.HelpBox(position, "Solo usar con strings", MessageType.Error);
                    return;
                }

                var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);
                if (scene == null && !string.IsNullOrEmpty(property.stringValue))
                {
                    string[] guids = AssetDatabase.FindAssets(property.stringValue + " t:scene");
                    if (guids.Length > 0) scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
                }

                EditorGUI.BeginChangeCheck();
                if(scene!=null) label.tooltip = property.stringValue;
                scene = EditorGUI.ObjectField(position, label, scene, typeof(SceneAsset), false) as SceneAsset;
                if (EditorGUI.EndChangeCheck())
                {
                    var att = attribute as SceneAssetPathAsStringAttribute;
                    if (att.incluirTodoElPath)
                    {
                        property.stringValue = AssetDatabase.GetAssetPath(scene);
                    }
                    else
                    {
                        property.stringValue = scene.name;
                    }
                }
            }
        }
#endif
    }
}