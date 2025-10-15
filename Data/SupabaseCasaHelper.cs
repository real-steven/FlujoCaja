using FlujoDeCajaApp.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlujoDeCajaApp.Data
{
    /// <summary>
    /// Helper class para manejar operaciones CRUD de Casas en Supabase
    /// Proporciona métodos para crear, leer, actualizar y eliminar casas
    /// </summary>
    public static class SupabaseCasaHelper
    {
        /// <summary>
        /// Obtiene todas las casas activas
        /// </summary>
        /// <returns>Lista de casas activas</returns>
        public static async Task<List<CasaSupabase>> ObtenerTodasCasas()
        {
            try
            {
                Console.WriteLine("Obteniendo todas las casas desde Supabase...");
                var client = await SupabaseHelper.GetClient();
                
                var response = await client
                    .From<CasaSupabase>()
                    .Where(c => c.Activo == true)
                    .Order(c => c.Nombre, Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                var casas = response.Models ?? new List<CasaSupabase>();
                Console.WriteLine($"Se obtuvieron {casas.Count} casas.");
                return casas;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener casas: {ex.Message}");
                return new List<CasaSupabase>();
            }
        }

        /// <summary>
        /// Obtiene una casa por su ID
        /// </summary>
        /// <param name="id">ID de la casa</param>
        /// <returns>Casa encontrada o null</returns>
        public static async Task<CasaSupabase?> ObtenerCasaPorId(int id)
        {
            try
            {
                Console.WriteLine($"Obteniendo casa con ID: {id}");
                var client = await SupabaseHelper.GetClient();
                
                var response = await client
                    .From<CasaSupabase>()
                    .Where(c => c.Id == id)
                    .Single();

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener casa por ID {id}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene casas por dueño
        /// </summary>
        /// <param name="duenoId">ID del dueño</param>
        /// <returns>Lista de casas del dueño</returns>
        public static async Task<List<CasaSupabase>> ObtenerCasasPorDueno(int duenoId)
        {
            try
            {
                Console.WriteLine($"Obteniendo casas del dueño ID: {duenoId}");
                var client = await SupabaseHelper.GetClient();
                
                var response = await client
                    .From<CasaSupabase>()
                    .Where(c => c.DuenoId == duenoId && c.Activo == true)
                    .Order(c => c.Nombre, Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                var casas = response.Models ?? new List<CasaSupabase>();
                Console.WriteLine($"Se encontraron {casas.Count} casas para el dueño.");
                return casas;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener casas por dueño: {ex.Message}");
                return new List<CasaSupabase>();
            }
        }

        /// <summary>
        /// Obtiene casas por categoría
        /// </summary>
        /// <param name="categoriaId">ID de la categoría</param>
        /// <returns>Lista de casas de la categoría</returns>
        public static async Task<List<CasaSupabase>> ObtenerCasasPorCategoria(int categoriaId)
        {
            try
            {
                Console.WriteLine($"Obteniendo casas de categoría ID: {categoriaId}");
                var client = await SupabaseHelper.GetClient();
                
                var response = await client
                    .From<CasaSupabase>()
                    .Where(c => c.CategoriaId == categoriaId && c.Activo == true)
                    .Order(c => c.Nombre, Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                var casas = response.Models ?? new List<CasaSupabase>();
                Console.WriteLine($"Se encontraron {casas.Count} casas para la categoría.");
                return casas;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener casas por categoría: {ex.Message}");
                return new List<CasaSupabase>();
            }
        }

        /// <summary>
        /// Busca casas por nombre
        /// </summary>
        /// <param name="termino">Término de búsqueda</param>
        /// <returns>Lista de casas que coinciden</returns>
        public static async Task<List<CasaSupabase>> BuscarCasas(string termino)
        {
            try
            {
                Console.WriteLine($"Buscando casas con término: {termino}");
                var client = await SupabaseHelper.GetClient();
                
                // Obtener todas las activas y filtrar en código
                var response = await client
                    .From<CasaSupabase>()
                    .Where(c => c.Activo == true)
                    .Order(c => c.Nombre, Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                var casas = response.Models ?? new List<CasaSupabase>();
                
                // Filtrar en código para buscar en nombre
                var casasFiltradas = casas
                    .Where(c => c.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                Console.WriteLine($"Se encontraron {casasFiltradas.Count} casas que coinciden.");
                return casasFiltradas;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar casas: {ex.Message}");
                return new List<CasaSupabase>();
            }
        }

        /// <summary>
        /// Crea una nueva casa
        /// </summary>
        /// <param name="casa">Datos de la casa a crear</param>
        /// <returns>True si se creó exitosamente</returns>
        public static async Task<bool> CrearCasa(CasaSupabase casa)
        {
            try
            {
                if (!casa.EsValido())
                {
                    Console.WriteLine("Los datos de la casa no son válidos.");
                    return false;
                }

                Console.WriteLine($"Creando nueva casa: {casa.Nombre}");
                Console.WriteLine($"Datos a enviar - Nombre: {casa.Nombre}, DuenoId: {casa.DuenoId}, CategoriaId: {casa.CategoriaId}, RutaImagen: {casa.RutaImagen ?? "null"}");
                
                var client = await SupabaseHelper.GetClient();
                
                // Asegurar que la fecha esté en UTC
                casa.FechaCreacion = DateTime.UtcNow;
                
                var response = await client
                    .From<CasaSupabase>()
                    .Insert(casa);

                Console.WriteLine($"Casa creada exitosamente: {casa.Nombre}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear casa: {ex.Message}");
                Console.WriteLine($"Detalles del error: {ex}");
                return false;
            }
        }

        /// <summary>
        /// Actualiza los datos de una casa existente
        /// </summary>
        /// <param name="casa">Casa con datos actualizados</param>
        /// <returns>True si se actualizó exitosamente</returns>
        public static async Task<bool> ActualizarCasa(CasaSupabase casa)
        {
            try
            {
                if (!casa.EsValido())
                {
                    Console.WriteLine("Los datos de la casa no son válidos.");
                    return false;
                }

                Console.WriteLine($"Actualizando casa: {casa.Nombre}");
                var client = await SupabaseHelper.GetClient();
                
                var response = await client
                    .From<CasaSupabase>()
                    .Where(c => c.Id == casa.Id)
                    .Update(casa);

                Console.WriteLine($"Casa actualizada exitosamente: {casa.Nombre}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar casa: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina una casa (soft delete - marca como inactivo)
        /// </summary>
        /// <param name="id">ID de la casa a eliminar</param>
        /// <returns>True si se eliminó exitosamente</returns>
        public static async Task<bool> EliminarCasa(int id)
        {
            try
            {
                Console.WriteLine($"Eliminando casa con ID: {id}");
                var client = await SupabaseHelper.GetClient();
                
                // Soft delete: marcar como inactivo en lugar de eliminar físicamente
                var casa = await ObtenerCasaPorId(id);
                if (casa == null)
                {
                    Console.WriteLine("Casa no encontrada.");
                    return false;
                }

                casa.Activo = false;
                
                var response = await client
                    .From<CasaSupabase>()
                    .Where(c => c.Id == id)
                    .Update(casa);

                Console.WriteLine($"Casa eliminada exitosamente: {casa.Nombre}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar casa: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si un nombre de casa ya está en uso
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
                    .From<CasaSupabase>()
                    .Where(c => c.Nombre.ToLower() == nombre.ToLower() && c.Activo == true)
                    .Get();

                var casas = response.Models ?? new List<CasaSupabase>();
                
                if (idExcluir > 0)
                {
                    casas = casas.Where(c => c.Id != idExcluir).ToList();
                }

                return casas.Any();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar nombre: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene todas las casas inactivas
        /// </summary>
        /// <returns>Lista de casas inactivas</returns>
        public static async Task<List<CasaSupabase>> ObtenerCasasInactivas()
        {
            try
            {
                Console.WriteLine("Obteniendo todas las casas inactivas desde Supabase...");
                var client = await SupabaseHelper.GetClient();
                
                var response = await client
                    .From<CasaSupabase>()
                    .Where(c => c.Activo == false)
                    .Order(c => c.Nombre, Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                var casas = response.Models ?? new List<CasaSupabase>();
                Console.WriteLine($"Se obtuvieron {casas.Count} casas inactivas.");
                return casas;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener casas inactivas: {ex.Message}");
                return new List<CasaSupabase>();
            }
        }

        /// <summary>
        /// Reactiva una casa (marca como activo)
        /// </summary>
        /// <param name="id">ID de la casa a reactivar</param>
        /// <returns>True si se reactivó exitosamente</returns>
        public static async Task<bool> ReactivarCasa(int id)
        {
            try
            {
                Console.WriteLine($"Reactivando casa con ID: {id}");
                var client = await SupabaseHelper.GetClient();
                
                // Obtener la casa
                var casa = await ObtenerCasaPorId(id);
                if (casa == null)
                {
                    Console.WriteLine("Casa no encontrada.");
                    return false;
                }

                casa.Activo = true;
                
                var response = await client
                    .From<CasaSupabase>()
                    .Where(c => c.Id == id)
                    .Update(casa);

                Console.WriteLine($"Casa reactivada exitosamente: {casa.Nombre}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al reactivar casa: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene el total de casas activas
        /// </summary>
        /// <returns>Número de casas activas</returns>
        public static async Task<int> ObtenerTotalCasas()
        {
            try
            {
                var casas = await ObtenerTodasCasas();
                return casas.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener total de casas: {ex.Message}");
                return 0;
            }
        }
    }
}