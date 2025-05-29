// MinionSpawner.cs
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    [Header("Prefabs de Minions (uno por patrón)")]
    public GameObject[] minionPrefabs; // Asigna aquí los 3 prefabs de minion

    [Header("Configuración de Spawn")]
    public int cantidadMinions = 5;
    public Vector2 areaMin; // Esquina inferior izquierda del área de spawn
    public Vector2 areaMax; // Esquina superior derecha del área de spawn

    [Header("Evitar solapamientos")]
    public float distanciaMinimaEntreMinions = 2f;
    [Header("LayerMask para plataformas válidas")]
    public LayerMask plataformaLayer;
    public float alturaRaycast = 10f;
    public float alturaSpawn = 1f;

    [Header("Puntos de spawn fijos para minions")]
    public Transform[] puntosSpawnMinion;
    [Header("Probabilidad de aparición por punto (0-1)")]
    [Range(0f, 1f)]
    public float[] probabilidadesPorPunto;
    [Header("Intervalo de aparición (segundos)")]
    public float intervaloMinion = 30f;
    private float tiempoSiguienteMinion;

    private int maxMinions = 10;
    private float tiempoCambio50 = 60f; // 1 minuto
    private float tiempoCambio100 = 300f; // 5 minutos
    private float tiempoUltimaRevision;

    void Start()
    {
        maxMinions = 10;
        tiempoUltimaRevision = Time.time;
        SpawnearMinionsIniciales();
    }

    void Update()
    {
        float tiempoJuego = Time.time;
        if (tiempoJuego >= tiempoCambio100)
            maxMinions = 100;
        else if (tiempoJuego >= tiempoCambio50)
            maxMinions = 50;
        else
            maxMinions = 10;

        // Cada 5 segundos, rellena minions hasta el máximo
        if (Time.time - tiempoUltimaRevision >= 5f)
        {
            tiempoUltimaRevision = Time.time;
            RellenarMinionsHastaMaximo();
        }

        // Spawn por probabilidad en puntos fijos, según intervaloMinion
        if (Time.time >= tiempoSiguienteMinion)
        {
            tiempoSiguienteMinion = Time.time + intervaloMinion;
            SpawnearMinionsPorProbabilidad();
        }
    }

    void SpawnearMinionsIniciales()
    {
        for (int i = 0; i < maxMinions; i++)
            SpawnearMinionAleatorio();
    }

    void RellenarMinionsHastaMaximo()
    {
        int minionsActuales = GameObject.FindGameObjectsWithTag("Minion").Length;
        int faltantes = maxMinions - minionsActuales;
        for (int i = 0; i < faltantes; i++)
            SpawnearMinionAleatorio();
    }

    void SpawnearMinionAleatorio()
    {
        GameObject prefab = minionPrefabs[Random.Range(0, minionPrefabs.Length)];
        Vector3 spawnPos = Vector3.zero;
        if (puntosSpawnMinion != null && puntosSpawnMinion.Length > 0)
        {
            int idx = Random.Range(0, puntosSpawnMinion.Length);
            spawnPos = puntosSpawnMinion[idx].position;
        }
        else
        {
            float x = Random.Range(areaMin.x, areaMax.x);
            float z = Random.Range(areaMin.y, areaMax.y);
            Vector3 origenRay = new Vector3(x, alturaRaycast, z);
            RaycastHit hit;
            if (Physics.Raycast(origenRay, Vector3.down, out hit, alturaRaycast * 2, plataformaLayer))
                spawnPos = hit.point + Vector3.up * alturaSpawn;
        }
        GameObject minion = Instantiate(prefab, spawnPos, Quaternion.identity);
        minion.tag = "Minion";
    }

    void SpawnearMinionsPorProbabilidad()
    {
        if (puntosSpawnMinion == null || probabilidadesPorPunto == null) return;
        for (int i = 0; i < puntosSpawnMinion.Length && i < probabilidadesPorPunto.Length; i++)
        {
            if (Random.value < probabilidadesPorPunto[i])
            {
                GameObject prefab = minionPrefabs[Random.Range(0, minionPrefabs.Length)];
                GameObject minion = Instantiate(prefab, puntosSpawnMinion[i].position, Quaternion.identity);
                minion.tag = "Minion";
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        // Dibuja un rectángulo en el área de spawn (asume XZ plano)
        Vector3 centro = new Vector3((areaMin.x + areaMax.x) * 0.5f, 0, (areaMin.y + areaMax.y) * 0.5f);
        Vector3 size = new Vector3(Mathf.Abs(areaMax.x - areaMin.x), 0.1f, Mathf.Abs(areaMax.y - areaMin.y));
        Gizmos.DrawWireCube(centro, size);
    }
#endif
}
