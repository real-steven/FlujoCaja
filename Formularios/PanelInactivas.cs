using FlujoDeCajaApp.Modelos;
using FlujoDeCajaApp.Data;
using System.IO;

namespace FlujoDeCajaApp.Formularios
{
    /// <summary>
    /// Panel para mostrar propiedades inactivas
    /// </summary>
    public partial class PanelInactivas : UserControl
    {
        private Panel panelTitulo = null!;
        private Label lblTitulo = null!;
        private Button btnVolver = null!;
        private Panel panelContenido = null!;
        private FlowLayoutPanel panelPropiedades = null!;
        
        // Lista de propiedades inactivas
        private List<Propiedad> propiedadesInactivas = null!;
        
        // Evento para notificar al formulario padre
        public event EventHandler? VolverSolicitado;

        public PanelInactivas()
        {
            InitializeComponent();
            CargarCasasInactivasDesdeBD();
            CargarPropiedadesInactivas();
        }

        /// <summary>
        /// Carga las casas inactivas desde la base de datos
        /// </summary>
        private void CargarCasasInactivasDesdeBD()
        {
            try
            {
                var casasInactivas = DatabaseHelper.ObtenerCasasInactivas();
                propiedadesInactivas = new List<Propiedad>();
                
                foreach (var casa in casasInactivas)
                {
                    var propiedad = new Propiedad
                    {
                        Id = casa.Id,
                        Nombre = casa.Nombre,
                        Descripcion = $"{casa.NombreCategoria} - Dueño: {casa.NombreDueno}",
                        RutaImagen = casa.RutaImagen,
                        Activa = false,
                        FechaCreacion = casa.FechaCreacion,
                        NombreDueno = casa.NombreDueno
                    };
                    propiedadesInactivas.Add(propiedad);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar casas inactivas: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Usar lista vacía en caso de error
                propiedadesInactivas = new List<Propiedad>();
            }
        }

        /// <summary>
        /// Inicializa los controles del panel
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Configuración del UserControl
            this.Size = new Size(1000, 600);
            this.BackColor = Color.FromArgb(32, 32, 32);

            // Panel del título
            panelTitulo = new Panel
            {
                Size = new Size(1000, 80),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(231, 76, 60),
                Dock = DockStyle.Top
            };

            // Título
            lblTitulo = new Label
            {
                Text = "Propiedades Inactivas",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(50, 20),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            // Botón volver
            btnVolver = new Button
            {
                Text = "← Volver",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Size = new Size(100, 35),
                Location = new Point(850, 22),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(231, 76, 60),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnVolver.FlatAppearance.BorderSize = 1;
            btnVolver.FlatAppearance.BorderColor = Color.White;
            btnVolver.Click += BtnVolver_Click;

            panelTitulo.Controls.AddRange(new Control[] { lblTitulo, btnVolver });

            // Panel de contenido
            panelContenido = new Panel
            {
                Location = new Point(0, 80),
                Size = new Size(1000, 520),
                BackColor = Color.FromArgb(32, 32, 32),
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // Panel para las tarjetas de propiedades
            panelPropiedades = new FlowLayoutPanel
            {
                Location = new Point(20, 20),
                Size = new Size(960, 480),
                BackColor = Color.Transparent,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            panelContenido.Controls.Add(panelPropiedades);

            // Agregar controles al UserControl
            this.Controls.AddRange(new Control[] { panelContenido, panelTitulo });

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Carga las propiedades inactivas en la vista
        /// </summary>
        private void CargarPropiedadesInactivas()
        {
            panelPropiedades.Controls.Clear();

            if (propiedadesInactivas.Count == 0)
            {
                // Mostrar mensaje cuando no hay propiedades inactivas
                Label lblSinDatos = new Label
                {
                    Text = "🏠\n\nNo hay propiedades inactivas\n\nTodas las propiedades están activas en el sistema.",
                    Font = new Font("Segoe UI", 16),
                    ForeColor = Color.FromArgb(200, 200, 200),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(400, 200),
                    Dock = DockStyle.Fill
                };
                panelPropiedades.Controls.Add(lblSinDatos);
            }
            else
            {
                foreach (var propiedad in propiedadesInactivas)
                {
                    Panel tarjeta = CrearTarjetaPropiedadInactiva(propiedad);
                    panelPropiedades.Controls.Add(tarjeta);
                }

                // Actualizar contador en el título
                lblTitulo.Text = $"Propiedades Inactivas ({propiedadesInactivas.Count})";
            }
        }

        /// <summary>
        /// Crea una tarjeta para mostrar una propiedad inactiva
        /// </summary>
        /// <param name="propiedad">Datos de la propiedad</param>
        /// <returns>Panel con la tarjeta</returns>
        private Panel CrearTarjetaPropiedadInactiva(Propiedad propiedad)
        {
            Panel tarjeta = new Panel
            {
                Size = new Size(280, 200),
                BackColor = Color.FromArgb(45, 45, 45),
                Margin = new Padding(10),
                Tag = propiedad
            };

            // Borde con estilo para propiedades inactivas
            tarjeta.Paint += (sender, e) =>
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(231, 76, 60), 2), 
                                       0, 0, tarjeta.Width - 1, tarjeta.Height - 1);
            };

            // Imagen de la propiedad con overlay inactivo
            PictureBox imgPropiedad = new PictureBox
            {
                Size = new Size(280, 100),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(220, 220, 220),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            // Crear imagen con overlay "INACTIVA"
            Bitmap imgConOverlay = CrearImagenInactiva(propiedad.RutaImagen);
            imgPropiedad.Image = imgConOverlay;

            // Nombre de la propiedad
            Label lblNombre = new Label
            {
                Text = propiedad.Nombre,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 110),
                Size = new Size(260, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            // Estado inactivo
            Label lblEstado = new Label
            {
                Text = "❌ INACTIVA",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(10, 135),
                Size = new Size(120, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            // Fecha de desactivación
            Label lblFecha = new Label
            {
                Text = $"Desde: {propiedad.FechaCreacion:dd/MM/yyyy}",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(10, 155),
                Size = new Size(150, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            // Botón para reactivar
            Button btnReactivar = new Button
            {
                Text = "Reactivar",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Size = new Size(80, 25),
                Location = new Point(190, 165),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnReactivar.FlatAppearance.BorderSize = 0;
            btnReactivar.Click += (sender, e) => ReactivarPropiedad(propiedad);

            tarjeta.Controls.AddRange(new Control[] {
                imgPropiedad, lblNombre, lblEstado, lblFecha, btnReactivar
            });

            return tarjeta;
        }

        /// <summary>
        /// Crea una imagen con overlay "INACTIVA" usando la foto real de la casa
        /// </summary>
        /// <param name="rutaImagen">Ruta de la imagen original de la casa</param>
        /// <returns>Imagen con overlay</returns>
        private Bitmap CrearImagenInactiva(string rutaImagen)
        {
            Bitmap img = new Bitmap(280, 100);
            using (Graphics g = Graphics.FromImage(img))
            {
                // Cargar imagen real de la casa si existe
                if (!string.IsNullOrEmpty(rutaImagen) && File.Exists(rutaImagen))
                {
                    try
                    {
                        using (var imagenOriginal = Image.FromFile(rutaImagen))
                        {
                            // Dibujar la imagen real escalada
                            g.DrawImage(imagenOriginal, 0, 0, 280, 100);
                        }
                    }
                    catch
                    {
                        // Si hay error cargando la imagen, usar fondo gris
                        DibujarFondoGenerico(g);
                    }
                }
                else
                {
                    // Si no hay imagen, usar fondo gris
                    DibujarFondoGenerico(g);
                }

                // Overlay semi-transparente "INACTIVA"
                using (SolidBrush overlay = new SolidBrush(Color.FromArgb(120, 231, 76, 60)))
                {
                    g.FillRectangle(overlay, 0, 0, img.Width, img.Height);
                }

                // Texto "INACTIVA" con fondo semi-transparente
                using (SolidBrush textBackground = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                {
                    Font font = new Font("Segoe UI", 14, FontStyle.Bold);
                    string texto = "INACTIVA";
                    SizeF medidas = g.MeasureString(texto, font);
                    float x = (img.Width - medidas.Width) / 2;
                    float y = (img.Height - medidas.Height) / 2;
                    
                    // Fondo del texto
                    RectangleF rectTexto = new RectangleF(x - 8, y - 4, medidas.Width + 16, medidas.Height + 8);
                    g.FillRectangle(textBackground, rectTexto);
                    
                    // Texto
                    g.DrawString(texto, font, textBrush, x, y);
                }
            }

            return img;
        }

        /// <summary>
        /// Dibuja un fondo genérico cuando no hay imagen disponible
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DibujarFondoGenerico(Graphics g)
        {
            // Fondo gris
            g.Clear(Color.FromArgb(220, 220, 220));

            // Patrón de líneas diagonales
            using (Pen pen = new Pen(Color.FromArgb(200, 200, 200), 2))
            {
                for (int i = 0; i < 280 + 100; i += 20)
                {
                    g.DrawLine(pen, i, 0, i - 100, 100);
                }
            }

            // Icono de casa en el centro
            using (SolidBrush iconBrush = new SolidBrush(Color.FromArgb(150, 150, 150)))
            {
                Font iconFont = new Font("Segoe UI", 24);
                string icono = "🏠";
                SizeF medidas = g.MeasureString(icono, iconFont);
                float x = (280 - medidas.Width) / 2;
                float y = (100 - medidas.Height) / 2;
                g.DrawString(icono, iconFont, iconBrush, x, y);
            }
        }

        /// <summary>
        /// Reactiva una propiedad
        /// </summary>
        /// <param name="propiedad">Propiedad a reactivar</param>
        private void ReactivarPropiedad(Propiedad propiedad)
        {
            DialogResult resultado = MessageBox.Show(
                $"¿Está seguro que desea reactivar '{propiedad.Nombre}'?\n\nLa casa volverá a aparecer en el menú principal.",
                "Reactivar Propiedad",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    bool exito = DatabaseHelper.ReactivarCasa(propiedad.Id);
                    
                    if (exito)
                    {
                        MessageBox.Show(
                            $"'{propiedad.Nombre}' ha sido reactivada correctamente.\n\nYa aparece nuevamente en el menú principal.", 
                            "Propiedad Reactivada", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Information
                        );
                        
                        // Recargar las propiedades inactivas para actualizar la vista
                        CargarCasasInactivasDesdeBD();
                        CargarPropiedadesInactivas();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo reactivar la propiedad. Inténtelo nuevamente.", 
                                      "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al reactivar la propiedad: {ex.Message}", 
                                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void BtnVolver_Click(object? sender, EventArgs e)
        {
            VolverSolicitado?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Libera los recursos utilizados
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

        /// <summary>
        /// Método público para actualizar la lista de propiedades inactivas desde fuera del panel
        /// </summary>
        public void ActualizarPropiedadesInactivas()
        {
            CargarCasasInactivasDesdeBD();
            CargarPropiedadesInactivas();
        }
    }
}
