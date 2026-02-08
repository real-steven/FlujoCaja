using FlujoCajaWpf.Models;

namespace FlujoCajaWpf.Data
{
    public static class SupabasePreferenciasHelper
    {
        /// <summary>
        /// Obtiene las preferencias de un usuario por su email
        /// </summary>
        public static async Task<(bool Success, PreferenciasUsuario? Data, string? Error)> ObtenerPreferenciasAsync(string usuarioEmail)
        {
            try
            {
                var client = SupabaseHelper.Client;

                var resultado = await client
                    .From<PreferenciasUsuarioSupabase>()
                    .Where(p => p.UsuarioEmail == usuarioEmail)
                    .Single();

                if (resultado == null)
                {
                    // No existe preferencias, crear por defecto
                    var nuevaPref = new PreferenciasUsuarioSupabase
                    {
                        UsuarioEmail = usuarioEmail,
                        ModoOscuro = false,
                        FechaCreacion = DateTime.Now,
                        FechaActualizacion = DateTime.Now
                    };

                    await client.From<PreferenciasUsuarioSupabase>().Insert(nuevaPref);

                    return (true, PreferenciasUsuario.FromSupabase(nuevaPref), null);
                }

                return (true, PreferenciasUsuario.FromSupabase(resultado), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener preferencias: {ex.Message}");
                // En caso de error, retornar preferencias por defecto
                return (true, new PreferenciasUsuario { UsuarioEmail = usuarioEmail, ModoOscuro = false }, null);
            }
        }

        /// <summary>
        /// Actualiza el modo oscuro del usuario
        /// </summary>
        public static async Task<(bool Success, string? Error)> ActualizarModoOscuroAsync(string usuarioEmail, bool modoOscuro)
        {
            try
            {
                var client = SupabaseHelper.Client;

                // Verificar si existen preferencias
                var existente = await client
                    .From<PreferenciasUsuarioSupabase>()
                    .Where(p => p.UsuarioEmail == usuarioEmail)
                    .Single();

                if (existente == null)
                {
                    // Crear nueva preferencia
                    var nuevaPref = new PreferenciasUsuarioSupabase
                    {
                        UsuarioEmail = usuarioEmail,
                        ModoOscuro = modoOscuro,
                        FechaCreacion = DateTime.Now,
                        FechaActualizacion = DateTime.Now
                    };

                    await client.From<PreferenciasUsuarioSupabase>().Insert(nuevaPref);
                }
                else
                {
                    // Actualizar existente
                    existente.ModoOscuro = modoOscuro;
                    existente.FechaActualizacion = DateTime.Now;

                    await client.From<PreferenciasUsuarioSupabase>().Update(existente);
                }

                Console.WriteLine($"✓ Preferencia de modo oscuro actualizada: {modoOscuro}");
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar modo oscuro: {ex.Message}");
                return (false, ex.Message);
            }
        }
    }
}
