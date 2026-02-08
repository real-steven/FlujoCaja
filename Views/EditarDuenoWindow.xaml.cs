using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using System.Windows;
using System.Windows.Controls;

namespace FlujoCajaWpf.Views
{
    public partial class EditarDuenoWindow : Window
    {
        private DuenoSupabase dueno;

        public EditarDuenoWindow(DuenoSupabase dueno)
        {
            InitializeComponent();
            this.dueno = dueno;
            CargarDatos();
        }

        private void CargarDatos()
        {
            // Construir nombre completo
            txtNombreCompleto.Text = dueno.NombreCompleto;
            txtCedula.Text = dueno.Cedula ?? string.Empty;
            txtTelefono.Text = dueno.Telefono ?? string.Empty;

            // Si el email contiene múltiples emails separados por ";", mostrar solo el primero
            if (!string.IsNullOrWhiteSpace(dueno.Email))
            {
                string[] emails = dueno.Email.Split(';');
                txtEmail.Text = emails[0].Trim();
            }
            else
            {
                txtEmail.Text = string.Empty;
            }

            // Seleccionar tipo de cédula
            string tipoCedula = dueno.TipoCedula ?? "Nacional";
            foreach (ComboBoxItem item in cmbTipoCedula.Items)
            {
                if (item.Content.ToString() == tipoCedula)
                {
                    cmbTipoCedula.SelectedItem = item;
                    break;
                }
            }
            
            // Si no se encontró, seleccionar el primero (Nacional)
            if (cmbTipoCedula.SelectedItem == null)
            {
                cmbTipoCedula.SelectedIndex = 0;
            }
        }

        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(txtNombreCompleto.Text))
                {
                    CustomMessageBox.Show(
                        "El nombre completo del dueño es requerido",
                        "Validación",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK);
                    txtNombreCompleto.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtCedula.Text))
                {
                    CustomMessageBox.Show(
                        "La cédula de identidad es requerida",
                        "Validación",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK);
                    txtCedula.Focus();
                    return;
                }

                if (cmbTipoCedula.SelectedItem == null)
                {
                    CustomMessageBox.Show(
                        "Debe seleccionar el tipo de cédula",
                        "Validación",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK);
                    cmbTipoCedula.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    CustomMessageBox.Show(
                        "El email principal es requerido",
                        "Validación",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK);
                    txtEmail.Focus();
                    return;
                }

                // Validar formato de email
                var emailPrincipal = txtEmail.Text.Trim();
                if (!System.Text.RegularExpressions.Regex.IsMatch(emailPrincipal, 
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    CustomMessageBox.Show(
                        "El formato del email principal no es válido. Ejemplo: usuario@ejemplo.com",
                        "Validación",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK);
                    txtEmail.Focus();
                    return;
                }

                // Validar teléfono si no está vacío
                if (!string.IsNullOrWhiteSpace(txtTelefono.Text))
                {
                    var telefono = txtTelefono.Text.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(telefono, @"^[\d\s\-\+\(\)]+$"))
                    {
                        CustomMessageBox.Show(
                            "El formato del teléfono no es válido. Solo números, espacios y símbolos (+, -, ( ))",
                            "Validación",
                            CustomMessageBox.MessageBoxType.Warning,
                            CustomMessageBox.MessageBoxButtons.OK);
                        txtTelefono.Focus();
                        return;
                    }
                }

                // Separar nombre completo en nombre y apellido
                var nombreCompleto = txtNombreCompleto.Text.Trim();
                var partes = nombreCompleto.Split(' ', 2);
                string nombre = partes[0];
                string apellido = partes.Length > 1 ? partes[1] : string.Empty;

                // Actualizar el objeto dueño
                dueno.Nombre = nombre;
                dueno.Apellido = apellido;
                dueno.Cedula = txtCedula.Text.Trim();
                dueno.TipoCedula = (cmbTipoCedula.SelectedItem as ComboBoxItem)?.Content.ToString();
                dueno.Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim();
                dueno.Email = emailPrincipal;

                // Llamar al helper para actualizar
                var resultado = await SupabaseDuenoHelper.ActualizarDuenoAsync(dueno);

                if (resultado.Success)
                {
                    CustomMessageBox.Show(
                        "Dueño actualizado exitosamente",
                        "Éxito",
                        CustomMessageBox.MessageBoxType.Success,
                        CustomMessageBox.MessageBoxButtons.OK);

                    DialogResult = true;
                    Close();
                }
                else
                {
                    CustomMessageBox.Show(
                        $"Error al actualizar dueño: {resultado.Error}",
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

        private void Restaurar_Click(object sender, RoutedEventArgs e)
        {
            // Recargar los datos originales sin cerrar la ventana
            CargarDatos();
            
            CustomMessageBox.Show(
                "Datos restaurados a su estado original",
                "Restaurar",
                CustomMessageBox.MessageBoxType.Info,
                CustomMessageBox.MessageBoxButtons.OK);
        }
    }
}
