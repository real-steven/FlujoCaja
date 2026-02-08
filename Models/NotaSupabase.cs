using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FlujoCajaWpf.Models
{
    [Table("notas_casa")]
    public class NotaSupabase : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("casaid")]
        public int CasaId { get; set; }

        [Column("contenido")]
        public string Contenido { get; set; } = string.Empty;

        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; }
    }
}
