using Supabase;
using System.IO;
using System.Text.Json;

namespace FlujoCajaWpf.Data
{
    /// <summary>
    /// Helper principal para inicializar y gestionar la conexión con Supabase
    /// </summary>
    public static class SupabaseHelper
    {
        private static Client? _client;
        private static string? _supabaseUrl;
        private static string? _supabaseKey;

        public static Client Client
        {
            get
            {
                if (_client == null)
                    throw new InvalidOperationException("Supabase no ha sido inicializado. Llame a InicializarSupabase primero.");
                return _client;
            }
        }

        /// <summary>
        /// Inicializa la conexión con Supabase
        /// </summary>
        public static async Task<bool> InicializarSupabase()
        {
            try
            {
                // Leer configuración de appsettings.json
                var appSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                
                if (!File.Exists(appSettingsPath))
                {
                    Console.WriteLine("Error: appsettings.json no encontrado");
                    return false;
                }

                var json = await File.ReadAllTextAsync(appSettingsPath);
                var config = JsonSerializer.Deserialize<AppSettings>(json);

                if (config?.Supabase == null)
                {
                    Console.WriteLine("Error: Configuración de Supabase no encontrada");
                    return false;
                }

                _supabaseUrl = config.Supabase.Url;
                _supabaseKey = config.Supabase.Key;

                if (string.IsNullOrEmpty(_supabaseUrl) || string.IsNullOrEmpty(_supabaseKey))
                {
                    Console.WriteLine("Error: URL o Key de Supabase vacíos");
                    return false;
                }

                // Configurar opciones de Supabase
                var options = new SupabaseOptions
                {
                    AutoConnectRealtime = true,
                    AutoRefreshToken = true
                };

                _client = new Client(_supabaseUrl, _supabaseKey, options);
                await _client.InitializeAsync();

                Console.WriteLine("✓ Supabase inicializado correctamente");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al inicializar Supabase: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si Supabase está inicializado
        /// </summary>
        public static bool EstaInicializado => _client != null;

        /// <summary>
        /// Actualiza el token de autenticación del cliente
        /// </summary>
        public static void ActualizarToken(string token)
        {
            if (_client != null)
            {
                _client.Auth.SetSession(token, token); // Actualiza la sesión con el token
                Console.WriteLine("✓ Token de autenticación actualizado en el cliente");
            }
        }
    }

    // Clases para deserializar appsettings.json
    public class AppSettings
    {
        public SupabaseConfig? Supabase { get; set; }
        public AzureConfig? Azure { get; set; }
    }

    public class SupabaseConfig
    {
        public string Url { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
    }

    public class AzureConfig
    {
        public DocumentIntelligenceConfig? DocumentIntelligence { get; set; }
    }

    public class DocumentIntelligenceConfig
    {
        public string Endpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }
}
