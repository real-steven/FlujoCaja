namespace FlujoCajaWpf.Models
{
    /// <summary>
    /// Modelo de usuario para autenticación
    /// </summary>
    public class Usuario
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
