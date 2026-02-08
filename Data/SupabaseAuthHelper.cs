using FlujoCajaWpf.Models;
using Supabase.Gotrue;

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

                // Verificar que tenemos un access token
                if (string.IsNullOrEmpty(session.AccessToken))
                {
                    return (false, null, "No se recibió token de acceso");
                }

                // Actualizar el token en el cliente (importante para Storage)
                Console.WriteLine($"📝 Token obtenido: {session.AccessToken.Substring(0, 20)}...");
                
                // Crear objeto Usuario
                var usuario = new Usuario
                {
                    Id = session.User.Id,
                    Email = session.User.Email ?? email,
                    CreatedAt = session.User.CreatedAt
                };

                Console.WriteLine($"✓ Login exitoso: {usuario.Email}");
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
                
                if (user == null)
                    return null;

                return new Usuario
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    CreatedAt = user.CreatedAt
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
    }
}
