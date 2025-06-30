namespace FlujoDeCajaApp.Modelos
{
    /// <summary>
    /// Modelo que representa una propiedad en el sistema
    /// </summary>
    public class Propiedad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string RutaImagen { get; set; } = string.Empty;
        public bool Activa { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public int IdDueno { get; set; }
        public string NombreDueno { get; set; } = string.Empty;
        
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Propiedad() { }

        /// <summary>
        /// Constructor con parámetros principales
        /// </summary>
        /// <param name="nombre">Nombre de la propiedad</param>
        /// <param name="descripcion">Descripción de la propiedad</param>
        /// <param name="rutaImagen">Ruta de la imagen</param>
        public Propiedad(string nombre, string descripcion, string rutaImagen)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            RutaImagen = rutaImagen;
        }
    }
}
