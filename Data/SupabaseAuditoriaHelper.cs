using FlujoCajaWpf.Models;
using Supabase;
using System.Text.Json;

namespace FlujoCajaWpf.Data
{
    /// <summary>
    /// Helper para gestionar el historial de auditoría del sistema
    /// </summary>
    public static class SupabaseAuditoriaHelper
    {
        /// <summary>
        /// Registra una acción en el historial de auditoría
        /// </summary>
        public static async Task<(bool Success, string? Error)> RegistrarAccionAsync(
            string usuarioEmail,
            string modulo,
            string tipoAccion,
            int? entidadId,
            string? entidadNombre,
            string descripcion,
            object? datosAnteriores = null,
            object? datosNuevos = null)
        {
            try
            {
                var client = SupabaseHelper.Client;

                var auditoria = new AuditoriaSupabase
                {
                    UsuarioEmail = usuarioEmail,
                    Modulo = modulo,
                    TipoAccion = tipoAccion,
                    EntidadId = entidadId,
                    EntidadNombre = entidadNombre,
                    Descripcion = descripcion,
                    DatosAnteriores = datosAnteriores != null ? JsonSerializer.Serialize(datosAnteriores) : null,
                    DatosNuevos = datosNuevos != null ? JsonSerializer.Serialize(datosNuevos) : null,
                    Fecha = DateTime.Now
                };

                await client.From<AuditoriaSupabase>().Insert(auditoria);

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene auditorías por módulo con paginación
        /// </summary>
        public static async Task<(bool Success, List<Auditoria>? Data, int TotalRegistros, string? Error)> ObtenerAuditoriasPorModuloAsync(
            string? modulo = null,
            string? usuarioEmail = null,
            string? tipoAccion = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            int pagina = 1,
            int registrosPorPagina = 50)
        {
            try
            {
                var client = SupabaseHelper.Client;

                // Construir query base
                var query = client.From<AuditoriaSupabase>().Select("*");

                // Aplicar filtros
                if (!string.IsNullOrEmpty(modulo))
                    query = query.Where(a => a.Modulo == modulo);

                if (!string.IsNullOrEmpty(usuarioEmail))
                    query = query.Where(a => a.UsuarioEmail == usuarioEmail);

                if (!string.IsNullOrEmpty(tipoAccion))
                    query = query.Where(a => a.TipoAccion == tipoAccion);

                if (fechaDesde.HasValue)
                    query = query.Where(a => a.Fecha >= fechaDesde.Value);

                if (fechaHasta.HasValue)
                    query = query.Where(a => a.Fecha <= fechaHasta.Value);

                // Obtener total de registros (para paginación)
                var resultadoTotal = await query.Get();
                int totalRegistros = resultadoTotal?.Models?.Count ?? 0;

                // Aplicar paginación
                int inicio = (pagina - 1) * registrosPorPagina;
                query = query
                    .Order("fecha", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Range(inicio, inicio + registrosPorPagina - 1);

                var resultado = await query.Get();

                if (resultado?.Models == null)
                {
                    return (false, null, 0, "Error al cargar auditorías");
                }

                // Convertir a modelo de UI
                var auditorias = resultado.Models.Select(a => new Auditoria
                {
                    Id = a.Id,
                    UsuarioEmail = a.UsuarioEmail,
                    Modulo = a.Modulo,
                    TipoAccion = a.TipoAccion,
                    EntidadId = a.EntidadId,
                    EntidadNombre = a.EntidadNombre,
                    Descripcion = a.Descripcion,
                    DatosAnteriores = a.DatosAnteriores,
                    DatosNuevos = a.DatosNuevos,
                    Fecha = a.Fecha
                }).ToList();

                return (true, auditorias, totalRegistros, null);
            }
            catch (Exception ex)
            {
                return (false, null, 0, ex.Message);
            }
        }

        /// <summary>
        /// Deshace un movimiento financiero eliminándolo
        /// </summary>
        public static async Task<(bool Success, string? Error)> DeshacerMovimientoAsync(int auditoriaId, string usuarioEmail)
        {
            try
            {
                var client = SupabaseHelper.Client;

                // Obtener la auditoría
                var resultadoAuditoria = await client
                    .From<AuditoriaSupabase>()
                    .Where(a => a.Id == auditoriaId)
                    .Single();

                if (resultadoAuditoria == null)
                {
                    return (false, "No se encontró el registro de auditoría");
                }

                // Verificar que sea un movimiento y que sea de creación
                if (resultadoAuditoria.Modulo != "movimiento" || resultadoAuditoria.TipoAccion != "crear")
                {
                    return (false, "Solo se pueden deshacer movimientos creados");
                }

                if (!resultadoAuditoria.EntidadId.HasValue)
                {
                    return (false, "No se encontró el ID del movimiento");
                }

                // Eliminar el movimiento (soft delete)
                var resultadoEliminar = await SupabaseMovimientoHelper.EliminarMovimientoAsync(resultadoAuditoria.EntidadId.Value);

                if (!resultadoEliminar.Success)
                {
                    return (false, resultadoEliminar.Error);
                }

                // Registrar la acción de deshacer
                await RegistrarAccionAsync(
                    usuarioEmail,
                    "movimiento",
                    "deshacer",
                    resultadoAuditoria.EntidadId,
                    resultadoAuditoria.EntidadNombre,
                    $"Deshizo movimiento: {resultadoAuditoria.Descripcion}"
                );

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene lista de usuarios únicos que han realizado acciones
        /// </summary>
        public static async Task<List<string>> ObtenerUsuariosAsync()
        {
            try
            {
                var client = SupabaseHelper.Client;

                var resultado = await client
                    .From<AuditoriaSupabase>()
                    .Select("usuario_email")
                    .Get();

                if (resultado?.Models == null)
                {
                    return new List<string>();
                }

                return resultado.Models
                    .Select(a => a.UsuarioEmail)
                    .Distinct()
                    .OrderBy(u => u)
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}
