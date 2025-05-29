using Interfaces;
using UnityEngine;

namespace Entorno
{
    /// <summary>
    /// Componente para objetos del entorno que pueden ser destruidos (cajas, barriles, etc.).
    /// Implementa IAtacable para recibir daño de cualquier entidad.
    /// </summary>
    [RequireComponent(typeof(SaludSistemaControlador))]
    public class ObjetoDestruible : EntidadNoJugador, IAtacable
    {
        [Header("Efectos de destrucción")]
        [SerializeField] private GameObject efectoDestruccion;
        [SerializeField] private AudioClip sonidoDestruccion;

        // Permite que cualquier entidad (jugador, enemigo, proyectil) le haga daño
        public void RecibirDano(int cantidad)
        {
            var salud = GetComponent<SaludSistemaControlador>();
            if (salud != null)
                salud.RecibirDano(cantidad);
        }

        protected override void OnMuerte()
        {
            // Efecto visual
            if (efectoDestruccion != null)
                Instantiate(efectoDestruccion, transform.position, Quaternion.identity);
            // Sonido
            if (sonidoDestruccion != null)
                AudioSource.PlayClipAtPoint(sonidoDestruccion, transform.position);
            // Pooling o destrucción
            base.OnMuerte();
        }
        // Puedes extender aquí para loot, animaciones, etc.
    }
}
