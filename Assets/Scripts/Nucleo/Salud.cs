// Salud.cs
using UnityEngine; // Necesario para Mathf
using System; // Necesario para Action

// [System.Serializable] // Puedes agregar esto si necesitas serializar instancias de esta clase en el Inspector de un MonoBehaviour
public class Salud
{
    private int saludMaxima;
    private int saludActual;

    // Eventos (Action) para notificar cambios. Estos eventos son parte de la clase pura.
    public event Action<int, int> OnSaludCambiada; // int actual, int max
    public event Action OnMuerte;

    // Propiedades públicas de solo lectura
    public int SaludActual => saludActual;
    public int SaludMaxima => saludMaxima;
    public bool EstaVivo => saludActual > 0;

    // Constructor de la clase Salud
    public Salud(int maxSalud)
    {
        this.saludMaxima = maxSalud;
        InicializarSalud();
    }

    // Método para inicializar o resetear la salud
    public void InicializarSalud()
    {
        saludActual = saludMaxima;
        // Notificar a los suscriptores. El operador ?.Invoke() es para Thread-safety
        OnSaludCambiada?.Invoke(saludActual, saludMaxima);
    }

    // Método para recibir daño
    public void RecibirDano(int cantidad)
    {
        if (!EstaVivo) return;

        saludActual -= cantidad;
        saludActual = Mathf.Max(saludActual, 0); // Asegura que la salud no baje de 0

        OnSaludCambiada?.Invoke(saludActual, saludMaxima); // Notificar cambio de salud

        if (saludActual <= 0)
        {
            Morir();
        }
    }

    // Método para curar (deshabilitado: el personaje jugable no puede curarse)
    // public void Curar(int cantidad)
    // {
    //     if (!EstaVivo) return;
    //
    //     saludActual += cantidad;
    //     saludActual = Mathf.Min(saludActual, saludMaxima); // Asegura que la salud no exceda la máxima
    //
    //     OnSaludCambiada?.Invoke(saludActual, saludMaxima); // Notificar cambio de salud
    // }

    // Lógica de muerte. Esto es lo que la clase Salud sabe hacer al morir.
    // La reacción específica del GameObject (desactivar, animar) será manejada por el MonoBehaviour.
    private void Morir()
    {
        OnMuerte?.Invoke(); // Notificar a los suscriptores que ha muerto
    }

    // Métodos de inicialización y limpieza
    public void Inicializar()
    {
        InicializarSalud();
    }

    public void Limpiar()
    {
        OnSaludCambiada = null;
        OnMuerte = null;
    }

    // NOTA: En este proyecto, el personaje jugable no puede curarse. Si se requiere curación para otros personajes, reactivar el método Curar.
}