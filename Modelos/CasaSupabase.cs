using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace FlujoDeCajaApp.Modelos
{
    /// <summary>
    /// Modelo de datos para representar una casa/propiedad en Supabase
    /// Compatible con BaseModel para operaciones CRUD automáticas
    /// </summary>
    [Table("casas")]
    public class CasaSupabase : BaseModel
    {
        /// <summary>
        /// Identificador único de la casa
        /// </summary>
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        /// <summary>
        /// Nombre descriptivo de la casa
        /// </summary>
        [Column("nombre")]
        [Required(ErrorMessage = "El nombre es requerido")]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Indica si el registro está activo
        /// </summary>
        [Column("activo")]
        public bool Activo { get; set; } = true;

        /// <summary>
        /// ID del dueño de la casa (referencia a tabla duenos)
        /// </summary>
        [Column("duenoid")]
        [Required(ErrorMessage = "El dueño es requerido")]
        public int DuenoId { get; set; }

        /// <summary>
        /// ID de la categoría de la casa (referencia a tabla categorias)
        /// </summary>
        [Column("categoriaid")]
        [Required(ErrorMessage = "La categoría es requerida")]
        public int CategoriaId { get; set; }

        /// <summary>
        /// Ruta de la imagen de la casa
        /// </summary>
        [Column("rutaimagen")]
        public string? RutaImagen { get; set; } = null;

        /// <summary>
        /// Moneda utilizada para la casa (USD, CRC, EUR)
        /// </summary>
        [Column("moneda")]
        [Required(ErrorMessage = "La moneda es requerida")]
        public string Moneda { get; set; } = "USD";

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public CasaSupabase()
        {
            FechaCreacion = DateTime.UtcNow;
        }

        /// <summary>
        /// Constructor con parámetros básicos
        /// </summary>
        public CasaSupabase(string nombre, int duenoId, int categoriaId, string moneda = "USD")
        {
            Nombre = nombre;
            DuenoId = duenoId;
            CategoriaId = categoriaId;
            Moneda = moneda;
            FechaCreacion = DateTime.UtcNow;
        }

        /// <summary>
        /// Constructor completo
        /// </summary>
        public CasaSupabase(string nombre, int duenoId, int categoriaId, string? rutaImagen = null, string moneda = "USD") 
            : this(nombre, duenoId, categoriaId, moneda)
        {
            RutaImagen = rutaImagen;
        }

        /// <summary>
        /// Valida que los datos de la casa sean correctos
        /// </summary>
        public bool EsValido()
        {
            var monedasValidas = new[] { "USD", "CRC", "EUR" };
            return !string.IsNullOrWhiteSpace(Nombre) && 
                   DuenoId > 0 && 
                   CategoriaId > 0 &&
                   !string.IsNullOrWhiteSpace(Moneda) &&
                   monedasValidas.Contains(Moneda);
        }

        /// <summary>
        /// Devuelve una representación en string de la casa
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