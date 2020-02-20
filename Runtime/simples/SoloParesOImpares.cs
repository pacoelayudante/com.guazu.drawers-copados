using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

namespace Guazu.DrawersCopados
{
    public class SoloParesAttribute : PropertyAttribute, ISoloParesOImpares
    {
        public bool Pares() => true;
    }
    public class SoloImparesAttribute : PropertyAttribute, ISoloParesOImpares
    {
        public bool Pares() => false;
    }
    public interface ISoloParesOImpares
    {
        bool Pares();
    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SoloParesAttribute))]
    [CustomPropertyDrawer(typeof(SoloImparesAttribute))]
    public class SoloParesOImparesDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                base.OnGUI(position, property, label);
                return;
            }
            var debeSerPar = (attribute as ISoloParesOImpares).Pares();
            var valorInicial = property.intValue;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);
            if (EditorGUI.EndChangeCheck())
            {
                if (property.intValue % 2 == 0 ^ debeSerPar)
                {
                    property.intValue += (property.intValue > valorInicial ? +1 : -1);
                }
            }
        }
    }
#endif
}