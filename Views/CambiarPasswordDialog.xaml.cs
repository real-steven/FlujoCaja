using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using System.Windows;

namespace FlujoCajaWpf.Views
{
    public partial class CambiarPasswordDialog : Window
    {
        private readonly string _email;

        public CambiarPasswordDialog(string email)
        {
            InitializeComponent();
            _email = email;
        }

        private async void Cambiar_Click(object sender, RoutedEventArgs e)
        {
            var actual    = pbActual.Password;
            var nueva     = pbNueva.Password;
            var confirmar = pbConfirmar.Password;

            if (string.IsNullOrWhiteSpace(actual) || string.IsNullOrWhiteSpace(nueva) || string.IsNullOrWhiteSpace(confirmar))
            {
                MostrarError("Completa todos los campos.");
                return;
            }

            if (nueva != confirmar)
            {
                MostrarError("La nueva contraseña y la confirmación no coinciden.");
                return;
            }

            if (nueva.Length < 6)
            {
                MostrarError("La nueva contraseña debe tener al menos 6 caracteres.");
                return;
            }

            btnCambiar.IsEnabled = false;
            btnCambiar.Content = "Cambiando...";
            txtError.Visibility = Visibility.Collapsed;

            var (ok, error) = await SupabaseAuthHelper.CambiarPasswordAsync(_email, actual, nueva);

            btnCambiar.IsEnabled = true;
            btnCambiar.Content = "Cambiar contraseña";

            if (ok)
            {
                CustomMessageBox.Show(
                    "¡Contraseña cambiada exitosamente!",
                    "Listo", CustomMessageBox.MessageBoxType.Success, CustomMessageBox.MessageBoxButtons.OK);
                DialogResult = true;
                Close();
            }
            else
            {
                MostrarError(error ?? "Error desconocido al cambiar la contraseña.");
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void MostrarError(string mensaje)
        {
            txtError.Text = mensaje;
            txtError.Visibility = Visibility.Visible;
        }
    }
}
