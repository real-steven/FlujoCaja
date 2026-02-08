using FlujoCajaWpf.Commands;
using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using FlujoCajaWpf.Services;
using FlujoCajaWpf.ViewModels.Base;
using FlujoCajaWpf.Views;
using System.Windows;
using System.Windows.Input;

namespace FlujoCajaWpf.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly Window _window;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _mensajeError = string.Empty;
        private bool _estaCargando;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string MensajeError
        {
            get => _mensajeError;
            set
            {
                SetProperty(ref _mensajeError, value);
                OnPropertyChanged(nameof(TieneMensajeError));
            }
        }

        public bool TieneMensajeError => !string.IsNullOrEmpty(MensajeError);

        public bool EstaCargando
        {
            get => _estaCargando;
            set => SetProperty(ref _estaCargando, value);
        }

        public ICommand IniciarSesionCommand { get; }

        public LoginViewModel(Window window)
        {
            _window = window;
            
            // Cargar email guardado si existe
            if (!string.IsNullOrEmpty(Properties.Settings.Default.SavedEmail))
            {
                Email = Properties.Settings.Default.SavedEmail;
            }
            
            IniciarSesionCommand = new RelayCommand(
                async () => await IniciarSesionAsync(),
                () => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password) && !EstaCargando
            );
        }

        private async Task IniciarSesionAsync()
        {
            try
            {
                EstaCargando = true;
                MensajeError = string.Empty;

                // Validar formato de email
                if (!Email.Contains("@"))
                {
                    MensajeError = "Email inválido";
                    return;
                }

                // Intentar login con Supabase
                var (success, usuario, error) = await SupabaseAuthHelper.SignInAsync(Email, Password);

                if (success && usuario != null)
                {
                    // Guardar email para futuros inicios de sesión
                    Properties.Settings.Default.SavedEmail = Email;
                    Properties.Settings.Default.Save();
                    
                    // Login exitoso - abrir menú principal
                    var menuWindow = new MenuPrincipalWindow(usuario);
                    NavigationService.NavigateTo(menuWindow, _window);
                }
                else
                {
                    // Mensaje amigable para credenciales inválidas
                    MensajeError = "Credenciales inválidas";
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error: {ex.Message}";
            }
            finally
            {
                EstaCargando = false;
                (IniciarSesionCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }
}
