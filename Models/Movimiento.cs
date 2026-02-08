using System.Globalization;

namespace FlujoCajaWpf.Models
{
    public class Movimiento
    {
        public int Id { get; set; }
        public int CasaId { get; set; }
        public int? HojaMensualId { get; set; }
        public string? CasaNombre { get; set; }
        public string? MonedaCasa { get; set; } = "USD"; // Moneda de la casa
        public string CategoriaNombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string TipoTexto => Tipo == "Ingreso" ? "💰 Ingreso" : "💸 Gasto";
        public decimal Monto { get; set; }
        public decimal MontoAbsoluto => Math.Abs(Monto);
        public string MontoFormateado
        {
            get
            {
                var culture = MonedaCasa switch
                {
                    "USD" => new CultureInfo("en-US"),
                    "CRC" => new CultureInfo("es-CR"),
                    _ => new CultureInfo("en-US")
                };
                return Monto.ToString("C", culture);
            }
        }
        public DateTime Fecha { get; set; }
        public string FechaTexto => Fecha.ToString("dd/MM/yyyy");
        public string Descripcion { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
