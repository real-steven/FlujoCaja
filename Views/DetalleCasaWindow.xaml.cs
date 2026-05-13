using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using FlujoCajaWpf.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace FlujoCajaWpf.Views
{
    public partial class DetalleCasaWindow : Window
    {
        private Casa _casa;
        private List<Movimiento> todosLosMovimientos = new List<Movimiento>();
        private ObservableCollection<Movimiento> movimientosFiltrados = new ObservableCollection<Movimiento>();
        private List<HojaMensual> hojasDisponibles = new List<HojaMensual>();
        private HojaMensual? hojaSeleccionada = null;
        private List<Nota> notasCasa = new List<Nota>();
        private List<FotoCasaSupabase> fotosCasa = new List<FotoCasaSupabase>();
        private ObservableCollection<MovimientoIA> _movimientosIA = new ObservableCollection<MovimientoIA>();

        // Lista estática para ComboBox de tipo en tabla IA
        public static readonly string[] TiposMovimientoIA = { "Ingreso", "Gasto" };

        public DetalleCasaWindow(Casa casa)
        {
            InitializeComponent();
            _casa = casa;
            
            txtNombreCasa.Text = casa.Nombre;
            txtDuenoCasa.Text = $"Dueño: {casa.DuenoNombre}";
            
            Loaded += async (s, e) => await CargarDatosAsync();
        }

        private async Task CargarDatosAsync()
        {
            await SupabaseHojaMensualHelper.AsegurarHojaProximoMesAsync(_casa.Id);
            await CargarEstadoGeneralAsync();
            await CargarFiltrosResumenAsync();
            await CargarHojasMensualesAsync();
            await CargarMovimientosAsync();
        }

        private async Task CargarHojasMensualesAsync()
        {
            // Cargar años disponibles
            var anios = await SupabaseHojaMensualHelper.ObtenerAniosDisponiblesAsync(_casa.Id);
            cmbAnio.ItemsSource = anios;

            // Seleccionar año actual
            var anioActual = DateTime.Now.Year;
            if (anios.Contains(anioActual))
            {
                cmbAnio.SelectedItem = anioActual;
            }
            else if (anios.Any())
            {
                cmbAnio.SelectedItem = anios.First();
            }
        }

        private async void CmbAnio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbAnio.SelectedItem == null) return;

            var anioSeleccionado = (int)cmbAnio.SelectedItem;

            // Cargar meses disponibles para el año seleccionado
            hojasDisponibles = await SupabaseHojaMensualHelper.ObtenerMesesDisponiblesAsync(_casa.Id, anioSeleccionado);
            cmbMes.ItemsSource = hojasDisponibles;

            // Seleccionar mes actual si estamos en el año actual
            if (anioSeleccionado == DateTime.Now.Year)
            {
                var mesActual = DateTime.Now.Month;
                var hojaActual = hojasDisponibles.FirstOrDefault(h => h.Mes == mesActual);
                if (hojaActual != null)
                {
                    cmbMes.SelectedItem = hojaActual;
                }
                else if (hojasDisponibles.Any())
                {
                    cmbMes.SelectedItem = hojasDisponibles.First();
                }
            }
            else if (hojasDisponibles.Any())
            {
                cmbMes.SelectedItem = hojasDisponibles.First();
            }
        }

        private async void CmbMes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbMes.SelectedItem == null) return;

            hojaSeleccionada = cmbMes.SelectedItem as HojaMensual;
            await CargarMovimientosAsync();
        }

        private async void RefrescarMovimientos_Click(object sender, RoutedEventArgs e)
        {
            await CargarMovimientosAsync();
        }

        private async Task CargarMovimientosAsync()
        {
            if (hojaSeleccionada == null)
            {
                todosLosMovimientos = new List<Movimiento>();
                movimientosFiltrados = new ObservableCollection<Movimiento>();
                dgMovimientos.ItemsSource = movimientosFiltrados;
                ActualizarContadorMovimientos(0);
                txtNoDataMovimientos.Visibility = Visibility.Visible;
                return;
            }

            var resultado = await SupabaseMovimientoHelper.ObtenerMovimientosPorHojaAsync(hojaSeleccionada.Id);
            
            if (resultado.Success && resultado.Data != null)
            {
                // Asignar la moneda de la casa a cada movimiento
                todosLosMovimientos = resultado.Data.Select(m =>
                {
                    m.MonedaCasa = _casa.Moneda;
                    return m;
                }).ToList();
                
                movimientosFiltrados = new ObservableCollection<Movimiento>(todosLosMovimientos);
                dgMovimientos.ItemsSource = movimientosFiltrados;
                ActualizarContadorMovimientos(movimientosFiltrados.Count);
                txtNoDataMovimientos.Visibility = movimientosFiltrados.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #region Tab Resumen - Estado General y Mensual

        /// <summary>
        /// Carga el estado financiero general (histórico completo)
        /// </summary>
        private async Task CargarEstadoGeneralAsync()
        {
            var resultado = await SupabaseMovimientoHelper.ObtenerMovimientosPorCasaAsync(_casa.Id);
            
            if (!resultado.Success || resultado.Data == null)
            {
                txtBalanceGeneral.Text = "$0.00";
                txtIngresosGenerales.Text = "$0.00";
                txtEgresosGenerales.Text = "$0.00";
                return;
            }

            var movimientos = resultado.Data;
            
            // Debug: verificar tipos
            Console.WriteLine($"=== Estado General - Total movimientos: {movimientos.Count} ===");
            var ingresos = movimientos.Where(m => m.Tipo.Equals("Ingreso", StringComparison.OrdinalIgnoreCase)).ToList();
            var gastos = movimientos.Where(m => m.Tipo.Equals("Gasto", StringComparison.OrdinalIgnoreCase)).ToList();
            Console.WriteLine($"Ingresos encontrados: {ingresos.Count}");
            Console.WriteLine($"Gastos encontrados: {gastos.Count}");
            foreach (var m in movimientos.Take(5))
            {
                Console.WriteLine($"  Tipo: '{m.Tipo}' | Monto: {m.Monto}");
            }
            
            var totalIngresos = ingresos.Sum(m => m.Monto);
            var totalEgresos = gastos.Sum(m => m.Monto);

            // Balance: sumar todos los montos (ya vienen con signo correcto)
            var balance = movimientos.Sum(m => m.Monto);
            
            Console.WriteLine($"Total Ingresos: {totalIngresos} | Total Gastos: {totalEgresos} (abs: {Math.Abs(totalEgresos)}) | Balance: {balance}");

            // Actualizar UI con formato de moneda de la casa
            txtBalanceGeneral.Text = FormatearMonedaCasa(balance);
            txtIngresosGenerales.Text = FormatearMonedaCasa(totalIngresos);
            txtEgresosGenerales.Text = FormatearMonedaCasa(Math.Abs(totalEgresos));

            // Color dinámico del balance
            if (balance > 1000)
            {
                txtBalanceGeneral.Foreground = new SolidColorBrush(Color.FromRgb(16, 185, 129)); // Verde
            }
            else if (balance > 0)
            {
                txtBalanceGeneral.Foreground = new SolidColorBrush(Color.FromRgb(251, 191, 36)); // Amarillo
            }
            else
            {
                txtBalanceGeneral.Foreground = new SolidColorBrush(Color.FromRgb(220, 38, 38)); // Rojo
            }

            // Actualizar indicador de salud
            ActualizarIndicadorSalud(balance);
        }

        /// <summary>
        /// Actualiza el indicador de salud financiera
        /// </summary>
        private void ActualizarIndicadorSalud(decimal balance)
        {
            if (balance == 0)
            {
                // ⚪ SIN MOVIMIENTOS
                txtEstadoSalud.Text = "⚪ SIN DATOS";
                txtEstadoSalud.Foreground = new SolidColorBrush(Color.FromRgb(156, 163, 175));
                txtMensajeSalud.Text = "Aún no hay movimientos registrados para esta casa";
                txtMensajeSalud.Foreground = new SolidColorBrush(Color.FromRgb(107, 114, 128));
            }
            else if (balance > 1000)
            {
                // 🟢 SALUDABLE
                txtEstadoSalud.Text = "🟢 SALUDABLE";
                txtEstadoSalud.Foreground = new SolidColorBrush(Color.FromRgb(16, 185, 129));
                txtMensajeSalud.Text = "Casa en buen estado financiero";
                txtMensajeSalud.Foreground = new SolidColorBrush(Color.FromRgb(107, 114, 128));
            }
            else if (balance > 0)
            {
                // 🟡 ATENCIÓN
                txtEstadoSalud.Text = "🟡 ATENCIÓN";
                txtEstadoSalud.Foreground = new SolidColorBrush(Color.FromRgb(251, 191, 36));
                txtMensajeSalud.Text = $"Quedan {balance:C} - Se recomienda solicitar depósito al dueño";
                txtMensajeSalud.Foreground = new SolidColorBrush(Color.FromRgb(217, 119, 6));
            }
            else
            {
                // 🔴 CRÍTICO
                txtEstadoSalud.Text = "🔴 CRÍTICO";
                txtEstadoSalud.Foreground = new SolidColorBrush(Color.FromRgb(220, 38, 38));
                txtMensajeSalud.Text = $"Balance negativo de {Math.Abs(balance):C} - Urgente solicitar depósito";
                txtMensajeSalud.Foreground = new SolidColorBrush(Color.FromRgb(220, 38, 38));
            }
        }

        /// <summary>
        /// Carga los filtros de año y mes para el resumen mensual y timeline
        /// </summary>
        private async Task CargarFiltrosResumenAsync()
        {
            var anios = await SupabaseMovimientoHelper.ObtenerAniosConMovimientosAsync(_casa.Id);
            
            cmbResumenAnio.ItemsSource = anios;
            cmbTimelineAnio.ItemsSource = anios;

            // Seleccionar año actual
            var anioActual = DateTime.Now.Year;
            if (anios.Contains(anioActual))
            {
                cmbResumenAnio.SelectedItem = anioActual;
                cmbTimelineAnio.SelectedItem = anioActual;
            }
            else if (anios.Any())
            {
                cmbResumenAnio.SelectedItem = anios.First();
                cmbTimelineAnio.SelectedItem = anios.First();
            }

            // Seleccionar mes actual
            var mesActual = DateTime.Now.Month;
            cmbResumenMes.SelectedIndex = mesActual - 1; // ComboBox es 0-indexed
            
            // Cargar los datos del detalle mensual y timeline después de configurar los filtros
            if (cmbResumenAnio.SelectedItem != null && cmbResumenMes.SelectedItem != null)
            {
                await CargarDetalleMensualAsync();
            }
            
            if (cmbTimelineAnio.SelectedItem != null)
            {
                await CargarTimelineAnualAsync();
            }
        }

        /// <summary>
        /// Evento cuando cambia el año en el resumen mensual
        /// </summary>
        private async void CmbResumenAnio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbResumenAnio.SelectedItem == null || cmbResumenMes.SelectedItem == null) return;
            await CargarDetalleMensualAsync();
        }

        /// <summary>
        /// Evento cuando cambia el mes en el resumen mensual
        /// </summary>
        private async void CmbResumenMes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbResumenAnio.SelectedItem == null || cmbResumenMes.SelectedItem == null) return;
            await CargarDetalleMensualAsync();
        }

        /// <summary>
        /// Carga el detalle mensual según los filtros seleccionados
        /// </summary>
        private async Task CargarDetalleMensualAsync()
        {
            var año = (int)cmbResumenAnio.SelectedItem;
            var mesItem = (ComboBoxItem)cmbResumenMes.SelectedItem;
            var mes = int.Parse(mesItem.Tag.ToString()!);

            var resultado = await SupabaseMovimientoHelper.ObtenerMovimientosPorMesAsync(_casa.Id, año, mes);
            
            if (!resultado.Success || resultado.Data == null)
            {
                txtBalanceMensual.Text = "$0.00";
                txtIngresosMensuales.Text = "$0.00";
                txtEgresosMensuales.Text = "$0.00";
                return;
            }

            var movimientos = resultado.Data;
            
            var ingresosMes = movimientos
                .Where(m => m.Tipo.Equals("Ingreso", StringComparison.OrdinalIgnoreCase))
                .Sum(m => m.Monto);

            var egresosMes = movimientos
                .Where(m => m.Tipo.Equals("Gasto", StringComparison.OrdinalIgnoreCase))
                .Sum(m => m.Monto);

            // Balance: sumar todos los montos (ya vienen con signo correcto)
            var balanceMes = movimientos.Sum(m => m.Monto);

            // Actualizar UI con formato de moneda de la casa
            txtBalanceMensual.Text = FormatearMonedaCasa(balanceMes);
            txtIngresosMensuales.Text = FormatearMonedaCasa(ingresosMes);
            txtEgresosMensuales.Text = FormatearMonedaCasa(Math.Abs(egresosMes));

            // Color del balance mensual
            txtBalanceMensual.Foreground = balanceMes >= 0 
                ? new SolidColorBrush(Color.FromRgb(16, 185, 129)) 
                : new SolidColorBrush(Color.FromRgb(220, 38, 38));
        }

        /// <summary>
        /// Evento cuando cambia el año en el timeline
        /// </summary>
        private async void CmbTimelineAnio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTimelineAnio.SelectedItem == null) return;
            await CargarTimelineAnualAsync();
        }

        /// <summary>
        /// Carga el timeline de balance anual (12 meses)
        /// </summary>
        private async Task CargarTimelineAnualAsync()
        {
            var año = (int)cmbTimelineAnio.SelectedItem;
            
            // Obtener hojas mensuales del año
            var hojasMensuales = await SupabaseHojaMensualHelper.ObtenerMesesDisponiblesAsync(_casa.Id, año);
            
            // Obtener todos los movimientos de la casa
            var resultadoMovimientos = await SupabaseMovimientoHelper.ObtenerMovimientosPorCasaAsync(_casa.Id);
            
            if (!resultadoMovimientos.Success || resultadoMovimientos.Data == null)
            {
                return;
            }

            var todosMovimientos = resultadoMovimientos.Data.OrderBy(m => m.Fecha).ToList();
            
            // Calcular balance inicial (todo lo anterior al año seleccionado)
            decimal balanceAcumulado = 0;
            
            // Obtener hojas anteriores al año seleccionado
            var hojasAnteriores = await SupabaseHojaMensualHelper.ObtenerHojasPorCasaAsync(_casa.Id);
            var hojasAnterioresIds = hojasAnteriores
                .Where(h => h.Anio < año)
                .Select(h => h.Id)
                .ToList();
            
            // Balance inicial: sumar todos los montos anteriores (ya vienen con signo correcto)
            balanceAcumulado = todosMovimientos
                .Where(m => m.HojaMensualId.HasValue && hojasAnterioresIds.Contains(m.HojaMensualId.Value))
                .Sum(m => m.Monto);

            // Calcular balances de cada mes
            var balancesMensuales = new decimal[12];
            var hayMovimientosPorMes = new bool[12];
            
            for (int mes = 1; mes <= 12; mes++)
            {
                var hojaMes = hojasMensuales.FirstOrDefault(h => h.Mes == mes);
                
                if (hojaMes != null)
                {
                    var movimientosDelMes = todosMovimientos
                        .Where(m => m.HojaMensualId == hojaMes.Id)
                        .ToList();

                    hayMovimientosPorMes[mes - 1] = movimientosDelMes.Any();

                    // Los montos ya vienen con signo correcto (negativos para gastos)
                    var balanceMes = movimientosDelMes.Sum(m => m.Monto);

                    balanceAcumulado += balanceMes;
                }
                
                balancesMensuales[mes - 1] = balanceAcumulado;
            }

            // Limpiar grids
            gridIndicadores.Children.Clear();
            gridBalances.Children.Clear();

            // Crear indicadores y balances
            for (int i = 0; i < 12; i++)
            {
                var balance = balancesMensuales[i];
                var hayMovimientos = hayMovimientosPorMes[i];
                
                // Indicador (círculo)
                var circulo = new System.Windows.Shapes.Ellipse
                {
                    Width = 40,
                    Height = 40,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                if (!hayMovimientos)
                    circulo.Fill = new SolidColorBrush(Color.FromRgb(209, 213, 219)); // Gris - sin movimientos
                else if (balance > 500)
                    circulo.Fill = new SolidColorBrush(Color.FromRgb(16, 185, 129)); // Verde
                else if (balance > 0)
                    circulo.Fill = new SolidColorBrush(Color.FromRgb(251, 191, 36)); // Amarillo
                else
                    circulo.Fill = new SolidColorBrush(Color.FromRgb(220, 38, 38)); // Rojo

                Grid.SetColumn(circulo, i);
                gridIndicadores.Children.Add(circulo);

                // Balance (texto)
                var texto = new System.Windows.Controls.TextBlock
                {
                    Text = !hayMovimientos ? "Sin datos" : FormatearMonto(balance),
                    FontSize = 13,
                    FontWeight = FontWeights.SemiBold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                };

                if (!hayMovimientos)
                    texto.Foreground = new SolidColorBrush(Color.FromRgb(156, 163, 175));
                else if (balance > 500)
                    texto.Foreground = new SolidColorBrush(Color.FromRgb(16, 185, 129));
                else if (balance > 0)
                    texto.Foreground = new SolidColorBrush(Color.FromRgb(217, 119, 6));
                else
                    texto.Foreground = new SolidColorBrush(Color.FromRgb(220, 38, 38));

                Grid.SetColumn(texto, i);
                gridBalances.Children.Add(texto);
            }
        }

        /// <summary>
        /// Formatea un monto para mostrarlo de forma compacta en el timeline
        /// </summary>
        private string FormatearMonto(decimal monto)
        {
            string simbolo = _casa.Moneda switch
            {
                "USD" => "$",
                "CRC" => "₡",
                _ => "$"
            };

            if (monto >= 1000000)
                return $"{simbolo}{monto / 1000000:0.#}M";
            else if (monto >= 1000)
                return $"{simbolo}{monto / 1000:0.#}K";
            else if (monto <= -1000000)
                return $"{simbolo}{monto / 1000000:0.#}M";
            else if (monto <= -1000)
                return $"{simbolo}{monto / 1000:0.#}K";
            else
                return FormatearMonedaCasa(monto);
        }

        #endregion

        private void ResetTabButtons()
        {
            var gray = new SolidColorBrush(Color.FromRgb(107, 114, 128));
            btnTabResumen.Background       = Brushes.Transparent;  btnTabResumen.Foreground       = gray;
            btnTabMovimientos.Background   = Brushes.Transparent;  btnTabMovimientos.Foreground   = gray;
            btnTabDetalles.Background      = Brushes.Transparent;  btnTabDetalles.Foreground      = gray;
            btnTabMovimientosIA.Background = Brushes.Transparent;  btnTabMovimientosIA.Foreground = gray;
            tabResumen.Visibility = Visibility.Collapsed;
            tabMovimientos.Visibility = Visibility.Collapsed;
            tabDetalles.Visibility = Visibility.Collapsed;
            tabMovimientosIA.Visibility = Visibility.Collapsed;
        }

        private async void TabResumen_Click(object sender, RoutedEventArgs e)
        {
            ResetTabButtons();
            btnTabResumen.Background = new SolidColorBrush(Color.FromRgb(59, 130, 246));
            btnTabResumen.Foreground = Brushes.White;
            tabResumen.Visibility = Visibility.Visible;

            await CargarEstadoGeneralAsync();
            await CargarFiltrosResumenAsync();
        }

        private void TabMovimientos_Click(object sender, RoutedEventArgs e)
        {
            ResetTabButtons();
            btnTabMovimientos.Background = new SolidColorBrush(Color.FromRgb(59, 130, 246));
            btnTabMovimientos.Foreground = Brushes.White;
            tabMovimientos.Visibility = Visibility.Visible;
        }

        private void BuscarMovimiento_TextChanged(object sender, TextChangedEventArgs e)
        {
            var busqueda = txtBuscarMovimiento.Text.ToLower();
            
            if (string.IsNullOrWhiteSpace(busqueda))
            {
                movimientosFiltrados = new ObservableCollection<Movimiento>(todosLosMovimientos);
            }
            else
            {
                var filtrados = todosLosMovimientos.Where(m =>
                    (m.Descripcion?.ToLower().Contains(busqueda) ?? false) ||
                    (m.CategoriaNombre?.ToLower().Contains(busqueda) ?? false)
                ).ToList();
                
                movimientosFiltrados = new ObservableCollection<Movimiento>(filtrados);
            }

            dgMovimientos.ItemsSource = movimientosFiltrados;
            ActualizarContadorMovimientos(movimientosFiltrados.Count);
            txtNoDataMovimientos.Visibility = movimientosFiltrados.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ActualizarContadorMovimientos(int count)
        {
            txtContadorMovimientos.Text = count == 1 ? "1 movimiento" : $"{count} movimientos";
        }

        private async void DescargarReporte_Click(object sender, RoutedEventArgs e)
        {
            if (hojaSeleccionada == null)
            {
                CustomMessageBox.Show(
                    "Selecciona un mes antes de generar el reporte.",
                    "Sin mes seleccionado",
                    CustomMessageBox.MessageBoxType.Warning,
                    CustomMessageBox.MessageBoxButtons.OK);
                return;
            }

            if (todosLosMovimientos.Count == 0)
            {
                CustomMessageBox.Show(
                    "No hay movimientos registrados en este mes.",
                    "Sin movimientos",
                    CustomMessageBox.MessageBoxType.Warning,
                    CustomMessageBox.MessageBoxButtons.OK);
                return;
            }

            // 1. Elegir formato
            string mesAnio = $"{hojaSeleccionada.NombreMes} {hojaSeleccionada.Anio}";
            var dialogo = new ReporteFormatoDialog(_casa.Nombre, mesAnio);
            dialogo.Owner = this;
            if (dialogo.ShowDialog() != true) return;

            bool esPdfTabla = dialogo.FormatoSeleccionado == ReporteFormatoDialog.FormatoReporte.PdfTabla;

            // 2. SaveFileDialog (siempre PDF)
            var save = new SaveFileDialog
            {
                Title = "Guardar reporte",
                FileName = $"Reporte_{_casa.Nombre.Replace(" ", "_")}_{hojaSeleccionada.Anio}_{hojaSeleccionada.Mes:D2}",
                Filter = "PDF (*.pdf)|*.pdf",
                DefaultExt = "pdf"
            };

            if (save.ShowDialog() != true) return;

            var btn = sender as System.Windows.Controls.Button;
            if (btn != null) btn.IsEnabled = false;

            try
            {
                var datos = new ReporteService.DatosReporte(
                    CasaNombre: _casa.Nombre,
                    DuenoNombre: _casa.DuenoNombre ?? "—",
                    Moneda: _casa.Moneda ?? "USD",
                    MesAnio: mesAnio,
                    Movimientos: todosLosMovimientos
                );

                if (esPdfTabla)
                    await ReporteService.GenerarPdfAsync(datos, save.FileName);
                else
                    await ReporteService.GenerarPdfFacturasAsync(datos, save.FileName, ObtenerImagenMovimientoAsync);

                // 3. Abrir el archivo
                Process.Start(new ProcessStartInfo(save.FileName) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(
                    $"Error al generar el reporte:\n{ex.Message}",
                    "Error",
                    CustomMessageBox.MessageBoxType.Error,
                    CustomMessageBox.MessageBoxButtons.OK);
            }
            finally
            {
                if (btn != null) btn.IsEnabled = true;
            }
        }

        private async void EnviarReporte_Click(object sender, RoutedEventArgs e)
        {
            if (hojaSeleccionada == null)
            {
                CustomMessageBox.Show(
                    "Selecciona un mes antes de enviar el reporte.",
                    "Sin mes seleccionado",
                    CustomMessageBox.MessageBoxType.Warning,
                    CustomMessageBox.MessageBoxButtons.OK);
                return;
            }

            if (todosLosMovimientos.Count == 0)
            {
                CustomMessageBox.Show(
                    "No hay movimientos registrados en este mes.",
                    "Sin movimientos",
                    CustomMessageBox.MessageBoxType.Warning,
                    CustomMessageBox.MessageBoxButtons.OK);
                return;
            }

            var btn = sender as System.Windows.Controls.Button;
            if (btn != null) btn.IsEnabled = false;

            string? rutaPdfTabla = null;
            string? rutaPdfImagenes = null;

            try
            {
                string mesAnio = $"{hojaSeleccionada.NombreMes} {hojaSeleccionada.Anio}";
                var datos = new ReporteService.DatosReporte(
                    CasaNombre: _casa.Nombre,
                    DuenoNombre: _casa.DuenoNombre ?? "—",
                    Moneda: _casa.Moneda ?? "USD",
                    MesAnio: mesAnio,
                    Movimientos: todosLosMovimientos
                );

                // 1. Generar ambos PDFs en carpeta temporal
                string nombreBase = $"Reporte_{_casa.Nombre.Replace(" ", "_")}_{hojaSeleccionada.Anio}_{hojaSeleccionada.Mes:D2}";
                rutaPdfTabla = Path.Combine(Path.GetTempPath(), $"{nombreBase}_Tabla.pdf");
                rutaPdfImagenes = Path.Combine(Path.GetTempPath(), $"{nombreBase}_Imagenes.pdf");

                await Task.WhenAll(
                    ReporteService.GenerarPdfAsync(datos, rutaPdfTabla),
                    ReporteService.GenerarPdfFacturasAsync(datos, rutaPdfImagenes, ObtenerImagenMovimientoAsync)
                );

                // 2. Abrir diálogo de envío
                var enviarDialog = new EnviarReporteDialog(_casa, rutaPdfTabla, rutaPdfImagenes) { Owner = this };
                enviarDialog.ShowDialog();

                // 3. Confirmación si fue exitoso
                if (enviarDialog.EnvioFueExitoso)
                {
                    CustomMessageBox.Show(
                        $"✅ Reporte enviado correctamente.\n\nPropiedad: {_casa.Nombre}\nPeríodo: {mesAnio}\nDestinatarios: {enviarDialog.DestinatariosEnviados}",
                        "Correo enviado",
                        CustomMessageBox.MessageBoxType.Success,
                        CustomMessageBox.MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(
                    $"Error al preparar el reporte:\n{ex.Message}",
                    "Error",
                    CustomMessageBox.MessageBoxType.Error,
                    CustomMessageBox.MessageBoxButtons.OK);
            }
            finally
            {
                if (btn != null) btn.IsEnabled = true;

                // Limpiar archivos temporales
                try { if (rutaPdfTabla != null && File.Exists(rutaPdfTabla)) File.Delete(rutaPdfTabla); } catch { }
                try { if (rutaPdfImagenes != null && File.Exists(rutaPdfImagenes)) File.Delete(rutaPdfImagenes); } catch { }
            }
        }

        private async void LeerComprobante_Click(object sender, RoutedEventArgs e)
        {
            int anio = DateTime.Now.Year;
            int mes  = DateTime.Now.Month;

            var hoja = hojasDisponibles.FirstOrDefault(h => h.Mes == mes && h.Anio == anio);
            var emailUsuario = SupabaseAuthHelper.GetCurrentUser()?.Email ?? "";

            var dlg = new ImportarFacturasDialog(_casa.Id, mes, anio, hoja?.Id, emailUsuario)
            {
                Owner = this,
                ArchivoCompletado = CargarMovimientosIAAsync
            };
            dlg.ShowDialog();

            // Recarga final por si el dialog se cerró antes de terminar el último archivo
            await CargarMovimientosIAAsync();
        }

        private async void NuevoMovimiento_Click(object sender, RoutedEventArgs e)
        {
            if (hojaSeleccionada == null)
            {
                CustomMessageBox.Show(
                    "Debes seleccionar un mes y año para agregar movimientos.",
                    "Advertencia",
                    CustomMessageBox.MessageBoxType.Warning,
                    CustomMessageBox.MessageBoxButtons.OK
                );
                return;
            }

            var ventana = new AgregarMovimientoWindow(_casa.Id, hojaSeleccionada.Id, _casa.Nombre, _casa.Moneda);
            ventana.Owner = this;
            
            if (ventana.ShowDialog() == true)
            {
                await CargarDatosAsync(); // Recargar todo incluyendo resumen
            }
        }

        private async void AdjuntarImagen_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var movimiento = button?.Tag as Movimiento;
            if (movimiento == null) return;

            if (movimiento.TieneImagen)
            {
                // Resolver URL: si es path privado, generar URL firmada temporal
                string urlParaMostrar = movimiento.ImagenUrl!;
                if (!urlParaMostrar.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                    !urlParaMostrar.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    var signedUrl = await SupabaseStorageHelper.ObtenerUrlFirmadaMovimientoAsync(urlParaMostrar);
                    if (signedUrl == null)
                    {
                        CustomMessageBox.Show("No se pudo obtener acceso a la imagen.", "Error",
                            CustomMessageBox.MessageBoxType.Error, CustomMessageBox.MessageBoxButtons.OK);
                        return;
                    }
                    urlParaMostrar = signedUrl;
                }

                // ── Movimiento YA tiene imagen: abrir VerImagenDialog ──
                var verDialog = new VerImagenDialog(urlParaMostrar);
                verDialog.Owner = this;
                verDialog.ShowDialog();

                if (verDialog.QuiereEliminar)
                {
                    // Eliminar imagen del Storage y limpiar DB
                    await SupabaseStorageHelper.EliminarImagenMovimientoAsync(movimiento.ImagenUrl!);
                    var delResult = await SupabaseMovimientoHelper.ActualizarImagenMovimientoAsync(movimiento.Id, null);
                    if (delResult.Success)
                    {
                        movimiento.ImagenUrl = null;
                        dgMovimientos.Items.Refresh();
                        CustomMessageBox.Show("Imagen eliminada correctamente.", "Éxito",
                            CustomMessageBox.MessageBoxType.Success, CustomMessageBox.MessageBoxButtons.OK);
                    }
                    else
                    {
                        CustomMessageBox.Show($"Error al eliminar: {delResult.Error}", "Error",
                            CustomMessageBox.MessageBoxType.Error, CustomMessageBox.MessageBoxButtons.OK);
                    }
                }
                else if (!string.IsNullOrEmpty(verDialog.RutaImagenSeleccionada))
                {
                    // Reemplazar: eliminar antigua y subir nueva
                    await SupabaseStorageHelper.EliminarImagenMovimientoAsync(movimiento.ImagenUrl!);
                    await SubirYGuardarImagenAsync(movimiento, verDialog.RutaImagenSeleccionada);
                }
            }
            else
            {
                // ── Movimiento SIN imagen: abrir AdjuntarImagenDialog ──
                var adjDialog = new AdjuntarImagenDialog();
                adjDialog.Owner = this;

                if (adjDialog.ShowDialog() == true && !string.IsNullOrEmpty(adjDialog.RutaImagenSeleccionada))
                {
                    await SubirYGuardarImagenAsync(movimiento, adjDialog.RutaImagenSeleccionada);
                }
            }
        }

        /// <summary>
        /// Sube la imagen al bucket privado de facturas y guarda el storage path en la BD.
        /// </summary>
        private async Task SubirYGuardarImagenAsync(Movimiento movimiento, string rutaLocal)
        {
            try
            {
                var imageBytes = await File.ReadAllBytesAsync(rutaLocal);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(rutaLocal).ToLowerInvariant()}";
                var uploadResult = await SupabaseStorageHelper.SubirImagenMovimientoAsync(imageBytes, fileName, _casa.Id);

                if (!uploadResult.Success)
                {
                    CustomMessageBox.Show($"Error al subir la imagen: {uploadResult.Error}", "Error",
                        CustomMessageBox.MessageBoxType.Error, CustomMessageBox.MessageBoxButtons.OK);
                    return;
                }

                // uploadResult.Path es el storage path privado (ej: "5/abc123.jpg")
                var updateResult = await SupabaseMovimientoHelper.ActualizarImagenMovimientoAsync(movimiento.Id, uploadResult.Path);
                if (updateResult.Success)
                {
                    movimiento.ImagenUrl = uploadResult.Path;
                    dgMovimientos.Items.Refresh();
                    CustomMessageBox.Show("Imagen guardada correctamente.", "Éxito",
                        CustomMessageBox.MessageBoxType.Success, CustomMessageBox.MessageBoxButtons.OK);
                }
                else
                {
                    CustomMessageBox.Show($"Error al guardar la referencia: {updateResult.Error}", "Error",
                        CustomMessageBox.MessageBoxType.Error, CustomMessageBox.MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error inesperado: {ex.Message}", "Error",
                    CustomMessageBox.MessageBoxType.Error, CustomMessageBox.MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// Descarga los bytes de la imagen de un movimiento (maneja paths privados y URLs legacy).
        /// Usado por ReporteService.GenerarPdfFacturasAsync.
        /// </summary>
        private static async Task<byte[]?> ObtenerImagenMovimientoAsync(Movimiento m)
        {
            if (string.IsNullOrEmpty(m.ImagenUrl)) return null;
            return await SupabaseStorageHelper.DescargarImagenMovimientoAsync(m.ImagenUrl);
        }

        private async void EditarMovimiento_Click(object sender, RoutedEventArgs e)        {
            var button = sender as Button;
            var movimiento = button?.Tag as Movimiento;
            
            if (movimiento == null) return;

            var ventana = new AgregarMovimientoWindow(_casa.Id, movimiento.HojaMensualId ?? 0, _casa.Nombre, _casa.Moneda, movimiento);
            ventana.Owner = this;
            
            if (ventana.ShowDialog() == true)
            {
                await CargarDatosAsync();
            }
        }

        private async void EliminarMovimiento_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var movimiento = button?.Tag as Movimiento;
            
            if (movimiento == null) return;

            var resultado = CustomMessageBox.Show(
                $"¿Estás seguro de eliminar este movimiento?\n\n{movimiento.CategoriaNombre} - {movimiento.Monto:C}",
                "Confirmar Eliminación",
                CustomMessageBox.MessageBoxType.Warning,
                CustomMessageBox.MessageBoxButtons.YesNo
            );

            if (resultado == true)
            {
                var resultadoEliminacion = await SupabaseMovimientoHelper.EliminarMovimientoAsync(movimiento.Id);
                
                if (resultadoEliminacion.Success)
                {
                    // 📊 REGISTRAR EN HISTORIAL
                    var user = SupabaseAuthHelper.GetCurrentUser();
                    await SupabaseAuditoriaHelper.RegistrarAccionAsync(
                        user?.Email ?? "desconocido",
                        "movimiento",
                        "eliminar",
                        movimiento.Id,
                        _casa.Nombre,
                        $"Eliminó {movimiento.Tipo}: {movimiento.Monto:C} - {movimiento.CategoriaNombre}"
                    );
                    
                    CustomMessageBox.Show(
                        "Movimiento eliminado correctamente",
                        "Éxito",
                        CustomMessageBox.MessageBoxType.Success,
                        CustomMessageBox.MessageBoxButtons.OK
                    );
                    
                    await CargarDatosAsync();
                }
                else
                {
                    CustomMessageBox.Show(
                        $"Error al eliminar: {resultadoEliminacion.Error}",
                        "Error",
                        CustomMessageBox.MessageBoxType.Error,
                        CustomMessageBox.MessageBoxButtons.OK
                    );
                }
            }
        }

        // ==================== TAB DETALLES ====================

        private void TabDetalles_Click(object sender, RoutedEventArgs e)
        {
            ResetTabButtons();
            btnTabDetalles.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"));
            btnTabDetalles.Foreground = new SolidColorBrush(Colors.White);
            tabDetalles.Visibility = Visibility.Visible;

            CargarDetallesCasa();
        }

        private async void CargarDetallesCasa()
        {
            // Cargar información de la casa
            txtDetalleNombre.Text = _casa.Nombre;
            txtDetalleDueno.Text = _casa.DuenoNombre;
            txtDetalleCategoria.Text = _casa.CategoriaNombre;
            txtDetalleMoneda.Text = _casa.Moneda;
            txtDetalleEstado.Text = _casa.Activo ? "✅ Activo" : "❌ Inactivo";
            txtLinkContrato.Text = _casa.LinkContrato ?? string.Empty;

            // Cargar notas
            await CargarNotasAsync();

            // Cargar fotos
            await CargarFotosAsync();
        }

        private async Task CargarNotasAsync()
        {
            var notasSupabase = await SupabaseNotaHelper.ObtenerNotasPorCasaAsync(_casa.Id);
            
            notasCasa = notasSupabase.Select(n => new Nota
            {
                Id = n.Id,
                CasaId = n.CasaId,
                Contenido = n.Contenido,
                FechaCreacion = n.FechaCreacion
            }).ToList();

            lstNotas.ItemsSource = notasCasa;

            if (notasCasa.Any())
            {
                txtNoHayNotas.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtNoHayNotas.Visibility = Visibility.Visible;
            }
        }

        private async void AgregarNota_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNuevaNota.Text))
            {
                CustomMessageBox.Show(
                    "Por favor ingrese el contenido de la nota",
                    "Campo Requerido",
                    CustomMessageBox.MessageBoxType.Warning,
                    CustomMessageBox.MessageBoxButtons.OK
                );
                return;
            }

            var nuevaNota = new NotaSupabase
            {
                CasaId = _casa.Id,
                Contenido = txtNuevaNota.Text.Trim(),
                FechaCreacion = DateTime.Now
            };

            var resultado = await SupabaseNotaHelper.InsertarNotaAsync(nuevaNota);

            if (resultado.Success)
            {
                txtNuevaNota.Clear();
                await CargarNotasAsync();

                CustomMessageBox.Show(
                    "Nota agregada exitosamente",
                    "Éxito",
                    CustomMessageBox.MessageBoxType.Success,
                    CustomMessageBox.MessageBoxButtons.OK
                );
            }
            else
            {
                CustomMessageBox.Show(
                    $"Error al agregar nota: {resultado.Error}",
                    "Error",
                    CustomMessageBox.MessageBoxType.Error,
                    CustomMessageBox.MessageBoxButtons.OK
                );
            }
        }

        private async void EditarNota_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var nota = button?.Tag as Nota;

            if (nota == null) return;

            var cardBg = Application.Current.Resources["CardBackgroundBrush"] as Brush ?? Brushes.White;
            var inputBg = Application.Current.Resources["InputBackgroundBrush"] as Brush ?? Brushes.White;
            var textPrimary = Application.Current.Resources["TextPrimaryBrush"] as Brush ?? Brushes.Black;
            var borderBrush = Application.Current.Resources["BorderBrush"] as Brush
                              ?? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E7EB"));
            var headerGradient = Application.Current.Resources["PrimaryGradient"] as Brush
                                 ?? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E3A8A"));

            var ventana = new Window
            {
                Title = "Editar Nota",
                Width = 500,
                SizeToContent = SizeToContent.Height,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                Background = cardBg,
                WindowStyle = WindowStyle.SingleBorderWindow,
                ResizeMode = ResizeMode.NoResize
            };

            // Layout principal
            var rootGrid = new Grid();
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Header azul
            var header = new Border
            {
                Background = headerGradient,
                Padding = new Thickness(25, 18, 25, 18)
            };
            var headerText = new TextBlock
            {
                Text = "✏️  Editar Nota",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White
            };
            header.Child = headerText;
            Grid.SetRow(header, 0);
            rootGrid.Children.Add(header);

            // Contenido
            var stackPanel = new StackPanel { Margin = new Thickness(25, 20, 25, 25) };
            Grid.SetRow(stackPanel, 1);
            rootGrid.Children.Add(stackPanel);

            var textBox = new TextBox
            {
                Text = nota.Contenido,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                MinHeight = 150,
                Padding = new Thickness(10),
                FontSize = 14,
                Background = inputBg,
                Foreground = textPrimary,
                BorderBrush = borderBrush,
                BorderThickness = new Thickness(1),
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 15, 0, 0)
            };

            var btnGuardar = new Button
            {
                Content = "💾 Guardar",
                Padding = new Thickness(20, 10, 20, 10),
                Margin = new Thickness(0, 0, 10, 0),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            var btnCancelar = new Button
            {
                Content = "✕ Cancelar",
                Padding = new Thickness(20, 10, 20, 10),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            btnGuardar.Click += async (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    CustomMessageBox.Show(
                        "La nota no puede estar vacía",
                        "Error",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK
                    );
                    return;
                }

                var notaActualizada = new NotaSupabase
                {
                    Id = nota.Id,
                    CasaId = nota.CasaId,
                    Contenido = textBox.Text.Trim(),
                    FechaCreacion = nota.FechaCreacion
                };

                var resultado = await SupabaseNotaHelper.ActualizarNotaAsync(notaActualizada);

                if (resultado.Success)
                {
                    ventana.DialogResult = true;
                    ventana.Close();
                    await CargarNotasAsync();
                }
                else
                {
                    CustomMessageBox.Show(
                        $"Error al actualizar: {resultado.Error}",
                        "Error",
                        CustomMessageBox.MessageBoxType.Error,
                        CustomMessageBox.MessageBoxButtons.OK
                    );
                }
            };

            btnCancelar.Click += (s, ev) => ventana.Close();

            buttonPanel.Children.Add(btnGuardar);
            buttonPanel.Children.Add(btnCancelar);
            stackPanel.Children.Add(textBox);
            stackPanel.Children.Add(buttonPanel);
            ventana.Content = rootGrid;

            ventana.ShowDialog();
        }

        private async void EliminarNota_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var nota = button?.Tag as Nota;

            if (nota == null) return;

            var resultado = CustomMessageBox.Show(
                "¿Estás seguro de eliminar esta nota?",
                "Confirmar Eliminación",
                CustomMessageBox.MessageBoxType.Warning,
                CustomMessageBox.MessageBoxButtons.YesNo
            );

            if (resultado == true)
            {
                var resultadoEliminacion = await SupabaseNotaHelper.EliminarNotaAsync(nota.Id);

                if (resultadoEliminacion.Success)
                {
                    CustomMessageBox.Show(
                        "Nota eliminada correctamente",
                        "Éxito",
                        CustomMessageBox.MessageBoxType.Success,
                        CustomMessageBox.MessageBoxButtons.OK
                    );

                    await CargarNotasAsync();
                }
                else
                {
                    CustomMessageBox.Show(
                        $"Error al eliminar: {resultadoEliminacion.Error}",
                        "Error",
                        CustomMessageBox.MessageBoxType.Error,
                        CustomMessageBox.MessageBoxButtons.OK
                    );
                }
            }
        }

        // ==================== FOTOS ====================

        private async Task CargarFotosAsync()
        {
            try
            {
                Console.WriteLine($"🔍 Cargando fotos para casa ID: {_casa.Id}");
                
                // Obtener fotos de la base de datos
                fotosCasa = await SupabaseFotoCasaHelper.ObtenerFotosPorCasaAsync(_casa.Id);
                
                Console.WriteLine($"📷 Fotos encontradas: {fotosCasa.Count}");
                foreach (var f in fotosCasa)
                {
                    Console.WriteLine($"   - URL: {f.Url}");
                }
                
                galeriaFotos.Children.Clear();

                if (fotosCasa.Any())
                {
                    txtNoHayFotos.Visibility = Visibility.Collapsed;

                    foreach (var foto in fotosCasa)
                    {
                        // Contenedor principal
                        var border = new Border
                        {
                            Width = 200,
                            Height = 200,
                            Margin = new Thickness(0, 0, 20, 20),
                            CornerRadius = new CornerRadius(12),
                            BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E7EB")),
                            BorderThickness = new Thickness(1),
                            Background = Brushes.White,
                            ClipToBounds = true,
                            Cursor = System.Windows.Input.Cursors.Hand,
                            Tag = foto
                        };

                        border.Effect = new System.Windows.Media.Effects.DropShadowEffect
                        {
                            Color = Colors.Black,
                            Opacity = 0.08,
                            BlurRadius = 12,
                            ShadowDepth = 4,
                            Direction = 270
                        };

                        // Crear menú contextual
                        var contextMenu = new ContextMenu();
                        
                        // Opción: Copiar imagen
                        var menuCopiar = new MenuItem
                        {
                            Header = "📋 Copiar imagen",
                            Tag = foto
                        };
                        menuCopiar.Click += CopiarImagen_Click;
                        contextMenu.Items.Add(menuCopiar);
                        
                        // Opción: Descargar imagen
                        var menuDescargar = new MenuItem
                        {
                            Header = "⬇️ Descargar imagen",
                            Tag = foto
                        };
                        menuDescargar.Click += DescargarImagen_Click;
                        contextMenu.Items.Add(menuDescargar);
                        
                        // Separador
                        contextMenu.Items.Add(new Separator());
                        
                        // Opción: Eliminar
                        var menuEliminar = new MenuItem
                        {
                            Header = "🗑️ Eliminar",
                            Tag = foto
                        };
                        menuEliminar.Click += EliminarFoto_Click;
                        contextMenu.Items.Add(menuEliminar);
                        
                        border.ContextMenu = contextMenu;

                        var grid = new Grid();

                        // Imagen
                        var image = new Image
                        {
                            Stretch = Stretch.UniformToFill,
                            Tag = foto
                        };

                        try
                        {
                            Console.WriteLine($"🖼️ Intentando cargar imagen desde: {foto.Url}");
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(foto.Url);
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            image.Source = bitmap;
                            Console.WriteLine($"✓ Imagen cargada correctamente");
                        }
                        catch (Exception imgEx)
                        {
                            Console.WriteLine($"✗ Error al cargar imagen: {imgEx.Message}");
                            // Si falla cargar la imagen, mostrar placeholder
                            var placeholderBorder = new Border
                            {
                                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3F4F6"))
                            };
                            var textBlock = new TextBlock
                            {
                                Text = "📷",
                                FontSize = 64,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D5DB"))
                            };
                            placeholderBorder.Child = textBlock;
                            grid.Children.Add(placeholderBorder);
                        }

                        if (image.Source != null)
                        {
                            grid.Children.Add(image);
                        }

                        border.Child = grid;
                        galeriaFotos.Children.Add(border);
                    }
                }
                else
                {
                    txtNoHayFotos.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar fotos: {ex.Message}");
                txtNoHayFotos.Visibility = Visibility.Visible;
            }
        }

        private async void EliminarFoto_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var foto = menuItem?.Tag as FotoCasaSupabase;

            if (foto == null) return;

            var resultado = CustomMessageBox.Show(
                "¿Estás seguro de eliminar esta foto?",
                "Confirmar Eliminación",
                CustomMessageBox.MessageBoxType.Warning,
                CustomMessageBox.MessageBoxButtons.YesNo
            );

            if (resultado == true)
            {
                try
                {
                    // Eliminar de Storage
                    var eliminadoStorage = await SupabaseStorageHelper.EliminarImagenCasaAsync(foto.Url);
                    
                    // Eliminar de base de datos
                    var resultadoDB = await SupabaseFotoCasaHelper.EliminarFotoAsync(foto.Id);

                    if (resultadoDB.Success)
                    {
                        CustomMessageBox.Show(
                            "Foto eliminada correctamente",
                            "Éxito",
                            CustomMessageBox.MessageBoxType.Success,
                            CustomMessageBox.MessageBoxButtons.OK
                        );

                        await CargarFotosAsync();
                    }
                    else
                    {
                        CustomMessageBox.Show(
                            $"Error al eliminar: {resultadoDB.Error}",
                            "Error",
                            CustomMessageBox.MessageBoxType.Error,
                            CustomMessageBox.MessageBoxButtons.OK
                        );
                    }
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show(
                        $"Error: {ex.Message}",
                        "Error",
                        CustomMessageBox.MessageBoxType.Error,
                        CustomMessageBox.MessageBoxButtons.OK
                    );
                }
            }
        }

        private async void CopiarImagen_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var foto = menuItem?.Tag as FotoCasaSupabase;

            if (foto == null) return;

            try
            {
                // Descargar imagen desde URL
                using var httpClient = new HttpClient();
                var imageBytes = await httpClient.GetByteArrayAsync(foto.Url);
                
                // Convertir a BitmapImage
                var bitmap = new BitmapImage();
                using (var stream = new MemoryStream(imageBytes))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }

                // Copiar al portapapeles
                Clipboard.SetImage(bitmap);

                CustomMessageBox.Show(
                    "Imagen copiada al portapapeles",
                    "Éxito",
                    CustomMessageBox.MessageBoxType.Success,
                    CustomMessageBox.MessageBoxButtons.OK
                );
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(
                    $"Error al copiar imagen: {ex.Message}",
                    "Error",
                    CustomMessageBox.MessageBoxType.Error,
                    CustomMessageBox.MessageBoxButtons.OK
                );
            }
        }

        private async void DescargarImagen_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var foto = menuItem?.Tag as FotoCasaSupabase;

            if (foto == null) return;

            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    FileName = foto.NombreArchivo,
                    Filter = "Archivos de Imagen|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                    DefaultExt = Path.GetExtension(foto.NombreArchivo)
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using var httpClient = new HttpClient();
                    var imageBytes = await httpClient.GetByteArrayAsync(foto.Url);
                    await File.WriteAllBytesAsync(saveFileDialog.FileName, imageBytes);

                    CustomMessageBox.Show(
                        "Imagen descargada correctamente",
                        "Éxito",
                        CustomMessageBox.MessageBoxType.Success,
                        CustomMessageBox.MessageBoxButtons.OK
                    );
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(
                    $"Error al descargar imagen: {ex.Message}",
                    "Error",
                    CustomMessageBox.MessageBoxType.Error,
                    CustomMessageBox.MessageBoxButtons.OK
                );
            }
        }

        private async void DescargarTodasFotos_Click(object sender, RoutedEventArgs e)
        {
            if (!fotosCasa.Any())
            {
                CustomMessageBox.Show(
                    "No hay fotos para descargar",
                    "Información",
                    CustomMessageBox.MessageBoxType.Warning,
                    CustomMessageBox.MessageBoxButtons.OK
                );
                return;
            }

            try
            {
                var folderDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Title = "Guardar fotos en carpeta",
                    FileName = $"{_casa.Nombre}_Fotos",
                    DefaultExt = ".zip",
                    Filter = "Archivo ZIP|*.zip"
                };

                if (folderDialog.ShowDialog() == true)
                {
                    using var httpClient = new HttpClient();
                    var rutaZip = folderDialog.FileName;

                    // Crear ZIP con las fotos
                    using (var zipArchive = System.IO.Compression.ZipFile.Open(rutaZip, System.IO.Compression.ZipArchiveMode.Create))
                    {
                        int contador = 1;
                        foreach (var foto in fotosCasa)
                        {
                            try
                            {
                                var imageBytes = await httpClient.GetByteArrayAsync(foto.Url);
                                var entry = zipArchive.CreateEntry(foto.NombreArchivo);
                                
                                using var entryStream = entry.Open();
                                await entryStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                                
                                Console.WriteLine($"✓ Foto {contador}/{fotosCasa.Count} agregada al ZIP");
                                contador++;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"✗ Error descargando {foto.NombreArchivo}: {ex.Message}");
                            }
                        }
                    }

                    CustomMessageBox.Show(
                        $"{fotosCasa.Count} fotos descargadas en:\n{rutaZip}",
                        "Éxito",
                        CustomMessageBox.MessageBoxType.Success,
                        CustomMessageBox.MessageBoxButtons.OK
                    );
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(
                    $"Error al descargar fotos: {ex.Message}",
                    "Error",
                    CustomMessageBox.MessageBoxType.Error,
                    CustomMessageBox.MessageBoxButtons.OK
                );
            }
        }

        private async void BuscarFotos_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Seleccionar Fotos",
                Filter = "Archivos de Imagen|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Todos los archivos (*.*)|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                await SubirFotosAsync(openFileDialog.FileNames);
            }
        }

        private async Task SubirFotosAsync(string[] rutasArchivos)
        {
            // Filtrar solo archivos de imagen
            var extensionesValidas = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
            var archivosValidos = rutasArchivos.Where(f => 
                extensionesValidas.Contains(Path.GetExtension(f).ToLower())).ToArray();

            if (!archivosValidos.Any())
            {
                CustomMessageBox.Show(
                    "No se seleccionaron archivos de imagen válidos",
                    "Error",
                    CustomMessageBox.MessageBoxType.Warning,
                    CustomMessageBox.MessageBoxButtons.OK
                );
                return;
            }

            int exitosas = 0;
            int fallidas = 0;

            foreach (var rutaArchivo in archivosValidos)
            {
                try
                {
                    Console.WriteLine($"📁 Procesando archivo: {Path.GetFileName(rutaArchivo)}");
                    
                    var nombreArchivo = $"casa_{_casa.Id}_{Guid.NewGuid()}{Path.GetExtension(rutaArchivo)}";
                    Console.WriteLine($"📝 Nombre generado: {nombreArchivo}");
                    
                    var bytes = await File.ReadAllBytesAsync(rutaArchivo);
                    Console.WriteLine($"📊 Bytes leídos: {bytes.Length}");

                    // Subir a Supabase Storage
                    Console.WriteLine($"⬆️ Iniciando upload a Storage...");
                    var resultadoStorage = await SupabaseStorageHelper.SubirImagenCasaAsync(bytes, nombreArchivo);

                    if (resultadoStorage.Success && resultadoStorage.Url != null)
                    {
                        Console.WriteLine($"✓ Storage OK. URL: {resultadoStorage.Url}");
                        
                        // Guardar registro en base de datos
                        var foto = new FotoCasaSupabase
                        {
                            CasaId = _casa.Id,
                            Url = resultadoStorage.Url,
                            NombreArchivo = nombreArchivo,
                            FechaCreacion = DateTime.Now
                        };

                        Console.WriteLine($"💾 Guardando en base de datos...");
                        var resultadoDB = await SupabaseFotoCasaHelper.InsertarFotoAsync(foto);

                        if (resultadoDB.Success)
                        {
                            exitosas++;
                            Console.WriteLine($"✓ ¡Foto completamente subida! {nombreArchivo}");
                        }
                        else
                        {
                            fallidas++;
                            Console.WriteLine($"✗ Error DB: {resultadoDB.Error}");
                        }
                    }
                    else
                    {
                        fallidas++;
                        Console.WriteLine($"✗ Error Storage: {resultadoStorage.Error}");
                    }
                }
                catch (Exception ex)
                {
                    fallidas++;
                    Console.WriteLine($"✗ EXCEPCIÓN procesando {Path.GetFileName(rutaArchivo)}:");
                    Console.WriteLine($"   Mensaje: {ex.Message}");
                    Console.WriteLine($"   Stack: {ex.StackTrace}");
                }
            }

            if (exitosas > 0)
            {
                await CargarFotosAsync();
            }

            if (fallidas == 0)
            {
                CustomMessageBox.Show(
                    $"{exitosas} foto(s) subida(s) correctamente",
                    "Éxito",
                    CustomMessageBox.MessageBoxType.Success,
                    CustomMessageBox.MessageBoxButtons.OK
                );
            }
            else
            {
                CustomMessageBox.Show(
                    $"Subidas: {exitosas}\nFallidas: {fallidas}",
                    "Resultado",
                    CustomMessageBox.MessageBoxType.Warning,
                    CustomMessageBox.MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Formatea un monto según la moneda de la casa
        /// </summary>
        private string FormatearMonedaCasa(decimal monto)
        {
            switch (_casa.Moneda)
            {
                case "USD":
                    return monto.ToString("C", new CultureInfo("en-US"));
                case "CRC":
                    return monto.ToString("C", new CultureInfo("es-CR"));
                default:
                    return monto.ToString("C", new CultureInfo("en-US"));
            }
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // ==================== TAB MOVIMIENTOS IA ====================

        private async void TabMovimientosIA_Click(object sender, RoutedEventArgs e)
        {
            ResetTabButtons();
            btnTabMovimientosIA.Background = new SolidColorBrush(Color.FromRgb(245, 158, 11));
            btnTabMovimientosIA.Foreground = Brushes.White;
            tabMovimientosIA.Visibility = Visibility.Visible;

            // Inicializar filtros si aún no están poblados
            if (cmbIAAnio.Items.Count == 0)
            {
                var anios = await SupabaseHojaMensualHelper.ObtenerAniosDisponiblesAsync(_casa.Id);
                cmbIAAnio.ItemsSource = anios;
                var anioActual = DateTime.Now.Year;
                cmbIAAnio.SelectedItem = anios.Contains(anioActual) ? (object)anioActual : anios.FirstOrDefault();
            }

            if (cmbIAMes.SelectedItem == null)
            {
                foreach (ComboBoxItem item in cmbIAMes.Items)
                {
                    if (item.Tag is string t && int.TryParse(t, out int m) && m == DateTime.Now.Month)
                    {
                        cmbIAMes.SelectedItem = item;
                        break;
                    }
                }
            }

            await CargarMovimientosIAAsync();
        }

        private async void CmbIAAnio_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => await CargarMovimientosIAAsync();

        private async void CmbIAMes_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => await CargarMovimientosIAAsync();

        private async Task CargarMovimientosIAAsync()
        {
            if (cmbIAAnio.SelectedItem is not int anio) return;
            if (cmbIAMes.SelectedItem is not ComboBoxItem mesItem) return;
            if (!int.TryParse(mesItem.Tag?.ToString(), out int mes)) return;

            var lista = await SupabaseMovimientoIAHelper.ObtenerPorCasaAsync(_casa.Id, mes, anio);
            _movimientosIA.Clear();
            foreach (var m in lista) _movimientosIA.Add(m);

            dgMovimientosIA.ItemsSource = _movimientosIA;
            txtNoDataIA.Visibility = _movimientosIA.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

            // Actualizar badge
            ActualizarBadgeIA();
        }

        private void ActualizarBadgeIA()
        {
            var pendientes = _movimientosIA.Count(m => m.Estado != "aprobado");
            badgeIA.Visibility = pendientes > 0 ? Visibility.Visible : Visibility.Collapsed;
            txtBadgeIA.Text = pendientes.ToString();
        }

        private void IASeleccionarTodos_Click(object sender, RoutedEventArgs e)
        {
            var todosSeleccionados = _movimientosIA.All(m => m.Seleccionado);
            foreach (var m in _movimientosIA)
                m.Seleccionado = !todosSeleccionados;
        }

        private async void IABorrarSeleccionados_Click(object sender, RoutedEventArgs e)
        {
            // Forzar commit de cualquier celda en edición antes de leer los valores
            dgMovimientosIA.CommitEdit(DataGridEditingUnit.Row, true);

            var seleccionados = _movimientosIA.Where(m => m.Seleccionado).ToList();
            if (!seleccionados.Any())
            {
                CustomMessageBox.Show("Selecciona al menos un registro para borrar.",
                    "Sin selección", CustomMessageBox.MessageBoxType.Warning, CustomMessageBox.MessageBoxButtons.OK);
                return;
            }

            var confirmacion = CustomMessageBox.Show(
                $"¿Borrar {seleccionados.Count} registro(s) de movimientos IA?",
                "Confirmar eliminación", CustomMessageBox.MessageBoxType.Warning, CustomMessageBox.MessageBoxButtons.YesNo);

            if (confirmacion != true) return;

            var (ok, err) = await SupabaseMovimientoIAHelper.EliminarVariosAsync(seleccionados.Select(m => m.Id));
            if (ok)
                await CargarMovimientosIAAsync();
            else
                CustomMessageBox.Show($"Error al borrar: {err}", "Error",
                    CustomMessageBox.MessageBoxType.Error, CustomMessageBox.MessageBoxButtons.OK);
        }

        private async void IAAceptarSeleccionados_Click(object sender, RoutedEventArgs e)
        {
            // Forzar commit de cualquier celda en edición antes de leer los valores
            dgMovimientosIA.CommitEdit(DataGridEditingUnit.Row, true);

            var seleccionados = _movimientosIA.Where(m => m.Seleccionado && m.Fecha.HasValue && m.Monto.HasValue).ToList();
            if (!seleccionados.Any())
            {
                CustomMessageBox.Show("Selecciona registros que tengan fecha y monto para poder aceptarlos.",
                    "Sin datos completos", CustomMessageBox.MessageBoxType.Warning, CustomMessageBox.MessageBoxButtons.OK);
                return;
            }

            // Siempre usar el mes/año de los filtros del tab IA (mes vigente de trabajo),
            // independientemente de la fecha de la factura.
            if (cmbIAMes.SelectedItem is not ComboBoxItem mesItem
                || !int.TryParse(mesItem.Tag?.ToString(), out int mes)
                || cmbIAAnio.SelectedItem is not int anio)
            {
                CustomMessageBox.Show("Selecciona el mes y año de destino en los filtros.",
                    "Sin filtro", CustomMessageBox.MessageBoxType.Warning, CustomMessageBox.MessageBoxButtons.OK);
                return;
            }

            var hoja = hojasDisponibles.FirstOrDefault(h => h.Mes == mes && h.Anio == anio);

            if (hoja == null)
            {
                CustomMessageBox.Show($"No existe hoja mensual para {mesItem.Content} {anio}. Asegúrate de que ese mes esté creado.",
                    "Hoja no encontrada", CustomMessageBox.MessageBoxType.Error, CustomMessageBox.MessageBoxButtons.OK);
                return;
            }

            // Mostrar estado de carga
            btnIAAceptar.IsEnabled = false;
            var txtBtnAceptar = btnIAAceptar.FindName("") as System.Windows.Controls.TextBlock;
            var originalContent = btnIAAceptar.Content;
            btnIAAceptar.Content = new System.Windows.Controls.TextBlock
            {
                Text = "⏳ Procesando...",
                FontSize = 12,
                FontWeight = FontWeights.SemiBold
            };

            try
            {
                var usuario = SupabaseAuthHelper.GetCurrentUser();

                // 1. Construir todos los movimientos de una vez
                var movimientosDb = seleccionados.Select(ia => new MovimientoSupabase
                {
                    CasaId         = _casa.Id,
                    HojaMensualId  = hoja.Id,
                    Fecha          = ia.Fecha!.Value,
                    Monto          = ia.Monto!.Value,
                    Descripcion    = ia.Descripcion ?? "",
                    Categoria      = ia.Categoria ?? "",
                    TipoMovimiento = ia.TipoMovimiento == "Ingreso" ? "Ingreso" : "Gasto",
                    ImagenUrl      = ia.FacturaUrl,
                    FechaCreacion  = DateTime.Now
                }).ToList();

                // 2. Batch insert — 1 sola llamada HTTP para todos
                var (ok, insertedIds, err) = await SupabaseMovimientoHelper.InsertarBatchAsync(movimientosDb);
                if (!ok)
                {
                    CustomMessageBox.Show($"Error al crear movimientos: {err}", "Error",
                        CustomMessageBox.MessageBoxType.Error, CustomMessageBox.MessageBoxButtons.OK);
                    return;
                }

                // 3. Bulk update IA records — 1 sola llamada HTTP para todos
                var iaIds = seleccionados.Select(s => s.Id);
                await SupabaseMovimientoIAHelper.MarcarAprobadosBatchAsync(iaIds, usuario?.Email ?? "desconocido");

                CustomMessageBox.Show(
                    $"{insertedIds.Count} movimiento(s) creados correctamente.",
                    "Éxito", CustomMessageBox.MessageBoxType.Success, CustomMessageBox.MessageBoxButtons.OK);

                // Quitar los aprobados de la lista local
                var aprobadosIds = new HashSet<int>(seleccionados.Select(s => s.Id));
                foreach (var item in _movimientosIA.Where(m => aprobadosIds.Contains(m.Id)).ToList())
                    _movimientosIA.Remove(item);

                txtNoDataIA.Visibility = _movimientosIA.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                ActualizarBadgeIA();
            }
            finally
            {
                btnIAAceptar.Content  = originalContent;
                btnIAAceptar.IsEnabled = true;
            }
        }

        private async void IAImportarFacturas_Click(object sender, RoutedEventArgs e)
        {
            int anio = DateTime.Now.Year;
            int mes  = DateTime.Now.Month;
            int? hojaId = null;

            if (cmbIAAnio.SelectedItem is int a) anio = a;
            if (cmbIAMes.SelectedItem is ComboBoxItem mi && int.TryParse(mi.Tag?.ToString(), out int m)) mes = m;

            var hoja = hojasDisponibles.FirstOrDefault(h => h.Mes == mes && h.Anio == anio);
            hojaId = hoja?.Id;

            var emailUsuario = SupabaseAuthHelper.GetCurrentUser()?.Email ?? "";

            var dlg = new ImportarFacturasDialog(_casa.Id, mes, anio, hojaId, emailUsuario)
            {
                Owner = this,
                ArchivoCompletado = CargarMovimientosIAAsync
            };
            dlg.ShowDialog();

            // Recarga final por si el dialog se cerró antes de terminar el último archivo
            await CargarMovimientosIAAsync();
        }

        /// <summary>
        /// Muestra preview de la factura cargada: imagen + descarga + cerrar.
        /// </summary>
        private void IAVerFoto_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not MovimientoIA ia) return;

            if (string.IsNullOrWhiteSpace(ia.FacturaUrl))
            {
                CustomMessageBox.Show("Este registro no tiene imagen adjunta.",
                    "Sin imagen", CustomMessageBox.MessageBoxType.Warning, CustomMessageBox.MessageBoxButtons.OK);
                return;
            }

            var dlg = new FotoPreviewDialog(ia.FacturaUrl) { Owner = this };
            dlg.ShowDialog();
        }

        /// <summary>
        /// Persiste en Supabase los cambios inline de una fila IA al salir de la edición.
        /// </summary>
        private void dgMovimientosIA_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;
            if (e.Row.Item is not MovimientoIA ia) return;

            // Esperar a que WPF aplique el commit antes de leer los valores
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                await SupabaseMovimientoIAHelper.ActualizarConDatosIAAsync(
                    ia.Id, ia.Fecha, ia.Monto, ia.Descripcion, ia.Categoria,
                    ia.TipoMovimiento, ia.Estado, ia.RawJson);
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private async void IAEliminarFila_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not MovimientoIA ia) return;

            var conf = CustomMessageBox.Show("¿Eliminar este registro IA?",
                "Confirmar", CustomMessageBox.MessageBoxType.Warning, CustomMessageBox.MessageBoxButtons.YesNo);
            if (conf != true) return;

            await SupabaseMovimientoIAHelper.EliminarAsync(ia.Id);
            await CargarMovimientosIAAsync();
        }

        private void SeleccionarContrato_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Seleccionar contrato",
                Filter = "Documentos|*.pdf;*.xlsx;*.xls;*.docx;*.doc|Todos los archivos|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                txtLinkContrato.Text = dialog.FileName;
            }
        }

        private void AbrirContrato_Click(object sender, RoutedEventArgs e)
        {
            var link = txtLinkContrato.Text.Trim();
            if (string.IsNullOrEmpty(link)) return;

            try
            {
                Process.Start(new ProcessStartInfo(link) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(
                    $"No se pudo abrir el contrato: {ex.Message}",
                    "Error",
                    CustomMessageBox.MessageBoxType.Error,
                    CustomMessageBox.MessageBoxButtons.OK
                );
            }
        }

        private async void GuardarContrato_Click(object sender, RoutedEventArgs e)
        {
            _casa.LinkContrato = txtLinkContrato.Text.Trim();

            var casaDb = _casa.ToSupabase();
            var resultado = await SupabaseCasaHelper.ActualizarCasaAsync(casaDb);

            if (resultado.Success)
            {
                CustomMessageBox.Show(
                    "Contrato guardado correctamente",
                    "Éxito",
                    CustomMessageBox.MessageBoxType.Success,
                    CustomMessageBox.MessageBoxButtons.OK
                );
            }
            else
            {
                CustomMessageBox.Show(
                    $"Error al guardar: {resultado.Error}",
                    "Error",
                    CustomMessageBox.MessageBoxType.Error,
                    CustomMessageBox.MessageBoxButtons.OK
                );
            }
        }
    }
}
