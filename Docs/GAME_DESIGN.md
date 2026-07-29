# Cargo Exit — diseño v0.1

## Visión

Cargo Exit es un puzle móvil de carga y reparto. El jugador debe acomodar paquetes dentro de una camioneta. Resolver el espacio es solo la primera mitad: al llegar a cada parada, el paquete correcto debe tener un camino libre hacia la puerta.

La fantasía es convertirse en una persona experta en logística, no administrar una economía ni esperar temporizadores.

## Objetivos

- Comprensible en menos de un minuto.
- Niveles de 1–4 minutos.
- Reinicio instantáneo y deshacer sin límites.
- Satisfacción visual al cargar y descargar.
- Progresión que cambia la forma de pensar.
- Juego completo sin conexión, anuncios, compras ni cuenta.

## Plataforma

- Android.
- Pantalla vertical.
- Interacción táctil como control principal.
- Mouse disponible durante el desarrollo.
- Objetivo inicial de 60 fps en teléfonos de gama media.

## Bucle

1. Ver la ruta y los paquetes.
2. Arrastrar y rotar paquetes en la cuadrícula del vehículo.
3. Pulsar **Entregar**.
4. Ver cómo se descargan en orden.
5. Si un paquete está bloqueado, volver inmediatamente al armado con el problema señalado.
6. Completar el contrato y avanzar.

## Reglas del vertical slice

- Tablero rectangular con la puerta en el borde inferior.
- Paquetes formados por una o más celdas.
- No pueden salir del tablero ni superponerse.
- Cada paquete tiene un número de entrega.
- Para descargarlo, todas sus columnas deben quedar libres hasta la puerta.
- Los paquetes descargados dejan espacio libre para los siguientes.

## Progresión propuesta

La campaña se organiza por zonas. Cada una introduce una idea y luego combina lo aprendido.

1. **El depósito:** colocar, rotar, deshacer y entender la puerta.
2. **El centro:** varias paradas y orden de descarga.
3. **El mercado:** paquetes frágiles y pesados.
4. **La cadena fría:** refrigerados y acumuladores de frío.
5. **La mudanza:** objetos grandes y uso limitado de herramientas.

El vertical slice solo implementa las zonas 1 y 2. Las demás no se producen hasta comprobar que el núcleo es divertido.

## Habilidades y herramientas

No habrá potenciadores abstractos. Toda habilidad debe representar una decisión logística:

- **Plan de ruta:** permite intercambiar dos paradas adyacentes una vez en niveles diseñados para ello.
- **Carretilla:** permite retirar y recolocar un paquete durante una entrega.
- **Cincha:** asegura una carga frágil cuando una regla del nivel lo requiere.

Son posibilidades para prototipar, no funciones aprobadas. Una herramienta se incorpora únicamente si crea decisiones interesantes y no resuelve el puzle por el jugador.

## Progreso

- Completar un nivel desbloquea el siguiente; no se exige repetir para avanzar.
- Las tres estrellas miden dominio y son opcionales.
- No hay moneda, experiencia numérica, energía ni calendario obligatorio.
- Los niveles perfectos pueden desbloquear elementos cosméticos y pequeños secretos de Bannoya's Games.

## Vertical slice

Debe contener:

- Seis niveles.
- Dos formas de paquete.
- Rotación, arrastre, deshacer y reinicio.
- Simulación de entrega con un caso exitoso y uno bloqueado.
- Una pantalla de selección mínima.
- Sonido y vibración configurables.
- Un momento pequeño de personalidad de marca al completar el último nivel.

No contiene:

- Arte definitivo.
- Herramientas especiales.
- Mapa de campaña completo.
- Localización.
- Servicios de red.
- Publicidad, compras o analítica remota.

## Criterios para aprobar el núcleo

En una prueba con ocho personas:

- seis completan el tutorial sin explicación verbal;
- cinco juegan voluntariamente más de tres niveles;
- el tiempo mediano de sesión supera ocho minutos;
- al menos cuatro quieren otro capítulo;
- los errores se atribuyen a decisiones propias, no a controles confusos.

