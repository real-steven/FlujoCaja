using FlujoCajaWpf.Models;

namespace FlujoCajaWpf.Data
{
    /// <summary>
    /// Helper para operaciones CRUD de dueños en Supabase
    /// </summary>
    public static class SupabaseDuenoHelper
    {
        /// <summary>
        /// Obtiene todos los dueños
        /// </summary>
        public static async Task<List<DuenoSupabase>> ObtenerTodosDuenosAsync()
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<DuenoSupabase>()
                    .Order("apellido", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                if (response.Models == null || !response.Models.Any())
                {
                    Console.WriteLine("No se encontraron dueños");
                    return new List<DuenoSupabase>();
                }

                Console.WriteLine($"✓ {response.Models.Count} dueños cargados");
                return response.Models.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener dueños: {ex.Message}");
                return new List<DuenoSupabase>();
            }
        }

        /// <summary>
        /// Obtiene todos los dueños con manejo de errores estructurado
        /// </summary>
        public static async Task<(bool Success, List<DuenoSupabase>? Data, string? Error)> ObtenerDuenosAsync()
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<DuenoSupabase>()
                    .Order("apellido", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                var duenos = response.Models?.ToList() ?? new List<DuenoSupabase>();
                Console.WriteLine($"✓ {duenos.Count} dueños cargados");
                return (true, duenos, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener dueños: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene un dueño por ID
        /// </summary>
        public static async Task<DuenoSupabase?> ObtenerDuenoPorIdAsync(int id)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<DuenoSupabase>()
                    .Where(d => d.Id == id)
                    .Single();

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener dueño: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Inserta un nuevo dueño
        /// </summary>
        public static async Task<(bool Success, int? Id, string? Error)> InsertarDuenoAsync(DuenoSupabase dueno)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<DuenoSupabase>()
                    .Insert(dueno);

                if (response.Models == null || !response.Models.Any())
                {
                    return (false, null, "No se pudo insertar el dueño");
                }

                var nuevoDueno = response.Models.First();
                Console.WriteLine($"✓ Dueño creado: {nuevoDueno.Nombre} {nuevoDueno.Apellido}");
                return (true, nuevoDueno.Id, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar dueño: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Actualiza un dueño existente
        /// </summary>
        public static async Task<(bool Success, string? Error)> ActualizarDuenoAsync(DuenoSupabase dueno)
        {
            try
            {
                await SupabaseHelper.Client
                    .From<DuenoSupabase>()
                    .Where(d => d.Id == dueno.Id)
                    .Update(dueno);

                Console.WriteLine($"✓ Dueño actualizado: {dueno.Nombre} {dueno.Apellido}");
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar dueño: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Elimina un dueño permanentemente
        /// </summary>
        public static async Task<(bool Success, string? Error)> EliminarDuenoAsync(int id)
        {
            try
            {
                await SupabaseHelper.Client
                    .From<DuenoSupabase>()
                    .Where(d => d.Id == id)
                    .Delete();

                Console.WriteLine($"✓ Dueño eliminado");
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar dueño: {ex.Message}");
                return (false, ex.Message);
            }
        }
    }
}
