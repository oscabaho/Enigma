namespace Jugabilidad
{
    /// <summary>
    /// Enum para los tipos de perks disponibles.
    /// </summary>
    public enum PerkTipo
    {
        Ataque,
        Defensa,
        Velocidad,
        Salud
    }

    /// <summary>
    /// Perk extendido con tipo y modificador asociado.
    /// </summary>
    public class PerkConTipo : Perk
    {
        public PerkTipo Tipo { get; }
        public float Modificador { get; }

        public PerkConTipo(string id, string nombre, string descripcion, int precio, PerkTipo tipo, float modificador)
            : base(id, nombre, descripcion, precio)
        {
            Tipo = tipo;
            Modificador = modificador;
        }
    }
}
