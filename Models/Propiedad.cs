using System.Windows;

namespace FlujoCajaWpf.Models
{
    /// <summary>
    /// Modelo extendido de propiedad para visualización en tarjetas del menú
    /// </summary>
    public class Propiedad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Moneda { get; set; } = "USD";
        public string? CategoriaNombre { get; set; }
        public bool Activo { get; set; }
        public string DuenoNombre { get; set; } = "Sin dueño";
        public string? Notas { get; set; }
        public string EstadoTexto => Activo ? "Activa" : "Inactiva";
        public string ColorEstado => Activo ? "#10B981" : "#64748B"; // Verde success o gris medio        
        // Propiedades para indicadores financieros
        public decimal Balance { get; set; }
        public string ColorBorde { get; set; } = "#E5E7EB"; // Gris por defecto
        public Thickness GrosordeBorde { get; set; } = new Thickness(1); // Thickness por defecto
        public static Propiedad FromCasa(Casa casa)
        {
            return new Propiedad
            {
                Id = casa.Id,
                Nombre = casa.Nombre,
                Moneda = casa.Moneda,
                CategoriaNombre = casa.CategoriaNombre,
                Activo = casa.Activo,
                DuenoNombre = casa.DuenoNombre ?? "Sin dueño",
                Notas = casa.Notas
            };
        }
    }
}
