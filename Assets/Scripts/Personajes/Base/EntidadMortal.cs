// EntidadMortal.cs
using UnityEngine;
using Personajes.Jugador;

/// <summary>
/// Clase base abstracta para entidades que pueden morir.
/// Gestiona la suscripción al evento de muerte y delega la acción concreta a las subclases.
/// </summary>
public abstract class EntidadMortal : MonoBehaviour
{
    protected SaludSistemaControlador saludControlador;

    protected virtual void Awake()
    {
        saludControlador = GetComponent<SaludSistemaControlador>();
        if (saludControlador != null && saludControlador.Salud != null)
        {
            // Suscribirse al evento de muerte de la lógica pura
            saludControlador.Salud.OnMuerte += OnMuerte;
        }
    }

    /// <summary>
    /// Acción a ejecutar cuando la entidad muere. Implementar en subclases.
    /// </summary>
    protected abstract void OnMuerte();

    protected virtual void OnDestroy()
    {
        if (saludControlador != null && saludControlador.Salud != null)
        {
            // Desuscribirse del evento de muerte
            saludControlador.Salud.OnMuerte -= OnMuerte;
        }
    }
}
