using UnityEngine;

namespace Personajes.Jugador
{
    /// <summary>
    /// Gestiona el respawn del jugador en el último checkpoint alcanzado o en el punto inicial si no hay checkpoint.
    /// </summary>
    public class PlayerRespawn : MonoBehaviour
    {
        [Header("Punto de respawn inicial (si no hay checkpoint)")]
        [SerializeField] private Transform puntoInicialRespawn;

        private Transform ultimoCheckpoint;
        private Transform jugador;

        private void Awake()
        {
            jugador = this.transform;
            ultimoCheckpoint = puntoInicialRespawn;
        }

        /// <summary>
        /// Llama este método cuando el jugador muere.
        /// </summary>
        public void PlayerDamage()
        {
            Respawnear();
        }

        /// <summary>
        /// Llama este método desde el checkpoint cuando el jugador lo alcanza.
        /// </summary>
        public void ActualizarCheckpoint(Transform nuevoCheckpoint)
        {
            ultimoCheckpoint = nuevoCheckpoint;
        }

        private void Respawnear()
        {
            if (ultimoCheckpoint != null)
                jugador.position = ultimoCheckpoint.position;
            // Reiniciar la salud al máximo (solo en respawn, nunca durante el juego)
            var saludControlador = jugador.GetComponent<SaludSistemaControlador>();
            if (saludControlador != null && saludControlador.Salud != null)
                saludControlador.Salud.InicializarSalud();
            // Aquí puedes resetear animaciones, estados, etc.
            // NOTA: No existe curación durante el juego, solo respawn tras la muerte.
        }
    }
}
