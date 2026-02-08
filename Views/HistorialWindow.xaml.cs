using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace FlujoCajaWpf.Views
{
    public partial class HistorialWindow : Window
    {
        private ObservableCollection<Auditoria> auditoriasCasas = new();
        private ObservableCollection<Auditoria> auditoriasMovimientos = new();
        
        private int paginaActualCasas = 1;
        private int paginaActualMovimientos = 1;
        private int totalRegistrosCasas = 0;
        private int totalRegistrosMovimientos = 0;
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
        }

        #endregion

        #region Casas

        private async Task CargarAuditoriasCasasAsync()
        {
            var usuarioSeleccionado = (cmbUsuarioCasas.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            var accionSeleccionada = (cmbAccionCasas.SelectedItem as ComboBoxItem)?.Tag?.ToString();

            var resultado = await SupabaseAuditoriaHelper.ObtenerAuditoriasPorModuloAsync(
                modulo: "casa",
                usuarioEmail: string.IsNullOrEmpty(usuarioSeleccionado) ? null : usuarioSeleccionado,
                tipoAccion: string.IsNullOrEmpty(accionSeleccionada) ? null : accionSeleccionada,
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

            var resultado = await SupabaseAuditoriaHelper.ObtenerAuditoriasPorModuloAsync(
                modulo: "movimiento",
                usuarioEmail: string.IsNullOrEmpty(usuarioSeleccionado) ? null : usuarioSeleccionado,
                tipoAccion: string.IsNullOrEmpty(accionSeleccionada) ? null : accionSeleccionada,
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

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
