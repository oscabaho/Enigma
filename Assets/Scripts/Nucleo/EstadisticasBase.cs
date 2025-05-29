namespace Nucleo
{
    /// <summary>
    /// Clase base para gestionar estadísticas comunes de entidades: salud, experiencia y monedas.
    /// </summary>
    public class EstadisticasBase
    {
        private int saludMaxima;
        private int saludActual;
        private int monedas;
        private ExperienciaJugador experiencia;

        public int SaludMaxima => saludMaxima;
        public int SaludActual => saludActual;
        /// <summary>
        /// Monedas del jugador. Solo se usan para intercambio con el mercader.
        /// No deben usarse para otros fines. Modificable solo por métodos internos.
        /// </summary>
        public int Monedas => monedas;
        public ExperienciaJugador Experiencia => experiencia;

        public EstadisticasBase(int saludMaxima, int monedasIniciales)
        {
            this.saludMaxima = saludMaxima;
            this.saludActual = saludMaxima;
            this.monedas = monedasIniciales;
            this.experiencia = new ExperienciaJugador();
        }

        public void ModificarSalud(int cantidad)
        {
            saludActual = System.Math.Clamp(saludActual + cantidad, 0, saludMaxima);
        }

        /// <summary>
        /// Agrega monedas al saldo. Solo debe llamarse desde la lógica de jugador o sistemas autorizados.
        /// </summary>
        public void AgregarMonedas(int cantidad)
        {
            monedas = System.Math.Max(0, monedas + cantidad);
        }

        /// <summary>
        /// Intenta gastar monedas. Solo debe llamarse desde la lógica de jugador o sistemas autorizados.
        /// </summary>
        public bool GastarMonedas(int cantidad)
        {
            if (monedas >= cantidad)
            {
                monedas -= cantidad;
                return true;
            }
            return false;
        }

        // POLÍTICA DE ENCAPSULAMIENTO:
        // Todos los campos internos deben ser privados y solo exponer propiedades/métodos necesarios.
        // Si se agregan nuevas estadísticas o sistemas, seguir este patrón para mantener la seguridad y escalabilidad.
    }
}
