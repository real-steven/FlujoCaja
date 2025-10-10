using FlujoDeCajaApp.Data;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace FlujoDeCajaApp.Formularios
{
    /// <summary>
    /// Formulario de login para el Sistema de Cuentas - Playa Sámara
    /// Permite la autenticación de usuarios y inicializa la base de datos
    /// </summary>
    public partial class FormLogin : Form
    {
        // Controles del formulario
        private Panel panelIzquierdo = null!;
        private Panel panelDerecho = null!;
        private Label lblTitulo = null!;
        private Label lblSubtitulo = null!;
        private Label lblUsuario = null!;
        private Label lblContrasena = null!;
        private TextBox txtUsuario = null!;
        private TextBox txtContrasena = null!;
        private Button btnIniciarSesion = null!;
        private LinkLabel linkOlvidarContrasena = null!;
        private PictureBox picLogo = null!;

        public FormLogin()
        {
            InitializeComponent();
            InicializarSistema();
        }

        /// <summary>
        /// Inicializa el sistema creando la base de datos y configurando el formulario
        /// </summary>
        private void InicializarSistema()
        {
            try
            {
                Console.WriteLine("Inicializando FormLogin...");
                
                // Inicializar base de datos SQLite
                Console.WriteLine("Llamando a DatabaseHelper.InicializarBaseDatos()...");
                DatabaseHelper.InicializarBaseDatos();
                Console.WriteLine("Base de datos inicializada correctamente.");
                
                // Configurar el formulario
                Console.WriteLine("Configurando formulario...");
                ConfigurarFormulario();
                
                // Configurar placeholders iniciales
                Console.WriteLine("Configurando placeholders...");
                ConfigurarPlaceholders();
                Console.WriteLine("FormLogin inicializado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR en InicializarSistema: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al inicializar el sistema: {ex.Message}", 
                              "Error de Inicialización", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Configura los placeholders iniciales de los campos de texto
        /// </summary>
        private void ConfigurarPlaceholders()
        {
            // Configurar placeholder del usuario
            txtUsuario.Text = "Ingrese su usuario";
            txtUsuario.ForeColor = Color.Gray;

            // Configurar placeholder de la contraseña
            txtContrasena.UseSystemPasswordChar = false;
            txtContrasena.Text = "Ingrese su contraseña";
            txtContrasena.ForeColor = Color.Gray;
        }

        /// <summary>
        /// Configura las propiedades del formulario principal
        /// </summary>
        private void ConfigurarFormulario()
        {
            this.Text = "Samara Rentals - Property Management";
            this.Size = new Size(800, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Icon = CrearIconoAplicacion();
        }

        /// <summary>
        /// Inicializa y configura todos los controles del formulario
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Panel izquierdo - Imagen de Sámara y logo
            panelIzquierdo = new Panel
            {
                Size = new Size(400, 550),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(135, 206, 235), // Azul cielo
                Dock = DockStyle.None
            };

            // Panel derecho - Formulario de login
            panelDerecho = new Panel
            {
                Size = new Size(400, 550),
                Location = new Point(400, 0),
                BackColor = Color.FromArgb(30, 58, 138), // Azul oscuro como en la imagen
                Dock = DockStyle.None
            };

            // ===== PANEL IZQUIERDO =====
            
            // Título principal en panel izquierdo
            lblTitulo = new Label
            {
                Text = "Sistema de Flujo de Caja",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 58, 138),
                Location = new Point(50, 30),
                Size = new Size(300, 35),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            // Logo/Imagen del hotel
            picLogo = new PictureBox
            {
                Size = new Size(150, 100),
                Location = new Point(125, 80),
                BackColor = Color.White,
                SizeMode = PictureBoxSizeMode.CenterImage
            };

            // Crear logo simulado
            Bitmap logo = new Bitmap(150, 100);
            using (Graphics g = Graphics.FromImage(logo))
            {
                g.Clear(Color.White);
                // Marco
                g.DrawRectangle(new Pen(Color.FromArgb(30, 58, 138), 2), 5, 5, 140, 90);
                // Casa/techo
                Point[] casa = { new Point(75, 15), new Point(45, 35), new Point(105, 35) };
                g.FillPolygon(new SolidBrush(Color.FromArgb(64, 164, 223)), casa);
                // Texto SÁMARA
                g.DrawString("SÁMARA", new Font("Segoe UI", 10, FontStyle.Bold), 
                           new SolidBrush(Color.FromArgb(64, 164, 223)), new PointF(50, 45));
                g.DrawString("RENTALS", new Font("Segoe UI", 8, FontStyle.Regular), 
                           new SolidBrush(Color.FromArgb(100, 100, 100)), new PointF(50, 60));
                g.DrawString("PROPERTY MANAGEMENT", new Font("Segoe UI", 6, FontStyle.Regular), 
                           new SolidBrush(Color.FromArgb(100, 100, 100)), new PointF(25, 75));
            }
            picLogo.Image = logo;

            // Imagen de fondo para simular la playa
            PictureBox imgFondo = new PictureBox
            {
                Size = new Size(400, 300),
                Location = new Point(0, 200),
                BackColor = Color.FromArgb(135, 206, 235),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            // Crear una imagen simple que simule la playa
            Bitmap imagenPlaya = new Bitmap(400, 300);
            using (Graphics g = Graphics.FromImage(imagenPlaya))
            {
                // Cielo
                g.FillRectangle(new SolidBrush(Color.FromArgb(135, 206, 235)), 0, 0, 400, 150);
                // Mar
                g.FillRectangle(new SolidBrush(Color.FromArgb(64, 164, 223)), 0, 150, 400, 100);
                // Arena
                g.FillRectangle(new SolidBrush(Color.FromArgb(238, 203, 173)), 0, 250, 400, 50);
            }
            imgFondo.Image = imagenPlaya;

            // Texto "Sámara" grande en el panel izquierdo
            lblSubtitulo = new Label
            {
                Text = "Samara",
                Font = new Font("Brush Script MT", 28, FontStyle.Italic),
                ForeColor = Color.White,
                Location = new Point(140, 420),
                Size = new Size(120, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            // ===== PANEL DERECHO - FORMULARIO =====

            // Título "¡Bienvenido!"
            Label lblBienvenido = new Label
            {
                Text = "¡Bienvenido!",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(50, 80),
                Size = new Size(300, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            // Etiqueta Usuario
            lblUsuario = new Label
            {
                Text = "Usuario",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.White,
                Location = new Point(50, 160),
                Size = new Size(100, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            // Campo de texto Usuario (simplificado)
            txtUsuario = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Location = new Point(50, 190),
                Size = new Size(300, 30),
                BackColor = Color.White,
                ForeColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Etiqueta Contraseña
            lblContrasena = new Label
            {
                Text = "Contraseña",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.White,
                Location = new Point(50, 240),
                Size = new Size(100, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            // Campo de texto Contraseña (simplificado)
            txtContrasena = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Location = new Point(50, 270),
                Size = new Size(300, 30),
                BackColor = Color.White,
                ForeColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            // Link "Olvidé mi contraseña"
            linkOlvidarContrasena = new LinkLabel
            {
                Text = "Olvidé mi contraseña",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 320),
                Size = new Size(150, 20),
                LinkColor = Color.FromArgb(173, 216, 230),
                VisitedLinkColor = Color.FromArgb(173, 216, 230),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            // Botón Iniciar Sesión
            btnIniciarSesion = new Button
            {
                Text = "Iniciar Sesión",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(300, 45),
                Location = new Point(50, 360),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 58, 138),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnIniciarSesion.FlatAppearance.BorderSize = 1;
            btnIniciarSesion.FlatAppearance.BorderColor = Color.FromArgb(30, 58, 138);

            // Agregar controles a los paneles
            panelIzquierdo.Controls.AddRange(new Control[] { 
                imgFondo, lblTitulo, picLogo, lblSubtitulo 
            });

            panelDerecho.Controls.AddRange(new Control[] { 
                lblBienvenido, lblUsuario, txtUsuario, lblContrasena, txtContrasena,
                linkOlvidarContrasena, btnIniciarSesion 
            });

            // Agregar paneles al formulario
            this.Controls.AddRange(new Control[] { panelIzquierdo, panelDerecho });

            // Configurar eventos
            btnIniciarSesion.Click += BtnIniciarSesion_Click;
            linkOlvidarContrasena.LinkClicked += LinkOlvidarContrasena_LinkClicked;
            txtContrasena.KeyPress += TxtContrasena_KeyPress;
            
            // Eventos para placeholder text (simplificados)
            txtUsuario.Enter += TxtUsuario_Enter;
            txtUsuario.Leave += TxtUsuario_Leave;
            txtContrasena.Enter += TxtContrasena_Enter;
            txtContrasena.Leave += TxtContrasena_Leave;

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Maneja el evento de clic en el botón Iniciar Sesión
        /// </summary>
        private void BtnIniciarSesion_Click(object? sender, EventArgs e)
        {
            ProcesarLogin();
        }

        /// <summary>
        /// Maneja el evento de presionar Enter en el campo de contraseña
        /// </summary>
        private void TxtContrasena_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ProcesarLogin();
            }
        }

        /// <summary>
        /// Procesa el intento de login validando credenciales
        /// </summary>
        private void ProcesarLogin()
        {
            try
            {
                // Obtener valores reales de los campos (sin placeholders)
                string usuario = txtUsuario.Text == "Ingrese su usuario" ? "" : txtUsuario.Text.Trim();
                string contrasena = txtContrasena.Text == "Ingrese su contraseña" ? "" : txtContrasena.Text;

                // Validar que los campos no estén vacíos
                if (string.IsNullOrWhiteSpace(usuario))
                {
                    MessageBox.Show("Por favor ingrese su nombre de usuario.", 
                                  "Campo Requerido", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Warning);
                    txtUsuario.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(contrasena))
                {
                    MessageBox.Show("Por favor ingrese su contraseña.", 
                                  "Campo Requerido", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Warning);
                    txtContrasena.Focus();
                    return;
                }

                // Mostrar cursor de espera
                this.Cursor = Cursors.WaitCursor;
                btnIniciarSesion.Enabled = false;

                // Validar credenciales en la base de datos
                bool credencialesValidas = DatabaseHelper.ValidarCredenciales(usuario, contrasena);

                if (credencialesValidas)
                {
                    // Obtener información del usuario
                    var infoUsuario = DatabaseHelper.ObtenerUsuario(usuario);
                    
                    if (infoUsuario != null)
                    {
                        // Ocultar formulario de login
                        this.Hide();
                        
                        // Abrir el menú principal
                        FormMenuPrincipal menuPrincipal = new FormMenuPrincipal();
                        menuPrincipal.ShowDialog();
                        
                        // Cerrar la aplicación cuando se cierre el menú principal
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.\n\n" +
                                  "Por favor verifique sus credenciales e intente nuevamente.", 
                                  "Error de Autenticación", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Error);
                    
                    // Limpiar campos y restaurar placeholders
                    txtContrasena.Text = "";
                    TxtContrasena_Leave(null, EventArgs.Empty);
                    txtUsuario.Focus();
                    txtUsuario.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante el proceso de autenticación:\n\n{ex.Message}", 
                              "Error del Sistema", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
            }
            finally
            {
                // Restaurar cursor y botón
                this.Cursor = Cursors.Default;
                btnIniciarSesion.Enabled = true;
            }
        }

        /// <summary>
        /// Maneja el evento de clic en el link "Olvidar Contraseña"
        /// </summary>
        private void LinkOlvidarContrasena_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Para recuperar su contraseña, póngase en contacto con el administrador del sistema.\n\n" +
                          "Administrador: admin@playasamara.com\n" +
                          "Teléfono: +506 2656-0000", 
                          "Recuperar Contraseña", 
                          MessageBoxButtons.OK, 
                          MessageBoxIcon.Information);
        }

        /// <summary>
        /// Libera los recursos utilizados por el formulario
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Liberar recursos de imagen
                picLogo?.Image?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Maneja el evento Enter del campo Usuario (quitar placeholder)
        /// </summary>
        private void TxtUsuario_Enter(object? sender, EventArgs e)
        {
            if (txtUsuario.Text == "Ingrese su usuario")
            {
                txtUsuario.Text = "";
                txtUsuario.ForeColor = Color.Black;
            }
        }

        /// <summary>
        /// Maneja el evento Leave del campo Usuario (restaurar placeholder si está vacío)
        /// </summary>
        private void TxtUsuario_Leave(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                txtUsuario.Text = "Ingrese su usuario";
                txtUsuario.ForeColor = Color.Gray;
            }
        }

        /// <summary>
        /// Maneja el evento Enter del campo Contraseña (quitar placeholder)
        /// </summary>
        private void TxtContrasena_Enter(object? sender, EventArgs e)
        {
            if (txtContrasena.Text == "Ingrese su contraseña")
            {
                txtContrasena.Text = "";
                txtContrasena.ForeColor = Color.Black;
                txtContrasena.UseSystemPasswordChar = true;
            }
        }

        /// <summary>
        /// Maneja el evento Leave del campo Contraseña (restaurar placeholder si está vacío)
        /// </summary>
        private void TxtContrasena_Leave(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                txtContrasena.UseSystemPasswordChar = false;
                txtContrasena.Text = "Ingrese su contraseña";
                txtContrasena.ForeColor = Color.Gray;
            }
        }

        /// <summary>
        /// Crea un icono personalizado basado en el logo de Samara Rentals
        /// </summary>
        /// <returns>Icono para la aplicación</returns>
        private Icon CrearIconoAplicacion()
        {
            try
            {
                // Cargar la imagen del logo desde recursos
                string logoPath = Path.Combine(Application.StartupPath, "Resources", "LogoSamaraRental.PNG");
                if (File.Exists(logoPath))
                {
                    using (Bitmap logoBitmap = new Bitmap(logoPath))
                    {
                        // Redimensionar a 32x32 para el icono
                        Bitmap iconBitmap = new Bitmap(32, 32);
                        using (Graphics g = Graphics.FromImage(iconBitmap))
                        {
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.DrawImage(logoBitmap, 0, 0, 32, 32);
                        }
                        
                        // Convertir bitmap a icono
                        IntPtr hIcon = iconBitmap.GetHicon();
                        return Icon.FromHandle(hIcon);
                    }
                }
            }
            catch (Exception ex)
            {
                // Si hay error cargando la imagen, usar icono por defecto
                System.Diagnostics.Debug.WriteLine($"Error cargando logo: {ex.Message}");
            }
            
            // Fallback: crear icono simple si no se puede cargar la imagen
            Bitmap fallbackBitmap = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(fallbackBitmap))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Fondo azul similar al logo de Samara Rentals
                using (SolidBrush fondoBrush = new SolidBrush(Color.FromArgb(70, 130, 180)))
                {
                    g.FillEllipse(fondoBrush, 2, 2, 28, 28);
                }
                
                // Casa estilizada en blanco
                using (SolidBrush casaBrush = new SolidBrush(Color.White))
                {
                    // Base de la casa
                    g.FillRectangle(casaBrush, 10, 16, 12, 10);
                    
                    // Techo triangular
                    Point[] techo = { new Point(16, 8), new Point(12, 14), new Point(20, 14) };
                    g.FillPolygon(casaBrush, techo);
                    
                    // Puerta en azul
                    using (SolidBrush puertaBrush = new SolidBrush(Color.FromArgb(70, 130, 180)))
                    {
                        g.FillRectangle(puertaBrush, 14, 20, 4, 6);
                    }
                }
            }
            
            // Convertir bitmap a icono
            IntPtr hFallbackIcon = fallbackBitmap.GetHicon();
            return Icon.FromHandle(hFallbackIcon);
        }
    }
}
