using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FlujoCajaWpf.Models
{
    [Table("hojas_mensuales")]
    public class HojaMensualSupabase : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("casaid")]
        public int CasaId { get; set; }

        [Column("mes")]
        public int Mes { get; set; }

        [Column("anio")]
        public int Anio { get; set; }

        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("cerrada")]
        public bool Cerrada { get; set; }
    }
}
