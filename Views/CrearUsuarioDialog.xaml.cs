using FlujoCajaWpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace FlujoCajaWpf.Views
{
    public partial class CrearUsuarioDialog : Window
    {
        public string Email { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public string Nombre { get; private set; } = string.Empty;
        public string Apellido { get; private set; } = string.Empty;
        public string Rol { get; private set; } = "usuario";
        public string Telefono { get; private set; } = string.Empty;

        public CrearUsuarioDialog()
        {
            InitializeComponent();
        }

        private void Crear_Click(object sender, RoutedEventArgs e)
        {
            var email = txtEmail.Text.Trim();
            var password = txtPassword.Password;
            var nombre = txtNombre.Text.Trim();
            var apellido = txtApellido.Text.Trim();
            var rol = (cmbRol.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "usuario";

            if (string.IsNullOrWhiteSpace(email))
            {
                MostrarError("El email es obligatorio.");
                return;
            }

            if (!EmailService.EsEmailValido(email))
            {
                MostrarError("El formato del email no es válido.");
                return;
            }

            if (password.Length < 6)
            {
                MostrarError("La contraseña debe tener al menos 6 caracteres.");
                return;
            }

            Email = email;
            Password = password;
            Nombre = nombre;
            Apellido = apellido;
            Rol = rol;
            Telefono = txtTelefono.Text.Trim();

            DialogResult = true;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void MostrarError(string mensaje)
        {
            txtError.Text = mensaje;
            txtError.Visibility = Visibility.Visible;
        }
    }
}
