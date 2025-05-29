# El Enigma del Portal – Guía de Implementación en Escena

## Resumen

Este proyecto es un juego de aventura y acción con progresión de personaje, combate, inventario, perks y un sistema de mercader. El código está estructurado para separar lógica pura y controladores Unity, facilitando la implementación y el mantenimiento.

---

## Guía de Implementación en Escena

### 1. Personaje Jugable

- **Prefab/Objeto:** Crea un GameObject vacío y añade el script `JugadorControlador`.
- **Componentes requeridos:** 
  - `Collider` (ej: CapsuleCollider)
  - `SaludSistemaControlador`
  - `PlayerInput` (configurado con el mapa de acciones)
- **Configuración:**
  - Asigna los valores de salud, velocidad, fuerza de salto, etc. en el inspector.
  - Asigna el prefab de proyectil y el punto de disparo.
  - Asocia la UI de inventario y el panel de pausa si existen.
- **Notas:** El jugador debe tener el tag "Player".

### 2. Minions (Enemigos Comunes)

- **Prefab/Objeto:** Crea un prefab de enemigo con el script `EnemigoControlador` y su lógica asociada.
- **Spawner:** Añade el script `MinionSpawner` en un GameObject vacío.
  - **Configuración:** 
    - Define los puntos de aparición (pueden ser hijos vacíos del spawner).
    - Asigna el prefab del minion.
    - Configura la cantidad máxima de minions y el intervalo de respawn.
- **Notas:** Los minions deben tener un `Collider`, `NavMeshAgent` (si usan pathfinding) y el tag adecuado.

### 3. Jefe (Boss)

- **Prefab/Objeto:** Crea un prefab de jefe con su propio `EnemigoControlador` y lógica de jefe.
- **Puntos de reaparición:** 
  - Crea varios GameObjects vacíos en la escena como puntos de respawn.
  - El sistema de respawn del jefe debe referenciar estos puntos.
- **Notas:** El jefe puede tener lógica especial para eventos al morir.

### 4. Mercader y Zona de Seguridad

- **Prefab/Objeto:** Crea un GameObject con el script `Mercader`.
  - Asigna los ScriptableObjects de ítems y perks en venta.
  - Asocia el panel de UI del mercader (`MercaderUI`).
- **Zona de seguridad:** 
  - Añade un `Collider` (con "Is Trigger" activado) para delimitar la zona de interacción.
  - El mercader abrirá la UI cuando el jugador entre y presione el botón de interactuar.
- **Notas:** El mercader debe estar en una zona segura donde los enemigos no puedan entrar (puedes usar layers o colliders adicionales para bloquearlos).

---

## Requisitos Previos

- **Versión de Unity recomendada:** 6000.2.0b2 (Beta)
- **Paquetes necesarios:**
  - Input System (com.unity.inputsystem)
  - TextMeshPro (com.unity.textmeshpro)
  - (Opcional) NavMeshComponents para IA de enemigos
- **Cómo instalar paquetes:**
  1. Abre el proyecto en Unity Hub con la versión 6000.2.0b2.
  2. Ve a `Window > Package Manager`.
  3. Busca e instala los paquetes mencionados arriba.

---

## Guía de Instalación y Apertura del Proyecto

1. Descarga o clona el repositorio en tu PC.
2. Abre Unity Hub y selecciona `Add` para añadir la carpeta del proyecto.
3. Abre el proyecto con Unity 6000.2.0b2.
4. Espera a que Unity importe todos los assets y compile los scripts.
5. Si aparecen advertencias, revisa que los paquetes requeridos estén instalados.

---

## Configuración Inicial de la Escena (Paso a Paso)

1. **Crea una nueva escena vacía.**
2. **Agrega un Canvas:**
   - Ve a `GameObject > UI > Canvas`.
   - Dentro del Canvas, crea los paneles: `InventarioUI`, `MercaderUI`, `FeedbackPanel`, `PausaUI`.
3. **Agrega el jugador:**
   - Ve a `GameObject > 3D Object > Capsule` (o tu prefab de jugador).
   - Añade los componentes: `JugadorControlador`, `SaludSistemaControlador`, `PlayerInput`, y un `Collider`.
   - Asigna el tag "Player".
   - En el Inspector, arrastra los paneles de UI a los campos correspondientes del script.
4. **Agrega enemigos minions:**
   - Crea un prefab de enemigo con `EnemigoControlador` y su lógica.
   - Añade un GameObject vacío llamado `MinionSpawner` y coloca hijos vacíos como puntos de spawn.
   - Asigna el prefab del minion y configura la cantidad máxima e intervalo de respawn.
5. **Agrega el jefe:**
   - Crea un prefab de jefe con su controlador y lógica.
   - Añade un GameObject vacío `BossRespawnPoints` y crea hijos vacíos como puntos de respawn.
   - Configura el sistema de respawn del jefe para usar estos puntos.
6. **Agrega el mercader y su zona:**
   - Crea un GameObject con el script `Mercader` y un `Collider` (isTrigger).
   - Asigna los ScriptableObjects de ítems y perks en venta.
   - Asocia el panel `MercaderUI`.
   - Coloca la zona de seguridad usando colliders/layers para evitar que enemigos entren.
7. **Configura el sistema de Input:**
   - Asegúrate de que el componente `PlayerInput` tenga asignado el mapa de acciones correcto.

---

## Explicación Visual

- **Jerarquía de ejemplo:**
  - Main Camera
  - Canvas
    - InventarioUI
    - MercaderUI
    - FeedbackPanel
    - PausaUI
  - Player (con JugadorControlador, SaludSistemaControlador, PlayerInput, Collider)
  - MinionSpawner
    - SpawnPoint1
    - SpawnPoint2
    - ...
  - BossRespawnPoints
    - BossPoint1
    - BossPoint2
    - ...
  - MercaderZone (con Collider isTrigger)
    - Mercader (con Mercader, MercaderUI, referencias a productos)

- **Inspector:**
  - Incluye capturas de pantalla o diagramas resaltando los campos importantes a asignar (recomendado agregar imágenes en la carpeta `Docs` o en el propio README si es posible).

---

## Preguntas Frecuentes y Solución de Problemas

- **El jugador no se mueve:**
  - Verifica que el `PlayerInput` esté correctamente configurado y el mapa de acciones esté asignado.
- **No aparece la UI:**
  - Asegúrate de que los paneles estén activos y referenciados en los scripts.
- **Los enemigos no aparecen:**
  - Revisa la configuración del `MinionSpawner` y que los prefabs estén asignados.
- **No puedo interactuar con el mercader:**
  - Verifica el collider de la zona y que el script `Mercader` esté activo.
- **Errores de paquetes:**
  - Instala los paquetes requeridos desde el Package Manager.

---

## Glosario de Términos

- **Prefab:** Objeto guardado como plantilla reutilizable.
- **Inspector:** Panel de Unity donde se configuran los componentes de un GameObject.
- **Collider:** Componente que define la forma física para colisiones.
- **ScriptableObject:** Asset para almacenar datos reutilizables.
- **Action Map:** Conjunto de acciones configuradas en el Input System.
- **Tag:** Etiqueta para identificar GameObjects.
- **Layer:** Capa para gestionar colisiones y visibilidad.

---

## Guía Rápida de Prueba

- Ejecuta la escena desde el botón `Play` en Unity.
- Usa las teclas configuradas (por defecto: WASD para moverse, Espacio para saltar, E para interactuar, I para inventario, Esc para pausa).
- Interactúa con el mercader, derrota enemigos y prueba el sistema de inventario y perks.
- Observa los mensajes de feedback y asegúrate de que las UI se abren/cierra correctamente.

---

## Recomendaciones Generales

- **Organización:** Usa prefabs para el jugador, enemigos, jefe y mercader para facilitar la reutilización.
- **Tags y Layers:** Usa tags ("Player", "Enemy", "Boss", "Mercader") y layers para gestionar colisiones y lógica de interacción.
- **UI:** Asegúrate de que los paneles de inventario, mercader y feedback estén correctamente referenciados en los scripts.
- **Pruebas:** Prueba cada sistema por separado antes de integrarlos todos en la escena final.

---

## Ejemplo de flujo de UI

1. El jugador abre el inventario (`InventarioUI`). El input de movimiento se bloquea y solo puede interactuar con la UI.
2. Cierra el inventario y recupera el control del personaje.
3. Entra en la zona del mercader y presiona interactuar. Se abre la `MercaderUI`, se bloquea el input de movimiento y se activa el Action Map de UI.
4. Al cerrar la `MercaderUI`, se reactiva el control del jugador.
5. Si el jugador pausa el juego, se muestra la `PausaUI` y se bloquean todas las demás interacciones.

---

## Ejemplo de Jerarquía Mínima en la Escena

```
- Main Camera
- Player (con JugadorControlador, SaludSistemaControlador, PlayerInput, Collider)
- MinionSpawner
    - SpawnPoint1
    - SpawnPoint2
    - ...
- BossRespawnPoints
    - BossPoint1
    - BossPoint2
    - ...
- MercaderZone (con Collider isTrigger)
    - Mercader (con Mercader, MercaderUI, referencias a productos)
- Canvas
    - InventarioUI
    - MercaderUI
    - FeedbackPanel
    - PausaUI
```

---

## Flujos de Interacción

### Ganar Experiencia y Monedas
1. El jugador derrota enemigos/minions/bosses.
2. Gana experiencia según el tipo de enemigo.
3. Al subir de nivel, recibe monedas automáticamente según el rango de nivel alcanzado.
4. Las monedas se usan para comprar ítems/perks al mercader.

### Equipar Ítems
1. El jugador compra o encuentra un ítem (arma, chaleco, casco, botas).
2. El ítem se añade al inventario.
3. Desde la UI de inventario, el jugador puede equipar el ítem en su ranura correspondiente.
4. Si ya hay un ítem equipado de ese tipo, se desequipa y vuelve al inventario.
5. Solo el ítem equipado de cada tipo modifica la estadística correspondiente.

### Comprar en el Mercader
1. El jugador abre la UI del mercader.
2. Selecciona un producto y pulsa comprar.
3. Si tiene suficientes monedas y no posee ya el producto, la compra se realiza y el ítem/perk se añade al inventario o perks.
4. El saldo de monedas se actualiza y la UI da feedback visual/sonoro.

### Spawneo y Comportamiento de Enemigos
1. El `MinionSpawner` controla la cantidad máxima de minions y su reposición automática.
2. Los enemigos patrullan, persiguen y atacan al jugador según su patrón de comportamiento.
3. Al morir, pueden otorgar experiencia, soltar objetos o activar eventos.
