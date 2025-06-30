using System;

namespace FlujoDeCajaApp.Modelos
{
    /// <summary>
    /// Modelo de datos para representar un dueño de propiedades
    /// </summary>
    public class Dueno
    {
        /// <summary>
        /// Identificador único del dueño
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del dueño
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del dueño
        /// </summary>
        public string Apellido { get; set; } = string.Empty;

        /// <summary>
        /// Número de teléfono del dueño
        /// </summary>
        public string Telefono { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico del dueño
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Indica si el registro está activo
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Obtiene el nombre completo del dueño
        /// </summary>
        public string NombreCompleto => $"{Nombre} {Apellido}".Trim();

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Dueno()
        {
            FechaCreacion = DateTime.Now;
        }

        /// <summary>
        /// Constructor con parámetros
        /// </summary>
        /// <param name="nombre">Nombre del dueño</param>
        /// <param name="apellido">Apellido del dueño</param>
        /// <param name="telefono">Teléfono del dueño</param>
        /// <param name="email">Email del dueño</param>
        public Dueno(string nombre, string apellido, string telefono = "", string email = "")
        {
            Nombre = nombre;
            Apellido = apellido;
            Telefono = telefono;
            Email = email;
            FechaCreacion = DateTime.Now;
        }

        /// <summary>
        /// Valida si los datos del dueño son válidos
        /// </summary>
        /// <returns>True si los datos son válidos, False en caso contrario</returns>
        public bool EsValido()
        {
            return !string.IsNullOrWhiteSpace(Nombre) && !string.IsNullOrWhiteSpace(Apellido);
        }

        /// <summary>
        /// Representación en string del dueño
        /// </summary>
        /// <returns>Nombre completo del dueño</returns>
        public override string ToString()
        {
            return NombreCompleto;
        }
    }
}
