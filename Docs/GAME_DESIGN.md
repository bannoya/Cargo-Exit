# Cargo Exit — Game Design Document

**Estudio:** Bannoya's Games

**Versión:** 0.2

**Fecha:** 29 de julio de 2026

**Estado:** preproducción; núcleo táctil en validación

**Plataforma principal:** Android, orientación vertical

**Modelo comercial:** juego completo gratuito, sin anuncios ni compras

---

## 0. Cómo usar este documento

Este GDD es la fuente de verdad de diseño de Cargo Exit. Debe ayudarnos a tomar decisiones, construir versiones pequeñas y detectar cuándo una idea atractiva amenaza con agrandar demasiado el proyecto.

Las afirmaciones importantes usan uno de estos estados:

- **Acordado:** define la identidad actual del juego. No se cambia sin una razón comprobable.
- **Provisional:** es una dirección concreta que todavía necesita balance o pruebas.
- **Por validar:** es una hipótesis; puede desaparecer si no mejora la experiencia.

Los números de economía, duración y contenido son provisionales hasta probarlos con jugadores. Los pilares, el modelo ético y la estructura general de la experiencia se consideran acordados.

---

## 1. Visión del producto

### 1.1 Concepto en una frase

**Cargo Exit es un juego de gestión táctil para móviles en el que el jugador transforma el caos de un pequeño depósito en una empresa logística, tomando decisiones físicas y económicas hasta afrontar su quiebra, dejar un legado familiar o vender la compañía y jubilarse.**

### 1.2 Presentación breve

El jugador comienza trabajando con sus propias manos: toma cajas desordenadas del centro del depósito y las arrastra hacia el pallet correcto. La acción debe ser inmediata, suave y satisfactoria, como una interacción que uno quiere repetir.

Con el crecimiento de la empresa aparecen nuevas responsabilidades. El jugador asigna empleados según sus aptitudes, decide cómo formar pallets, elige qué camiones y contratos aceptar, resuelve imprevistos y reinvierte lo ganado. Las tareas dominadas pueden delegarse; las excepciones y las decisiones importantes siempre permanecen en manos del jugador.

La campaña es finita. Cada jornada representa un momento importante dentro de aproximadamente veinte años de empresa. Las decisiones acumuladas determinan no sólo cuánto dinero se obtuvo, sino qué clase de compañía se construyó y quién se queda con ella al final.

### 1.3 Fantasía del jugador

> “Pongo orden donde había caos, hago que una operación compleja funcione con mis propias decisiones y termino construyendo una empresa que significa algo.”

La fantasía no es ser una persona que pulsa botones desde una oficina. El jugador debe sentir la mercadería, comprender el depósito y conocer al equipo antes de convertirse en gerente. El crecimiento se expresa como una transición desde **hacer**, hacia **organizar**, luego **delegar** y finalmente **decidir el futuro**.

### 1.4 Diferenciador

Cargo Exit combina tres elementos que deben sentirse inseparables:

1. Una interacción táctil repetible y placentera.
2. Decisiones logísticas fáciles de leer pero con consecuencias.
3. Una campaña empresarial finita con finales distintos.

Si una función sólo pertenece a uno de estos elementos y no refuerza a los demás, se reconsidera.

### 1.5 Pregunta central de diseño

> ¿Podemos hacer que ordenar, asignar y despachar con un solo gesto siga siendo agradable mientras las decisiones cambian y la empresa crece?

El prototipo actual responde únicamente a la primera parte: arrastrar cajas desde una pila central hacia pallets periféricos se siente prometedor. El próximo prototipo debe comprobar que esa acción también puede sostener decisiones de gestión.

---

## 2. Ficha del juego

| Dato | Definición |
|---|---|
| Título | Cargo Exit |
| Estudio | Bannoya's Games |
| Género | Gestión táctil, puzle logístico y estrategia ligera |
| Modo | Un jugador |
| Plataforma | Android |
| Orientación | Vertical |
| Cámara | Vista 2D cenital o semicenital, adaptada a una mano |
| Controles | Tocar, arrastrar y soltar; mouse durante el desarrollo |
| Sesión objetivo | 3–6 minutos por jornada decisiva |
| Campaña objetivo | 30–40 jornadas; duración total por validar |
| Conectividad | Funciona sin conexión |
| Cuenta | No requerida |
| Monetización | Ninguna |
| Idioma inicial | Español |
| Idioma posterior | Inglés, después de estabilizar todos los textos |
| Motor | Unity 6.3 LTS, C# |
| Identificador técnico | `com.bannoyasgames.cargoexit` |

---

## 3. Pilares de diseño

### 3.1 Tacto satisfactorio

Mover una caja tiene que resultar agradable incluso antes de que exista una decisión difícil. La selección, el peso visual, la atracción, el encastre, el sonido y la vibración forman parte de la mecánica, no del decorado.

**Prueba del pilar:** una persona debería querer ordenar otra tanda aunque no reciba una recompensa externa.

### 3.2 Logística con sentido

Las reglas deben surgir de objetos y trabajos reconocibles:

- lo pesado necesita fuerza y va debajo;
- lo frágil necesita cuidado y va protegido;
- lo refrigerado no puede romper la cadena de frío;
- lo urgente debe salir antes;
- un camión tiene capacidad y equipamiento concretos;
- un empleado no es intercambiable con cualquier otro.

No se incorporan poderes abstractos que contradigan el mundo.

### 3.3 Crecimiento visible

La empresa debe cambiar ante los ojos del jugador:

- más espacio y estaciones;
- pallets y camiones nuevos;
- empleados conocidos;
- tareas que antes eran manuales y luego se delegan;
- clientes más grandes;
- un depósito que pasa de improvisado a profesional.

La progresión no puede consistir únicamente en números que suben.

### 3.4 Decisiones con consecuencias legibles

El jugador debe poder explicar por qué una jornada salió bien o mal. La interfaz muestra causas concretas: daño, demora, mala asignación, falta de capacidad, deuda o un contrato mal elegido.

El azar puede crear situaciones, pero no debe decidir por sí solo el resultado.

### 3.5 Una historia con final

La empresa no existe para siempre. El paso del tiempo y los tres desenlaces convierten cada mejora en parte de una historia:

- perder la empresa por insolvencia;
- preparar un legado familiar sostenible;
- construir una compañía atractiva, venderla y jubilarse.

La herencia familiar es una victoria distinta, no un premio de consolación.

### 3.6 Respeto por quien juega

- Sin anuncios.
- Sin compras.
- Sin vidas ni energía.
- Sin recompensas diarias, miedo a perderse algo o rachas obligatorias.
- Sin castigos por dejar de jugar.
- Sin cuenta ni conexión forzosa.
- Sin analítica remota en la primera versión.
- Pausa, repetición y accesibilidad desde el comienzo.

---

## 4. Lo que Cargo Exit no es

Definir estos límites evita que el proyecto pierda identidad.

- No es un idle game que progresa mientras el jugador espera.
- No es un simulador realista de logística industrial.
- No es un juego infinito como servicio.
- No es una colección de minijuegos desconectados.
- No es una planilla de cálculo disfrazada.
- No es un juego de velocidad extrema o reflejos puros.
- No es un producto creado para mostrar anuncios.
- No necesita multijugador, servidores, cuentas, eventos temporales ni tablas globales.
- No necesita una ciudad explorable ni personajes 3D para contar su historia.

---

## 5. Público y experiencia objetivo

### 5.1 Público principal

Personas que usan Android y disfrutan:

- ordenar, clasificar y encastrar objetos;
- juegos de gestión accesibles;
- optimizar sistemas sin estudiar manuales;
- sesiones cortas con progreso permanente;
- una campaña que pueden terminar.

No se exige conocimiento previo de logística ni experiencia con juegos de estrategia.

### 5.2 Curva emocional buscada

Cada jornada debe recorrer una secuencia sencilla:

1. **Caos comprensible:** hay trabajo por hacer, pero el problema se puede leer.
2. **Concentración:** el jugador entra en ritmo al mover y asignar.
3. **Dominio:** descubre una mejor forma de organizar la operación.
4. **Alivio:** el camión sale y el depósito queda ordenado.
5. **Orgullo o reflexión:** el informe conecta el desempeño con la empresa.
6. **Anticipación:** una mejora, un contrato o un evento plantea la próxima decisión.

### 5.3 Duración

- **Jornada inicial:** 2–4 minutos.
- **Jornada avanzada:** 4–6 minutos.
- **Decisión entre jornadas:** menos de 90 segundos.
- **Campaña completa:** provisionalmente 4–6 horas.

Una jornada puede suspenderse y continuar. Ninguna pantalla debe obligar a jugar “una más”.

---

## 6. Estructura general

Cargo Exit funciona en tres escalas conectadas.

### 6.1 Escala inmediata: el gesto

1. Leer una caja, persona, pallet o camión.
2. Tocar y levantar visualmente el elemento.
3. Arrastrarlo hacia un destino.
4. Percibir atracción al acercarse a una opción válida.
5. Soltar.
6. Recibir una respuesta inmediata y comprensible.

### 6.2 Escala de jornada: la operación

1. Revisar el contrato y la carga.
2. Preparar el turno y asignar personal.
3. Clasificar y procesar mercadería.
4. Formar pallets.
5. Elegir y cargar camiones.
6. Despachar.
7. Revisar resultados.

Las primeras jornadas contienen sólo algunas fases. Las jornadas avanzadas pueden recorrer todas, pero deben mantenerse ágiles mediante delegación y automatización.

### 6.3 Escala de campaña: la empresa

1. Elegir un contrato o una prioridad.
2. Operar una jornada decisiva.
3. Cobrar, pagar costos y asumir consecuencias.
4. Tomar una decisión de personas, procesos, infraestructura o flota.
5. Avanzar en el tiempo.
6. Afrontar un nuevo desafío.
7. Llegar a la sucesión, venta o quiebra.

---

## 7. El verbo principal

### 7.1 Una misma acción, decisiones diferentes

No se agregará otra mecánica principal. El juego reutiliza el mismo lenguaje táctil en contextos distintos:

| Capa | Qué se arrastra | Hacia dónde | Decisión |
|---|---|---|---|
| Preparación | Empleados | Estaciones | Quién conviene para cada tarea |
| Clasificación | Cajas | Pallets o zonas | Destino y condición de la carga |
| Armado | Cajas o grupos | Posiciones del pallet | Peso, fragilidad y orden |
| Despacho | Pallets | Camiones | Capacidad y equipamiento |
| Rutas | Camiones | Salidas o contratos | Prioridad, costo y riesgo |
| Mejora | Recursos o fichas | Proyecto elegido | En qué se transforma la empresa |

La acción física es estable; el significado cambia. Eso permite aprender rápido sin repetir siempre el mismo problema.

El arrastre se usa cuando expresa una relación espacial —quién trabaja dónde,
qué caja pertenece a qué pallet o qué carga entra en qué camión—. Los diálogos,
las confirmaciones y las opciones simples pueden usar toques normales. No se
forzará el gesto principal en lugares donde sólo vuelva más lenta la interfaz.

### 7.2 Especificación táctil provisional

Al tocar un elemento:

- se eleva sobre los demás;
- aumenta levemente de escala;
- su sombra y sonido indican que fue tomado;
- el objetivo relevante se destaca sin ocultar otras opciones.

Durante el arrastre:

- el objeto sigue al dedo sin retraso perceptible;
- el dedo no tapa la información importante;
- sólo los destinos posibles reaccionan;
- el destino correcto ejerce una atracción suave dentro de una zona generosa;
- el objeto conserva una sensación de peso sin volverse torpe.

Al soltar correctamente:

- el objeto encastra en menos de 0,25 segundos;
- la animación tiene una pequeña compresión o rebote;
- sonido y vibración opcional confirman;
- el estado del sistema se actualiza de inmediato.

Al soltar incorrectamente:

- no se pierde dinero por un error de dedo;
- se explica el conflicto en una frase o símbolo;
- el objeto vuelve en aproximadamente 0,3 segundos;
- se mantiene el contexto para intentar nuevamente.

### 7.3 Racha

La racha del prototipo es una respuesta de ritmo, no una moneda ni un multiplicador económico. Puede mejorar sonido, animación o celebración, pero no debe presionar al jugador ni castigar la exploración.

### 7.4 Criterios de sensación

- Los objetivos táctiles deben ser cómodos en teléfonos pequeños.
- El objeto seleccionado siempre queda visible.
- El arrastre debe funcionar con un dedo.
- La atracción nunca debe llevar una caja al destino equivocado.
- Se puede desactivar vibración y reducir movimiento.
- El juego debe sostener 60 fps en un teléfono Android de gama media.

---

## 8. Jornada jugable

### 8.1 Fase 1 — Briefing

El jugador recibe:

- cliente y recompensa;
- cantidad y tipos de carga;
- horario o condición especial;
- riesgos conocidos;
- recursos disponibles.

El briefing debe poder entenderse visualmente en pocos segundos. Los contratos complejos se introducen después de que sus reglas hayan sido enseñadas por separado.

### 8.2 Fase 2 — Asignación

El jugador arrastra empleados hacia estaciones. Cada persona tiene una especialidad clara y, como máximo, una condición temporal relevante.

Ejemplos:

- una persona fuerte trabaja mejor con carga pesada;
- una persona cuidadosa reduce daños en carga frágil;
- una persona rápida procesa urgencias;
- una persona capacitada puede operar frío o maquinaria.

Una asignación imperfecta es posible. Produce una consecuencia visible, no un bloqueo arbitrario.

### 8.3 Fase 3 — Clasificación

Las cajas aparecen desordenadas en la zona central. Los destinos rodean el área de trabajo. El jugador arrastra cada caja hacia el lugar correspondiente.

La dificultad no aumenta achicando cajas o escondiendo información. Aumenta al combinar reglas:

- destino;
- prioridad;
- peso;
- fragilidad;
- refrigeración.

### 8.4 Fase 4 — Armado de pallets

Una vez clasificada la carga, el jugador decide cómo agruparla:

- peso abajo;
- frágiles arriba o protegidos;
- urgentes accesibles;
- refrigerados en una unidad compatible;
- capacidad máxima respetada.

Esta fase puede resolverse mediante posiciones grandes o bloques, no con una cuadrícula diminuta. El objetivo es leer decisiones logísticas, no pelear contra la precisión táctil.

### 8.5 Fase 5 — Despacho

El jugador asigna pallets a camiones y confirma salidas. Debe considerar:

- capacidad;
- refrigeración;
- costo;
- confiabilidad;
- destino o ruta;
- prioridad de entrega.

Los camiones salen con una animación breve. El resultado no se decide mediante conducción.

### 8.6 Fase 6 — Cierre

El informe de jornada muestra causas y consecuencias:

- entregas correctas;
- puntualidad;
- daños;
- costos;
- ingreso neto;
- efecto en reputación;
- efecto en el equipo;
- cambio aproximado del valor de la empresa.

Luego aparece una sola decisión importante o una mejora. Evitaremos obligar al jugador a navegar varias tiendas y submenús después de cada turno.

---

## 9. Progresión de responsabilidades

El jugador no elige una profesión distinta para cada jornada. Representa a la persona fundadora, cuya atención cambia a medida que la empresa crece.

| Etapa | Rol dominante | Qué hace manualmente | Qué empieza a decidir |
|---|---|---|---|
| Inicio | Operario | Mueve y clasifica cajas | Orden básico |
| Crecimiento | Supervisor | Resuelve carga especial | Asignación de empleados |
| Consolidación | Despachante | Interviene en excepciones | Pallets, camiones y prioridades |
| Expansión | Gerente | Supervisa cuellos de botella | Contratos, procesos e inversión |
| Cierre | Dueño | Atiende crisis decisivas | Sucesión, venta y legado |

### 9.1 Delegación

Una tarea dominada puede delegarse cuando existe:

- una persona capacitada;
- un proceso definido;
- equipamiento adecuado;
- confianza suficiente.

Delegar no elimina la mecánica central. Reduce el volumen rutinario y hace aparecer excepciones:

- una etiqueta dañada;
- un pallet sobrecargado;
- una ausencia;
- una cámara de frío con problemas;
- un cliente que cambia la prioridad;
- un camión que llega tarde.

El jugador vuelve a tocar el sistema donde su decisión importa.

### 9.2 Automatización

La automatización cambia el flujo, no sólo un porcentaje:

- cinta transportadora que acerca cajas;
- lector que destaca destinos;
- autoelevador que mueve pallets pesados;
- software que sugiere carga de camiones;
- cámara de frío con mayor capacidad.

Cada automatización introduce un beneficio, un costo y al menos una limitación. Si una mejora sólo dice “+10 %”, se rediseña o se elimina.

---

## 10. Sistemas de carga

### 10.1 Atributos

| Atributo | Lectura visual | Consecuencia |
|---|---|---|
| Destino | Letra en prototipo; icono y color en arte final | Determina pallet y ruta |
| Peso | Tamaño, asas o símbolo de peso | Requiere fuerza; debe ir debajo |
| Fragilidad | Símbolo y forma visible | Necesita cuidado y protección |
| Refrigeración | Color frío, escarcha o termómetro | Requiere estación y vehículo compatibles |
| Urgencia | Etiqueta o pulso visual moderado | Debe procesarse y salir antes |

El color nunca será la única forma de comunicar un atributo.

### 10.2 Introducción de reglas

1. Sólo destino.
2. Destino y peso.
3. Destino y fragilidad.
4. Peso más fragilidad.
5. Refrigeración.
6. Urgencia.
7. Combinaciones dentro de contratos.

No se agrega un sexto atributo principal antes de comprobar que estas cinco variables producen suficiente variedad.

### 10.3 Errores operativos

Los errores posibles son:

- destino equivocado;
- pallet sobrecargado;
- frágil debajo de una carga pesada;
- pérdida de frío;
- salida tardía;
- camión incompatible;
- personal mal asignado.

Cada error debe mostrar:

1. qué ocurrió;
2. por qué ocurrió;
3. qué consecuencia tuvo;
4. cómo podría evitarse.

---

## 11. Empleados

### 11.1 Identidad

Los empleados son personas persistentes, no fichas descartables. Tendrán nombre, silueta, especialidad y una breve personalidad comunicada con pocas líneas.

No se simularán necesidades individuales complejas. El objetivo es que el jugador los reconozca y valore sin convertir el juego en un simulador social.

### 11.2 Aptitudes

Las tres aptitudes base son:

- **Fuerza:** carga pesada y maquinaria.
- **Cuidado:** carga frágil y precisión.
- **Rapidez:** urgencias y volumen.

Las certificaciones habilitan tareas específicas, como cadena de frío o autoelevador.

### 11.3 Estado del equipo

El sistema de equipo combina:

- capacitación;
- moral;
- confianza;
- estabilidad.

No habrá una barra de cansancio que obligue a esperar en tiempo real. Las jornadas exigentes pueden afectar temporalmente al equipo, pero siempre se resuelven mediante decisiones dentro de la campaña.

### 11.4 Decisiones humanas

Ejemplos:

- contratar ahora o capacitar a alguien existente;
- cubrir un turno exigente o rechazar un contrato;
- premiar al equipo o conservar efectivo;
- automatizar una tarea o crear un puesto nuevo;
- preparar a una persona para liderar la sucesión.

Las decisiones no deben reducir a las personas a “bonificaciones” sin contexto.

---

## 12. Pallets, estaciones y camiones

### 12.1 Estaciones

Conjunto inicial previsto:

- recepción;
- clasificación;
- armado;
- frío;
- despacho.

No todas están presentes desde el inicio. Cada estación nueva debe introducir una decisión diferente.

### 12.2 Pallets

Los pallets tienen:

- destino;
- capacidad;
- orden de carga;
- compatibilidad especial;
- estado de completitud.

El jugador debe poder identificar un pallet problemático sin abrir una ventana de estadísticas.

### 12.3 Camiones

Los vehículos se diferencian por:

- capacidad;
- costo operativo;
- refrigeración;
- confiabilidad;
- tipo de ruta.

La flota debe ser pequeña y reconocible. Preferimos tres camiones con personalidad funcional a veinte vehículos definidos por porcentajes.

---

## 13. Contratos y eventos

### 13.1 Contratos

Un contrato combina:

- volumen;
- mezcla de carga;
- exigencias;
- plazo;
- ingreso;
- riesgo de reputación;
- oportunidad futura.

El jugador siempre debe poder comparar al menos una opción segura y una ambiciosa cuando la historia lo permita.

Tipos previstos:

- comercio local;
- alimentos refrigerados;
- mudanza o carga pesada;
- insumos frágiles;
- entrega urgente;
- cliente corporativo de gran volumen.

### 13.2 Eventos

Los eventos rompen la rutina y revelan el tipo de empresa construida. Ejemplos:

- ausencia de una persona clave;
- avería de un camión;
- falla temporal de frío;
- cliente que cambia una prioridad;
- aumento de costos;
- oferta de expansión;
- oportunidad de capacitar al equipo;
- propuesta de compra;
- familiar que desea incorporarse.

Los eventos:

- no dependen de conectarse en una fecha real;
- no se resuelven pagando dinero real;
- no deben ocultar probabilidades críticas;
- ofrecen consecuencias previsibles;
- no anulan una campaña sana con un único resultado azaroso.

---

## 14. Economía de la empresa

### 14.1 Indicadores principales

| Indicador | Pregunta que responde | Riesgo |
|---|---|---|
| Caja | ¿Podemos pagar la próxima etapa? | Insolvencia |
| Reputación | ¿Los clientes confían en nosotros? | Menos contratos y oportunidades |
| Equipo | ¿La gente puede y quiere sostener la operación? | Errores, rotación y mala sucesión |
| Valor de empresa | ¿Qué construimos a largo plazo? | Venta débil o crecimiento estancado |

Durante una jornada también se calculan puntualidad, daños, exactitud y eficiencia, pero son causas del resultado; no necesitan vivir permanentemente en la pantalla principal.

### 14.2 Ingresos y costos

Ingresos:

- pago base de contrato;
- bono por servicio excepcional;
- relaciones comerciales estables.

Costos:

- salarios;
- operación de vehículos;
- mantenimiento;
- capacitación;
- infraestructura;
- deuda;
- daños y compensaciones.

### 14.3 Valor de la empresa

El valor combina de forma provisional:

- activos útiles;
- caja;
- cartera de clientes;
- reputación;
- procesos confiables;
- equipo preparado;
- capacidad de crecer sin depender completamente del fundador;
- deuda y riesgos pendientes.

El cálculo exacto puede permanecer oculto, pero sus causas deben ser visibles. “La empresa vale más” nunca puede aparecer sin explicación.

### 14.4 Endeudamiento y quiebra

La quiebra no se activa por un error aislado.

Secuencia prevista:

1. La caja entra en zona de riesgo.
2. El juego advierte la causa y ofrece reducir costos, renegociar o elegir trabajo seguro.
3. Si el problema continúa, aparece una reestructuración con una consecuencia real.
4. La campaña termina en quiebra sólo si la empresa sigue siendo insostenible.

El jugador debe sentir que perdió por una cadena de decisiones comprensibles.

### 14.5 Inversiones

Cuatro familias:

- **Personas:** contratar, capacitar y preparar liderazgo.
- **Procesos:** etiquetas, control, delegación y planificación.
- **Infraestructura:** espacio, estaciones y frío.
- **Flota:** capacidad, confiabilidad y especialización.

Las mejoras compiten por recursos. No es posible maximizar todo en una sola campaña.

---

## 15. Campaña finita

### 15.1 Tiempo de empresa

La campaña representa aproximadamente veinte años, divididos en jornadas decisivas. No se juega cada día del calendario. Entre una jornada y otra pueden transcurrir semanas, meses o años según el capítulo.

Duración provisional: **36 jornadas**.

### 15.2 Capítulos

| Capítulo | Años aproximados | Jornadas | Tema | Sistemas nuevos |
|---|---:|---:|---|---|
| 1. Fundación | 1–2 | 4 | Aprender el oficio | Arrastre, destinos, primer contrato |
| 2. Crecimiento | 3–6 | 8 | Dejar de hacerlo todo solo | Empleados, peso, fragilidad |
| 3. Crisis | 7–10 | 8 | Sostener lo construido | Costos, deuda, eventos, frío |
| 4. Expansión | 11–16 | 10 | Crear una organización | Flota, procesos, delegación |
| 5. Futuro | 17–20 | 6 | Decidir qué queda | Sucesión, oferta de compra y cierre |

La cantidad final se ajustará según pruebas. El juego no se rellena hasta alcanzar 36 si sus situaciones ya se repiten.

### 15.3 Ritmo de introducción

Cada capítulo:

1. enseña una regla en un entorno seguro;
2. la combina con una regla conocida;
3. presenta una decisión económica relacionada;
4. plantea una jornada de dominio;
5. cierra con una consecuencia narrativa o empresarial.

### 15.4 Dificultad

La dificultad crece mediante:

- más incompatibilidades relevantes;
- decisiones con costos de oportunidad;
- carga mixta;
- prioridades simultáneas;
- menor margen empresarial, claramente comunicado;
- eventos que exigen usar sistemas conocidos de otra manera.

No crece mediante:

- objetivos táctiles más pequeños;
- instrucciones ocultas;
- velocidades ilegibles;
- castigos desproporcionados;
- números inflados sin una decisión nueva.

---

## 16. Finales

### 16.1 Quiebra

**Naturaleza:** desenlace de fracaso, pero narrativamente digno.

**Causa:** insolvencia sostenida después de advertencias y una oportunidad de reestructuración.

**Mensaje:** crecer sin una base sostenible tiene consecuencias.

El epílogo muestra qué decisiones llevaron al cierre. La campaña puede reiniciarse sin castigo.

### 16.2 Legado familiar

**Naturaleza:** victoria de continuidad.

**Requisitos provisionales:**

- empresa solvente;
- reputación estable;
- equipo preparado;
- sucesor o sucesora capacitada;
- dependencia razonable del fundador;
- decisión explícita de conservar el negocio.

**Mensaje:** una buena empresa puede valer por las personas y la continuidad, no sólo por su precio de venta.

El epílogo muestra a la siguiente generación asumiendo una compañía con la identidad que el jugador construyó.

### 16.3 Venta y jubilación

**Naturaleza:** victoria de valor y escalabilidad.

**Requisitos provisionales:**

- valor empresarial alto;
- deuda controlada;
- procesos confiables;
- cartera atractiva;
- capacidad de operar sin el fundador;
- aceptación de una oferta de compra.

**Mensaje:** convertir un pequeño depósito en una organización valiosa también es un éxito.

El mejor precio no debería exigir destruir al equipo. Puede existir una variante de epílogo según cómo se realizó la venta.

### 16.4 Lectura del cierre

El final resume:

- años de operación;
- contratos decisivos;
- empleados que marcaron la empresa;
- inversiones principales;
- crisis superadas;
- indicadores finales;
- estilo de gestión demostrado.

No habrá un puntaje global que declare una única forma “correcta” de jugar.

---

## 17. Interfaz y flujo de pantallas

### 17.1 Pantallas necesarias

1. Identidad de Bannoya's Games.
2. Menú principal: continuar, nueva empresa, opciones y créditos.
3. Mapa o línea temporal de la empresa.
4. Briefing de jornada.
5. Depósito jugable.
6. Informe de jornada.
7. Decisión o mejora.
8. Evento de capítulo.
9. Epílogo.

### 17.2 Pantalla del depósito

Distribución base en vertical:

- encabezado compacto con fase y objetivo;
- destinos o estaciones alrededor;
- área central de carga;
- información económica fuera del espacio táctil principal;
- acción de pausa accesible;
- mensajes contextuales breves.

No se reservará permanentemente media pantalla para barras y monedas.

### 17.3 Onboarding

Primera jornada:

1. Una caja y un destino.
2. El destino reacciona al tocar la caja.
3. El jugador arrastra sin texto largo.
4. El encastre confirma la regla.
5. Aparece una segunda caja.
6. Al final se explica que esa entrega es el primer trabajo de la empresa.

La enseñanza se realiza jugando. Las instrucciones verbales son apoyo, no requisito.

### 17.4 Información

Prioridad visual:

1. qué objeto estoy moviendo;
2. dónde puede ir;
3. qué regla importa ahora;
4. qué falta para terminar;
5. qué consecuencia tendrá.

---

## 18. Dirección visual

### 18.1 Estilo

**Dirección provisional:** 2D estilizado, limpio y cálido, con formas grandes, bordes claros y una ligera perspectiva cenital.

El depósito debe verse trabajado y humano, no como una interfaz corporativa fría. La complejidad visual crece junto con la empresa, pero la lectura se conserva.

### 18.2 Evolución del lugar

- Inicio: galpón pequeño, marcas hechas a mano y equipamiento usado.
- Crecimiento: zonas pintadas, mejores pallets y uniformes.
- Crisis: desgaste, reparaciones y espacio tensionado.
- Expansión: estaciones especializadas y flota propia.
- Futuro: depósito profesional con rastros visibles de su historia.

### 18.3 Código visual de carga

Las cajas usan una combinación de:

- icono;
- forma o accesorio;
- etiqueta;
- color;
- animación moderada.

Las letras A–G son exclusivas de prototipo. Se reemplazarán por destinos y símbolos reconocibles después de validar distribución y ritmo.

### 18.4 Marca

Bannoya's Games aparece con moderación:

- pantalla inicial;
- créditos;
- pequeños detalles o secretos;
- una celebración final.

La banana con sombrero de olla debe aportar personalidad sin invadir la ficción del depósito.

---

## 19. Sonido y respuesta

### 19.1 Objetivos

- reforzar peso y material;
- confirmar sin depender del texto;
- crear ritmo;
- diferenciar éxito, advertencia y error;
- acompañar el crecimiento del depósito.

### 19.2 Sonidos principales

- tomar caja;
- deslizar;
- entrar en zona magnética;
- encastrar;
- error suave y retorno;
- pallet completo;
- puerta o camión;
- cierre de jornada;
- mejora instalada.

### 19.3 Música

Música ligera, laboriosa y optimista. Puede ganar capas a medida que la empresa crece. No debe producir urgencia constante.

### 19.4 Vibración

- pulso corto al tomar;
- pulso definido al encastrar;
- patrón distinto para advertencia;
- opción para desactivarla.

---

## 20. Accesibilidad y comodidad

- Información redundante: color más icono o texto.
- Vibración configurable.
- Volúmenes separados.
- Opción de reducir animaciones.
- Objetivos táctiles grandes.
- Tamaño de texto legible.
- Contraste suficiente.
- Pausa en cualquier momento.
- Repetición de instrucciones.
- Jornada reanudable.
- Modo sin presión temporal cuando una regla no necesita reloj.
- Sin penalidad económica por errores de precisión táctil.
- Soporte de mouse para desarrollo y posible accesibilidad.

Las pruebas incluirán al menos una pantalla de teléfono pequeña y diferentes relaciones de aspecto.

---

## 21. Modelo comercial, privacidad y ética

### 21.1 Lanzamiento

Cargo Exit será gratuito y completo.

- Sin anuncios.
- Sin compras dentro de la aplicación.
- Sin moneda premium.
- Sin contenido bloqueado por conexión.
- Sin registro.

El objetivo comercial indirecto es construir reputación para Bannoya's Games y demostrar capacidad de terminar un producto propio.

### 21.2 Costos

El desarrollo prioriza herramientas y recursos existentes. El pago de la cuenta de Google Play u otro gasto requiere confirmación del dueño del proyecto en el momento correspondiente.

### 21.3 Datos

La primera versión guarda progreso local. No incorpora analítica remota. Las pruebas se medirán mediante observación, formularios y, si resulta útil, registros locales compartidos voluntariamente por testers.

---

## 22. Alcance de producción

### 22.1 Prototipo táctil actual

Objetivo: validar el placer de arrastrar y encastrar.

Incluye:

- catorce cajas;
- siete pallets A–G;
- dos cajas por destino;
- pila central;
- atracción suave;
- encastre;
- retorno sin penalidad;
- racha visual;
- siguiente tanda automática.

No valida todavía gestión, economía, variedad ni progresión.

### 22.2 Próximo vertical slice — una jornada completa

Objetivo: demostrar que el gesto principal sostiene decisiones logísticas conectadas.

Contenido:

- dos empleados: uno fuerte y uno cuidadoso;
- dos estaciones;
- ocho cajas;
- carga pesada y frágil;
- dos pallets;
- dos camiones con capacidades diferentes;
- un contrato;
- tres fases: asignar, procesar y despachar;
- informe de resultados;
- una decisión de mejora;
- duración de 5–8 minutos.

No incluye:

- campaña;
- árbol de mejoras;
- múltiples clientes;
- economía persistente;
- arte definitivo;
- final narrativo.

### 22.3 MVP de campaña

Objetivo: comprobar el arco completo de aprendizaje y crecimiento con contenido limitado.

Contenido provisional:

- capítulo 1 completo y parte del capítulo 2;
- 8–10 jornadas;
- destino, peso y fragilidad;
- hasta cuatro empleados;
- tres estaciones;
- dos o tres vehículos;
- seis mejoras con efecto visible;
- eventos simples;
- guardado local;
- opciones básicas de accesibilidad;
- español completo;
- principio y cierre temporal del MVP.

### 22.4 Versión 1.0

Contenido objetivo:

- cinco capítulos;
- 30–40 jornadas, sólo si mantienen variedad;
- cinco atributos de carga;
- equipo persistente;
- delegación;
- contratos y eventos;
- economía completa;
- tres finales;
- arte, audio y vibración terminados;
- español e inglés;
- guardado local robusto;
- pruebas en dispositivos Android.

### 22.5 Fuera de alcance de 1.0

- iOS;
- PC comercial;
- multijugador;
- juego en la nube;
- tablas de clasificación;
- creación de niveles por usuarios;
- eventos en vivo;
- contenidos diarios;
- microtransacciones;
- publicidad;
- doblaje;
- conducción de camiones;
- simulación física compleja;
- construcción libre del depósito.

---

## 23. Validación

### 23.1 Etapa A — sensación táctil

Muestra objetivo: 5–8 personas.

Preguntas:

- ¿Entienden qué hacer sin explicación verbal?
- ¿El objeto sigue bien el dedo?
- ¿El encastre se siente claro y agradable?
- ¿Los destinos son suficientemente grandes?
- ¿Quieren ordenar otra tanda?

Indicadores:

- 80 % completa la primera tanda sin ayuda.
- 70 % juega voluntariamente una segunda tanda.
- Menos del 10 % de los intentos fallidos se atribuye al control.
- La mayoría describe la interacción con palabras como “suave”, “satisfactoria” o equivalentes.

### 23.2 Etapa B — jornada con decisiones

Muestra objetivo: 5–8 personas.

Preguntas:

- ¿Entienden por qué un empleado conviene para una estación?
- ¿Distinguen pesado y frágil sin leer un manual?
- ¿Pueden explicar por qué eligieron un camión?
- ¿El cambio de fase se siente como parte del mismo juego?
- ¿La jornada se vuelve repetitiva antes de terminar?

Indicadores:

- 75 % completa el turno sin explicación externa.
- 70 % puede explicar al menos dos consecuencias de sus decisiones.
- Menos de una consulta verbal promedio por fase.
- 60 % pide jugar otra jornada o pregunta qué sistema viene después.

### 23.3 Etapa C — MVP de campaña

Preguntas:

- ¿El crecimiento se ve y se siente?
- ¿Las mejoras cambian decisiones?
- ¿El jugador se preocupa por el equipo?
- ¿La economía es comprensible?
- ¿El final del capítulo genera curiosidad?

Indicadores:

- 70 % de los testers que empiezan completa al menos seis jornadas.
- 50 % completa el MVP.
- 60 % identifica una decisión de la que se siente responsable.
- La mayoría puede describir el estado de su empresa sin leer números exactos.
- Ningún sistema principal es mencionado como relleno por la mayoría.

### 23.4 Registro sin analítica remota

Durante pruebas observaremos:

- tiempo hasta la primera acción;
- errores de arrastre;
- repeticiones voluntarias;
- tiempo por fase;
- reglas confundidas;
- decisiones ignoradas;
- causa de abandono;
- comentarios espontáneos;
- deseo de continuar.

### 23.5 Condiciones de revisión

- Si el gesto no resulta satisfactorio después de dos rondas de pulido, se rediseña antes de añadir economía.
- Si la jornada parece una suma de minijuegos, se unifica el lenguaje visual y se eliminan fases.
- Si las personas miran más menús que el depósito, se simplifica la gestión.
- Si tres atributos simultáneos confunden, no se añade el siguiente.
- Si delegar elimina diversión, se conservan manualmente las excepciones más interesantes.
- Si el contenido se repite antes de diez jornadas, se reduce la campaña o se profundizan reglas existentes.

---

## 24. MDA — de reglas a experiencia

| Mecánica | Dinámica que produce | Sensación buscada |
|---|---|---|
| Arrastrar y encastrar | Entrar en ritmo y limpiar el caos | Satisfacción |
| Atributos visibles de carga | Comparar y priorizar | Comprensión |
| Aptitudes de empleados | Formar equipos y compensar debilidades | Responsabilidad |
| Capacidad de pallets y camiones | Planificar espacio y orden | Dominio |
| Contratos con riesgo | Elegir seguridad o crecimiento | Agencia |
| Caja, reputación y equipo | Equilibrar corto y largo plazo | Propiedad |
| Delegación | Soltar tareas dominadas y atender excepciones | Progreso |
| Tiempo finito y finales | Dar significado acumulativo a decisiones | Legado |

### 24.1 Cadena central

**Mecánica:** arrastrar una caja con atributos hacia un destino limitado.

**Dinámica:** priorizar, corregir cuellos de botella y coordinar recursos.

**Estética:** sentir que una operación caótica funciona gracias al criterio del jugador.

---

## 25. Dirección técnica

### 25.1 Base

- Unity 6000.3.20f1.
- C#.
- Android como objetivo principal.
- Interfaz a 1080 × 1920 de referencia, adaptable.
- Orientación vertical.
- Objetivo de 60 fps.
- Progreso local.
- Tipografía jugable de al menos 40 unidades en la resolución de referencia
  durante el prototipo.
- Git en rama `main`.

### 25.2 Separación recomendada

- **Core:** reglas puras de carga, empleados, contratos y economía.
- **Application:** coordinación de jornada, campaña y guardado.
- **Presentation:** interfaz, animación, sonido y entrada.
- **Content:** datos de empleados, contratos, eventos y capítulos.
- **Tests:** reglas de economía, compatibilidad y transiciones.

Las reglas importantes no deben depender de animaciones ni objetos de escena. Esto permite probarlas automáticamente.

#### Dirección obligatoria de dependencias

La separación es una regla del proyecto, no una sugerencia:

1. **Core** usa C# puro y no conoce `UnityEngine`, escenas ni interfaz.
2. **Presentation** puede depender de Core para mostrar y ejecutar sus reglas.
3. **Editor** puede depender de Presentation para construir y abrir escenas, pero nunca forma parte del juego compilado.
4. **Tests EditMode** comprueban Core sin levantar una escena.
5. **Tests PlayMode** comprueban la integración visual y táctil.

#### Convención física del proyecto

Los recursos del juego se agrupan dentro de `Assets/CargoExit`: el contenido
visual en `Art`, las escenas en `Scenes` y todo el código en `Scripts`. Dentro
de `Scripts`, Core, Presentation, Editor y Tests conservan carpetas y
definiciones de ensamblado separadas. Esta organización puede crecer con
Application o Content cuando exista una necesidad concreta, pero no se
adelantan sistemas vacíos.

Core nunca depende de Presentation o Editor. La lógica de una jornada no se
concentrará en un único controlador: reglas, flujo, vista, entrada y herramientas
de autoría se mantendrán como responsabilidades distintas. Si aparece un error,
debe ser posible determinar qué capa lo produce antes de modificar otra.

### 25.3 Datos

Los contenidos variables se definirán mediante datos editables, no mediante condiciones dispersas en scripts:

- tipos de carga;
- empleados;
- estaciones;
- camiones;
- contratos;
- eventos;
- mejoras;
- capítulos.

### 25.4 Guardado

El guardado mínimo contiene:

- versión de datos;
- capítulo y jornada;
- indicadores;
- equipo;
- activos y mejoras;
- decisiones narrativas;
- configuración.

Se requiere migración simple o reinicio seguro mientras el proyecto esté en desarrollo.

### 25.5 Estado del proyecto

La única escena activa es `Assets/CargoExit/Scenes/CargoExit.unity`. Contiene una
vista previa visible desde el editor y se transforma en la versión interactiva
al entrar en Play Mode. Los experimentos anteriores de cuadrícula y descarga
fueron retirados después de cumplir su función de aprendizaje.

Antes de generar una versión para dispositivo se debe instalar el módulo Android correspondiente a Unity. No se realizará ninguna publicación ni aceptación de términos sin confirmación.

---

## 26. Riesgos principales

| Riesgo | Señal temprana | Respuesta |
|---|---|---|
| Repetición | La segunda tanda ya se siente igual | Introducir decisiones, no más volumen |
| Demasiadas reglas | El jugador consulta etiquetas constantemente | Reducir atributos simultáneos |
| Gestión desconectada | Las pantallas parecen otro juego | Mantener el mismo verbo y mostrar consecuencias físicas |
| Exceso de alcance | Se crean sistemas antes de validar una jornada | Trabajar por puertas de aprobación |
| Economía opaca | El jugador no entiende pérdidas | Informes causales y números pequeños |
| UI apretada | El dedo tapa cajas o destinos | Menos elementos, objetivos más grandes |
| Delegación aburrida | El juego se observa solo | Excepciones y decisiones activas |
| Contenido costoso | Cada jornada requiere lógica única | Reglas combinables y eventos basados en datos |
| Arte prematuro | Se rehacen assets por cambios de diseño | Prototipo legible antes de producción visual |
| Finales inalcanzables | Una ruta exige jugar perfecto | Ventanas amplias y advertencias |
| Quiebra injusta | Un evento aleatorio destruye la campaña | Crisis escalonada y oportunidad de recuperación |

---

## 27. Puertas de producción

No se avanza sólo porque una función “ya está programada”. Cada etapa necesita demostrar una pregunta.

### Puerta 0 — GDD

**Pregunta:** ¿sabemos qué juego estamos construyendo?

**Resultado esperado:** visión, pilares, bucles, alcance y finales acordados.

### Puerta 1 — Núcleo táctil

**Pregunta:** ¿ordenar cajas se siente bien?

**Resultado esperado:** primera validación con jugadores y ajustes de control.

### Puerta 2 — Jornada vertical

**Pregunta:** ¿el gesto puede sostener gestión?

**Resultado esperado:** asignación, carga y despacho conectados.

### Puerta 3 — MVP de campaña

**Pregunta:** ¿el crecimiento hace querer continuar?

**Resultado esperado:** 8–10 jornadas, progreso persistente y pruebas.

### Puerta 4 — Campaña completa

**Pregunta:** ¿las decisiones producen historias y finales diferentes?

**Resultado esperado:** contenido completo sin relleno.

### Puerta 5 — Preparación Android

**Pregunta:** ¿el juego es estable, legible y cómodo en dispositivos reales?

**Resultado esperado:** rendimiento, guardado, accesibilidad y pruebas cerradas.

### Puerta 6 — Lanzamiento

**Pregunta:** ¿Cargo Exit representa dignamente a Bannoya's Games?

**Resultado esperado:** versión terminada, materiales y publicación confirmada por el dueño del proyecto.

---

## 28. Decisiones abiertas

Estas preguntas no bloquean el próximo vertical slice:

- dirección visual final: cenital plana o semicenital;
- nombres y apariencia del fundador y el equipo;
- si la empresa dentro del mundo también se llama Cargo Exit;
- duración definitiva de la campaña;
- cantidad exacta de jornadas por capítulo;
- uso de reloj en contratos específicos;
- profundidad de formación de pallets;
- forma visual de la línea temporal;
- tono exacto de los eventos familiares;
- alcance del inglés en la primera publicación;
- música original o recursos con licencia compatible.

Se resolverán mediante prototipos, costos reales y pruebas, no sólo por preferencia.

---

## 29. Registro de decisiones

| Fecha | Decisión | Estado |
|---|---|---|
| 29/07/2026 | Bannoya's Games será la marca del estudio | Acordado |
| 29/07/2026 | Cargo Exit será el primer juego Android del estudio | Acordado |
| 29/07/2026 | El juego será completo, gratuito, sin anuncios ni compras | Acordado |
| 29/07/2026 | El gesto principal es arrastrar y encastrar elementos | Acordado |
| 29/07/2026 | La distribución base usa carga central y destinos periféricos | Acordado para prototipo |
| 29/07/2026 | Cargo Exit será un juego de gestión táctil | Acordado |
| 29/07/2026 | La mecánica principal se reutiliza para cajas, empleados, pallets y camiones | Acordado |
| 29/07/2026 | Las tareas rutinarias podrán delegarse | Acordado |
| 29/07/2026 | La campaña tendrá tiempo finito y tres finales | Acordado |
| 29/07/2026 | Quiebra, legado familiar y venta con jubilación son los tres desenlaces | Acordado |
| 29/07/2026 | La campaña representará cerca de veinte años | Provisional |
| 29/07/2026 | La versión completa tendrá cerca de 36 jornadas | Por validar |

---

## 30. Próxima construcción aprobada por este GDD

El siguiente paso no es implementar toda la economía ni producir arte final.

Se construirá una sola jornada vertical con:

1. asignación de dos empleados;
2. clasificación de ocho cajas pesadas o frágiles;
3. armado simple de dos pallets;
4. asignación a dos camiones;
5. informe causal de jornada;
6. una elección de mejora.

Esta prueba decidirá si Cargo Exit ya posee una base de juego completa. Hasta aprobarla no se añaden campaña, deuda, eventos complejos ni finales.

---

## 31. Criterio rector

> Cada caja que el jugador mueve debe ayudar a responder qué clase de empresa está construyendo.

Si una mecánica es agradable pero no se conecta con esa pregunta, puede ser un buen juguete, pero no pertenece necesariamente a Cargo Exit.
