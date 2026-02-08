using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FlujoCajaWpf.Views.Controls
{
    public partial class GestionDuenosControl : UserControl
    {
        private List<DuenoSupabase> todosLosDuenos = new List<DuenoSupabase>();
        
        public GestionDuenosControl()
        {
            InitializeComponent();
            Loaded += async (s, e) => await CargarDuenosAsync();
        }

        private async Task CargarDuenosAsync()
        {
            try
            {
                var resultado = await SupabaseDuenoHelper.ObtenerDuenosAsync();
                
                if (resultado.Success)
                {
                    todosLosDuenos = resultado.Data ?? new List<DuenoSupabase>();
                    dgDuenos.ItemsSource = todosLosDuenos;
                    ActualizarContador(todosLosDuenos.Count);
                    txtNoData.Visibility = todosLosDuenos.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    CustomMessageBox.Show(
                        $"Error al cargar dueños: {resultado.Error}",
                        "Error",
                        CustomMessageBox.MessageBoxType.Error,
                        CustomMessageBox.MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(
                    $"Error inesperado: {ex.Message}",
                    "Error",
                    CustomMessageBox.MessageBoxType.Error,
                    CustomMessageBox.MessageBoxButtons.OK);
            }
        }

        private void BuscarDuenos_TextChanged(object sender, TextChangedEventArgs e)
        {
            var busqueda = txtBuscar.Text.ToLower();
            
            if (string.IsNullOrWhiteSpace(busqueda))
            {
                dgDuenos.ItemsSource = todosLosDuenos;
                ActualizarContador(todosLosDuenos.Count);
            }
            else
            {
                var filtrados = todosLosDuenos.Where(d =>
                    d.NombreCompleto.ToLower().Contains(busqueda) ||
                    (d.Cedula != null && d.Cedula.ToLower().Contains(busqueda)) ||
                    (d.Email != null && d.Email.ToLower().Contains(busqueda)) ||
                    (d.Telefono != null && d.Telefono.ToLower().Contains(busqueda))
                ).ToList();
                
                dgDuenos.ItemsSource = filtrados;
                ActualizarContador(filtrados.Count);
                txtNoData.Visibility = filtrados.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private async void Recargar_Click(object sender, RoutedEventArgs e)
        {
            loadingOverlay.Visibility = Visibility.Visible;
            txtBuscar.Clear();
            await CargarDuenosAsync();
            await Task.Delay(300); // Pequeña pausa para que se vea la animación
            loadingOverlay.Visibility = Visibility.Collapsed;
        }

        private async void NuevoDueno_Click(object sender, RoutedEventArgs e)
        {
            var agregarWindow = new AgregarDuenoWindow();
            agregarWindow.Owner = Window.GetWindow(this);
            
            if (agregarWindow.ShowDialog() == true)
            {
                // Recargar la lista después de agregar
                await CargarDuenosAsync();
            }
        }

        private async void EditarDueno_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var dueno = button?.Tag as DuenoSupabase;
            
            if (dueno == null) return;

            var editarWindow = new EditarDuenoWindow(dueno);
            editarWindow.Owner = Window.GetWindow(this);
            
            if (editarWindow.ShowDialog() == true)
            {
                await CargarDuenosAsync();
            }
        }

        private async void EliminarDueno_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var dueno = button?.Tag as DuenoSupabase;
            
            if (dueno == null) return;

            var resultado = CustomMessageBox.Show(
                $"¿Está seguro que desea eliminar al dueño '{dueno.NombreCompleto}'?\n\nEsta acción no se puede deshacer.",
                "Confirmar Eliminación",
                CustomMessageBox.MessageBoxType.Question,
                CustomMessageBox.MessageBoxButtons.YesNo);

            if (resultado == true)
            {
                var eliminado = await SupabaseDuenoHelper.EliminarDuenoAsync(dueno.Id);
                
                if (eliminado.Success)
                {
                    CustomMessageBox.Show(
                        $"Dueño '{dueno.NombreCompleto}' eliminado exitosamente",
                        "Éxito",
                        CustomMessageBox.MessageBoxType.Success,
                        CustomMessageBox.MessageBoxButtons.OK);
                    
                    await CargarDuenosAsync();
                }
                else
                {
                    CustomMessageBox.Show(
                        $"Error al eliminar dueño: {eliminado.Error}",
                        "Error",
                        CustomMessageBox.MessageBoxType.Error,
                        CustomMessageBox.MessageBoxButtons.OK);
                }
            }
        }

        private void ActualizarContador(int cantidad)
        {
            txtContador.Text = cantidad == 1 
                ? "1 dueño encontrado" 
                : $"{cantidad} dueños encontrados";
        }
    }
}
