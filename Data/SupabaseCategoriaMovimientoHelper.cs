using FlujoDeCajaApp.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlujoDeCajaApp.Data
{
    /// <summary>
    /// Helper class para manejar operaciones CRUD de Categorías de Movimiento en Supabase
    /// Proporciona métodos para crear, leer, actualizar y eliminar categorías de movimiento
    /// </summary>
    public static class SupabaseCategoriaMovimientoHelper
    {
        /// <summary>
        /// Crea una nueva categoría de movimiento en Supabase
        /// </summary>
        /// <param name="categoriaMovimiento">Categoría de movimiento a crear</param>
        /// <returns>True si se creó exitosamente, False en caso contrario</returns>
        public static async Task<bool> CrearCategoriaMovimiento(CategoriaMovimientoSupabase categoriaMovimiento)
        {
            try
            {
                Console.WriteLine($"Creando nueva categoría de movimiento: {categoriaMovimiento.Nombre}");
                var client = await SupabaseHelper.GetClient();
                
                var response = await client
                    .From<CategoriaMovimientoSupabase>()
                    .Insert(categoriaMovimiento);

                Console.WriteLine($"Categoría de movimiento creada exitosamente: {categoriaMovimiento.Nombre}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear categoría de movimiento: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene todas las categorías de movimiento activas
        /// </summary>
        /// <returns>Lista de categorías de movimiento activas</returns>
        public static async Task<List<CategoriaMovimientoSupabase>> ObtenerTodasCategoriasMovimiento()
        {
            try
            {
                Console.WriteLine("Obteniendo todas las categorías de movimiento desde Supabase...");
                var client = await SupabaseHelper.GetClient();
                
                var response = await client
                    .From<CategoriaMovimientoSupabase>()
                    .Where(c => c.Activo == true)
                    .Order(c => c.Nombre, Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                var categorias = response.Models ?? new List<CategoriaMovimientoSupabase>();
                Console.WriteLine($"Se obtuvieron {categorias.Count} categorías de movimiento.");
                return categorias;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener categorías de movimiento: {ex.Message}");
                return new List<CategoriaMovimientoSupabase>();
            }
        }

        /// <summary>
        /// Obtiene una categoría de movimiento por su ID
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <returns>Categoría encontrada o null</returns>
        public static async Task<CategoriaMovimientoSupabase?> ObtenerCategoriaMovimientoPorId(int id)
        {
            try
            {
                Console.WriteLine($"Obteniendo categoría de movimiento con ID: {id}");
                var client = await SupabaseHelper.GetClient();
                
                var response = await client
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
        /// Actualiza una categoría de movimiento existente
        /// </summary>
        /// <param name="categoriaMovimiento">Categoría con los datos actualizados</param>
        /// <returns>True si se actualizó exitosamente</returns>
        public static async Task<bool> ActualizarCategoriaMovimiento(CategoriaMovimientoSupabase categoriaMovimiento)
        {
            try
            {
                Console.WriteLine($"Actualizando categoría de movimiento: {categoriaMovimiento.Nombre}");
                var client = await SupabaseHelper.GetClient();
                
                var response = await client
                    .From<CategoriaMovimientoSupabase>()
                    .Where(c => c.Id == categoriaMovimiento.Id)
                    .Update(categoriaMovimiento);

                Console.WriteLine($"Categoría de movimiento actualizada exitosamente: {categoriaMovimiento.Nombre}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar categoría de movimiento: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina una categoría de movimiento (soft delete - marca como inactivo)
        /// </summary>
        /// <param name="id">ID de la categoría a eliminar</param>
        /// <returns>True si se eliminó exitosamente</returns>
        public static async Task<bool> EliminarCategoriaMovimiento(int id)
        {
            try
            {
                Console.WriteLine($"Eliminando categoría de movimiento con ID: {id}");
                var client = await SupabaseHelper.GetClient();
                
                // Soft delete: marcar como inactivo en lugar de eliminar físicamente
                var categoria = await ObtenerCategoriaMovimientoPorId(id);
                if (categoria == null)
                {
                    Console.WriteLine("Categoría de movimiento no encontrada.");
                    return false;
                }

                categoria.Activo = false;
                
                var response = await client
                    .From<CategoriaMovimientoSupabase>()
                    .Where(c => c.Id == id)
                    .Update(categoria);

                Console.WriteLine($"Categoría de movimiento eliminada exitosamente: {categoria.Nombre}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar categoría de movimiento: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si una categoría de movimiento con el nombre dado ya existe
        /// </summary>
        /// <param name="nombre">Nombre a verificar</param>
        /// <param name="idExcluir">ID a excluir de la verificación (para updates)</param>
        /// <returns>True si ya existe</returns>
        public static async Task<bool> ExisteNombre(string nombre, int idExcluir = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return false;

                var client = await SupabaseHelper.GetClient();
                
                var response = await client
                    .From<CategoriaMovimientoSupabase>()
                    .Where(c => c.Nombre.ToLower() == nombre.ToLower() && c.Activo == true)
                    .Get();

                var categorias = response.Models ?? new List<CategoriaMovimientoSupabase>();
                
                if (idExcluir > 0)
                {
                    categorias = categorias.Where(c => c.Id != idExcluir).ToList();
                }

                return categorias.Any();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar nombre: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene el total de categorías de movimiento activas
        /// </summary>
        /// <returns>Número de categorías activas</returns>
        public static async Task<int> ObtenerTotalCategoriasMovimiento()
        {
            try
            {
                var categorias = await ObtenerTodasCategoriasMovimiento();
                return categorias.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener total de categorías de movimiento: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Busca categorías de movimiento por nombre (búsqueda parcial)
        /// </summary>
        /// <param name="termino">Término de búsqueda</param>
        /// <returns>Lista de categorías que coinciden con el término</returns>
        public static async Task<List<CategoriaMovimientoSupabase>> BuscarCategoriasMovimientoPorNombre(string termino)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termino))
                    return await ObtenerTodasCategoriasMovimiento();

                var todasLasCategorias = await ObtenerTodasCategoriasMovimiento();
                return todasLasCategorias
                    .Where(c => c.Nombre.ToLower().Contains(termino.ToLower()))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar categorías de movimiento: {ex.Message}");
                return new List<CategoriaMovimientoSupabase>();
            }
        }
    }
}