using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using an = UnityEditor.Animations;
#endif

// el "Tipo que Listar" indica si busca los parametros o todos los estados del AnimatorController
// el "buscar en children" indica que se le permite buscar en sus hijos algun Animator (en vez de solo en si mismo)
// el "buscar campo especifico" se fija en ese mismo script un campo Animator con ese nombre, y levanta eso

// ejemplitos
/*

[AnimatorStringListAttribute(Tipo.Parametros)]
public string animBoolEstaAgachado;

[AnimatorStringListAttribute(Tipo.Parametros)]
public string floatNivelDePoder;

[AnimatorStringListAttribute(Tipo.Estados)]
public string animacionDeMuerte;

void Update() {
    miAnimator.SetBool(animBoolEstaAgachado, Character.estaAgachado);
    miAnimator.SetFloat(floatNivelDePoder, Character.poder);
}
void AlMorir(){
    animator.Play(animacionDeMuerte);
}
===============================================

Animator miSkinAnimator;//el skin es un objeto hijo

[AnimatorStringListAttribute(Tipo.Parametros, true)]
public string animBoolEstaAgachado; // el 'true' significa que busca en si mismo o en sus children el AnimatorController

[AnimatorStringListAttribute(Tipo.Parametros, true)] 
public string floatNivelDePoder;

===============================================
public Animator manosAnimator;
public Animator cabezaAnimator;

[AnimatorStringListAttribute(Tipo.Parametros, "manosAnimator")]
public string animBoolManosAbiertas; // el parametro pasado indica que busque el AnimatorController vinculado a ese field

[AnimatorStringListAttribute(Tipo.Parametros, "cabezaAnimator")]
public string animacionDeExplotarCabeza; // el parametro pasado indica que busque el AnimatorController vinculado a ese field

void Update() {
    manosAnimator.SetBool(animBoolManosAbiertas, Character.manosAbiertas);
}
void AlMorir(){
    cabezaAnimator.Play(animacionDeExplotarCabeza);
}

 */

namespace Guazu.DrawersCopados
{

    public class AnimatorStringListAttribute : PropertyAttribute
    {
        bool buscarEnChildren = false;
        string buscarCampoEspecifico = "";
        public enum Tipo { Parametros, Estados, Todo }
        Tipo queListar;

        public AnimatorStringListAttribute(Tipo queListar) { this.queListar = queListar; }
        public AnimatorStringListAttribute(Tipo queListar, bool buscarEnChildren) : this(queListar) { this.buscarEnChildren = buscarEnChildren; }
        public AnimatorStringListAttribute(Tipo queListar, string buscarCampoEspecifico) : this(queListar) { this.buscarCampoEspecifico = buscarCampoEspecifico; }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(AnimatorStringListAttribute))]
        public class AnimatorStringListAttributeDrawer : PropertyDrawer
        {
            readonly static GUIContent[] noMulti = new GUIContent[] { new GUIContent("Multiobject editing no permitido") };
            readonly static GUIContent[] noAnimator = new GUIContent[] { new GUIContent("Animator no hallado o hallado pero sin Animator Controller") };
            //readonly static GUIContent[] noAnimCont = new GUIContent[] { new GUIContent("Animator sin Animator Controller") };
            readonly static GUIContent[] noField = new GUIContent[] { new GUIContent("El campo que se quiere referenciar no se encuentra en este componente") };
            //readonly static GUIContent[] noFieldGameObj = new GUIContent[] { new GUIContent("El campo que se quiere referenciar debe ser un vinculo a un objeto") };
            readonly static GUIContent[] noBehav = new GUIContent[] { new GUIContent("No reconocido como behaviour") };
            readonly static GUIContent[] noGameObj = new GUIContent[] { new GUIContent("No asociado a Objeto") };
            static GUIContent[] errorNoAnimator;

            static an.AnimatorController RecuperarAnimator(SerializedProperty property, AnimatorStringListAttribute att, Behaviour beh)
            {
                errorNoAnimator = null;
                if (string.IsNullOrEmpty(att.buscarCampoEspecifico))
                {
                    Animator animator = beh.GetComponent<Animator>();
                    if (!animator)
                    {
                        if (att.buscarEnChildren) animator = beh.GetComponentInChildren<Animator>();
                    }
                    if (animator)
                    {
                        if (animator.runtimeAnimatorController is AnimatorOverrideController) return (animator.runtimeAnimatorController as AnimatorOverrideController).runtimeAnimatorController as an.AnimatorController;
                        else return animator.runtimeAnimatorController as an.AnimatorController;
                    }
                    return null;
                }
                else
                {
                    SerializedProperty prop = property.serializedObject.FindProperty(att.buscarCampoEspecifico);
                    if (prop == null)
                    {
                        errorNoAnimator = noField;
                        return null;
                    }
                    else if (prop.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        Object elOtro = prop.objectReferenceValue;
                        if (!elOtro) return null;
                        an.AnimatorController esController = elOtro as an.AnimatorController;
                        if (esController) return esController;
                        else if (elOtro is AnimatorOverrideController) return (elOtro as AnimatorOverrideController).runtimeAnimatorController as an.AnimatorController;
                        Animator esAnimator = elOtro as Animator;
                        if (!esAnimator)
                        {
                            GameObject esGameObject = elOtro as GameObject;
                            if (esGameObject) esAnimator = esGameObject.GetComponent<Animator>();
                            else
                            {
                                Component esComponent = elOtro as Component;
                                if (esComponent) esAnimator = esComponent.GetComponent<Animator>();
                            }
                        }
                        if (esAnimator)
                        {
                            if (esAnimator.runtimeAnimatorController is AnimatorOverrideController) return (esAnimator.runtimeAnimatorController as AnimatorOverrideController).runtimeAnimatorController as an.AnimatorController;
                            else return esAnimator.runtimeAnimatorController as an.AnimatorController;
                        }
                        else return null;
                    }
                    else
                    {
                        errorNoAnimator = noGameObj;
                        return null;
                    }
                }
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var attributoComoEsto = attribute as AnimatorStringListAttribute;
                if (property.propertyType == SerializedPropertyType.String)
                {
                    position.width -= EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(position, property);
                    position.x += position.width;
                    position.width = EditorGUIUtility.singleLineHeight;
                    if (property.serializedObject.isEditingMultipleObjects)
                    {
                        EditorGUI.Popup(position, -1, noMulti);
                    }
                    else if (property.serializedObject.targetObject)
                    {
                        Behaviour beh = property.serializedObject.targetObject as Behaviour;
                        if (!beh && string.IsNullOrEmpty(attributoComoEsto.buscarCampoEspecifico))
                        {
                            EditorGUI.Popup(position, -1, noBehav);
                        }
                        else if (!string.IsNullOrEmpty(attributoComoEsto.buscarCampoEspecifico) || beh.gameObject)
                        {
                            an.AnimatorController animatorController = RecuperarAnimator(property, attributoComoEsto, beh);

                            if (animatorController)
                            {
                                string[] estadosDelAnim = new string[0];
                                string[] parametrosDelAnim = new string[0];
                                if (attributoComoEsto.queListar == Tipo.Estados || attributoComoEsto.queListar == Tipo.Todo) estadosDelAnim = ListaDeEstados(animatorController);
                                if (attributoComoEsto.queListar == Tipo.Parametros || attributoComoEsto.queListar == Tipo.Todo) parametrosDelAnim = ListaDeVariables(animatorController);

                                ArrayUtility.AddRange(ref estadosDelAnim, parametrosDelAnim);
                                var anims = estadosDelAnim;
                                {
                                    int sel = 0;
                                    for (sel = 0; sel < anims.Length; sel++) { if (anims[sel].Equals(property.stringValue)) break; }
                                    EditorGUI.BeginChangeCheck();
                                    sel = EditorGUI.Popup(position, sel, anims);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        property.stringValue = anims[sel];
                                        property.serializedObject.ApplyModifiedProperties();
                                    }
                                }
                            }
                            else EditorGUI.Popup(position, -1, errorNoAnimator == null ? noAnimator : errorNoAnimator);
                        }
                        else EditorGUI.Popup(position, -1, noGameObj);
                    }
                }
                else
                {
                    EditorGUI.LabelField(position, label, "AnimatorStateString solo con strings");
                }
            }
        }

        public static string[] ListaDeVariables(Animator anim)
        {
            if (anim)
            {
                if (anim.runtimeAnimatorController)
                {
                    return ListaDeVariables(anim.runtimeAnimatorController as an.AnimatorController);
                }
                else return null;
            }
            else return null;
        }
        public static string[] ListaDeVariables(an.AnimatorController cont)
        {
            if (cont)
            {
                string[] salida = new string[cont.parameters.Length];
                for (int i = 0; i < salida.Length; i++)
                {
                    salida[i] = cont.parameters[i].name;
                }
                return salida;
            }
            else return null;
        }

        public static string[] ListaDeEstados(Animator anim)
        {
            if (anim)
            {
                if (anim.runtimeAnimatorController)
                {
                    return ListaDeEstados(anim.runtimeAnimatorController as an.AnimatorController);
                }
                else return null;
            }
            else return null;
        }
        public static string[] ListaDeEstados(an.AnimatorController cont)
        {
            if (cont)
            {
                List<string> resultados = new List<string>();
                foreach (an.AnimatorControllerLayer lay in cont.layers)
                {
                    AgregarEstadosRecursivo(resultados, lay.stateMachine);
                }
                return resultados.ToArray();
            }
            else return null;
        }
        public static void AgregarEstadosRecursivo(List<string> lista, an.AnimatorStateMachine stMach)
        {
            foreach (an.ChildAnimatorState stat in stMach.states)
            {
                lista.Add(stMach.name + "." + stat.state.name);
            }
            foreach (an.ChildAnimatorStateMachine stat in stMach.stateMachines)
            {
                AgregarEstadosRecursivo(lista, stat.stateMachine);
            }
        }
#endif
    }
}