using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using FlujoCajaWpf.Views;
using System.Windows;

namespace FlujoCajaWpf.Views
{
    public partial class EditarCategoriaWindow : Window
    {
        private readonly CategoriaSupabase _categoria;

        public EditarCategoriaWindow(CategoriaSupabase categoria)
        {
            InitializeComponent();
            _categoria = categoria;
            CargarDatos();
        }

        private void CargarDatos()
        {
            txtNombreCategoria.Text = _categoria.Nombre;
        }

        private void Restaurar_Click(object sender, RoutedEventArgs e)
        {
            // Recargar el dato original
            txtNombreCategoria.Text = _categoria.Nombre;
            
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

                // Actualizar categoría
                _categoria.Nombre = txtNombreCategoria.Text.Trim();

                // Actualizar en Supabase
                var resultado = await SupabaseCategoriaHelper.ActualizarCategoriaAsync(_categoria);

                if (resultado.Success)
                {
                    CustomMessageBox.Show(
                        "La categoría se ha actualizado exitosamente.",
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
