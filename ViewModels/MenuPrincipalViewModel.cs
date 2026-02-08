using FlujoCajaWpf.Commands;
using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using FlujoCajaWpf.Services;
using FlujoCajaWpf.ViewModels.Base;
using FlujoCajaWpf.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace FlujoCajaWpf.ViewModels
{
    public enum TipoOrdenamiento
    {
        NombreAZ,
        NombreZA,
        DuenoAZ,
        DuenoZA,
        BalanceMayorMenor,
        BalanceMenorMayor
    }

    public class MenuPrincipalViewModel : ViewModelBase
    {
        private readonly Window _window;
        private readonly Usuario _usuario;
        private ObservableCollection<Propiedad> _propiedades;
        private ObservableCollection<Propiedad> _propiedadesFiltradas;
        private string _textoBusqueda = string.Empty;
        private bool _estaCargando = true; // Iniciar como true para ocultar mensaje
        private TipoOrdenamiento _ordenamientoActual = TipoOrdenamiento.NombreAZ;
        private string _textoOrdenamiento = "Nombre A-Z";

        public string NombreUsuario => $"👤 {_usuario.Email}";

        public ObservableCollection<Propiedad> Propiedades
        {
            get => _propiedades;
            set => SetProperty(ref _propiedades, value);
        }

        public ObservableCollection<Propiedad> PropiedadesFiltradas
        {
            get => _propiedadesFiltradas;
            set
            {
                SetProperty(ref _propiedadesFiltradas, value);
                OnPropertyChanged(nameof(TieneCasas));
                OnPropertyChanged(nameof(MostrarMensajeVacio));
            }
        }

        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                SetProperty(ref _textoBusqueda, value);
                FiltrarPropiedades();
            }
        }

        public string TextoOrdenamiento
        {
            get => _textoOrdenamiento;
            set => SetProperty(ref _textoOrdenamiento, value);
        }

        public bool EstaCargando
        {
            get => _estaCargando;
            set
            {
                SetProperty(ref _estaCargando, value);
                OnPropertyChanged(nameof(MostrarMensajeVacio));
            }
        }

        public bool TieneCasas => PropiedadesFiltradas?.Count > 0;

        public bool MostrarMensajeVacio => !EstaCargando && !TieneCasas;

        public ICommand CerrarSesionCommand { get; }
        public ICommand AbrirCasaCommand { get; }
        public ICommand AbrirNotasCommand { get; }
        public ICommand AbrirHistorialCommand { get; }
        public ICommand AbrirInactivasCommand { get; }
        public ICommand AbrirTutorialCommand { get; }

        public MenuPrincipalViewModel(Window window, Usuario usuario)
        {
            _window = window;
            _usuario = usuario;
            _propiedades = new ObservableCollection<Propiedad>();
            _propiedadesFiltradas = new ObservableCollection<Propiedad>();

            CerrarSesionCommand = new RelayCommand(CerrarSesion);
            AbrirCasaCommand = new RelayCommand(AbrirCasa);
            AbrirNotasCommand = new RelayCommand(AbrirNotas);
            AbrirHistorialCommand = new RelayCommand(AbrirHistorial);
            AbrirInactivasCommand = new RelayCommand(AbrirInactivas);
            AbrirTutorialCommand = new RelayCommand(AbrirTutorial);

            _ = CargarCasasAsync();
        }

        public async Task CargarCasasAsync()
        {
            try
            {
                EstaCargando = true;

                var casas = await SupabaseCasaHelper.ObtenerTodasCasasAsync();
                
                Propiedades.Clear();
                PropiedadesFiltradas.Clear();

                // Filtrar solo casas activas
                foreach (var casa in casas.Where(c => c.Activo))
                {
                    var propiedad = Propiedad.FromCasa(casa);
                    
                    // Calcular balance de la casa
                    var resultadoBalance = await SupabaseMovimientoHelper.ObtenerBalanceCasaAsync(casa.Id);
                    var balance = resultadoBalance.Success ? resultadoBalance.Balance : 0m;
                    propiedad.Balance = balance;
                    
                    // Establecer color de borde según balance
                    if (balance < 0)
                    {
                        propiedad.ColorBorde = "#DC2626"; // Rojo crítico (negativo)
                        propiedad.GrosordeBorde = new System.Windows.Thickness(3);
                    }
                    else if (balance >= 1 && balance <= 1000)
                    {
                        propiedad.ColorBorde = "#F59E0B"; // Amarillo atención (1-1000)
                        propiedad.GrosordeBorde = new System.Windows.Thickness(3);
                    }
                    else // balance = 0 o balance > 1000
                    {
                        propiedad.ColorBorde = "#E5E7EB"; // Gris normal
                        propiedad.GrosordeBorde = new System.Windows.Thickness(1);
                    }
                    
                    Console.WriteLine($"Casa: {propiedad.Nombre} | Balance: {balance:C} | Color: {propiedad.ColorBorde} | Grosor: {propiedad.GrosordeBorde}");
                    
                    Propiedades.Add(propiedad);
                    PropiedadesFiltradas.Add(propiedad);
                }

                Console.WriteLine($"✓ {Propiedades.Count} propiedades activas cargadas en UI");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar casas: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                EstaCargando = false;
            }
        }

        private void FiltrarPropiedades()
        {
            if (string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                PropiedadesFiltradas = new ObservableCollection<Propiedad>(Propiedades);
            }
            else
            {
                var filtro = TextoBusqueda.ToLower();
                var filtradas = Propiedades.Where(p =>
                    p.Nombre.ToLower().Contains(filtro) ||
                    (p.CategoriaNombre?.ToLower().Contains(filtro) ?? false) ||
                    p.DuenoNombre.ToLower().Contains(filtro)
                ).ToList();

                PropiedadesFiltradas = new ObservableCollection<Propiedad>(filtradas);
            }
            
            AplicarOrdenamiento();
        }

        public void CambiarOrdenamiento()
        {
            _ordenamientoActual = _ordenamientoActual switch
            {
                TipoOrdenamiento.NombreAZ => TipoOrdenamiento.NombreZA,
                TipoOrdenamiento.NombreZA => TipoOrdenamiento.DuenoAZ,
                TipoOrdenamiento.DuenoAZ => TipoOrdenamiento.DuenoZA,
                TipoOrdenamiento.DuenoZA => TipoOrdenamiento.BalanceMayorMenor,
                TipoOrdenamiento.BalanceMayorMenor => TipoOrdenamiento.BalanceMenorMayor,
                TipoOrdenamiento.BalanceMenorMayor => TipoOrdenamiento.NombreAZ,
                _ => TipoOrdenamiento.NombreAZ
            };

            TextoOrdenamiento = _ordenamientoActual switch
            {
                TipoOrdenamiento.NombreAZ => "Nombre A-Z",
                TipoOrdenamiento.NombreZA => "Nombre Z-A",
                TipoOrdenamiento.DuenoAZ => "Dueño A-Z",
                TipoOrdenamiento.DuenoZA => "Dueño Z-A",
                TipoOrdenamiento.BalanceMayorMenor => "Balance ↓",
                TipoOrdenamiento.BalanceMenorMayor => "Balance ↑",
                _ => "Nombre A-Z"
            };

            AplicarOrdenamiento();
        }

        private void AplicarOrdenamiento()
        {
            if (PropiedadesFiltradas == null || !PropiedadesFiltradas.Any())
                return;

            List<Propiedad> ordenadas = _ordenamientoActual switch
            {
                TipoOrdenamiento.NombreAZ => PropiedadesFiltradas.OrderBy(p => p.Nombre).ToList(),
                TipoOrdenamiento.NombreZA => PropiedadesFiltradas.OrderByDescending(p => p.Nombre).ToList(),
                TipoOrdenamiento.DuenoAZ => PropiedadesFiltradas.OrderBy(p => p.DuenoNombre).ToList(),
                TipoOrdenamiento.DuenoZA => PropiedadesFiltradas.OrderByDescending(p => p.DuenoNombre).ToList(),
                TipoOrdenamiento.BalanceMayorMenor => PropiedadesFiltradas.OrderByDescending(p => p.Balance).ToList(),
                TipoOrdenamiento.BalanceMenorMayor => PropiedadesFiltradas.OrderBy(p => p.Balance).ToList(),
                _ => PropiedadesFiltradas.OrderBy(p => p.Nombre).ToList()
            };

            PropiedadesFiltradas = new ObservableCollection<Propiedad>(ordenadas);
        }

        private async void CerrarSesion()
        {
            var result = MessageBox.Show("¿Estás seguro que deseas cerrar sesión?", 
                "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await SupabaseAuthHelper.SignOutAsync();
                var loginWindow = new LoginWindow();
                NavigationService.NavigateTo(loginWindow, _window);
            }
        }

        private async void AbrirCasa(object? parameter)
        {
            if (parameter is Propiedad propiedad)
            {
                // Buscar la casa completa para pasar al detalle
                var casa = await SupabaseCasaHelper.ObtenerCasaPorIdAsync(propiedad.Id);
                
                if (casa != null)
                {
                    // Ocultar el menú principal mientras se muestra el detalle de la casa
                    _window.Hide();
                    
                    var detalleCasaWindow = new DetalleCasaWindow(casa);
                    detalleCasaWindow.Owner = _window;
                    detalleCasaWindow.ShowDialog();
                    
                    // Mostrar el menú principal nuevamente
                    _window.Show();
                    
                    // Recargar casas por si hubo cambios
                    await CargarCasasAsync();
                }
                else
                {
                    MessageBox.Show("Error al cargar casa", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void AbrirNotas(object? parameter)
        {
            if (parameter is Propiedad propiedad)
            {
                var notasPopup = new NotasPopup(
                    propiedad.Id,
                    propiedad.Nombre,
                    propiedad.DuenoNombre,
                    propiedad.Notas
                );

                notasPopup.Owner = _window;
                
                if (notasPopup.ShowDialog() == true)
                {
                    // Recargar las casas para reflejar los cambios
                    await CargarCasasAsync();
                }
            }
        }

        private void AbrirHistorial(object? parameter)
        {
            _window.Hide();
            var historialWindow = new HistorialWindow();
            historialWindow.Owner = _window;
            historialWindow.ShowDialog();
            _window.Show();
        }

        private void AbrirInactivas(object? parameter)
        {
            var inactivasWindow = new InactivasWindow();
            inactivasWindow.Owner = _window;
            inactivasWindow.ShowDialog();
            
            // Recargar casas por si algo cambió
            _ = CargarCasasAsync();
        }

        private void AbrirTutorial(object? parameter)
        {
            var tutorialWindow = new TutorialWindow();
            tutorialWindow.Owner = _window;
            tutorialWindow.ShowDialog();
        }
    }
}
