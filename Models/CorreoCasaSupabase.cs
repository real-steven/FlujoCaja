using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FlujoCajaWpf.Models
{
    [Table("correos_casa")]
    public class CorreoCasaSupabase : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("casa_id")]
        public long CasaId { get; set; }

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("etiqueta")]
        public string? Etiqueta { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }
    }
}
