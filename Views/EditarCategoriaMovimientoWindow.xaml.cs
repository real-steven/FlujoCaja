using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using FlujoCajaWpf.Views;
using System.Windows;
using System.Windows.Controls;

namespace FlujoCajaWpf.Views
{
    public partial class EditarCategoriaMovimientoWindow : Window
    {
        private readonly CategoriaMovimientoSupabase _categoria;

        public EditarCategoriaMovimientoWindow(CategoriaMovimientoSupabase categoria)
        {
            InitializeComponent();
            _categoria = categoria;
            CargarDatos();
        }

        private void CargarDatos()
        {
            txtNombreCategoria.Text = _categoria.Nombre;
            
            // Seleccionar el tipo
            foreach (ComboBoxItem item in cmbTipo.Items)
            {
                var contenido = item.Content.ToString();
                if ((contenido == "Ingreso" && _categoria.Tipo.Equals("Ingreso", StringComparison.OrdinalIgnoreCase)) ||
                    (contenido == "Gasto" && _categoria.Tipo.Equals("Gasto", StringComparison.OrdinalIgnoreCase)))
                {
                    cmbTipo.SelectedItem = item;
                    break;
                }
            }
        }

        private void Restaurar_Click(object sender, RoutedEventArgs e)
        {
            CargarDatos();
            
            CustomMessageBox.Show(
                "Datos restaurados a su estado original",
                "Restaurar",
                CustomMessageBox.MessageBoxType.Info,
                CustomMessageBox.MessageBoxButtons.OK);
        }

        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validar nombre
                if (string.IsNullOrWhiteSpace(txtNombreCategoria.Text))
                {
                    CustomMessageBox.Show(
                        "Por favor ingrese el nombre de la categoría.",
                        "Campo Requerido",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK
                    );
                    txtNombreCategoria.Focus();
                    return;
                }

                // Validar tipo
                if (cmbTipo.SelectedItem == null)
                {
                    CustomMessageBox.Show(
                        "Por favor seleccione el tipo de movimiento.",
                        "Campo Requerido",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK
                    );
                    cmbTipo.Focus();
                    return;
                }

                // Actualizar categoría
                _categoria.Nombre = txtNombreCategoria.Text.Trim();
                _categoria.Tipo = ((ComboBoxItem)cmbTipo.SelectedItem).Content.ToString() == "Ingreso" ? "ingreso" : "egreso";

                // Actualizar en Supabase
                var resultado = await SupabaseCategoriaMovimientoHelper.ActualizarCategoriaMovimientoAsync(_categoria);

                if (resultado.Success)
                {
                    CustomMessageBox.Show(
                        "La categoría de movimiento se ha actualizado exitosamente.",
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
                        $"Error al actualizar la categoría: {resultado.Error}",
                        "Error",
                        CustomMessageBox.MessageBoxType.Error,
                        CustomMessageBox.MessageBoxButtons.OK
                    );
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(
                    $"Error inesperado: {ex.Message}",
                    "Error",
                    CustomMessageBox.MessageBoxType.Error,
                    CustomMessageBox.MessageBoxButtons.OK
                );
            }
        }
    }
}
