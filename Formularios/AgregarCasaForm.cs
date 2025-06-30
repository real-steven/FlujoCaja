using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FlujoDeCajaApp.Data;
using FlujoDeCajaApp.Modelos;

namespace FlujoDeCajaApp.Formularios
{
    /// <summary>
    /// Formulario para agregar una nueva casa al sistema
    /// </summary>
    public partial class AgregarCasaForm : Form
    {
        #region Variables privadas
        
        private List<(int Id, string NombreCompleto, string Telefono, string Email)> duenos = new();
        private List<(int Id, string Nombre, string Descripcion)> categorias = new();
        private string rutaImagenSeleccionada = string.Empty;
        private bool imagenCargada = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor del formulario
        /// </summary>
        public AgregarCasaForm()
        {
            InitializeComponent();
            InicializarFormulario();
            CargarDatos();
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
                btnSeleccionarFoto.Click += BtnSeleccionarFoto_Click;
                btnEliminarFoto.Click += BtnEliminarFoto_Click;
                btnGuardar.Click += BtnGuardar_Click;
                btnCancelar.Click += BtnCancelar_Click;

                // Configurar validación en tiempo real
                txtNombre.TextChanged += ValidarFormulario;
                cmbDueno.SelectedIndexChanged += ValidarFormulario;
                cmbCategoria.SelectedIndexChanged += ValidarFormulario;

                // Configurar autocompletado personalizado para ComboBox
                ConfigurarAutocompletado();

                // Configurar imagen por defecto
                MostrarImagenPorDefecto();

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
        /// Configura el autocompletado personalizado para los ComboBox
        /// </summary>
        private void ConfigurarAutocompletado()
        {
            // El autocompletado ya está configurado en el Designer
            // Aquí se pueden agregar configuraciones adicionales si es necesario
        }

        /// <summary>
        /// Muestra una imagen por defecto en el PictureBox
        /// </summary>
        private void MostrarImagenPorDefecto()
        {
            try
            {
                // Crear una imagen por defecto simple
                Bitmap imagenDefecto = new Bitmap(pictureBoxFoto.Width, pictureBoxFoto.Height);
                using (Graphics g = Graphics.FromImage(imagenDefecto))
                {
                    g.Clear(Color.FromArgb(240, 240, 240));
                    
                    // Dibujar texto indicativo
                    string texto = "Haga clic en 'Seleccionar Foto'\npara agregar una imagen";
                    Font fuente = new Font("Segoe UI", 10, FontStyle.Regular);
                    Brush brocha = new SolidBrush(Color.FromArgb(149, 165, 166));
                    
                    SizeF tamanoTexto = g.MeasureString(texto, fuente);
                    float x = (imagenDefecto.Width - tamanoTexto.Width) / 2;
                    float y = (imagenDefecto.Height - tamanoTexto.Height) / 2;
                    
                    g.DrawString(texto, fuente, brocha, x, y);
                    
                    fuente.Dispose();
                    brocha.Dispose();
                }
                
                pictureBoxFoto.Image = imagenDefecto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear imagen por defecto: {ex.Message}");
            }
        }

        #endregion

        #region Carga de datos

        /// <summary>
        /// Carga los datos de dueños y categorías desde la base de datos
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                // Cargar dueños
                duenos = DatabaseHelper.ObtenerDuenos();
                cmbDueno.Items.Clear();
                foreach (var dueno in duenos)
                {
                    cmbDueno.Items.Add(dueno.NombreCompleto);
                }

                // Cargar categorías
                categorias = DatabaseHelper.ObtenerCategorias();
                cmbCategoria.Items.Clear();
                foreach (var categoria in categorias)
                {
                    cmbCategoria.Items.Add(categoria.Nombre);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Eventos de botones

        /// <summary>
        /// Maneja el evento click del botón Seleccionar Foto
        /// </summary>
        private void BtnSeleccionarFoto_Click(object? sender, EventArgs e)
        {
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    rutaImagenSeleccionada = openFileDialog.FileName;
                    
                    // Cargar y mostrar la imagen
                    using (var imagen = Image.FromFile(rutaImagenSeleccionada))
                    {
                        pictureBoxFoto.Image?.Dispose();
                        pictureBoxFoto.Image = new Bitmap(imagen);
                    }
                    
                    imagenCargada = true;
                    btnEliminarFoto.Enabled = true;
                    
                    ValidarFormulario(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la imagen: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Maneja el evento click del botón Eliminar Foto
        /// </summary>
        private void BtnEliminarFoto_Click(object? sender, EventArgs e)
        {
            try
            {
                rutaImagenSeleccionada = string.Empty;
                imagenCargada = false;
                btnEliminarFoto.Enabled = false;
                
                // Restaurar imagen por defecto
                MostrarImagenPorDefecto();
                
                ValidarFormulario(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar la foto: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                
                // Obtener los IDs seleccionados
                int duenoId = ObtenerIdDuenoSeleccionado();
                int categoriaId = ObtenerIdCategoriaSeleccionada();
                
                // Copiar la imagen si se seleccionó una
                string rutaImagenRelativa = string.Empty;
                if (imagenCargada && !string.IsNullOrEmpty(rutaImagenSeleccionada))
                {
                    rutaImagenRelativa = await CopiarImagenACarpetaLocal();
                }
                
                // Guardar en la base de datos
                int casaId = DatabaseHelper.GuardarCasa(
                    txtNombre.Text.Trim(),
                    duenoId,
                    categoriaId,
                    rutaImagenRelativa
                );
                
                MessageBox.Show("Casa guardada exitosamente.", 
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Cerrar el formulario con resultado OK
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                btnGuardar.Enabled = true;
                btnGuardar.Text = "Guardar";
                
                MessageBox.Show($"Error al guardar la casa: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Maneja el evento click del botón Cancelar
        /// </summary>
        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cancelar: {ex.Message}", 
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
            // Validar nombre
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                if (mostrarMensajes)
                    MessageBox.Show("Debe ingresar un nombre para la casa.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validar dueño
            if (cmbDueno.SelectedIndex < 0 || string.IsNullOrWhiteSpace(cmbDueno.Text))
            {
                if (mostrarMensajes)
                    MessageBox.Show("Debe seleccionar un dueño.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validar categoría
            if (cmbCategoria.SelectedIndex < 0 || string.IsNullOrWhiteSpace(cmbCategoria.Text))
            {
                if (mostrarMensajes)
                    MessageBox.Show("Debe seleccionar una categoría.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        #endregion

        #region Métodos auxiliares

        /// <summary>
        /// Obtiene el ID del dueño seleccionado
        /// </summary>
        /// <returns>ID del dueño</returns>
        private int ObtenerIdDuenoSeleccionado()
        {
            if (cmbDueno.SelectedIndex >= 0)
            {
                return duenos[cmbDueno.SelectedIndex].Id;
            }
            
            // Si se escribió un nombre, buscar coincidencia
            string nombreSeleccionado = cmbDueno.Text;
            var dueno = duenos.FirstOrDefault(d => d.NombreCompleto.Equals(nombreSeleccionado, StringComparison.OrdinalIgnoreCase));
            return dueno.Id;
        }

        /// <summary>
        /// Obtiene el ID de la categoría seleccionada
        /// </summary>
        /// <returns>ID de la categoría</returns>
        private int ObtenerIdCategoriaSeleccionada()
        {
            if (cmbCategoria.SelectedIndex >= 0)
            {
                return categorias[cmbCategoria.SelectedIndex].Id;
            }
            
            // Si se escribió un nombre, buscar coincidencia
            string nombreSeleccionado = cmbCategoria.Text;
            var categoria = categorias.FirstOrDefault(c => c.Nombre.Equals(nombreSeleccionado, StringComparison.OrdinalIgnoreCase));
            return categoria.Id;
        }

        /// <summary>
        /// Copia la imagen seleccionada a la carpeta local de fotos
        /// </summary>
        /// <returns>Ruta relativa de la imagen copiada</returns>
        private async Task<string> CopiarImagenACarpetaLocal()
        {
            try
            {
                // Crear la carpeta de fotos si no existe
                string carpetaFotos = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "FlujoDeCajaApp",
                    "FotosCasas"
                );
                
                if (!Directory.Exists(carpetaFotos))
                {
                    Directory.CreateDirectory(carpetaFotos);
                }
                
                // Generar un nombre único para el archivo
                string extension = Path.GetExtension(rutaImagenSeleccionada);
                string nombreArchivo = $"{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString("N")[..8]}{extension}";
                string rutaDestino = Path.Combine(carpetaFotos, nombreArchivo);
                
                // Copiar el archivo
                await Task.Run(() => File.Copy(rutaImagenSeleccionada, rutaDestino, true));
                
                return nombreArchivo;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al copiar la imagen: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
