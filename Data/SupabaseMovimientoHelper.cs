using FlujoCajaWpf.Models;
using Supabase;

namespace FlujoCajaWpf.Data
{
    public static class SupabaseMovimientoHelper
    {
        /// <summary>
        /// Obtiene movimientos por hoja mensual (nuevo método principal)
        /// </summary>
        public static async Task<(bool Success, List<Movimiento>? Data, string? Error)> ObtenerMovimientosPorHojaAsync(int hojaMensualId)
        {
            try
            {
                var client = SupabaseHelper.Client;

                // Obtener movimientos activos de la hoja mensual
                var resultadoMovimientos = await client
                    .From<MovimientoSupabase>()
                    .Where(m => m.HojaMensualId == hojaMensualId && m.Activo == true)
                    .Order("fecha", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                if (resultadoMovimientos?.Models == null)
                {
                    return (false, null, "Error al cargar movimientos");
                }

                // Convertir a modelo de UI
                var movimientos = resultadoMovimientos.Models.Select(m => new Movimiento
                {
                    Id = m.Id,
                    CasaId = m.CasaId,                    HojaMensualId = m.HojaMensualId,                    CategoriaNombre = m.Categoria,
                    Tipo = m.TipoMovimiento,
                    Monto = m.Monto,
                    Fecha = m.Fecha,
                    Descripcion = m.Descripcion,
                    Activo = m.Activo,
                    FechaCreacion = m.FechaCreacion
                }).ToList();

                return (true, movimientos, null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene todos los movimientos de una casa (sin filtro de hoja)
        /// </summary>
        public static async Task<(bool Success, List<Movimiento>? Data, string? Error)> ObtenerMovimientosPorCasaAsync(int casaId)
        {
            try
            {
                var client = SupabaseHelper.Client;

                // Obtener movimientos activos
                var resultadoMovimientos = await client
                    .From<MovimientoSupabase>()
                    .Where(m => m.CasaId == casaId && m.Activo == true)
                    .Order("fecha", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                if (resultadoMovimientos?.Models == null)
                {
                    return (false, null, "Error al cargar movimientos");
                }

                // Convertir a modelo de UI
                var movimientos = resultadoMovimientos.Models.Select(m => new Movimiento
                {
                    Id = m.Id,
                    CasaId = m.CasaId,
                    HojaMensualId = m.HojaMensualId,
                    CategoriaNombre = m.Categoria,
                    Tipo = m.TipoMovimiento,
                    Monto = m.Monto,
                    Fecha = m.Fecha,
                    Descripcion = m.Descripcion,
                    Activo = m.Activo,
                    FechaCreacion = m.FechaCreacion
                }).ToList();

                return (true, movimientos, null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        public static async Task<(bool Success, decimal Balance, string? Error)> ObtenerBalanceCasaAsync(int casaId)
        {
            try
            {
                var resultado = await ObtenerMovimientosPorCasaAsync(casaId);
                
                if (!resultado.Success || resultado.Data == null)
                {
                    return (false, 0, resultado.Error);
                }

                // Calcular balance: sumar todos los montos (ya vienen con signo correcto)
                var balance = resultado.Data.Sum(m => m.Monto);

                return (true, balance, null);
            }
            catch (Exception ex)
            {
                return (false, 0, ex.Message);
            }
        }

        public static async Task<(bool Success, string? Error)> InsertarMovimientoAsync(MovimientoSupabase movimiento)
        {
            try
            {
                var client = SupabaseHelper.Client;
                await client.From<MovimientoSupabase>().Insert(movimiento);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public static async Task<(bool Success, string? Error)> ActualizarMovimientoAsync(MovimientoSupabase movimiento)
        {
            try
            {
                var client = SupabaseHelper.Client;
                await client.From<MovimientoSupabase>().Update(movimiento);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public static async Task<(bool Success, string? Error)> EliminarMovimientoAsync(int id)
        {
            try
            {
                var client = SupabaseHelper.Client;
                await client.From<MovimientoSupabase>()
                    .Where(m => m.Id == id)
                    .Delete();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public static async Task<(bool Success, Dictionary<string, decimal>? Data, string? Error)> ObtenerEstadisticasPorCategoriaAsync(int casaId)
        {
            try
            {
                var resultado = await ObtenerMovimientosPorCasaAsync(casaId);
                
                if (!resultado.Success || resultado.Data == null)
                {
                    return (false, null, resultado.Error);
                }

                var estadisticas = resultado.Data
                    .GroupBy(m => new { m.CategoriaNombre, m.Tipo })
                    .ToDictionary(
                        g => $"{g.Key.CategoriaNombre} ({(g.Key.Tipo.Equals("Ingreso", StringComparison.OrdinalIgnoreCase) ? "Ingreso" : "Gasto")})",
                        g => g.Sum(m => m.Monto)
                    );

                return (true, estadisticas, null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        public static async Task<(bool Success, Dictionary<DateTime, decimal>? Data, string? Error)> ObtenerBalanceMensualAsync(int casaId, int meses = 6)
        {
            try
            {
                var resultado = await ObtenerMovimientosPorCasaAsync(casaId);
                
                if (!resultado.Success || resultado.Data == null)
                {
                    return (false, null, resultado.Error);
                }

                var fechaInicio = DateTime.Now.AddMonths(-meses);
                
                // Los montos ya vienen con signo correcto (negativos para gastos)
                var balanceMensual = resultado.Data
                    .Where(m => m.Fecha >= fechaInicio)
                    .GroupBy(m => new DateTime(m.Fecha.Year, m.Fecha.Month, 1))
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(m => m.Monto)
                    );

                return (true, balanceMensual, null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene movimientos de un mes específico
        /// </summary>
        public static async Task<(bool Success, List<Movimiento>? Data, string? Error)> ObtenerMovimientosPorMesAsync(int casaId, int año, int mes)
        {
            try
            {
                // Obtener la hoja mensual correspondiente
                var hojaMensual = await SupabaseHojaMensualHelper.ObtenerHojaPorPeriodoAsync(casaId, mes, año);
                
                if (hojaMensual == null)
                {
                    // No hay hoja mensual para este periodo, retornar lista vacía
                    return (true, new List<Movimiento>(), null);
                }

                // Obtener movimientos por hoja_mensual_id
                var resultado = await ObtenerMovimientosPorCasaAsync(casaId);
                
                if (!resultado.Success || resultado.Data == null)
                {
                    return (false, null, resultado.Error);
                }

                var movimientosMes = resultado.Data
                    .Where(m => m.HojaMensualId == hojaMensual.Id)
                    .ToList();

                return (true, movimientosMes, null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene años disponibles con movimientos para una casa
        /// </summary>
        public static async Task<List<int>> ObtenerAniosConMovimientosAsync(int casaId)
        {
            try
            {
                var resultado = await ObtenerMovimientosPorCasaAsync(casaId);
                
                if (!resultado.Success || resultado.Data == null || !resultado.Data.Any())
                {
                    return new List<int> { DateTime.Now.Year };
                }

                return resultado.Data
                    .Select(m => m.Fecha.Year)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToList();
            }
            catch
            {
                return new List<int> { DateTime.Now.Year };
            }
        }

        /// <summary>
        /// Obtiene balance acumulado al final de cada mes para un año específico (para gráfico)
        /// Retorna un diccionario con mes (1-12) y balance acumulado hasta ese mes
        /// </summary>
        public static async Task<(bool Success, Dictionary<int, decimal>? Data, string? Error)> ObtenerBalancesMensualesAnualesAsync(int casaId, int año)
        {
            try
            {
                var resultado = await ObtenerMovimientosPorCasaAsync(casaId);
                
                if (!resultado.Success || resultado.Data == null)
                {
                    return (false, null, resultado.Error);
                }

                // Ordenar movimientos por fecha
                var movimientosOrdenados = resultado.Data
                    .OrderBy(m => m.Fecha)
                    .ToList();

                // Calcular balance acumulado al final de cada mes
                var balancesPorMes = new Dictionary<int, decimal>();
                decimal balanceAcumulado = 0;

                // Calcular balance inicial (todo lo anterior al año seleccionado)
                // Los montos ya vienen con signo correcto (negativos para gastos)
                balanceAcumulado = movimientosOrdenados
                    .Where(m => m.Fecha.Year < año)
                    .Sum(m => m.Monto);

                // Calcular balance al final de cada mes del año
                for (int mes = 1; mes <= 12; mes++)
                {
                    var movimientosDelMes = movimientosOrdenados
                        .Where(m => m.Fecha.Year == año && m.Fecha.Month == mes)
                        .ToList();

                    // Los montos ya vienen con signo correcto (negativos para gastos)
                    var balanceDelMes = movimientosDelMes.Sum(m => m.Monto);

                    balanceAcumulado += balanceDelMes;
                    balancesPorMes[mes] = balanceAcumulado;
                }

                return (true, balancesPorMes, null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }
    }
}
