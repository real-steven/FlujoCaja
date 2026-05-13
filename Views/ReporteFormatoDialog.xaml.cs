using System.Windows;

namespace FlujoCajaWpf.Views
{
    public partial class ReporteFormatoDialog : Window
    {
        public enum FormatoReporte { PdfTabla, PdfFacturas }
        public FormatoReporte FormatoSeleccionado { get; private set; }

        public ReporteFormatoDialog(string casaNombre, string mesAnio)
        {
            InitializeComponent();
            txtSubtitulo.Text = $"{casaNombre} — {mesAnio}";
        }

        private void GenerarPdf_Click(object sender, RoutedEventArgs e)
        {
            FormatoSeleccionado = FormatoReporte.PdfTabla;
            DialogResult = true;
            Close();
        }

        private void GenerarPdfFacturas_Click(object sender, RoutedEventArgs e)
        {
            FormatoSeleccionado = FormatoReporte.PdfFacturas;
            DialogResult = true;
            Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
