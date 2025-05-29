// Personaje.cs
using UnityEngine; // Todavía necesitamos Vector3 para algunos tipos de datos
using System; // Para interfaces

// Clase base abstracta pura de C# para cualquier personaje.
public abstract class Personaje : IAtacable, IInicializable, ILimpiable
{
    // Las propiedades se pueden hacer públicas, o tener setters protegidos si se necesitan mutaciones.
    public Salud Salud { get; protected set; } // Composición: un Personaje "tiene una" Salud
    public float VelocidadBase { get; protected set; }
    public int Ataque { get; protected set; }
    public int Defensa { get; protected set; }
    public float VelocidadMovimiento { get; protected set; }

    // Cooldown genérico para ataques (puede ser sobrescrito por subclases)
    protected float tiempoUltimoAtaque = -999f;
    protected float cooldownAtaque = 1f; // Valor por defecto, sobrescribir en subclases

    // Constructor base para Personaje
    public Personaje(int saludMaxima, float velocidadBase, int ataque = 10, int defensa = 0, float velocidadMovimiento = 1f)
    {
        this.Salud = new Salud(saludMaxima); // Inicializa el sistema de salud
        this.VelocidadBase = velocidadBase;
        this.Ataque = ataque;
        this.Defensa = defensa;
        this.VelocidadMovimiento = velocidadMovimiento;
    }

    // Método virtual para que un personaje reciba daño a través de su sistema de salud
    public virtual void RecibirDano(int cantidad)
    {
        Salud?.RecibirDano(cantidad);
    }

    // Método abstracto para el comportamiento específico al morir.
    // Este se llamará cuando el Salud Sistema de la clase pura detecte la muerte.
    protected abstract void OnMuertePersonaje();

    // Método de inicialización que el MonoBehaviour "Controlador" llamará.
    // Esto es crucial para configurar las suscripciones a eventos de la clase pura.
    public virtual void Inicializar()
    {
        // Suscribirse al evento de muerte del sistema de salud del personaje.
        // Cuando el sistema de salud interna detecte la muerte, llamará a OnMuertePersonaje.
        Salud.OnMuerte += OnMuertePersonaje;
    }

    // Método para limpiar suscripciones, llamado por el MonoBehaviour controlador.
    public virtual void Limpiar()
    {
        if (Salud != null)
        {
            Salud.OnMuerte -= OnMuertePersonaje;
        }
    }

    // Método abstracto para atacar (debe ser implementado por cada tipo de personaje)
    public abstract void Atacar(Vector3 objetivo);

    // Método para verificar si puede atacar según cooldown
    public virtual bool PuedeAtacar()
    {
        return (Time.time - tiempoUltimoAtaque) >= cooldownAtaque;
    }

    // MÉTODOS DE EXTENSIÓN PARA FUTURAS MECÁNICAS COMUNES

    // Defensa: permite que cualquier personaje tenga lógica de defensa personalizada
    public virtual void Defender(int cantidad)
    {
        // Por defecto, no hay defensa. Las subclases pueden sobrescribir.
    }

    // Estados: ejemplo de método virtual para aplicar estados (stun, slow, etc.)
    public virtual void AplicarEstado(string estado, float duracion)
    {
        // Por defecto, no hace nada. Las subclases pueden sobrescribir.
    }

    // Efectos: ejemplo de método virtual para efectos especiales (quemadura, veneno, etc.)
    public virtual void AplicarEfecto(string efecto, float intensidad, float duracion)
    {
        // Por defecto, no hace nada. Las subclases pueden sobrescribir.
    }

    // NOTA: Si se requieren mecánicas más complejas, considerar interfaces como IDefendible, IEstado, IEfecto
}