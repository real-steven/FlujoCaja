using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using FlujoCajaWpf.Views;
using System.Windows;
using System.Windows.Controls;

namespace FlujoCajaWpf.Views
{
    public partial class AgregarCategoriaMovimientoWindow : Window
    {
        public AgregarCategoriaMovimientoWindow()
        {
            InitializeComponent();
        }

        private void Limpiar_Click(object sender, RoutedEventArgs e)
        {
            txtNombreCategoria.Clear();
            cmbTipo.SelectedIndex = -1;
            txtNombreCategoria.Focus();
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

                // Crear nueva categoría
                var nuevaCategoria = new CategoriaMovimientoSupabase
                {
                    Id = 0, // Importante: 0 para que la DB genere el ID
                    Nombre = txtNombreCategoria.Text.Trim(),
                    Tipo = ((ComboBoxItem)cmbTipo.SelectedItem).Content.ToString() == "Ingreso" ? "ingreso" : "egreso"
                };

                // Insertar en Supabase
                var resultado = await SupabaseCategoriaMovimientoHelper.InsertarCategoriaMovimientoAsync(nuevaCategoria);

                if (resultado.Success)
                {
                    CustomMessageBox.Show(
                        "La categoría de movimiento se ha creado exitosamente.",
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
                        $"Error al crear la categoría: {resultado.Error}",
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
