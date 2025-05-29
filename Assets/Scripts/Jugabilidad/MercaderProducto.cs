namespace Jugabilidad
{
    // Clase base para productos del mercader (perk o item)
    public abstract class MercaderProducto
    {
        public string Id { get; }
        public string Nombre { get; }
        public string Descripcion { get; }
        public int Precio { get; }

        protected MercaderProducto(string id, string nombre, string descripcion, int precio)
        {
            Id = id;
            Nombre = nombre;
            Descripcion = descripcion;
            Precio = precio;
        }
    }
}
