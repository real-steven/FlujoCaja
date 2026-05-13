using FlujoCajaWpf.Models;
using Supabase.Gotrue;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FlujoCajaWpf.Data
{
    /// <summary>
    /// Helper para autenticación con Supabase Auth
    /// </summary>
    public static class SupabaseAuthHelper
    {
        /// <summary>
        /// Inicia sesión con email y contraseña
        /// </summary>
        public static async Task<(bool Success, Usuario? Usuario, string? Error)> SignInAsync(string email, string password)
        {
            try
            {
                var session = await SupabaseHelper.Client.Auth.SignIn(email, password);

                if (session?.User == null)
                {
                    return (false, null, "Credenciales inválidas");
                }

                if (string.IsNullOrEmpty(session.AccessToken))
                {
                    return (false, null, "No se recibió token de acceso");
                }

                Console.WriteLine($"📝 Token obtenido: {session.AccessToken.Substring(0, 20)}...");

                // Leer rol desde tabla usuarios
                var rol = await ObtenerRolAsync(session.User.Id ?? "");

                var usuario = new Usuario
                {
                    Id = session.User.Id,
                    Email = session.User.Email ?? email,
                    CreatedAt = session.User.CreatedAt,
                    Rol = rol
                };

                Console.WriteLine($"✓ Login exitoso: {usuario.Email} | Rol: {usuario.Rol}");
                return (true, usuario, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en login: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        public static async Task<(bool Success, string? Error)> SignUpAsync(string email, string password)
        {
            try
            {
                var session = await SupabaseHelper.Client.Auth.SignUp(email, password);

                if (session?.User == null)
                {
                    return (false, "Error al crear usuario");
                }

                Console.WriteLine($"✓ Usuario registrado: {email}");
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en registro: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Cierra la sesión actual
        /// </summary>
        public static async Task SignOutAsync()
        {
            try
            {
                await SupabaseHelper.Client.Auth.SignOut();
                Console.WriteLine("✓ Sesión cerrada");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cerrar sesión: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene el usuario actual (si hay sesión activa)
        /// </summary>
        public static Usuario? GetCurrentUser()
        {
            try
            {
                var user = SupabaseHelper.Client.Auth.CurrentUser;
                if (user == null) return null;

                return new Usuario
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    CreatedAt = user.CreatedAt
                    // Rol no disponible de forma síncrona; usar SignInAsync para obtenerlo con rol
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuario actual: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Verifica si hay una sesión activa
        /// </summary>
        public static bool IsAuthenticated()
        {
            return SupabaseHelper.Client.Auth.CurrentUser != null;
        }

        // ── Helpers internos ────────────────────────────────────────────────

        /// <summary>
        /// Cambia la contraseña del usuario autenticado verificando primero la actual.
        /// </summary>
        public static async Task<(bool Success, string? Error)> CambiarPasswordAsync(
            string email, string passwordActual, string passwordNueva)
        {
            try
            {
                // Verificar contraseña actual re-autenticando
                var session = await SupabaseHelper.Client.Auth.SignIn(email, passwordActual);
                if (session?.User == null)
                    return (false, "La contraseña actual es incorrecta.");

                // Cambiar via Admin API con service role (más confiable que el SDK client)
                var authId = session.User.Id;
                var url        = SupabaseHelper.Url;
                var serviceKey = SupabaseHelper.ServiceRoleKey;

                using var http = new HttpClient();
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", serviceKey);
                http.DefaultRequestHeaders.Add("apikey", serviceKey);

                var body = JsonSerializer.Serialize(new { password = passwordNueva });
                var resp = await http.PutAsync(
                    $"{url}/auth/v1/admin/users/{authId}",
                    new StringContent(body, Encoding.UTF8, "application/json"));

                if (!resp.IsSuccessStatusCode)
                {
                    var err = await resp.Content.ReadAsStringAsync();
                    return (false, $"Error al cambiar contraseña: {err}");
                }

                // Marcar debe_cambiar_password = false en tabla usuarios
                await SupabaseHelper.Client
                    .From<UsuarioSupabase>()
                    .Where(u => u.AuthId == authId)
                    .Set(u => u.DebeCambiarPassword, false)
                    .Update();

                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CambiarPasswordAsync: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Actualiza nombre, apellido, teléfono y foto_perfil del usuario en tabla usuarios.
        /// </summary>
        public static async Task<(bool Success, string? Error)> ActualizarPerfilAsync(
            string authId, string nombre, string apellido, string? telefono, string? fotoPerfil)
        {
            try
            {
                var query = SupabaseHelper.Client
                    .From<UsuarioSupabase>()
                    .Where(u => u.AuthId == authId)
                    .Set(u => u.Nombre, nombre)
                    .Set(u => u.Apellido, apellido)
                    .Set(u => u.Telefono, telefono);

                if (fotoPerfil != null)
                    query = query.Set(u => u.FotoPerfil, fotoPerfil);

                await query.Update();
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarPerfilAsync: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el registro completo del usuario desde la tabla usuarios.
        /// </summary>
        public static async Task<UsuarioSupabase?> ObtenerPerfilCompletoAsync(string authId)
        {
            try
            {
                var res = await SupabaseHelper.Client
                    .From<UsuarioSupabase>()
                    .Where(u => u.AuthId == authId)
                    .Get();
                return res.Models.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerPerfilCompletoAsync: {ex.Message}");
                return null;
            }
        }

        // ── Helpers internos ────────────────────────────────────────────────

        private static async Task<string> ObtenerRolAsync(string authId)
        {
            try
            {
                var resultado = await SupabaseHelper.Client
                    .From<UsuarioSupabase>()
                    .Where(u => u.AuthId == authId)
                    .Get();

                var registro = resultado.Models.FirstOrDefault();
                return registro?.Rol ?? "usuario";
            }
            catch
            {
                return "usuario";
            }
        }
    }
}
