using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FlujoDeCajaApp.Modelos
{
    /// <summary>
    /// Modelo de datos para representar una categoría de movimiento en Supabase
    /// Compatible con BaseModel para operaciones CRUD automáticas
    /// </summary>
    [Table("categorias_movimientos")]
    public class CategoriaMovimientoSupabase : BaseModel
    {
        /// <summary>
        /// Identificador único de la categoría de movimiento
        /// </summary>
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        /// <summary>
        /// Nombre de la categoría de movimiento
        /// </summary>
        [Column("nombre")]
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Descripción opcional de la categoría
        /// </summary>
        [Column("descripcion")]
        public string? Descripcion { get; set; } = null;

        /// <summary>
        /// Tipo de entidad (siempre será "movimiento" para esta tabla)
        /// </summary>
        [Column("tipo_entidad")]
        public string TipoEntidad { get; set; } = "movimiento";

        /// <summary>
        /// Indica si el registro está activo
        /// </summary>
        [Column("activo")]
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public CategoriaMovimientoSupabase()
        {
            FechaCreacion = DateTime.UtcNow;
        }

        /// <summary>
        /// Constructor con parámetros básicos
        /// </summary>
        /// <param name="nombre">Nombre de la categoría</param>
        public CategoriaMovimientoSupabase(string nombre)
        {
            Nombre = nombre;
            TipoEntidad = "movimiento";
            FechaCreacion = DateTime.UtcNow;
        }

        /// <summary>
        /// Constructor con parámetros completos
        /// </summary>
        /// <param name="nombre">Nombre de la categoría</param>
        /// <param name="descripcion">Descripción opcional</param>
        public CategoriaMovimientoSupabase(string nombre, string? descripcion)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            TipoEntidad = "movimiento";
            FechaCreacion = DateTime.UtcNow;
        }

        /// <summary>
        /// Representación en cadena del objeto
        /// </summary>
        /// <returns>Nombre de la categoría</returns>
        public override string ToString()
        {
            return Nombre;
        }
    }
}