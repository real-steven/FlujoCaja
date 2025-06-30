using System;

namespace FlujoDeCajaApp.Modelos
{
    /// <summary>
    /// Modelo de datos para representar una categoría de propiedades
    /// </summary>
    public class Categoria
    {
        /// <summary>
        /// Identificador único de la categoría
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de la categoría
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la categoría
        /// </summary>
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Indica si el registro está activo
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Categoria()
        {
            FechaCreacion = DateTime.Now;
        }

        /// <summary>
        /// Constructor con parámetros
        /// </summary>
        /// <param name="nombre">Nombre de la categoría</param>
        /// <param name="descripcion">Descripción de la categoría</param>
        public Categoria(string nombre, string descripcion = "")
        {
            Nombre = nombre;
            Descripcion = descripcion;
            FechaCreacion = DateTime.Now;
        }

        /// <summary>
        /// Valida si los datos de la categoría son válidos
        /// </summary>
        /// <returns>True si los datos son válidos, False en caso contrario</returns>
        public bool EsValida()
        {
            return !string.IsNullOrWhiteSpace(Nombre);
        }

        /// <summary>
        /// Representación en string de la categoría
        /// </summary>
        /// <returns>Nombre de la categoría</returns>
        public override string ToString()
        {
            return Nombre;
        }
    }
}
