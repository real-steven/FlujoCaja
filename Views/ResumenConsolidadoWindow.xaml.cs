using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FlujoCajaWpf.Views
{
    public partial class ResumenConsolidadoWindow : Window
    {
        private List<Casa> todasLasCasas = new List<Casa>();
        private int? anioFiltro = null;
        private int? mesFiltro = null;

        public ResumenConsolidadoWindow()
        {
            InitializeComponent();
            Loaded += async (s, e) => await CargarDatosAsync();
        }

        private async Task CargarDatosAsync()
        {
            await CargarFiltrosAsync();
            await CargarResumenAsync();
        }

        private async Task CargarFiltrosAsync()
        {
            // Cargar años desde la creación del sistema hasta el año actual
            var anios = new List<int>();
            for (int anio = 2020; anio <= DateTime.Now.Year; anio++)
            {
                anios.Add(anio);
            }
            anios.Reverse(); // Más recientes primero

            cmbAnio.ItemsSource = anios;
            
            // No seleccionar ningún año por defecto (toda la vida)
            cmbAnio.SelectedIndex = -1;
        }

        private async Task CargarResumenAsync()
        {
            try
            {
                // Obtener usuario actual
                var usuarioActual = SupabaseAuthHelper.GetCurrentUser();
                if (usuarioActual == null)
                {
                    CustomMessageBox.Show(
                        "No se pudo obtener el usuario actual",
                        "Error",
                        CustomMessageBox.MessageBoxType.Error,
                        CustomMessageBox.MessageBoxButtons.OK
                    );
                    return;
                }

                // Obtener todas las casas activas
                var resultadoCasas = await SupabaseCasaHelper.ObtenerCasasAsync();
                
                if (!resultadoCasas.Success || resultadoCasas.Data == null)
                {
                    return;
                }

                todasLasCasas = resultadoCasas.Data.Where(c => c.Activo).ToList();

                // Calcular KPIs
                var totalCasas = todasLasCasas.Count;
                var casasUSD = todasLasCasas.Count(c => c.Moneda == "USD");
                var casasCRC = todasLasCasas.Count(c => c.Moneda == "CRC");

                // Actualizar UI de contadores
                txtTotalCasas.Text = totalCasas.ToString();
                txtCasasUSD.Text = casasUSD.ToString();
                txtCasasCRC.Text = casasCRC.ToString();

                // Calcular balances por moneda
                decimal balanceUSD = 0, balanceCRC = 0;
                decimal ingresosUSD = 0, ingresosCRC = 0;
                decimal gastosUSD = 0, gastosCRC = 0;

                var casasConBalance = new List<CasaConBalance>();

                foreach (var casa in todasLasCasas)
                {
                    // Obtener movimientos según filtro
                    List<Movimiento> movimientos;

                    if (anioFiltro.HasValue && mesFiltro.HasValue)
                    {
                        // Filtrar por mes/año específico
                        var resultado = await SupabaseMovimientoHelper.ObtenerMovimientosPorMesAsync(casa.Id, anioFiltro.Value, mesFiltro.Value);
                        movimientos = resultado.Success && resultado.Data != null ? resultado.Data : new List<Movimiento>();
                    }
                    else if (anioFiltro.HasValue)
                    {
                        // Filtrar por año completo
                        var resultado = await SupabaseMovimientoHelper.ObtenerMovimientosPorCasaAsync(casa.Id);
                        movimientos = resultado.Success && resultado.Data != null 
                            ? resultado.Data.Where(m => m.Fecha.Year == anioFiltro.Value).ToList()
                            : new List<Movimiento>();
                    }
                    else
                    {
                        // Toda la vida
                        var resultado = await SupabaseMovimientoHelper.ObtenerMovimientosPorCasaAsync(casa.Id);
                        movimientos = resultado.Success && resultado.Data != null ? resultado.Data : new List<Movimiento>();
                    }

                    // Calcular balance de la casa (los montos ya vienen con signo correcto)
                    var balanceCasa = movimientos.Sum(m => m.Monto);
                    var ingresosCasa = movimientos.Where(m => m.Tipo == "Ingreso").Sum(m => m.Monto);
                    var gastosCasa = movimientos.Where(m => m.Tipo == "Gasto").Sum(m => Math.Abs(m.Monto));

                    // Agregar a la lista de casas con balance
                    casasConBalance.Add(new CasaConBalance
                    {
                        Nombre = casa.Nombre,
                        Moneda = casa.Moneda,
                        DuenoNombre = casa.DuenoNombre ?? "",
                        Balance = balanceCasa,
                        BalanceFormateado = FormatearMoneda(balanceCasa, casa.Moneda)
                    });

                    // Acumular por moneda
                    switch (casa.Moneda)
                    {
                        case "USD":
                            balanceUSD += balanceCasa;
                            ingresosUSD += ingresosCasa;
                            gastosUSD += gastosCasa;
                            break;
                        case "CRC":
                            balanceCRC += balanceCasa;
                            ingresosCRC += ingresosCasa;
                            gastosCRC += gastosCasa;
                            break;
                    }
                }

                // Actualizar UI de balances USD
                txtIngresosUSD.Text = FormatearMoneda(ingresosUSD, "USD");
                txtGastosUSD.Text = FormatearMoneda(gastosUSD, "USD");
                txtBalanceUSD.Text = FormatearMoneda(balanceUSD, "USD");
                txtBalanceUSD.Foreground = new SolidColorBrush(balanceUSD >= 0 ? Color.FromRgb(16, 185, 129) : Color.FromRgb(220, 38, 38));

                // Actualizar UI de balances CRC
                txtIngresosCRC.Text = FormatearMoneda(ingresosCRC, "CRC");
                txtGastosCRC.Text = FormatearMoneda(gastosCRC, "CRC");
                txtBalanceCRC.Text = FormatearMoneda(balanceCRC, "CRC");
                txtBalanceCRC.Foreground = new SolidColorBrush(balanceCRC >= 0 ? Color.FromRgb(59, 130, 246) : Color.FromRgb(220, 38, 38));

                // Actualizar DataGrid
                dgCasas.ItemsSource = casasConBalance.OrderByDescending(c => c.Balance);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(
                    $"Error al cargar el resumen: {ex.Message}",
                    "Error",
                    CustomMessageBox.MessageBoxType.Error,
                    CustomMessageBox.MessageBoxButtons.OK
                );
            }
        }

        private string FormatearMoneda(decimal monto, string moneda)
        {
            var culture = moneda switch
            {
                "USD" => new CultureInfo("en-US"),
                "CRC" => new CultureInfo("es-CR"),
                _ => new CultureInfo("en-US")
            };
            return monto.ToString("C", culture);
        }

        private async void Filtro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;

            // Actualizar filtros
            anioFiltro = cmbAnio.SelectedIndex >= 0 ? (int?)cmbAnio.SelectedItem : null;
            
            var mesItem = cmbMes.SelectedItem as ComboBoxItem;
            if (mesItem != null && mesItem.Tag != null)
            {
                var mesTag = int.Parse(mesItem.Tag.ToString()!);
                mesFiltro = mesTag > 0 ? (int?)mesTag : null;
            }

            await CargarResumenAsync();
        }

        private async void LimpiarFiltros_Click(object sender, RoutedEventArgs e)
        {
            cmbAnio.SelectedIndex = -1;
            cmbMes.SelectedIndex = 0; // "Todos"
            anioFiltro = null;
            mesFiltro = null;
            await CargarResumenAsync();
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Clase auxiliar para el DataGrid
        private class CasaConBalance
        {
            public string Nombre { get; set; } = string.Empty;
            public string Moneda { get; set; } = string.Empty;
            public string DuenoNombre { get; set; } = string.Empty;
            public decimal Balance { get; set; }
            public string BalanceFormateado { get; set; } = string.Empty;
        }
    }
}
