namespace FlujoCajaWpf.Models
{
    public class LogCorreo
    {
        public int Id { get; set; }
        public string UsuarioEmail { get; set; } = string.Empty;
        public string CasaNombre { get; set; } = string.Empty;
        public string Destinatarios { get; set; } = string.Empty;
        public string Asunto { get; set; } = string.Empty;
        public string Adjuntos { get; set; } = string.Empty;
        public string? MensajeId { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? ErrorDetalle { get; set; }
        public string? UrlPdf { get; set; }
        public string? UrlExcel { get; set; }
        public DateTime Fecha { get; set; }

        // Propiedades calculadas para UI
        public string FechaTexto => Fecha.ToString("dd/MM/yyyy HH:mm");

        public string EstadoTexto => Estado switch
        {
            "enviado" => "✅ Enviado",
            "error"   => "❌ Error",
            _         => Estado
        };

        public string AdjuntosTexto => Adjuntos switch
        {
            "pdf"      => "📄 PDF",
            "excel"    => "📊 Excel",
            "pdf+excel"=> "📄📊 PDF + Excel",
            _          => Adjuntos
        };
    }
}
