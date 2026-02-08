using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using FlujoCajaWpf.Views;
using System.Windows;

namespace FlujoCajaWpf.Views
{
    public partial class AgregarCategoriaWindow : Window
    {
        public AgregarCategoriaWindow()
        {
            InitializeComponent();
        }

        private void Limpiar_Click(object sender, RoutedEventArgs e)
        {
            txtNombreCategoria.Clear();
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

                // Crear nueva categoría
                var nuevaCategoria = new CategoriaSupabase
                {
                    Nombre = txtNombreCategoria.Text.Trim()
                };

                // Insertar en Supabase
                var resultado = await SupabaseCategoriaHelper.InsertarCategoriaAsync(nuevaCategoria);

                if (resultado.Success)
                {
                    CustomMessageBox.Show(
                        "La categoría se ha creado exitosamente.",
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
