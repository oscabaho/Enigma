using UnityEngine;
using System.Collections.Generic;

public class BossSpawner : MonoBehaviour
{
    [Header("Prefab del Boss")]
    public GameObject bossPrefab;

    [Header("Puntos de spawn posibles para el Boss")]
    public Transform[] puntosSpawnBoss;

    [Header("Intervalo de aparición (segundos)")]
    public float intervaloBoss = 30f;

    [Header("Probabilidad de aparición (0-1)")]
    [Range(0f, 1f)]
    public float probabilidadAparicion = 0.3f;

    private float tiempoSiguienteBoss;
    private GameObject bossInstanciado;

    void Start()
    {
        tiempoSiguienteBoss = Time.time + intervaloBoss;
    }

    void Update()
    {
        if (bossInstanciado == null && Time.time >= tiempoSiguienteBoss)
        {
            tiempoSiguienteBoss = Time.time + intervaloBoss;
            if (Random.value < probabilidadAparicion)
            {
                SpawnBoss();
            }
        }
    }

    void SpawnBoss()
    {
        if (puntosSpawnBoss.Length == 0 || bossPrefab == null) return;
        int idx = Random.Range(0, puntosSpawnBoss.Length);
        bossInstanciado = Instantiate(bossPrefab, puntosSpawnBoss[idx].position, Quaternion.identity);
    }
}
