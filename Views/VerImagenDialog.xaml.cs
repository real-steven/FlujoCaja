using Microsoft.Win32;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FlujoCajaWpf.Views
{
    public partial class VerImagenDialog : Window
    {
        private static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png", ".webp" };

        public string? RutaImagenSeleccionada { get; private set; }
        public bool QuiereEliminar { get; private set; }

        private string? _imagenUrl;

        public VerImagenDialog(string imagenUrl)
        {
            InitializeComponent();
            _imagenUrl = imagenUrl;
            Loaded += async (s, e) => await CargarImagenActualAsync(imagenUrl);
        }

        private async Task CargarImagenActualAsync(string url)
        {
            try
            {
                txtCargando.Visibility = Visibility.Visible;
                imgActual.Visibility = Visibility.Collapsed;

                using var httpClient = new HttpClient();
                var imageBytes = await httpClient.GetByteArrayAsync(url);

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(imageBytes);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                imgActual.Source = bitmap;
                imgActual.Visibility = Visibility.Visible;
                txtCargando.Visibility = Visibility.Collapsed;
            }
            catch
            {
                txtCargando.Text = "No se pudo cargar la imagen";
                txtCargando.Visibility = Visibility.Visible;
                imgActual.Visibility = Visibility.Collapsed;
            }
        }

        // ==================== DRAG & DROP REEMPLAZAR ====================

        private void BorderReplace_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var archivos = (string[])e.Data.GetData(DataFormats.FileDrop);
                var ext = Path.GetExtension(archivos[0]).ToLowerInvariant();
                if (ExtensionesPermitidas.Contains(ext))
                {
                    e.Effects = DragDropEffects.Copy;
                    borderDropReplace.BorderBrush = System.Windows.Media.Brushes.DodgerBlue;
                    borderDropReplace.Background = System.Windows.Media.Brushes.AliceBlue;
                    e.Handled = true;
                    return;
                }
            }
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void BorderReplace_DragLeave(object sender, DragEventArgs e)
        {
            ResetBorderStyle();
        }

        private void BorderReplace_Drop(object sender, DragEventArgs e)
        {
            ResetBorderStyle();
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            var archivos = (string[])e.Data.GetData(DataFormats.FileDrop);
            var ruta = archivos[0];
            var ext = Path.GetExtension(ruta).ToLowerInvariant();

            if (!ExtensionesPermitidas.Contains(ext))
            {
                CustomMessageBox.Show("Solo se permiten PNG, JPG, JPEG o WEBP.", "Formato no válido",
                    CustomMessageBox.MessageBoxType.Warning, CustomMessageBox.MessageBoxButtons.OK);
                return;
            }
            AplicarNuevaImagen(ruta);
        }

        private void BorderReplace_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AbrirSelectorArchivo();
        }

        private void CambiarNueva_Click(object sender, RoutedEventArgs e)
        {
            AbrirSelectorArchivo();
        }

        private void AbrirSelectorArchivo()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Seleccionar imagen de reemplazo",
                Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.webp|Todos los archivos|*.*"
            };
            if (dialog.ShowDialog() == true)
                AplicarNuevaImagen(dialog.FileName);
        }

        private void AplicarNuevaImagen(string ruta)
        {
            try
            {
                RutaImagenSeleccionada = ruta;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(ruta);
                bitmap.DecodePixelWidth = 130;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                imgNueva.Source = bitmap;
                txtNombreNueva.Text = Path.GetFileName(ruta);

                panelReplaceVacio.Visibility = Visibility.Collapsed;
                panelReplaceSeleccionado.Visibility = Visibility.Visible;
                btnGuardar.IsEnabled = true;
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"No se pudo cargar la imagen: {ex.Message}", "Error",
                    CustomMessageBox.MessageBoxType.Error, CustomMessageBox.MessageBoxButtons.OK);
            }
        }

        private void ResetBorderStyle()
        {
            borderDropReplace.BorderBrush = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CBD5E1"));
            borderDropReplace.Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F8FAFC"));
        }

        // ==================== BOTONES ====================

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void Descargar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_imagenUrl)) return;

            var saveDialog = new SaveFileDialog
            {
                Title = "Guardar imagen",
                Filter = "Imagen|*.jpg;*.jpeg;*.png;*.webp|Todos los archivos|*.*",
                FileName = $"factura_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (saveDialog.ShowDialog() != true) return;

            try
            {
                using var httpClient = new HttpClient();
                var bytes = await httpClient.GetByteArrayAsync(_imagenUrl);
                await File.WriteAllBytesAsync(saveDialog.FileName, bytes);
                CustomMessageBox.Show("Imagen descargada correctamente.", "Éxito",
                    CustomMessageBox.MessageBoxType.Success, CustomMessageBox.MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error al descargar: {ex.Message}", "Error",
                    CustomMessageBox.MessageBoxType.Error, CustomMessageBox.MessageBoxButtons.OK);
            }
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            var confirm = CustomMessageBox.Show(
                "¿Estás seguro de que quieres eliminar la imagen adjunta?",
                "Confirmar eliminación",
                CustomMessageBox.MessageBoxType.Warning,
                CustomMessageBox.MessageBoxButtons.YesNo);

            if (confirm == true)
            {
                QuiereEliminar = true;
                DialogResult = true;
                Close();
            }
        }
    }
}
