// Proyectil_FisicasManuales.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Proyectil_FisicasManuales : MonoBehaviour
{
    [SerializeField] private float duracionVida = 3f;
    [SerializeField] private int dano = 10;
    [SerializeField] private float gravedadProyectil = 9.8f;

    private Vector3 velocidadActual;
    private float tiempoInicio;

    public void Inicializar(Vector3 velocidadInicial)
    {
        this.velocidadActual = velocidadInicial;
        tiempoInicio = Time.time;
    }

    void Update()
    {
        velocidadActual = FisicasProyecto.AplicarGravedad(velocidadActual, Time.deltaTime, gravedadProyectil);
        transform.position += velocidadActual * Time.deltaTime;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.2f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == gameObject || hitCollider.gameObject.CompareTag("Player"))
            {
                continue;
            }

            // Intenta obtener el COMPONENTE IAtacable del objeto colisionado
            IAtacable atacable = hitCollider.GetComponent<IAtacable>();
            if (atacable != null)
            {
                atacable.RecibirDano(dano);
            }

            if (!hitCollider.isTrigger)
            {
                Destroy(gameObject);
                return;
            }
        }

        if (Time.time - tiempoInicio >= duracionVida)
        {
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
#endif
    }
}