using FlujoCajaWpf.Data;
using System.Windows;
using System.Windows.Input;
using FlujoCajaWpf.ViewModels;

namespace FlujoCajaWpf.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel(this);
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
                viewModel.Password = txtPassword.Password;
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext is LoginViewModel viewModel)
            {
                if (viewModel.IniciarSesionCommand.CanExecute(null))
                    viewModel.IniciarSesionCommand.Execute(null);
            }
        }

        private async void OlvidePassword_Click(object sender, RoutedEventArgs e)
        {
            var email = txtEmail.Text.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                CustomMessageBox.Show("Ingresa tu correo electrónico primero.",
                    "Campo requerido", CustomMessageBox.MessageBoxType.Warning, CustomMessageBox.MessageBoxButtons.OK);
                return;
            }

            var confirm = CustomMessageBox.Show(
                $"Se enviará una contraseña temporal a:\n{email}\n\n¿Continuar?",
                "Restablecer contraseña",
                CustomMessageBox.MessageBoxType.Question,
                CustomMessageBox.MessageBoxButtons.YesNo);
            if (confirm != true) return;

            lnkOlvidePassword.IsEnabled = false;
            var (ok, error) = await SupabaseAdminHelper.EnviarPasswordTemporalPorCorreoAsync(email);
            lnkOlvidePassword.IsEnabled = true;

            if (ok)
                CustomMessageBox.Show(
                    "Se envió una contraseña temporal a tu correo.\nÚsala para iniciar sesión y cámbiala después.",
                    "Correo enviado", CustomMessageBox.MessageBoxType.Success, CustomMessageBox.MessageBoxButtons.OK);
            else
                CustomMessageBox.Show($"No se pudo enviar el correo:\n{error}",
                    "Error", CustomMessageBox.MessageBoxType.Error, CustomMessageBox.MessageBoxButtons.OK);
        }

        private async void Soporte_Click(object sender, RoutedEventArgs e)
        {
            btnSoporte.IsEnabled = false;
            try
            {
                var admin = await SupabaseAdminHelper.ObtenerAdminSoporteAsync();

                if (admin == null)
                {
                    CustomMessageBox.Show(
                        "No se encontró un administrador registrado en el sistema.",
                        "Soporte", CustomMessageBox.MessageBoxType.Warning, CustomMessageBox.MessageBoxButtons.OK);
                    return;
                }

                var telefono = string.IsNullOrWhiteSpace(admin.Telefono) ? "No registrado" : admin.Telefono;
                var mensaje = $"Para soporte técnico contacta al administrador:\n\n" +
                              $"👤  {admin.NombreCompleto}\n" +
                              $"📧  {admin.Email}\n" +
                              $"📱  {telefono}";

                CustomMessageBox.Show(mensaje, "Contacto de Soporte",
                    CustomMessageBox.MessageBoxType.Info, CustomMessageBox.MessageBoxButtons.OK);
            }
            finally
            {
                btnSoporte.IsEnabled = true;
            }
        }
    }
}
