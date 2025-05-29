// JugadorLogica.cs (VERSION CORREGIDA)
using UnityEngine; // Necesario para Vector3, RaycastHit, etc.
using System; // Para eventos Action
using System.Collections.Generic;
using Jugabilidad;
using Interfaces;
using Nucleo;

namespace Personajes.Jugador
{

    // Esta es la clase de lógica pura del jugador, no es un MonoBehaviour.
    public class JugadorLogica : Personaje, IPerkeable, IInventariable
    {
        private EstadisticasBase estadisticas;
        // Evento que el MonoBehaviour 'JugadorControlador' escuchará para aplicar el movimiento.
        public event Action<Vector3> OnMover;
        public event Action<Vector3, float> OnDisparar; // Ahora envía la dirección y la fuerza del disparo

        private float fuerzaSalto;
        private float gravedad;
        private float tiempoEntreDisparos;
        private float proximoDisparoTiempo;
        private float fuerzaDisparo; // <--- NUEVA VARIABLE AGREGADA AQUÍ

        private readonly SistemaPerks sistemaPerks = new SistemaPerks();
        private readonly Inventario inventario = new Inventario();
        // Elimina campos duplicados de experiencia y monedas, usa los de EstadisticasBase
        public ExperienciaJugador Experiencia => estadisticas.Experiencia; // Propiedad pública para acceder a la experiencia del jugador
        /// <summary>
        /// Monedas actuales del jugador. Solo para compras con el mercader.
        /// </summary>
        public int Monedas => estadisticas.Monedas;
        /// <summary>
        /// Agrega monedas al jugador. Solo debe usarse desde la lógica interna.
        /// </summary>
        public void AgregarMonedas(int cantidad) => estadisticas.AgregarMonedas(cantidad);
        /// <summary>
        /// Intenta gastar monedas. Solo debe usarse desde la lógica interna.
        /// </summary>
        public bool GastarMonedas(int cantidad) => estadisticas.GastarMonedas(cantidad);
        public Vector3 VelocidadActual { get; private set; }
        public bool EstaEnSuelo { get; private set; }

        // --- Soporte para equipamiento por tipo ---
        private Item armaEquipada;
        private Item chalecoEquipado;
        private Item cascoEquipado;
        private Item botasEquipadas;

        public Item ArmaEquipada => armaEquipada;
        public Item ChalecoEquipado => chalecoEquipado;
        public Item CascoEquipado => cascoEquipado;
        public Item BotasEquipadas => botasEquipadas;

        // Equipa un ítem del inventario en su ranura correspondiente
        public bool EquiparItem(Item item)
        {
            if (item == null || !System.Linq.Enumerable.Any(inventario.Items, i => i == item))
                return false;
            switch (item.Tipo)
            {
                case ItemTipo.Arma:
                    if (armaEquipada != null)
                        inventario.AgregarItem(armaEquipada);
                    armaEquipada = item;
                    break;
                case ItemTipo.Chaleco:
                    if (chalecoEquipado != null)
                        inventario.AgregarItem(chalecoEquipado);
                    chalecoEquipado = item;
                    break;
                case ItemTipo.Casco:
                    if (cascoEquipado != null)
                        inventario.AgregarItem(cascoEquipado);
                    cascoEquipado = item;
                    break;
                case ItemTipo.Botas:
                    if (botasEquipadas != null)
                        inventario.AgregarItem(botasEquipadas);
                    botasEquipadas = item;
                    break;
            }
            inventario.RemoverItem(item);
            return true;
        }

        // Desequipa un ítem de su ranura y lo devuelve al inventario
        public void DesequiparItem(ItemTipo tipo)
        {
            switch (tipo)
            {
                case ItemTipo.Arma:
                    if (armaEquipada != null)
                    {
                        inventario.AgregarItem(armaEquipada);
                        armaEquipada = null;
                    }
                    break;
                case ItemTipo.Chaleco:
                    if (chalecoEquipado != null)
                    {
                        inventario.AgregarItem(chalecoEquipado);
                        chalecoEquipado = null;
                    }
                    break;
                case ItemTipo.Casco:
                    if (cascoEquipado != null)
                    {
                        inventario.AgregarItem(cascoEquipado);
                        cascoEquipado = null;
                    }
                    break;
                case ItemTipo.Botas:
                    if (botasEquipadas != null)
                    {
                        inventario.AgregarItem(botasEquipadas);
                        botasEquipadas = null;
                    }
                    break;
            }
        }

        // Modificadores solo de los ítems equipados
        public float ModificadorAtaqueEquipada => armaEquipada != null ? armaEquipada.ModificadorAtaque : 0f;
        public float ModificadorSaludEquipada => chalecoEquipado != null ? chalecoEquipado.ModificadorSalud : 0f;
        public float ModificadorDefensaEquipada => cascoEquipado != null ? cascoEquipado.ModificadorDefensa : 0f;
        public float ModificadorVelocidadEquipada => botasEquipadas != null ? botasEquipadas.ModificadorVelocidad : 0f;

        // Propiedades calculadas con modificadores de ítems equipados
        public int AtaqueTotal => Ataque + Mathf.RoundToInt(Ataque * ModificadorAtaqueEquipada);
        public int DefensaTotal => Defensa + Mathf.RoundToInt(Defensa * ModificadorDefensaEquipada);
        public float VelocidadMovimientoTotal => VelocidadMovimiento * (1f + ModificadorVelocidadEquipada);
        public int SaludMaximaTotal => Salud.SaludMaxima + Mathf.RoundToInt(Salud.SaludMaxima * ModificadorSaludEquipada);

        public float ModificadorAtaque => CalcularModificador(item => item.ModificadorAtaque);
        public float ModificadorSalud => CalcularModificador(item => item.ModificadorSalud);
        public float ModificadorVelocidad => CalcularModificador(item => item.ModificadorVelocidad);
        public float ModificadorDefensa => CalcularModificador(item => item.ModificadorDefensa);

        // Constructor actualizado para aceptar ataque, defensa y velocidad de movimiento base
        public JugadorLogica(int saludMaxima, float velocidadBase, float fuerzaSalto, float gravedad, float tiempoEntreDisparos, float fuerzaDisparo, int monedasIniciales = 0, int ataque = 10, int defensa = 0, float velocidadMovimiento = 1f)
            : base(saludMaxima, velocidadBase, ataque, defensa, velocidadMovimiento)
        {
            estadisticas = new EstadisticasBase(saludMaxima, monedasIniciales);
            this.fuerzaSalto = fuerzaSalto;
            this.gravedad = gravedad;
            this.tiempoEntreDisparos = tiempoEntreDisparos;
            this.fuerzaDisparo = fuerzaDisparo;
            this.VelocidadActual = Vector3.zero;
            this.EstaEnSuelo = false;
            this.proximoDisparoTiempo = 0f; // Permite disparar inmediatamente al inicio
        }

        // Método para procesar la entrada y actualizar la lógica de movimiento
        // Este método será llamado desde el Update del MonoBehaviour 'JugadorControlador'.
        public void ProcesarMovimiento(float inputHorizontal, bool saltoPresionado, bool enSueloDetectado)
        {
            if (!Salud.EstaVivo) return;

            // 1. Movimiento Horizontal
            Vector3 direccionHorizontal = new Vector3(inputHorizontal, 0, 0);
            Vector3 movimientoHorizontal = direccionHorizontal * VelocidadBase;

            // 2. Detección de Suelo
            EstaEnSuelo = enSueloDetectado;

            // 3. Aplicación de físicas personalizadas
            if (!EstaEnSuelo)
            {
                // Gravedad
                VelocidadActual = FisicasProyecto.AplicarGravedad(VelocidadActual, Time.deltaTime, gravedad);
                // Arrastre en el aire (opcional, puedes ajustar el coeficiente)
                VelocidadActual = FisicasProyecto.AplicarArrastre(VelocidadActual, 0.01f, Time.deltaTime);
            }
            else
            {
                // Fricción en el suelo
                VelocidadActual = FisicasProyecto.AplicarFriccion(VelocidadActual, 0.5f, 1f, Time.deltaTime);
                VelocidadActual = new Vector3(VelocidadActual.x, -0.1f, VelocidadActual.z);
            }

            // 4. Salto Manual
            if (saltoPresionado && EstaEnSuelo)
            {
                VelocidadActual = FisicasProyecto.AplicarImpulso(VelocidadActual, new Vector3(0, fuerzaSalto, 0));
                EstaEnSuelo = false;
            }

            Vector3 movimientoTotal = movimientoHorizontal * Time.deltaTime + VelocidadActual * Time.deltaTime;
            OnMover?.Invoke(movimientoTotal);
        }

        // Método para procesar el disparo
        // Este método será llamado desde el Update del MonoBehaviour 'JugadorControlador'.
        public void ProcesarDisparo(bool disparoPresionado, Vector3 direccionDisparo)
        {
            if (!Salud.EstaVivo) return;

            if (disparoPresionado && Time.time >= proximoDisparoTiempo)
            {
                // Notificamos al controlador que debe instanciar un proyectil,
                // pasándole la dirección y LA FUERZA DEL DISPARO QUE AHORA ES UNA VARIABLE INTERNA.
                OnDisparar?.Invoke(direccionDisparo, fuerzaDisparo); // <--- CAMBIADO AQUÍ
                proximoDisparoTiempo = Time.time + tiempoEntreDisparos;
            }
        }

        // Implementación del método abstracto de ataque
        public override void Atacar(Vector3 direccionObjetivo)
        {
            if (!Salud.EstaVivo) return;
            if (PuedeAtacar())
            {
                // El jugador ataca disparando un proyectil en la dirección indicada
                OnDisparar?.Invoke(direccionObjetivo, fuerzaDisparo);
                tiempoUltimoAtaque = Time.time;
            }
        }

        // Implementación del comportamiento al morir para el Jugador Lógica.
        protected override void OnMuertePersonaje()
        {
#if UNITY_EDITOR
            Debug.Log("Lógica del Jugador: El personaje ha muerto.");
#endif
            // Aquí la lógica pura puede decidir qué hacer al morir (ej. resetear estadísticas, notificar un Game Over a un gestor de juego)
            // La desactivación visual del GameObject será responsabilidad del JugadorControlador.
        }

        // --- Implementación de IPerkeable ---
        public void AgregarPerk(Jugabilidad.Perk perk)
        {
            ((Interfaces.IPerkeable)sistemaPerks).AgregarPerk(perk);
        }
        public void RemoverPerk(Jugabilidad.Perk perk) => sistemaPerks.RemoverPerk(perk);
        public bool TienePerk(string idPerk) => sistemaPerks.TienePerk(idPerk);
        // Permite reemplazar un perk existente por uno nuevo (para la lógica de compra de perks)
        public bool ReemplazarPerk(Perk perkNuevo, Perk perkAEliminar)
        {
            return sistemaPerks.ReemplazarPerk(perkNuevo, perkAEliminar);
        }

        // Intenta equipar un perk, retorna true si se equipa directamente, false si requiere reemplazo
        public bool TryEquiparPerk(Perk perk)
        {
            return sistemaPerks.TryAgregarPerk(perk);
        }

        // Exponer perks equipados para la UI
        public IEnumerable<Perk> PerksEquipados => sistemaPerks.Perks;

        // --- Implementación de IInventariable ---
        public void AgregarItem(Jugabilidad.Item item) => inventario.AgregarItem(item);
        public void RemoverItem(Jugabilidad.Item item) => inventario.RemoverItem(item);
        public bool TieneItem(string idItem) => inventario.TieneItem(idItem);

        // Propiedades públicas para obtener perks e items (más simple y estándar)
        public IEnumerable<Perk> Perks => sistemaPerks.Perks;
        public IEnumerable<Item> Items => inventario.Items;
        // Puedes llamar a experiencia.GanarExperiencia(cantidad) cuando el jugador derrote enemigos o recoja objetos de experiencia.

        // Centraliza la compra de productos del mercader (perk o item)
        public bool ComprarProducto(MercaderProducto producto)
        {
            if (producto.Precio > Monedas)
                return false;
            if (producto is Perk perk)
            {
                if (!TienePerk(perk.Id))
                {
                    AgregarPerk(perk);
                    estadisticas.GastarMonedas(perk.Precio);
                    return true;
                }
            }
            else if (producto is Item item)
            {
                if (!TieneItem(item.Id))
                {
                    AgregarItem(item);
                    estadisticas.GastarMonedas(item.Precio);
                    return true;
                }
            }
            return false;
        }

        // Método para otorgar monedas al subir de nivel
        public void OtorgarMonedasPorNivel(int nivelAnterior, int nivelActual)
        {
            int monedasGanadas = 0;
            for (int nivel = nivelAnterior + 1; nivel <= nivelActual && nivel <= 15; nivel++)
            {
                if (nivel <= 5)
                    monedasGanadas += 10;
                else if (nivel <= 10)
                    monedasGanadas += 15;
                else if (nivel <= 15)
                    monedasGanadas += 30;
            }
            if (monedasGanadas > 0)
                estadisticas.AgregarMonedas(monedasGanadas);
        }

        private float CalcularModificador(Func<Item, float> selector)
        {
            float total = 0f;
            foreach (var item in inventario.Items)
                total += selector(item);
            return total;
        }

        // POLÍTICA DE ENCAPSULAMIENTO:
        // Todos los campos internos deben ser privados y solo exponer propiedades/métodos necesarios.
        // Si se agregan nuevas estadísticas o sistemas, seguir este patrón para mantener la seguridad y escalabilidad.
    }
}