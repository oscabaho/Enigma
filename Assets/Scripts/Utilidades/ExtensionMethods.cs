// ExtensionMethods.cs
using UnityEngine;

public static class ExtensionMethods
{
    // Ejemplo: Compara dos Vector3 con tolerancia
    public static bool Aproximadamente(this Vector3 a, Vector3 b, float tolerancia = 0.01f)
    {
        return Vector3.Distance(a, b) < tolerancia;
    }
}
