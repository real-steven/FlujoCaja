using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FlujoDeCajaApp.Data;

namespace FlujoDeCajaApp.Formularios
{
    /// <summary>
    /// Formulario para agregar nuevos usuarios al sistema
    /// </summary>
    public partial class AgregarUsuarioForm : Form
    {
        #region Variables privadas

        private System.Windows.Forms.Timer timerMensajeExito = new();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor del formulario
        /// </summary>
        public AgregarUsuarioForm()
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
                txtNombreUsuario.TextChanged += ValidarFormulario;
                txtContrasena.TextChanged += ValidarFormulario;
                txtCorreo.TextChanged += ValidarFormulario;

                // Configurar placeholders
                ConfigurarPlaceholders();

                // Configurar dropdown de tipo de entidad
                ConfigurarDropdownTipoEntidad();

                // Configurar timer para mensaje de éxito
                ConfigurarTimerMensajeExito();

                // Deshabilitar el botón guardar inicialmente
                btnGuardar.Enabled = false;

                // Configurar KeyPress para Enter
                this.KeyPreview = true;
                this.KeyPress += AgregarUsuarioForm_KeyPress;
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
            ConfigurarPlaceholder(txtNombreUsuario, "Ingrese el nombre");
            ConfigurarPlaceholder(txtContrasena, "Ingrese la contraseña");
            ConfigurarPlaceholder(txtCorreo, "Ingrese el correo");
        }

        /// <summary>
        /// Configura el placeholder de un TextBox específico
        /// </summary>
        /// <param name="textBox">TextBox a configurar</param>
        /// <param name="placeholder">Texto del placeholder</param>
        private void ConfigurarPlaceholder(TextBox textBox, string placeholder)
        {
            textBox.Text = placeholder;
            textBox.ForeColor = Color.Gray;

            textBox.Enter += (sender, e) =>
            {
                if (textBox.ForeColor == Color.Gray)
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
            cmbTipoEntidad.Items.AddRange(new string[] { "Usuario", "Dueño", "Casa", "Categoría" });
            cmbTipoEntidad.SelectedIndex = 0; // Seleccionar "Usuario" por defecto
            cmbTipoEntidad.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        /// <summary>
        /// Configura el timer para el mensaje de éxito
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
        private void AgregarUsuarioForm_KeyPress(object? sender, KeyPressEventArgs e)
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
                string nombreUsuario = ObtenerTextoLimpio(txtNombreUsuario);
                string contrasena = ObtenerTextoLimpio(txtContrasena);
                string correo = ObtenerTextoLimpio(txtCorreo);
                
                // Guardar en la base de datos
                int usuarioId = DatabaseHelper.GuardarUsuario(nombreUsuario, contrasena, correo, "Usuario");
                
                // Mostrar mensaje de éxito
                MostrarMensajeExito("Usuario agregado correctamente");
                
                // Limpiar el formulario
                LimpiarFormulario();
                
                btnGuardar.Enabled = false; // Se habilitará cuando se llene el formulario nuevamente
                btnGuardar.Text = "Guardar";
            }
            catch (Exception ex)
            {
                btnGuardar.Enabled = true;
                btnGuardar.Text = "Guardar";
                
                MessageBox.Show($"Error al guardar el usuario: {ex.Message}", 
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
            // Validar nombre de usuario
            string nombreUsuario = ObtenerTextoLimpio(txtNombreUsuario);
            
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                if (mostrarMensajes)
                    MessageBox.Show("Debe ingresar el nombre de usuario.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validar longitud mínima del nombre de usuario
            if (nombreUsuario.Length < 3)
            {
                if (mostrarMensajes)
                    MessageBox.Show("El nombre de usuario debe tener al menos 3 caracteres.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validar longitud máxima del nombre de usuario
            if (nombreUsuario.Length > 20)
            {
                if (mostrarMensajes)
                    MessageBox.Show("El nombre de usuario no puede tener más de 20 caracteres.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validar contraseña
            string contrasena = ObtenerTextoLimpio(txtContrasena);
            
            if (string.IsNullOrWhiteSpace(contrasena))
            {
                if (mostrarMensajes)
                    MessageBox.Show("Debe ingresar la contraseña.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validar longitud mínima de la contraseña
            if (contrasena.Length < 4)
            {
                if (mostrarMensajes)
                    MessageBox.Show("La contraseña debe tener al menos 4 caracteres.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validar correo
            string correo = ObtenerTextoLimpio(txtCorreo);
            
            if (string.IsNullOrWhiteSpace(correo))
            {
                if (mostrarMensajes)
                    MessageBox.Show("Debe ingresar el correo electrónico.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validar formato del correo
            if (!DatabaseHelper.ValidarFormatoCorreo(correo))
            {
                if (mostrarMensajes)
                    MessageBox.Show("El formato del correo electrónico no es válido.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Verificar que el nombre de usuario no exista
            if (mostrarMensajes && DatabaseHelper.ExisteUsuarioConNombre(nombreUsuario))
            {
                MessageBox.Show("Ya existe un usuario con ese nombre.", 
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Verificar que el correo no exista
            if (mostrarMensajes && DatabaseHelper.ExisteUsuarioConCorreo(correo))
            {
                MessageBox.Show("Ya existe un usuario con ese correo electrónico.", 
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
            // Limpiar y restaurar placeholders
            ConfigurarPlaceholders();
            
            // Enfocar el primer campo
            txtNombreUsuario.Focus();
        }

        #endregion
    }
}
