using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FlujoCajaWpf.Views
{
    public partial class AdjuntarImagenDialog : Window
    {
        private static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png", ".webp" };

        public string? RutaImagenSeleccionada { get; private set; }

        public AdjuntarImagenDialog()
        {
            InitializeComponent();
        }

        // ==================== DRAG & DROP ====================

        private void Border_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var archivos = (string[])e.Data.GetData(DataFormats.FileDrop);
                var ext = Path.GetExtension(archivos[0]).ToLowerInvariant();
                if (ExtensionesPermitidas.Contains(ext))
                {
                    e.Effects = DragDropEffects.Copy;
                    borderDrop.BorderBrush = System.Windows.Media.Brushes.DodgerBlue;
                    borderDrop.Background = System.Windows.Media.Brushes.AliceBlue;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Border_DragLeave(object sender, DragEventArgs e)
        {
            borderDrop.BorderBrush = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CBD5E1"));
            borderDrop.Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F8FAFC"));
        }

        private void Border_Drop(object sender, DragEventArgs e)
        {
            Border_DragLeave(sender, e);
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            var archivos = (string[])e.Data.GetData(DataFormats.FileDrop);
            var ruta = archivos[0];
            var ext = Path.GetExtension(ruta).ToLowerInvariant();

            if (!ExtensionesPermitidas.Contains(ext))
            {
                CustomMessageBox.Show(
                    "Solo se permiten archivos PNG, JPG, JPEG o WEBP.",
                    "Formato no válido",
                    CustomMessageBox.MessageBoxType.Warning,
                    CustomMessageBox.MessageBoxButtons.OK);
                return;
            }

            AplicarImagenSeleccionada(ruta);
        }

        // ==================== CLIC PARA BUSCAR ====================

        private void Border_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AbrirDialogoSeleccion();
        }

        private void CambiarImagen_Click(object sender, RoutedEventArgs e)
        {
            AbrirDialogoSeleccion();
        }

        private void AbrirDialogoSeleccion()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Seleccionar imagen",
                Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.webp|Todos los archivos|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                AplicarImagenSeleccionada(dialog.FileName);
            }
        }

        // ==================== HELPERS ====================

        private void AplicarImagenSeleccionada(string ruta)
        {
            try
            {
                RutaImagenSeleccionada = ruta;

                // Thumbnail
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(ruta);
                bitmap.DecodePixelWidth = 160;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                imgPreview.Source = bitmap;

                // Metadata
                var info = new FileInfo(ruta);
                txtNombreArchivo.Text = info.Name;
                txtTamanioArchivo.Text = $"{info.Length / 1024.0:F1} KB";

                // Mostrar panel con imagen
                panelVacio.Visibility = Visibility.Collapsed;
                panelSeleccionado.Visibility = Visibility.Visible;
                btnAdjuntar.IsEnabled = true;
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(
                    $"No se pudo cargar la imagen: {ex.Message}",
                    "Error",
                    CustomMessageBox.MessageBoxType.Error,
                    CustomMessageBox.MessageBoxButtons.OK);
            }
        }

        // ==================== BOTONES ====================

        private void Adjuntar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
