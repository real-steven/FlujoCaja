using FlujoCajaWpf.Models;

namespace FlujoCajaWpf.Data
{
    /// <summary>
    /// Helper para operaciones CRUD de categorías de movimientos en Supabase
    /// </summary>
    public static class SupabaseCategoriaMovimientoHelper
    {
        /// <summary>
        /// Obtiene todas las categorías de movimientos
        /// </summary>
        public static async Task<List<CategoriaMovimientoSupabase>> ObtenerTodasCategoriasMovimientosAsync()
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<CategoriaMovimientoSupabase>()
                    .Where(c => c.Activo == true)
                    .Order("nombre", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                if (response.Models == null || !response.Models.Any())
                {
                    Console.WriteLine("No se encontraron categorías de movimientos");
                    return new List<CategoriaMovimientoSupabase>();
                }

                Console.WriteLine($"✓ {response.Models.Count} categorías de movimientos cargadas");
                return response.Models.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener categorías de movimientos: {ex.Message}");
                return new List<CategoriaMovimientoSupabase>();
            }
        }

        /// <summary>
        /// Obtiene categorías de movimientos por tipo
        /// </summary>
        public static async Task<List<CategoriaMovimientoSupabase>> ObtenerCategoriasPorTipoAsync(string tipo)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<CategoriaMovimientoSupabase>()
                    .Where(c => c.Activo == true)
                    .Where(c => c.Tipo == tipo)
                    .Order("nombre", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                if (response.Models == null || !response.Models.Any())
                {
                    return new List<CategoriaMovimientoSupabase>();
                }

                return response.Models.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener categorías por tipo: {ex.Message}");
                return new List<CategoriaMovimientoSupabase>();
            }
        }

        /// <summary>
        /// Obtiene una categoría de movimiento por ID
        /// </summary>
        public static async Task<CategoriaMovimientoSupabase?> ObtenerCategoriaPorIdAsync(int id)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<CategoriaMovimientoSupabase>()
                    .Where(c => c.Id == id)
                    .Single();

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener categoría de movimiento: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Inserta una nueva categoría de movimiento
        /// </summary>
        public static async Task<(bool Success, int? Id, string? Error)> InsertarCategoriaMovimientoAsync(CategoriaMovimientoSupabase categoria)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<CategoriaMovimientoSupabase>()
                    .Insert(categoria);

                if (response.Models == null || !response.Models.Any())
                {
                    return (false, null, "No se pudo insertar la categoría de movimiento");
                }

                var nuevaCategoria = response.Models.First();
                Console.WriteLine($"✓ Categoría de movimiento creada: {nuevaCategoria.Nombre}");
                return (true, nuevaCategoria.Id, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar categoría de movimiento: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Actualiza una categoría de movimiento existente
        /// </summary>
        public static async Task<(bool Success, string? Error)> ActualizarCategoriaMovimientoAsync(CategoriaMovimientoSupabase categoria)
        {
            try
            {
                await SupabaseHelper.Client
                    .From<CategoriaMovimientoSupabase>()
                    .Where(c => c.Id == categoria.Id)
                    .Update(categoria);

                Console.WriteLine($"✓ Categoría de movimiento actualizada: {categoria.Nombre}");
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar categoría de movimiento: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Elimina una categoría de movimiento permanentemente
        /// </summary>
        public static async Task<(bool Success, string? Error)> EliminarCategoriaMovimientoAsync(int id)
        {
            try
            {
                await SupabaseHelper.Client
                    .From<CategoriaMovimientoSupabase>()
                    .Where(c => c.Id == id)
                    .Delete();

                Console.WriteLine($"✓ Categoría de movimiento eliminada");
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar categoría de movimiento: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene todas las categorías de movimientos con manejo de errores estructurado
        /// </summary>
        public static async Task<(bool Success, List<CategoriaMovimientoSupabase>? Data, string? Error)> ObtenerCategoriasMovimientosAsync()
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<CategoriaMovimientoSupabase>()
                    .Order("nombre", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                var categorias = response.Models?.ToList() ?? new List<CategoriaMovimientoSupabase>();
                return (true, categorias, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener categorías de movimientos: {ex.Message}");
                return (false, null, ex.Message);
            }
        }
    }
}
