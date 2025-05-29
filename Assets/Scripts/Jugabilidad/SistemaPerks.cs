using System.Collections.Generic;
using Interfaces;

namespace Jugabilidad
{
    // Sistema de gestión de perks para un personaje
    public class SistemaPerks : IPerkeable
    {
        private readonly List<Perk> perks = new List<Perk>();
        public IEnumerable<Perk> Perks => perks.AsReadOnly();
        public const int MaxPerks = 3;

        // Eventos para notificar cambios
        public event System.Action<Perk> OnPerkAdded;
        public event System.Action<Perk> OnPerkRemoved;

        /// <summary>
        /// Intenta agregar un perk. Si ya hay 3, retorna false (requiere reemplazo).
        /// Solo permite un perk por tipo (PerkConTipo). Si ya existe uno del mismo tipo:
        /// - Si el nuevo perk es superior (mayor modificador), lo reemplaza automáticamente.
        /// - Si es igual o inferior, rechaza el cambio.
        /// </summary>
        public bool TryAgregarPerk(Perk perk)
        {
            if (TienePerk(perk.Id))
                return false; // Ya lo tiene

            // Lógica para perks con tipo
            if (perk is PerkConTipo nuevoPerkConTipo)
            {
                // Buscar si ya hay un perk de ese tipo
                var perkExistente = perks.Find(p => p is PerkConTipo pct && pct.Tipo == nuevoPerkConTipo.Tipo) as PerkConTipo;
                if (perkExistente != null)
                {
                    // Si el nuevo perk es superior, reemplazar automáticamente
                    if (nuevoPerkConTipo.Modificador > perkExistente.Modificador)
                    {
                        perks.Remove(perkExistente);
                        OnPerkRemoved?.Invoke(perkExistente);
                        perks.Add(nuevoPerkConTipo);
                        OnPerkAdded?.Invoke(nuevoPerkConTipo);
                        return true;
                    }
                    // Si es igual o inferior, rechazar
                    return false;
                }
            }
            // Si hay espacio y no hay perk de ese tipo, agregar
            if (perks.Count < MaxPerks)
            {
                perks.Add(perk);
                OnPerkAdded?.Invoke(perk);
                return true;
            }
            // Si ya hay 3, requiere reemplazo manual
            return false;
        }

        // Implementación estándar de la interfaz (agrega solo si no existe y hay espacio)
        void IPerkeable.AgregarPerk(Perk perk)
        {
            if (!TienePerk(perk.Id) && perks.Count < MaxPerks)
            {
                perks.Add(perk);
                OnPerkAdded?.Invoke(perk);
            }
        }

        /// <summary>
        /// Reemplaza un perk existente por uno nuevo, solo si el nuevo es superior (PerkConTipo).
        /// </summary>
        public bool ReemplazarPerk(Perk perkNuevo, Perk perkAEliminar)
        {
            if (!perks.Contains(perkAEliminar) || TienePerk(perkNuevo.Id))
                return false;
            // Si ambos son PerkConTipo, validar que el nuevo sea superior
            if (perkNuevo is PerkConTipo nuevo && perkAEliminar is PerkConTipo viejo)
            {
                if (nuevo.Tipo != viejo.Tipo || nuevo.Modificador <= viejo.Modificador)
                    return false; // Solo reemplaza si es del mismo tipo y superior
            }
            perks.Remove(perkAEliminar);
            OnPerkRemoved?.Invoke(perkAEliminar);
            perks.Add(perkNuevo);
            OnPerkAdded?.Invoke(perkNuevo);
            return true;
        }

        public void RemoverPerk(Perk perk)
        {
            if (perks.Remove(perk))
            {
                OnPerkRemoved?.Invoke(perk);
            }
        }

        public bool TienePerk(string idPerk)
        {
            return perks.Exists(p => p.Id == idPerk);
        }
    }
}
