namespace FlujoCajaWpf.Models
{
    /// <summary>
    /// Modelo de auditoría para UI
    /// </summary>
    public class Auditoria
    {
        public int Id { get; set; }
        public string UsuarioEmail { get; set; } = string.Empty;
        public string Modulo { get; set; } = string.Empty;
        public string TipoAccion { get; set; } = string.Empty;
        public int? EntidadId { get; set; }
        public string? EntidadNombre { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string? DatosAnteriores { get; set; }
        public string? DatosNuevos { get; set; }
        public DateTime Fecha { get; set; }

        // Propiedades calculadas para UI
        public string ModuloTexto => Modulo switch
        {
            "casa" => "🏠 Casa",
            "movimiento" => "💰 Movimiento",
            "dueno" => "👤 Dueño",
            "categoria" => "📁 Categoría",
            "categoria_movimiento" => "📊 Cat. Movimiento",
            _ => Modulo
        };

        public string AccionTexto => TipoAccion switch
        {
            "crear" => "➕ Crear",
            "editar" => "✏️ Editar",
            "eliminar" => "❌ Eliminar",
            "activar" => "🟢 Activar",
            "desactivar" => "🔴 Desactivar",
            _ => TipoAccion
        };

        public string FechaTexto => Fecha.ToString("dd/MM/yyyy HH:mm");

        /// <summary>
        /// Indica si esta acción puede ser deshecha
        /// </summary>
        public bool PuedeDeshacer => Modulo == "movimiento" && TipoAccion == "crear";
    }
}
