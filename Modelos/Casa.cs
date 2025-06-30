using System;

namespace FlujoDeCajaApp.Modelos
{
    /// <summary>
    /// Modelo de datos para representar una casa en el sistema
    /// </summary>
    public class Casa
    {
        /// <summary>
        /// Identificador único de la casa
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre descriptivo de la casa
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// ID del dueño de la casa
        /// </summary>
        public int DuenoId { get; set; }

        /// <summary>
        /// Nombre completo del dueño de la casa
        /// </summary>
        public string NombreDueno { get; set; } = string.Empty;

        /// <summary>
        /// ID de la categoría de la casa
        /// </summary>
        public int CategoriaId { get; set; }

        /// <summary>
        /// Nombre de la categoría de la casa
        /// </summary>
        public string NombreCategoria { get; set; } = string.Empty;

        /// <summary>
        /// Ruta relativa de la imagen de la casa
        /// </summary>
        public string RutaImagen { get; set; } = string.Empty;

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
        public Casa()
        {
            FechaCreacion = DateTime.Now;
        }

        /// <summary>
        /// Constructor con parámetros básicos
        /// </summary>
        /// <param name="nombre">Nombre de la casa</param>
        /// <param name="duenoId">ID del dueño</param>
        /// <param name="categoriaId">ID de la categoría</param>
        /// <param name="rutaImagen">Ruta de la imagen</param>
        public Casa(string nombre, int duenoId, int categoriaId, string rutaImagen = "")
        {
            Nombre = nombre;
            DuenoId = duenoId;
            CategoriaId = categoriaId;
            RutaImagen = rutaImagen;
            FechaCreacion = DateTime.Now;
        }

        /// <summary>
        /// Obtiene la ruta completa de la imagen de la casa
        /// </summary>
        /// <returns>Ruta completa del archivo de imagen</returns>
        public string ObtenerRutaCompletaImagen()
        {
            if (string.IsNullOrEmpty(RutaImagen))
                return string.Empty;

            string carpetaFotos = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FlujoDeCajaApp",
                "FotosCasas"
            );

            return Path.Combine(carpetaFotos, RutaImagen);
        }

        /// <summary>
        /// Valida si los datos de la casa son válidos
        /// </summary>
        /// <returns>True si los datos son válidos, False en caso contrario</returns>
        public bool EsValida()
        {
            return !string.IsNullOrWhiteSpace(Nombre) && 
                   DuenoId > 0 && 
                   CategoriaId > 0;
        }

        /// <summary>
        /// Representación en string de la casa
        /// </summary>
        /// <returns>Descripción de la casa</returns>
        public override string ToString()
        {
            return $"{Nombre} - {NombreDueno} ({NombreCategoria})";
        }
    }
}
