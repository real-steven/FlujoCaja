using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace FlujoCajaWpf.Views
{
    public partial class HistorialWindow : Window
    {
        private ObservableCollection<Auditoria> auditoriasCasas = new();
        private ObservableCollection<Auditoria> auditoriasMovimientos = new();
        private ObservableCollection<Auditoria> auditoriasDuenos = new();
        private ObservableCollection<Auditoria> auditoriasCategorias = new();
        private ObservableCollection<LogCorreo> logsCorreos = new();

        private int paginaActualCasas = 1;
        private int paginaActualMovimientos = 1;
        private int paginaActualDuenos = 1;
        private int paginaActualCategorias = 1;
        private int paginaActualCorreos = 1;
        private int totalRegistrosCasas = 0;
        private int totalRegistrosMovimientos = 0;
        private int totalRegistrosDuenos = 0;
        private int totalRegistrosCategorias = 0;
        private int totalRegistrosCorreos = 0;
        private const int REGISTROS_POR_PAGINA = 50;

        private string? usuarioActual;

        public HistorialWindow()
        {
            InitializeComponent();
            Loaded += HistorialWindow_Loaded;
        }

        private async void HistorialWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Obtener usuario actual
            var user = SupabaseAuthHelper.GetCurrentUser();
            usuarioActual = user?.Email;

            // Cargar filtros
            await CargarUsuariosAsync();

            // Cargar datos iniciales
            await CargarAuditoriasCasasAsync();
            await CargarAuditoriasMovimientosAsync();
            await CargarAuditoriasDuenosAsync();
            await CargarAuditoriasCatergoriasAsync();
            await CargarLogsCorreosAsync();
        }

        #region Filtros

        private async Task CargarUsuariosAsync()
        {
            var usuarios = await SupabaseAuditoriaHelper.ObtenerUsuariosAsync();

            // ComboBox de Casas
            cmbUsuarioCasas.Items.Clear();
            cmbUsuarioCasas.Items.Add(new ComboBoxItem { Content = "Todos", Tag = "" });
            foreach (var usuario in usuarios)
            {
                cmbUsuarioCasas.Items.Add(new ComboBoxItem { Content = usuario, Tag = usuario });
            }
            cmbUsuarioCasas.SelectedIndex = 0;

            // ComboBox de Movimientos
            cmbUsuarioMovimientos.Items.Clear();
            cmbUsuarioMovimientos.Items.Add(new ComboBoxItem { Content = "Todos", Tag = "" });
            foreach (var usuario in usuarios)
            {
                cmbUsuarioMovimientos.Items.Add(new ComboBoxItem { Content = usuario, Tag = usuario });
            }
            cmbUsuarioMovimientos.SelectedIndex = 0;

            // ComboBox de Dueños
            cmbUsuarioDuenos.Items.Clear();
            cmbUsuarioDuenos.Items.Add(new ComboBoxItem { Content = "Todos", Tag = "" });
            foreach (var usuario in usuarios)
            {
                cmbUsuarioDuenos.Items.Add(new ComboBoxItem { Content = usuario, Tag = usuario });
            }
            cmbUsuarioDuenos.SelectedIndex = 0;

            // ComboBox de Categorías
            cmbUsuarioCategorias.Items.Clear();
            cmbUsuarioCategorias.Items.Add(new ComboBoxItem { Content = "Todos", Tag = "" });
            foreach (var usuario in usuarios)
            {
                cmbUsuarioCategorias.Items.Add(new ComboBoxItem { Content = usuario, Tag = usuario });
            }
            cmbUsuarioCategorias.SelectedIndex = 0;

            // ComboBox de Correos — Usuario
            var usuariosCorreo = await SupabaseLogCorreoHelper.ObtenerUsuariosAsync();
            cmbUsuarioCorreos.Items.Clear();
            cmbUsuarioCorreos.Items.Add(new ComboBoxItem { Content = "Todos", Tag = "" });
            foreach (var u in usuariosCorreo)
                cmbUsuarioCorreos.Items.Add(new ComboBoxItem { Content = u, Tag = u });
            cmbUsuarioCorreos.SelectedIndex = 0;

            // ComboBox de Correos — Casa
            var casasCorreo = await SupabaseLogCorreoHelper.ObtenerCasasAsync();
            cmbCasaCorreos.Items.Clear();
            cmbCasaCorreos.Items.Add(new ComboBoxItem { Content = "Todas", Tag = "" });
            foreach (var c in casasCorreo)
                cmbCasaCorreos.Items.Add(new ComboBoxItem { Content = c, Tag = c });
            cmbCasaCorreos.SelectedIndex = 0;
        }

        #endregion

        #region Casas

        private async Task CargarAuditoriasCasasAsync()
        {
            var usuarioSeleccionado = (cmbUsuarioCasas.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            var accionSeleccionada = (cmbAccionCasas.SelectedItem as ComboBoxItem)?.Tag?.ToString();

            var buscarCasas = txtBuscarCasas.Text.Trim();

            var resultado = await SupabaseAuditoriaHelper.ObtenerAuditoriasPorModuloAsync(
                modulo: "casa",
                usuarioEmail: string.IsNullOrEmpty(usuarioSeleccionado) ? null : usuarioSeleccionado,
                tipoAccion: string.IsNullOrEmpty(accionSeleccionada) ? null : accionSeleccionada,
                buscar: string.IsNullOrEmpty(buscarCasas) ? null : buscarCasas,
                pagina: paginaActualCasas,
                registrosPorPagina: REGISTROS_POR_PAGINA
            );

            if (resultado.Success && resultado.Data != null)
            {
                auditoriasCasas = new ObservableCollection<Auditoria>(resultado.Data);
                dgCasas.ItemsSource = auditoriasCasas;
                totalRegistrosCasas = resultado.TotalRegistros;
                ActualizarPaginacionCasas();
            }
            else
            {
                MessageBox.Show(resultado.Error ?? "Error al cargar historial de casas", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActualizarPaginacionCasas()
        {
            int totalPaginas = (int)Math.Ceiling((double)totalRegistrosCasas / REGISTROS_POR_PAGINA);
            int inicio = (paginaActualCasas - 1) * REGISTROS_POR_PAGINA + 1;
            int fin = Math.Min(paginaActualCasas * REGISTROS_POR_PAGINA, totalRegistrosCasas);

            txtInfoCasas.Text = $"Mostrando {inicio}-{fin} de {totalRegistrosCasas} registros";
            txtPaginaCasas.Text = $"Página {paginaActualCasas} de {Math.Max(1, totalPaginas)}";

            btnAnteriorCasas.IsEnabled = paginaActualCasas > 1;
            btnSiguienteCasas.IsEnabled = paginaActualCasas < totalPaginas;
        }

        private async void BtnAnteriorCasas_Click(object sender, RoutedEventArgs e)
        {
            if (paginaActualCasas > 1)
            {
                paginaActualCasas--;
                await CargarAuditoriasCasasAsync();
            }
        }

        private async void BtnSiguienteCasas_Click(object sender, RoutedEventArgs e)
        {
            int totalPaginas = (int)Math.Ceiling((double)totalRegistrosCasas / REGISTROS_POR_PAGINA);
            if (paginaActualCasas < totalPaginas)
            {
                paginaActualCasas++;
                await CargarAuditoriasCasasAsync();
            }
        }

        private async void BtnBuscarCasas_Click(object sender, RoutedEventArgs e)
        {
            paginaActualCasas = 1;
            await CargarAuditoriasCasasAsync();
        }

        private async void FiltrosCasas_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            paginaActualCasas = 1;
            await CargarAuditoriasCasasAsync();
        }

        #endregion

        #region Movimientos

        private async Task CargarAuditoriasMovimientosAsync()
        {
            var usuarioSeleccionado = (cmbUsuarioMovimientos.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            var accionSeleccionada = (cmbAccionMovimientos.SelectedItem as ComboBoxItem)?.Tag?.ToString();

            var buscarMovimientos = txtBuscarMovimientos.Text.Trim();

            var resultado = await SupabaseAuditoriaHelper.ObtenerAuditoriasPorModuloAsync(
                modulo: "movimiento",
                usuarioEmail: string.IsNullOrEmpty(usuarioSeleccionado) ? null : usuarioSeleccionado,
                tipoAccion: string.IsNullOrEmpty(accionSeleccionada) ? null : accionSeleccionada,
                buscar: string.IsNullOrEmpty(buscarMovimientos) ? null : buscarMovimientos,
                pagina: paginaActualMovimientos,
                registrosPorPagina: REGISTROS_POR_PAGINA
            );

            if (resultado.Success && resultado.Data != null)
            {
                auditoriasMovimientos = new ObservableCollection<Auditoria>(resultado.Data);
                dgMovimientos.ItemsSource = auditoriasMovimientos;
                totalRegistrosMovimientos = resultado.TotalRegistros;
                ActualizarPaginacionMovimientos();
            }
            else
            {
                MessageBox.Show(resultado.Error ?? "Error al cargar historial de movimientos", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActualizarPaginacionMovimientos()
        {
            int totalPaginas = (int)Math.Ceiling((double)totalRegistrosMovimientos / REGISTROS_POR_PAGINA);
            int inicio = (paginaActualMovimientos - 1) * REGISTROS_POR_PAGINA + 1;
            int fin = Math.Min(paginaActualMovimientos * REGISTROS_POR_PAGINA, totalRegistrosMovimientos);

            txtInfoMovimientos.Text = $"Mostrando {inicio}-{fin} de {totalRegistrosMovimientos} registros";
            txtPaginaMovimientos.Text = $"Página {paginaActualMovimientos} de {Math.Max(1, totalPaginas)}";

            btnAnteriorMovimientos.IsEnabled = paginaActualMovimientos > 1;
            btnSiguienteMovimientos.IsEnabled = paginaActualMovimientos < totalPaginas;
        }

        private async void BtnAnteriorMovimientos_Click(object sender, RoutedEventArgs e)
        {
            if (paginaActualMovimientos > 1)
            {
                paginaActualMovimientos--;
                await CargarAuditoriasMovimientosAsync();
            }
        }

        private async void BtnSiguienteMovimientos_Click(object sender, RoutedEventArgs e)
        {
            int totalPaginas = (int)Math.Ceiling((double)totalRegistrosMovimientos / REGISTROS_POR_PAGINA);
            if (paginaActualMovimientos < totalPaginas)
            {
                paginaActualMovimientos++;
                await CargarAuditoriasMovimientosAsync();
            }
        }

        private async void BtnBuscarMovimientos_Click(object sender, RoutedEventArgs e)
        {
            paginaActualMovimientos = 1;
            await CargarAuditoriasMovimientosAsync();
        }

        private async void FiltrosMovimientos_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            paginaActualMovimientos = 1;
            await CargarAuditoriasMovimientosAsync();
        }

        private async void BtnDeshacer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int auditoriaId)
            {
                var auditoria = auditoriasMovimientos.FirstOrDefault(a => a.Id == auditoriaId);
                if (auditoria == null) return;

                var result = MessageBox.Show(
                    $"¿Está seguro de deshacer esta acción?\n\n" +
                    $"Casa: {auditoria.EntidadNombre}\n" +
                    $"Descripción: {auditoria.Descripcion}\n\n" +
                    $"Esta acción eliminará el movimiento de la base de datos.",
                    "⚠️ Confirmar Deshacer",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    if (string.IsNullOrEmpty(usuarioActual))
                    {
                        MessageBox.Show("No se pudo obtener el usuario actual", 
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var resultado = await SupabaseAuditoriaHelper.DeshacerMovimientoAsync(auditoriaId, usuarioActual);

                    if (resultado.Success)
                    {
                        MessageBox.Show("Movimiento deshecho exitosamente", 
                            "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        await CargarAuditoriasMovimientosAsync();
                    }
                    else
                    {
                        MessageBox.Show(resultado.Error ?? "Error al deshacer movimiento", 
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        #endregion

        #region Dueños

        private async Task CargarAuditoriasDuenosAsync()
        {
            var usuarioSeleccionado = (cmbUsuarioDuenos.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            var accionSeleccionada = (cmbAccionDuenos.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            var buscar = txtBuscarDuenos.Text.Trim();

            var resultado = await SupabaseAuditoriaHelper.ObtenerAuditoriasPorModuloAsync(
                modulo: "dueno",
                usuarioEmail: string.IsNullOrEmpty(usuarioSeleccionado) ? null : usuarioSeleccionado,
                tipoAccion: string.IsNullOrEmpty(accionSeleccionada) ? null : accionSeleccionada,
                buscar: string.IsNullOrEmpty(buscar) ? null : buscar,
                pagina: paginaActualDuenos,
                registrosPorPagina: REGISTROS_POR_PAGINA
            );

            if (resultado.Success && resultado.Data != null)
            {
                auditoriasDuenos = new ObservableCollection<Auditoria>(resultado.Data);
                dgDuenos.ItemsSource = auditoriasDuenos;
                totalRegistrosDuenos = resultado.TotalRegistros;
                ActualizarPaginacionDuenos();
            }
            else
            {
                MessageBox.Show(resultado.Error ?? "Error al cargar historial de dueños",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActualizarPaginacionDuenos()
        {
            int totalPaginas = (int)Math.Ceiling((double)totalRegistrosDuenos / REGISTROS_POR_PAGINA);
            int inicio = (paginaActualDuenos - 1) * REGISTROS_POR_PAGINA + 1;
            int fin = Math.Min(paginaActualDuenos * REGISTROS_POR_PAGINA, totalRegistrosDuenos);

            txtInfoDuenos.Text = totalRegistrosDuenos == 0
                ? "Sin registros"
                : $"Mostrando {inicio}-{fin} de {totalRegistrosDuenos} registros";
            txtPaginaDuenos.Text = $"Página {paginaActualDuenos} de {Math.Max(1, totalPaginas)}";

            btnAnteriorDuenos.IsEnabled = paginaActualDuenos > 1;
            btnSiguienteDuenos.IsEnabled = paginaActualDuenos < totalPaginas;
        }

        private async void BtnAnteriorDuenos_Click(object sender, RoutedEventArgs e)
        {
            if (paginaActualDuenos > 1) { paginaActualDuenos--; await CargarAuditoriasDuenosAsync(); }
        }

        private async void BtnSiguienteDuenos_Click(object sender, RoutedEventArgs e)
        {
            int totalPaginas = (int)Math.Ceiling((double)totalRegistrosDuenos / REGISTROS_POR_PAGINA);
            if (paginaActualDuenos < totalPaginas) { paginaActualDuenos++; await CargarAuditoriasDuenosAsync(); }
        }

        private async void BtnBuscarDuenos_Click(object sender, RoutedEventArgs e)
        {
            paginaActualDuenos = 1;
            await CargarAuditoriasDuenosAsync();
        }

        private async void FiltrosDuenos_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            paginaActualDuenos = 1;
            await CargarAuditoriasDuenosAsync();
        }

        #endregion

        #region Categorías

        private async Task CargarAuditoriasCatergoriasAsync()
        {
            var usuarioSeleccionado = (cmbUsuarioCategorias.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            var accionSeleccionada = (cmbAccionCategorias.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            var buscar = txtBuscarCategorias.Text.Trim();

            var resultado = await SupabaseAuditoriaHelper.ObtenerAuditoriasPorModuloAsync(
                modulo: "categoria",
                usuarioEmail: string.IsNullOrEmpty(usuarioSeleccionado) ? null : usuarioSeleccionado,
                tipoAccion: string.IsNullOrEmpty(accionSeleccionada) ? null : accionSeleccionada,
                buscar: string.IsNullOrEmpty(buscar) ? null : buscar,
                pagina: paginaActualCategorias,
                registrosPorPagina: REGISTROS_POR_PAGINA
            );

            if (resultado.Success && resultado.Data != null)
            {
                auditoriasCategorias = new ObservableCollection<Auditoria>(resultado.Data);
                dgCategorias.ItemsSource = auditoriasCategorias;
                totalRegistrosCategorias = resultado.TotalRegistros;
                ActualizarPaginacionCategorias();
            }
            else
            {
                MessageBox.Show(resultado.Error ?? "Error al cargar historial de categorías",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActualizarPaginacionCategorias()
        {
            int totalPaginas = (int)Math.Ceiling((double)totalRegistrosCategorias / REGISTROS_POR_PAGINA);
            int inicio = (paginaActualCategorias - 1) * REGISTROS_POR_PAGINA + 1;
            int fin = Math.Min(paginaActualCategorias * REGISTROS_POR_PAGINA, totalRegistrosCategorias);

            txtInfoCategorias.Text = totalRegistrosCategorias == 0
                ? "Sin registros"
                : $"Mostrando {inicio}-{fin} de {totalRegistrosCategorias} registros";
            txtPaginaCategorias.Text = $"Página {paginaActualCategorias} de {Math.Max(1, totalPaginas)}";

            btnAnteriorCategorias.IsEnabled = paginaActualCategorias > 1;
            btnSiguienteCategorias.IsEnabled = paginaActualCategorias < totalPaginas;
        }

        private async void BtnAnteriorCategorias_Click(object sender, RoutedEventArgs e)
        {
            if (paginaActualCategorias > 1) { paginaActualCategorias--; await CargarAuditoriasCatergoriasAsync(); }
        }

        private async void BtnSiguienteCategorias_Click(object sender, RoutedEventArgs e)
        {
            int totalPaginas = (int)Math.Ceiling((double)totalRegistrosCategorias / REGISTROS_POR_PAGINA);
            if (paginaActualCategorias < totalPaginas) { paginaActualCategorias++; await CargarAuditoriasCatergoriasAsync(); }
        }

        private async void BtnBuscarCategorias_Click(object sender, RoutedEventArgs e)
        {
            paginaActualCategorias = 1;
            await CargarAuditoriasCatergoriasAsync();
        }

        private async void FiltrosCategorias_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            paginaActualCategorias = 1;
            await CargarAuditoriasCatergoriasAsync();
        }

        #endregion

        #region Correos

        private async Task CargarLogsCorreosAsync()
        {
            var usuarioSeleccionado = (cmbUsuarioCorreos.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            var casaSeleccionada = (cmbCasaCorreos.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            var estadoSeleccionado = (cmbEstadoCorreos.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            var buscar = txtBuscarCorreos.Text.Trim();

            var resultado = await SupabaseLogCorreoHelper.ObtenerLogsAsync(
                usuarioEmail: string.IsNullOrEmpty(usuarioSeleccionado) ? null : usuarioSeleccionado,
                casaNombre: string.IsNullOrEmpty(casaSeleccionada) ? null : casaSeleccionada,
                estado: string.IsNullOrEmpty(estadoSeleccionado) ? null : estadoSeleccionado,
                buscar: string.IsNullOrEmpty(buscar) ? null : buscar,
                pagina: paginaActualCorreos,
                registrosPorPagina: REGISTROS_POR_PAGINA
            );

            if (resultado.Success && resultado.Data != null)
            {
                logsCorreos = new ObservableCollection<LogCorreo>(resultado.Data);
                dgCorreos.ItemsSource = logsCorreos;
                totalRegistrosCorreos = resultado.TotalRegistros;
                ActualizarPaginacionCorreos();
            }
            else
            {
                MessageBox.Show(resultado.Error ?? "Error al cargar historial de correos",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActualizarPaginacionCorreos()
        {
            int totalPaginas = (int)Math.Ceiling((double)totalRegistrosCorreos / REGISTROS_POR_PAGINA);
            int inicio = (paginaActualCorreos - 1) * REGISTROS_POR_PAGINA + 1;
            int fin = Math.Min(paginaActualCorreos * REGISTROS_POR_PAGINA, totalRegistrosCorreos);

            txtInfoCorreos.Text = totalRegistrosCorreos == 0
                ? "Sin registros"
                : $"Mostrando {inicio}-{fin} de {totalRegistrosCorreos} registros";
            txtPaginaCorreos.Text = $"Página {paginaActualCorreos} de {Math.Max(1, totalPaginas)}";

            btnAnteriorCorreos.IsEnabled = paginaActualCorreos > 1;
            btnSiguienteCorreos.IsEnabled = paginaActualCorreos < totalPaginas;
        }

        private async void BtnAnteriorCorreos_Click(object sender, RoutedEventArgs e)
        {
            if (paginaActualCorreos > 1) { paginaActualCorreos--; await CargarLogsCorreosAsync(); }
        }

        private async void BtnSiguienteCorreos_Click(object sender, RoutedEventArgs e)
        {
            int totalPaginas = (int)Math.Ceiling((double)totalRegistrosCorreos / REGISTROS_POR_PAGINA);
            if (paginaActualCorreos < totalPaginas) { paginaActualCorreos++; await CargarLogsCorreosAsync(); }
        }

        private async void BtnBuscarCorreos_Click(object sender, RoutedEventArgs e)
        {
            paginaActualCorreos = 1;
            await CargarLogsCorreosAsync();
        }

        private async void FiltrosCorreos_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            paginaActualCorreos = 1;
            await CargarLogsCorreosAsync();
        }

        private void BtnAbrirPdf_Click(object sender, RoutedEventArgs e)
        {
            var url = (sender as Button)?.Tag?.ToString();
            if (!string.IsNullOrEmpty(url))
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        private void BtnAbrirExcel_Click(object sender, RoutedEventArgs e)
        {
            var url = (sender as Button)?.Tag?.ToString();
            if (!string.IsNullOrEmpty(url))
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        #endregion

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
