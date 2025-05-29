using UnityEngine;

/// <summary>
/// Gestiona la experiencia y el nivel del jugador.
/// </summary>
public class ExperienciaJugador
{
    private int nivelActual = 1;
    private int experienciaActual = 0;
    private int experienciaParaSiguienteNivel = 100;

    // Evento para notificar subida de nivel
    public event System.Action<int> OnSubioNivel;
    // Evento para notificar cambio de experiencia
    public event System.Action<int, int> OnExperienciaCambiada;

    public int NivelActual => nivelActual;
    public int ExperienciaActual => experienciaActual;
    public int ExperienciaParaSiguienteNivel => experienciaParaSiguienteNivel;

    public int Nivel => nivelActual;

    public void GanarExperiencia(int cantidad)
    {
        if (nivelActual >= 15)
            return; // Nivel máximo alcanzado, no se gana más experiencia
        experienciaActual += cantidad;
        while (experienciaActual >= experienciaParaSiguienteNivel && nivelActual < 15)
        {
            experienciaActual -= experienciaParaSiguienteNivel;
            nivelActual++;
            experienciaParaSiguienteNivel = CalcularExperienciaParaNivel(nivelActual);
            OnSubioNivel?.Invoke(nivelActual);
        }
        if (nivelActual >= 15)
        {
            experienciaActual = 0;
            experienciaParaSiguienteNivel = 0;
        }
        OnExperienciaCambiada?.Invoke(experienciaActual, experienciaParaSiguienteNivel);
    }

    private int CalcularExperienciaParaNivel(int nivel)
    {
        // Siempre redondea hacia arriba a entero
        return Mathf.CeilToInt(100 * Mathf.Pow(1.1f, nivel - 1));
    }
}
