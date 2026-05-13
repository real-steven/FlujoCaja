using FlujoCajaWpf.Models;

namespace FlujoCajaWpf.Data
{
    public static class SupabaseCorreoCasaHelper
    {
        /// <summary>
        /// Obtiene todos los correos activos asociados a una casa.
        /// </summary>
        public static async Task<List<CorreoCasaSupabase>> ObtenerCorreosPorCasaAsync(int casaId)
        {
            try
            {
                var response = await SupabaseHelper.Client
                    .From<CorreoCasaSupabase>()
                    .Where(c => c.CasaId == casaId && c.Activo == true)
                    .Order("fecha_creacion", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                return response.Models ?? new List<CorreoCasaSupabase>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CorreoCasaHelper] Error al obtener correos: {ex.Message}");
                return new List<CorreoCasaSupabase>();
            }
        }

        /// <summary>
        /// Inserta un correo adicional para una casa. Devuelve el registro creado o error.
        /// </summary>
        public static async Task<(bool Success, CorreoCasaSupabase? Correo, string? Error)> InsertarCorreoAsync(int casaId, string email, string? etiqueta = null)
        {
            try
            {
                var nuevo = new CorreoCasaSupabase
                {
                    CasaId = casaId,
                    Email = email.Trim().ToLowerInvariant(),
                    Etiqueta = etiqueta,
                    Activo = true
                };

                var response = await SupabaseHelper.Client
                    .From<CorreoCasaSupabase>()
                    .Insert(nuevo);

                var insertado = response.Models?.FirstOrDefault();
                if (insertado == null)
                    return (false, null, "No se pudo insertar el correo.");

                return (true, insertado, null);
            }
            catch (Exception ex)
            {
                string msg;
                if (ex.Message.Contains("unique") || ex.Message.Contains("duplicate"))
                    msg = "Este correo ya existe para la casa.";
                else if (ex.Message.Contains("42501") || ex.Message.Contains("row-level security") || ex.Message.Contains("row level security"))
                    msg = "Sin permisos para guardar el correo. Ejecuta el script '06_agregar_email_casas_y_tabla_correos.sql' en el SQL Editor de Supabase para habilitar la política de acceso.";
                else
                    msg = ex.Message;

                Console.WriteLine($"[CorreoCasaHelper] Error al insertar correo: {ex.Message}");
                return (false, null, msg);
            }
        }

        /// <summary>
        /// Desactiva (soft-delete) un correo por su id.
        /// </summary>
        public static async Task<(bool Success, string? Error)> EliminarCorreoAsync(long correoId)
        {
            try
            {
                await SupabaseHelper.Client
                    .From<CorreoCasaSupabase>()
                    .Where(c => c.Id == correoId)
                    .Set(c => c.Activo!, false)
                    .Update();

                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CorreoCasaHelper] Error al eliminar correo: {ex.Message}");
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Actualiza el email_principal de una casa.
        /// </summary>
        public static async Task<(bool Success, string? Error)> ActualizarEmailPrincipalAsync(int casaId, string email)
        {
            try
            {
                await SupabaseHelper.Client
                    .From<CasaSupabase>()
                    .Where(c => c.Id == casaId)
                    .Set(c => c.EmailPrincipal!, email.Trim().ToLowerInvariant())
                    .Update();

                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CorreoCasaHelper] Error al actualizar email principal: {ex.Message}");
                return (false, ex.Message);
            }
        }
    }
}
