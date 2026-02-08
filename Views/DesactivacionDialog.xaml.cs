using System.Windows;

namespace FlujoCajaWpf.Views
{
    public partial class DesactivacionDialog : Window
    {
        public string Motivo { get; set; } = string.Empty;

        public DesactivacionDialog(string propiedadNombre)
        {
            InitializeComponent();
            
            // Crear un DataContext simple
            var dataContext = new { PropiedadNombre = propiedadNombre };
            DataContext = dataContext;
        }

        private void CancelarButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ConfirmarButton_Click(object sender, RoutedEventArgs e)
        {
            Motivo = MotivoTextBox.Text ?? string.Empty;
            DialogResult = true;
            Close();
        }
    }
}
