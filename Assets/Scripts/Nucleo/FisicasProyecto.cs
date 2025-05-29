// FisicasProyecto.cs
using UnityEngine;

public static class FisicasProyecto
{
    public static float Gravedad = 9.8f;

    public static Vector3 AplicarGravedad(Vector3 velocidad, float deltaTime, float gravedadPersonalizada = -1f)
    {
        // Fórmula: v = v0 + a * t
        // En este caso, a = -gravedad (aceleración hacia abajo)
        float gravedad = gravedadPersonalizada > 0 ? gravedadPersonalizada : Gravedad;
        velocidad.y -= gravedad * deltaTime;
        return velocidad;
    }
    // Aplica fricción a la velocidad, simulando la fricción normal entre dos objetos.
    // Fórmula: Ff = μ * N (fuerza de fricción)
    // Fórmula desaceleración: v = v0 - a * t, donde a = Ff / m (aquí m=1)
    // coefFriccion: coeficiente de fricción (ejemplo típico: 0.1 a 1.0)
    // normal: magnitud de la fuerza normal (usualmente masa * gravedad)
    public static Vector3 AplicarFriccion(Vector3 velocidad, float coefFriccion, float normal, float deltaTime)
    {
        // Calcula la fuerza de fricción: Ff = mu * N
        float fuerzaFriccion = coefFriccion * normal;
        // Calcula la desaceleración debida a la fricción
        float desaceleracion = fuerzaFriccion; // Asumiendo masa = 1 para simplificar
        // Solo afecta a los ejes X y Z (horizontal)
        Vector3 velocidadHorizontal = new Vector3(velocidad.x, 0, velocidad.z);
        float magnitud = velocidadHorizontal.magnitude;
        if (magnitud > 0)
        {
            float nuevaMagnitud = Mathf.Max(0, magnitud - desaceleracion * deltaTime);
            velocidadHorizontal = velocidadHorizontal.normalized * nuevaMagnitud;
            velocidad.x = velocidadHorizontal.x;
            velocidad.z = velocidadHorizontal.z;
        }
        return velocidad;
    }
    // Aplica aceleración a la velocidad (por ejemplo, fuerzas externas, impulso, etc.)
    // Fórmula: v = v0 + a * t
    // aceleracion: vector de aceleración a aplicar
    // deltaTime: tiempo de integración
    public static Vector3 AplicarAceleracion(Vector3 velocidad, Vector3 aceleracion, float deltaTime)
    {
        return velocidad + aceleracion * deltaTime;
    }
    // Calcula el rebote (colisión elástica) entre dos objetos sólidos.
    // Fórmula: v' = v - (1 + e) * (v · n) * n
    // velocidad: velocidad incidente antes del rebote
    // normalColision: normal de la superficie de colisión (debe estar normalizada)
    // coefRestitucion: coeficiente de restitución (0 = rebote inelástico, 1 = rebote perfectamente elástico)
    public static Vector3 AplicarRebote(Vector3 velocidad, Vector3 normalColision, float coefRestitucion)
    {
        // Fórmula: v' = v - (1 + e) * (v · n) * n
        float velocidadNormal = Vector3.Dot(velocidad, normalColision);
        Vector3 rebote = velocidad - (1 + coefRestitucion) * velocidadNormal * normalColision;
        return rebote;
    }
    // Aplica arrastre (drag) a la velocidad, simulando resistencia del aire o agua.
    // Fórmula: v = v0 * (1 - drag * t)
    // coefDrag: coeficiente de arrastre (ejemplo típico: 0.01 a 0.5)
    public static Vector3 AplicarArrastre(Vector3 velocidad, float coefDrag, float deltaTime)
    {
        // Fórmula: v = v * (1 - drag * dt)
        return velocidad * Mathf.Clamp01(1f - coefDrag * deltaTime);
    }

    // Aplica un impulso instantáneo a la velocidad (por ejemplo, salto, explosión, golpe).
    // Fórmula: v = v0 + Δv (donde Δv es el impulso/m, aquí m=1)
    public static Vector3 AplicarImpulso(Vector3 velocidad, Vector3 impulso)
    {
        // Impulso = cambio instantáneo de velocidad
        return velocidad + impulso;
    }

    // Calcula la posición en un tiro parabólico en el tiempo t.
    // Fórmulas:
    // x(t) = x0 + v0x * t
    // y(t) = y0 + v0y * t - 0.5 * g * t^2
    // z(t) = z0 + v0z * t
    public static Vector3 CalcularTiroParabolico(Vector3 posInicial, Vector3 velInicial, float gravedad, float t)
    {
        // Fórmulas:
        // x(t) = x0 + v0x * t
        // y(t) = y0 + v0y * t - 0.5 * g * t^2
        // z(t) = z0 + v0z * t
        Vector3 pos = posInicial + velInicial * t;
        pos.y = posInicial.y + velInicial.y * t - 0.5f * gravedad * t * t;
        return pos;
    }

    // Calcula el deslizamiento (sliding) a lo largo de una superficie tras colisión.
    // Fórmula: v' = v - (v · n) * n
    public static Vector3 CalcularDeslizamiento(Vector3 velocidad, Vector3 normalColision)
    {
        // Proyecta la velocidad sobre el plano de la superficie
        // return velocidad - Vector3.Dot(velocidad, normalColision) * normalColision;
        // Implementar si se requiere
        return velocidad;
    }

    // Aplica una fuerza radial (atracción o repulsión) desde un punto.
    // Fórmula: F = k * (1 - d/radio) * dirección
    public static Vector3 AplicarFuerzaRadial(Vector3 posicion, Vector3 origen, float fuerza, float radio)
    {
        // Implementar si se requiere
        return Vector3.zero;
    }

    // Predice si un proyectil impactará un objetivo en su trayectoria parabólica.
    // (No hay una fórmula cerrada simple, requiere integración numérica o aproximación)
    public static bool PredecirImpactoParabolico(Vector3 posInicial, Vector3 velInicial, float gravedad, Collider objetivo, out Vector3 puntoImpacto)
    {
        // Implementar si se requiere
        puntoImpacto = Vector3.zero;
        return false;
    }

    // Resuelve la penetración entre dos objetos (corrige el solapamiento).
    // Fórmula: separación = dirección.normalizada * (radioA + radioB - distancia)
    public static Vector3 ResolverPenetracion(Vector3 posicionA, float radioA, Vector3 posicionB, float radioB)
    {
        // Implementar si se requiere
        return Vector3.zero;
    }
}
