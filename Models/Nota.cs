namespace FlujoCajaWpf.Models
{
    /// <summary>
    /// Modelo de nota para uso en la UI
    /// </summary>
    public class Nota
    {
        public int Id { get; set; }
        public int CasaId { get; set; }
        public string Contenido { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        
        // Propiedad calculada para mostrar en UI
        public string FechaFormateada => FechaCreacion.ToString("dd/MM/yyyy HH:mm");
    }
}
