using FlujoCajaWpf.Models;
using Supabase.Postgrest;

namespace FlujoCajaWpf.Data
{
    public static class SupabaseMovimientoIAHelper
    {
        public static async Task<List<MovimientoIA>> ObtenerPorCasaAsync(int casaId, int? mes = null, int? anio = null)
        {
            try
            {
                var client = SupabaseHelper.Client;
                var query = client.From<MovimientoIASupabase>()
                    .Where(x => x.CasaId == casaId)
                    .Where(x => x.Estado != "aprobado"); // Solo los pendientes/con errores

                if (mes.HasValue)
                    query = query.Where(x => x.Mes == mes.Value);
                if (anio.HasValue)
                    query = query.Where(x => x.Anio == anio.Value);

                var result = await query.Order(x => x.FechaCreacion, Constants.Ordering.Ascending).Get();
                return result.Models.Select(MovimientoIA.FromSupabase).ToList();
            }
            catch
            {
                return new List<MovimientoIA>();
            }
        }

        public static async Task<(bool Success, MovimientoIA? Data, string? Error)> InsertarAsync(MovimientoIASupabase mov)
        {
            try
            {
                var client = SupabaseHelper.Client;
                var result = await client.From<MovimientoIASupabase>().Insert(mov);
                var inserted = result.Models.FirstOrDefault();
                return (true, inserted != null ? MovimientoIA.FromSupabase(inserted) : null, null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        public static async Task<(bool Success, string? Error)> ActualizarAsync(MovimientoIASupabase mov)
        {
            try
            {
                var client = SupabaseHelper.Client;
                await client.From<MovimientoIASupabase>().Upsert(mov);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public static async Task<(bool Success, string? Error)> EliminarAsync(int id)
        {
            try
            {
                var client = SupabaseHelper.Client;
                await client.From<MovimientoIASupabase>()
                    .Where(x => x.Id == id)
                    .Delete();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public static async Task<(bool Success, string? Error)> EliminarVariosAsync(IEnumerable<int> ids)
        {
            try
            {
                var client = SupabaseHelper.Client;
                foreach (var id in ids)
                {
                    await client.From<MovimientoIASupabase>()
                        .Where(x => x.Id == id)
                        .Delete();
                }
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Marca el movimiento IA como aprobado y guarda el ID del movimiento real creado.
        /// </summary>
        public static async Task<(bool Success, string? Error)> MarcarAprobadoAsync(
            int iaId, int movimientoIdCreado, string usuarioAprobo)
        {
            try
            {
                await SupabaseHelper.Client
                    .From<MovimientoIASupabase>()
                    .Where(x => x.Id == iaId)
                    .Set(x => x.Estado,             "aprobado")
                    .Set(x => x.MovimientoIdCreado, movimientoIdCreado)
                    .Set(x => x.UsuarioAprobo,      usuarioAprobo)
                    .Set(x => x.FechaAprobacion,    DateTime.UtcNow)
                    .Update();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Marca múltiples movimientos IA como aprobados en una sola llamada HTTP (bulk update).
        /// No persiste movimiento_id_creado individual; usar cuando el vínculo exacto no es crítico.
        /// </summary>
        public static async Task<(bool Success, string? Error)> MarcarAprobadosBatchAsync(
            IEnumerable<int> iaIds, string usuarioAprobo)
        {
            try
            {
                var idList = iaIds.Select(id => (object)id).ToList();
                await SupabaseHelper.Client
                    .From<MovimientoIASupabase>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.In, idList)
                    .Set(x => x.Estado,          "aprobado")
                    .Set(x => x.UsuarioAprobo,   usuarioAprobo)
                    .Set(x => x.FechaAprobacion, DateTime.UtcNow)
                    .Update();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Actualiza los campos de un movimiento IA con los datos recibidos de n8n (reintento).
        /// </summary>
        public static async Task<(bool Success, string? Error)> ActualizarConDatosIAAsync(
            int iaId,
            DateTime? fecha,
            decimal? monto,
            string? descripcion,
            string? categoria,
            string? tipoMovimiento,
            string estado,
            string? rawJson)
        {
            try
            {
                var client = SupabaseHelper.Client;
                var existing = await client.From<MovimientoIASupabase>()
                    .Where(x => x.Id == iaId)
                    .Single();

                if (existing == null) return (false, "No encontrado");

                existing.Fecha          = fecha;
                existing.Monto          = monto;
                existing.Descripcion    = descripcion;
                existing.Categoria      = categoria;
                existing.TipoMovimiento = tipoMovimiento;
                existing.Estado         = estado;
                existing.RawJson        = rawJson;

                await client.From<MovimientoIASupabase>().Upsert(existing);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
