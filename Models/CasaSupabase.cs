using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FlujoCajaWpf.Models
{
    /// <summary>
    /// Modelo para tabla 'casas' en Supabase
    /// </summary>
    [Table("casas")]
    public class CasaSupabase : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("duenoid")]
        public int DuenoId { get; set; }

        [Column("categoriaid")]
        public int CategoriaId { get; set; }

        [Column("rutaimagen")]
        public string? RutaImagen { get; set; }

        [Column("moneda")]
        public string Moneda { get; set; } = "USD";

        [Column("notas")]
        public string? Notas { get; set; }

        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; }
    }
}
