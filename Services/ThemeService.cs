using System.Windows;
using System.Windows.Media;
using FlujoCajaWpf.Properties;

namespace FlujoCajaWpf.Services
{
    /// <summary>
    /// Servicio para gestionar el cambio de tema (claro/oscuro) en toda la aplicación
    /// </summary>
    public static class ThemeService
    {
        public static bool ModoOscuro
        {
            get => Settings.Default.ModoOscuro;
            set
            {
                Settings.Default.ModoOscuro = value;
                Settings.Default.Save();
                AplicarTema();
            }
        }

        /// <summary>
        /// Aplica el tema guardado al iniciar la aplicación
        /// </summary>
        public static void CargarTema()
        {
            AplicarTema();
        }

        /// <summary>
        /// Aplica el tema actual a todos los recursos de la aplicación
        /// </summary>
        private static void AplicarTema()
        {
            var app = Application.Current;
            
            if (ModoOscuro)
            {
                // Colores modo oscuro - Fondo negro, cards gris claro
                app.Resources["BackgroundBrush"] = new SolidColorBrush(Color.FromRgb(0, 0, 0)); // Negro
                app.Resources["CardBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(55, 55, 55)); // Gris claro
                app.Resources["TextPrimaryBrush"] = new SolidColorBrush(Color.FromRgb(255, 255, 255)); // Blanco puro
                app.Resources["TextSecondaryBrush"] = new SolidColorBrush(Color.FromRgb(200, 200, 200));
                app.Resources["DarkBlueBrush"] = new SolidColorBrush(Color.FromRgb(96, 165, 250)); // Azul más claro
                app.Resources["BorderBrush"] = new SolidColorBrush(Color.FromRgb(70, 70, 70));
                app.Resources["InputBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(45, 45, 45));
                app.Resources["HoverBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(65, 65, 65));
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
            
            // Forzar actualización de todas las ventanas abiertas
            RefrescarVentanas();
        }
        
        /// <summary>
        /// Fuerza el refresh visual de todas las ventanas abiertas
        /// </summary>
        private static void RefrescarVentanas()
        {
            foreach (Window ventana in Application.Current.Windows)
            {
                ventana.InvalidateVisual();
                ventana.UpdateLayout();
            }
        }
    }
}
