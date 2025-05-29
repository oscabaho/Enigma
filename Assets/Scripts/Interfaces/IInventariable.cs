using Jugabilidad;

namespace Interfaces
{
    // Interfaz para personajes que pueden tener inventario
    public interface IInventariable
    {
        void AgregarItem(Item item);
        void RemoverItem(Item item);
        bool TieneItem(string idItem);
    }
}
