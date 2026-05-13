using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FlujoCajaWpf.Views
{
    public partial class EditarCasaWindow : Window
    {
        private CasaSupabase casa;
        private CasaSupabase casaOriginal; // Para comparar cambios
        private List<DuenoSupabase> duenos = new List<DuenoSupabase>();
        private List<CategoriaSupabase> categorias = new List<CategoriaSupabase>();

        public EditarCasaWindow(CasaSupabase casa)
        {
            InitializeComponent();
            this.casa = casa;
            // Guardar copia de los datos originales
            this.casaOriginal = new CasaSupabase
            {
                Id = casa.Id,
                Nombre = casa.Nombre,
                DuenoId = casa.DuenoId,
                CategoriaId = casa.CategoriaId,
                Moneda = casa.Moneda,
                Activo = casa.Activo,
                Notas = casa.Notas,
                EmailPrincipal = casa.EmailPrincipal
            };
            Loaded += async (s, e) => await CargarDatosAsync();
        }

        private async Task CargarDatosAsync()
        {
            try
            {
                // Cargar dueños
                var resultadoDuenos = await SupabaseDuenoHelper.ObtenerDuenosAsync();
                if (resultadoDuenos.Success)
                {
                    duenos = resultadoDuenos.Data ?? new List<DuenoSupabase>();
                    cmbDueno.ItemsSource = duenos;
                    
                    // Seleccionar el dueño actual
                    var duenoActual = duenos.FirstOrDefault(d => d.Id == casa.DuenoId);
                    if (duenoActual != null)
                    {
                        cmbDueno.SelectedItem = duenoActual;
                    }
                }

                // Cargar categorías
                var resultadoCategorias = await SupabaseCategoriaHelper.ObtenerCategoriasAsync();
                if (resultadoCategorias.Success)
                {
                    categorias = resultadoCategorias.Data ?? new List<CategoriaSupabase>();
                    cmbCategoria.ItemsSource = categorias;
                    
                    // Seleccionar la categoría actual
                    var categoriaActual = categorias.FirstOrDefault(c => c.Id == casa.CategoriaId);
                    if (categoriaActual != null)
                    {
                        cmbCategoria.SelectedItem = categoriaActual;
                    }
                }

                // Cargar datos de la casa
                txtNombre.Text = casa.Nombre;
                chkActiva.IsChecked = casa.Activo;
                txtEmailPrincipal.Text = casa.EmailPrincipal ?? string.Empty;
                txtNotas.Text = casa.Notas ?? string.Empty;

                // Seleccionar moneda
                foreach (ComboBoxItem item in cmbMoneda.Items)
                {
                    if (item.Content.ToString() == casa.Moneda)
                    {
                        cmbMoneda.SelectedItem = item;
                        break;
                    }
                }
                
                // Si no se encuentra la moneda, seleccionar USD
                if (cmbMoneda.SelectedItem == null)
                {
                    cmbMoneda.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(
                    $"Error al cargar datos: {ex.Message}",
                    "Error",
                    CustomMessageBox.MessageBoxType.Error,
                    CustomMessageBox.MessageBoxButtons.OK);
            }
        }

        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    CustomMessageBox.Show(
                        "El nombre de la casa es requerido",
                        "Validación",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK);
                    txtNombre.Focus();
                    return;
                }

                if (cmbDueno.SelectedItem == null)
                {
                    CustomMessageBox.Show(
                        "Debe seleccionar un dueño",
                        "Validación",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK);
                    cmbDueno.Focus();
                    return;
                }

                if (cmbCategoria.SelectedItem == null)
                {
                    CustomMessageBox.Show(
                        "Debe seleccionar una categoría de propiedad",
                        "Validación",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK);
                    cmbCategoria.Focus();
                    return;
                }

                if (cmbMoneda.SelectedItem == null)
                {
                    CustomMessageBox.Show(
                        "Debe seleccionar una moneda",
                        "Validación",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK);
                    cmbMoneda.Focus();
                    return;
                }

                // Actualizar datos de la casa
                casa.Nombre = txtNombre.Text.Trim();
                casa.DuenoId = (cmbDueno.SelectedItem as DuenoSupabase)!.Id;
                casa.CategoriaId = (cmbCategoria.SelectedItem as CategoriaSupabase)!.Id;
                casa.Moneda = (cmbMoneda.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "USD";
                casa.Activo = chkActiva.IsChecked ?? true;
                casa.EmailPrincipal = string.IsNullOrWhiteSpace(txtEmailPrincipal.Text) ? null : txtEmailPrincipal.Text.Trim();
                casa.Notas = string.IsNullOrWhiteSpace(txtNotas.Text) ? null : txtNotas.Text.Trim();

                // Llamar al helper para actualizar
                var resultado = await SupabaseCasaHelper.ActualizarCasaAsync(casa);

                if (resultado.Success)
                {
                    // 📊 REGISTRAR EN HISTORIAL
                    var cambios = new List<string>();
                    if (casaOriginal.Nombre != casa.Nombre)
                        cambios.Add($"Nombre: '{casaOriginal.Nombre}' → '{casa.Nombre}'");
                    if (casaOriginal.DuenoId != casa.DuenoId)
                        cambios.Add("Cambió dueño");
                    if (casaOriginal.CategoriaId != casa.CategoriaId)
                        cambios.Add("Cambió categoría");
                    if (casaOriginal.Moneda != casa.Moneda)
                        cambios.Add($"Moneda: {casaOriginal.Moneda} → {casa.Moneda}");
                    if (casaOriginal.Activo != casa.Activo)
                        cambios.Add($"Estado: {(casaOriginal.Activo ? "Activa" : "Inactiva")} → {(casa.Activo ? "Activa" : "Inactiva")}");
                    
                    var user = SupabaseAuthHelper.GetCurrentUser();
                    await SupabaseAuditoriaHelper.RegistrarAccionAsync(
                        user?.Email ?? "desconocido",
                        "casa",
                        "editar",
                        casa.Id,
                        casa.Nombre,
                        $"Editó casa {casa.Nombre}: {string.Join(", ", cambios)}",
                        datosAnteriores: new {
                            nombre = casaOriginal.Nombre,
                            duenoId = casaOriginal.DuenoId,
                            categoriaId = casaOriginal.CategoriaId,
                            moneda = casaOriginal.Moneda,
                            activo = casaOriginal.Activo
                        },
                        datosNuevos: new {
                            nombre = casa.Nombre,
                            duenoId = casa.DuenoId,
                            categoriaId = casa.CategoriaId,
                            moneda = casa.Moneda,
                            activo = casa.Activo
                        }
                    );
                    
                    CustomMessageBox.Show(
                        "Casa actualizada exitosamente",
                        "Éxito",
                        CustomMessageBox.MessageBoxType.Success,
                        CustomMessageBox.MessageBoxButtons.OK);

                    DialogResult = true;
                    Close();
                }
                else
                {
                    CustomMessageBox.Show(
                        $"Error al actualizar casa: {resultado.Error}",
                        "Error",
                        CustomMessageBox.MessageBoxType.Error,
                        CustomMessageBox.MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(
                    $"Error inesperado: {ex.Message}",
                    "Error",
                    CustomMessageBox.MessageBoxType.Error,
                    CustomMessageBox.MessageBoxButtons.OK);
            }
        }

        private async void Restaurar_Click(object sender, RoutedEventArgs e)
        {
            // Recargar los datos originales sin cerrar la ventana
            await CargarDatosAsync();
            
            CustomMessageBox.Show(
                "Datos restaurados a su estado original",
                "Restaurar",
                CustomMessageBox.MessageBoxType.Info,
                CustomMessageBox.MessageBoxButtons.OK);
        }
    }
}
