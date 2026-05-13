using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace FlujoCajaWpf.Models
{
    /// <summary>
    /// Modelo de UI para movimientos parseados por IA en la tabla temporal.
    /// </summary>
    public class MovimientoIA : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int CasaId { get; set; }
        public int? HojaMensualId { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public DateTime? Fecha { get; set; }
        public decimal? Monto { get; set; }
        public string? Descripcion { get; set; }
        public string? Categoria { get; set; }
        public string? TipoMovimiento { get; set; }   // "Ingreso" o "Gasto"
        public string? FacturaUrl { get; set; }        // path privado en bucket "facturas"
        public string Estado { get; set; } = "pendiente";
        public string? RawJson { get; set; }
        public string? UsuarioCreador { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Selección en UI (checkbox por fila)
        private bool _seleccionado;
        public bool Seleccionado
        {
            get => _seleccionado;
            set { _seleccionado = value; OnPropertyChanged(); }
        }

        // Propiedades de presentación
        public string FechaTexto => Fecha?.ToString("dd/MM/yyyy") ?? "—";
        public string MontoTexto => Monto.HasValue ? Monto.Value.ToString("C", new CultureInfo("es-CR")) : "—";

        public string IconoTipo => TipoMovimiento == "Ingreso" ? "💰" : "💸";

        public string EstadoBadge => Estado switch
        {
            "pendiente" => "⏳ Pendiente",
            "parcial"   => "⚠️ Incompleto",
            "aprobado"  => "✅ Aprobado",
            "error"     => "❌ Error",
            _           => Estado
        };

        public bool TieneImagen => !string.IsNullOrWhiteSpace(FacturaUrl);

        // Fila marcada como problemática (parcial o error) → sombra roja en UI
        public bool EsProblematico => Estado == "parcial" || Estado == "error";

        public static MovimientoIA FromSupabase(MovimientoIASupabase s) => new()
        {
            Id              = s.Id,
            CasaId          = s.CasaId,
            HojaMensualId   = s.HojaMensualId,
            Mes             = s.Mes,
            Anio            = s.Anio,
            Fecha           = s.Fecha,
            Monto           = s.Monto,
            Descripcion     = s.Descripcion,
            Categoria       = s.Categoria,
            TipoMovimiento  = s.TipoMovimiento,
            FacturaUrl      = s.FacturaUrl,
            Estado          = s.Estado,
            RawJson         = s.RawJson,
            UsuarioCreador  = s.UsuarioCreador,
            FechaCreacion   = s.FechaCreacion
        };

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
