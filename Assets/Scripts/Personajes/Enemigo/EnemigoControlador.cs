// EnemigoControlador.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(SaludSistemaControlador))]
public class EnemigoControlador : MonoBehaviour
{
    [Header("Configuración del Enemigo")]
    [SerializeField] private int saludMaxima = 50;
    [SerializeField] private float velocidadBase = 2f;
    [SerializeField] private float velocidadPatrulla = 1.5f;
    [SerializeField] private float rangoAgro = 5f;
    [SerializeField] private float rangoAtaque = 1.2f;
    [SerializeField] private Transform[] puntosPatrulla;

    [Header("Patrón de Comportamiento")]
    [SerializeField] private PatronEnemigo patron = PatronEnemigo.PatrullaSimple;

    private EnemigoLogica enemigoLogica;
    private SaludSistemaControlador saludControlador;
    private Transform jugador;

    void Awake()
    {
        saludControlador = GetComponent<SaludSistemaControlador>();
        Vector3[] waypoints = new Vector3[puntosPatrulla.Length];
        for (int i = 0; i < puntosPatrulla.Length; i++)
            waypoints[i] = puntosPatrulla[i].position;
        enemigoLogica = new EnemigoLogica(saludMaxima, velocidadBase, waypoints, velocidadPatrulla, rangoAgro, rangoAtaque, patron);
        enemigoLogica.Inicializar();
        enemigoLogica.OnMover += AplicarMovimiento;
    }

    void Update()
    {
        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                jugador = playerObj.transform;
        }
        if (jugador != null)
        {
            // Si implementas pathfinding, aquí deberías calcular los nodos actuales y objetivo
            // y pasarlos a enemigoLogica.Actualizar. Por ahora, solo pasamos posiciones.
            enemigoLogica.Actualizar(transform.position, jugador.position);
        }
    }

    private void AplicarMovimiento(Vector3 movimiento)
    {
        transform.position += movimiento * Time.deltaTime;
    }

    void OnDestroy()
    {
        if (enemigoLogica != null)
            enemigoLogica.Limpiar();
    }
}
