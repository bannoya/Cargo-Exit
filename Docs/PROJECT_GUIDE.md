# Cargo Exit — guía del proyecto

Esta guía complementa al GDD. El GDD decide **qué** juego construimos; este
documento recuerda **dónde** debe vivir cada tipo de cambio.

## Abrir y ejecutar

1. Abrir la carpeta `game` desde Unity Hub con Unity `6000.3.20f1`.
2. En Unity, elegir **Cargo Exit > Open Main Scene**.
3. Abrir la pestaña **Game**.
4. Seleccionar una relación vertical `9:16`.
5. La pantalla completa debe verse aun sin Play Mode.
6. Presionar **Play** para probar el arrastre.

Escena vigente:

`Assets/CargoExit/Scenes/CargoExit.unity`

Si la escena se borró o quedó dañada, usar **Cargo Exit > Rebuild Main Scene**.
Esa opción reemplaza la escena, por lo que sólo debe usarse cuando realmente se
quiera reconstruir la vista desde el código de autoría.

Si sólo falta una fuente o algún texto se ve mal, usar **Cargo Exit > Apply
Project Fonts**. Esta herramienta cambia únicamente las referencias
tipográficas de la escena y conserva posiciones, tamaños, colores y demás
ajustes manuales.

La interfaz usa una referencia lógica de `540 × 960`, coordenadas enteras,
Canvas alineado a píxeles y texto SDF mediante TextMesh Pro. Ningún texto
jugable del prototipo baja de 20 unidades. Usar **Cargo Exit > Apply Project
Typography** para restaurar la escala sin reconstruir paneles ni contenido.
No se vuelve a usar el componente heredado `UnityEngine.UI.Text`.

## Estructura física

```text
Assets/CargoExit/
├── Art/
│   ├── Fonts/
│   └── TextMeshPro/
├── Scenes/
└── Scripts/
    ├── Core/
    ├── Presentation/
    ├── Editor/
    └── Tests/
        ├── EditMode/
        └── PlayMode/
```

Esta es la convención estable del proyecto:

- los recursos visuales viven en `Art`;
- las escenas viven en `Scenes`;
- todo el código vive en `Scripts`;
- cada responsabilidad mantiene su carpeta y su módulo independiente;
- una carpeta nueva se crea cuando contiene una responsabilidad o un recurso
  real, no para anticipar sistemas futuros.

## Capas y dependencias

```text
Core  <-  Presentation  <-  Editor
  ^              ^
  |              |
EditMode       PlayMode
 Tests          Tests
```

### Core

Contiene reglas C# puras:

- destinos;
- manifiesto de cajas;
- aceptación o rechazo de una clasificación;
- progreso de una tanda.

No usa `UnityEngine`. No conoce colores, animaciones, escenas, botones ni
coordenadas de pantalla.

### Presentation

Contiene:

- vista de cajas;
- entrada táctil;
- animaciones;
- unión entre la escena y Core;
- creación visual provisional.

Puede consultar Core, pero no inventar reglas económicas o logísticas dentro de
una animación.

### Editor

Contiene herramientas que sólo existen dentro de Unity:

- abrir la escena principal;
- reconstruir la escena visible;
- reparar las referencias de las fuentes;
- preparar la lista de escenas de compilación.

Nunca se referencia desde Core o desde el juego ejecutable.

### Tests

- **EditMode:** prueban reglas de Core sin abrir escenas.
- **PlayMode:** prueban que la escena principal se abra y cree la interfaz
  jugable, que use exclusivamente TextMesh Pro y que mantenga la escala
  tipográfica mínima.

Cuando aparezca un bug, primero se identifica la capa:

- resultado incorrecto: Core;
- caja que no sigue el dedo: Presentation;
- escena vacía o mal generada: Editor/autoría;
- referencia perdida al cargar: integración PlayMode.

## Regla de crecimiento

Una función nueva sólo entra al proyecto cuando:

1. responde a una sección del GDD;
2. tiene una responsabilidad clara;
3. respeta la dirección de dependencias;
4. posee una prueba proporcional al riesgo;
5. no reactiva un prototipo descartado.

No se agregan sistemas futuros “por si acaso”.

## Fuente de verdad

- Diseño: `Docs/GAME_DESIGN.md`
- Marca: `Docs/BRAND.md`
- Trabajo técnico: este documento
- Escena vigente: `Assets/CargoExit/Scenes/CargoExit.unity`
- Fuente de interfaz: `Assets/CargoExit/Art/Fonts/AtkinsonHyperlegibleNext`
- Recursos SDF: `Assets/CargoExit/Art/TextMeshPro`
