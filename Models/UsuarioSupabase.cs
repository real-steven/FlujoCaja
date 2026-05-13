using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FlujoCajaWpf.Models
{
    [Table("usuarios")]
    public class UsuarioSupabase : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; } = string.Empty;

        [Column("auth_id")]
        public string? AuthId { get; set; }

        [Column("nombre")]
        public string? Nombre { get; set; }

        [Column("apellido")]
        public string? Apellido { get; set; }

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("rol")]
        public string Rol { get; set; } = "usuario";

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("foto_perfil")]
        public string? FotoPerfil { get; set; }

        [Column("debe_cambiar_password")]
        public bool DebeCambiarPassword { get; set; } = false;

        public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
    }
}
