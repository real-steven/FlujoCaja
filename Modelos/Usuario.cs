using System;

namespace FlujoDeCajaApp.Modelos
{
    /// <summary>
    /// Modelo de datos para representar un usuario del sistema
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Identificador único del usuario
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de usuario único para el login
        /// </summary>
        public string NombreUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña del usuario (almacenada como hash)
        /// </summary>
        public string Contrasena { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico del usuario
        /// </summary>
        public string Correo { get; set; } = string.Empty;

        /// <summary>
        /// Rol del usuario en el sistema (Admin, Usuario, etc.)
        /// </summary>
        public string Rol { get; set; } = "Usuario";

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Indica si el usuario está activo
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Usuario()
        {
            FechaCreacion = DateTime.Now;
        }

        /// <summary>
        /// Constructor con parámetros
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario</param>
        /// <param name="contrasena">Contraseña</param>
        /// <param name="correo">Correo electrónico</param>
        /// <param name="rol">Rol del usuario</param>
        public Usuario(string nombreUsuario, string contrasena, string correo, string rol = "Usuario")
        {
            NombreUsuario = nombreUsuario;
            Contrasena = contrasena;
            Correo = correo;
            Rol = rol;
            FechaCreacion = DateTime.Now;
        }

        /// <summary>
        /// Devuelve una representación string del usuario
        /// </summary>
        /// <returns>String con información del usuario</returns>
        public override string ToString()
        {
            return $"{NombreUsuario} ({Correo}) - {Rol}";
        }

        /// <summary>
        /// Verifica si el usuario tiene un rol específico
        /// </summary>
        /// <param name="rol">Rol a verificar</param>
        /// <returns>True si tiene el rol, False en caso contrario</returns>
        public bool TieneRol(string rol)
        {
            return string.Equals(Rol, rol, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Valida si los datos del usuario son válidos
        /// </summary>
        /// <returns>True si son válidos, False en caso contrario</returns>
        public bool EsValido()
        {
            return !string.IsNullOrWhiteSpace(NombreUsuario) &&
                   !string.IsNullOrWhiteSpace(Contrasena) &&
                   !string.IsNullOrWhiteSpace(Correo) &&
                   ValidarFormatoCorreo(Correo);
        }

        /// <summary>
        /// Valida el formato de un correo electrónico
        /// </summary>
        /// <param name="correo">Correo a validar</param>
        /// <returns>True si el formato es válido, False en caso contrario</returns>
        private static bool ValidarFormatoCorreo(string correo)
        {
            try
            {
                var direccion = new System.Net.Mail.MailAddress(correo);
                return direccion.Address == correo;
            }
            catch
            {
                return false;
            }
        }
    }
}
