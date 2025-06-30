namespace FlujoDeCajaApp.Modelos
{
    /// <summary>
    /// Modelo que representa un movimiento/historial en el sistema
    /// </summary>
    public class Movimiento
    {
        public int Id { get; set; }
        public int IdPropiedad { get; set; }
        public string NombrePropiedad { get; set; } = string.Empty;
        public string TipoMovimiento { get; set; } = string.Empty; // Ingreso, Gasto, etc.
        public decimal Monto { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string UsuarioCreador { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Movimiento() { }

        /// <summary>
        /// Constructor con parámetros principales
        /// </summary>
        public Movimiento(int idPropiedad, string tipoMovimiento, decimal monto, string descripcion)
        {
            IdPropiedad = idPropiedad;
            TipoMovimiento = tipoMovimiento;
            Monto = monto;
            Descripcion = descripcion;
        }
    }
}
