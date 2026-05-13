using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FlujoCajaWpf.Models
{
    [Table("movimientos_ia_temporales")]
    public class MovimientoIASupabase : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("casaid")]
        public int CasaId { get; set; }

        [Column("hoja_mensual_id")]
        public int? HojaMensualId { get; set; }

        [Column("mes")]
        public int Mes { get; set; }

        [Column("anio")]
        public int Anio { get; set; }

        [Column("fecha")]
        public DateTime? Fecha { get; set; }

        [Column("monto")]
        public decimal? Monto { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("categoria")]
        public string? Categoria { get; set; }

        [Column("tipo_movimiento")]
        public string? TipoMovimiento { get; set; }

        [Column("factura_url")]
        public string? FacturaUrl { get; set; }

        [Column("estado")]
        public string Estado { get; set; } = "pendiente";

        [Column("raw_json")]
        public string? RawJson { get; set; }

        [Column("usuario_creador")]
        public string? UsuarioCreador { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("usuario_aprobo")]
        public string? UsuarioAprobo { get; set; }

        [Column("fecha_aprobacion")]
        public DateTime? FechaAprobacion { get; set; }

        [Column("movimiento_id_creado")]
        public int? MovimientoIdCreado { get; set; }
    }
}
