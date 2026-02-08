using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FlujoCajaWpf.Views.Controls
{
    public partial class GestionCasasControl : UserControl
    {
        private List<CasaViewModel> todasLasCasas = new List<CasaViewModel>();
        
        public GestionCasasControl()
        {
            InitializeComponent();
            Loaded += async (s, e) => await CargarCasasAsync();
        }

        private async Task CargarCasasAsync()
        {
            try
            {
                var resultado = await SupabaseCasaHelper.ObtenerCasasAsync();
                
                if (resultado.Success)
                {
                    var casas = resultado.Data ?? new List<Casa>();
                    
                    // Cargar dueños y categorías para mostrar nombres
                    var resultadoDuenos = await SupabaseDuenoHelper.ObtenerDuenosAsync();
                    var resultadoCategorias = await SupabaseCategoriaHelper.ObtenerCategoriasAsync();
                    
                    var duenos = resultadoDuenos.Success ? resultadoDuenos.Data : new List<DuenoSupabase>();
                    var categorias = resultadoCategorias.Success ? resultadoCategorias.Data : new List<CategoriaSupabase>();
                    
                    // Crear ViewModels para mostrar en el grid
                    todasLasCasas = casas.Select(c => new CasaViewModel
                    {
                        Casa = c,
                        Nombre = c.Nombre,
                        DuenoNombre = duenos?.FirstOrDefault(d => d.Id == c.DuenoId)?.NombreCompleto ?? "Desconocido",
                        CategoriaNombre = categorias?.FirstOrDefault(cat => cat.Id == c.CategoriaId)?.Nombre ?? "Desconocida",
                        Moneda = c.Moneda,
                        EstadoTexto = c.Activo ? "Activa" : "Inactiva"
                    }).ToList();
                    
                    dgCasas.ItemsSource = todasLasCasas;
                    ActualizarContador(todasLasCasas.Count);
                    txtNoData.Visibility = todasLasCasas.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    CustomMessageBox.Show(
                        $"Error al cargar casas: {resultado.Error}",
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

        private void BuscarCasas_TextChanged(object sender, TextChangedEventArgs e)
        {
            var busqueda = txtBuscar.Text.ToLower();
            
            if (string.IsNullOrWhiteSpace(busqueda))
            {
                dgCasas.ItemsSource = todasLasCasas;
                ActualizarContador(todasLasCasas.Count);
            }
            else
            {
                var filtradas = todasLasCasas.Where(c =>
                    c.Nombre.ToLower().Contains(busqueda)
                ).ToList();
                
                dgCasas.ItemsSource = filtradas;
                ActualizarContador(filtradas.Count);
                txtNoData.Visibility = filtradas.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private async void Recargar_Click(object sender, RoutedEventArgs e)
        {
            loadingOverlay.Visibility = Visibility.Visible;
            txtBuscar.Clear();
            await CargarCasasAsync();
            await Task.Delay(300);
            loadingOverlay.Visibility = Visibility.Collapsed;
        }

        private async void NuevaCasa_Click(object sender, RoutedEventArgs e)
        {
            var agregarWindow = new AgregarCasaWindow();
            agregarWindow.Owner = Window.GetWindow(this);
            
            if (agregarWindow.ShowDialog() == true)
            {
                // Recargar la lista después de agregar
                await CargarCasasAsync();
            }
        }

        private async void EditarCasa_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var casaViewModel = button?.Tag as CasaViewModel;
            
            if (casaViewModel == null) return;

            // Convertir Casa a CasaSupabase para la edición
            var casaSupabase = new CasaSupabase
            {
                Id = casaViewModel.Casa.Id,
                Nombre = casaViewModel.Casa.Nombre,
                DuenoId = casaViewModel.Casa.DuenoId,
                CategoriaId = casaViewModel.Casa.CategoriaId,
                Moneda = casaViewModel.Casa.Moneda,
                Activo = casaViewModel.Casa.Activo,
                Notas = casaViewModel.Casa.Notas
            };

            var editarWindow = new EditarCasaWindow(casaSupabase);
            editarWindow.Owner = Window.GetWindow(this);
            
            if (editarWindow.ShowDialog() == true)
            {
                await CargarCasasAsync();
            }
        }

        private async void EliminarCasa_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var casaViewModel = button?.Tag as CasaViewModel;
            
            if (casaViewModel == null) return;

            var resultado = CustomMessageBox.Show(
                $"¿Está seguro que desea eliminar la casa '{casaViewModel.Nombre}'?\n\nEsta acción no se puede deshacer.",
                "Confirmar Eliminación",
                CustomMessageBox.MessageBoxType.Question,
                CustomMessageBox.MessageBoxButtons.YesNo);

            if (resultado == true)
            {
                var eliminado = await SupabaseCasaHelper.EliminarCasaAsync(casaViewModel.Casa.Id);
                
                if (eliminado.Success)
                {
                    // 📊 REGISTRAR EN HISTORIAL
                    var user = SupabaseAuthHelper.GetCurrentUser();
                    await SupabaseAuditoriaHelper.RegistrarAccionAsync(
                        user?.Email ?? "desconocido",
                        "casa",
                        "eliminar",
                        casaViewModel.Casa.Id,
                        casaViewModel.Nombre,
                        $"Eliminó casa: {casaViewModel.Nombre}"
                    );
                    
                    CustomMessageBox.Show(
                        $"Casa '{casaViewModel.Nombre}' eliminada exitosamente",
                        "Éxito",
                        CustomMessageBox.MessageBoxType.Success,
                        CustomMessageBox.MessageBoxButtons.OK);
                    
                    await CargarCasasAsync();
                }
                else
                {
                    CustomMessageBox.Show(
                        $"Error al eliminar casa: {eliminado.Error}",
                        "Error",
                        CustomMessageBox.MessageBoxType.Error,
                        CustomMessageBox.MessageBoxButtons.OK);
                }
            }
        }

        private void ActualizarContador(int cantidad)
        {
            txtContador.Text = cantidad == 1 
                ? "1 casa encontrada" 
                : $"{cantidad} casas encontradas";
        }
    }

    // Clase auxiliar para mostrar datos en el DataGrid
    public class CasaViewModel
    {
        public Casa Casa { get; set; } = new Casa();
        public string Nombre { get; set; } = string.Empty;
        public string DuenoNombre { get; set; } = string.Empty;
        public string CategoriaNombre { get; set; } = string.Empty;
        public string Moneda { get; set; } = string.Empty;
        public string EstadoTexto { get; set; } = string.Empty;
    }
}
