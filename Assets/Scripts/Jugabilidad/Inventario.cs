using System.Collections.Generic;
using Interfaces;

namespace Jugabilidad
{
    // Inventario estándar para personajes, hereda de InventarioBase
    public class Inventario : InventarioBase, IInventariable
    {
        void IInventariable.AgregarItem(Item item) => base.AgregarItem(item);
        void IInventariable.RemoverItem(Item item) => base.RemoverItem(item);
        bool IInventariable.TieneItem(string idItem) => base.TieneItem(idItem);
        // Puedes agregar lógica específica del inventario de personajes aquí
    }
}
