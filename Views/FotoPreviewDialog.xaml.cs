using FlujoCajaWpf.Data;
using Microsoft.Win32;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FlujoCajaWpf.Views
{
    public partial class FotoPreviewDialog : Window
    {
        private readonly string _storagePath;
        private byte[]? _imageBytes;
        private readonly string _fileName;

        public FotoPreviewDialog(string storagePath)
        {
            InitializeComponent();
            _storagePath = storagePath;
            _fileName = Path.GetFileName(storagePath);
            txtNombreArchivo.Text = _fileName;
            Loaded += async (_, _) => await CargarImagenAsync();
        }

        private async Task CargarImagenAsync()
        {
            try
            {
                txtCargando.Text = "⏳ Obteniendo URL segura…";

                var signedUrl = await SupabaseStorageHelper.ObtenerUrlFirmadaMovimientoAsync(_storagePath, 3600);
                if (string.IsNullOrEmpty(signedUrl))
                {
                    txtCargando.Text = "❌ No se pudo obtener la URL de la imagen.";
                    return;
                }

                txtCargando.Text = "⏳ Descargando imagen…";

                using var http = new HttpClient();
                _imageBytes = await http.GetByteArrayAsync(signedUrl);

                var bitmap = new BitmapImage();
                using var ms = new MemoryStream(_imageBytes);
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();

                imgPreview.Source = bitmap;
                gridCargando.Visibility = Visibility.Collapsed;
                btnDescargar.IsEnabled = true;
            }
            catch (Exception ex)
            {
                txtCargando.Text = $"❌ Error al cargar: {ex.Message}";
            }
        }

        private void Descargar_Click(object sender, RoutedEventArgs e)
        {
            if (_imageBytes == null) return;

            var ext = Path.GetExtension(_fileName);
            var dlg = new SaveFileDialog
            {
                FileName = _fileName,
                DefaultExt = ext,
                Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.webp;*.pdf|Todos los archivos|*.*",
                Title = "Guardar factura"
            };

            if (dlg.ShowDialog() == true)
            {
                File.WriteAllBytes(dlg.FileName, _imageBytes);
            }
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e) => Close();
    }
}
