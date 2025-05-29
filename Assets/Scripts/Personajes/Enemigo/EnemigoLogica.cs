// EnemigoLogica.cs
using UnityEngine;
using System;
using System.Collections.Generic;

// Enum para patrones de comportamiento de enemigos
public enum PatronEnemigo
{
    PatrullaSimple,
    PerseguidorLento,
    SprinterAgresivo
}

// Clase pura de lógica para enemigos (no MonoBehaviour)
public class EnemigoLogica : Personaje
{
    // Eventos para que el controlador escuche y mueva el GameObject
    public event Action<Vector3> OnMover;

    // Patrullaje
    private readonly Vector3[] puntosPatrulla;
    private int indicePatrullaActual;
    private float velocidadPatrulla;

    // Detección y persecución
    private float rangoAgro;
    private float rangoAtaque;

    private PatronEnemigo patron;

    private List<Vector3> caminoActual; // Ruta calculada por A*
    private int indiceCamino;
    private float toleranciaDestino = 0.2f;

    // Variables para físicas personalizadas
    private Vector3 velocidadActual = Vector3.zero;
    private bool estaEnSuelo = false;

    public int AtaqueTotal => Ataque;
    public int DefensaCuerpoTotal => Defensa;
    public float VelocidadMovimientoTotal => VelocidadMovimiento;
    public int DefensaProyectilTotal => Defensa;

    public EnemigoLogica(int saludMaxima, float velocidadBase, Vector3[] puntosPatrulla, float velocidadPatrulla, float rangoAgro, float rangoAtaque, PatronEnemigo patron, int ataque = 10, int defensa = 0, float velocidadMovimiento = 1f)
        : base(saludMaxima, velocidadBase, ataque, defensa, velocidadMovimiento)
    {
        this.puntosPatrulla = puntosPatrulla;
        this.velocidadPatrulla = velocidadPatrulla;
        this.rangoAgro = rangoAgro;
        this.rangoAtaque = rangoAtaque;
        this.indicePatrullaActual = 0;
        this.patron = patron;
    }

    // Lógica de actualización (llamar desde el controlador)
    public void Actualizar(Vector3 posicionActual, Vector3 posicionJugador, AStarPathfinding.Nodo nodoActual = null, AStarPathfinding.Nodo nodoObjetivo = null)
    {
        float distanciaJugador = Vector3.Distance(posicionActual, posicionJugador);
        // Simulación de físicas personalizadas
        if (!estaEnSuelo)
        {
            velocidadActual = FisicasProyecto.AplicarGravedad(velocidadActual, Time.deltaTime);
            velocidadActual = FisicasProyecto.AplicarArrastre(velocidadActual, 0.01f, Time.deltaTime);
        }
        else
        {
            velocidadActual = FisicasProyecto.AplicarFriccion(velocidadActual, 0.5f, 1f, Time.deltaTime);
            velocidadActual = new Vector3(velocidadActual.x, -0.1f, velocidadActual.z);
        }

        bool jugadorVisible = false;
        if (distanciaJugador <= rangoAgro)
        {
            // Comprobación de línea de visión (raycast)
            Vector3 origen = posicionActual + Vector3.up * 1.0f;
            Vector3 destino = posicionJugador + Vector3.up * 1.0f;
            Vector3 direccion = (destino - origen).normalized;
            float distancia = Vector3.Distance(origen, destino);
            if (!Physics.Raycast(origen, direccion, distancia, LayerMask.GetMask("Default"))) // Ajusta el LayerMask según tu escenario
            {
                jugadorVisible = true;
            }
        }

        switch (patron)
        {
            case PatronEnemigo.PatrullaSimple:
                Patrullar(posicionActual);
                break;
            case PatronEnemigo.PerseguidorLento:
            case PatronEnemigo.SprinterAgresivo:
                if (distanciaJugador <= rangoAgro && jugadorVisible)
                {
                    // Si se proveen nodos, usa A*
                    if (nodoActual != null && nodoObjetivo != null)
                    {
                        if (caminoActual == null || caminoActual.Count == 0 || Vector3.Distance(caminoActual[caminoActual.Count - 1], nodoObjetivo.posicion) > toleranciaDestino)
                        {
                            caminoActual = AStarPathfinding.EncontrarCamino(nodoActual, nodoObjetivo);
                            indiceCamino = 0;
                        }
                        AvanzarPorCamino(posicionActual);
                    }
                    else
                    {
                        // Movimiento directo hacia el jugador
                        Vector3 direccion = (posicionJugador - posicionActual).normalized;
                        velocidadActual = FisicasProyecto.AplicarAceleracion(velocidadActual, direccion * velocidadPatrulla, Time.deltaTime);
                    }
                }
                else
                {
                    Patrullar(posicionActual);
                }
                break;
        }
        // Notificar movimiento
        OnMover?.Invoke(velocidadActual);
    }

    private void AvanzarPorCamino(Vector3 posicionActual)
    {
        if (caminoActual == null || caminoActual.Count == 0 || indiceCamino >= caminoActual.Count) return;
        Vector3 destino = caminoActual[indiceCamino];
        Vector3 direccion = (destino - posicionActual).normalized;
        float factorVel = patron == PatronEnemigo.SprinterAgresivo ? 1.8f : 0.7f;
        OnMover?.Invoke(direccion * (VelocidadBase * factorVel));
        if (Vector3.Distance(posicionActual, destino) < toleranciaDestino)
        {
            indiceCamino++;
        }
    }

    private void Patrullar(Vector3 posicionActual)
    {
        if (puntosPatrulla == null || puntosPatrulla.Length == 0) return;
        Vector3 destino = puntosPatrulla[indicePatrullaActual];
        Vector3 direccion = (destino - posicionActual).normalized;
        OnMover?.Invoke(direccion * velocidadPatrulla);
        if (Vector3.Distance(posicionActual, destino) < 0.2f)
        {
            indicePatrullaActual = (indicePatrullaActual + 1) % puntosPatrulla.Length;
        }
    }

    // Implementación del método abstracto de ataque para enemigos
    public override void Atacar(Vector3 objetivo)
    {
        if (!Salud.EstaVivo) return;
        if (PuedeAtacar())
        {
            // Aquí puedes agregar la lógica de ataque del enemigo (ejemplo: daño cuerpo a cuerpo, disparo, etc.)
            // Por ejemplo, podrías lanzar un evento, aplicar daño directo, o iniciar una animación:
            // OnAtacar?.Invoke(); // Si decides volver a usar eventos
            tiempoUltimoAtaque = Time.time;
        }
    }

    protected override void OnMuertePersonaje()
    {
        // Aquí puedes agregar lógica de muerte específica del enemigo
    }
}
// No se requiere cambio directo aquí, ya que Personaje implementa las interfaces.
// Si necesitas lógica adicional, puedes sobreescribir Inicializar() o Limpiar().
