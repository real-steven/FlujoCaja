using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Text.Json.Serialization;

namespace FlujoCajaWpf.Models
{
    /// <summary>
    /// Modelo para tabla 'duenos' en Supabase
    /// </summary>
    [Table("duenos")]
    public class DuenoSupabase : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Column("cedula")]
        public string? Cedula { get; set; }

        [Column("tipo_cedula")]
        public string? TipoCedula { get; set; } = "Nacional";

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("fecha_actualizacion")]
        public DateTime FechaActualizacion { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        // Propiedad calculada (NO se guarda en BD, solo para uso en UI)
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
    }
}
