using Supabase.Storage;
using System.IO;

namespace FlujoCajaWpf.Data
{
    public static class SupabaseStorageHelper
    {
        private const string BUCKET_NAME = "CasasFotos";

        // Bucket privado para imágenes de facturas/comprobantes de movimientos
        private const string BUCKET_FACTURAS = "facturas";

        // Bucket público para avatares de usuario
        private const string BUCKET_AVATARES = "avatares";

        /// <summary>
        /// Sube la foto de perfil del usuario al bucket público "avatares".
        /// fileName debe incluir la extensión, ej: "avatares/uuid.jpg"
        /// </summary>
        public static async Task<(bool Success, string? Url, string? Error)> SubirAvatarAsync(
            byte[] imageBytes, string fileName)
        {
            try
            {
                var client = SupabaseHelper.Client;
                await client.Storage
                    .From(BUCKET_AVATARES)
                    .Upload(imageBytes, fileName, new Supabase.Storage.FileOptions { Upsert = true });

                var url = client.Storage.From(BUCKET_AVATARES).GetPublicUrl(fileName);
                return (true, url, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Storage] Error subiendo avatar: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

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

        /// <summary>
        /// Sube una imagen adjunta a un movimiento al bucket PRIVADO "facturas".
        /// Retorna el storage path (p.ej. "{casaId}/{fileName}"), NO una URL pública.
        /// Ese path se guarda en imagen_url del movimiento.
        /// </summary>
        public static async Task<(bool Success, string? Path, string? Error)> SubirImagenMovimientoAsync(
            byte[] imageBytes, string fileName, int casaId)
        {
            try
            {
                var client = SupabaseHelper.Client;
                var storagePath = $"{casaId}/{fileName}";

                await client.Storage
                    .From(BUCKET_FACTURAS)
                    .Upload(imageBytes, storagePath, new Supabase.Storage.FileOptions { Upsert = true });

                return (true, storagePath, null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Genera una URL firmada (temporal, ~1 hora) para una imagen privada de movimiento.
        /// storagePath: valor guardado en imagen_url (p.ej. "5/abc123.jpg").
        /// Retorna null si falla.
        /// </summary>
        public static async Task<string?> ObtenerUrlFirmadaMovimientoAsync(string storagePath, int expiresInSeconds = 3600)
        {
            try
            {
                var client = SupabaseHelper.Client;
                var signedUrl = await client.Storage
                    .From(BUCKET_FACTURAS)
                    .CreateSignedUrl(storagePath, expiresInSeconds);

                return signedUrl;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Descarga los bytes de una imagen de movimiento usando el path o URL guardado en imagen_url.
        /// Maneja tanto paths privados nuevos como URLs públicas legacy.
        /// </summary>
        public static async Task<byte[]?> DescargarImagenMovimientoAsync(string imagenUrl)
        {
            try
            {
                string urlToFetch;

                if (imagenUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    imagenUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    // URL pública legacy: intentar descargar directamente
                    urlToFetch = imagenUrl;
                }
                else
                {
                    // Path privado: generar signed URL
                    var signed = await ObtenerUrlFirmadaMovimientoAsync(imagenUrl);
                    if (signed == null) return null;
                    urlToFetch = signed;
                }

                using var http = new System.Net.Http.HttpClient();
                return await http.GetByteArrayAsync(urlToFetch);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Elimina una imagen de movimiento del Storage.
        /// Acepta tanto paths privados ("5/abc.jpg") como URLs públicas legacy.
        /// </summary>
        public static async Task<bool> EliminarImagenMovimientoAsync(string imagenUrl)
        {
            try
            {
                var client = SupabaseHelper.Client;

                if (imagenUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    imagenUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    // Formato legacy: URL pública → intentar eliminar del bucket original
                    var marker = $"/public/{BUCKET_NAME}/";
                    var idx = imagenUrl.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                    if (idx < 0) return false;

                    var storagePath = imagenUrl[(idx + marker.Length)..];

                    await client.Storage
                        .From(BUCKET_NAME)
                        .Remove(storagePath);
                }
                else
                {
                    // Nuevo formato: path privado en bucket facturas
                    await client.Storage
                        .From(BUCKET_FACTURAS)
                        .Remove(imagenUrl);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sube un reporte (PDF o Excel) al bucket "reportes".
        /// Path: reportes/{casaNombre}/{fecha}_{nombreArchivo}
        /// Retorna la URL pública o error.
        /// </summary>
        public static async Task<(bool Success, string? Url, string? Error)> SubirReporteAsync(
            string rutaArchivo,
            string casaNombre,
            string nombreArchivo)
        {
            try
            {
                var bytes = await File.ReadAllBytesAsync(rutaArchivo);
                var client = SupabaseHelper.Client;

                // Sanitizar nombre de la casa para usarlo como carpeta
                var carpeta = string.Join("_", casaNombre.Split(Path.GetInvalidFileNameChars()));
                var fecha = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
                var storagePath = $"{carpeta}/{fecha}_{nombreArchivo}";

                await client.Storage
                    .From(BUCKET_REPORTES)
                    .Upload(bytes, storagePath, new Supabase.Storage.FileOptions { Upsert = false });

                var url = client.Storage
                    .From(BUCKET_REPORTES)
                    .GetPublicUrl(storagePath);

                return (true, url, null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        private const string BUCKET_REPORTES = "reportes";
    }
}
