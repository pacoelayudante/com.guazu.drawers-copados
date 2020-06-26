_Unity Game Engine Package_
# Drawers Copados
#### _Paco Álvarez Lojo de [Team Guazú](https://teamguazu.com/)_

Un paquete de Custom Attribute Drawers para ser mas feliz. Mas abajo explico como _instalarlo_ usando el [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui.html) (versiones mayor a `2018.1`)

## ¿Attribute Drawers?

Los [Attribute Drawers](https://docs.unity3d.com/ScriptReference/PropertyDrawer.html) son atributos que se agregan a los scripts de Unity para modificar la forma en que se ven algunas variables en el Inspector sin tener que escribir un CustomEditor para el script. Tambien pueden utilizarse para agregar algunas cosas al Inspector de un script sin estar asociado a una variable especificamente.

### ¿Como?

Para agregar un atributo es muy sencillo: Ejemplo
[RangeAttribute](https://docs.unity3d.com/ScriptReference/RangeAttribute.html)
```csharp
// este atributo (nativo de unity) muestra la siguiente variable como un slider en el inspector
[Range(1,3)]
public float valorEntreUnoHastaTres = 1.5f;
```

## ¿Cuales?

Armé tres categorias: simples, complejos y para shaders, pero eso da igual la verdad

A los drawers mas **notables** les voy a poner un ❧ al lado

#### Complejos
- **`[AnimatorStringList]`** ❧ _muestra de un Animator que sea en base a alguna variable, o que este en el mismo gameobject la lista de estados y o parametros que tiene el Animator Controller asignado, y lo escribe como string_

#### Shaders
 
- **`[MaskFlags]`** ❧ _en el material editor que use este shader, este valor se va a ver como una serie de toggles que responden al valor bitwise de la variable_
- `[ToggleZeroMaterial]` _Un slider que cuando la variable es == 0, le setea al material una variable de compilacion_

#### Simples
- **`[SingleLayer]`** ❧ _Muestra un menu dropdown para elegir un layer, se diferencia de LayerMask, porque es un layer especifico. Solo ints!_
- `[AttrDrawBotonPegador]` _pone un boton que pega lo que tengas en el clipboard aca. Solo strings!_
- `[CopyCurrentFolder]` _copia en el clipboard el path de la carpeta seleccionada actualmente en el project_
- **`[CreameScriptable]`** ❧ _puesto en campo que apunta a un objeto de tipo ScriptableObject, si ese campo esta vacio, te aparece un boton para crear un scriptable de ese mismo tipo y lo asigna a ese campo_
- `[GlobalShader]` _Este no afecta ninguna variable, pero hace visible en el inspector un slider que modifica el valor de un uniform global de shader_
- **`[Ocultador]`** ❧ _A este se le pasa como parametro otra variable del script, y va a ocultar la variable decorada si se cumplen ciertas condiciones basicas, tiene varios parametros diferentes para pasarle_
- **`[SceneAssetPathAsString]`** ❧ _simple, un string lo muestra en el inspector como un selector de objetos que toma Scenes del proyecto (el valor asignado es el path de la escena, no el asset de la escena)_
- `[SoloPares]` _fuerza los valores a ser pares... no es mucho pero que se yo_
- `[SoloImpares]` _solo valores impares_,

## Instalación usando el UPM
Abrir `<project>/Packages/manifest.json` y agregar el scope para los paquetes de guazu.
```json
{
  "scopedRegistries": [
    {
      "name": "Paquetes de Guazu",
      "url": "https://registry.npmjs.org/",
      "scopes": [
        "com.guazu"
      ]
    }
    // ...
  ],
  "dependencies": {
    // ...
  }
}
```

Luego en el Unity Editor abrir `Window > Package Manager` y podras ver los paquetes de Guazu listos para ser instalados