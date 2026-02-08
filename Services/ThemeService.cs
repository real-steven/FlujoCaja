using System.Windows;
using System.Windows.Media;

namespace FlujoCajaWpf.Services
{
    /// <summary>
    /// Servicio para gestionar el cambio de tema (claro/oscuro) en todo elúltimo aplicación
    /// </summary>
    public static class ThemeService
    {
        private static bool _modoOscuro = false;

        public static bool ModoOscuro
        {
            get => _modoOscuro;
            set
            {
                _modoOscuro = value;
                AplicarTema();
            }
        }

        /// <summary>
        /// Aplica el tema actual a todos los recursos de la aplicación
        /// </summary>
        private static void AplicarTema()
        {
            var app = Application.Current;
            
            if (_modoOscuro)
            {
                // Colores modo oscuro
                app.Resources["BackgroundBrush"] = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                app.Resources["CardBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(45, 45, 45));
                app.Resources["TextPrimaryBrush"] = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                app.Resources["TextSecondaryBrush"] = new SolidColorBrush(Color.FromRgb(180, 180, 180));
                app.Resources["DarkBlueBrush"] = new SolidColorBrush(Color.FromRgb(96, 165, 250)); // Azul más claro
                app.Resources["BorderBrush"] = new SolidColorBrush(Color.FromRgb(60, 60, 60));
                app.Resources["InputBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(50, 50, 50));
                app.Resources["HoverBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(55, 55, 55));
            }
            else
            {
                // Colores modo claro (original)
                app.Resources["BackgroundBrush"] = new SolidColorBrush(Color.FromRgb(243, 244, 246));
                app.Resources["CardBackgroundBrush"] = new SolidColorBrush(Colors.White);
                app.Resources["TextPrimaryBrush"] = new SolidColorBrush(Colors.Black);
                app.Resources["TextSecondaryBrush"] = new SolidColorBrush(Color.FromRgb(107, 114, 128));
                app.Resources["DarkBlueBrush"] = new SolidColorBrush(Color.FromRgb(32, 35, 85));
                app.Resources["BorderBrush"] = new SolidColorBrush(Color.FromRgb(229, 231, 235));
                app.Resources["InputBackgroundBrush"] = new SolidColorBrush(Colors.White);
                app.Resources["HoverBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            }
        }

        /// <summary>
        /// Inicializa el tema de la aplicación
        /// </summary>
        public static void Inicializar(bool modoOscuro = false)
        {
            _modoOscuro = modoOscuro;
            AplicarTema();
        }
    }
}
