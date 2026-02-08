using FlujoCajaWpf.Models;

namespace FlujoCajaWpf.Data
{
    /// <summary>
    /// Helper para operaciones CRUD de categorías de propiedades en Supabase
    /// </summary>
    public static class SupabaseCategoriaHelper
    {
        /// <summary>
        /// Obtiene todas las categorías
        /// </summary>
        public static async Task<List<CategoriaSupabase>> ObtenerTodasCategoriasAsync()
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<CategoriaSupabase>()
                    .Order("nombre", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                if (response.Models == null || !response.Models.Any())
                {
                    Console.WriteLine("No se encontraron categorías");
                    return new List<CategoriaSupabase>();
                }

                Console.WriteLine($"✓ {response.Models.Count} categorías cargadas");
                return response.Models.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener categorías: {ex.Message}");
                return new List<CategoriaSupabase>();
            }
        }

        /// <summary>
        /// Obtiene una categoría por ID
        /// </summary>
        public static async Task<CategoriaSupabase?> ObtenerCategoriaPorIdAsync(int id)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<CategoriaSupabase>()
                    .Where(c => c.Id == id)
                    .Single();

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener categoría: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Inserta una nueva categoría
        /// </summary>
        public static async Task<(bool Success, int? Id, string? Error)> InsertarCategoriaAsync(CategoriaSupabase categoria)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<CategoriaSupabase>()
                    .Insert(categoria);

                if (response.Models == null || !response.Models.Any())
                {
                    return (false, null, "No se pudo insertar la categoría");
                }

                var nuevaCategoria = response.Models.First();
                Console.WriteLine($"✓ Categoría creada: {nuevaCategoria.Nombre}");
                return (true, nuevaCategoria.Id, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar categoría: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Actualiza una categoría existente
        /// </summary>
        public static async Task<(bool Success, string? Error)> ActualizarCategoriaAsync(CategoriaSupabase categoria)
        {
            try
            {
                await SupabaseHelper.Client
                    .From<CategoriaSupabase>()
                    .Where(c => c.Id == categoria.Id)
                    .Update(categoria);

                Console.WriteLine($"✓ Categoría actualizada: {categoria.Nombre}");
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar categoría: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Elimina una categoría permanentemente
        /// </summary>
        public static async Task<(bool Success, string? Error)> EliminarCategoriaAsync(int id)
        {
            try
            {
                await SupabaseHelper.Client
                    .From<CategoriaSupabase>()
                    .Where(c => c.Id == id)
                    .Delete();

                Console.WriteLine($"✓ Categoría eliminada");
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar categoría: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene todas las categorías con manejo de errores estructurado
        /// </summary>
        public static async Task<(bool Success, List<CategoriaSupabase>? Data, string? Error)> ObtenerCategoriasAsync()
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<CategoriaSupabase>()
                    .Order("nombre", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                var categorias = response.Models?.ToList() ?? new List<CategoriaSupabase>();
                return (true, categorias, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener categorías: {ex.Message}");
                return (false, null, ex.Message);
            }
        }
    }
}
