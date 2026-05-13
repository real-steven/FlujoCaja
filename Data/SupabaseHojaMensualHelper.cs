using FlujoCajaWpf.Models;

namespace FlujoCajaWpf.Data
{
    public static class SupabaseHojaMensualHelper
    {
        /// <summary>
        /// Obtiene todas las hojas mensuales de una casa
        /// </summary>
        public static async Task<List<HojaMensual>> ObtenerHojasPorCasaAsync(int casaId)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<HojaMensualSupabase>()
                    .Where(h => h.CasaId == casaId)
                    .Order("anio", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Order("mes", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                if (response.Models == null || !response.Models.Any())
                {
                    return new List<HojaMensual>();
                }

                return response.Models.Select(HojaMensual.FromSupabase).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener hojas mensuales: {ex.Message}");
                return new List<HojaMensual>();
            }
        }

        /// <summary>
        /// Obtiene años distintos disponibles para una casa
        /// </summary>
        public static async Task<List<int>> ObtenerAniosDisponiblesAsync(int casaId)
        {
            try
            {
                var hojas = await ObtenerHojasPorCasaAsync(casaId);
                return hojas.Select(h => h.Anio).Distinct().OrderDescending().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener años: {ex.Message}");
                return new List<int>();
            }
        }

        /// <summary>
        /// Crea automáticamente hojas mensuales para casa nueva:
        /// todo 2025 + desde enero 2026 hasta el mes siguiente al actual.
        /// </summary>
        public static async Task<(bool Success, string? Error)> CrearHojasMensualesParaCasaNuevaAsync(int casaId)
        {
            try
            {
                var hojasMensuales = new List<HojaMensualSupabase>();

                // Crear todas las hojas de 2025 (enero a diciembre)
                for (int mes = 1; mes <= 12; mes++)
                {
                    hojasMensuales.Add(new HojaMensualSupabase
                    {
                        CasaId = casaId,
                        Mes = mes,
                        Anio = 2025,
                        Cerrada = false,
                        FechaCreacion = DateTime.UtcNow
                    });
                }

                // Desde enero 2026 hasta el mes siguiente al actual (inclusive)
                var cursor = new DateTime(2026, 1, 1);
                var hasta = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);
                while (cursor <= hasta)
                {
                    hojasMensuales.Add(new HojaMensualSupabase
                    {
                        CasaId = casaId,
                        Mes = cursor.Month,
                        Anio = cursor.Year,
                        Cerrada = false,
                        FechaCreacion = DateTime.UtcNow
                    });
                    cursor = cursor.AddMonths(1);
                }

                await SupabaseHelper.Client
                    .From<HojaMensualSupabase>()
                    .Insert(hojasMensuales);

                Console.WriteLine($"✓ Creadas {hojasMensuales.Count} hojas mensuales para casa {casaId} (2025 completo + ene-{hasta:MMMM} {hasta.Year})");
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear hojas mensuales: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene meses disponibles para una casa en un año específico
        /// </summary>
        public static async Task<List<HojaMensual>> ObtenerMesesDisponiblesAsync(int casaId, int anio)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<HojaMensualSupabase>()
                    .Where(h => h.CasaId == casaId && h.Anio == anio)
                    .Order("mes", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                if (response.Models == null || !response.Models.Any())
                {
                    return new List<HojaMensual>();
                }

                return response.Models.Select(HojaMensual.FromSupabase).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener meses: {ex.Message}");
                return new List<HojaMensual>();
            }
        }

        /// <summary>
        /// Obtiene una hoja mensual específica por casa, mes y año
        /// </summary>
        public static async Task<HojaMensual?> ObtenerHojaPorPeriodoAsync(int casaId, int mes, int anio)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<HojaMensualSupabase>()
                    .Where(h => h.CasaId == casaId)
                    .Where(h => h.Mes == mes)
                    .Where(h => h.Anio == anio)
                    .Single();

                return response != null ? HojaMensual.FromSupabase(response) : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener hoja mensual: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene la hoja del mes actual para una casa
        /// </summary>
        public static async Task<HojaMensual?> ObtenerHojaMesActualAsync(int casaId)
        {
            var mesActual = DateTime.Now.Month;
            var anioActual = DateTime.Now.Year;

            return await ObtenerHojaPorPeriodoAsync(casaId, mesActual, anioActual);
        }

        /// <summary>
        /// Crea una nueva hoja mensual
        /// </summary>
        public static async Task<(bool Success, string? Error)> CrearHojaMensualAsync(HojaMensual hoja)
        {
            try
            {
                var hojaDb = hoja.ToSupabase();
                await SupabaseHelper.Client.From<HojaMensualSupabase>().Insert(hojaDb);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Cerrar una hoja mensual (marcar como cerrada)
        /// </summary>
        public static async Task<(bool Success, string? Error)> CerrarHojaMensualAsync(int hojaId)
        {
            try
            {
                var hoja = await SupabaseHelper.Client
                    .From<HojaMensualSupabase>()
                    .Where(h => h.Id == hojaId)
                    .Single();

                if (hoja == null)
                {
                    return (false, "Hoja mensual no encontrada");
                }

                hoja.Cerrada = true;
                await hoja.Update<HojaMensualSupabase>();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Garantiza que exista la hoja del mes siguiente al actual para una casa.
        /// Debe llamarse al abrir DetalleCasaWindow para mantener siempre 1 mes adelante.
        /// </summary>
        public static async Task AsegurarHojaProximoMesAsync(int casaId)
        {
            try
            {
                var proximo = DateTime.Now.AddMonths(1);
                var mes = proximo.Month;
                var anio = proximo.Year;

                var existe = await ObtenerHojaPorPeriodoAsync(casaId, mes, anio);
                if (existe != null) return;

                await SupabaseHelper.Client
                    .From<HojaMensualSupabase>()
                    .Insert(new HojaMensualSupabase
                    {
                        CasaId = casaId,
                        Mes = mes,
                        Anio = anio,
                        Cerrada = false,
                        FechaCreacion = DateTime.UtcNow
                    });

                Console.WriteLine($"✓ Hoja {mes}/{anio} creada automáticamente para casa {casaId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al asegurar hoja próximo mes: {ex.Message}");
            }
        }
    }
}
