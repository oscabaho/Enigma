using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemigoConfig", menuName = "Config/Enemigo")]
public class EnemigoConfig : ScriptableObject
{
    public string id;
    public string nombre;
    public int saludMaxima;
    public int ataque;
    public int defensa;
    public float velocidadMovimiento;
    public GameObject prefab;
    public Sprite icono;
}
