using UnityEngine;
using Jugabilidad;
using Interfaces;
using System.Collections.Generic;
using UnityEngine.InputSystem;

/// <summary>
/// Componente para el mercader en el escenario que permite la compra de productos (perks e items).
/// </summary>
public class Mercader : MonoBehaviour
{
    [Header("Items en venta (ScriptableObjects)")]
    [SerializeField] private ItemData[] itemsEnVenta;
    [Header("Perks en venta (ScriptableObjects)")]
    [SerializeField] private PerkData[] perksEnVenta;
    [Header("Perks con tipo en venta (ScriptableObjects)")]
    [SerializeField] private PerkConTipoData[] perksConTipoEnVenta;
    [SerializeField] private GameObject mercaderUI; // Asigna el panel de la UI del mercader en el inspector

    private bool jugadorDentro = false;
    private JugadorControlador jugadorActual;

    // Devuelve todos los productos en venta como instancias de MercaderProducto
    public List<MercaderProducto> ProductosEnVenta
    {
        get
        {
            var productos = new List<MercaderProducto>();
            foreach (var itemData in itemsEnVenta)
                productos.Add(ItemDesdeData(itemData));
            foreach (var perkData in perksEnVenta)
                productos.Add(PerkDesdeData(perkData));
            foreach (var perkConTipoData in perksConTipoEnVenta)
                productos.Add(PerkConTipoDesdeData(perkConTipoData));
            return productos;
        }
    }

    // Convierte un ItemData a una instancia de Item
    private Item ItemDesdeData(ItemData data)
    {
        // Asegúrate de que ItemData tenga un campo público: public ItemTipo tipo;
        return new Item(
            data.id,
            data.itemName,
            data.description,
            data.precio,
            data.modAtaque,
            data.modSalud,
            data.modVelocidad,
            data.modDefensa,
            data.tipo 
        );
    }

    // Convierte un PerkData a una instancia de Perk
    private Perk PerkDesdeData(PerkData data)
    {
        return new Perk(
            data.id,
            data.perkName,
            data.description,
            data.precio
        );
    }

    // Convierte un PerkConTipoData a una instancia de PerkConTipo
    private PerkConTipo PerkConTipoDesdeData(PerkConTipoData data)
    {
        return new PerkConTipo(
            data.id,
            data.perkName,
            data.description,
            data.precio,
            data.tipo,
            data.modificador
        );
    }

    // Método genérico para comprar un producto
    public bool ComprarProducto(object comprador, string idProducto)
    {
        var producto = ProductosEnVenta.Find(p => p.Id == idProducto);
        if (producto != null)
        {
            if (producto is Perk perk && comprador is IPerkeable perkeable)
            {
                perkeable.AgregarPerk(perk);
                return true;
            }
            else if (producto is Item item && comprador is IInventariable inventariable)
            {
                inventariable.AgregarItem(item);
                return true;
            }
            else if (producto is PerkConTipo perkConTipo && comprador is IPerkeable perkeableConTipo)
            {
                perkeableConTipo.AgregarPerk(perkConTipo);
                return true;
            }
        }
        return false;
    }

    public List<MercaderProducto> GetProductosEnVenta(IPerkeable perkeable)
    {
        var productos = new List<MercaderProducto>();
        foreach (var itemData in itemsEnVenta)
            productos.Add(ItemDesdeData(itemData));
        foreach (var perkData in perksEnVenta)
        {
            var perk = PerkDesdeData(perkData);
            if (!perkeable.TienePerk(perk.Id))
                productos.Add(perk);
        }
        return productos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = true;
            jugadorActual = other.GetComponent<JugadorControlador>();
            if (jugadorActual != null)
            {
                var input = jugadorActual.GetComponent<PlayerInput>();
                if (input != null)
                    input.actions["Interact"].performed += OnInteract;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = false;
            if (jugadorActual != null)
            {
                var input = jugadorActual.GetComponent<PlayerInput>();
                if (input != null)
                    input.actions["Interact"].performed -= OnInteract;
            }
            jugadorActual = null;
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (jugadorDentro)
            AbrirUI();
    }

    public void AbrirUI()
    {
        if (mercaderUI != null)
        {
            mercaderUI.SetActive(true);
            Time.timeScale = 0f;
            if (jugadorActual != null)
            {
                var playerInput = jugadorActual.GetComponent<UnityEngine.InputSystem.PlayerInput>();
                if (playerInput != null)
                    playerInput.SwitchCurrentActionMap("UI");
            }
        }
    }

    // Llama esto desde el botón "Volver al juego" en la UI del mercader
    public void CerrarUI()
    {
        if (mercaderUI != null)
        {
            mercaderUI.SetActive(false);
            Time.timeScale = 1f;
            if (jugadorActual != null)
            {
                var playerInput = jugadorActual.GetComponent<UnityEngine.InputSystem.PlayerInput>();
                if (playerInput != null)
                    playerInput.SwitchCurrentActionMap("Player");
            }
        }
    }
}
