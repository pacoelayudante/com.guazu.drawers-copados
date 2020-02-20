// No esta funcionando como se espera al usar arrays (muestra array y oculta los children mas bien)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Guazu.DrawersCopados
{

    public class OcultadorAttribute : PropertyAttribute
    {
        string nombreData;
        double numeroMin, numeroMax;
        bool xorInverter;
#if UNITY_EDITOR
        public SerializedProperty GetProperty(SerializedProperty propHermana)
        {
            return propHermana.FindPropertyRelative(nombreData);
        }
        public SerializedProperty GetProperty(SerializedObject objContenedor)
        {
            return objContenedor.FindProperty(nombreData);
        }
        public bool Comp(SerializedProperty prop)
        {
            if (prop == null) return true;//El default es true porqu si  todo  falla queremos que la  cosa  sea visible
            if (prop.propertyType == SerializedPropertyType.Boolean) return prop.boolValue ^ xorInverter;
            else if (prop.propertyType == SerializedPropertyType.Enum) return Comp(prop.enumValueIndex) ^ xorInverter;
            else if (prop.propertyType == SerializedPropertyType.Integer) return Comp(prop.intValue) ^ xorInverter;
            else if (prop.propertyType == SerializedPropertyType.Float) return Comp(prop.floatValue) ^ xorInverter;
            else if (prop.propertyType == SerializedPropertyType.String) return Comp(prop.stringValue) ^ xorInverter;
            else if (prop.propertyType == SerializedPropertyType.ObjectReference) return (prop.objectReferenceValue != null) ^ xorInverter;
            return true;//El default es true porqu si  todo  falla queremos que la  cosa  sea visible
        }
        bool Comp(double valor)
        {
            if (valor >= numeroMin) return valor <= numeroMax;
            else return false;
        }
        bool Comp(string valor)
        {
            return !string.IsNullOrEmpty(valor);
        }
#endif
        public OcultadorAttribute(string nombreData, bool negarValorBool = false)
        {
            this.nombreData = nombreData;
            this.xorInverter = negarValorBool;
        }
        public OcultadorAttribute(string nombreData, double numero, bool negarValorBool = false) : this(nombreData, numero, numero, negarValorBool) { }
        public OcultadorAttribute(string nombreData, double numeroMenor, double numeroMayor, bool negarValorBool = false)
        {
            this.nombreData = nombreData;
            this.numeroMin = numeroMenor;
            this.numeroMax = numeroMayor;
            this.xorInverter = negarValorBool;
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(OcultadorAttribute))]
        public class OcultadorAttributeDrawer : PropertyDrawer
        {
            GUIContent propDeControlNoHallada = new GUIContent(EditorGUIUtility.FindTexture("console.warnicon.inactive.sml")
                , "No se encuentra el campo de referencia. Puede que haya cambiado el nombre de la variable, o que tenga un error de tipeo.");

            float Visibilidad
            {
                get
                {
                    if (visibilidad == null) return 1f;
                    else return visibilidad.target ? 1f : 0f;
                }

            }
            OcultadorAttribute ocultador;
            SerializedProperty propControl;
            UnityEditor.AnimatedValues.AnimBool visibilidad;

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (ocultador == null)
                {
                    ocultador = attribute as OcultadorAttribute;
                    propControl = ocultador.GetProperty(property.serializedObject);
                    visibilidad = new UnityEditor.AnimatedValues.AnimBool(ocultador.Comp(propControl));
                    // HACER: Animacion de esto, aunque no se si se puede siendo que es un attributedrawer 
                    // visibilidad.valueChanged += 
                }
                else visibilidad.target = ocultador.Comp(propControl);
                if (propControl == null)
                {
                    EditorGUI.LabelField(new Rect(position.x - EditorGUIUtility.singleLineHeight, position.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight), propDeControlNoHallada);
                }

                if (visibilidad.target) EditorGUI.PropertyField(position, property);
            }
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return EditorGUI.GetPropertyHeight(property) * Visibilidad;
            }
        }
#endif
    }
}