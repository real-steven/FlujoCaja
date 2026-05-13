using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FlujoCajaWpf.Views
{
    public partial class PerfilUsuarioDialog : Window
    {
        private readonly Usuario _usuario;
        private UsuarioSupabase? _perfil;
        private byte[]? _nuevaFotoBytes;
        private string? _nuevaFotoExtension;

        public PerfilUsuarioDialog(Usuario usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            Loaded += async (s, e) => await CargarPerfilAsync();
        }

        private async Task CargarPerfilAsync()
        {
            _perfil = await SupabaseAuthHelper.ObtenerPerfilCompletoAsync(_usuario.Id);
            if (_perfil == null) return;

            txtNombre.Text   = _perfil.Nombre ?? "";
            txtApellido.Text = _perfil.Apellido ?? "";
            txtEmail.Text    = _perfil.Email;
            txtTelefono.Text = _perfil.Telefono ?? "";
            txtRol.Text      = _perfil.Rol;
            txtRolBadge.Text = _perfil.Rol == "admin" ? "🛡 Admin" : "👤 Usuario";
            txtMiembroDesde.Text = $"Estado: {(_perfil.Activo ? "Activo" : "Inactivo")}";

            ActualizarIniciales();

            if (!string.IsNullOrWhiteSpace(_perfil.FotoPerfil))
                await CargarImagenUrlAsync(_perfil.FotoPerfil);
        }

        private void ActualizarIniciales()
        {
            var nombre   = txtNombre.Text.Trim();
            var apellido = txtApellido.Text.Trim();
            var iniciales = "";
            if (nombre.Length > 0)   iniciales += nombre[0];
            if (apellido.Length > 0) iniciales += apellido[0];
            txtIniciales.Text = iniciales.Length > 0 ? iniciales.ToUpper() : "?";
        }

        private async Task CargarImagenUrlAsync(string url)
        {
            try
            {
                using var http = new System.Net.Http.HttpClient();
                var bytes = await http.GetByteArrayAsync(url);
                MostrarImagenBytes(bytes);
            }
            catch { /* si falla la carga, se muestran las iniciales */ }
        }

        private void MostrarImagenBytes(byte[] bytes)
        {
            var ms = new MemoryStream(bytes);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            imgAvatarBrush.ImageSource = bitmap;
            elipseAvatar.Visibility = Visibility.Visible;
            txtIniciales.Visibility = Visibility.Collapsed;
        }

        private void CambiarFoto_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Seleccionar foto de perfil",
                Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.webp",
                Multiselect = false
            };
            if (dlg.ShowDialog() != true) return;

            _nuevaFotoBytes     = File.ReadAllBytes(dlg.FileName);
            _nuevaFotoExtension = Path.GetExtension(dlg.FileName).ToLowerInvariant();
            MostrarImagenBytes(_nuevaFotoBytes);
        }

        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            var nombre   = txtNombre.Text.Trim();
            var apellido = txtApellido.Text.Trim();
            var telefono = txtTelefono.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MostrarError("El nombre no puede estar vacío.");
                return;
            }

            btnGuardar.IsEnabled = false;
            btnGuardar.Content = "Guardando...";
            txtError.Visibility = Visibility.Collapsed;

            string? urlFoto = _perfil?.FotoPerfil;

            // Subir nueva foto si se seleccionó una
            if (_nuevaFotoBytes != null)
            {
                var fileName = $"avatares/{_usuario.Id}{_nuevaFotoExtension}";
                var (ok, url, err) = await SupabaseStorageHelper.SubirAvatarAsync(_nuevaFotoBytes, fileName);
                if (ok && url != null)
                    urlFoto = url;
                else
                    MostrarError($"No se pudo subir la foto: {err}");
            }

            var (guardado, error) = await SupabaseAuthHelper.ActualizarPerfilAsync(
                _usuario.Id, nombre, apellido,
                string.IsNullOrWhiteSpace(telefono) ? null : telefono,
                urlFoto);

            btnGuardar.IsEnabled = true;
            btnGuardar.Content = "Guardar cambios";

            if (guardado)
            {
                CustomMessageBox.Show("Perfil actualizado correctamente.",
                    "Listo", CustomMessageBox.MessageBoxType.Success, CustomMessageBox.MessageBoxButtons.OK);
                _nuevaFotoBytes = null;
                await CargarPerfilAsync();
            }
            else
            {
                MostrarError(error ?? "Error al guardar el perfil.");
            }
        }

        private void CambiarPassword_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CambiarPasswordDialog(_usuario.Email) { Owner = this };
            dlg.ShowDialog();
        }

        private void MostrarError(string mensaje)
        {
            txtError.Text = mensaje;
            txtError.Visibility = Visibility.Visible;
        }
    }
}
