// SaludSistemaControlador.cs
using UnityEngine;

public class SaludSistemaControlador : MonoBehaviour
{
    [SerializeField]
    private int saludMaximaInicial = 100; // Valor inicial configurable en el Inspector

    // Esta es la instancia de la clase pura de salud.
    // [System.NonSerialized] para evitar que Unity intente serializarla (porque la creamos nosotros en Awake).
    // [SerializeField] si queremos que la inicialice el inspector de alguna manera, pero no para este caso.
    public Salud Salud { get; private set; }

    void Awake()
    {
        // Crea una nueva instancia de la clase pura Salud.
        Salud = new Salud(saludMaximaInicial);

        // Opcional: Suscribirse a eventos de la Salud para debug o lógica visual en este controlador.
        // Salud.OnSaludCambiada += (actual, max) => Debug.Log($"Salud de {gameObject.name}: {actual}/{max}");
        // Salud.OnMuerte += () => Debug.Log($"Controlador de Salud: {gameObject.name} ha muerto.");
    }

    // Puedes agregar métodos aquí para que otros MonoBehaviours de Unity interactúen con la salud.
    // Por ejemplo, un collider que inflija daño al tocarlo.
    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("EnemyAttack"))
    //     {
    //         Salud.RecibirDano(10);
    //     }
    // }

    public void RecibirDano(int cantidad)
    {
        if (Salud != null)
            Salud.RecibirDano(cantidad);
    }
}