namespace Jugabilidad
{
    // Representa un perk individual como producto del mercader
    public class Perk : MercaderProducto
    {
        public Perk(string id, string nombre, string descripcion, int precio)
            : base(id, nombre, descripcion, precio) { }

        public new string Id { get { return base.Id; } }
        public new string Nombre { get { return base.Nombre; } }
        public new string Descripcion { get { return base.Descripcion; } }
        public new int Precio { get { return base.Precio; } }
    }
}
