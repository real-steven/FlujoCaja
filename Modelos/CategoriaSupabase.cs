using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FlujoDeCajaApp.Modelos
{
    /// <summary>
    /// Modelo de datos para representar una categoría de propiedades en Supabase
    /// Compatible con BaseModel para operaciones CRUD automáticas
    /// </summary>
    [Table("categorias")]
    public class CategoriaSupabase : BaseModel
    {
        /// <summary>
        /// Identificador único de la categoría
        /// </summary>
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        /// <summary>
        /// Nombre de la categoría
        /// </summary>
        [Column("nombre")]
        [Required(ErrorMessage = "El nombre es requerido")]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la categoría
        /// </summary>
        [Column("descripcion")]
        public string? Descripcion { get; set; } = null;

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Indica si el registro está activo
        /// </summary>
        [Column("activo")]
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public CategoriaSupabase()
        {
            FechaCreacion = DateTime.UtcNow;
        }

        /// <summary>
        /// Constructor con parámetros básicos
        /// </summary>
        public CategoriaSupabase(string nombre, string? descripcion = null)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            FechaCreacion = DateTime.UtcNow;
        }

        /// <summary>
        /// Valida que los datos de la categoría sean correctos
        /// </summary>
        public bool EsValido()
        {
            return !string.IsNullOrWhiteSpace(Nombre);
        }

        /// <summary>
        /// Devuelve una representación en string de la categoría
        /// </summary>
        public override string ToString()
        {
            return Nombre;
        }

        /// <summary>
        /// Actualiza la fecha de modificación
        /// </summary>
        public void ActualizarFecha()
        {
            FechaCreacion = DateTime.UtcNow;
        }
    }
}