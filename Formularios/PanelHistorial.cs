using FlujoDeCajaApp.Modelos;

namespace FlujoDeCajaApp.Formularios
{
    /// <summary>
    /// Panel para mostrar el historial de movimientos con filtros
    /// </summary>
    public partial class PanelHistorial : UserControl
    {
        private Panel panelTitulo = null!;
        private Label lblTitulo = null!;
        private Panel panelFiltros = null!;
        private TextBox txtBuscarHistorial = null!;
        private DateTimePicker dtpFechaInicio = null!;
        private DateTimePicker dtpFechaFin = null!;
        private Button btnFiltrar = null!;
        private Button btnLimpiar = null!;
        private Button btnVolver = null!;
        private DataGridView dgvHistorial = null!;
        
        // Lista de movimientos (datos de ejemplo)
        private List<Movimiento> movimientos = null!;
        private List<Movimiento> movimientosFiltrados = null!;
        
        // Evento para notificar al formulario padre
        public event EventHandler? VolverSolicitado;

        public PanelHistorial()
        {
            InitializeComponent();
            InicializarDatosEjemplo();
            CargarHistorial();
        }

        /// <summary>
        /// Inicializa datos de ejemplo para el historial
        /// </summary>
        private void InicializarDatosEjemplo()
        {
            movimientos = new List<Movimiento>
            {
                new Movimiento(1, "Ingreso", 1500.00m, "Renta mensual") 
                { 
                    NombrePropiedad = "CASA ALICIA", 
                    Fecha = DateTime.Now.AddDays(-5),
                    UsuarioCreador = "admin",
                    Categoria = "Renta"
                },
                new Movimiento(2, "Gasto", 250.00m, "Mantenimiento piscina") 
                { 
                    NombrePropiedad = "CASA AMANECER", 
                    Fecha = DateTime.Now.AddDays(-3),
                    UsuarioCreador = "admin",
                    Categoria = "Mantenimiento"
                },
                new Movimiento(1, "Ingreso", 800.00m, "Depósito de garantía") 
                { 
                    NombrePropiedad = "CASA ALICIA", 
                    Fecha = DateTime.Now.AddDays(-2),
                    UsuarioCreador = "admin",
                    Categoria = "Depósito"
                },
                new Movimiento(3, "Gasto", 150.00m, "Limpieza") 
                { 
                    NombrePropiedad = "CASA ARBOLES", 
                    Fecha = DateTime.Now.AddDays(-1),
                    UsuarioCreador = "admin",
                    Categoria = "Servicio"
                },
                new Movimiento(2, "Ingreso", 2000.00m, "Renta semanal") 
                { 
                    NombrePropiedad = "CASA AMANECER", 
                    Fecha = DateTime.Now,
                    UsuarioCreador = "admin",
                    Categoria = "Renta"
                }
            };
            
            movimientosFiltrados = new List<Movimiento>(movimientos);
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
                Text = "Historial de Movimientos",
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
                ForeColor = Color.FromArgb(52, 152, 219),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnVolver.FlatAppearance.BorderSize = 1;
            btnVolver.FlatAppearance.BorderColor = Color.White;
            btnVolver.Click += BtnVolver_Click;

            panelTitulo.Controls.AddRange(new Control[] { lblTitulo, btnVolver });

            // Panel de filtros
            panelFiltros = new Panel
            {
                Size = new Size(1000, 80),
                Location = new Point(0, 80),
                BackColor = Color.FromArgb(40, 40, 40),
                Dock = DockStyle.Top
            };

            // Campo de búsqueda
            Label lblBuscar = new Label
            {
                Text = "Buscar:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(60, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtBuscarHistorial = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 35),
                Size = new Size(200, 25),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtBuscarHistorial.TextChanged += TxtBuscarHistorial_TextChanged;

            // Filtro por fecha inicio
            Label lblFechaInicio = new Label
            {
                Text = "Fecha Inicio:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                Location = new Point(250, 15),
                Size = new Size(80, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dtpFechaInicio = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(250, 35),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddMonths(-1)
            };

            // Filtro por fecha fin
            Label lblFechaFin = new Label
            {
                Text = "Fecha Fin:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                Location = new Point(420, 15),
                Size = new Size(80, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dtpFechaFin = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(420, 35),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };

            // Botón filtrar
            btnFiltrar = new Button
            {
                Text = "Filtrar",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Size = new Size(80, 25),
                Location = new Point(590, 35),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnFiltrar.FlatAppearance.BorderSize = 0;
            btnFiltrar.Click += BtnFiltrar_Click;

            // Botón limpiar
            btnLimpiar = new Button
            {
                Text = "Limpiar",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Size = new Size(80, 25),
                Location = new Point(680, 35),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLimpiar.FlatAppearance.BorderSize = 0;
            btnLimpiar.Click += BtnLimpiar_Click;

            panelFiltros.Controls.AddRange(new Control[] {
                lblBuscar, txtBuscarHistorial, lblFechaInicio, dtpFechaInicio,
                lblFechaFin, dtpFechaFin, btnFiltrar, btnLimpiar
            });

            // DataGridView para mostrar el historial
            dgvHistorial = new DataGridView
            {
                Location = new Point(20, 180),
                Size = new Size(960, 400),
                BackgroundColor = Color.FromArgb(45, 45, 45),
                GridColor = Color.FromArgb(60, 60, 60),
                BorderStyle = BorderStyle.FixedSingle,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                Dock = DockStyle.Fill
            };

            // Configurar columnas
            ConfigurarColumnas();

            // Agregar controles al UserControl
            this.Controls.AddRange(new Control[] { dgvHistorial, panelFiltros, panelTitulo });

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Configura las columnas del DataGridView
        /// </summary>
        private void ConfigurarColumnas()
        {
            dgvHistorial.Columns.Add("Fecha", "Fecha");
            dgvHistorial.Columns.Add("Propiedad", "Propiedad");
            dgvHistorial.Columns.Add("Tipo", "Tipo");
            dgvHistorial.Columns.Add("Categoria", "Categoría");
            dgvHistorial.Columns.Add("Descripcion", "Descripción");
            dgvHistorial.Columns.Add("Monto", "Monto");
            dgvHistorial.Columns.Add("Usuario", "Usuario");

            // Configurar anchos específicos
            foreach (DataGridViewColumn columna in dgvHistorial.Columns)
            {
                switch (columna.Name)
                {
                    case "Fecha": columna.Width = 100; break;
                    case "Propiedad": columna.Width = 150; break;
                    case "Tipo": columna.Width = 80; break;
                    case "Categoria": columna.Width = 100; break;
                    case "Descripcion": columna.Width = 200; break;
                    case "Monto": 
                        columna.Width = 100;
                        columna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        columna.DefaultCellStyle.Format = "C2";
                        break;
                    case "Usuario": columna.Width = 80; break;
                }
            }

            // Estilo del encabezado
            dgvHistorial.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dgvHistorial.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHistorial.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvHistorial.EnableHeadersVisualStyles = false;
            
            // Estilo de las celdas para tema oscuro
            dgvHistorial.DefaultCellStyle.BackColor = Color.FromArgb(45, 45, 45);
            dgvHistorial.DefaultCellStyle.ForeColor = Color.White;
            dgvHistorial.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dgvHistorial.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvHistorial.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
            dgvHistorial.AlternatingRowsDefaultCellStyle.ForeColor = Color.White;
        }

        /// <summary>
        /// Carga los datos del historial en el DataGridView
        /// </summary>
        private void CargarHistorial()
        {
            dgvHistorial.Rows.Clear();
            
            foreach (var movimiento in movimientosFiltrados.OrderByDescending(m => m.Fecha))
            {
                dgvHistorial.Rows.Add(
                    movimiento.Fecha.ToString("dd/MM/yyyy"),
                    movimiento.NombrePropiedad,
                    movimiento.TipoMovimiento,
                    movimiento.Categoria,
                    movimiento.Descripcion,
                    movimiento.Monto,
                    movimiento.UsuarioCreador
                );

                // Colorear fila según el tipo de movimiento
                int indice = dgvHistorial.Rows.Count - 1;
                if (movimiento.TipoMovimiento == "Ingreso")
                {
                    dgvHistorial.Rows[indice].DefaultCellStyle.ForeColor = Color.FromArgb(39, 174, 96);
                }
                else
                {
                    dgvHistorial.Rows[indice].DefaultCellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                }
            }

            // Mostrar total de registros
            ActualizarConteoRegistros();
        }

        /// <summary>
        /// Actualiza el conteo de registros mostrados
        /// </summary>
        private void ActualizarConteoRegistros()
        {
            lblTitulo.Text = $"Historial de Movimientos ({movimientosFiltrados.Count} registros)";
        }

        // ===== EVENTOS =====

        private void TxtBuscarHistorial_TextChanged(object? sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void BtnFiltrar_Click(object? sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void BtnLimpiar_Click(object? sender, EventArgs e)
        {
            txtBuscarHistorial.Text = "";
            dtpFechaInicio.Value = DateTime.Now.AddMonths(-1);
            dtpFechaFin.Value = DateTime.Now;
            movimientosFiltrados = new List<Movimiento>(movimientos);
            CargarHistorial();
        }
        
        private void BtnVolver_Click(object? sender, EventArgs e)
        {
            VolverSolicitado?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Aplica los filtros de búsqueda y fecha
        /// </summary>
        private void AplicarFiltros()
        {
            var query = movimientos.AsEnumerable();

            // Filtro por texto
            if (!string.IsNullOrWhiteSpace(txtBuscarHistorial.Text))
            {
                string filtro = txtBuscarHistorial.Text.ToLower();
                query = query.Where(m => 
                    m.NombrePropiedad.ToLower().Contains(filtro) ||
                    m.Descripcion.ToLower().Contains(filtro) ||
                    m.Categoria.ToLower().Contains(filtro) ||
                    m.TipoMovimiento.ToLower().Contains(filtro)
                );
            }

            // Filtro por fecha
            query = query.Where(m => m.Fecha.Date >= dtpFechaInicio.Value.Date && 
                                   m.Fecha.Date <= dtpFechaFin.Value.Date);

            movimientosFiltrados = query.ToList();
            CargarHistorial();
        }
    }
}
