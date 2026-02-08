using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using FlujoCajaWpf.Views;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FlujoCajaWpf.Views.Controls
{
    public partial class GestionCategoriasMovimientosControl : UserControl
    {
        private List<CategoriaMovimientoViewModel> todasLasCategorias = new List<CategoriaMovimientoViewModel>();
        
        public GestionCategoriasMovimientosControl()
        {
            InitializeComponent();
            Loaded += async (s, e) => await CargarCategoriasAsync();
        }

        private async Task CargarCategoriasAsync()
        {
            try
            {
                var resultado = await SupabaseCategoriaMovimientoHelper.ObtenerCategoriasMovimientosAsync();
                
                if (resultado.Success)
                {
                    var categorias = resultado.Data ?? new List<CategoriaMovimientoSupabase>();
                    
                    // Crear ViewModels para mostrar el tipo formateado
                    todasLasCategorias = categorias.Select(c => new CategoriaMovimientoViewModel
                    {
                        Categoria = c,
                        Nombre = c.Nombre,
                        TipoTexto = c.Tipo.Equals("Ingreso", StringComparison.OrdinalIgnoreCase) ? "Ingreso" : "Gasto",
                        Descripcion = c.Descripcion ?? string.Empty
                    }).ToList();
                    
                    dgCategorias.ItemsSource = todasLasCategorias;
                    ActualizarContador(todasLasCategorias.Count);
                    txtNoData.Visibility = todasLasCategorias.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    CustomMessageBox.Show(
                        $"Error al cargar categorías: {resultado.Error}",
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

        private void BuscarCategorias_TextChanged(object sender, TextChangedEventArgs e)
        {
            var busqueda = txtBuscar.Text.ToLower();
            
            if (string.IsNullOrWhiteSpace(busqueda))
            {
                dgCategorias.ItemsSource = todasLasCategorias;
                ActualizarContador(todasLasCategorias.Count);
            }
            else
            {
                var filtradas = todasLasCategorias.Where(c =>
                    c.Nombre.ToLower().Contains(busqueda) ||
                    c.TipoTexto.ToLower().Contains(busqueda) ||
                    c.Descripcion.ToLower().Contains(busqueda)
                ).ToList();
                
                dgCategorias.ItemsSource = filtradas;
                ActualizarContador(filtradas.Count);
                txtNoData.Visibility = filtradas.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private async void Recargar_Click(object sender, RoutedEventArgs e)
        {
            loadingOverlay.Visibility = Visibility.Visible;
            txtBuscar.Clear();
            await CargarCategoriasAsync();
            await Task.Delay(300);
            loadingOverlay.Visibility = Visibility.Collapsed;
        }
        private async void NuevaCategoriaMovimiento_Click(object sender, RoutedEventArgs e)
        {
            var ventanaAgregar = new AgregarCategoriaMovimientoWindow();
            if (ventanaAgregar.ShowDialog() == true)
            {
                await CargarCategoriasAsync();
            }
        }
        private async void EditarCategoria_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var categoriaViewModel = button?.Tag as CategoriaMovimientoViewModel;
            
            if (categoriaViewModel?.Categoria == null) return;

            var ventanaEditar = new EditarCategoriaMovimientoWindow(categoriaViewModel.Categoria);
            if (ventanaEditar.ShowDialog() == true)
            {
                await CargarCategoriasAsync();
            }
        }

        private async void EliminarCategoria_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var categoriaViewModel = button?.Tag as CategoriaMovimientoViewModel;
            
            if (categoriaViewModel == null) return;

            var resultado = CustomMessageBox.Show(
                $"¿Está seguro que desea eliminar la categoría '{categoriaViewModel.Nombre}'?\n\nEsta acción no se puede deshacer.",
                "Confirmar Eliminación",
                CustomMessageBox.MessageBoxType.Question,
                CustomMessageBox.MessageBoxButtons.YesNo);

            if (resultado == true)
            {
                var eliminado = await SupabaseCategoriaMovimientoHelper.EliminarCategoriaMovimientoAsync(categoriaViewModel.Categoria.Id);
                
                if (eliminado.Success)
                {
                    CustomMessageBox.Show(
                        $"Categoría '{categoriaViewModel.Nombre}' eliminada exitosamente",
                        "Éxito",
                        CustomMessageBox.MessageBoxType.Success,
                        CustomMessageBox.MessageBoxButtons.OK);
                    
                    await CargarCategoriasAsync();
                }
                else
                {
                    CustomMessageBox.Show(
                        $"Error al eliminar categoría: {eliminado.Error}",
                        "Error",
                        CustomMessageBox.MessageBoxType.Error,
                        CustomMessageBox.MessageBoxButtons.OK);
                }
            }
        }

        private void ActualizarContador(int cantidad)
        {
            txtContador.Text = cantidad == 1 
                ? "1 categoría encontrada" 
                : $"{cantidad} categorías encontradas";
        }
    }

    // Clase auxiliar para mostrar datos formateados
    public class CategoriaMovimientoViewModel
    {
        public CategoriaMovimientoSupabase Categoria { get; set; } = new CategoriaMovimientoSupabase();
        public string Nombre { get; set; } = string.Empty;
        public string TipoTexto { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
