using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FlujoCajaWpf.Models
{
    [Table("fotos_casa")]
    public class FotoCasaSupabase : BaseModel
    {
        [PrimaryKey("id", true)] // true = incluir en INSERT porque lo asignamos manualmente
        [Column("id")]
        public int Id { get; set; }

        [Column("casaid")]
        public int CasaId { get; set; }

        [Column("url")]
        public string Url { get; set; } = string.Empty;

        [Column("nombre_archivo")]
        public string NombreArchivo { get; set; } = string.Empty;

        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; }
    }
}
