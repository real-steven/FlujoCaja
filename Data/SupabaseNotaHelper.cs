using FlujoCajaWpf.Models;

namespace FlujoCajaWpf.Data
{
    /// <summary>
    /// Helper para operaciones CRUD de notas de casas en Supabase
    /// </summary>
    public static class SupabaseNotaHelper
    {
        /// <summary>
        /// Obtiene todas las notas de una casa específica
        /// </summary>
        public static async Task<List<NotaSupabase>> ObtenerNotasPorCasaAsync(int casaId)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<NotaSupabase>()
                    .Where(n => n.CasaId == casaId)
                    .Order("fechacreacion", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                if (response.Models == null || !response.Models.Any())
                {
                    Console.WriteLine($"No se encontraron notas para la casa {casaId}");
                    return new List<NotaSupabase>();
                }

                Console.WriteLine($"✓ {response.Models.Count} notas cargadas para casa {casaId}");
                return response.Models.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener notas: {ex.Message}");
                return new List<NotaSupabase>();
            }
        }

        /// <summary>
        /// Inserta una nueva nota
        /// </summary>
        public static async Task<(bool Success, int? Id, string? Error)> InsertarNotaAsync(NotaSupabase nota)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<NotaSupabase>()
                    .Insert(nota);

                if (response.Models == null || !response.Models.Any())
                {
                    return (false, null, "No se pudo insertar la nota");
                }

                var nuevaNota = response.Models.First();
                Console.WriteLine($"✓ Nota creada con ID: {nuevaNota.Id}");
                return (true, nuevaNota.Id, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar nota: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Actualiza una nota existente
        /// </summary>
        public static async Task<(bool Success, string? Error)> ActualizarNotaAsync(NotaSupabase nota)
        {
            try
            {
                await SupabaseHelper.Client
                    .From<NotaSupabase>()
                    .Where(n => n.Id == nota.Id)
                    .Update(nota);

                Console.WriteLine($"✓ Nota {nota.Id} actualizada");
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar nota: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Elimina una nota
        /// </summary>
        public static async Task<(bool Success, string? Error)> EliminarNotaAsync(int notaId)
        {
            try
            {
                await SupabaseHelper.Client
                    .From<NotaSupabase>()
                    .Where(n => n.Id == notaId)
                    .Delete();

                Console.WriteLine($"✓ Nota {notaId} eliminada");
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar nota: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una nota específica por ID
        /// </summary>
        public static async Task<NotaSupabase?> ObtenerNotaPorIdAsync(int notaId)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<NotaSupabase>()
                    .Where(n => n.Id == notaId)
                    .Single();

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener nota: {ex.Message}");
                return null;
            }
        }
    }
}
