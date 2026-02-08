using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FlujoCajaWpf.Models
{
    [Table("movimientos")]
    public class MovimientoSupabase : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("casaid")]
        public int CasaId { get; set; }

        [Column("hoja_mensual_id")]
        public int? HojaMensualId { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [Column("monto")]
        public decimal Monto { get; set; }

        [Column("categoria")]
        public string Categoria { get; set; } = string.Empty;

        [Column("fechacreacion")]
        public DateTime? FechaCreacion { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("tipo_movimiento")]
        public string TipoMovimiento { get; set; } = "Ingreso"; // "Ingreso" o "Gasto"
    }
}
