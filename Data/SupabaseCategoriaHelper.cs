using FlujoDeCajaApp.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlujoDeCajaApp.Data
{
    /// <summary>
    /// Helper class para manejar operaciones CRUD de Categorías en Supabase
    /// Proporciona métodos para crear, leer, actualizar y eliminar categorías
    /// </summary>
    public static class SupabaseCategoriaHelper
    {
        /// <summary>
        /// Obtiene todas las categorías activas
        /// </summary>
        /// <returns>Lista de categorías activas</returns>
        public static async Task<List<CategoriaSupabase>> ObtenerTodasCategorias()
        {
            try
            {
                Console.WriteLine("Obteniendo todas las categorías desde Supabase...");
                var client = await SupabaseHelper.GetClient();
                
                var response = await client
                    .From<CategoriaSupabase>()
                    .Where(c => c.Activo == true)
                    .Order(c => c.Nombre, Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                var categorias = response.Models ?? new List<CategoriaSupabase>();
                Console.WriteLine($"Se obtuvieron {categorias.Count} categorías.");
                return categorias;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener categorías: {ex.Message}");
                return new List<CategoriaSupabase>();
            }
        }

        /// <summary>
        /// Obtiene una categoría por su ID
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <returns>Categoría encontrada o null</returns>
        public static async Task<CategoriaSupabase?> ObtenerCategoriaPorId(int id)
        {
            try
            {
                Console.WriteLine($"Obteniendo categoría con ID: {id}");
                var client = await SupabaseHelper.GetClient();
                
                var response = await client
                    .From<CategoriaSupabase>()
                    .Where(c => c.Id == id)
                    .Single();

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener categoría por ID {id}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Busca categorías por nombre
        /// </summary>
        /// <param name="termino">Término de búsqueda</param>
        /// <returns>Lista de categorías que coinciden</returns>
        public static async Task<List<CategoriaSupabase>> BuscarCategorias(string termino)
        {
            try
            {
                Console.WriteLine($"Buscando categorías con término: {termino}");
                var client = await SupabaseHelper.GetClient();
                
                // Obtener todas las activas y filtrar en código
                var response = await client
                    .From<CategoriaSupabase>()
                    .Where(c => c.Activo == true)
                    .Order(c => c.Nombre, Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                var categorias = response.Models ?? new List<CategoriaSupabase>();
                
                // Filtrar en código para buscar en nombre o descripción
                var categoriasFiltradas = categorias
                    .Where(c => 
                        c.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase) || 
                        (!string.IsNullOrEmpty(c.Descripcion) && c.Descripcion.Contains(termino, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
                
                Console.WriteLine($"Se encontraron {categoriasFiltradas.Count} categorías que coinciden.");
                return categoriasFiltradas;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar categorías: {ex.Message}");
                return new List<CategoriaSupabase>();
            }
        }

        /// <summary>
        /// Crea una nueva categoría
        /// </summary>
        /// <param name="categoria">Datos de la categoría a crear</param>
        /// <returns>True si se creó exitosamente</returns>
        public static async Task<bool> CrearCategoria(CategoriaSupabase categoria)
        {
            try
            {
                if (!categoria.EsValido())
                {
                    Console.WriteLine("Los datos de la categoría no son válidos.");
                    return false;
                }

                Console.WriteLine($"Creando nueva categoría: {categoria.Nombre}");
                Console.WriteLine($"Datos a enviar - Nombre: {categoria.Nombre}, Descripción: {categoria.Descripcion ?? "null"}");
                
                var client = await SupabaseHelper.GetClient();
                
                // Asegurar que la fecha esté en UTC
                categoria.FechaCreacion = DateTime.UtcNow;
                
                var response = await client
                    .From<CategoriaSupabase>()
                    .Insert(categoria);

                Console.WriteLine($"Categoría creada exitosamente: {categoria.Nombre}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear categoría: {ex.Message}");
                Console.WriteLine($"Detalles del error: {ex}");
                return false;
            }
        }

        /// <summary>
        /// Actualiza los datos de una categoría existente
        /// </summary>
        /// <param name="categoria">Categoría con datos actualizados</param>
        /// <returns>True si se actualizó exitosamente</returns>
        public static async Task<bool> ActualizarCategoria(CategoriaSupabase categoria)
        {
            try
            {
                if (!categoria.EsValido())
                {
                    Console.WriteLine("Los datos de la categoría no son válidos.");
                    return false;
                }

                Console.WriteLine($"Actualizando categoría: {categoria.Nombre}");
                var client = await SupabaseHelper.GetClient();
                
                var response = await client
                    .From<CategoriaSupabase>()
                    .Where(c => c.Id == categoria.Id)
                    .Update(categoria);

                Console.WriteLine($"Categoría actualizada exitosamente: {categoria.Nombre}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar categoría: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina una categoría (soft delete - marca como inactivo)
        /// </summary>
        /// <param name="id">ID de la categoría a eliminar</param>
        /// <returns>True si se eliminó exitosamente</returns>
        public static async Task<bool> EliminarCategoria(int id)
        {
            try
            {
                Console.WriteLine($"Eliminando categoría con ID: {id}");
                var client = await SupabaseHelper.GetClient();
                
                // Soft delete: marcar como inactivo en lugar de eliminar físicamente
                var categoria = await ObtenerCategoriaPorId(id);
                if (categoria == null)
                {
                    Console.WriteLine("Categoría no encontrada.");
                    return false;
                }

                categoria.Activo = false;
                
                var response = await client
                    .From<CategoriaSupabase>()
                    .Where(c => c.Id == id)
                    .Update(categoria);

                Console.WriteLine($"Categoría eliminada exitosamente: {categoria.Nombre}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar categoría: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si un nombre de categoría ya está en uso
        /// </summary>
        /// <param name="nombre">Nombre a verificar</param>
        /// <param name="idExcluir">ID a excluir de la verificación (para updates)</param>
        /// <returns>True si el nombre ya existe</returns>
        public static async Task<bool> ExisteNombre(string nombre, int idExcluir = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return false;

                var client = await SupabaseHelper.GetClient();
                
                var response = await client
                    .From<CategoriaSupabase>()
                    .Where(c => c.Nombre.ToLower() == nombre.ToLower() && c.Activo == true)
                    .Get();

                var categorias = response.Models ?? new List<CategoriaSupabase>();
                
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
        /// Obtiene el total de categorías activas
        /// </summary>
        /// <returns>Número de categorías activas</returns>
        public static async Task<int> ObtenerTotalCategorias()
        {
            try
            {
                var categorias = await ObtenerTodasCategorias();
                return categorias.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener total de categorías: {ex.Message}");
                return 0;
            }
        }
    }
}