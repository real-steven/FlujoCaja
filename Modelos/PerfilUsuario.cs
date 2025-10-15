using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace FlujoDeCajaApp.Modelos
{
    /// <summary>
    /// Modelo de datos para representar un perfil de usuario extendido en Supabase
    /// Este modelo se complementa con Supabase Auth para el manejo completo de usuarios
    /// </summary>
    [Table("perfiles")]
    public class PerfilUsuario : BaseModel
    {
        /// <summary>
        /// ID del usuario (debe coincidir con el ID de auth.users en Supabase)
        /// </summary>
        [PrimaryKey("id", false)]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        [Column("nombre_completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        /// <summary>
        /// Nombre de usuario único para mostrar
        /// </summary>
        [Column("nombre_usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Rol del usuario en el sistema (Admin, Usuario, etc.)
        /// </summary>
        [Column("rol")]
        public string Rol { get; set; } = "Usuario";

        /// <summary>
        /// Teléfono del usuario
        /// </summary>
        [Column("telefono")]
        public string? Telefono { get; set; }

        /// <summary>
        /// Avatar/foto del usuario (URL)
        /// </summary>
        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Indica si el usuario está activo
        /// </summary>
        [Column("activo")]
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Fecha de creación del perfil
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public PerfilUsuario()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Devuelve una representación en string del usuario
        /// </summary>
        public override string ToString()
        {
            return $"{NombreCompleto} ({NombreUsuario})";
        }

        /// <summary>
        /// Valida que los datos del usuario sean correctos
        /// </summary>
        public bool EsValido()
        {
            return !string.IsNullOrWhiteSpace(NombreCompleto) && 
                   !string.IsNullOrWhiteSpace(NombreUsuario);
        }
    }
}