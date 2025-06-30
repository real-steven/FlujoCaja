using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlujoDeCajaApp.Formularios
{
    /// <summary>
    /// Panel para agregar entidades al sistema
    /// (Propiedades, Dueños, Usuarios, Categorías, Movimientos)
    /// </summary>
    public partial class PanelAgregar : UserControl
    {
        private Panel panelTitulo = null!;
        private Label lblTitulo = null!;
        private Panel panelOpciones = null!;
        private Button btnAgregarPropiedad = null!;
        private Button btnAgregarDueno = null!;
        private Button btnAgregarUsuario = null!;
        private Button btnAgregarCategoria = null!;
        private Button btnAgregarMovimiento = null!;
        private Button btnVolver = null!;
        
        // Evento para notificar al formulario padre
        public event EventHandler? VolverSolicitado;

        public PanelAgregar()
        {
            InitializeComponent();
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
                BackColor = Color.FromArgb(52, 152, 219),
                Dock = DockStyle.Top
            };

            // Título
            lblTitulo = new Label
            {
                Text = "Agregar Nuevo",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(50, 20),
                Size = new Size(300, 40),
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
                ForeColor = Color.FromArgb(52, 152, 219),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnVolver.FlatAppearance.BorderSize = 0;
            btnVolver.Click += BtnVolver_Click;

            panelTitulo.Controls.AddRange(new Control[] { lblTitulo, btnVolver });

            // Panel de opciones
            panelOpciones = new Panel
            {
                Size = new Size(800, 450),
                Location = new Point(100, 120),
                BackColor = Color.Transparent
            };

            // Crear botones
            CrearBotones();

            // Agregar paneles al control
            this.Controls.AddRange(new Control[] { panelTitulo, panelOpciones });

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Crea todos los botones del panel
        /// </summary>
        private void CrearBotones()
        {
            // Botón Agregar Casa
            btnAgregarPropiedad = CrearBotonOpcion("�️", "Nueva Casa", "Agregar una nueva casa al sistema", 0, 0);
            btnAgregarPropiedad.Click += BtnAgregarPropiedad_Click;

            // Botón Agregar Dueño
            btnAgregarDueno = CrearBotonOpcion("�‍💼", "Nuevo Dueño", "Registrar un nuevo propietario", 1, 0);
            btnAgregarDueno.Click += BtnAgregarDueno_Click;

            // Botón Agregar Usuario
            btnAgregarUsuario = CrearBotonOpcion("�", "Nuevo Usuario", "Registrar un nuevo usuario del sistema", 0, 1);
            btnAgregarUsuario.Click += BtnAgregarUsuario_Click;

            // Botón Agregar Categoría
            btnAgregarCategoria = CrearBotonOpcion("�", "Nueva Categoría", "Crear una nueva categoría de propiedad", 1, 1);
            btnAgregarCategoria.Click += BtnAgregarCategoria_Click;

            // Botón Agregar Movimiento
            btnAgregarMovimiento = CrearBotonOpcion("�", "Nuevo Movimiento", "Registrar ingreso o gasto", 0, 2);
            btnAgregarMovimiento.Click += BtnAgregarMovimiento_Click;

            panelOpciones.Controls.AddRange(new Control[] {
                btnAgregarPropiedad, btnAgregarDueno, btnAgregarUsuario, 
                btnAgregarCategoria, btnAgregarMovimiento
            });
        }

        /// <summary>
        /// Crea un botón de opción con estilo uniforme
        /// </summary>
        /// <param name="icono">Icono del botón</param>
        /// <param name="titulo">Título del botón</param>
        /// <param name="descripcion">Descripción de la función</param>
        /// <param name="columna">Columna en la cuadrícula (0-1)</param>
        /// <param name="fila">Fila en la cuadrícula (0-1)</param>
        /// <returns>Botón configurado</returns>
        private Button CrearBotonOpcion(string icono, string titulo, string descripcion, int columna, int fila)
        {
            Button btn = new Button
            {
                Size = new Size(350, 150),
                Location = new Point(columna * 380, fila * 180),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.TopLeft,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Text = "" // Limpiamos el texto porque lo dibujaremos manualmente
            };

            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 70, 70);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 30, 30);

            // Crear el contenido del botón usando Paint
            btn.Paint += (sender, e) =>
            {
                // Configurar la calidad del renderizado
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                // Área del botón
                Rectangle rect = new Rectangle(0, 0, btn.Width, btn.Height);

                // Dibujar fondo con gradiente sutil
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    rect, 
                    Color.FromArgb(60, 60, 60), 
                    Color.FromArgb(40, 40, 40), 
                    45f))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }

                // Icono - posición centrada horizontalmente en la parte superior
                var iconFont = new Font("Segoe UI Emoji", 28, FontStyle.Regular);
                var iconSize = e.Graphics.MeasureString(icono, iconFont);
                float iconX = (btn.Width - iconSize.Width) / 2;
                float iconY = 15;
                
                e.Graphics.DrawString(icono, iconFont, 
                                    new SolidBrush(Color.FromArgb(52, 152, 219)), 
                                    new PointF(iconX, iconY));

                // Título - centrado horizontalmente
                var titleFont = new Font("Segoe UI", 14, FontStyle.Bold);
                var titleSize = e.Graphics.MeasureString(titulo, titleFont);
                float titleX = (btn.Width - titleSize.Width) / 2;
                float titleY = iconY + iconSize.Height + 5;
                
                e.Graphics.DrawString(titulo, titleFont, 
                                    new SolidBrush(Color.White), 
                                    new PointF(titleX, titleY));

                // Descripción - centrada horizontalmente
                var descFont = new Font("Segoe UI", 9, FontStyle.Regular);
                var descSize = e.Graphics.MeasureString(descripcion, descFont, btn.Width - 20);
                float descX = (btn.Width - descSize.Width) / 2;
                float descY = titleY + titleSize.Height + 3;
                
                var descRect = new RectangleF(10, descY, btn.Width - 20, btn.Height - descY - 10);
                var format = new StringFormat() 
                { 
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Near,
                    Trimming = StringTrimming.Word
                };
                
                e.Graphics.DrawString(descripcion, descFont, 
                                    new SolidBrush(Color.FromArgb(180, 180, 180)), 
                                    descRect, format);

                // Borde sutil
                using (var borderPen = new Pen(Color.FromArgb(100, 100, 100), 1))
                {
                    e.Graphics.DrawRectangle(borderPen, 0, 0, btn.Width - 1, btn.Height - 1);
                }

                // Cleanup
                iconFont.Dispose();
                titleFont.Dispose();
                descFont.Dispose();
                format.Dispose();
            };

            // Efecto hover
            btn.MouseEnter += (sender, e) =>
            {
                btn.Invalidate(); // Redibuja el botón
            };

            btn.MouseLeave += (sender, e) =>
            {
                btn.Invalidate(); // Redibuja el botón
            };

            return btn;
        }

        #region Eventos de botones

        private void BtnAgregarPropiedad_Click(object? sender, EventArgs e)
        {
            try
            {
                using (var formularioAgregarCasa = new AgregarCasaForm())
                {
                    if (formularioAgregarCasa.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show("Casa agregada exitosamente.\n\nLos datos estarán disponibles inmediatamente.", 
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        VolverSolicitado?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el formulario de agregar casa: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAgregarDueno_Click(object? sender, EventArgs e)
        {
            try
            {
                using (var formularioAgregarDueno = new AgregarDuenoForm())
                {
                    if (formularioAgregarDueno.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show("Dueño agregado exitosamente.\n\nLos datos estarán disponibles inmediatamente.", 
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        VolverSolicitado?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el formulario de agregar dueño: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAgregarUsuario_Click(object? sender, EventArgs e)
        {
            try
            {
                using (var formularioAgregarUsuario = new AgregarUsuarioForm())
                {
                    if (formularioAgregarUsuario.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show("Usuario agregado exitosamente.\n\nLos datos estarán disponibles inmediatamente.", 
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        VolverSolicitado?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el formulario de agregar usuario: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAgregarCategoria_Click(object? sender, EventArgs e)
        {
            try
            {
                using (var formularioAgregarCategoria = new AgregarCategoriaForm())
                {
                    if (formularioAgregarCategoria.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show("Categoría agregada exitosamente.\n\nLos datos estarán disponibles inmediatamente.", 
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        VolverSolicitado?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el formulario de agregar categoría: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAgregarMovimiento_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Agregar Nuevo Movimiento\n\nEsta funcionalidad se implementará más adelante.\n\nIncluirá formulario con:\n• Tipo (Ingreso/Gasto)\n• Monto\n• Descripción\n• Propiedad\n• Categoría", 
                          "Nuevo Movimiento", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void BtnVolver_Click(object? sender, EventArgs e)
        {
            VolverSolicitado?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
