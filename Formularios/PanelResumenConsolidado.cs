using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlujoDeCajaApp.Data;
using FlujoDeCajaApp.Modelos;

namespace FlujoDeCajaApp.Formularios
{
    /// <summary>
    /// Panel para mostrar el resumen consolidado de todas las casas
    /// Incluye balance total, número de casas activas/inactivas y estadísticas generales
    /// </summary>
    public partial class PanelResumenConsolidado : UserControl
    {
        #region Variables privadas

        private Panel panelTitulo = null!;
        private Label lblTitulo = null!;
        private Button btnVolver = null!;
        private Panel panelContenido = null!;
        
        // Panels para las métricas principales
        private Panel panelBalanceTotal = null!;
        private Panel panelCasasActivas = null!;
        private Panel panelCasasInactivas = null!;
        private Panel panelIngresosMes = null!;
        private Panel panelGastosMes = null!;
        private Panel panelBalanceMes = null!;
        
        // Labels para mostrar los datos
        private Label lblBalanceTotalValor = null!;
        private Label lblCasasActivasValor = null!;
        private Label lblCasasInactivasValor = null!;
        private Label lblIngresosMesValor = null!;
        private Label lblGastosMesValor = null!;
        private Label lblBalanceMesValor = null!;

        // Datos
        private List<Propiedad> casasActivas = new();
        private List<Propiedad> casasInactivas = new();
        private decimal balanceTotal = 0;
        private decimal ingresosMesActual = 0;
        private decimal gastosMesActual = 0;

        #endregion

        #region Eventos

        /// <summary>
        /// Evento que se dispara cuando el usuario solicita volver al menú anterior
        /// </summary>
        public event EventHandler? VolverSolicitado;

        #endregion

        #region Constructor

        public PanelResumenConsolidado()
        {
            InitializeComponent();
            _ = InicializarPanelAsync();
        }

        #endregion

        #region Métodos de inicialización

        /// <summary>
        /// Inicializa el panel de manera asíncrona
        /// </summary>
        private async Task InicializarPanelAsync()
        {
            try
            {
                await CargarDatos();
                ActualizarInterfaz();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar el panel de resumen: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            this.Dock = DockStyle.Fill;

            CrearPanelTitulo();
            CrearPanelContenido();
            CrearTarjetasMetricas();

            // Agregar controles al UserControl
            this.Controls.AddRange(new Control[] { panelContenido, panelTitulo });

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Crea el panel del título
        /// </summary>
        private void CrearPanelTitulo()
        {
            panelTitulo = new Panel
            {
                Size = new Size(1000, 80),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(52, 152, 219),
                Dock = DockStyle.Top
            };

            lblTitulo = new Label
            {
                Text = "Resumen Consolidado",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(50, 20),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };

            btnVolver = new Button
            {
                Text = "← Volver",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Size = new Size(100, 35),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(52, 152, 219),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };
            
            // Posicionar el botón volver dinámicamente
            btnVolver.Location = new Point(panelTitulo.Width - btnVolver.Width - 50, 22);
            btnVolver.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnVolver.FlatAppearance.BorderSize = 0;
            btnVolver.Click += BtnVolver_Click;

            panelTitulo.Controls.AddRange(new Control[] { lblTitulo, btnVolver });
        }

        /// <summary>
        /// Crea el panel de contenido principal
        /// </summary>
        private void CrearPanelContenido()
        {
            panelContenido = new Panel
            {
                Location = new Point(0, 80),
                Size = new Size(1000, 520),
                BackColor = Color.FromArgb(32, 32, 32),
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(30)
            };
        }

        /// <summary>
        /// Crea las tarjetas con las métricas principales
        /// </summary>
        private void CrearTarjetasMetricas()
        {
            // Fila superior: Balance Total, Casas Activas, Casas Inactivas
            panelBalanceTotal = CrearTarjetaMetrica("Balance Total", "💰", Color.FromArgb(52, 152, 219), 0, 0);
            panelCasasActivas = CrearTarjetaMetrica("Casas Activas", "🏠", Color.FromArgb(46, 204, 113), 320, 0);
            panelCasasInactivas = CrearTarjetaMetrica("Casas Inactivas", "🔒", Color.FromArgb(231, 76, 60), 640, 0);

            // Fila inferior: Ingresos del Mes, Gastos del Mes, Balance del Mes
            panelIngresosMes = CrearTarjetaMetrica("Ingresos del Mes", "📈", Color.FromArgb(39, 174, 96), 0, 200);
            panelGastosMes = CrearTarjetaMetrica("Gastos del Mes", "📉", Color.FromArgb(231, 76, 60), 320, 200);
            panelBalanceMes = CrearTarjetaMetrica("Balance del Mes", "📊", Color.FromArgb(155, 89, 182), 640, 200);

            // Obtener referencias a los labels de valores
            lblBalanceTotalValor = panelBalanceTotal.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "valor")!;
            lblCasasActivasValor = panelCasasActivas.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "valor")!;
            lblCasasInactivasValor = panelCasasInactivas.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "valor")!;
            lblIngresosMesValor = panelIngresosMes.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "valor")!;
            lblGastosMesValor = panelGastosMes.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "valor")!;
            lblBalanceMesValor = panelBalanceMes.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "valor")!;

            // Agregar las tarjetas al panel de contenido
            panelContenido.Controls.AddRange(new Control[] { 
                panelBalanceTotal, panelCasasActivas, panelCasasInactivas,
                panelIngresosMes, panelGastosMes, panelBalanceMes
            });
        }

        /// <summary>
        /// Crea una tarjeta individual para mostrar una métrica
        /// </summary>
        private Panel CrearTarjetaMetrica(string titulo, string icono, Color colorAccento, int x, int y)
        {
            Panel tarjeta = new Panel
            {
                Size = new Size(300, 150),
                Location = new Point(x, y),
                BackColor = Color.White,
                Margin = new Padding(10)
            };

            // Borde redondeado (simulado con un borde)
            tarjeta.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, tarjeta.Width - 1, tarjeta.Height - 1);
                }
            };

            // Panel de color de acento en la parte superior
            Panel panelAccento = new Panel
            {
                Size = new Size(300, 5),
                Location = new Point(0, 0),
                BackColor = colorAccento,
                Dock = DockStyle.Top
            };

            // Label del icono
            Label lblIcono = new Label
            {
                Text = icono,
                Font = new Font("Segoe UI Emoji", 28, FontStyle.Regular),
                Location = new Point(20, 25),
                Size = new Size(60, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            // Label del título
            Label lblTituloTarjeta = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(100, 30),
                Size = new Size(180, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            // Label del valor (será actualizado dinámicamente)
            Label lblValor = new Label
            {
                Name = "valor",
                Text = "Cargando...",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = colorAccento,
                Location = new Point(100, 65),
                Size = new Size(180, 40),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            tarjeta.Controls.AddRange(new Control[] { panelAccento, lblIcono, lblTituloTarjeta, lblValor });

            return tarjeta;
        }

        #endregion

        #region Métodos de datos

        /// <summary>
        /// Carga todos los datos necesarios desde la base de datos
        /// </summary>
        private async Task CargarDatos()
        {
            try
            {
                // Cargar casas activas e inactivas
                await CargarCasas();

                // Calcular balance total
                await CalcularBalanceTotal();

                // Calcular ingresos y gastos del mes actual
                await CalcularMovimientosMesActual();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Carga las casas activas e inactivas
        /// </summary>
        private async Task CargarCasas()
        {
            try
            {
                // Usar el helper de Supabase para obtener todas las casas
                var casasActivasSupabase = await SupabaseCasaHelper.ObtenerTodasCasas();
                var casasInactivasSupabase = await SupabaseCasaHelper.ObtenerCasasInactivas();

                // Convertir a tipo Propiedad
                casasActivas = casasActivasSupabase.Select(c => new Propiedad 
                { 
                    Id = c.Id, 
                    Nombre = c.Nombre,
                    Activa = true
                }).ToList();
                
                casasInactivas = casasInactivasSupabase.Select(c => new Propiedad 
                { 
                    Id = c.Id, 
                    Nombre = c.Nombre,
                    Activa = false
                }).ToList();

                Console.WriteLine($"Casas activas: {casasActivas.Count}, Casas inactivas: {casasInactivas.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar casas: {ex.Message}");
                casasActivas = new List<Propiedad>();
                casasInactivas = new List<Propiedad>();
            }
        }

        /// <summary>
        /// Calcula el balance total de todas las casas activas
        /// </summary>
        private async Task CalcularBalanceTotal()
        {
            balanceTotal = 0;

            try
            {
                foreach (var casa in casasActivas)
                {
                    // Obtener movimientos de todos los meses para esta casa
                    var balanceCasa = await ObtenerBalanceTotalCasa(casa.Id);
                    balanceTotal += balanceCasa;
                }

                Console.WriteLine($"Balance total calculado: {balanceTotal:C}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al calcular balance total: {ex.Message}");
                balanceTotal = 0;
            }

            await Task.CompletedTask; // Para mantener consistencia con el patrón async
        }

        /// <summary>
        /// Obtiene el balance total de una casa específica
        /// </summary>
        private async Task<decimal> ObtenerBalanceTotalCasa(int casaId)
        {
            decimal balance = 0;
            
            try
            {
                // Obtener movimientos de los últimos 24 meses para calcular el balance total
                for (int i = 0; i < 24; i++)
                {
                    var fecha = DateTime.Now.AddMonths(-i);
                    var movimientos = DatabaseHelper.ObtenerMovimientosPorMes(casaId, fecha.Year, fecha.Month);
                    
                    foreach (var movimiento in movimientos)
                    {
                        balance += movimiento.Monto;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener balance de casa {casaId}: {ex.Message}");
            }

            await Task.CompletedTask;
            return balance;
        }

        /// <summary>
        /// Calcula los ingresos y gastos del mes actual
        /// </summary>
        private async Task CalcularMovimientosMesActual()
        {
            ingresosMesActual = 0;
            gastosMesActual = 0;

            try
            {
                var mesActual = DateTime.Now.Month;
                var añoActual = DateTime.Now.Year;

                foreach (var casa in casasActivas)
                {
                    var movimientos = DatabaseHelper.ObtenerMovimientosPorMes(casa.Id, añoActual, mesActual);

                    foreach (var movimiento in movimientos)
                    {
                        if (movimiento.Monto > 0)
                            ingresosMesActual += movimiento.Monto;
                        else
                            gastosMesActual += Math.Abs(movimiento.Monto);
                    }
                }

                Console.WriteLine($"Ingresos del mes: {ingresosMesActual:C}, Gastos del mes: {gastosMesActual:C}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al calcular movimientos del mes: {ex.Message}");
                ingresosMesActual = 0;
                gastosMesActual = 0;
            }

            await Task.CompletedTask;
        }

        #endregion

        #region Métodos de interfaz

        /// <summary>
        /// Actualiza la interfaz con los datos cargados
        /// </summary>
        private void ActualizarInterfaz()
        {
            try
            {
                // Actualizar balance total
                lblBalanceTotalValor.Text = FormatearMoneda(balanceTotal);
                lblBalanceTotalValor.ForeColor = balanceTotal >= 0 ? Color.FromArgb(46, 204, 113) : Color.FromArgb(231, 76, 60);

                // Actualizar casas
                lblCasasActivasValor.Text = casasActivas.Count.ToString();
                lblCasasInactivasValor.Text = casasInactivas.Count.ToString();

                // Actualizar movimientos del mes
                lblIngresosMesValor.Text = FormatearMoneda(ingresosMesActual);
                lblGastosMesValor.Text = FormatearMoneda(gastosMesActual);

                // Calcular y mostrar balance del mes
                var balanceMes = ingresosMesActual - gastosMesActual;
                lblBalanceMesValor.Text = FormatearMoneda(balanceMes);
                lblBalanceMesValor.ForeColor = balanceMes >= 0 ? Color.FromArgb(46, 204, 113) : Color.FromArgb(231, 76, 60);

                Console.WriteLine("Interfaz actualizada correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar interfaz: {ex.Message}");
                MessageBox.Show($"Error al actualizar la interfaz: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Formatea un valor monetario para mostrar
        /// </summary>
        private string FormatearMoneda(decimal valor)
        {
            return valor.ToString("C", System.Globalization.CultureInfo.CurrentCulture);
        }

        #endregion

        #region Eventos

        /// <summary>
        /// Maneja el evento click del botón volver
        /// </summary>
        private void BtnVolver_Click(object? sender, EventArgs e)
        {
            VolverSolicitado?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}