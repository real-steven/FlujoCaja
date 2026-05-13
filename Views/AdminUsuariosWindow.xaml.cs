using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FlujoCajaWpf.Views
{
    public partial class AdminUsuariosWindow : Window
    {
        private UsuarioSupabase? _usuarioSeleccionado;
        private readonly string _adminEmail;

        public AdminUsuariosWindow(string adminEmail)
        {
            InitializeComponent();
            _adminEmail = adminEmail;
            Loaded += async (s, e) => await CargarUsuariosAsync();
        }

        private async Task CargarUsuariosAsync()
        {
            MostrarLoading(true);
            txtStatus.Text = "Cargando usuarios...";
            try
            {
                var usuarios = await SupabaseAdminHelper.ObtenerUsuariosAsync();
                dgUsuarios.ItemsSource = usuarios;
                txtStatus.Text = $"{usuarios.Count} usuario(s) registrado(s).";
            }
            catch (Exception ex)
            {
                txtStatus.Text = $"Error al cargar: {ex.Message}";
            }
            finally
            {
                MostrarLoading(false);
            }
        }

        private void dgUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _usuarioSeleccionado = dgUsuarios.SelectedItem as UsuarioSupabase;
            var seleccionado = _usuarioSeleccionado != null;

            btnCambiarRol.IsEnabled = seleccionado;
            // Solo habilitar cambio de correo para usuarios con rol "usuario" (no admins)
            btnCambiarEmail.IsEnabled = seleccionado && _usuarioSeleccionado?.Rol == "usuario";
            btnDesactivar.IsEnabled = seleccionado;

            if (_usuarioSeleccionado != null)
            {
                btnDesactivar.Content = _usuarioSeleccionado.Activo ? "⛔ Desactivar" : "✅ Activar";
            }
        }

        private async void NuevoUsuario_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CrearUsuarioDialog { Owner = this };
            if (dlg.ShowDialog() != true) return;

            MostrarLoading(true);
            txtStatus.Text = "Creando usuario...";
            try
            {
                var (ok, error) = await SupabaseAdminHelper.CrearUsuarioAsync(
                    dlg.Email, dlg.Password, dlg.Nombre, dlg.Apellido, dlg.Rol, dlg.Telefono);

                if (ok)
                {
                    txtStatus.Text = $"Usuario {dlg.Email} creado exitosamente.";
                    await CargarUsuariosAsync();
                }
                else
                {
                    var esCorreoDuplicado = error != null && (
                        error.Contains("email_exists") ||
                        error.Contains("already registered") ||
                        error.Contains("already been registered") ||
                        error.Contains("already exists") ||
                        error.Contains("duplicate") ||
                        error.Contains("unique"));
                    var mensajeAmigable = esCorreoDuplicado
                        ? "El correo ingresado ya está registrado en el sistema."
                        : $"No se pudo crear el usuario:\n{error}";
                    txtStatus.Text = esCorreoDuplicado ? "Error: Correo ya registrado." : $"Error: {error}";
                    CustomMessageBox.Show(mensajeAmigable, "Error al crear usuario",
                        CustomMessageBox.MessageBoxType.Error, CustomMessageBox.MessageBoxButtons.OK);
                }
            }
            finally
            {
                MostrarLoading(false);
            }
        }

        private async void CambiarRol_Click(object sender, RoutedEventArgs e)
        {
            if (_usuarioSeleccionado == null) return;

            // No permitir cambiar el rol del propio admin
            if (_usuarioSeleccionado.Email == _adminEmail)
            {
                CustomMessageBox.Show("No puedes cambiar tu propio rol.", "Aviso",
                    CustomMessageBox.MessageBoxType.Warning, CustomMessageBox.MessageBoxButtons.OK);
                return;
            }

            var nuevoRol = _usuarioSeleccionado.Rol == "admin" ? "usuario" : "admin";
            var msgBox = new CustomMessageBox(
                $"¿Cambiar el rol de {_usuarioSeleccionado.Email} a '{nuevoRol}'?",
                "Confirmar cambio de rol",
                CustomMessageBox.MessageBoxType.Question,
                CustomMessageBox.MessageBoxButtons.YesNo)
            { Owner = this };
            msgBox.ShowDialog();
            if (msgBox.DialogResultValue != true) return;

            MostrarLoading(true);
            txtStatus.Text = "Cambiando rol...";
            var (ok, error) = await SupabaseAdminHelper.CambiarRolAsync(_usuarioSeleccionado.Id, nuevoRol);
            MostrarLoading(false);

            if (ok)
            {
                txtStatus.Text = $"Rol actualizado a '{nuevoRol}' para {_usuarioSeleccionado.Email}.";
                await CargarUsuariosAsync();
            }
            else
            {
                txtStatus.Text = $"Error: {error}";
                CustomMessageBox.Show($"Error al cambiar rol:\n{error}", "Error",
                    CustomMessageBox.MessageBoxType.Error, CustomMessageBox.MessageBoxButtons.OK);
            }
        }

        private async void CambiarEmail_Click(object sender, RoutedEventArgs e)
        {
            if (_usuarioSeleccionado == null) return;

            // Doble-check de seguridad: nunca operar sobre administradores
            if (_usuarioSeleccionado.Rol == "admin")
            {
                CustomMessageBox.Show("No puedes cambiar el correo de otro administrador.", "Sin permiso",
                    CustomMessageBox.MessageBoxType.Warning, CustomMessageBox.MessageBoxButtons.OK);
                return;
            }

            if (string.IsNullOrWhiteSpace(_usuarioSeleccionado.AuthId))
            {
                CustomMessageBox.Show("Este usuario no tiene ID de autenticación registrado y no se puede actualizar su correo.", "Error",
                    CustomMessageBox.MessageBoxType.Error, CustomMessageBox.MessageBoxButtons.OK);
                return;
            }

            // ── Mini-dialog de entrada ─────────────────────────────────────────
            string? nuevoEmail = null;

            var txtNuevo   = new TextBox { Text = _usuarioSeleccionado.Email, Padding = new Thickness(8, 6, 8, 6), FontSize = 13, Margin = new Thickness(0, 0, 0, 4) };
            var txtRepetir = new TextBox { Padding = new Thickness(8, 6, 8, 6), FontSize = 13, Margin = new Thickness(0, 0, 0, 4) };
            var txtErr     = new TextBlock { Foreground = Brushes.Red, FontSize = 12, TextWrapping = TextWrapping.Wrap, Visibility = Visibility.Collapsed };
            var btnGuardar  = new Button { Content = "Guardar", IsDefault = true, Padding = new Thickness(20, 8, 20, 8), Background = new SolidColorBrush(Color.FromRgb(245, 158, 11)), Foreground = Brushes.White, BorderThickness = new Thickness(0) };
            var btnCancelar = new Button { Content = "Cancelar", IsCancel = true, Padding = new Thickness(20, 8, 20, 8), Margin = new Thickness(10, 0, 0, 0) };

            var dlg = new Window
            {
                Title  = "Cambiar correo electrónico",
                Width  = 440, Height = 310,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner  = this,
                ResizeMode = ResizeMode.NoResize,
                Background = (Brush)Application.Current.Resources["BackgroundBrush"]
            };

            btnGuardar.Click += (s2, args) =>
            {
                var correoNuevo    = txtNuevo.Text.Trim();
                var correoRepetido = txtRepetir.Text.Trim();
                if (string.IsNullOrWhiteSpace(correoNuevo) || !correoNuevo.Contains('@'))
                {
                    txtErr.Text = "Ingresa un correo electrónico válido.";
                    txtErr.Visibility = Visibility.Visible;
                    return;
                }
                if (correoNuevo != correoRepetido)
                {
                    txtErr.Text = "Los correos no coinciden.";
                    txtErr.Visibility = Visibility.Visible;
                    return;
                }
                nuevoEmail = correoNuevo;
                dlg.DialogResult = true;
            };

            var btnRow = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 12, 0, 0) };
            btnRow.Children.Add(btnGuardar);
            btnRow.Children.Add(btnCancelar);

            var panel = new StackPanel { Margin = new Thickness(24) };
            panel.Children.Add(new TextBlock { Text = $"Usuario: {_usuarioSeleccionado.NombreCompleto} ({_usuarioSeleccionado.Email})", FontSize = 13, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 0, 0, 14), Foreground = (Brush)Application.Current.Resources["TextPrimaryBrush"], TextWrapping = TextWrapping.Wrap });
            panel.Children.Add(new TextBlock { Text = "Nuevo correo electrónico", FontSize = 12, Margin = new Thickness(0, 0, 0, 4), Foreground = (Brush)Application.Current.Resources["TextPrimaryBrush"] });
            panel.Children.Add(txtNuevo);
            panel.Children.Add(new TextBlock { Text = "Confirmar nuevo correo", FontSize = 12, Margin = new Thickness(0, 8, 0, 4), Foreground = (Brush)Application.Current.Resources["TextPrimaryBrush"] });
            panel.Children.Add(txtRepetir);
            panel.Children.Add(txtErr);
            panel.Children.Add(btnRow);
            dlg.Content = panel;

            if (dlg.ShowDialog() != true || nuevoEmail == null) return;

            if (nuevoEmail == _usuarioSeleccionado.Email)
            {
                CustomMessageBox.Show("El correo nuevo es igual al actual. Sin cambios.", "Sin cambios",
                    CustomMessageBox.MessageBoxType.Warning, CustomMessageBox.MessageBoxButtons.OK);
                return;
            }

            MostrarLoading(true);
            txtStatus.Text = "Actualizando correo...";
            var (ok, error) = await SupabaseAdminHelper.CambiarEmailAsync(
                _usuarioSeleccionado.Id, _usuarioSeleccionado.AuthId, nuevoEmail);
            MostrarLoading(false);

            if (ok)
            {
                txtStatus.Text = $"Correo de {_usuarioSeleccionado.NombreCompleto} actualizado correctamente.";
                await CargarUsuariosAsync();
            }
            else
            {
                txtStatus.Text = $"Error: {error}";
                CustomMessageBox.Show($"Error al cambiar correo:\n{error}", "Error",
                    CustomMessageBox.MessageBoxType.Error, CustomMessageBox.MessageBoxButtons.OK);
            }
        }

        private async void ToggleActivo_Click(object sender, RoutedEventArgs e)
        {
            if (_usuarioSeleccionado == null) return;

            if (_usuarioSeleccionado.Email == _adminEmail)
            {
                CustomMessageBox.Show("No puedes desactivarte a ti mismo.", "Aviso",
                    CustomMessageBox.MessageBoxType.Warning, CustomMessageBox.MessageBoxButtons.OK);
                return;
            }

            var accion = _usuarioSeleccionado.Activo ? "desactivar" : "activar";
            var msgBox = new CustomMessageBox(
                $"¿Deseas {accion} al usuario {_usuarioSeleccionado.Email}?",
                $"Confirmar {accion}",
                CustomMessageBox.MessageBoxType.Question,
                CustomMessageBox.MessageBoxButtons.YesNo)
            { Owner = this };
            msgBox.ShowDialog();
            if (msgBox.DialogResultValue != true) return;

            MostrarLoading(true);
            txtStatus.Text = $"{accion}ando usuario...";
            var (ok, error) = await SupabaseAdminHelper.CambiarEstadoAsync(
                _usuarioSeleccionado.Id, !_usuarioSeleccionado.Activo);
            MostrarLoading(false);

            if (ok)
            {
                txtStatus.Text = $"Usuario {_usuarioSeleccionado.Email} {accion}do.";
                await CargarUsuariosAsync();
            }
            else
            {
                txtStatus.Text = $"Error: {error}";
                CustomMessageBox.Show($"Error al {accion} usuario:\n{error}", "Error",
                    CustomMessageBox.MessageBoxType.Error, CustomMessageBox.MessageBoxButtons.OK);
            }
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e) => Close();

        private void MostrarLoading(bool mostrar)
        {
            overlayLoading.Visibility = mostrar ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
