using UnityEngine;
using Personajes.Jugador;

public static class GestorMuertePersonaje
{
    private static int contadorMinions = 0;
    private const int experienciaPorGrupoMinions = 5;
    private const int minionsPorGrupo = 3;
    private const int experienciaPorBoss = 10;
    // Referencia al jugador principal (debe ser asignada al iniciar el juego)
    public static JugadorLogica jugadorPrincipal;

    public static void ProcesarMuerte(Personaje personaje)
    {
        if (personaje is JugadorLogica)
        {
            Debug.Log("Jugador ha muerto. Respawn o Game Over.");
        }
        else if (personaje.GetType().FullName == "BossLogica")
        {
            Debug.Log("Boss derrotado. Drop especial y efectos.");
            if (jugadorPrincipal != null)
                jugadorPrincipal.Experiencia.GanarExperiencia(experienciaPorBoss);
        }
        else // Enemigos normales
        {
            Debug.Log("Enemigo derrotado. Solo otorga experiencia al jugador.");
            contadorMinions++;
            if (contadorMinions >= minionsPorGrupo)
            {
                if (jugadorPrincipal != null)
                    jugadorPrincipal.Experiencia.GanarExperiencia(experienciaPorGrupoMinions);
                contadorMinions = 0;
            }
        }
    }
}
