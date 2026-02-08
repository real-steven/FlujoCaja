using System.Windows;
using FlujoCajaWpf.Models;
using FlujoCajaWpf.ViewModels;

namespace FlujoCajaWpf.Views
{
    public partial class MenuPrincipalWindow : Window
    {
        public MenuPrincipalWindow(Usuario usuario)
        {
            InitializeComponent();
            DataContext = new MenuPrincipalViewModel(this, usuario);
        }

        private async void AbrirGestion_Click(object sender, RoutedEventArgs e)
        {
            var gestionWindow = new GestionWindow();
            gestionWindow.Owner = this;
            this.Hide(); // Ocultar MenuPrincipal
            gestionWindow.ShowDialog();
            this.Show(); // Mostrar MenuPrincipal al cerrar Gestión
            
            // Recargar casas después de cerrar la ventana de gestión
            if (DataContext is MenuPrincipalViewModel viewModel)
            {
                await viewModel.CargarCasasAsync();
            }
        }

        private void AbrirResumen_Click(object sender, RoutedEventArgs e)
        {
            var resumenWindow = new ResumenConsolidadoWindow();
            resumenWindow.Owner = this;
            resumenWindow.ShowDialog();
        }

        private void BtnOrdenar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MenuPrincipalViewModel viewModel)
            {
                viewModel.CambiarOrdenamiento();
            }
        }
    }
}
