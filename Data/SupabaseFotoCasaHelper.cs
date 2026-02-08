using FlujoCajaWpf.Models;

namespace FlujoCajaWpf.Data
{
    /// <summary>
    /// Helper para operaciones CRUD de fotos de casas en Supabase
    /// </summary>
    public static class SupabaseFotoCasaHelper
    {
        /// <summary>
        /// Obtiene todas las fotos de una casa específica
        /// </summary>
        public static async Task<List<FotoCasaSupabase>> ObtenerFotosPorCasaAsync(int casaId)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<FotoCasaSupabase>()
                    .Where(f => f.CasaId == casaId)
                    .Order("fechacreacion", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                if (response.Models == null || !response.Models.Any())
                {
                    Console.WriteLine($"No se encontraron fotos para la casa {casaId}");
                    return new List<FotoCasaSupabase>();
                }

                Console.WriteLine($"✓ {response.Models.Count} fotos cargadas para casa {casaId}");
                return response.Models.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener fotos: {ex.Message}");
                return new List<FotoCasaSupabase>();
            }
        }

        /// <summary>
        /// Inserta un nuevo registro de foto
        /// </summary>
        public static async Task<(bool Success, int? Id, string? Error)> InsertarFotoAsync(FotoCasaSupabase foto)
        {
            try
            {
                // Obtener el siguiente ID disponible manualmente
                var todasLasFotos = await SupabaseHelper.Client
                    .From<FotoCasaSupabase>()
                    .Select("id")
                    .Order("id", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Limit(1)
                    .Get();

                int siguienteId = 1; // Por defecto si no hay fotos
                if (todasLasFotos.Models != null && todasLasFotos.Models.Any())
                {
                    siguienteId = todasLasFotos.Models.First().Id + 1;
                }

                Console.WriteLine($"📝 Siguiente ID a usar: {siguienteId}");

                // Crear la foto con el ID específico
                var nuevaFoto = new FotoCasaSupabase
                {
                    Id = siguienteId,
                    CasaId = foto.CasaId,
                    Url = foto.Url,
                    NombreArchivo = foto.NombreArchivo,
                    FechaCreacion = foto.FechaCreacion
                };

                var response = await SupabaseHelper.Client
                    .From<FotoCasaSupabase>()
                    .Insert(nuevaFoto);

                if (response.Models == null || !response.Models.Any())
                {
                    return (false, null, "No se pudo insertar la foto");
                }

                var fotoInsertada = response.Models.First();
                Console.WriteLine($"✓ Foto registrada con ID: {fotoInsertada.Id}");
                return (true, fotoInsertada.Id, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar foto: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Elimina una foto de la base de datos
        /// </summary>
        public static async Task<(bool Success, string? Error)> EliminarFotoAsync(int fotoId)
        {
            try
            {
                await SupabaseHelper.Client
                    .From<FotoCasaSupabase>()
                    .Where(f => f.Id == fotoId)
                    .Delete();

                Console.WriteLine($"✓ Foto {fotoId} eliminada");
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar foto: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una foto específica por ID
        /// </summary>
        public static async Task<FotoCasaSupabase?> ObtenerFotoPorIdAsync(int fotoId)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<FotoCasaSupabase>()
                    .Where(f => f.Id == fotoId)
                    .Single();

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener foto: {ex.Message}");
                return null;
            }
        }
    }
}
