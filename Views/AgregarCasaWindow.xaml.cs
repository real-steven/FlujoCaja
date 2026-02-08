using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace FlujoCajaWpf.Views
{
    public partial class AgregarCasaWindow : Window
    {
        private List<DuenoSupabase> duenos = new List<DuenoSupabase>();
        private List<CategoriaSupabase> categorias = new List<CategoriaSupabase>();
        private ObservableCollection<string> fotosSeleccionadas = new ObservableCollection<string>();

        public AgregarCasaWindow()
        {
            InitializeComponent();
            lstFotos.ItemsSource = fotosSeleccionadas;
            CargarComboBoxes();
        }

        private async void CargarComboBoxes()
        {
            try
            {
                // Cargar dueños
                var resultadoDuenos = await SupabaseDuenoHelper.ObtenerDuenosAsync();
                if (resultadoDuenos.Success)
                {
                    duenos = resultadoDuenos.Data ?? new List<DuenoSupabase>();
                    cmbDueno.ItemsSource = duenos;
                }

                // Cargar categorías
                var resultadoCategorias = await SupabaseCategoriaHelper.ObtenerCategoriasAsync();
                if (resultadoCategorias.Success)
                {
                    categorias = resultadoCategorias.Data ?? new List<CategoriaSupabase>();
                    cmbCategoria.ItemsSource = categorias;
                }

                // Seleccionar valores por defecto
                chkActiva.IsChecked = true;
                
                // Seleccionar USD como moneda por defecto
                foreach (ComboBoxItem item in cmbMoneda.Items)
                {
                    if (item.Content.ToString() == "USD")
                    {
                        cmbMoneda.SelectedItem = item;
                        break;
                    }
                }
                
                if (cmbMoneda.SelectedItem == null && cmbMoneda.Items.Count > 0)
                {
                    cmbMoneda.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(
                    $"Error al cargar datos: {ex.Message}",
                    "Error",
                    CustomMessageBox.MessageBoxType.Error,
                    CustomMessageBox.MessageBoxButtons.OK);
            }
        }

        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    CustomMessageBox.Show(
                        "El nombre de la casa es requerido",
                        "Validación",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK);
                    txtNombre.Focus();
                    return;
                }

                if (cmbDueno.SelectedItem == null)
                {
                    CustomMessageBox.Show(
                        "Debe seleccionar un dueño",
                        "Validación",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK);
                    cmbDueno.Focus();
                    return;
                }

                if (cmbCategoria.SelectedItem == null)
                {
                    CustomMessageBox.Show(
                        "Debe seleccionar una categoría de propiedad",
                        "Validación",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK);
                    cmbCategoria.Focus();
                    return;
                }

                if (cmbMoneda.SelectedItem == null)
                {
                    CustomMessageBox.Show(
                        "Debe seleccionar una moneda",
                        "Validación",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK);
                    cmbMoneda.Focus();
                    return;
                }

                // Crear nuevo objeto CasaSupabase
                var nuevaCasa = new CasaSupabase
                {
                    Nombre = txtNombre.Text.Trim(),
                    DuenoId = (cmbDueno.SelectedItem as DuenoSupabase)!.Id,
                    CategoriaId = (cmbCategoria.SelectedItem as CategoriaSupabase)!.Id,
                    Moneda = (cmbMoneda.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "USD",
                    Activo = chkActiva.IsChecked ?? true,
                    Notas = string.IsNullOrWhiteSpace(txtNotas.Text) ? null : txtNotas.Text.Trim()
                };

                // Llamar al helper para insertar
                var resultado = await SupabaseCasaHelper.InsertarCasaAsync(nuevaCasa);

                if (resultado.Success)
                {
                    // 📊 REGISTRAR EN HISTORIAL
                    var user = SupabaseAuthHelper.GetCurrentUser();
                    await SupabaseAuditoriaHelper.RegistrarAccionAsync(
                        user?.Email ?? "desconocido",
                        "casa",
                        "crear",
                        null,
                        nuevaCasa.Nombre,
                        $"Creó nueva casa: {nuevaCasa.Nombre}",
                        datosNuevos: new {
                            nombre = nuevaCasa.Nombre,
                            duenoId = nuevaCasa.DuenoId,
                            categoriaId = nuevaCasa.CategoriaId,
                            moneda = nuevaCasa.Moneda,
                            activo = nuevaCasa.Activo
                        }
                    );
                    
                    CustomMessageBox.Show(
                        "Casa agregada exitosamente",
                        "Éxito",
                        CustomMessageBox.MessageBoxType.Success,
                        CustomMessageBox.MessageBoxButtons.OK);

                    DialogResult = true;
                    Close();
                }
                else
                {
                    CustomMessageBox.Show(
                        $"Error al agregar casa: {resultado.Error}",
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

        private void Limpiar_Click(object sender, RoutedEventArgs e)
        {
            // Limpiar todos los campos
            txtNombre.Clear();
            cmbDueno.SelectedIndex = -1;
            cmbCategoria.SelectedIndex = -1;
            cmbMoneda.SelectedIndex = 0; // USD por defecto
            chkActiva.IsChecked = true;
            txtNotas.Clear();
            fotosSeleccionadas.Clear();
            
            // Enfocar el primer campo
            txtNombre.Focus();
        }

        private void SeleccionarFotos_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.gif;*.bmp",
                Title = "Seleccionar Fotos de la Casa"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    if (!fotosSeleccionadas.Contains(fileName))
                    {
                        fotosSeleccionadas.Add(fileName);
                    }
                }
            }
        }

        private void DropZone_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

                foreach (var file in files)
                {
                    var extension = Path.GetExtension(file).ToLower();
                    if (imageExtensions.Contains(extension) && !fotosSeleccionadas.Contains(file))
                    {
                        fotosSeleccionadas.Add(file);
                    }
                }
            }

            dropZone.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D5DB"));
            dropZone.Background = new SolidColorBrush(Colors.White);
        }

        private void DropZone_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                dropZone.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"));
                dropZone.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EFF6FF"));
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void DropZone_DragLeave(object sender, DragEventArgs e)
        {
            dropZone.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D5DB"));
            dropZone.Background = new SolidColorBrush(Colors.White);
        }

        private void DropZone_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            dropZone.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"));
        }

        private void DropZone_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            dropZone.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D5DB"));
        }

        private void EliminarFoto_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is string fileName)
            {
                fotosSeleccionadas.Remove(fileName);
            }
        }
    }
}
