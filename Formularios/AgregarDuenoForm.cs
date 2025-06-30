using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FlujoDeCajaApp.Data;

namespace FlujoDeCajaApp.Formularios
{
    /// <summary>
    /// Formulario para agregar nuevos dueños al sistema
    /// </summary>
    public partial class AgregarDuenoForm : Form
    {
        #region Variables privadas

        private System.Windows.Forms.Timer timerMensajeExito = new();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor del formulario
        /// </summary>
        public AgregarDuenoForm()
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
                txtNombreCompleto.TextChanged += ValidarFormulario;
                txtIdentificacion.TextChanged += ValidarFormulario;
                txtCorreoElectronico.TextChanged += ValidarFormulario;
                txtNumeroTelefonico.TextChanged += ValidarFormulario;

                // Configurar placeholders
                ConfigurarPlaceholders();

                // Configurar dropdown de tipo de entidad
                ConfigurarDropdownTipoEntidad();

                // Configurar timer para mensaje de éxito
                ConfigurarTimerMensajeExito();

                // Deshabilitar el botón guardar inicialmente
                btnGuardar.Enabled = false;
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
        /// Configura los placeholders de los TextBox
        /// </summary>
        private void ConfigurarPlaceholders()
        {
            // Configurar placeholders usando eventos Enter/Leave
            ConfigurarPlaceholder(txtNombreCompleto, "Ingrese nombre y apellidos");
            ConfigurarPlaceholder(txtIdentificacion, "Ingrese número de identificación");
            ConfigurarPlaceholder(txtCorreoElectronico, "ejemplo@correo.com");
            ConfigurarPlaceholder(txtNumeroTelefonico, "Ingrese número de teléfono");
        }

        /// <summary>
        /// Configura el placeholder para un TextBox específico
        /// </summary>
        /// <param name="textBox">TextBox a configurar</param>
        /// <param name="placeholder">Texto del placeholder</param>
        private void ConfigurarPlaceholder(TextBox textBox, string placeholder)
        {
            textBox.Text = placeholder;
            textBox.ForeColor = Color.Gray;

            textBox.Enter += (sender, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.Leave += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        /// <summary>
        /// Configura el dropdown de tipo de entidad
        /// </summary>
        private void ConfigurarDropdownTipoEntidad()
        {
            cmbTipoEntidad.Items.Clear();
            cmbTipoEntidad.Items.Add("Dueño");
            cmbTipoEntidad.Items.Add("Categoría");
            cmbTipoEntidad.Items.Add("Usuario");
            
            // Seleccionar "Dueño" por defecto
            cmbTipoEntidad.SelectedIndex = 0;
            
            // Deshabilitar temporalmente otros tipos
            cmbTipoEntidad.Enabled = false; // Solo Dueño por ahora
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

        #region Eventos de botones

        /// <summary>
        /// Maneja el evento click del botón Guardar
        /// </summary>
        private void BtnGuardar_Click(object? sender, EventArgs e)
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
                string nombreCompleto = ObtenerTextoLimpio(txtNombreCompleto);
                string identificacion = ObtenerTextoLimpio(txtIdentificacion);
                string correo = ObtenerTextoLimpio(txtCorreoElectronico);
                string telefono = ObtenerTextoLimpio(txtNumeroTelefonico);
                
                // Separar nombre y apellido
                var (nombre, apellido) = SepararNombreApellido(nombreCompleto);
                
                // Guardar en la base de datos
                int duenoId = DatabaseHelper.GuardarDueno(nombre, apellido, identificacion, correo, telefono);
                
                // Mostrar mensaje de éxito
                MostrarMensajeExito("Dueño agregado correctamente");
                
                // Limpiar el formulario
                LimpiarFormulario();
                
                btnGuardar.Enabled = false; // Se habilitará cuando se llene el formulario nuevamente
                btnGuardar.Text = "Guardar";
            }
            catch (Exception ex)
            {
                btnGuardar.Enabled = true;
                btnGuardar.Text = "Guardar";
                
                MessageBox.Show($"Error al guardar el dueño: {ex.Message}", 
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
            // Validar nombre completo
            string nombreCompleto = ObtenerTextoLimpio(txtNombreCompleto);
            if (string.IsNullOrWhiteSpace(nombreCompleto))
            {
                if (mostrarMensajes)
                    MessageBox.Show("Debe ingresar el nombre completo del dueño.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validar que tenga al menos nombre y apellido
            if (nombreCompleto.Trim().Split(' ').Length < 2)
            {
                if (mostrarMensajes)
                    MessageBox.Show("Debe ingresar al menos nombre y apellido.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validar identificación
            string identificacion = ObtenerTextoLimpio(txtIdentificacion);
            if (string.IsNullOrWhiteSpace(identificacion))
            {
                if (mostrarMensajes)
                    MessageBox.Show("Debe ingresar el número de identificación.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Verificar que la identificación no exista
            if (mostrarMensajes && DatabaseHelper.ExisteDuenoConIdentificacion(identificacion))
            {
                MessageBox.Show("Ya existe un dueño con esa identificación.", 
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validar correo electrónico
            string correo = ObtenerTextoLimpio(txtCorreoElectronico);
            if (string.IsNullOrWhiteSpace(correo))
            {
                if (mostrarMensajes)
                    MessageBox.Show("Debe ingresar el correo electrónico.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!EsCorreoValido(correo))
            {
                if (mostrarMensajes)
                    MessageBox.Show("El formato del correo electrónico no es válido.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Verificar que el correo no exista
            if (mostrarMensajes && DatabaseHelper.ExisteDuenoConEmail(correo))
            {
                MessageBox.Show("Ya existe un dueño con ese correo electrónico.", 
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validar número telefónico
            string telefono = ObtenerTextoLimpio(txtNumeroTelefonico);
            if (string.IsNullOrWhiteSpace(telefono))
            {
                if (mostrarMensajes)
                    MessageBox.Show("Debe ingresar el número telefónico.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida si un correo electrónico tiene formato válido
        /// </summary>
        /// <param name="correo">Correo a validar</param>
        /// <returns>True si es válido, False en caso contrario</returns>
        private bool EsCorreoValido(string correo)
        {
            try
            {
                string patron = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(correo, patron);
            }
            catch
            {
                return false;
            }
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
        /// Separa un nombre completo en nombre y apellido
        /// </summary>
        /// <param name="nombreCompleto">Nombre completo</param>
        /// <returns>Tupla con nombre y apellido</returns>
        private (string nombre, string apellido) SepararNombreApellido(string nombreCompleto)
        {
            var partes = nombreCompleto.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (partes.Length == 0)
                return (string.Empty, string.Empty);
            
            if (partes.Length == 1)
                return (partes[0], string.Empty);
            
            // El primer elemento es el nombre, el resto son apellidos
            string nombre = partes[0];
            string apellido = string.Join(" ", partes.Skip(1));
            
            return (nombre, apellido);
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
            // Limpiar y restaurar placeholders
            ConfigurarPlaceholders();
        }

        #endregion
    }
}
