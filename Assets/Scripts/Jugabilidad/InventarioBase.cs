using System.Collections.Generic;
using Jugabilidad;

namespace Jugabilidad
{
    /// <summary>
    /// Clase base abstracta para cualquier inventario (jugador, NPC, cofre, tienda, etc.)
    /// </summary>
    public abstract class InventarioBase
    {
        protected readonly List<Item> items = new List<Item>();
        public virtual IEnumerable<Item> Items => items.AsReadOnly();

        public event System.Action<Item> OnItemAdded;
        public event System.Action<Item> OnItemRemoved;

        public virtual void AgregarItem(Item item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
                OnItemAdded?.Invoke(item);
            }
        }

        public virtual void RemoverItem(Item item)
        {
            if (items.Remove(item))
            {
                OnItemRemoved?.Invoke(item);
            }
        }

        public virtual bool TieneItem(string idItem)
        {
            return items.Exists(i => i.Id == idItem);
        }
    }
}
