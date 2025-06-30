using FlujoDeCajaApp.Modelos;
using FlujoDeCajaApp.Data;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace FlujoDeCajaApp.Formularios
{
    /// <summary>
    /// Formulario principal del sistema con menú de propiedades
    /// Diseño basado en el prototipo con cuadrícula de tarjetas
    /// </summary>
    public partial class FormMenuPrincipal : Form
    {
        // Controles del encabezado
        private Panel panelEncabezado = null!;
        private Label lblTitulo = null!;
        private PictureBox picLogo = null!;
        private Panel panelBotones = null!;
        private Button btnAgregar = null!;
        private Button btnHistorial = null!;
        private Button btnInactivas = null!;
        private Button btnCerrar = null!;
        
        // Controles de búsqueda
        private Panel panelBusqueda = null!;
        private TextBox txtBusqueda = null!;
        
        // Panel principal de contenido
        private Panel panelContenido = null!;
        private FlowLayoutPanel panelPropiedades = null!;
        
        // Controles para paneles secundarios
        private Panel panelSecundario = null!;
        
        // Lista de propiedades (datos de ejemplo)
        private List<Propiedad> propiedades = null!;
        private List<Propiedad> propiedadesFiltradas = null!;
        
        // Estado actual del panel
        private string panelActual = "propiedades";

        public FormMenuPrincipal()
        {
            InitializeComponent();
            InicializarDatos();
            CargarPropiedades();
        }

        /// <summary>
        /// Inicializa los datos del sistema cargando las casas desde la base de datos
        /// </summary>
        private void InicializarDatos()
        {
            try
            {
                // Cargar casas desde la base de datos
                CargarCasasDesdeBD();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Usar datos por defecto en caso de error
                propiedades = new List<Propiedad>();
                propiedadesFiltradas = new List<Propiedad>();
            }
        }

        /// <summary>
        /// Carga las casas desde la base de datos y las convierte a propiedades
        /// </summary>
        private void CargarCasasDesdeBD()
        {
            try
            {
                var casas = DatabaseHelper.ObtenerCasas();
                propiedades = new List<Propiedad>();
                
                foreach (var casa in casas)
                {
                    var propiedad = new Propiedad
                    {
                        Id = casa.Id,
                        Nombre = casa.Nombre,
                        Descripcion = $"{casa.NombreCategoria} - Dueño: {casa.NombreDueno}",
                        RutaImagen = casa.RutaImagen,
                        Activa = true,
                        FechaCreacion = casa.FechaCreacion,
                        NombreDueno = casa.NombreDueno
                    };
                    propiedades.Add(propiedad);
                }
                
                propiedadesFiltradas = new List<Propiedad>(propiedades);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar casas desde la base de datos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Configura las propiedades del formulario
        /// </summary>
        private void ConfigurarFormulario()
        {
            this.Text = "Samara Rentals - Property Management";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.Icon = CrearIconoAplicacion();
        }

        /// <summary>
        /// Inicializa y configura todos los controles del formulario
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            ConfigurarFormulario();

            // ===== PANEL ENCABEZADO =====
            panelEncabezado = new Panel
            {
                Size = new Size(1400, 120),
                Location = new Point(0, 0),
                Dock = DockStyle.Top
            };
            
            // Crear gradiente azul para el encabezado
            panelEncabezado.Paint += (sender, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    panelEncabezado.ClientRectangle,
                    Color.FromArgb(25, 118, 210),
                    Color.FromArgb(33, 150, 243),
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, panelEncabezado.ClientRectangle);
                }
            };

            // Logo en el encabezado
            picLogo = new PictureBox
            {
                Size = new Size(80, 80),
                Location = new Point(30, 20),
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.CenterImage
            };
            
            // Crear logo usando la imagen real de Samara Rentals
            Bitmap logo = new Bitmap(80, 80);
            using (Graphics g = Graphics.FromImage(logo))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                
                try
                {
                    // Intentar cargar la imagen real del logo
                    string logoPath = Path.Combine(Application.StartupPath, "Resources", "LogoSamaraRental.PNG");
                    if (File.Exists(logoPath))
                    {
                        using (Bitmap logoBitmap = new Bitmap(logoPath))
                        {
                            // Crear fondo circular blanco
                            g.FillEllipse(new SolidBrush(Color.White), 10, 10, 60, 60);
                            
                            // Dibujar el logo redimensionado dentro del círculo
                            g.SetClip(new Rectangle(12, 12, 56, 56));
                            g.DrawImage(logoBitmap, 12, 12, 56, 56);
                            g.ResetClip();
                            
                            // Borde circular sutil
                            g.DrawEllipse(new Pen(Color.FromArgb(200, 200, 200), 1), 10, 10, 60, 60);
                        }
                    }
                    else
                    {
                        // Fallback si no se encuentra la imagen
                        throw new FileNotFoundException("Logo no encontrado");
                    }
                }
                catch
                {
                    // Fallback: logo generado programáticamente
                    // Círculo blanco de fondo
                    g.FillEllipse(new SolidBrush(Color.White), 10, 10, 60, 60);
                    
                    // Casa representativa en azul Samara Rentals
                    using (SolidBrush casaBrush = new SolidBrush(Color.FromArgb(70, 130, 180)))
                    {
                        // Base de la casa
                        g.FillRectangle(casaBrush, 25, 40, 30, 20);
                        
                        // Techo triangular
                        Point[] techo = { new Point(40, 25), new Point(30, 38), new Point(50, 38) };
                        g.FillPolygon(casaBrush, techo);
                        
                        // Puerta
                        using (SolidBrush puertaBrush = new SolidBrush(Color.White))
                        {
                            g.FillRectangle(puertaBrush, 35, 48, 10, 12);
                        }
                        
                        // Ventanas
                        g.FillRectangle(casaBrush, 28, 45, 4, 4);
                        g.FillRectangle(casaBrush, 48, 45, 4, 4);
                        
                        // Elemento verde (palmera pequeña o planta)
                        using (SolidBrush verdeBrush = new SolidBrush(Color.FromArgb(34, 139, 34)))
                        {
                            g.FillEllipse(verdeBrush, 52, 35, 6, 8);
                            g.FillRectangle(verdeBrush, 54, 40, 2, 8);
                        }
                    }
                    
                    // Texto "SR" pequeño (Samara Rentals)
                    using (Font logoFont = new Font("Segoe UI", 8, FontStyle.Bold))
                    {
                        g.DrawString("SR", logoFont, new SolidBrush(Color.FromArgb(70, 130, 180)), 
                                   new PointF(33, 62));
                    }
                }
            }
            picLogo.Image = logo;

            // Título "Menu Principal"
            lblTitulo = new Label
            {
                Text = "Samara Rentals - Property Management",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(130, 35),
                Size = new Size(600, 50),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            // Panel de botones en la parte superior derecha
            panelBotones = new Panel
            {
                Size = new Size(400, 80),
                Location = new Point(950, 20),
                BackColor = Color.Transparent
            };

            // Botón Agregar
            btnAgregar = CrearBotonEncabezado("Agregar", "📄", 0);
            btnAgregar.Click += BtnAgregar_Click;

            // Botón Historial
            btnHistorial = CrearBotonEncabezado("Historial", "📋", 1);
            btnHistorial.Click += BtnHistorial_Click;

            // Botón Inactivas
            btnInactivas = CrearBotonEncabezado("Inactivas", "🏠", 2);
            btnInactivas.Click += BtnInactivas_Click;

            // Botón Cerrar
            btnCerrar = CrearBotonEncabezado("Cerrar", "❌", 3);
            btnCerrar.Click += BtnCerrar_Click;

            panelBotones.Controls.AddRange(new Control[] {
                btnAgregar, btnHistorial, btnInactivas, btnCerrar
            });

            panelEncabezado.Controls.AddRange(new Control[] {
                picLogo, lblTitulo, panelBotones
            });

            // ===== PANEL DE BÚSQUEDA =====
            panelBusqueda = new Panel
            {
                Size = new Size(1200, 60),
                Location = new Point(0, 80),
                BackColor = Color.FromArgb(40, 40, 40),
                Dock = DockStyle.Top
            };

            // Campo de búsqueda
            txtBusqueda = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Location = new Point(450, 15),
                Size = new Size(300, 30),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Text = "Ingrese el nombre"
            };
            txtBusqueda.TextChanged += TxtBusqueda_TextChanged;
            txtBusqueda.Enter += TxtBusqueda_Enter;
            txtBusqueda.Leave += TxtBusqueda_Leave;

            panelBusqueda.Controls.Add(txtBusqueda);

            // ===== PANEL PRINCIPAL DE CONTENIDO =====
            panelContenido = new Panel
            {
                Location = new Point(0, 140),
                Size = new Size(1200, 600),
                BackColor = Color.FromArgb(32, 32, 32),
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // Panel para mostrar las propiedades en cuadrícula
            panelPropiedades = new FlowLayoutPanel
            {
                Location = new Point(20, 20),
                Size = new Size(1160, 560),
                BackColor = Color.Transparent,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Panel secundario para otras vistas (inicialmente oculto)
            panelSecundario = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1200, 600),
                BackColor = Color.FromArgb(32, 32, 32),
                Dock = DockStyle.Fill,
                Visible = false
            };

            panelContenido.Controls.Add(panelPropiedades);
            panelContenido.Controls.Add(panelSecundario);

            // Agregar controles principales al formulario
            this.Controls.AddRange(new Control[] {
                panelContenido, panelBusqueda, panelEncabezado
            });

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Crea un botón para el encabezado con estilo uniforme
        /// </summary>
        /// <param name="texto">Texto del botón</param>
        /// <param name="icono">Icono emoji del botón</param>
        /// <param name="posicion">Posición en la fila (0-4)</param>
        /// <returns>Botón configurado</returns>
        private Button CrearBotonEncabezado(string texto, string icono, int posicion)
        {
            Button btn = new Button
            {
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Size = new Size(70, 40),
                Location = new Point(posicion * 75, 10),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(52, 73, 94),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.BottomCenter,
                Cursor = Cursors.Hand,
                ImageAlign = ContentAlignment.TopCenter
            };
            
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(248, 249, 250);
            
            // Crear imagen con icono
            Bitmap iconImage = new Bitmap(20, 20);
            using (Graphics g = Graphics.FromImage(iconImage))
            {
                g.Clear(Color.Transparent);
                // Dibujar icono simple basado en el tipo
                switch (texto)
                {
                    case "Agregar":
                        g.FillRectangle(new SolidBrush(Color.FromArgb(52, 152, 219)), 8, 2, 4, 16);
                        g.FillRectangle(new SolidBrush(Color.FromArgb(52, 152, 219)), 2, 8, 16, 4);
                        break;
                    case "Historial":
                        g.DrawRectangle(new Pen(Color.FromArgb(52, 152, 219), 2), 2, 2, 16, 16);
                        for (int i = 5; i < 15; i += 3)
                            g.DrawLine(new Pen(Color.FromArgb(52, 152, 219), 1), 5, i, 15, i);
                        break;
                    case "Inactivas":
                        g.DrawRectangle(new Pen(Color.FromArgb(231, 76, 60), 2), 2, 2, 16, 12);
                        g.FillPolygon(new SolidBrush(Color.FromArgb(231, 76, 60)), 
                                    new Point[] { new Point(5, 2), new Point(10, 0), new Point(15, 2) });
                        break;
                    case "Buscar":
                        g.DrawEllipse(new Pen(Color.FromArgb(52, 152, 219), 2), 2, 2, 12, 12);
                        g.DrawLine(new Pen(Color.FromArgb(52, 152, 219), 2), 12, 12, 18, 18);
                        break;
                    case "Cerrar":
                        g.DrawLine(new Pen(Color.FromArgb(231, 76, 60), 2), 4, 4, 16, 16);
                        g.DrawLine(new Pen(Color.FromArgb(231, 76, 60), 2), 16, 4, 4, 16);
                        break;
                }
            }
            
            btn.Image = iconImage;
            btn.Text = texto;
            
            return btn;
        }

        /// <summary>
        /// Carga y muestra las tarjetas de propiedades en la cuadrícula
        /// </summary>
        private void CargarPropiedades()
        {
            panelPropiedades.Controls.Clear();
            
            foreach (var propiedad in propiedadesFiltradas)
            {
                Panel tarjeta = CrearTarjetaPropiedad(propiedad);
                panelPropiedades.Controls.Add(tarjeta);
            }
        }

        /// <summary>
        /// Crea una tarjeta visual para mostrar una propiedad
        /// </summary>
        /// <param name="propiedad">Datos de la propiedad</param>
        /// <returns>Panel con la tarjeta configurada</returns>
        private Panel CrearTarjetaPropiedad(Propiedad propiedad)
        {
            Panel tarjeta = new Panel
            {
                Size = new Size(270, 180),
                BackColor = Color.FromArgb(45, 45, 45),
                Margin = new Padding(15),
                Tag = propiedad
            };
            
            // Sombra y bordes redondeados más pronunciados
            tarjeta.Paint += (sender, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Sombra
                using (GraphicsPath shadowPath = GetRoundedRectanglePath(
                    new Rectangle(2, 2, tarjeta.Width - 2, tarjeta.Height - 2), 12))
                {
                    using (PathGradientBrush shadowBrush = new PathGradientBrush(shadowPath))
                    {
                        shadowBrush.CenterColor = Color.FromArgb(50, 0, 0, 0);
                        shadowBrush.SurroundColors = new[] { Color.FromArgb(0, 0, 0, 0) };
                        e.Graphics.FillPath(shadowBrush, shadowPath);
                    }
                }
                
                // Tarjeta principal
                using (GraphicsPath path = GetRoundedRectanglePath(
                    new Rectangle(0, 0, tarjeta.Width - 2, tarjeta.Height - 2), 12))
                {
                    e.Graphics.FillPath(new SolidBrush(Color.FromArgb(45, 45, 45)), path);
                    e.Graphics.DrawPath(new Pen(Color.FromArgb(60, 60, 60), 1), path);
                }
            };

            // Imagen de la propiedad con bordes redondeados
            PictureBox imgPropiedad = new PictureBox
            {
                Size = new Size(270, 120),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(240, 240, 240),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            
            // Cargar imagen real o generar una por defecto
            Bitmap imgPropiedad2 = CargarImagenCasa(propiedad);
            imgPropiedad.Image = imgPropiedad2;
            
            // Hacer que la imagen tenga bordes redondeados
            imgPropiedad.Paint += (sender, e) =>
            {
                if (imgPropiedad.Image != null)
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (GraphicsPath path = GetRoundedRectanglePath(
                        new Rectangle(0, 0, imgPropiedad.Width, imgPropiedad.Height), 12))
                    {
                        e.Graphics.SetClip(path);
                        e.Graphics.DrawImage(imgPropiedad.Image, 0, 0, imgPropiedad.Width, imgPropiedad.Height);
                    }
                }
            };

            // Nombre de la propiedad
            Label lblNombre = new Label
            {
                Text = propiedad.Nombre,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 130),
                Size = new Size(200, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            // Botón de opciones más sutil
            Button btnOpciones = new Button
            {
                Text = "⋮",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(25, 25),
                Location = new Point(235, 145),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(200, 200, 200),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };
            btnOpciones.FlatAppearance.BorderSize = 0;
            btnOpciones.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60);
            btnOpciones.Click += (sender, e) => MostrarMenuPropiedad(propiedad, btnOpciones);

            tarjeta.Controls.AddRange(new Control[] {
                imgPropiedad, lblNombre, btnOpciones
            });

            return tarjeta;
        }

        /// <summary>
        /// Crea una imagen más realista para las propiedades basada en el nombre
        /// </summary>
        /// <param name="nombrePropiedad">Nombre de la propiedad para generar imagen</param>
        /// <returns>Imagen generada</returns>
        private Bitmap CrearImagenPropiedadRealista(string nombrePropiedad)
        {
            Bitmap img = new Bitmap(270, 120);
            using (Graphics g = Graphics.FromImage(img))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Colores base según el tipo de casa
                Color colorPrincipal, colorSecundario, colorAccento;
                
                switch (nombrePropiedad.ToUpper())
                {
                    case var name when name.Contains("ALICIA"):
                        colorPrincipal = Color.FromArgb(46, 125, 50);  // Verde
                        colorSecundario = Color.FromArgb(129, 199, 132);
                        colorAccento = Color.FromArgb(255, 193, 7);
                        break;
                    case var name when name.Contains("AMANECER"):
                        colorPrincipal = Color.FromArgb(255, 152, 0);  // Naranja
                        colorSecundario = Color.FromArgb(255, 204, 128);
                        colorAccento = Color.FromArgb(244, 67, 54);
                        break;
                    case var name when name.Contains("ARBOLES"):
                        colorPrincipal = Color.FromArgb(76, 175, 80);  // Verde bosque
                        colorSecundario = Color.FromArgb(165, 214, 167);
                        colorAccento = Color.FromArgb(139, 69, 19);
                        break;
                    case var name when name.Contains("BAMBOO"):
                        colorPrincipal = Color.FromArgb(56, 142, 60);  // Verde bambú
                        colorSecundario = Color.FromArgb(200, 230, 201);
                        colorAccento = Color.FromArgb(255, 235, 59);
                        break;
                    case var name when name.Contains("BRISA"):
                        colorPrincipal = Color.FromArgb(33, 150, 243);  // Azul océano
                        colorSecundario = Color.FromArgb(144, 202, 249);
                        colorAccento = Color.FromArgb(255, 255, 255);
                        break;
                    default:
                        // Colores por defecto
                        Random rnd = new Random(nombrePropiedad.GetHashCode());
                        colorPrincipal = Color.FromArgb(rnd.Next(60, 150), rnd.Next(120, 200), rnd.Next(60, 150));
                        colorSecundario = Color.FromArgb(200, 220, 240);
                        colorAccento = Color.FromArgb(255, 193, 7);
                        break;
                }
                
                // Cielo/fondo
                using (LinearGradientBrush skyBrush = new LinearGradientBrush(
                    new Rectangle(0, 0, 270, 60),
                    Color.FromArgb(135, 206, 250),
                    Color.FromArgb(176, 224, 230),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(skyBrush, 0, 0, 270, 60);
                }
                
                // Vegetación/paisaje de fondo
                using (SolidBrush vegBrush = new SolidBrush(colorSecundario))
                {
                    g.FillRectangle(vegBrush, 0, 60, 270, 60);
                }
                
                // Casa principal (rectángulo base)
                Rectangle casaBase = new Rectangle(60, 70, 120, 40);
                using (SolidBrush casaBrush = new SolidBrush(colorPrincipal))
                {
                    g.FillRectangle(casaBrush, casaBase);
                }
                
                // Techo
                Point[] techo = { 
                    new Point(50, 70), 
                    new Point(120, 50), 
                    new Point(190, 70) 
                };
                using (SolidBrush techoBrush = new SolidBrush(Color.FromArgb(121, 85, 72)))
                {
                    g.FillPolygon(techoBrush, techo);
                }
                
                // Piscina/agua (si es apropiado)
                if (nombrePropiedad.ToUpper().Contains("ALICIA") || 
                    nombrePropiedad.ToUpper().Contains("BRISA") ||
                    nombrePropiedad.ToUpper().Contains("BAMBOO"))
                {
                    Rectangle piscina = new Rectangle(200, 90, 60, 25);
                    using (SolidBrush aguaBrush = new SolidBrush(Color.FromArgb(64, 196, 255)))
                    {
                        g.FillRectangle(aguaBrush, piscina);
                    }
                }
                
                // Ventanas
                using (SolidBrush ventanaBrush = new SolidBrush(Color.FromArgb(100, 149, 237)))
                {
                    g.FillRectangle(ventanaBrush, 80, 80, 15, 15);
                    g.FillRectangle(ventanaBrush, 125, 80, 15, 15);
                }
                
                // Puerta
                using (SolidBrush puertaBrush = new SolidBrush(Color.FromArgb(139, 69, 19)))
                {
                    g.FillRectangle(puertaBrush, 110, 85, 10, 25);
                }
                
                // Vegetación decorativa
                using (SolidBrush plantaBrush = new SolidBrush(colorAccento))
                {
                    g.FillEllipse(plantaBrush, 20, 95, 25, 15);
                    g.FillEllipse(plantaBrush, 225, 100, 20, 12);
                }
                
                // Overlay sutil con el nombre
                using (SolidBrush overlayBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
                {
                    g.FillRectangle(overlayBrush, 0, 100, 270, 20);
                }
                
                // Nombre de la propiedad en la imagen
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                {
                    Font font = new Font("Segoe UI", 8, FontStyle.Bold);
                    string textoCorto = nombrePropiedad.Length > 15 ? 
                        nombrePropiedad.Substring(0, 12) + "..." : nombrePropiedad;
                    g.DrawString(textoCorto, font, textBrush, new PointF(10, 105));
                }
            }
            
            return img;
        }

        /// <summary>
        /// Carga la imagen real de una casa o genera una por defecto
        /// </summary>
        /// <param name="propiedad">Propiedad con la información de la casa</param>
        /// <returns>Imagen de la casa</returns>
        private Bitmap CargarImagenCasa(Propiedad propiedad)
        {
            try
            {
                // Si la propiedad tiene una imagen, intentar cargarla
                if (!string.IsNullOrEmpty(propiedad.RutaImagen))
                {
                    string rutaCompleta = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "FlujoDeCajaApp",
                        "FotosCasas",
                        propiedad.RutaImagen
                    );
                    
                    if (File.Exists(rutaCompleta))
                    {
                        using (var imagenOriginal = Image.FromFile(rutaCompleta))
                        {
                            return new Bitmap(imagenOriginal, 270, 120);
                        }
                    }
                }
                
                // Si no hay imagen o no se puede cargar, crear una por defecto
                return CrearImagenPropiedadRealista(propiedad.Nombre);
            }
            catch (Exception ex)
            {
                // En caso de error, crear imagen por defecto
                Console.WriteLine($"Error al cargar imagen para {propiedad.Nombre}: {ex.Message}");
                return CrearImagenPropiedadRealista(propiedad.Nombre);
            }
        }

        /// <summary>
        /// Obtiene un path con bordes redondeados
        /// </summary>
        private GraphicsPath GetRoundedRectanglePath(Rectangle rectangle, int cornerRadius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rectangle.X, rectangle.Y, cornerRadius, cornerRadius, 180, 90);
            path.AddArc(rectangle.X + rectangle.Width - cornerRadius, rectangle.Y, cornerRadius, cornerRadius, 270, 90);
            path.AddArc(rectangle.X + rectangle.Width - cornerRadius, rectangle.Y + rectangle.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
            path.AddArc(rectangle.X, rectangle.Y + rectangle.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
            path.CloseAllFigures();
            return path;
        }

        // ===== EVENTOS DE LOS BOTONES =====

        private void BtnAgregar_Click(object? sender, EventArgs e)
        {
            MostrarPanelAgregar();
        }

        private void BtnHistorial_Click(object? sender, EventArgs e)
        {
            MostrarPanelHistorial();
        }

        private void BtnInactivas_Click(object? sender, EventArgs e)
        {
            MostrarPropiedadesInactivas();
        }

        private void BtnCerrar_Click(object? sender, EventArgs e)
        {
            CerrarSesion();
        }

        // ===== EVENTOS DE BÚSQUEDA =====

        private void TxtBusqueda_TextChanged(object? sender, EventArgs e)
        {
            if (txtBusqueda.Text != "Ingrese el nombre")
            {
                FiltrarPropiedades(txtBusqueda.Text);
            }
        }

        private void TxtBusqueda_Enter(object? sender, EventArgs e)
        {
            if (txtBusqueda.Text == "Ingrese el nombre")
            {
                txtBusqueda.Text = "";
                txtBusqueda.ForeColor = Color.Black;
            }
        }

        private void TxtBusqueda_Leave(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBusqueda.Text))
            {
                txtBusqueda.Text = "Ingrese el nombre";
                txtBusqueda.ForeColor = Color.Gray;
            }
        }

        // ===== MÉTODOS DE FUNCIONALIDAD =====

        /// <summary>
        /// Filtra las propiedades por nombre
        /// </summary>
        /// <param name="filtro">Texto a buscar</param>
        private void FiltrarPropiedades(string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
            {
                propiedadesFiltradas = new List<Propiedad>(propiedades);
            }
            else
            {
                propiedadesFiltradas = propiedades
                    .Where(p => p.Nombre.ToLower().Contains(filtro.ToLower()))
                    .ToList();
            }
            
            CargarPropiedades();
        }

        /// <summary>
        /// Muestra el menú contextual de una propiedad
        /// </summary>
        /// <param name="propiedad">Propiedad seleccionada</param>
        /// <param name="botonOrigen">Botón que activó el menú</param>
        private void MostrarMenuPropiedad(Propiedad propiedad, Button botonOrigen)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            
            menu.Items.Add("Ver Detalles", null, (s, e) => VerDetallesPropiedad(propiedad));
            menu.Items.Add("Editar", null, (s, e) => EditarPropiedad(propiedad));
            menu.Items.Add("Movimientos", null, (s, e) => VerMovimientosPropiedad(propiedad));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Desactivar", null, (s, e) => DesactivarPropiedad(propiedad));
            
            menu.Show(botonOrigen, new Point(0, botonOrigen.Height));
        }

        /// <summary>
        /// Muestra el panel para agregar nuevos elementos
        /// </summary>
        private void MostrarPanelAgregar()
        {
            panelActual = "agregar";
            panelPropiedades.Visible = false;
            panelBusqueda.Visible = false; // Ocultar barra de búsqueda
            panelSecundario.Visible = true;
            panelSecundario.Controls.Clear();
            
            // Cargar UserControl PanelAgregar
            PanelAgregar panelAgregar = new PanelAgregar();
            panelAgregar.Dock = DockStyle.Fill;
            panelAgregar.VolverSolicitado += (sender, e) => VolverAMenuPrincipal();
            panelSecundario.Controls.Add(panelAgregar);
        }

        /// <summary>
        /// Muestra el panel de historial
        /// </summary>
        private void MostrarPanelHistorial()
        {
            panelActual = "historial";
            panelPropiedades.Visible = false;
            panelBusqueda.Visible = false; // Ocultar barra de búsqueda
            panelSecundario.Visible = true;
            panelSecundario.Controls.Clear();
            
            // Cargar UserControl PanelHistorial
            PanelHistorial panelHistorial = new PanelHistorial();
            panelHistorial.Dock = DockStyle.Fill;
            panelHistorial.VolverSolicitado += (sender, e) => VolverAMenuPrincipal();
            panelSecundario.Controls.Add(panelHistorial);
        }

        /// <summary>
        /// Muestra solo las propiedades inactivas
        /// </summary>
        private void MostrarPropiedadesInactivas()
        {
            panelActual = "inactivas";
            panelPropiedades.Visible = false;
            panelBusqueda.Visible = false; // Ocultar barra de búsqueda
            panelSecundario.Visible = true;
            panelSecundario.Controls.Clear();
            
            // Cargar UserControl PanelInactivas
            PanelInactivas panelInactivas = new PanelInactivas();
            panelInactivas.Dock = DockStyle.Fill;
            panelInactivas.VolverSolicitado += (sender, e) => VolverAMenuPrincipal();
            panelSecundario.Controls.Add(panelInactivas);
        }

        /// <summary>
        /// Vuelve al menú principal de propiedades
        /// </summary>
        private void VolverAMenuPrincipal()
        {
            panelActual = "propiedades";
            panelSecundario.Visible = false;
            panelPropiedades.Visible = true;
            panelBusqueda.Visible = true; // Mostrar barra de búsqueda
            
            // Recargar casas desde la base de datos para mostrar las nuevas
            CargarCasasDesdeBD();
            CargarPropiedades();
        }

        /// <summary>
        /// Cierra la sesión y regresa al login
        /// </summary>
        private void CerrarSesion()
        {
            DialogResult resultado = MessageBox.Show(
                "¿Está seguro que desea cerrar sesión?",
                "Cerrar Sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (resultado == DialogResult.Yes)
            {
                this.Hide();
                FormLogin formLogin = new FormLogin();
                formLogin.ShowDialog();
                this.Close();
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

        // ===== MÉTODOS PLACEHOLDER PARA FUNCIONALIDADES FUTURAS =====

        private void VerDetallesPropiedad(Propiedad propiedad)
        {
            MessageBox.Show($"Ver detalles de: {propiedad.Nombre}\n\nEsta funcionalidad se implementará más adelante.", 
                          "Detalles de Propiedad", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EditarPropiedad(Propiedad propiedad)
        {
            MessageBox.Show($"Editar: {propiedad.Nombre}\n\nEsta funcionalidad se implementará más adelante.", 
                          "Editar Propiedad", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void VerMovimientosPropiedad(Propiedad propiedad)
        {
            MessageBox.Show($"Ver movimientos de: {propiedad.Nombre}\n\nEsta funcionalidad se implementará más adelante.", 
                          "Movimientos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DesactivarPropiedad(Propiedad propiedad)
        {
            DialogResult resultado = MessageBox.Show(
                $"¿Está seguro que desea desactivar '{propiedad.Nombre}'?\n\nLa casa se moverá al panel de propiedades inactivas pero conservará todos sus datos.",
                "Desactivar Propiedad",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    bool exito = DatabaseHelper.DesactivarCasa(propiedad.Id);
                    
                    if (exito)
                    {
                        MessageBox.Show(
                            $"'{propiedad.Nombre}' ha sido desactivada correctamente.\n\nPodrá encontrarla en el panel de 'Propiedades Inactivas'.", 
                            "Propiedad Desactivada", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Information
                        );
                        
                        // Recargar las propiedades para actualizar la vista
                        CargarCasasDesdeBD();
                        CargarPropiedades();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo desactivar la propiedad. Inténtelo nuevamente.", 
                                      "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al desactivar la propiedad: {ex.Message}", 
                                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Libera los recursos del formulario
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Liberar imágenes creadas dinámicamente
                foreach (Control control in panelPropiedades.Controls)
                {
                    if (control is Panel tarjeta)
                    {
                        foreach (Control subControl in tarjeta.Controls)
                        {
                            if (subControl is PictureBox pic && pic.Image != null)
                            {
                                pic.Image.Dispose();
                            }
                        }
                    }
                }
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Extensión para dibujar rectángulos con bordes redondeados
    /// </summary>
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics graphics, SolidBrush brush, int x, int y, int width, int height, int cornerRadius)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(x, y, cornerRadius, cornerRadius, 180, 90);
                path.AddArc(x + width - cornerRadius, y, cornerRadius, cornerRadius, 270, 90);
                path.AddArc(x + width - cornerRadius, y + height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                path.AddArc(x, y + height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
                path.CloseAllFigures();
                graphics.FillPath(brush, path);
            }
        }
    }
}
