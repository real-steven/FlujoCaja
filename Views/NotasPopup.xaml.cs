using FlujoCajaWpf.ViewModels;
using System.Windows;

namespace FlujoCajaWpf.Views
{
    public partial class NotasPopup : Window
    {
        private readonly NotasViewModel _viewModel;

        public NotasPopup(int casaId, string nombreCasa, string duenoPrincipal, string? notasActuales)
        {
            InitializeComponent();
            
            _viewModel = new NotasViewModel(casaId, nombreCasa, duenoPrincipal, notasActuales);
            DataContext = _viewModel;
        }

        private void CerrarButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CancelarButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void GuardarButton_Click(object sender, RoutedEventArgs e)
        {
            var resultado = await _viewModel.GuardarNotasAsync();
            
            if (resultado.Success)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show(resultado.Error ?? "Error al guardar las notas", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
