using System.Windows;
using FlujoCajaWpf.Data;

namespace FlujoCajaWpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Configurar manejo global de excepciones
        DispatcherUnhandledException += (s, args) =>
        {
            MessageBox.Show($"Error inesperado: {args.Exception.Message}", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };

        // Copiar appsettings.json al directorio de salida
        CopiarAppSettings();

        // Inicializar Supabase
        try
        {
            var inicializado = await SupabaseHelper.InicializarSupabase();
            
            if (!inicializado)
            {
                MessageBox.Show(
                    "Error al conectar con Supabase.\n\n" +
                    "Por favor verifica:\n" +
                    "1. Que appsettings.json existe en la carpeta de la aplicación\n" +
                    "2. Que las credenciales de Supabase son correctas\n" +
                    "3. Que tienes conexión a internet",
                    "Error de Conexión",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                
                Shutdown();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al inicializar: {ex.Message}", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }

    private void CopiarAppSettings()
    {
        try
        {
            var sourceFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            
            if (!System.IO.File.Exists(sourceFile))
            {
                Console.WriteLine("Advertencia: appsettings.json no encontrado en directorio base");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al copiar appsettings.json: {ex.Message}");
        }
    }
}

