using FlujoDeCajaApp.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlujoDeCajaApp.Data
{
    /// <summary>
    /// Helper class para manejar operaciones CRUD de Dueños en Supabase
    /// Proporciona métodos para crear, leer, actualizar y eliminar dueños
    /// </summary>
    public static class SupabaseDuenoHelper
    {
        /// <summary>
        /// Obtiene todos los dueños activos
        /// </summary>
        /// <returns>Lista de dueños activos</returns>
        public static async Task<List<DuenoSupabase>> ObtenerTodosDuenos()
        {
            try
            {
                Console.WriteLine("Obteniendo todos los dueños desde Supabase...");
                var client = await SupabaseHelper.GetClient();
                
                var response = await client
                    .From<DuenoSupabase>()
                    .Where(d => d.Activo == true)
                    .Order(d => d.Nombre, Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                var duenos = response.Models ?? new List<DuenoSupabase>();
                Console.WriteLine($"Se obtuvieron {duenos.Count} dueños.");
                return duenos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener dueños: {ex.Message}");
                return new List<DuenoSupabase>();
            }
        }

        /// <summary>
        /// Obtiene un dueño por su ID
        /// </summary>
        /// <param name="id">ID del dueño</param>
        /// <returns>Dueño encontrado o null</returns>
        public static async Task<DuenoSupabase?> ObtenerDuenoPorId(int id)
        {
            try
            {
                Console.WriteLine($"Obteniendo dueño con ID: {id}");
                var client = await SupabaseHelper.GetClient();
                
                var response = await client
                    .From<DuenoSupabase>()
                    .Where(d => d.Id == id)
                    .Single();

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener dueño por ID {id}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Busca dueños por nombre o apellido
        /// </summary>
        /// <param name="termino">Término de búsqueda</param>
        /// <returns>Lista de dueños que coinciden</returns>
        public static async Task<List<DuenoSupabase>> BuscarDuenos(string termino)
        {
            try
            {
                Console.WriteLine($"Buscando dueños con término: {termino}");
                var client = await SupabaseHelper.GetClient();
                
                // Simplificar la búsqueda - obtener todos los activos y filtrar en código
                var response = await client
                    .From<DuenoSupabase>()
                    .Where(d => d.Activo == true)
                    .Order(d => d.Nombre, Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                var duenos = response.Models ?? new List<DuenoSupabase>();
                
                // Filtrar en código para buscar en nombre o apellido
                var duenosFiltrados = duenos
                    .Where(d => 
                        d.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase) || 
                        d.Apellido.Contains(termino, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                Console.WriteLine($"Se encontraron {duenosFiltrados.Count} dueños que coinciden.");
                return duenosFiltrados;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar dueños: {ex.Message}");
                return new List<DuenoSupabase>();
            }
        }

        /// <summary>
        /// Crea un nuevo dueño
        /// </summary>
        /// <param name="dueno">Datos del dueño a crear</param>
        /// <returns>True si se creó exitosamente</returns>
        public static async Task<bool> CrearDueno(DuenoSupabase dueno)
        {
            try
            {
                if (!dueno.EsValido())
                {
                    Console.WriteLine("Los datos del dueño no son válidos.");
                    return false;
                }

                Console.WriteLine($"Creando nuevo dueño: {dueno.Nombre} {dueno.Apellido}");
                Console.WriteLine($"Datos a enviar - Nombre: {dueno.Nombre}, Apellido: {dueno.Apellido}, Telefono: {dueno.Telefono}, Email: {dueno.Email}");
                
                var client = await SupabaseHelper.GetClient();
                
                // Asegurar que las fechas estén en UTC
                dueno.FechaCreacion = DateTime.UtcNow;
                dueno.FechaActualizacion = DateTime.UtcNow;
                
                var response = await client
                    .From<DuenoSupabase>()
                    .Insert(dueno);

                Console.WriteLine($"Dueño creado exitosamente: {dueno.Nombre} {dueno.Apellido}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear dueño: {ex.Message}");
                Console.WriteLine($"Detalles del error: {ex}");
                return false;
            }
        }

        /// <summary>
        /// Actualiza los datos de un dueño existente
        /// </summary>
        /// <param name="dueno">Dueño con datos actualizados</param>
        /// <returns>True si se actualizó exitosamente</returns>
        public static async Task<bool> ActualizarDueno(DuenoSupabase dueno)
        {
            try
            {
                if (!dueno.EsValido())
                {
                    Console.WriteLine("Los datos del dueño no son válidos.");
                    return false;
                }

                Console.WriteLine($"Actualizando dueño: {dueno.Nombre} {dueno.Apellido}");
                var client = await SupabaseHelper.GetClient();
                
                dueno.ActualizarFecha();
                
                var response = await client
                    .From<DuenoSupabase>()
                    .Where(d => d.Id == dueno.Id)
                    .Update(dueno);

                Console.WriteLine($"Dueño actualizado exitosamente: {dueno.Nombre} {dueno.Apellido}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar dueño: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un dueño (soft delete - marca como inactivo)
        /// </summary>
        /// <param name="id">ID del dueño a eliminar</param>
        /// <returns>True si se eliminó exitosamente</returns>
        public static async Task<bool> EliminarDueno(int id)
        {
            try
            {
                Console.WriteLine($"Eliminando dueño con ID: {id}");
                var client = await SupabaseHelper.GetClient();
                
                // Soft delete: marcar como inactivo en lugar de eliminar físicamente
                var dueno = await ObtenerDuenoPorId(id);
                if (dueno == null)
                {
                    Console.WriteLine("Dueño no encontrado.");
                    return false;
                }

                dueno.Activo = false;
                dueno.ActualizarFecha();
                
                var response = await client
                    .From<DuenoSupabase>()
                    .Where(d => d.Id == id)
                    .Update(dueno);

                Console.WriteLine($"Dueño eliminado exitosamente: {dueno.Nombre} {dueno.Apellido}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar dueño: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si un email ya está en uso por otro dueño
        /// </summary>
        /// <param name="email">Email a verificar</param>
        /// <param name="idExcluir">ID a excluir de la verificación (para updates)</param>
        /// <returns>True si el email ya existe</returns>
        public static async Task<bool> ExisteEmail(string email, int idExcluir = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                var client = await SupabaseHelper.GetClient();
                
                var query = client
                    .From<DuenoSupabase>()
                    .Where(d => d.Email == email && d.Activo == true);

                if (idExcluir > 0)
                {
                    query = query.Where(d => d.Id != idExcluir);
                }

                var response = await query.Get();
                return response.Models?.Any() == true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar email: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene el total de dueños activos
        /// </summary>
        /// <returns>Número de dueños activos</returns>
        public static async Task<int> ObtenerTotalDuenos()
        {
            try
            {
                var duenos = await ObtenerTodosDuenos();
                return duenos.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener total de dueños: {ex.Message}");
                return 0;
            }
        }
    }
}