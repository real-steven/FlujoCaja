using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FlujoCajaWpf.Models
{
    [Table("categorias_movimientos")]
    public class CategoriaMovimientoSupabase : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("tipo")]
        public string Tipo { get; set; } = "ingreso";

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; }
    }
}
