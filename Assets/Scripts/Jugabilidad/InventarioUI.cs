using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Personajes.Jugador;
using Jugabilidad;

public class InventarioUI : MonoBehaviour
{
    [SerializeField] private Transform panelItems;
    [SerializeField] private GameObject prefabBotonItem;
    [SerializeField] private JugadorControlador jugadorControlador;
    [SerializeField] private Text textoArmaEquipada;
    [SerializeField] private Text textoChalecoEquipado;
    [SerializeField] private Text textoCascoEquipado;
    [SerializeField] private Text textoBotasEquipadas;

    private void OnEnable()
    {
        ActualizarUI();
    }

    public void ActualizarUI()
    {
        foreach (Transform child in panelItems)
            Destroy(child.gameObject);

        var jugadorLogica = jugadorControlador.JugadorLogica;
        if (jugadorLogica == null) return;

        // Mostrar arma equipada
        if (textoArmaEquipada != null)
        {
            textoArmaEquipada.text = jugadorLogica.ArmaEquipada != null ?
                $"Arma equipada: {jugadorLogica.ArmaEquipada.Nombre}" :
                "Sin arma equipada";
        }
        if (textoChalecoEquipado != null)
        {
            textoChalecoEquipado.text = jugadorLogica.ChalecoEquipado != null ?
                $"Chaleco equipado: {jugadorLogica.ChalecoEquipado.Nombre}" :
                "Sin chaleco equipado";
        }
        if (textoCascoEquipado != null)
        {
            textoCascoEquipado.text = jugadorLogica.CascoEquipado != null ?
                $"Casco equipado: {jugadorLogica.CascoEquipado.Nombre}" :
                "Sin casco equipado";
        }
        if (textoBotasEquipadas != null)
        {
            textoBotasEquipadas.text = jugadorLogica.BotasEquipadas != null ?
                $"Botas equipadas: {jugadorLogica.BotasEquipadas.Nombre}" :
                "Sin botas equipadas";
        }

        // Mostrar items en inventario
        foreach (var item in jugadorLogica.Items)
        {
            var botonGO = Instantiate(prefabBotonItem, panelItems);
            var texto = botonGO.GetComponentInChildren<Text>();
            if (texto != null)
                texto.text = $"{item.Nombre} ({item.Tipo})";
            var boton = botonGO.GetComponent<Button>();
            if (boton != null)
            {
                var itemCierre = item;
                boton.onClick.AddListener(() => OnClickEquipar(itemCierre));
            }
        }
    }

    private void OnClickEquipar(Item item)
    {
        var jugadorLogica = jugadorControlador.JugadorLogica;
        if (jugadorLogica.EquiparItem(item))
        {
            ActualizarUI();
        }
    }
}
