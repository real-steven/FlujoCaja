using FlujoCajaWpf.Models;

namespace FlujoCajaWpf.Data
{
    /// <summary>
    /// Helper para operaciones CRUD de casas en Supabase
    /// </summary>
    public static class SupabaseCasaHelper
    {
        /// <summary>
        /// Obtiene todas las casas sin filtrar por estado
        /// </summary>
        public static async Task<List<Casa>> ObtenerTodasCasasSinFiltroAsync()
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<CasaSupabase>()
                    .Order("nombre", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                if (response.Models == null || !response.Models.Any())
                {
                    Console.WriteLine("No se encontraron casas");
                    return new List<Casa>();
                }

                // Obtener todos los dueños
                var responseDuenos = await SupabaseHelper.Client
                    .From<DuenoSupabase>()
                    .Get();

                var duenosDict = responseDuenos.Models?
                    .ToDictionary(d => d.Id, d => d.NombreCompleto ?? $"{d.Nombre} {d.Apellido}") 
                    ?? new Dictionary<int, string>();

                // Obtener todas las categorías
                var responseCategorias = await SupabaseHelper.Client
                    .From<CategoriaSupabase>()
                    .Get();

                var categoriasDict = responseCategorias.Models?
                    .ToDictionary(c => c.Id, c => c.Nombre) 
                    ?? new Dictionary<int, string>();

                var casas = response.Models.Select(casaDb =>
                {
                    var casa = Casa.FromSupabase(casaDb);
                    casa.DuenoNombre = duenosDict.ContainsKey(casaDb.DuenoId) 
                        ? duenosDict[casaDb.DuenoId] 
                        : "Sin dueño";
                    casa.CategoriaNombre = categoriasDict.ContainsKey(casaDb.CategoriaId)
                        ? categoriasDict[casaDb.CategoriaId]
                        : "Sin categoría";
                    return casa;
                }).ToList();

                Console.WriteLine($"✓ {casas.Count} casas cargadas (todas)");
                return casas;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener casas: {ex.Message}");
                return new List<Casa>();
            }
        }

        /// <summary>
        /// Obtiene todas las casas activas
        /// </summary>
        public static async Task<List<Casa>> ObtenerTodasCasasAsync()
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<CasaSupabase>()
                    .Where(c => c.Activo == true)
                    .Order("nombre", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                if (response.Models == null || !response.Models.Any())
                {
                    Console.WriteLine("No se encontraron casas");
                    return new List<Casa>();
                }

                // Obtener todos los dueños
                var responseDuenos = await SupabaseHelper.Client
                    .From<DuenoSupabase>()
                    .Get();

                var duenosDict = responseDuenos.Models?
                    .ToDictionary(d => d.Id, d => d.NombreCompleto ?? $"{d.Nombre} {d.Apellido}") 
                    ?? new Dictionary<int, string>();

                // Obtener todas las categorías
                var responseCategorias = await SupabaseHelper.Client
                    .From<CategoriaSupabase>()
                    .Get();

                var categoriasDict = responseCategorias.Models?
                    .ToDictionary(c => c.Id, c => c.Nombre) 
                    ?? new Dictionary<int, string>();

                var casas = response.Models.Select(casaDb =>
                {
                    var casa = Casa.FromSupabase(casaDb);
                    casa.DuenoNombre = duenosDict.ContainsKey(casaDb.DuenoId) 
                        ? duenosDict[casaDb.DuenoId] 
                        : "Sin dueño";
                    casa.CategoriaNombre = categoriasDict.ContainsKey(casaDb.CategoriaId)
                        ? categoriasDict[casaDb.CategoriaId]
                        : "Sin categoría";
                    return casa;
                }).ToList();

                Console.WriteLine($"✓ {casas.Count} casas cargadas");
                return casas;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener casas: {ex.Message}");
                return new List<Casa>();
            }
        }

        /// <summary>
        /// Obtiene una casa por ID
        /// </summary>
        public static async Task<Casa?> ObtenerCasaPorIdAsync(int id)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<CasaSupabase>()
                    .Where(c => c.Id == id)
                    .Single();

                if (response == null)
                    return null;

                var casa = Casa.FromSupabase(response);

                // Obtener nombre del dueño
                var dueno = await SupabaseHelper.Client
                    .From<DuenoSupabase>()
                    .Where(d => d.Id == response.DuenoId)
                    .Single();

                if (dueno != null)
                {
                    casa.DuenoNombre = dueno.NombreCompleto ?? $"{dueno.Nombre} {dueno.Apellido}";
                }

                // Obtener nombre de la categoría
                var categoria = await SupabaseHelper.Client
                    .From<CategoriaSupabase>()
                    .Where(c => c.Id == response.CategoriaId)
                    .Single();

                if (categoria != null)
                {
                    casa.CategoriaNombre = categoria.Nombre;
                }

                return casa;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener casa: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Inserta una nueva casa
        /// </summary>
        public static async Task<(bool Success, int? Id, string? Error)> InsertarCasaAsync(CasaSupabase casa)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<CasaSupabase>()
                    .Insert(casa);

                if (response.Models == null || !response.Models.Any())
                {
                    return (false, null, "No se pudo insertar la casa");
                }

                var nuevaCasa = response.Models.First();
                Console.WriteLine($"✓ Casa creada: {nuevaCasa.Nombre}");
                
                // Crear automáticamente las hojas mensuales (2025 completo + ene-feb 2026)
                var resultadoHojas = await SupabaseHojaMensualHelper.CrearHojasMensualesParaCasaNuevaAsync(nuevaCasa.Id);
                if (!resultadoHojas.Success)
                {
                    Console.WriteLine($"⚠️ Advertencia: Casa creada pero falló al crear hojas mensuales: {resultadoHojas.Error}");
                }
                
                return (true, nuevaCasa.Id, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar casa: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Actualiza una casa existente
        /// </summary>
        public static async Task<(bool Success, string? Error)> ActualizarCasaAsync(CasaSupabase casa)
        {
            try
            {
                await SupabaseHelper.Client
                    .From<CasaSupabase>()
                    .Where(c => c.Id == casa.Id)
                    .Update(casa);

                Console.WriteLine($"✓ Casa actualizada: {casa.Nombre}");
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar casa: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Marca una casa como inactiva (soft delete)
        /// </summary>
        public static async Task<(bool Success, string? Error)> DesactivarCasaAsync(int id)
        {
            try
            {
                var casa = new CasaSupabase { Id = id, Activo = false };
                
                await SupabaseHelper.Client
                    .From<CasaSupabase>()
                    .Where(c => c.Id == id)
                    .Update(casa);

                Console.WriteLine($"✓ Casa desactivada");
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al desactivar casa: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Elimina una casa permanentemente
        /// </summary>
        public static async Task<(bool Success, string? Error)> EliminarCasaAsync(int id)
        {
            try
            {
                await SupabaseHelper.Client
                    .From<CasaSupabase>()
                    .Where(c => c.Id == id)
                    .Delete();

                Console.WriteLine($"✓ Casa eliminada");
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar casa: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene todas las casas con manejo de errores estructurado
        /// </summary>
        public static async Task<(bool Success, List<Casa>? Data, string? Error)> ObtenerCasasAsync()
        {
            try
            {
                var casas = await ObtenerTodasCasasAsync();
                return (true, casas, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener casas: {ex.Message}");
                return (false, null, ex.Message);
            }
        }
    }
}
