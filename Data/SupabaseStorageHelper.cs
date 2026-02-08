using Supabase.Storage;
using System.IO;

namespace FlujoCajaWpf.Data
{
    public static class SupabaseStorageHelper
    {
        private const string BUCKET_NAME = "CasasFotos";

        public static async Task<(bool Success, string? Url, string? Error)> SubirImagenCasaAsync(
            byte[] imageBytes, string fileName)
        {
            try
            {
                Console.WriteLine($"[Storage] Iniciando upload de {fileName}");
                var client = SupabaseHelper.Client;
                
                // Verificar autenticación
                var currentUser = client.Auth.CurrentUser;
                var currentSession = client.Auth.CurrentSession;
                
                Console.WriteLine($"[Storage] Usuario actual: {currentUser?.Email ?? "NULL"}");
                Console.WriteLine($"[Storage] Sesión activa: {(currentSession != null ? "SÍ" : "NO")}");
                Console.WriteLine($"[Storage] Token presente: {(!string.IsNullOrEmpty(currentSession?.AccessToken) ? "SÍ" : "NO")}");
                
                // Subir imagen al bucket con opción de sobrescribir
                Console.WriteLine($"[Storage] Cliente obtenido, subiendo {imageBytes.Length} bytes...");
                await client.Storage
                    .From(BUCKET_NAME)
                    .Upload(imageBytes, fileName, new Supabase.Storage.FileOptions 
                    { 
                        Upsert = true  // Permitir sobrescribir si existe
                    });

                Console.WriteLine($"[Storage] Upload exitoso, obteniendo URL pública...");
                
                // Obtener URL pública
                var url = client.Storage
                    .From(BUCKET_NAME)
                    .GetPublicUrl(fileName);

                Console.WriteLine($"[Storage] URL generada: {url}");
                return (true, url, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Storage] ERROR: {ex.Message}");
                Console.WriteLine($"[Storage] Stack: {ex.StackTrace}");
                return (false, null, ex.Message);
            }
        }

        public static async Task<bool> EliminarImagenCasaAsync(string url)
        {
            try
            {
                // Extraer nombre del archivo de la URL
                var uri = new Uri(url);
                var fileName = Path.GetFileName(uri.LocalPath);

                var client = SupabaseHelper.Client;
                
                await client.Storage
                    .From(BUCKET_NAME)
                    .Remove(fileName);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
