using Jugabilidad;

namespace Interfaces
{
    // Interfaz para personajes que pueden tener perks
    public interface IPerkeable
    {
        void AgregarPerk(Perk perk);
        void RemoverPerk(Perk perk);
        bool TienePerk(string idPerk);
    }
}
