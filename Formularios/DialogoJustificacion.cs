using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlujoDeCajaApp.Formularios
{
    /// <summary>
    /// Diálogo para solicitar justificación de acciones de edición o eliminación
    /// </summary>
    public partial class DialogoJustificacion : Form
    {
        #region Propiedades

        /// <summary>
        /// Obtiene la justificación ingresada por el usuario
        /// </summary>
        public string Justificacion { get; private set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece el título de la acción
        /// </summary>
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string TituloAccion { get; set; } = "Acción";

        /// <summary>
        /// Obtiene o establece el mensaje descriptivo de la acción
        /// </summary>
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string MensajeAccion { get; set; } = "Ingrese la justificación para esta acción:";

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor del diálogo de justificación
        /// </summary>
        public DialogoJustificacion()
        {
            InitializeComponent();
            ConfigurarDialogo();
        }

        /// <summary>
        /// Constructor con parámetros personalizados
        /// </summary>
        /// <param name="titulo">Título del diálogo</param>
        /// <param name="mensaje">Mensaje descriptivo</param>
        /// <param name="placeholderJustificacion">Texto de ayuda para el campo de justificación</param>
        public DialogoJustificacion(string titulo, string mensaje, string placeholderJustificacion = "Escriba la justificación aquí...")
        {
            InitializeComponent();
            TituloAccion = titulo;
            MensajeAccion = mensaje;
            ConfigurarDialogo();
            txtJustificacion.PlaceholderText = placeholderJustificacion;
        }

        #endregion

        #region Métodos privados

        /// <summary>
        /// Configura la apariencia y comportamiento del diálogo
        /// </summary>
        private void ConfigurarDialogo()
        {
            this.Text = TituloAccion;
            lblMensaje.Text = MensajeAccion;
            
            // Configurar eventos
            btnAceptar.Click += BtnAceptar_Click;
            btnCancelar.Click += BtnCancelar_Click;
            
            // Configurar teclas de acceso rápido
            this.AcceptButton = btnAceptar;
            this.CancelButton = btnCancelar;
            
            // Enfocar el campo de texto
            txtJustificacion.Focus();
        }

        /// <summary>
        /// Maneja el evento click del botón Aceptar
        /// </summary>
        private void BtnAceptar_Click(object? sender, EventArgs e)
        {
            // Validar que se haya ingresado una justificación
            if (string.IsNullOrWhiteSpace(txtJustificacion.Text))
            {
                MessageBox.Show(
                    "Debe ingresar una justificación para continuar.",
                    "Justificación requerida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                
                txtJustificacion.Focus();
                return;
            }

            // Guardar la justificación y cerrar el diálogo
            Justificacion = txtJustificacion.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Maneja el evento click del botón Cancelar
        /// </summary>
        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Métodos estáticos de conveniencia

        /// <summary>
        /// Muestra un diálogo para justificar una edición
        /// </summary>
        /// <param name="parent">Ventana padre</param>
        /// <param name="descripcionMovimiento">Descripción del movimiento a editar</param>
        /// <returns>La justificación ingresada, o null si se canceló</returns>
        public static string? SolicitarJustificacionEdicion(IWin32Window? parent, string descripcionMovimiento)
        {
            using var dialogo = new DialogoJustificacion(
                "Justificar Edición",
                $"¿Por qué desea editar el movimiento '{descripcionMovimiento}'?",
                "Ej: Corrección de monto, cambio de categoría, etc."
            );

            return dialogo.ShowDialog(parent) == DialogResult.OK ? dialogo.Justificacion : null;
        }

        /// <summary>
        /// Muestra un diálogo para justificar una eliminación
        /// </summary>
        /// <param name="parent">Ventana padre</param>
        /// <param name="descripcionMovimiento">Descripción del movimiento a eliminar</param>
        /// <returns>La justificación ingresada, o null si se canceló</returns>
        public static string? SolicitarJustificacionEliminacion(IWin32Window? parent, string descripcionMovimiento)
        {
            using var dialogo = new DialogoJustificacion(
                "Justificar Eliminación",
                $"¿Por qué desea eliminar el movimiento '{descripcionMovimiento}'?",
                "Ej: Movimiento duplicado, registro incorrecto, etc."
            );

            return dialogo.ShowDialog(parent) == DialogResult.OK ? dialogo.Justificacion : null;
        }

        #endregion
    }
}