// JugadorControlador.cs (VERSION CORREGIDA)
using UnityEngine;
using System; // Para Action
using Personajes.Jugador; // <-- AÑADIDO PARA QUE RECONOZCA JUGADORLOGICA
using Jugabilidad; // <-- AÑADIDO PARA QUE RECONOZCA MERCADERPRODUCTO
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))] // Necesita un Collider para la detección de suelo.
[RequireComponent(typeof(SaludSistemaControlador))] // Ahora, requiere un controlador para el sistema de salud.
public class JugadorControlador : MonoBehaviour
{
    [Header("Configuración de la Lógica del Jugador")]
    [SerializeField]
    private int saludMaxima = 100;

    [SerializeField]
    private float velocidadBase = 5f;

    [SerializeField]
    private float fuerzaSalto = 10f;

    [SerializeField]
    private float gravedad = 20f;

    [SerializeField]
    private float tiempoEntreDisparos = 0.5f;

    [SerializeField]
    private float fuerzaDisparo = 20f; // <--- AHI DEBE ESTAR SERIALIZADO, NO SE MUEVE AL OTRO SCRIPT


    [Header("Configuración de Colisiones Físicas")]
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private float checkGroundRadius = 0.4f;
    [SerializeField]
    private Vector3 checkGroundOffset = new Vector3(0, -0.9f, 0);

    [Header("Configuración de Proyectiles")]
    [SerializeField]
    private GameObject prefabProyectil;
    [SerializeField]
    private Transform puntoDisparo; // Punto desde donde se disparará el proyectil

    [Header("Respawn y Checkpoint")]
    [SerializeField] private PlayerRespawn playerRespawn;
    [SerializeField] private InventarioUI inventarioUI;
    [SerializeField] private GameObject pausaUI;

    private JugadorLogica jugadorLogica; // Referencia a la clase de lógica pura del jugador
    private SaludSistemaControlador saludControlador; // Referencia al controlador de salud

    private int nivelAnterior = 1; // NUEVO: Para llevar el control del nivel anterior del jugador

    private PlayerController inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;
    private bool attackPressed;
    private bool openInventoryPressed;
    private bool pauseMenuPressed;
    private bool inventarioAbierto = false;
    private bool pausaActiva = false;

    void Awake()
    {
        // Obtener el controlador de salud (componente MonoBehaviour)
        saludControlador = GetComponent<SaludSistemaControlador>();
        if (saludControlador == null)
        {
#if UNITY_EDITOR
            Debug.LogError("JugadorControlador requiere un SaludSistemaControlador.", this);
#endif
            enabled = false;
            return;
        }

        // Crear una instancia de la clase de lógica pura del jugador.
        // Se le pasan los datos configurables desde el Inspector del MonoBehaviour.
        // AGREGADO: fuerzaDisparo al constructor.
        jugadorLogica = new JugadorLogica(saludMaxima, velocidadBase, fuerzaSalto, gravedad, tiempoEntreDisparos, fuerzaDisparo);

        // Inicializar la lógica del personaje (suscribirse a eventos internos de la clase pura).
        jugadorLogica.Inicializar();

        // Suscribirse a los eventos de la lógica del jugador para que este controlador actúe en Unity.
        jugadorLogica.OnMover += AplicarMovimiento;
        jugadorLogica.OnDisparar += InstanciarProyectil; // <--- FIRMA CAMBIADA AQUÍ

        // Suscribirse al evento de muerte del sistema de salud
        saludControlador.Salud.OnMuerte += OnJugadorMuere;
        // Suscribirse al evento de subida de nivel para otorgar monedas automáticamente
        jugadorLogica.Experiencia.OnSubioNivel += OnSubioNivelOtorgarMonedas;

        inputActions = new PlayerController();
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;
        inputActions.Player.Jump.performed += ctx => jumpPressed = true;
        inputActions.Player.Attack.performed += ctx => attackPressed = true;
        inputActions.Player.OpenInventory.performed += ctx => openInventoryPressed = true;
        inputActions.Player.PauseMenu.performed += ctx => pauseMenuPressed = true;
    }

    void Update()
    {
        // Sustituye el input legacy por el nuevo sistema
        float inputHorizontal = moveInput.x;
        bool saltoPresionado = jumpPressed;
        bool disparoPresionado = attackPressed;

        // Detección física de suelo (realizada por el MonoBehaviour)
        bool enSueloDetectado = Physics.CheckSphere(transform.position + checkGroundOffset, checkGroundRadius, groundLayer);

        // Procesar el movimiento en la lógica pura, pasándole la información de Unity.
        jugadorLogica.ProcesarMovimiento(inputHorizontal, saltoPresionado, enSueloDetectado);

        // Procesar el disparo en la lógica pura, pasándole la información de Unity (dirección de disparo).
        // La dirección de disparo se obtiene del Transform del puntoDisparo.
        jugadorLogica.ProcesarDisparo(disparoPresionado, puntoDisparo.forward);

        // Comprobar si el jugador subió de nivel y otorgar monedas
        int nivelActual = jugadorLogica.Experiencia.Nivel;
        if (nivelActual > nivelAnterior)
        {
            jugadorLogica.OtorgarMonedasPorNivel(nivelAnterior, nivelActual);
            nivelAnterior = nivelActual;
        }

        if (openInventoryPressed)
        {
            inventarioAbierto = !inventarioAbierto;
            if (inventarioUI != null)
                inventarioUI.gameObject.SetActive(inventarioAbierto);
        }

        if (pauseMenuPressed)
        {
            pausaActiva = !pausaActiva;
            if (pausaUI != null)
                pausaUI.SetActive(pausaActiva);
            Time.timeScale = pausaActiva ? 0f : 1f;
        }

        // Reset de flags de botones (solo true un frame)
        jumpPressed = false;
        attackPressed = false;
        openInventoryPressed = false;
        pauseMenuPressed = false;
    }

    // Método para aplicar el movimiento al Transform del GameObject (llamado por el evento OnMover).
    private void AplicarMovimiento(Vector3 movimientoTotal)
    {
        transform.position += movimientoTotal;
    }

    // Método para instanciar el proyectil (llamado por el evento OnDisparar).
    // CAMBIADO: Ahora recibe la dirección y la fuerza del disparo.
    private void InstanciarProyectil(Vector3 direccionInicial, float fuerzaInicial)
    {
        if (prefabProyectil != null && puntoDisparo != null)
        {
            GameObject proyectilGO = Instantiate(prefabProyectil, puntoDisparo.position, puntoDisparo.rotation);
            Proyectil_FisicasManuales proyectilScript = proyectilGO.GetComponent<Proyectil_FisicasManuales>();
            if (proyectilScript != null)
            {
                // Pasa la dirección y la fuerza al proyectil.
                proyectilScript.Inicializar(direccionInicial * fuerzaInicial); // <--- FUERZA INICIAL APLICADA AQUÍ
            }
#if UNITY_EDITOR
            Debug.LogWarning("El prefab del proyectil no tiene el script Proyectil_FisicasManuales.");
#endif
        }
    }

    // Método que se activa cuando el jugador muere (escuchando el evento del Salud de la lógica).
    private void OnJugadorMuere()
    {
#if UNITY_EDITOR
        Debug.Log("Controlador del Jugador: El GameObject del jugador ha muerto. Desactivando y respawneando.");
#endif
        if (playerRespawn != null)
        {
            playerRespawn.PlayerDamage();
            saludControlador.Salud.InicializarSalud(); // Opcional: restaurar salud al respawnear
            gameObject.SetActive(true); // Reactivar el jugador tras respawn
        }
        else
        {
            gameObject.SetActive(false); // Fallback: desactivar si no hay respawn
        }
        // Aquí podrías cargar una escena de Game Over, mostrar un mensaje, etc.
    }

    // Cuando el GameObject se deshabilita o destruye, es importante limpiar las suscripciones.
    void OnDisable()
    {
        if (jugadorLogica != null)
        {
            jugadorLogica.OnMover -= AplicarMovimiento;
            jugadorLogica.OnDisparar -= InstanciarProyectil;
            jugadorLogica.Limpiar(); // Limpia también las suscripciones internas de la lógica.
            // Limpiar suscripción al evento de subida de nivel
            jugadorLogica.Experiencia.OnSubioNivel -= OnSubioNivelOtorgarMonedas;
        }
        if (saludControlador != null && saludControlador.Salud != null)
        {
            saludControlador.Salud.OnMuerte -= OnJugadorMuere;
        }
        if (inputActions != null)
        {
            inputActions.Player.Disable();
            inputActions.Dispose();
        }
    }

    // Método público para comprar un producto del mercader desde el controlador Unity
    public bool ComprarProductoDesdeMercader(MercaderProducto producto)
    {
        if (jugadorLogica != null)
        {
            return jugadorLogica.ComprarProducto(producto);
        }
        return false;
    }

    // Propiedad pública para exponer el valor de monedas del jugador de forma segura
    public int Monedas => jugadorLogica != null ? jugadorLogica.Monedas : 0;

    // Exponer JugadorLogica como propiedad pública
    public JugadorLogica JugadorLogica => jugadorLogica;

    // Opcional: Para visualizar la esfera de detección de suelo en el editor
    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + checkGroundOffset, checkGroundRadius);
#endif
    }

    // Manejador para otorgar monedas al subir de nivel
    private void OnSubioNivelOtorgarMonedas(int nuevoNivel)
    {
        jugadorLogica.OtorgarMonedasPorNivel(nivelAnterior, nuevoNivel);
        nivelAnterior = nuevoNivel;
    }
}