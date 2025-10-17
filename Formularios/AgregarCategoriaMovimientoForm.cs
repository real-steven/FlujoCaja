using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FlujoDeCajaApp.Data;
using FlujoDeCajaApp.Modelos;

namespace FlujoDeCajaApp.Formularios
{
    /// <summary>
    /// Formulario para agregar nuevas categorías de movimiento al sistema
    /// </summary>
    public partial class AgregarCategoriaMovimientoForm : Form
    {
        #region Variables privadas

        private System.Windows.Forms.Timer timerMensajeExito = new();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor del formulario
        /// </summary>
        public AgregarCategoriaMovimientoForm()
        {
            InitializeComponent();
            InicializarFormulario();
        }

        #endregion

        #region Métodos de inicialización

        /// <summary>
        /// Inicializa los componentes del formulario
        /// </summary>
        private void InicializarFormulario()
        {
            try
            {
                // Configurar el icono del formulario
                ConfigurarIcono();

                // Configurar eventos
                btnGuardar.Click += BtnGuardar_Click;
                btnRegresar.Click += BtnRegresar_Click;

                // Configurar validación en tiempo real
                txtNombreCategoria.TextChanged += ValidarFormulario;

                // Configurar placeholder
                ConfigurarPlaceholder();

                // Configurar dropdown de tipo de entidad
                ConfigurarDropdownTipoEntidad();

                // Configurar timer para mensaje de éxito
                ConfigurarTimerMensajeExito();

                // Deshabilitar el botón guardar inicialmente
                btnGuardar.Enabled = false;

                // Configurar KeyPress para Enter
                this.KeyPreview = true;
                this.KeyPress += AgregarCategoriaMovimientoForm_KeyPress;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar el formulario: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Configura el icono del formulario
        /// </summary>
        private void ConfigurarIcono()
        {
            try
            {
                string rutaLogo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "LogoSamaraRental.PNG");
                if (File.Exists(rutaLogo))
                {
                    using (var imagen = Image.FromFile(rutaLogo))
                    {
                        this.Icon = Icon.FromHandle(((Bitmap)imagen).GetHicon());
                        pictureBoxLogo.Image = new Bitmap(imagen);
                    }
                }
            }
            catch (Exception ex)
            {
                // Si no se puede cargar el icono, continuar sin él
                Console.WriteLine($"No se pudo cargar el icono: {ex.Message}");
            }
        }

        /// <summary>
        /// Configura el placeholder del TextBox
        /// </summary>
        private void ConfigurarPlaceholder()
        {
            string placeholder = "Ingrese el nombre de la categoría de movimiento";
            txtNombreCategoria.Text = placeholder;
            txtNombreCategoria.ForeColor = Color.Gray;

            txtNombreCategoria.Enter += (sender, e) =>
            {
                if (txtNombreCategoria.Text == placeholder)
                {
                    txtNombreCategoria.Text = "";
                    txtNombreCategoria.ForeColor = Color.Black;
                }
            };

            txtNombreCategoria.Leave += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNombreCategoria.Text))
                {
                    txtNombreCategoria.Text = placeholder;
                    txtNombreCategoria.ForeColor = Color.Gray;
                }
            };
        }

        /// <summary>
        /// Configura el dropdown de tipo de entidad
        /// </summary>
        private void ConfigurarDropdownTipoEntidad()
        {
            cmbTipoEntidad.Items.Clear();
            cmbTipoEntidad.Items.Add("Categoría de Movimiento");
            cmbTipoEntidad.Items.Add("Categoría de Propiedad");
            
            // Seleccionar "Categoría de Movimiento" por defecto
            cmbTipoEntidad.SelectedIndex = 0;
            
            // Deshabilitar temporalmente - solo categoría de movimiento
            cmbTipoEntidad.Enabled = false;
        }

        /// <summary>
        /// Configura el timer para ocultar el mensaje de éxito
        /// </summary>
        private void ConfigurarTimerMensajeExito()
        {
            timerMensajeExito.Interval = 3000; // 3 segundos
            timerMensajeExito.Tick += (sender, e) =>
            {
                lblMensajeExito.Visible = false;
                timerMensajeExito.Stop();
            };
        }

        #endregion

        #region Eventos

        /// <summary>
        /// Maneja el evento KeyPress del formulario para detectar Enter
        /// </summary>
        private void AgregarCategoriaMovimientoForm_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && btnGuardar.Enabled)
            {
                BtnGuardar_Click(sender, EventArgs.Empty);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Maneja el evento click del botón Guardar
        /// </summary>
        private async void BtnGuardar_Click(object? sender, EventArgs e)
        {
            try
            {
                // Deshabilitar el botón para evitar clicks múltiples
                btnGuardar.Enabled = false;
                btnGuardar.Text = "Guardando...";
                
                if (!ValidarDatos())
                {
                    btnGuardar.Enabled = true;
                    btnGuardar.Text = "Guardar";
                    return;
                }
                
                // Obtener datos del formulario
                string nombreCategoria = ObtenerTextoLimpio(txtNombreCategoria);
                
                // Crear nueva categoría para Supabase (especificando que es de movimiento)
                var nuevaCategoriaMovimiento = new CategoriaMovimientoSupabase(nombreCategoria);
                
                // Guardar en Supabase
                bool guardadoExitoso = await SupabaseCategoriaMovimientoHelper.CrearCategoriaMovimiento(nuevaCategoriaMovimiento);
                
                if (guardadoExitoso)
                {
                    // Mostrar mensaje de éxito
                    MostrarMensajeExito("Categoría de movimiento agregada correctamente en Supabase");
                    
                    // Limpiar el formulario
                    LimpiarFormulario();
                    
                    // Cerrar el formulario con resultado OK
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Error al guardar la categoría de movimiento en Supabase. Intente nuevamente.", 
                        "Error de Guardado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
                btnGuardar.Enabled = false;
                btnGuardar.Text = "Guardar";
            }
            catch (Exception ex)
            {
                btnGuardar.Enabled = true;
                btnGuardar.Text = "Guardar";
                
                MessageBox.Show($"Error al guardar la categoría de movimiento: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Maneja el evento click del botón Regresar
        /// </summary>
        private void BtnRegresar_Click(object? sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al regresar: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Validación

        /// <summary>
        /// Valida el formulario y habilita/deshabilita el botón guardar
        /// </summary>
        private void ValidarFormulario(object? sender, EventArgs e)
        {
            bool esValido = ValidarDatos(false);
            btnGuardar.Enabled = esValido;
        }

        /// <summary>
        /// Valida los datos del formulario
        /// </summary>
        /// <param name="mostrarMensajes">Indica si se deben mostrar mensajes de error</param>
        /// <returns>True si los datos son válidos, False en caso contrario</returns>
        private bool ValidarDatos(bool mostrarMensajes = true)
        {
            // Validar nombre de categoría
            string nombreCategoria = ObtenerTextoLimpio(txtNombreCategoria);
            
            if (string.IsNullOrWhiteSpace(nombreCategoria))
            {
                if (mostrarMensajes)
                    MessageBox.Show("Debe ingresar el nombre de la categoría de movimiento.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validar longitud mínima
            if (nombreCategoria.Length < 2)
            {
                if (mostrarMensajes)
                    MessageBox.Show("El nombre de la categoría debe tener al menos 2 caracteres.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validar longitud máxima
            if (nombreCategoria.Length > 50)
            {
                if (mostrarMensajes)
                    MessageBox.Show("El nombre de la categoría no puede tener más de 50 caracteres.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        #endregion

        #region Métodos auxiliares

        /// <summary>
        /// Obtiene el texto limpio de un TextBox (sin placeholder)
        /// </summary>
        /// <param name="textBox">TextBox del cual obtener el texto</param>
        /// <returns>Texto limpio o cadena vacía</returns>
        private string ObtenerTextoLimpio(TextBox textBox)
        {
            if (textBox.ForeColor == Color.Gray)
                return string.Empty;
            
            return textBox.Text.Trim();
        }

        /// <summary>
        /// Muestra un mensaje de éxito temporalmente
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar</param>
        private void MostrarMensajeExito(string mensaje)
        {
            lblMensajeExito.Text = mensaje;
            lblMensajeExito.Visible = true;
            
            // Centrar el mensaje
            lblMensajeExito.Location = new Point(
                (panelFormulario.Width - lblMensajeExito.Width) / 2,
                lblMensajeExito.Location.Y
            );
            
            timerMensajeExito.Start();
        }

        /// <summary>
        /// Limpia todos los campos del formulario
        /// </summary>
        private void LimpiarFormulario()
        {
            // Limpiar y restaurar placeholder
            ConfigurarPlaceholder();
            
            // Enfocar el campo de nombre
            txtNombreCategoria.Focus();
        }

        #endregion

        #region Eventos adicionales

        /// <summary>
        /// Maneja el evento Load del formulario
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            // Enfocar el campo de nombre al cargar
            txtNombreCategoria.Focus();
        }

        /// <summary>
        /// Maneja el evento KeyDown del TextBox para Enter
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter && btnGuardar.Enabled)
            {
                BtnGuardar_Click(this, EventArgs.Empty);
                return true;
            }
            
            if (keyData == Keys.Escape)
            {
                BtnRegresar_Click(this, EventArgs.Empty);
                return true;
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion
    }
}