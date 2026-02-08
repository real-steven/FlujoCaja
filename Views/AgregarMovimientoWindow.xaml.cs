using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FlujoCajaWpf.Views
{
    public partial class AgregarMovimientoWindow : Window
    {
        private int _casaId;
        private int _hojaMensualId;
        private string _casaNombre;
        private Movimiento? _movimiento;
        private bool _esEdicion = false;

        public AgregarMovimientoWindow(int casaId, int hojaMensualId, string casaNombre, Movimiento? movimiento = null)
        {
            InitializeComponent();
            _casaId = casaId;
            _hojaMensualId = hojaMensualId;
            _casaNombre = casaNombre;
            _movimiento = movimiento;
            _esEdicion = movimiento != null;

            dpFecha.SelectedDate = DateTime.Now;

            Loaded += async (s, e) => await CargarCategoriasAsync();

            if (_esEdicion && _movimiento != null)
            {
                txtTitulo.Text = "Editar Movimiento";
                CargarDatosMovimiento();
            }
        }

        private async Task CargarCategoriasAsync()
        {
            // No cargar categorías inicialmente, esperar a que el usuario seleccione un tipo
            // Si es edición, se cargarán en CargarDatosMovimiento
        }

        private void CmbTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTipo.SelectedIndex == -1) return;

            var tipoSeleccionado = ((ComboBoxItem)cmbTipo.SelectedItem).Content.ToString();
            // El contenido tiene emojis: "💰 Ingreso" o "💸 Egreso"
            var tipo = tipoSeleccionado.Contains("Ingreso") ? "ingreso" : "egreso";

            // Cargar categorías filtradas por tipo
            _ = Task.Run(async () =>
            {
                var categorias = await SupabaseCategoriaMovimientoHelper.ObtenerCategoriasPorTipoAsync(tipo);
                
                Dispatcher.Invoke(() =>
                {
                    cmbCategoria.ItemsSource = categorias;
                    cmbCategoria.SelectedIndex = categorias.Any() ? 0 : -1;
                });
            });
        }

        private void CargarDatosMovimiento()
        {
            if (_movimiento == null) return;

            cmbTipo.SelectedIndex = _movimiento.Tipo == "Ingreso" ? 0 : 1;
            txtMonto.Text = Math.Abs(_movimiento.Monto).ToString();
            dpFecha.SelectedDate = _movimiento.Fecha;
            txtDescripcion.Text = _movimiento.Descripcion ?? string.Empty;

            // Seleccionar categoría después de que se filtren
            Dispatcher.InvokeAsync(async () =>
            {
                await Task.Delay(100); // Dar tiempo para que se actualice el ComboBox
                var categorias = cmbCategoria.ItemsSource as List<CategoriaMovimientoSupabase>;
                if (categorias != null)
                {
                    var categoria = categorias.FirstOrDefault(c => c.Nombre == _movimiento.CategoriaNombre);
                    if (categoria != null)
                    {
                        cmbCategoria.SelectedItem = categoria;
                    }
                }
            });
        }

        private void TxtMonto_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Solo permitir números y punto decimal
            Regex regex = new Regex(@"[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones
            if (cmbTipo.SelectedIndex == -1)
            {
                CustomMessageBox.Show(
                    "Debes seleccionar un tipo de movimiento",
                    "Validación",
                    CustomMessageBox.MessageBoxType.Warning,
                    CustomMessageBox.MessageBoxButtons.OK
                );
                return;
            }

            // Validar que hay categoría (ya sea seleccionada o escrita)
            string? categoriaNombre = null;
            
            if (cmbCategoria.SelectedItem is CategoriaMovimientoSupabase categoriaExistente)
            {
                categoriaNombre = categoriaExistente.Nombre;
            }
            else if (!string.IsNullOrWhiteSpace(cmbCategoria.Text))
            {
                categoriaNombre = cmbCategoria.Text.Trim();
            }

            if (string.IsNullOrWhiteSpace(categoriaNombre))
            {
                CustomMessageBox.Show(
                    "Debes seleccionar o escribir una categoría",
                    "Validación",
                    CustomMessageBox.MessageBoxType.Warning,
                    CustomMessageBox.MessageBoxButtons.OK
                );
                return;
            }

            if (!decimal.TryParse(txtMonto.Text, out decimal monto) || monto <= 0)
            {
                CustomMessageBox.Show(
                    "El monto debe ser un número mayor a cero",
                    "Validación",
                    CustomMessageBox.MessageBoxType.Warning,
                    CustomMessageBox.MessageBoxButtons.OK
                );
                return;
            }

            if (dpFecha.SelectedDate == null)
            {
                CustomMessageBox.Show(
                    "Debes seleccionar una fecha",
                    "Validación",
                    CustomMessageBox.MessageBoxType.Warning,
                    CustomMessageBox.MessageBoxButtons.OK
                );
                return;
            }

            var tipo = cmbTipo.SelectedIndex == 0 ? "Ingreso" : "Gasto";

            // Si el usuario escribió una categoría nueva y marcó el checkbox, guardarla en BD
            if (chkGuardarCategoria.IsChecked == true && 
                cmbCategoria.SelectedItem == null && 
                !string.IsNullOrWhiteSpace(cmbCategoria.Text))
            {
                var nuevaCategoria = new CategoriaMovimientoSupabase
                {
                    Nombre = categoriaNombre!,
                    Tipo = tipo == "Ingreso" ? "ingreso" : "egreso",
                    Descripcion = $"Categoría creada automáticamente desde movimiento",
                    Activo = true
                };

                var resultado = await SupabaseCategoriaMovimientoHelper.InsertarCategoriaMovimientoAsync(nuevaCategoria);
                
                if (!resultado.Success)
                {
                    CustomMessageBox.Show(
                        $"No se pudo guardar la categoría: {resultado.Error}",
                        "Advertencia",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK
                    );
                    // Continuar de todas formas con el movimiento
                }
            }
            
            // El monto es positivo para Ingresos, negativo para Gastos
            decimal montoFinal = tipo == "Ingreso" ? monto : -monto;

            var movimientoSupabase = new MovimientoSupabase
            {
                CasaId = _casaId,
                HojaMensualId = _hojaMensualId,
                Categoria = categoriaNombre!,
                TipoMovimiento = tipo,
                Monto = montoFinal,
                Fecha = dpFecha.SelectedDate.Value,
                Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text) ? "Sin descripción" : txtDescripcion.Text.Trim(),
                Activo = true
            };

            if (_esEdicion && _movimiento != null)
            {
                movimientoSupabase.Id = _movimiento.Id;
                var resultado = await SupabaseMovimientoHelper.ActualizarMovimientoAsync(movimientoSupabase);
                
                if (resultado.Success)
                {
                    // 📊 REGISTRAR EDICIÓN EN HISTORIAL
                    var user = SupabaseAuthHelper.GetCurrentUser();
                    await SupabaseAuditoriaHelper.RegistrarAccionAsync(
                        user?.Email ?? "desconocido",
                        "movimiento",
                        "editar",
                        _movimiento.Id,
                        _casaNombre,
                        $"Editó {tipo}: {monto:C} - {categoriaNombre} en {_casaNombre}",
                        datosAnteriores: new {
                            tipo = _movimiento.Tipo,
                            monto = _movimiento.Monto,
                            categoria = _movimiento.CategoriaNombre,
                            descripcion = _movimiento.Descripcion
                        },
                        datosNuevos: new {
                            tipo = tipo,
                            monto = monto,
                            categoria = categoriaNombre,
                            descripcion = movimientoSupabase.Descripcion
                        }
                    );
                    
                    CustomMessageBox.Show(
                        "Movimiento actualizado correctamente",
                        "Éxito",
                        CustomMessageBox.MessageBoxType.Success,
                        CustomMessageBox.MessageBoxButtons.OK
                    );
                    DialogResult = true;
                    Close();
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
            }
            else
            {
                var resultado = await SupabaseMovimientoHelper.InsertarMovimientoAsync(movimientoSupabase);
                
                if (resultado.Success)
                {
                    // 📊 REGISTRAR EN HISTORIAL
                    var user = SupabaseAuthHelper.GetCurrentUser();
                    await SupabaseAuditoriaHelper.RegistrarAccionAsync(
                        user?.Email ?? "desconocido",
                        "movimiento",
                        "crear",
                        null, // El ID se asignará automáticamente en Supabase
                        _casaNombre,
                        $"{tipo}: {monto:C} - {categoriaNombre} en {_casaNombre}",
                        datosNuevos: new {
                            casa = _casaNombre,
                            tipo = tipo,
                            monto = monto,
                            categoria = categoriaNombre,
                            descripcion = movimientoSupabase.Descripcion
                        }
                    );
                    
                    CustomMessageBox.Show(
                        "Movimiento creado correctamente",
                        "Éxito",
                        CustomMessageBox.MessageBoxType.Success,
                        CustomMessageBox.MessageBoxButtons.OK
                    );
                    DialogResult = true;
                    Close();
                }
                else
                {
                    CustomMessageBox.Show(
                        $"Error al crear: {resultado.Error}",
                        "Error",
                        CustomMessageBox.MessageBoxType.Error,
                        CustomMessageBox.MessageBoxButtons.OK
                    );
                }
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
