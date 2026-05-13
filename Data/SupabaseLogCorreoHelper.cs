using FlujoCajaWpf.Models;

namespace FlujoCajaWpf.Data
{
    public static class SupabaseLogCorreoHelper
    {
        /// <summary>
        /// Registra un intento de envío de correo (exitoso o fallido).
        /// </summary>
        public static async Task<(bool Success, string? Error)> RegistrarEnvioAsync(
            string usuarioEmail,
            string casaNombre,
            IEnumerable<string> destinatarios,
            string asunto,
            string adjuntos,
            string estado,
            string? mensajeId = null,
            string? errorDetalle = null,
            string? urlPdf = null,
            string? urlExcel = null)
        {
            try
            {
                var registro = new LogCorreoSupabase
                {
                    UsuarioEmail = usuarioEmail,
                    CasaNombre = casaNombre,
                    Destinatarios = string.Join(", ", destinatarios),
                    Asunto = asunto,
                    Adjuntos = adjuntos,
                    Estado = estado,
                    MensajeId = mensajeId,
                    ErrorDetalle = errorDetalle,
                    UrlPdf = urlPdf,
                    UrlExcel = urlExcel,
                    Fecha = DateTime.Now
                };

                await SupabaseHelper.Client.From<LogCorreoSupabase>().Insert(registro);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene logs de correos con filtros y paginación.
        /// </summary>
        public static async Task<(bool Success, List<LogCorreo>? Data, int TotalRegistros, string? Error)> ObtenerLogsAsync(
            string? usuarioEmail = null,
            string? casaNombre = null,
            string? estado = null,
            string? buscar = null,
            int pagina = 1,
            int registrosPorPagina = 50)
        {
            try
            {
                var client = SupabaseHelper.Client;
                var query = client.From<LogCorreoSupabase>().Select("*");

                if (!string.IsNullOrEmpty(usuarioEmail))
                    query = query.Where(l => l.UsuarioEmail == usuarioEmail);

                if (!string.IsNullOrEmpty(casaNombre))
                    query = query.Where(l => l.CasaNombre == casaNombre);

                if (!string.IsNullOrEmpty(estado))
                    query = query.Where(l => l.Estado == estado);

                var resultadoTotal = await query
                    .Order("fecha", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                var todos = resultadoTotal?.Models ?? new List<LogCorreoSupabase>();

                // Filtro de texto client-side
                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    var b = buscar.ToLower();
                    todos = todos.Where(l =>
                        (l.CasaNombre?.ToLower().Contains(b) ?? false) ||
                        (l.Destinatarios?.ToLower().Contains(b) ?? false) ||
                        (l.Asunto?.ToLower().Contains(b) ?? false) ||
                        (l.UsuarioEmail?.ToLower().Contains(b) ?? false)
                    ).ToList();
                }

                int totalRegistros = todos.Count;

                int inicio = (pagina - 1) * registrosPorPagina;
                var paginados = todos.Skip(inicio).Take(registrosPorPagina).ToList();

                var logs = paginados.Select(l => new LogCorreo
                {
                    Id = l.Id,
                    UsuarioEmail = l.UsuarioEmail,
                    CasaNombre = l.CasaNombre,
                    Destinatarios = l.Destinatarios,
                    Asunto = l.Asunto,
                    Adjuntos = l.Adjuntos,
                    MensajeId = l.MensajeId,
                    Estado = l.Estado,
                    ErrorDetalle = l.ErrorDetalle,
                    UrlPdf = l.UrlPdf,
                    UrlExcel = l.UrlExcel,
                    Fecha = l.Fecha
                }).ToList();

                return (true, logs, totalRegistros, null);
            }
            catch (Exception ex)
            {
                return (false, null, 0, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene los nombres de casas únicos que aparecen en el log.
        /// </summary>
        public static async Task<List<string>> ObtenerCasasAsync()
        {
            try
            {
                var client = SupabaseHelper.Client;
                var resultado = await client.From<LogCorreoSupabase>().Select("casa_nombre").Get();
                return resultado?.Models
                    .Select(l => l.CasaNombre)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .Distinct()
                    .OrderBy(n => n)
                    .ToList() ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Obtiene los usuarios únicos que aparecen en el log.
        /// </summary>
        public static async Task<List<string>> ObtenerUsuariosAsync()
        {
            try
            {
                var client = SupabaseHelper.Client;
                var resultado = await client.From<LogCorreoSupabase>().Select("usuario_email").Get();
                return resultado?.Models
                    .Select(l => l.UsuarioEmail)
                    .Where(e => !string.IsNullOrEmpty(e))
                    .Distinct()
                    .OrderBy(e => e)
                    .ToList() ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}
