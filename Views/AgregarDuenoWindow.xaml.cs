using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FlujoCajaWpf.Views
{
    public partial class AgregarDuenoWindow : Window
    {
        private ObservableCollection<string> emailsSecundarios = new ObservableCollection<string>();

        public AgregarDuenoWindow()
        {
            InitializeComponent();
            lstEmailsSecundarios.ItemsSource = emailsSecundarios;
        }

        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    CustomMessageBox.Show(
                        "El nombre del dueño es requerido",
                        "Validación",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK);
                    txtNombre.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtApellido.Text))
                {
                    CustomMessageBox.Show(
                        "El apellido del dueño es requerido",
                        "Validación",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK);
                    txtApellido.Focus();
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
                if (!EsEmailValido(txtEmail.Text.Trim()))
                {
                    CustomMessageBox.Show(
                        "El formato del email no es válido",
                        "Validación",
                        CustomMessageBox.MessageBoxType.Warning,
                        CustomMessageBox.MessageBoxButtons.OK);
                    txtEmail.Focus();
                    return;
                }

                // Crear el objeto DuenoSupabase
                var nuevoDueno = new DuenoSupabase
                {
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    Cedula = txtCedula.Text.Trim(),
                    TipoCedula = ((ComboBoxItem)cmbTipoCedula.SelectedItem).Content.ToString(),
                    Telefono = txtTelefono.Text.Trim(),
                    Email = txtEmail.Text.Trim()
                };

                // Llamar al helper para insertar
                var resultado = await SupabaseDuenoHelper.InsertarDuenoAsync(nuevoDueno);

                if (resultado.Success)
                {
                    CustomMessageBox.Show(
                        "El dueño ha sido registrado exitosamente",
                        "Éxito",
                        CustomMessageBox.MessageBoxType.Success,
                        CustomMessageBox.MessageBoxButtons.OK);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    CustomMessageBox.Show(
                        $"Error al registrar el dueño: {resultado.Error}",
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

        private void Limpiar_Click(object sender, RoutedEventArgs e)
        {
            // Limpiar todos los campos
            txtNombre.Clear();
            txtApellido.Clear();
            txtCedula.Clear();
            cmbTipoCedula.SelectedIndex = 0;
            txtTelefono.Clear();
            txtEmail.Clear();
            emailsSecundarios.Clear();
            
            // Enfocar el primer campo
            txtNombre.Focus();
        }

        private bool EsEmailValido(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private void AgregarEmailSecundario_Click(object sender, RoutedEventArgs e)
        {
            var inputWindow = new Window
            {
                Title = "Agregar Email Secundario",
                Width = 450,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3F4F6"))
            };

            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Header con gradiente
            var headerBorder = new Border
            {
                Padding = new Thickness(20, 15, 20, 15)
            };
            
            var headerGradient = new LinearGradientBrush();
            headerGradient.StartPoint = new Point(0, 0);
            headerGradient.EndPoint = new Point(1, 1);
            headerGradient.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#0F172A"), 0));
            headerGradient.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#1E3A8A"), 0.5));
            headerGradient.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#3B82F6"), 1));
            headerBorder.Background = headerGradient;

            var headerStack = new StackPanel();
            var titleText = new TextBlock
            {
                Text = "✉️ Email Secundario",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.White
            };
            var subtitleText = new TextBlock
            {
                Text = "Agregar correo electrónico adicional",
                FontSize = 11,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FCD34D")),
                Margin = new Thickness(0, 3, 0, 0)
            };
            headerStack.Children.Add(titleText);
            headerStack.Children.Add(subtitleText);
            headerBorder.Child = headerStack;
            Grid.SetRow(headerBorder, 0);
            mainGrid.Children.Add(headerBorder);

            // Content
            var contentStack = new StackPanel { Margin = new Thickness(20, 15, 20, 15) };
            
            var lblEmail = new TextBlock
            {
                Text = "Email:",
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151")),
                Margin = new Thickness(0, 0, 0, 6)
            };
            
            var txtEmailSecundario = new TextBox
            {
                FontSize = 12,
                Padding = new Thickness(10, 8, 10, 8),
                Margin = new Thickness(0, 0, 0, 15),
                Background = System.Windows.Media.Brushes.White,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D5DB")),
                BorderThickness = new Thickness(1, 1, 1, 1)
            };

            var btnPanel = new Grid();
            btnPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            btnPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var btnCancelar = new Button
            {
                Content = "Cancelar",
                Height = 38,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626")),
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0, 0, 0, 0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(0, 0, 8, 0),
                FontSize = 13,
                FontWeight = FontWeights.SemiBold
            };
            
            var btnAgregar = new Button
            {
                Content = "✓ Agregar",
                Height = 38,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")),
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0, 0, 0, 0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(8, 0, 0, 0),
                FontSize = 13,
                FontWeight = FontWeights.SemiBold
            };

            btnAgregar.Click += (s, args) =>
            {
                var email = txtEmailSecundario.Text.Trim();
                if (string.IsNullOrWhiteSpace(email))
                {
                    CustomMessageBox.Show("Ingrese un email", "Validación", CustomMessageBox.MessageBoxType.Warning);
                    return;
                }

                if (!EsEmailValido(email))
                {
                    CustomMessageBox.Show("Email inválido", "Validación", CustomMessageBox.MessageBoxType.Warning);
                    return;
                }

                if (emailsSecundarios.Contains(email))
                {
                    CustomMessageBox.Show("Este email ya fue agregado", "Validación", CustomMessageBox.MessageBoxType.Warning);
                    return;
                }

                emailsSecundarios.Add(email);
                inputWindow.Close();
            };

            btnCancelar.Click += (s, args) => inputWindow.Close();

            Grid.SetColumn(btnCancelar, 0);
            Grid.SetColumn(btnAgregar, 1);
            btnPanel.Children.Add(btnCancelar);
            btnPanel.Children.Add(btnAgregar);

            contentStack.Children.Add(lblEmail);
            contentStack.Children.Add(txtEmailSecundario);
            contentStack.Children.Add(btnPanel);
            
            Grid.SetRow(contentStack, 1);
            mainGrid.Children.Add(contentStack);
            
            inputWindow.Content = mainGrid;
            inputWindow.ShowDialog();
        }

        private void EliminarEmailSecundario_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is string email)
            {
                emailsSecundarios.Remove(email);
            }
        }
    }
}
