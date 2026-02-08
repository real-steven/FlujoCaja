using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FlujoCajaWpf.Models
{
    /// <summary>
    /// Modelo para tabla 'categorias' en Supabase
    /// </summary>
    [Table("categorias")]
    public class CategoriaSupabase : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;
    }
}
