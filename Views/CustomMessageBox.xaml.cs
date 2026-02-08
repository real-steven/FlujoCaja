using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace FlujoCajaWpf.Views
{
    public partial class CustomMessageBox : Window
    {
        public enum MessageBoxType
        {
            Info,
            Success,
            Warning,
            Error,
            Question
        }

        public enum MessageBoxButtons
        {
            OK,
            OKCancel,
            YesNo
        }

        public bool? DialogResultValue { get; private set; }

        public CustomMessageBox(string mensaje, string titulo, MessageBoxType tipo, MessageBoxButtons botones)
        {
            InitializeComponent();
            
            txtMensaje.Text = mensaje;
            txtTitulo.Text = titulo;
            
            ConfigurarTipo(tipo);
            ConfigurarBotones(botones);
        }

        private void ConfigurarTipo(MessageBoxType tipo)
        {
            LinearGradientBrush gradiente;
            
            switch (tipo)
            {
                case MessageBoxType.Success:
                    txtIcono.Text = "✅";
                    gradiente = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1)
                    };
                    gradiente.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#059669"), 0));
                    gradiente.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#10B981"), 0.5));
                    gradiente.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#34D399"), 1));
                    borderHeader.Background = gradiente;
                    break;
                    
                case MessageBoxType.Warning:
                    txtIcono.Text = "⚠️";
                    gradiente = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1)
                    };
                    gradiente.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#D97706"), 0));
                    gradiente.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#F59E0B"), 0.5));
                    gradiente.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FBBF24"), 1));
                    borderHeader.Background = gradiente;
                    break;
                    
                case MessageBoxType.Error:
                    txtIcono.Text = "❌";
                    gradiente = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1)
                    };
                    gradiente.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#B91C1C"), 0));
                    gradiente.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#DC2626"), 0.5));
                    gradiente.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#EF4444"), 1));
                    borderHeader.Background = gradiente;
                    break;
                    
                case MessageBoxType.Question:
                    txtIcono.Text = "❓";
                    gradiente = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1)
                    };
                    gradiente.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#6D28D9"), 0));
                    gradiente.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#8B5CF6"), 0.5));
                    gradiente.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#A78BFA"), 1));
                    borderHeader.Background = gradiente;
                    break;
                    
                default: // Info
                    txtIcono.Text = "ℹ️";
                    gradiente = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1)
                    };
                    gradiente.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#0F172A"), 0));
                    gradiente.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#1E3A8A"), 0.5));
                    gradiente.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#3B82F6"), 1));
                    borderHeader.Background = gradiente;
                    break;
            }
        }

        private void ConfigurarBotones(MessageBoxButtons botones)
        {
            stackBotones.Children.Clear();

            switch (botones)
            {
                case MessageBoxButtons.OK:
                    stackBotones.Children.Add(CrearBoton("Aceptar", true, "#3B82F6"));
                    break;
                    
                case MessageBoxButtons.OKCancel:
                    stackBotones.Children.Add(CrearBoton("Cancelar", false, "#6B7280"));
                    stackBotones.Children.Add(CrearBoton("Aceptar", true, "#3B82F6"));
                    break;
                    
                case MessageBoxButtons.YesNo:
                    stackBotones.Children.Add(CrearBoton("No", false, "#EF4444"));
                    stackBotones.Children.Add(CrearBoton("Sí", true, "#10B981"));
                    break;
            }
        }

        private Button CrearBoton(string texto, bool isPositive, string colorHex)
        {
            var boton = new Button
            {
                Content = texto,
                MinWidth = 110,
                Height = 42,
                Margin = new Thickness(10, 0, 0, 0),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = Brushes.White,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex)),
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            var style = new Style(typeof(Button));
            var template = new ControlTemplate(typeof(Button));
            
            var borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(10));
            borderFactory.SetValue(Border.PaddingProperty, new Thickness(20, 0, 20, 0));
            
            // Sombra suave
            var dropShadow = new DropShadowEffect
            {
                Color = Colors.Black,
                Direction = 270,
                ShadowDepth = 2,
                BlurRadius = 8,
                Opacity = 0.15
            };
            borderFactory.SetValue(Border.EffectProperty, dropShadow);
            
            var contentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            
            borderFactory.AppendChild(contentFactory);
            template.VisualTree = borderFactory;
            
            style.Setters.Add(new Setter(Button.TemplateProperty, template));
            
            // Hover effect - oscurecer
            var trigger = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
            var hoverColor = AdjustarBrillo(colorHex, -0.15f);
            trigger.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(hoverColor)));
            style.Triggers.Add(trigger);
            
            boton.Style = style;
            
            boton.Click += (s, e) =>
            {
                DialogResultValue = isPositive;
                Close();
            };

            return boton;
        }

        private Color AdjustarBrillo(string hexColor, float ajuste)
        {
            var color = (Color)ColorConverter.ConvertFromString(hexColor);
            var r = (byte)Math.Max(0, Math.Min(255, color.R + (255 * ajuste)));
            var g = (byte)Math.Max(0, Math.Min(255, color.G + (255 * ajuste)));
            var b = (byte)Math.Max(0, Math.Min(255, color.B + (255 * ajuste)));
            return Color.FromRgb(r, g, b);
        }

        public static bool? Show(string mensaje, string titulo = "Información", 
            MessageBoxType tipo = MessageBoxType.Info, 
            MessageBoxButtons botones = MessageBoxButtons.OK)
        {
            var dialog = new CustomMessageBox(mensaje, titulo, tipo, botones);
            dialog.Owner = Application.Current.MainWindow;
            dialog.ShowDialog();
            return dialog.DialogResultValue;
        }
    }
}
