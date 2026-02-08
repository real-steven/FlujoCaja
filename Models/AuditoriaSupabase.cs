using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FlujoCajaWpf.Models
{
    [Table("auditoria")]
    public class AuditoriaSupabase : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("usuario_email")]
        public string UsuarioEmail { get; set; } = string.Empty;

        [Column("modulo")]
        public string Modulo { get; set; } = string.Empty; // "casa", "movimiento", "dueno", "categoria"

        [Column("tipo_accion")]
        public string TipoAccion { get; set; } = string.Empty; // "crear", "editar", "eliminar", "activar", "desactivar"

        [Column("entidad_id")]
        public int? EntidadId { get; set; }

        [Column("entidad_nombre")]
        public string? EntidadNombre { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [Column("datos_anteriores")]
        public string? DatosAnteriores { get; set; } // JSON serializado

        [Column("datos_nuevos")]
        public string? DatosNuevos { get; set; } // JSON serializado

        [Column("fecha")]
        public DateTime Fecha { get; set; }
    }
}
