using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FlujoCajaWpf.Models
{
    [Table("preferencias_usuario")]
    public class PreferenciasUsuarioSupabase : BaseModel
    {
        [PrimaryKey("id", false)]
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_email")]
        public string UsuarioEmail { get; set; } = string.Empty;

        [Column("modo_oscuro")]
        public bool ModoOscuro { get; set; } = false;

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("fecha_actualizacion")]
        public DateTime FechaActualizacion { get; set; }
    }

    /// <summary>
    /// Modelo de UI para preferencias de usuario
    /// </summary>
    public class PreferenciasUsuario
    {
        public int Id { get; set; }
        public string UsuarioEmail { get; set; } = string.Empty;
        public bool ModoOscuro { get; set; } = false;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public static PreferenciasUsuario FromSupabase(PreferenciasUsuarioSupabase? db)
        {
            if (db == null)
            {
                return new PreferenciasUsuario();
            }

            return new PreferenciasUsuario
            {
                Id = db.Id,
                UsuarioEmail = db.UsuarioEmail,
                ModoOscuro = db.ModoOscuro,
                FechaCreacion = db.FechaCreacion,
                FechaActualizacion = db.FechaActualizacion
            };
        }
    }
}
