using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FlujoDeCajaApp.Modelos
{
    /// <summary>
    /// Modelo de datos para representar un dueño de propiedades en Supabase
    /// Compatible con BaseModel para operaciones CRUD automáticas
    /// </summary>
    [Table("duenos")]
    public class DuenoSupabase : BaseModel
    {
        /// <summary>
        /// Identificador único del dueño
        /// </summary>
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        /// <summary>
        /// Nombre del dueño
        /// </summary>
        [Column("nombre")]
        [Required(ErrorMessage = "El nombre es requerido")]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del dueño
        /// </summary>
        [Column("apellido")]
        [Required(ErrorMessage = "El apellido es requerido")]
        public string Apellido { get; set; } = string.Empty;

        /// <summary>
        /// Número de teléfono del dueño
        /// </summary>
        [Column("telefono")]
        public string Telefono { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico del dueño
        /// </summary>
        [Column("email")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        [Column("fecha_actualizacion")]
        public DateTime FechaActualizacion { get; set; }

        /// <summary>
        /// Indica si el registro está activo
        /// </summary>
        [Column("activo")]
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Obtiene el nombre completo del dueño (propiedad calculada, no se guarda en DB)
        /// </summary>
        [JsonIgnore]
        public string NombreCompleto => $"{Nombre} {Apellido}".Trim();

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public DuenoSupabase()
        {
            FechaCreacion = DateTime.UtcNow;
            FechaActualizacion = DateTime.UtcNow;
        }

        /// <summary>
        /// Constructor con parámetros básicos
        /// </summary>
        public DuenoSupabase(string nombre, string apellido)
        {
            Nombre = nombre;
            Apellido = apellido;
            FechaCreacion = DateTime.UtcNow;
            FechaActualizacion = DateTime.UtcNow;
        }

        /// <summary>
        /// Constructor completo
        /// </summary>
        public DuenoSupabase(string nombre, string apellido, string telefono, string email) 
            : this(nombre, apellido)
        {
            Telefono = telefono;
            Email = email;
        }

        /// <summary>
        /// Valida que los datos del dueño sean correctos
        /// </summary>
        public bool EsValido()
        {
            return !string.IsNullOrWhiteSpace(Nombre) && 
                   !string.IsNullOrWhiteSpace(Apellido);
        }

        /// <summary>
        /// Devuelve una representación en string del dueño
        /// </summary>
        public override string ToString()
        {
            return NombreCompleto;
        }

        /// <summary>
        /// Actualiza la fecha de modificación
        /// </summary>
        public void ActualizarFecha()
        {
            FechaActualizacion = DateTime.UtcNow;
        }
    }
}