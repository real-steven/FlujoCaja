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
            {
                viewModel.Password = txtPassword.Password;
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext is LoginViewModel viewModel)
            {
                if (viewModel.IniciarSesionCommand.CanExecute(null))
                {
                    viewModel.IniciarSesionCommand.Execute(null);
                }
            }
        }
    }
}
