using Supabase;
using Supabase.Gotrue;
using System;
using System.Threading.Tasks;

namespace FlujoDeCajaApp.Data
{
    /// <summary>
    /// Helper class para manejar la conexión y operaciones con Supabase
    /// Proporciona funcionalidades de autenticación y base de datos
    /// </summary>
    public static class SupabaseHelper
    {
        // Configuración de Supabase
        private static readonly string SupabaseUrl = "https://txytwyrujgdnfbrrjgvz.supabase.co";
        private static readonly string SupabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InR4eXR3eXJ1amdkbmZicnJqZ3Z6Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjA0NjUwOTUsImV4cCI6MjA3NjA0MTA5NX0.9_32RlveUG052QwOM_5ZZzS9wAkE44rjk07C-liV6s4";
        
        private static Supabase.Client? _supabase;

        /// <summary>
        /// Obtiene la instancia del cliente Supabase (Singleton)
        /// </summary>
        public static async Task<Supabase.Client> GetClient()
        {
            if (_supabase == null)
            {
                var options = new SupabaseOptions
                {
                    AutoConnectRealtime = true
                };

                _supabase = new Supabase.Client(SupabaseUrl, SupabaseKey, options);
                await _supabase.InitializeAsync();
            }

            return _supabase;
        }

        /// <summary>
        /// Inicializa la conexión con Supabase
        /// </summary>
        public static async Task<bool> InicializarSupabase()
        {
            try
            {
                Console.WriteLine("Inicializando conexión con Supabase...");
                var client = await GetClient();
                Console.WriteLine("Conexión con Supabase establecida correctamente.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al inicializar Supabase: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Valida las credenciales del usuario usando Supabase Auth
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <param name="password">Contraseña del usuario</param>
        /// <returns>True si las credenciales son válidas</returns>
        public static async Task<bool> ValidarCredenciales(string email, string password)
        {
            try
            {
                Console.WriteLine($"Intentando autenticar usuario: {email}");
                var client = await GetClient();
                
                // Intentar iniciar sesión
                var session = await client.Auth.SignIn(email, password);
                
                if (session?.User != null)
                {
                    Console.WriteLine($"Usuario autenticado correctamente: {session.User.Email}");
                    return true;
                }
                
                Console.WriteLine("Credenciales inválidas.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error durante la autenticación: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Registra un nuevo usuario en Supabase
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <param name="password">Contraseña del usuario</param>
        /// <returns>True si el registro fue exitoso</returns>
        public static async Task<bool> RegistrarUsuario(string email, string password)
        {
            try
            {
                Console.WriteLine($"Registrando nuevo usuario: {email}");
                var client = await GetClient();
                
                var session = await client.Auth.SignUp(email, password);
                
                if (session?.User != null)
                {
                    Console.WriteLine($"Usuario registrado correctamente: {session.User.Email}");
                    return true;
                }
                
                Console.WriteLine("Error al registrar usuario.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error durante el registro: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene información del usuario actualmente autenticado
        /// </summary>
        /// <returns>Usuario actual o null si no hay sesión activa</returns>
        public static async Task<User?> ObtenerUsuarioActual()
        {
            try
            {
                var client = await GetClient();
                return client.Auth.CurrentUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuario actual: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Cierra la sesión del usuario actual
        /// </summary>
        public static async Task<bool> CerrarSesion()
        {
            try
            {
                var client = await GetClient();
                await client.Auth.SignOut();
                Console.WriteLine("Sesión cerrada correctamente.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cerrar sesión: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si hay una sesión activa
        /// </summary>
        /// <returns>True si hay una sesión activa</returns>
        public static async Task<bool> TieneSesionActiva()
        {
            try
            {
                var client = await GetClient();
                return client.Auth.CurrentSession != null;
            }
            catch
            {
                return false;
            }
        }
    }
}