using Resend;
using System.IO;
using System.Text.RegularExpressions;

namespace FlujoCajaWpf.Services
{
    public record EmailAttachment(string FilePath);

    public record EmailResult(bool Success, string? MessageId, string? Error);

    public static class EmailService
    {
        private static readonly Regex _emailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Valida que una cadena tenga formato de correo electrónico válido.
        /// </summary>
        public static bool EsEmailValido(string email) =>
            !string.IsNullOrWhiteSpace(email) && _emailRegex.IsMatch(email.Trim());

        /// <summary>
        /// Envía un correo con adjuntos opcionales usando el SDK oficial de Resend.
        /// </summary>
        public static async Task<EmailResult> EnviarAsync(
            string apiKey,
            string from,
            IReadOnlyList<string> to,
            string subject,
            string htmlBody,
            IReadOnlyList<EmailAttachment>? attachments = null)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return new EmailResult(false, null, "API key de Resend no configurada.");

            if (to == null || to.Count == 0)
                return new EmailResult(false, null, "Se requiere al menos un destinatario.");

            try
            {
                IResend resend = ResendClient.Create(apiKey);

                var message = new EmailMessage
                {
                    From = from,
                    Subject = subject,
                    HtmlBody = htmlBody,
                };

                foreach (var recipient in to)
                    message.To.Add(recipient);

                if (attachments != null && attachments.Count > 0)
                {
                    var resendAttachments = new List<Resend.EmailAttachment>();
                    foreach (var att in attachments)
                    {
                        var sdkAtt = await Resend.EmailAttachment.FromAsync(att.FilePath);
                        resendAttachments.Add(sdkAtt);
                    }
                    message.Attachments = resendAttachments;
                }

                var resp = await resend.EmailSendAsync(message);
                return new EmailResult(true, resp.Content.ToString(), null);
            }
            catch (ResendException ex)
            {
                return new EmailResult(false, null, ex.Message);
            }
            catch (Exception ex)
            {
                return new EmailResult(false, null, $"Error inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica que un archivo exista en disco para poder adjuntarlo.
        /// </summary>
        public static EmailAttachment? CrearAttachment(string rutaArchivo) =>
            File.Exists(rutaArchivo) ? new EmailAttachment(rutaArchivo) : null;
    }
}
