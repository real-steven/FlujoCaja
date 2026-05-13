using FlujoCajaWpf.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FlujoCajaWpf.Data
{
    /// <summary>
    /// Helper para gestión de usuarios (solo admin).
    /// Usa la tabla "usuarios" en Supabase con la columna auth_id para referenciar al auth.users.
    /// </summary>
    public static class SupabaseAdminHelper
    {
        /// <summary>
        /// Obtiene todos los usuarios registrados en la tabla usuarios.
        /// </summary>
        public static async Task<List<UsuarioSupabase>> ObtenerUsuariosAsync()
        {
            try
            {
                var resultado = await SupabaseHelper.Client
                    .From<UsuarioSupabase>()
                    .Order(u => u.Email, Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                return resultado.Models ?? new List<UsuarioSupabase>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
                return new List<UsuarioSupabase>();
            }
        }

        /// <summary>
        /// Obtiene el usuario admin para mostrar en el botón de soporte (sin auth, usa service role key).
        /// </summary>
        public static async Task<UsuarioSupabase?> ObtenerAdminSoporteAsync()
        {
            try
            {
                var url = SupabaseHelper.Url;
                var serviceKey = SupabaseHelper.ServiceRoleKey;

                using var http = new HttpClient();
                http.DefaultRequestHeaders.Add("apikey", serviceKey);
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", serviceKey);
                http.DefaultRequestHeaders.Add("Accept", "application/json");

                var response = await http.GetAsync(
                    $"{url}/rest/v1/usuarios?rol=eq.admin&activo=eq.true&select=nombre,apellido,email,telefono&limit=1");

                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var arr = doc.RootElement;
                if (arr.GetArrayLength() == 0) return null;

                var obj = arr[0];
                return new UsuarioSupabase
                {
                    Nombre   = obj.TryGetProperty("nombre",   out var n) ? n.GetString() : null,
                    Apellido = obj.TryGetProperty("apellido", out var a) ? a.GetString() : null,
                    Email    = obj.TryGetProperty("email",    out var e) ? e.GetString() ?? "" : "",
                    Telefono = obj.TryGetProperty("telefono", out var t) ? t.GetString() : null,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener admin soporte: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Crea un nuevo usuario en Supabase Auth (Admin API, sin enviar email de confirmación)
        /// y luego lo registra en la tabla usuarios.
        /// </summary>
        public static async Task<(bool Success, string? Error)> CrearUsuarioAsync(
            string email, string password, string nombre, string apellido, string rol = "usuario", string telefono = "")
        {
            try
            {
                var url = SupabaseHelper.Url;
                var serviceKey = SupabaseHelper.ServiceRoleKey;

                if (string.IsNullOrWhiteSpace(serviceKey) || serviceKey == "TU_SERVICE_ROLE_KEY_AQUI")
                    return (false, "ServiceRoleKey no configurada en appsettings.json.");

                // 1. Crear usuario via Admin API (email_confirm=true → no envía correo, crea ya confirmado)
                using var http = new HttpClient();
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", serviceKey);
                http.DefaultRequestHeaders.Add("apikey", serviceKey);

                var body = JsonSerializer.Serialize(new
                {
                    email,
                    password,
                    email_confirm = true   // crea el usuario ya confirmado, sin enviar email
                });

                var response = await http.PostAsync(
                    $"{url}/auth/v1/admin/users",
                    new StringContent(body, Encoding.UTF8, "application/json"));

                var respContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return (false, respContent);

                using var doc = JsonDocument.Parse(respContent);
                var authId = doc.RootElement.GetProperty("id").GetString();

                if (string.IsNullOrEmpty(authId))
                    return (false, "No se recibió el ID del usuario creado.");

                // 2. Insertar en tabla usuarios
                var nuevo = new UsuarioSupabase
                {
                    AuthId   = authId,
                    Email    = email,
                    Nombre   = nombre,
                    Apellido = apellido,
                    Rol      = rol,
                    Telefono = string.IsNullOrWhiteSpace(telefono) ? null : telefono,
                    Activo   = true
                };

                await SupabaseHelper.Client.From<UsuarioSupabase>().Insert(nuevo);
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear usuario: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Cambia el rol de un usuario en la tabla usuarios.
        /// </summary>
        public static async Task<(bool Success, string? Error)> CambiarRolAsync(string usuarioId, string nuevoRol)
        {
            try
            {
                await SupabaseHelper.Client
                    .From<UsuarioSupabase>()
                    .Where(u => u.Id == usuarioId)
                    .Set(u => u.Rol!, nuevoRol)
                    .Update();

                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cambiar rol: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Activa o desactiva un usuario (campo activo en tabla usuarios).
        /// </summary>
        public static async Task<(bool Success, string? Error)> CambiarEstadoAsync(string usuarioId, bool activo)
        {
            try
            {
                await SupabaseHelper.Client
                    .From<UsuarioSupabase>()
                    .Where(u => u.Id == usuarioId)
                    .Set(u => u.Activo, activo)
                    .Update();

                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cambiar estado: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Cambia el correo electrónico de un usuario en auth.users (Admin API) y en la tabla usuarios.
        /// Solo debe llamarse para usuarios con rol "usuario", nunca para admins.
        /// </summary>
        public static async Task<(bool Success, string? Error)> CambiarEmailAsync(
            string usuarioId, string authId, string nuevoEmail)
        {
            try
            {
                var url        = SupabaseHelper.Url;
                var serviceKey = SupabaseHelper.ServiceRoleKey;

                if (string.IsNullOrWhiteSpace(serviceKey) || serviceKey == "TU_SERVICE_ROLE_KEY_AQUI")
                    return (false, "ServiceRoleKey no configurada en appsettings.json.");

                // 1. Actualizar email en auth.users vía Admin API
                using var http = new HttpClient();
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", serviceKey);
                http.DefaultRequestHeaders.Add("apikey", serviceKey);

                var body = JsonSerializer.Serialize(new { email = nuevoEmail, email_confirm = true });
                var response = await http.PutAsync(
                    $"{url}/auth/v1/admin/users/{authId}",
                    new StringContent(body, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    var respContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[CambiarEmail] HTTP {(int)response.StatusCode} | authId={authId} | body={respContent}");
                    return (false, $"Error en Auth API (HTTP {(int)response.StatusCode}): {(string.IsNullOrWhiteSpace(respContent) ? "sin respuesta del servidor" : respContent)}");
                }

                // 2. Actualizar email en tabla usuarios
                await SupabaseHelper.Client
                    .From<UsuarioSupabase>()
                    .Where(u => u.Id == usuarioId)
                    .Set(u => u.Email, nuevoEmail)
                    .Update();

                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cambiar email: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Busca al usuario por email, genera contraseña temporal y la envía por Resend.
        /// Usado desde el login ("¿Olvidó su contraseña?").
        /// </summary>
        public static async Task<(bool Success, string? Error)> EnviarPasswordTemporalPorCorreoAsync(string email)
        {
            try
            {
                // 1. Buscar el auth_id desde la tabla usuarios usando service role (bypass RLS)
                var url        = SupabaseHelper.Url;
                var serviceKey = SupabaseHelper.ServiceRoleKey;

                using var httpLookup = new HttpClient();
                httpLookup.DefaultRequestHeaders.Add("apikey", serviceKey);
                httpLookup.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", serviceKey);

                var emailEncoded = Uri.EscapeDataString(email);
                var lookupResp = await httpLookup.GetAsync(
                    $"{url}/rest/v1/usuarios?email=eq.{emailEncoded}&select=id,auth_id,email&limit=1");

                if (!lookupResp.IsSuccessStatusCode)
                    return (false, $"[Paso 1] Error al buscar usuario (HTTP {(int)lookupResp.StatusCode}): {await lookupResp.Content.ReadAsStringAsync()}");

                var json = await lookupResp.Content.ReadAsStringAsync();
                Console.WriteLine($"[OlvidePass] Lookup response: {json}");
                var rows = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(json);

                if (rows == null || rows.Count == 0)
                    return (false, "No se encontró ningún usuario con ese correo.");

                rows[0].TryGetValue("auth_id", out var authIdEl);
                var authId = authIdEl.ValueKind == JsonValueKind.String ? authIdEl.GetString() : null;

                if (string.IsNullOrWhiteSpace(authId))
                    return (false, "No se encontró ningún usuario con ese correo.");

                // 2. Generar y establecer contraseña temporal
                Console.WriteLine($"[OlvidePass] authId encontrado: {authId}");
                var (ok, tempPass, err) = await EstablecerPasswordTemporalAsync(authId);
                Console.WriteLine($"[OlvidePass] EstablecerPassword ok={ok} err={err}");
                if (!ok || tempPass == null)
                    return (false, string.IsNullOrWhiteSpace(err) ? "Error al generar contraseña temporal (sin detalle)." : err);

                // 3. Enviar por Resend
                Console.WriteLine($"[OlvidePass] Contraseña generada, enviando email a {email}...");
                var resendConfig = await SupabaseHelper.ObtenerConfigResendAsync();
                if (resendConfig == null || string.IsNullOrWhiteSpace(resendConfig.ApiKey))
                    return (false, "Resend no está configurado en appsettings.json.");

                using var http = new HttpClient();
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", resendConfig.ApiKey);

                var html = $@"
                    <div style='font-family:Segoe UI,sans-serif;max-width:480px;margin:auto;padding:24px'>
                        <h2 style='color:#202355;margin-bottom:8px'>🔐 Contraseña temporal</h2>
                        <p style='color:#374151'>Recibiste este correo porque solicitaste restablecer tu contraseña en <strong>Samara Rentals</strong>.</p>
                        <div style='background:#F3F4F6;border-radius:8px;padding:16px 24px;margin:20px 0;text-align:center'>
                            <p style='margin:0;font-size:13px;color:#6B7280'>Tu contraseña temporal es:</p>
                            <p style='margin:8px 0 0;font-size:22px;font-weight:bold;color:#202355;letter-spacing:2px'>{tempPass}</p>
                        </div>
                        <p style='color:#374151'>Usa esta contraseña para iniciar sesión y cámbiala cuanto antes.</p>
                        <p style='color:#9CA3AF;font-size:12px'>Si no solicitaste esto, ignora este correo.</p>
                    </div>";

                var payload = JsonSerializer.Serialize(new
                {
                    from    = resendConfig.From,
                    to      = new[] { email },
                    subject = "Tu contraseña temporal - Samara Rentals",
                    html
                });

                var response = await http.PostAsync(
                    "https://api.resend.com/emails",
                    new StringContent(payload, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    var resendErr = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[OlvidePass] Resend error {(int)response.StatusCode}: {resendErr}");
                    return (false, $"Error Resend ({(int)response.StatusCode}): {resendErr}");
                }

                Console.WriteLine("[OlvidePass] Email enviado correctamente.");

                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OlvidePass] EXCEPCIÓN: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                return (false, string.IsNullOrWhiteSpace(ex.Message) ? $"Excepción inesperada: {ex.GetType().Name}" : ex.Message);
            }
        }

        /// <summary>
        /// Establece una contraseña temporal para el usuario directamente via Admin API.
        /// No envía correo — el admin le comunica la contraseña al usuario.
        /// </summary>
        public static async Task<(bool Success, string? TempPassword, string? Error)> EstablecerPasswordTemporalAsync(string authId)
        {
            try
            {
                var url        = SupabaseHelper.Url;
                var serviceKey = SupabaseHelper.ServiceRoleKey;

                if (string.IsNullOrWhiteSpace(serviceKey) || serviceKey == "TU_SERVICE_ROLE_KEY_AQUI")
                    return (false, null, "ServiceRoleKey no configurada en appsettings.json.");

                Console.WriteLine($"[EstablecerPass] url={url}, authId={authId}, serviceKeyLen={serviceKey?.Length}");
                // Generar contraseña
                var rng      = new Random();
                var palabras = new[] { "Casa", "Renta", "Pago", "Flujo", "Admin", "Caja", "Samara" };
                var tempPass = $"{palabras[rng.Next(palabras.Length)]}{rng.Next(100, 999)}!{palabras[rng.Next(palabras.Length)]}";

                using var http = new HttpClient();
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", serviceKey);
                http.DefaultRequestHeaders.Add("apikey", serviceKey);

                var body = JsonSerializer.Serialize(new { password = tempPass });

                var response = await http.PutAsync(
                    $"{url}/auth/v1/admin/users/{authId}",
                    new StringContent(body, Encoding.UTF8, "application/json"));

                var respContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[EstablecerPass] HTTP {(int)response.StatusCode}, body: {respContent}");
                if (!response.IsSuccessStatusCode)
                    return (false, null, $"Admin API HTTP {(int)response.StatusCode} (authId={authId}): {(string.IsNullOrWhiteSpace(respContent) ? "sin respuesta" : respContent)}");

                return (true, tempPass, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al establecer contraseña temporal: {ex.Message}");
                return (false, null, ex.Message);
            }
        }
    }
}
