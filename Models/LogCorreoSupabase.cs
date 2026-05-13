using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FlujoCajaWpf.Models
{
    [Table("log_correos")]
    public class LogCorreoSupabase : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("usuario_email")]
        public string UsuarioEmail { get; set; } = string.Empty;

        [Column("casa_nombre")]
        public string CasaNombre { get; set; } = string.Empty;

        [Column("destinatarios")]
        public string Destinatarios { get; set; } = string.Empty;

        [Column("asunto")]
        public string Asunto { get; set; } = string.Empty;

        [Column("adjuntos")]
        public string Adjuntos { get; set; } = string.Empty; // "pdf", "excel", "pdf+excel"

        [Column("mensaje_id")]
        public string? MensajeId { get; set; }

        [Column("estado")]
        public string Estado { get; set; } = string.Empty; // "enviado", "error"

        [Column("error_detalle")]
        public string? ErrorDetalle { get; set; }

        [Column("url_pdf")]
        public string? UrlPdf { get; set; }

        [Column("url_excel")]
        public string? UrlExcel { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }
    }
}
