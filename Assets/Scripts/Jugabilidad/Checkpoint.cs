using Personajes.Jugador;
using UnityEngine;

/// <summary>
/// Checkpoint que actualiza el punto de respawn del jugador al ser alcanzado.
/// </summary>
public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var respawn = collision.GetComponent<PlayerRespawn>();
        if (respawn != null)
        {
            respawn.ActualizarCheckpoint(this.transform);
            // Aquí puedes agregar feedback visual o de sonido
        }
    }
}
