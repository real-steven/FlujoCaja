using System.Windows;

namespace FlujoCajaWpf.Services
{
    /// <summary>
    /// Servicio para gestionar la navegación entre ventanas
    /// </summary>
    public static class NavigationService
    {
        /// <summary>
        /// Abre una ventana y cierra la ventana actual
        /// </summary>
        public static void NavigateTo(Window newWindow, Window? currentWindow = null)
        {
            newWindow.Show();
            currentWindow?.Close();
        }

        /// <summary>
        /// Abre una ventana modal (diálogo)
        /// </summary>
        public static bool? ShowDialog(Window dialog)
        {
            return dialog.ShowDialog();
        }

        /// <summary>
        /// Cierra todas las ventanas y abre una nueva (útil para logout)
        /// </summary>
        public static void NavigateToAndCloseAll(Window newWindow)
        {
            var windows = Application.Current.Windows.Cast<Window>().ToList();
            newWindow.Show();
            
            foreach (var window in windows)
            {
                if (window != newWindow)
                    window.Close();
            }
        }
    }
}
