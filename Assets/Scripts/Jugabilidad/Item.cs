using Jugabilidad;

namespace Jugabilidad
{
    public enum ItemTipo { Arma, Chaleco, Casco, Botas }

    // Representa un item individual como producto del mercader
    public class Item : MercaderProducto
    {
        public ItemTipo Tipo { get; }

        public Item(string id, string nombre, string descripcion, int precio)
            : base(id, nombre, descripcion, precio) { }

        public new string Id { get { return base.Id; } }
        public new string Nombre { get { return base.Nombre; } }
        public new string Descripcion { get { return base.Descripcion; } }
        public new int Precio { get { return base.Precio; } }

        // Modificadores de estadísticas al equipar el item
        public float ModificadorAtaque { get; } = 0f;
        public float ModificadorSalud { get; } = 0f;
        public float ModificadorVelocidad { get; } = 0f;
        public float ModificadorDefensa { get; } = 0f;

        // Constructor extendido actualizado
        public Item(string id, string nombre, string descripcion, int precio, float modAtaque, float modSalud, float modVelocidad, float modDefensa, ItemTipo tipo)
            : base(id, nombre, descripcion, precio)
        {
            ModificadorAtaque = modAtaque;
            ModificadorSalud = modSalud;
            ModificadorVelocidad = modVelocidad;
            ModificadorDefensa = modDefensa;
            Tipo = tipo;
        }

        public static Item PistolaBasica { get; } = new Item(
            "pistola_basica", "Pistola Básica", "Arma sencilla, daño bajo.", 10, 0.10f, 0f, 0f, 0f, ItemTipo.Arma);
        public static Item PistolaAvanzada { get; } = new Item(
            "pistola_avanzada", "Pistola Avanzada", "Mejor cadencia y daño moderado.", 20, 0.25f, 0f, 0f, 0f, ItemTipo.Arma);
        public static Item PistolaElite { get; } = new Item(
            "pistola_elite", "Pistola Élite", "Alta precisión y daño alto.", 40, 0.75f, 0f, 0f, 0f, ItemTipo.Arma);

        public static Item ChalecoLigero { get; } = new Item(
            "chaleco_ligero", "Chaleco Ligero", "Protección básica, aumenta la salud.", 10, 0f, 0.10f, 0f, 0f, ItemTipo.Chaleco);
        public static Item ChalecoRefuerzo { get; } = new Item(
            "chaleco_refuerzo", "Chaleco Refuerzo", "Mejor protección, aumenta más la salud.", 20, 0f, 0.25f, 0f, 0f, ItemTipo.Chaleco);
        public static Item ChalecoElite { get; } = new Item(
            "chaleco_elite", "Chaleco Élite", "Máxima protección, gran aumento de salud.", 40, 0f, 0.75f, 0f, 0f, ItemTipo.Chaleco);

        public static Item BotasBasicas { get; } = new Item(
            "botas_basicas", "Botas Básicas", "Botas simples, aumentan ligeramente la velocidad.", 10, 0f, 0f, 0.10f, 0f, ItemTipo.Botas);
        public static Item BotasRefuerzo { get; } = new Item(
            "botas_refuerzo", "Botas Refuerzo", "Botas reforzadas, mejoran la velocidad y el salto.", 20, 0f, 0f, 0.25f, 0f, ItemTipo.Botas);
        public static Item BotasElite { get; } = new Item(
            "botas_elite", "Botas Élite", "Botas de élite, máxima velocidad y salto.", 40, 0f, 0f, 0.75f, 0f, ItemTipo.Botas);

        public static Item CascoBasico { get; } = new Item(
            "casco_basico", "Casco Básico", "Casco sencillo, aumenta la defensa.", 10, 0f, 0f, 0f, 0.10f, ItemTipo.Casco);
        public static Item CascoRefuerzo { get; } = new Item(
            "casco_refuerzo", "Casco Refuerzo", "Casco reforzado, buena protección.", 20, 0f, 0f, 0f, 0.25f, ItemTipo.Casco);
        public static Item CascoElite { get; } = new Item(
            "casco_elite", "Casco Élite", "Casco de élite, máxima protección.", 40, 0f, 0f, 0f, 0.75f, ItemTipo.Casco);
    }
}
