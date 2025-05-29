using UnityEngine;


/// <summary>
/// Controlador de entidad mortal para enemigos y objetos destruibles.
/// Gestiona pooling o destrucción según configuración.
/// </summary>
public class EntidadNoJugador : EntidadMortal
{
    [SerializeField] private bool usarPooling = false;

    /// <summary>
    /// Al morir, devuelve al pool o destruye el objeto según corresponda.
    /// </summary>
    protected override void OnMuerte()
    {
        if (usarPooling)
        {
            gameObject.SetActive(false);
            var salud = GetComponent<SaludSistemaControlador>();
            if (salud != null) salud.Salud.InicializarSalud();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
