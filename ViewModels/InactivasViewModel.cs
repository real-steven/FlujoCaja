using FlujoCajaWpf.Commands;
using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using FlujoCajaWpf.ViewModels.Base;
using FlujoCajaWpf.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace FlujoCajaWpf.ViewModels
{
    public class InactivasViewModel : ViewModelBase
    {
        private readonly Window _window;
        private ObservableCollection<Propiedad> _propiedades;
        private ObservableCollection<Propiedad> _propiedadesFiltradas;
        private string _textoBusqueda = string.Empty;
        private bool _estaCargando = true;

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

        public ICommand AbrirCasaCommand { get; }
        public ICommand AbrirDialogoReactivarCommand { get; }

        public InactivasViewModel(Window window)
        {
            _window = window;
            _propiedades = new ObservableCollection<Propiedad>();
            _propiedadesFiltradas = new ObservableCollection<Propiedad>();

            AbrirCasaCommand = new RelayCommand(AbrirCasa);
            AbrirDialogoReactivarCommand = new RelayCommand(AbrirDialogoReactivar);

            _ = CargarCasasAsync();
        }

        private async Task CargarCasasAsync()
        {
            try
            {
                EstaCargando = true;

                var casas = await SupabaseCasaHelper.ObtenerTodasCasasSinFiltroAsync();
                
                Propiedades.Clear();
                PropiedadesFiltradas.Clear();

                // Filtrar solo casas inactivas
                foreach (var casa in casas.Where(c => !c.Activo))
                {
                    var propiedad = Propiedad.FromCasa(casa);
                    Propiedades.Add(propiedad);
                    PropiedadesFiltradas.Add(propiedad);
                }

                Console.WriteLine($"✓ {Propiedades.Count} propiedades inactivas cargadas en UI");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar casas inactivas: {ex.Message}", "Error", 
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

        private void AbrirDialogoReactivar(object? parameter)
        {
            if (Propiedades.Count == 0)
            {
                MessageBox.Show("No hay propiedades inactivas para reactivar.", 
                    "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new ReactivacionDialog(Propiedades.ToList());
            dialog.Owner = _window;

            if (dialog.ShowDialog() == true && dialog.PropiedadSeleccionada != null)
            {
                _ = ReactivarCasaAsync(dialog.PropiedadSeleccionada, dialog.Motivo);
            }
        }

        private async Task ReactivarCasaAsync(Propiedad propiedad, string motivo)
        {
            try
            {
                // Obtener la casa completa
                var casa = await SupabaseCasaHelper.ObtenerCasaPorIdAsync(propiedad.Id);
                if (casa != null)
                {
                    // Reactivar la casa
                    casa.Activo = true;
                    var casaSupabase = casa.ToSupabase();
                    await SupabaseCasaHelper.ActualizarCasaAsync(casaSupabase);
                    
                    MessageBox.Show($"Propiedad '{propiedad.Nombre}' reactivada exitosamente.", 
                        "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Recargar la lista
                    await CargarCasasAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al reactivar casa: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
