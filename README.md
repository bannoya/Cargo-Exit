# Cargo Exit

Primer juego de **Bannoya's Games**.

Cargo Exit es un juego de gestión táctil para Android. El jugador comienza
ordenando cajas en un pequeño depósito y termina administrando empleados,
pallets, camiones, contratos y el futuro de la empresa.

La campaña es finita y puede terminar en quiebra, legado familiar o venta con
jubilación. Cada responsabilidad utiliza el mismo lenguaje táctil de tocar,
arrastrar y encastrar.

## Principios

- Gratis, sin anuncios y sin compras.
- Una acción táctil agradable con decisiones logísticas comprensibles.
- Crecimiento visible mediante personas, procesos, infraestructura y flota.
- Sin vidas, energía, rachas obligatorias ni castigos por dejar de jugar.
- Funciona sin cuenta y sin conexión.
- Alcance pequeño, validación temprana y campaña con final.

## Estado

Preproducción. El prototipo actual valida la clasificación de cajas desde una
pila central hacia pallets periféricos. El próximo vertical slice conectará
asignación de empleados, carga y despacho dentro de una jornada.

## Abrir el prototipo

1. Abrir el proyecto `game` con Unity 6.3 LTS.
2. Usar el menú **Cargo Exit > Open Main Scene**.
3. Abrir la pestaña **Game** y elegir una relación vertical `9:16`.
4. La composición puede verse sin ejecutar; presionar **Play** para arrastrar.

También se puede abrir directamente
`Assets/CargoExit/Scenes/CargoExit.unity`. Es la única escena activa del
proyecto.

## Estructura

```text
Assets/CargoExit/
├── Art/
│   └── Fonts/
├── Scenes/
└── Scripts/
    ├── Core/
    ├── Presentation/
    ├── Editor/
    └── Tests/
        ├── EditMode/
        └── PlayMode/
```

- `Art`: fuentes, imágenes, materiales, modelos y demás recursos visuales.
- `Scenes`: una sola escena vigente.
- `Scripts/Core`: reglas C# independientes de Unity.
- `Scripts/Presentation`: vista, entrada, animaciones y coordinación.
- `Scripts/Editor`: herramientas de autoría; no entra al juego final.
- `Scripts/Tests/EditMode`: pruebas rápidas de reglas.
- `Scripts/Tests/PlayMode`: pruebas de la escena y la interacción.
- `Docs`: GDD, identidad y guía de trabajo.

La dirección de dependencias es `Core <- Presentation <- Editor`. Core nunca
conoce la interfaz ni las herramientas de Unity.

La interfaz usa **Atkinson Hyperlegible Next** en variantes Regular y Bold. Si
alguna referencia de fuente se pierde, usar **Cargo Exit > Apply Project
Fonts**: actualiza únicamente las fuentes y conserva el resto de la escena.
Los tamaños se diseñan para un Canvas móvil de `1080 × 1920`, con un mínimo
provisional de 40 unidades. **Cargo Exit > Apply Project Typography** restaura
esa escala si un texto queda demasiado pequeño.

## Tecnología

- Unity 6.3 LTS (`6000.3.20f1`)
- C#
- Orientación vertical
- Android como plataforma principal
- Reglas separadas de la presentación

La visión, los sistemas y el alcance vigente están en
[Docs/GAME_DESIGN.md](Docs/GAME_DESIGN.md).

Las instrucciones para trabajar sin mezclar responsabilidades están en
[Docs/PROJECT_GUIDE.md](Docs/PROJECT_GUIDE.md).
