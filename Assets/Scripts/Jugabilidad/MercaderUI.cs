using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Jugabilidad;
using Personajes.Jugador;

public class MercaderUI : MonoBehaviour
{
    [SerializeField] private Mercader mercader;
    [SerializeField] private JugadorControlador jugadorControlador;
    [SerializeField] private Transform panelProductos; // Panel donde se instancian los botones
    [SerializeField] private GameObject prefabBotonProducto; // Prefab de botón con componente Button y Text
    [SerializeField] private GameObject panelFeedback;
    [SerializeField] private Text textoFeedback;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoCompraExitosa;
    [SerializeField] private AudioClip sonidoCompraFallida;
    [SerializeField] private AudioClip sonidoMaxNivel;
    [SerializeField] private GameObject panelConfirmacionPerk;
    [SerializeField] private Transform panelPerksActuales;
    [SerializeField] private GameObject prefabBotonPerkActual;
    private Perk perkPendienteCompra;

    void Start()
    {
        ActualizarUI();
    }

    private void OnEnable()
    {
        if (jugadorControlador != null && jugadorControlador.JugadorLogica != null)
        {
            jugadorControlador.JugadorLogica.Experiencia.OnSubioNivel += OnSubioNivel;
        }
        ActualizarUI();
    }

    private void OnDisable()
    {
        if (jugadorControlador != null && jugadorControlador.JugadorLogica != null)
        {
            jugadorControlador.JugadorLogica.Experiencia.OnSubioNivel -= OnSubioNivel;
        }
    }

    public void ActualizarUI()
    {
        foreach (Transform child in panelProductos)
            Destroy(child.gameObject);

        var jugadorLogica = jugadorControlador.JugadorLogica;
        if (jugadorLogica == null) return;

        // Mostrar solo perks que el jugador no tiene
        var productos = mercader.GetProductosEnVenta(jugadorLogica);
        int monedas = jugadorControlador != null ? jugadorControlador.Monedas : 0;
        foreach (var producto in productos)
        {
            var botonGO = Instantiate(prefabBotonProducto, panelProductos);
            var texto = botonGO.GetComponentInChildren<Text>();
            if (texto != null)
                texto.text = $"{producto.Nombre} - {producto.Precio} monedas";
            var boton = botonGO.GetComponent<Button>();
            bool puedeComprar = monedas >= producto.Precio;
            if (boton != null)
            {
                var prod = producto;
                boton.interactable = puedeComprar;
                boton.onClick.AddListener(() => ComprarProducto(prod));
            }
        }
    }

    void ComprarProducto(MercaderProducto producto)
    {
        if (producto is Perk perk)
        {
            var jugadorLogica = jugadorControlador.JugadorLogica;
            if (jugadorLogica.PerksEquipados.Count() >= Jugabilidad.SistemaPerks.MaxPerks)
            {
                // Mostrar panel de confirmación para reemplazo
                perkPendienteCompra = perk;
                MostrarPanelConfirmacionPerk(jugadorLogica);
                return;
            }
            // Si hay espacio, equipar directamente
            if (jugadorLogica.TryEquiparPerk(perk))
            {
                jugadorLogica.GastarMonedas(perk.Precio);
                MostrarFeedback("¡Perk equipado!", Color.green, sonidoCompraExitosa);
                ActualizarUI();
            }
            else
            {
                // Nueva lógica: feedback específico para perks de tipo
                if (perk is PerkConTipo nuevoPerkConTipo)
                {
                    // Buscar si ya hay un perk de ese tipo equipado
                    var perkExistente = jugadorLogica.PerksEquipados.FirstOrDefault(p => p is PerkConTipo pct && pct.Tipo == nuevoPerkConTipo.Tipo) as PerkConTipo;
                    if (perkExistente != null)
                    {
                        if (nuevoPerkConTipo.Modificador <= perkExistente.Modificador)
                        {
                            MostrarFeedback($"No puedes equipar un perk de {nuevoPerkConTipo.Tipo} inferior o igual al que ya tienes (actual: {perkExistente.Modificador}, nuevo: {nuevoPerkConTipo.Modificador})", Color.yellow, sonidoCompraFallida);
                            return;
                        }
                    }
                }
                MostrarFeedback("No se pudo equipar el perk.", Color.red, sonidoCompraFallida);
            }
            return;
        }
        bool exito = jugadorControlador.ComprarProductoDesdeMercader(producto);
        if (exito)
        {
            MostrarFeedback("¡Compra exitosa!", Color.green, sonidoCompraExitosa);
        }
        else
        {
            MostrarFeedback("No se pudo comprar el producto.", Color.red, sonidoCompraFallida);
        }
        ActualizarUI();
    }

    private void OnSubioNivel(int nuevoNivel)
    {
        ActualizarUI();
        if (jugadorControlador.JugadorLogica.Experiencia.Nivel == 15)
        {
            MostrarFeedback("¡Nivel máximo alcanzado!", Color.yellow, sonidoMaxNivel);
        }
    }

    void MostrarPanelConfirmacionPerk(Personajes.Jugador.JugadorLogica jugadorLogica)
    {
        panelConfirmacionPerk.SetActive(true);
        foreach (Transform child in panelPerksActuales)
            Destroy(child.gameObject);
        foreach (var perk in jugadorLogica.PerksEquipados)
        {
            var botonGO = Instantiate(prefabBotonPerkActual, panelPerksActuales);
            var texto = botonGO.GetComponentInChildren<Text>();
            if (texto != null)
                texto.text = perk.Nombre;
            var boton = botonGO.GetComponent<Button>();
            if (boton != null)
            {
                var perkAEliminar = perk;
                boton.onClick.AddListener(() => ConfirmarReemplazoPerk(perkAEliminar));
            }
        }
    }

    void ConfirmarReemplazoPerk(Perk perkAEliminar)
    {
        var jugadorLogica = jugadorControlador.JugadorLogica;
        if (jugadorLogica.ReemplazarPerk(perkPendienteCompra, perkAEliminar))
        {
            jugadorLogica.GastarMonedas(perkPendienteCompra.Precio);
            MostrarFeedback($"Perk reemplazado por {perkPendienteCompra.Nombre}", Color.green, sonidoCompraExitosa);
        }
        else
        {
            MostrarFeedback("No se pudo reemplazar el perk.", Color.red, sonidoCompraFallida);
        }
        panelConfirmacionPerk.SetActive(false);
        ActualizarUI();
    }

    public void BotonVolverAlJuego()
    {
        var mercader = FindAnyObjectByType<Mercader>();
        if (mercader != null)
            mercader.CerrarUI();
    }

    private void MostrarFeedback(string mensaje, Color color, AudioClip sonido)
    {
        if (panelFeedback != null && textoFeedback != null)
        {
            panelFeedback.SetActive(true);
            textoFeedback.text = mensaje;
            textoFeedback.color = color;
            CancelInvoke(nameof(EsconderFeedback));
            Invoke(nameof(EsconderFeedback), 1.5f);
        }
        if (audioSource != null && sonido != null)
        {
            audioSource.PlayOneShot(sonido);
        }
    }

    private void EsconderFeedback()
    {
        if (panelFeedback != null)
            panelFeedback.SetActive(false);
    }
}
