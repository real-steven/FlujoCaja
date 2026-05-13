using FlujoCajaWpf.Models;
using FlujoCajaWpf.ViewModels;
using System.Windows;

namespace FlujoCajaWpf.Views
{
    public partial class EnviarReporteDialog : Window
    {
        private readonly EnviarReporteViewModel _vm;

        /// <summary>True si el correo fue enviado exitosamente antes de cerrar el diálogo.</summary>
        public bool EnvioFueExitoso => _vm.EnvioFueExitoso;

        /// <summary>Lista de destinatarios a los que se envió el correo, separados por coma.</summary>
        public string DestinatariosEnviados => _vm.DestinatariosEnviados;

        /// <summary>
        /// Abre el diálogo de envío de reporte.
        /// </summary>
        /// <param name="casa">Casa para la que se envía el reporte.</param>
        /// <param name="rutaPdfTabla">Ruta al PDF de tabla de movimientos, o null si no existe.</param>
        /// <param name="rutaPdfImagenes">Ruta al PDF de imágenes/facturas, o null si no existe.</param>
        public EnviarReporteDialog(Casa casa, string? rutaPdfTabla = null, string? rutaPdfImagenes = null)
        {
            InitializeComponent();

            _vm = new EnviarReporteViewModel(
                casa,
                rutaPdfTabla,
                rutaPdfImagenes,
                cerrarDialogo: () => Close());

            DataContext = _vm;
            txtSubtitulo.Text = casa.Nombre;

            // Interceptar el comando Enviar para mostrar confirmación
            _vm.ConfirmarEnvio = MostrarConfirmacion;
        }

        private bool MostrarConfirmacion(string destinatarios)
        {
            var msgBox = new CustomMessageBox(
                mensaje: $"¿Deseas enviar el reporte a los siguientes destinatarios?\n\n{destinatarios}",
                titulo: "Confirmar envío",
                tipo: CustomMessageBox.MessageBoxType.Question,
                botones: CustomMessageBox.MessageBoxButtons.YesNo)
            {
                Owner = this
            };
            msgBox.ShowDialog();
            return msgBox.DialogResultValue == true;
        }
    }
}
