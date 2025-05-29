using UnityEngine;

// Zona de seguridad para el mercader: los enemigos no pueden entrar ni atacar al jugador dentro de esta zona.
[RequireComponent(typeof(Collider))]
public class ZonaSeguridadMercader : MonoBehaviour
{
    public static ZonaSeguridadMercader Instancia { get; private set; }
    private Collider zonaCollider;

    private void Awake()
    {
        Instancia = this;
        zonaCollider = GetComponent<Collider>();
        zonaCollider.isTrigger = true;
    }

    // Comprueba si una posición está dentro de la zona de seguridad
    public bool EstaDentroZona(Vector3 posicion)
    {
        return zonaCollider.bounds.Contains(posicion);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (GetComponent<Collider>() != null)
        {
            Gizmos.color = new Color(0, 1, 1, 0.2f);
            Gizmos.DrawCube(GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.size);
        }
    }
#endif
}
