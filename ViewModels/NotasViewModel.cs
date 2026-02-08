using FlujoCajaWpf.Data;
using FlujoCajaWpf.ViewModels.Base;

namespace FlujoCajaWpf.ViewModels
{
    public class NotasViewModel : ViewModelBase
    {
        private readonly int _casaId;
        private string _nombreCasa = string.Empty;
        private string _duenoPrincipal = string.Empty;
        private string? _notas;

        public string NombreCasa
        {
            get => _nombreCasa;
            set => SetProperty(ref _nombreCasa, value);
        }

        public string DuenoPrincipal
        {
            get => _duenoPrincipal;
            set => SetProperty(ref _duenoPrincipal, value);
        }

        public string? Notas
        {
            get => _notas;
            set => SetProperty(ref _notas, value);
        }

        public NotasViewModel(int casaId, string nombreCasa, string duenoPrincipal, string? notasActuales)
        {
            _casaId = casaId;
            _nombreCasa = nombreCasa;
            _duenoPrincipal = duenoPrincipal;
            _notas = notasActuales;
        }

        public async Task<(bool Success, string? Error)> GuardarNotasAsync()
        {
            try
            {
                // Obtener la casa actual
                var casa = await SupabaseCasaHelper.ObtenerCasaPorIdAsync(_casaId);
                
                if (casa == null)
                {
                    return (false, "No se encontró la casa");
                }

                // Actualizar solo las notas
                var casaActualizada = new Models.CasaSupabase
                {
                    Id = _casaId,
                    Nombre = casa.Nombre,
                    Activo = casa.Activo,
                    DuenoId = casa.DuenoId,
                    CategoriaId = casa.CategoriaId,
                    RutaImagen = casa.RutaImagen,
                    Moneda = casa.Moneda,
                    Notas = Notas,
                    FechaCreacion = casa.FechaCreacion
                };

                var resultado = await SupabaseCasaHelper.ActualizarCasaAsync(casaActualizada);
                
                if (resultado.Success)
                {
                    Console.WriteLine($"✓ Notas guardadas para: {_nombreCasa}");
                }

                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar notas: {ex.Message}");
                return (false, ex.Message);
            }
        }
    }
}
