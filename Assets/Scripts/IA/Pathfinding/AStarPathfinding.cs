// AStarPathfinding.cs
using System.Collections.Generic;
using UnityEngine;

// Clase pura para pathfinding A*
public class AStarPathfinding
{
    // Nodo interno para el algoritmo A*
    public class Nodo
    {
        public Vector3 posicion;
        public bool esObstaculo;
        public float costoG;
        public float costoH;
        public float costoF => costoG + costoH;
        public Nodo padre;
        public List<Nodo> vecinos;

        public Nodo(Vector3 posicion, bool esObstaculo = false)
        {
            this.posicion = posicion;
            this.esObstaculo = esObstaculo;
            vecinos = new List<Nodo>();
        }
    }

    // Encuentra el camino óptimo entre dos nodos usando A*
    public static List<Vector3> EncontrarCamino(Nodo inicio, Nodo objetivo)
    {
        var abiertos = new List<Nodo> { inicio };
        var cerrados = new HashSet<Nodo>();
        inicio.costoG = 0;
        inicio.costoH = Vector3.Distance(inicio.posicion, objetivo.posicion);
        inicio.padre = null;

        while (abiertos.Count > 0)
        {
            // Selecciona el nodo con menor F
            abiertos.Sort((a, b) => a.costoF.CompareTo(b.costoF));
            Nodo actual = abiertos[0];
            if (actual == objetivo)
            {
                // Reconstruye el camino
                var camino = new List<Vector3>();
                while (actual != null)
                {
                    camino.Add(actual.posicion);
                    actual = actual.padre;
                }
                camino.Reverse();
                return camino;
            }
            abiertos.Remove(actual);
            cerrados.Add(actual);
            foreach (var vecino in actual.vecinos)
            {
                if (vecino.esObstaculo || cerrados.Contains(vecino)) continue;
                float nuevoCostoG = actual.costoG + Vector3.Distance(actual.posicion, vecino.posicion);
                if (!abiertos.Contains(vecino) || nuevoCostoG < vecino.costoG)
                {
                    vecino.costoG = nuevoCostoG;
                    vecino.costoH = Vector3.Distance(vecino.posicion, objetivo.posicion);
                    vecino.padre = actual;
                    if (!abiertos.Contains(vecino)) abiertos.Add(vecino);
                }
            }
        }
        // No se encontró camino
        return null;
    }
}
