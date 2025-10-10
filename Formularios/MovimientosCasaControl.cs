using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using FlujoDeCajaApp.Data;

namespace FlujoDeCajaApp.Formularios
{
    /// <summary>
    /// Panel para gestionar los movimientos financieros de una casa específica
    /// Permite agregar, editar y eliminar ingresos y gastos filtrados por mes/año
    /// </summary>
    public partial class MovimientosCasaControl : UserControl
    {
        #region Variables privadas

        private int casaIdActual;
        private string nombreCasaActual = "";
        private DateTime fechaActual = DateTime.Now;
        private List<MovimientoFila> movimientos = new();
        private bool cargandoDatos = false;
        private bool enModoEdicion = false; // Variable para controlar el modo de edición
        private int filaEnEdicion = -1; // Índice de la fila que está siendo editada

        #endregion

        #region Eventos

        /// <summary>
        /// Evento que se dispara cuando el usuario solicita volver al menú anterior
        /// </summary>
        public event EventHandler<EventArgs>? VolverSolicitado;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor del control
        /// </summary>
        public MovimientosCasaControl()
        {
            InitializeComponent();
            InicializarControl();
        }

        #endregion

        #region Métodos públicos

        /// <summary>
        /// Configura el control para mostrar los movimientos de una casa específica
        /// </summary>
        /// <param name="casaId">ID de la casa</param>
        /// <param name="nombreCasa">Nombre de la casa</param>
        public void ConfigurarCasa(int casaId, string nombreCasa)
        {
            casaIdActual = casaId;
            nombreCasaActual = nombreCasa;
            
            // Actualizar el título
            lblTituloCasa.Text = $"Movimientos - {nombreCasa}";
            
            // Cargar datos del mes actual
            CargarMovimientos();
        }

        #endregion

        #region Métodos de inicialización

        /// <summary>
        /// Inicializa los controles y configuraciones del panel
        /// </summary>
        private void InicializarControl()
        {
            try
            {
                ConfigurarDatePickers();
                ConfigurarDataGridView();
                ConfigurarEventos();
                InicializarCalculos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar el control: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Configura los selectores de fecha (mes y año)
        /// </summary>
        private void ConfigurarDatePickers()
        {
            // Configurar ComboBox de meses
            cmbMes.Items.Clear();
            string[] nombresMeses = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            for (int i = 0; i < 12; i++)
            {
                cmbMes.Items.Add($"{i + 1:00} - {nombresMeses[i]}");
            }
            cmbMes.SelectedIndex = fechaActual.Month - 1;

            // Configurar NumericUpDown de año
            nudAño.Minimum = 2020;
            nudAño.Maximum = 2030;
            nudAño.Value = fechaActual.Year;
        }

        /// <summary>
        /// Configura el DataGridView para mostrar los movimientos
        /// </summary>
        private void ConfigurarDataGridView()
        {
            dgvMovimientos.AutoGenerateColumns = false;
            dgvMovimientos.AllowUserToAddRows = false;
            dgvMovimientos.AllowUserToDeleteRows = false;
            dgvMovimientos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMovimientos.MultiSelect = false;
            dgvMovimientos.RowHeadersVisible = false;
            dgvMovimientos.BackgroundColor = Color.White;
            dgvMovimientos.BorderStyle = BorderStyle.FixedSingle;
            dgvMovimientos.GridColor = Color.FromArgb(220, 220, 220);
            dgvMovimientos.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvMovimientos.ReadOnly = true; // Por defecto solo lectura
            
            // Evitar errores de binding
            dgvMovimientos.DataError += (s, e) => {
                Console.WriteLine($"DataError en fila {e.RowIndex}, columna {e.ColumnIndex}: {e.Exception?.Message}");
                e.Cancel = true;
            };
            
            // Crear columnas
            CrearColumnas();
            
            // Configurar estilos
            ConfigurarEstilosDataGridView();
        }

        /// <summary>
        /// Crea las columnas del DataGridView
        /// </summary>
        private void CrearColumnas()
        {
            dgvMovimientos.Columns.Clear();

            // Columna Fecha
            var colFecha = new DataGridViewTextBoxColumn
            {
                Name = "colFecha",
                HeaderText = "Fecha",
                DataPropertyName = "FechaTexto",
                Width = 100,
                ReadOnly = true
            };
            dgvMovimientos.Columns.Add(colFecha);

            // Columna Descripción
            var colDescripcion = new DataGridViewTextBoxColumn
            {
                Name = "colDescripcion",
                HeaderText = "Descripción",
                DataPropertyName = "Descripcion",
                Width = 250,
                ReadOnly = true
            };
            dgvMovimientos.Columns.Add(colDescripcion);

            // Columna Monto
            var colMonto = new DataGridViewTextBoxColumn
            {
                Name = "colMonto",
                HeaderText = "Monto",
                DataPropertyName = "MontoTexto",
                Width = 120,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "C2"
                }
            };
            dgvMovimientos.Columns.Add(colMonto);

            // Columna Categoría
            var colCategoria = new DataGridViewComboBoxColumn
            {
                Name = "colCategoria",
                HeaderText = "Categoría",
                DataPropertyName = "Categoria",
                Width = 150,
                ReadOnly = true,
                DataSource = DatabaseHelper.ObtenerCategoriasMovimientos(),
                FlatStyle = FlatStyle.Flat
            };
            dgvMovimientos.Columns.Add(colCategoria);

            // Columna Opciones (tres puntos)
            var colOpciones = new DataGridViewButtonColumn
            {
                Name = "colOpciones",
                HeaderText = "⋮",
                Width = 50,
                Text = "⋮",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat
            };
            dgvMovimientos.Columns.Add(colOpciones);
        }

        /// <summary>
        /// Configura los estilos visuales del DataGridView
        /// </summary>
        private void ConfigurarEstilosDataGridView()
        {
            // Estilo del encabezado
            dgvMovimientos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvMovimientos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvMovimientos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvMovimientos.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMovimientos.ColumnHeadersHeight = 40;

            // Estilo de las filas
            dgvMovimientos.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvMovimientos.DefaultCellStyle.BackColor = Color.White;
            dgvMovimientos.DefaultCellStyle.ForeColor = Color.FromArgb(44, 62, 80);
            dgvMovimientos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dgvMovimientos.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvMovimientos.RowTemplate.Height = 35;

            // Estilo de filas alternadas
            dgvMovimientos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
        }

        /// <summary>
        /// Configura los eventos de los controles
        /// </summary>
        private void ConfigurarEventos()
        {
            // Eventos de filtros de fecha
            cmbMes.SelectedIndexChanged += (s, e) => { if (!cargandoDatos) CargarMovimientos(); };
            nudAño.ValueChanged += (s, e) => { if (!cargandoDatos) CargarMovimientos(); };

            // Eventos del DataGridView
            dgvMovimientos.CellClick += DgvMovimientos_CellClick;
            dgvMovimientos.CellValueChanged += DgvMovimientos_CellValueChanged;
            dgvMovimientos.CellFormatting += DgvMovimientos_CellFormatting;
            dgvMovimientos.CellEndEdit += DgvMovimientos_CellEndEdit;

            // Evento especial para detectar cuando se termina la edición automática
            dgvMovimientos.CellLeave += (s, e) => {
                if (enModoEdicion && e.RowIndex != filaEnEdicion)
                {
                    // El usuario salió de la fila en edición, terminar edición automáticamente
                    DeshabilitarEdicion();
                }
            };

            // Eventos de botones superiores
            btnGenerar.Click += (s, e) => InsertarDatosPrueba(); // Temporal: datos de prueba
            btnExportar.Click += (s, e) => MessageBox.Show("Función 'Exportar' en desarrollo", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnExtraer.Click += (s, e) => MessageBox.Show("Función 'Extraer' en desarrollo", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Evento del botón agregar reglones
            btnAgregarReglones.Click += BtnAgregarReglones_Click;

            // Evento del botón volver
            btnVolver.Click += (s, e) => VolverSolicitado?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Inicializa los labels de cálculos
        /// </summary>
        private void InicializarCalculos()
        {
            lblBalanceAnterior.Text = "₡0.00";
            lblTotalGastos.Text = "₡0.00";
            lblTotalIngresos.Text = "₡0.00";
            lblBalanceActual.Text = "₡0.00";
        }

        #endregion

        #region Métodos de datos

        /// <summary>
        /// Carga los movimientos del mes y año seleccionados
        /// </summary>
        private void CargarMovimientos()
        {
            if (casaIdActual <= 0) return;

            try
            {
                cargandoDatos = true;
                
                int mesSeleccionado = cmbMes.SelectedIndex + 1;
                int añoSeleccionado = (int)nudAño.Value;

                // Obtener movimientos de la base de datos
                var movimientosBD = DatabaseHelper.ObtenerMovimientosPorMes(casaIdActual, añoSeleccionado, mesSeleccionado);
                
                // Convertir a objetos MovimientoFila
                movimientos.Clear();
                foreach (var mov in movimientosBD)
                {
                    movimientos.Add(new MovimientoFila
                    {
                        Id = mov.Id,
                        Fecha = mov.Fecha,
                        Descripcion = mov.Descripcion,
                        Monto = mov.Monto,
                        Categoria = mov.Categoria
                    });
                }

                // Actualizar el DataGridView
                ActualizarDataGridView();
                
                // Actualizar cálculos
                ActualizarCalculos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar movimientos: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cargandoDatos = false;
            }
        }

        /// <summary>
        /// Actualiza el contenido del DataGridView
        /// </summary>
        private void ActualizarDataGridView()
        {
            try
            {
                // Suspender el layout para evitar múltiples actualizaciones
                dgvMovimientos.SuspendLayout();
                
                // Limpiar selección y CurrentCell antes de cambiar DataSource
                dgvMovimientos.ClearSelection();
                dgvMovimientos.CurrentCell = null;
                
                // Crear una nueva BindingList para forzar actualización completa
                var bindingList = new System.ComponentModel.BindingList<MovimientoFila>(movimientos);
                dgvMovimientos.DataSource = bindingList;
                
                dgvMovimientos.ResumeLayout();
                dgvMovimientos.Refresh();
                
                Console.WriteLine($"DataGridView actualizado: {dgvMovimientos.Rows.Count} filas");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarDataGridView: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza los cálculos de balance, ingresos y gastos
        /// </summary>
        private void ActualizarCalculos()
        {
            try
            {
                int mesSeleccionado = cmbMes.SelectedIndex + 1;
                int añoSeleccionado = (int)nudAño.Value;

                // Balance anterior
                decimal balanceAnterior = DatabaseHelper.ObtenerBalanceAnterior(casaIdActual, añoSeleccionado, mesSeleccionado);
                lblBalanceAnterior.Text = FormatearMoneda(balanceAnterior);

                // Totales del mes actual
                decimal totalIngresos = movimientos.Where(m => m.Monto > 0).Sum(m => m.Monto);
                decimal totalGastos = Math.Abs(movimientos.Where(m => m.Monto < 0).Sum(m => m.Monto));
                decimal balanceActual = totalIngresos - totalGastos;

                lblTotalIngresos.Text = FormatearMoneda(totalIngresos);
                lblTotalIngresos.ForeColor = totalIngresos > 0 ? Color.FromArgb(39, 174, 96) : Color.Black;

                lblTotalGastos.Text = FormatearMoneda(totalGastos);
                lblTotalGastos.ForeColor = totalGastos > 0 ? Color.FromArgb(231, 76, 60) : Color.Black;

                lblBalanceActual.Text = FormatearMoneda(balanceActual);
                lblBalanceActual.ForeColor = balanceActual >= 0 ? Color.FromArgb(39, 174, 96) : Color.FromArgb(231, 76, 60);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar cálculos: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Formatea un valor decimal como moneda
        /// </summary>
        /// <param name="valor">Valor a formatear</param>
        /// <returns>Texto formateado</returns>
        private string FormatearMoneda(decimal valor)
        {
            return $"₡{valor:N2}";
        }

        #endregion

        #region Eventos del DataGridView

        /// <summary>
        /// Maneja el evento de click en las celdas del DataGridView
        /// </summary>
        private void DgvMovimientos_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.RowIndex >= dgvMovimientos.Rows.Count) return;

                // Si estamos en modo edición y se hace clic en una fila diferente, terminar edición
                if (enModoEdicion && e.RowIndex != filaEnEdicion)
                {
                    DeshabilitarEdicion();
                    return;
                }

                // Si se hizo clic en la columna de opciones (tres puntos)
                var colOpciones = dgvMovimientos.Columns["colOpciones"];
                if (colOpciones != null && e.ColumnIndex == colOpciones.Index)
                {
                    MostrarMenuOpciones(e.RowIndex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DgvMovimientos_CellClick: {ex.Message}");
            }
        }

        /// <summary>
        /// Maneja el evento de cambio de valor en las celdas
        /// </summary>
        private void DgvMovimientos_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || cargandoDatos) return;

            try
            {
                var movimiento = movimientos[e.RowIndex];
                ActualizarMovimientoDesdeGrid(movimiento, e.RowIndex);
                ActualizarCalculos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar movimiento: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CargarMovimientos(); // Recargar datos en caso de error
            }
        }

        /// <summary>
        /// Maneja el formato de las celdas para mostrar colores según el monto
        /// </summary>
        private void DgvMovimientos_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Colorear la columna de monto según sea ingreso (verde) o gasto (rojo)
            var colMonto = dgvMovimientos.Columns["colMonto"];
            if (colMonto != null && e.ColumnIndex == colMonto.Index && e.RowIndex < movimientos.Count)
            {
                var monto = movimientos[e.RowIndex].Monto;
                if (monto > 0)
                {
                    e.CellStyle.ForeColor = Color.FromArgb(39, 174, 96); // Verde para ingresos
                }
                else if (monto < 0)
                {
                    e.CellStyle.ForeColor = Color.FromArgb(231, 76, 60); // Rojo para gastos
                }
            }
        }

        /// <summary>
        /// Maneja el fin de edición de una celda
        /// </summary>
        private void DgvMovimientos_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            // Guardar los cambios
            if (e.RowIndex >= 0 && e.RowIndex < movimientos.Count && !cargandoDatos)
            {
                try
                {
                    var movimiento = movimientos[e.RowIndex];
                    ActualizarMovimientoDesdeGrid(movimiento, e.RowIndex);
                    ActualizarCalculos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar movimiento: {ex.Message}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Métodos auxiliares

        /// <summary>
        /// Muestra el menú contextual con opciones para editar/eliminar
        /// </summary>
        /// <param name="filaIndex">Índice de la fila</param>
        private void MostrarMenuOpciones(int filaIndex)
        {
            var contextMenu = new ContextMenuStrip();
            
            // Opción Editar - Permitir editar toda la fila
            var menuEditar = new ToolStripMenuItem("Editar Fila", null, (s, e) => {
                HabilitarEdicionFila(filaIndex);
            });
            
            // Opción Eliminar
            var menuEliminar = new ToolStripMenuItem("Eliminar", null, (s, e) => {
                EliminarMovimiento(filaIndex);
            });
            
            contextMenu.Items.Add(menuEditar);
            contextMenu.Items.Add(menuEliminar);
            
            // Mostrar el menú en la posición del mouse
            var colOpciones = dgvMovimientos.Columns["colOpciones"];
            if (colOpciones != null)
            {
                var cellRect = dgvMovimientos.GetCellDisplayRectangle(colOpciones.Index, filaIndex, false);
                var punto = dgvMovimientos.PointToScreen(new Point(cellRect.X, cellRect.Y + cellRect.Height));
                contextMenu.Show(punto);
            }
        }

        /// <summary>
        /// Habilita edición para una fila específica de forma automática (sin mostrar mensaje)
        /// </summary>
        private void HabilitarEdicionFilaAutomatica(int filaIndex)
        {
            if (filaIndex < 0 || filaIndex >= dgvMovimientos.Rows.Count) return;

            try
            {
                // Primero deshabilitar cualquier edición en curso
                if (enModoEdicion)
                {
                    DeshabilitarEdicion();
                }

                // Establecer el modo de edición
                enModoEdicion = true;
                filaEnEdicion = filaIndex;

                // Habilitar edición en las columnas editables
                var colFecha = dgvMovimientos.Columns["colFecha"];
                var colDescripcion = dgvMovimientos.Columns["colDescripcion"];
                var colMonto = dgvMovimientos.Columns["colMonto"];
                var colCategoria = dgvMovimientos.Columns["colCategoria"];

                if (colFecha != null) colFecha.ReadOnly = false;
                if (colDescripcion != null) colDescripcion.ReadOnly = false;
                if (colMonto != null) colMonto.ReadOnly = false;
                if (colCategoria != null) colCategoria.ReadOnly = false;

                // Configurar el DataGridView para edición normal
                dgvMovimientos.ReadOnly = false;
                dgvMovimientos.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
                dgvMovimientos.SelectionMode = DataGridViewSelectionMode.CellSelect;
                
                // Seleccionar la primera celda editable de forma segura
                dgvMovimientos.ClearSelection();
                if (filaIndex < dgvMovimientos.Rows.Count && dgvMovimientos.Columns["colDescripcion"] != null)
                {
                    var celda = dgvMovimientos.Rows[filaIndex].Cells["colDescripcion"];
                    if (celda != null)
                    {
                        dgvMovimientos.CurrentCell = celda;
                        // Opcional: entrar inmediatamente en modo de edición
                        dgvMovimientos.BeginEdit(true);
                    }
                }
                
                Console.WriteLine($"Edición automática habilitada en fila {filaIndex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al habilitar edición automática: {ex.Message}");
                DeshabilitarEdicion();
            }
        }

        /// <summary>
        /// Habilita edición para una fila específica
        /// </summary>
        private void HabilitarEdicionFila(int filaIndex)
        {
            if (filaIndex < 0 || filaIndex >= dgvMovimientos.Rows.Count) return;

            try
            {
                // Primero deshabilitar cualquier edición en curso
                if (enModoEdicion)
                {
                    DeshabilitarEdicion();
                }

                // Establecer el modo de edición
                enModoEdicion = true;
                filaEnEdicion = filaIndex;

                // Habilitar edición en las columnas editables
                var colFecha = dgvMovimientos.Columns["colFecha"];
                var colDescripcion = dgvMovimientos.Columns["colDescripcion"];
                var colMonto = dgvMovimientos.Columns["colMonto"];
                var colCategoria = dgvMovimientos.Columns["colCategoria"];

                if (colFecha != null) colFecha.ReadOnly = false;
                if (colDescripcion != null) colDescripcion.ReadOnly = false;
                if (colMonto != null) colMonto.ReadOnly = false;
                if (colCategoria != null) colCategoria.ReadOnly = false;

                // Configurar el DataGridView para edición normal
                dgvMovimientos.ReadOnly = false;
                dgvMovimientos.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
                dgvMovimientos.SelectionMode = DataGridViewSelectionMode.CellSelect;
                
                // Seleccionar la primera celda editable de forma segura
                dgvMovimientos.ClearSelection();
                if (filaIndex < dgvMovimientos.Rows.Count && dgvMovimientos.Columns["colFecha"] != null)
                {
                    var celda = dgvMovimientos.Rows[filaIndex].Cells["colFecha"];
                    if (celda != null)
                    {
                        dgvMovimientos.CurrentCell = celda;
                    }
                }
                
                MessageBox.Show("Fila en modo edición. Haga clic en las celdas para editarlas. Haga clic fuera de la fila para terminar.", 
                    "Modo Edición", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al habilitar edición: {ex.Message}");
                MessageBox.Show($"Error al habilitar edición: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DeshabilitarEdicion();
            }
        }

        /// <summary>
        /// Deshabilita la edición y vuelve al modo de solo lectura
        /// </summary>
        private void DeshabilitarEdicion()
        {
            try
            {
                // Terminar cualquier edición en curso
                if (dgvMovimientos.IsCurrentCellInEditMode)
                {
                    dgvMovimientos.EndEdit();
                }

                // Resetear variables de control
                enModoEdicion = false;
                filaEnEdicion = -1;

                // Volver a poner todas las columnas en modo de solo lectura
                foreach (DataGridViewColumn column in dgvMovimientos.Columns)
                {
                    if (column.Name != "colOpciones")
                    {
                        column.ReadOnly = true;
                    }
                }

                // Volver al modo de solo lectura
                dgvMovimientos.EditMode = DataGridViewEditMode.EditProgrammatically;
                dgvMovimientos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvMovimientos.ReadOnly = true;
                
                // Limpiar selección de forma segura
                dgvMovimientos.ClearSelection();
                dgvMovimientos.CurrentCell = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeshabilitarEdicion: {ex.Message}");
                // Si hay error, forzar el reseteo
                enModoEdicion = false;
                filaEnEdicion = -1;
                dgvMovimientos.EditMode = DataGridViewEditMode.EditProgrammatically;
                dgvMovimientos.ReadOnly = true;
                dgvMovimientos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
        }

        /// <summary>
        /// Elimina un movimiento después de confirmar con el usuario
        /// </summary>
        /// <param name="filaIndex">Índice de la fila a eliminar</param>
        private void EliminarMovimiento(int filaIndex)
        {
            if (filaIndex < 0 || filaIndex >= movimientos.Count) return;

            var movimiento = movimientos[filaIndex];
            
            var resultado = MessageBox.Show(
                $"¿Está seguro de que desea eliminar este movimiento?\n\nFecha: {movimiento.FechaTexto}\nDescripción: {movimiento.Descripcion}\nMonto: {FormatearMoneda(movimiento.Monto)}",
                "Confirmar Eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                if (movimiento.Id > 0)
                {
                    // Eliminar de la base de datos
                    if (DatabaseHelper.EliminarMovimiento(movimiento.Id))
                    {
                        // Eliminar de la lista local
                        movimientos.RemoveAt(filaIndex);
                        ActualizarDataGridView();
                        ActualizarCalculos();
                        
                        MessageBox.Show("Movimiento eliminado correctamente.", 
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Error al eliminar el movimiento.", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Movimiento nuevo sin guardar, solo eliminar de la lista
                    movimientos.RemoveAt(filaIndex);
                    ActualizarDataGridView();
                    ActualizarCalculos();
                }
            }
        }

        /// <summary>
        /// Actualiza un movimiento con los datos del DataGridView
        /// </summary>
        /// <param name="movimiento">Movimiento a actualizar</param>
        /// <param name="filaIndex">Índice de la fila</param>
        private void ActualizarMovimientoDesdeGrid(MovimientoFila movimiento, int filaIndex)
        {
            var fila = dgvMovimientos.Rows[filaIndex];
            
            // Obtener valores de las celdas
            string fechaTexto = fila.Cells["colFecha"].Value?.ToString() ?? "";
            string descripcion = fila.Cells["colDescripcion"].Value?.ToString() ?? "";
            string montoTexto = fila.Cells["colMonto"].Value?.ToString() ?? "";
            string categoria = fila.Cells["colCategoria"].Value?.ToString() ?? "";

            // Validar y convertir fecha
            if (DateTime.TryParse(fechaTexto, out DateTime fecha))
            {
                movimiento.Fecha = fecha;
            }

            // Validar y convertir monto
            if (decimal.TryParse(montoTexto.Replace("₡", "").Replace(",", ""), out decimal monto))
            {
                movimiento.Monto = monto;
            }

            movimiento.Descripcion = descripcion;
            movimiento.Categoria = categoria;

            // Guardar en la base de datos si es válido
            if (ValidarMovimiento(movimiento))
            {
                if (movimiento.Id > 0)
                {
                    // Actualizar movimiento existente
                    DatabaseHelper.ActualizarMovimiento(movimiento.Id, movimiento.Fecha, 
                        movimiento.Descripcion, movimiento.Monto, movimiento.Categoria);
                }
                else
                {
                    // Crear nuevo movimiento
                    int nuevoId = DatabaseHelper.GuardarMovimiento(casaIdActual, movimiento.Fecha, 
                        movimiento.Descripcion, movimiento.Monto, movimiento.Categoria);
                    movimiento.Id = nuevoId;
                }
            }
        }

        /// <summary>
        /// Valida que un movimiento tenga datos válidos
        /// </summary>
        /// <param name="movimiento">Movimiento a validar</param>
        /// <returns>True si es válido, False en caso contrario</returns>
        private bool ValidarMovimiento(MovimientoFila movimiento)
        {
            return !string.IsNullOrWhiteSpace(movimiento.Descripcion) &&
                   !string.IsNullOrWhiteSpace(movimiento.Categoria) &&
                   movimiento.Monto != 0;
        }

        /// <summary>
        /// Maneja el evento de click del botón agregar reglones
        /// </summary>
        private void BtnAgregarReglones_Click(object? sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("Iniciando BtnAgregarReglones_Click...");
                
                // Validar que el control existe y tiene un valor válido
                if (nudCantidadReglones == null)
                {
                    MessageBox.Show("Error: El control de cantidad de reglones no está inicializado.", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int cantidadReglones = (int)nudCantidadReglones.Value;
                Console.WriteLine($"Cantidad de reglones solicitados: {cantidadReglones}");
                
                if (cantidadReglones <= 0)
                {
                    MessageBox.Show("La cantidad de reglones debe ser mayor a 0.", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Guardar el índice del primer reglón nuevo
                int indiceInicialNuevos = movimientos.Count;
                
                for (int i = 0; i < cantidadReglones; i++)
                {
                    var nuevoMovimiento = new MovimientoFila
                    {
                        Id = 0, // Nuevo, sin ID
                        Fecha = DateTime.Now,
                        Descripcion = "",
                        Monto = 0,
                        Categoria = "Alquiler" // Categoría por defecto
                    };
                    
                    movimientos.Add(nuevoMovimiento);
                    Console.WriteLine($"Agregado movimiento {i + 1}/{cantidadReglones}");
                }
                
                Console.WriteLine("Actualizando DataGridView...");
                ActualizarDataGridView();
                
                // Automáticamente habilitar edición en el primer reglón nuevo
                if (dgvMovimientos.Rows.Count > indiceInicialNuevos)
                {
                    Console.WriteLine($"Habilitando edición automática en fila {indiceInicialNuevos}");
                    
                    // Usar un timer para dar tiempo a que el DataGridView se actualice completamente
                    var timer = new System.Windows.Forms.Timer();
                    timer.Interval = 100; // 100ms
                    timer.Tick += (s, args) =>
                    {
                        timer.Stop();
                        timer.Dispose();
                        
                        try
                        {
                            if (indiceInicialNuevos < dgvMovimientos.Rows.Count)
                            {
                                HabilitarEdicionFilaAutomatica(indiceInicialNuevos);
                            }
                        }
                        catch (Exception autoEx)
                        {
                            Console.WriteLine($"Error en edición automática: {autoEx.Message}");
                        }
                    };
                    timer.Start();
                }
                
                Console.WriteLine("BtnAgregarReglones_Click completado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR en BtnAgregarReglones_Click: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al agregar reglones: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Inserta datos de prueba para diciembre 2024 y julio 2025
        /// </summary>
        private void InsertarDatosPrueba()
        {
            if (casaIdActual <= 0)
            {
                MessageBox.Show("Primero debe seleccionar una casa.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Datos de prueba para Diciembre 2024
                var movimientosDiciembre = new[]
                {
                    new { Fecha = new DateTime(2024, 12, 1), Descripcion = "Pago de alquiler", Monto = 450000m, Categoria = "Alquiler" },
                    new { Fecha = new DateTime(2024, 12, 15), Descripcion = "Reparación fontanería", Monto = -35000m, Categoria = "Reparaciones" },
                    new { Fecha = new DateTime(2024, 12, 25), Descripcion = "Depósito de garantía", Monto = 100000m, Categoria = "Otros Ingresos" }
                };

                // Datos de prueba para Julio 2025 (mes actual)
                var movimientosJulio = new[]
                {
                    new { Fecha = new DateTime(2025, 7, 5), Descripcion = "Pago mensual alquiler", Monto = 475000m, Categoria = "Alquiler" },
                    new { Fecha = new DateTime(2025, 7, 12), Descripcion = "Servicio de limpieza", Monto = -25000m, Categoria = "Limpieza" },
                    new { Fecha = new DateTime(2025, 7, 20), Descripcion = "Recargo por pago tardío", Monto = 15000m, Categoria = "Otros Ingresos" }
                };

                // Insertar movimientos de diciembre
                foreach (var mov in movimientosDiciembre)
                {
                    DatabaseHelper.GuardarMovimiento(casaIdActual, mov.Fecha, mov.Descripcion, mov.Monto, mov.Categoria);
                }

                // Insertar movimientos de julio
                foreach (var mov in movimientosJulio)
                {
                    DatabaseHelper.GuardarMovimiento(casaIdActual, mov.Fecha, mov.Descripcion, mov.Monto, mov.Categoria);
                }

                MessageBox.Show("Datos de prueba insertados correctamente.\n\n- 3 movimientos en Diciembre 2024\n- 3 movimientos en Julio 2025", 
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Recargar datos si estamos viendo diciembre o julio
                CargarMovimientos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al insertar datos de prueba: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }

    /// <summary>
    /// Clase para representar una fila de movimiento en el DataGridView
    /// </summary>
    public class MovimientoFila
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = "";
        public decimal Monto { get; set; }
        public string Categoria { get; set; } = "";

        public string FechaTexto
        {
            get => Fecha.ToString("dd/MM/yyyy");
            set
            {
                if (DateTime.TryParse(value, out DateTime fecha))
                    Fecha = fecha;
            }
        }

        public string MontoTexto
        {
            get => Monto.ToString("N2");
            set
            {
                if (decimal.TryParse(value.Replace("₡", "").Replace(",", ""), out decimal monto))
                    Monto = monto;
            }
        }
    }
}
