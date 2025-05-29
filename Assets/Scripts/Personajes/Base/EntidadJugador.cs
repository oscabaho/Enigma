using UnityEngine;
using Personajes.Jugador;

/// <summary>
/// Controlador de entidad mortal para el jugador. Gestiona el respawn.
/// </summary>
public class EntidadJugador : EntidadMortal
{
    [SerializeField] private PlayerRespawn respawn;

    /// <summary>
    /// Al morir, delega el respawn al sistema correspondiente.
    /// </summary>
    protected override void OnMuerte()
    {
        if (respawn != null)
        {
            respawn.PlayerDamage();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
