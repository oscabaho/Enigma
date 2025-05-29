using UnityEngine;
using System;
using Nucleo;

public class BossLogica : Personaje
{
    public event Action<Vector3> OnDisparoBoss;

    private float cooldownDisparo = 3f;
    private float tiempoUltimoDisparo = -999f;
    public float rangoDisparo = 15f;
    public GameObject proyectilPrefab;
    public Transform puntoDisparo;

    public BossLogica(int saludMaxima, float velocidadBase, int ataque = 30, int defensa = 10, float velocidadMovimiento = 0.8f)
        : base(saludMaxima, velocidadBase, ataque, defensa, velocidadMovimiento) { }

    public override void Atacar(Vector3 objetivo)
    {
        if (!Salud.EstaVivo) return;
        if (PuedeAtacar())
        {
            // Ataque cuerpo a cuerpo (puedes expandirlo)
            tiempoUltimoAtaque = Time.time;
        }
    }

    public void AtacarADistancia(Vector3 objetivo)
    {
        if (!Salud.EstaVivo) return;
        if (Time.time - tiempoUltimoDisparo >= cooldownDisparo)
        {
            OnDisparoBoss?.Invoke(objetivo);
            tiempoUltimoDisparo = Time.time;
        }
    }

    protected override void OnMuertePersonaje()
    {
        GestorMuertePersonaje.ProcesarMuerte(this);
    }
}
