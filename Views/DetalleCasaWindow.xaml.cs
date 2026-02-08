using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using System.Net.Http;

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

        private async void TabResumen_Click(object sender, RoutedEventArgs e)
        {
            btnTabResumen.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(59, 130, 246));
            btnTabResumen.Foreground = System.Windows.Media.Brushes.White;
            btnTabMovimientos.Background = System.Windows.Media.Brushes.Transparent;
            btnTabMovimientos.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(107, 114, 128));
            btnTabDetalles.Background = System.Windows.Media.Brushes.Transparent;
            btnTabDetalles.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(107, 114, 128));

            tabResumen.Visibility = Visibility.Visible;
            tabMovimientos.Visibility = Visibility.Collapsed;
            tabDetalles.Visibility = Visibility.Collapsed;
            
            // Recargar todos los datos del resumen al cambiar a esta pestaña
            await CargarEstadoGeneralAsync();
            await CargarFiltrosResumenAsync();
        }

        private void TabMovimientos_Click(object sender, RoutedEventArgs e)
        {
            btnTabResumen.Background = System.Windows.Media.Brushes.Transparent;
            btnTabResumen.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(107, 114, 128));
            btnTabMovimientos.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(59, 130, 246));
            btnTabMovimientos.Foreground = System.Windows.Media.Brushes.White;
            btnTabDetalles.Background = System.Windows.Media.Brushes.Transparent;
            btnTabDetalles.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(107, 114, 128));

            tabResumen.Visibility = Visibility.Collapsed;
            tabMovimientos.Visibility = Visibility.Visible;
            tabDetalles.Visibility = Visibility.Collapsed;
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

            var ventana = new AgregarMovimientoWindow(_casa.Id, hojaSeleccionada.Id, _casa.Nombre);
            ventana.Owner = this;
            
            if (ventana.ShowDialog() == true)
            {
                await CargarDatosAsync(); // Recargar todo incluyendo resumen
            }
        }

        private async void EditarMovimiento_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var movimiento = button?.Tag as Movimiento;
            
            if (movimiento == null) return;

            var ventana = new AgregarMovimientoWindow(_casa.Id, movimiento.HojaMensualId ?? 0, _casa.Nombre, movimiento);
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
            tabResumen.Visibility = Visibility.Collapsed;
            tabMovimientos.Visibility = Visibility.Collapsed;
            tabDetalles.Visibility = Visibility.Visible;

            btnTabResumen.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
            btnTabResumen.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
            btnTabMovimientos.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
            btnTabMovimientos.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
            btnTabDetalles.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"));
            btnTabDetalles.Foreground = new SolidColorBrush(Colors.White);

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
            txtDetalleFecha.Text = _casa.FechaCreacion.ToString("dd/MM/yyyy");

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

            var ventana = new Window
            {
                Title = "Editar Nota",
                Width = 500,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3F4F6"))
            };

            var stackPanel = new StackPanel { Margin = new Thickness(20) };
            
            var textBox = new TextBox
            {
                Text = nota.Contenido,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                MinHeight = 150,
                Padding = new Thickness(10),
                FontSize = 14,
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E7EB")),
                BorderThickness = new Thickness(1)
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
            ventana.Content = stackPanel;

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
            var button = sender as Button;
            var foto = button?.Tag as FotoCasaSupabase;

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
    }
}
