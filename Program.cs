using FlujoDeCajaApp.Formularios;
using System.Diagnostics;

namespace FlujoDeCajaApp;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        try
        {
            // Configurar manejo global de excepciones
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            
            Console.WriteLine("Iniciando aplicación...");
            Application.Run(new FormLogin());
        }
        catch (Exception ex)
        {
            LogError("Error fatal en Main", ex);
            MessageBox.Show($"Error fatal al iniciar la aplicación:\n{ex.Message}\n\nConsulte los logs para más detalles.", 
                "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        LogError("Excepción en Thread de UI", e.Exception);
        MessageBox.Show($"Error en la aplicación:\n{e.Exception.Message}", 
            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            LogError("Excepción no manejada en AppDomain", ex);
        }
    }

    private static void LogError(string context, Exception ex)
    {
        string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {context}\n" +
                           $"Message: {ex.Message}\n" +
                           $"StackTrace: {ex.StackTrace}\n";
        
        if (ex.InnerException != null)
        {
            logMessage += $"InnerException: {ex.InnerException.Message}\n";
        }
        
        logMessage += new string('-', 80) + "\n";

        Console.WriteLine(logMessage);
        Debug.WriteLine(logMessage);
        
        // También escribir a un archivo de log
        try
        {
            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "FlujoDeCajaApp", "error.log");
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
            File.AppendAllText(logPath, logMessage);
        }
        catch
        {
            // Si no se puede escribir el log, continuar
        }
    }
}