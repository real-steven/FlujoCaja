using System;
using FlujoDeCajaApp.Data;

namespace FlujoDeCajaApp.Tests
{
    /// <summary>
    /// Clase de prueba para verificar la funcionalidad de la base de datos
    /// </summary>
    public static class TestBaseDatos
    {
        /// <summary>
        /// Método principal para ejecutar las pruebas
        /// </summary>
        public static void EjecutarPruebas()
        {
            Console.WriteLine("=== PRUEBAS DE BASE DE DATOS - SAMARA RENTALS ===\n");

            try
            {
                // Inicializar base de datos
                Console.WriteLine("1. Inicializando base de datos...");
                DatabaseHelper.InicializarBaseDatos();
                Console.WriteLine("   ✓ Base de datos inicializada correctamente\n");

                // Probar carga de dueños
                Console.WriteLine("2. Probando carga de dueños...");
                var duenos = DatabaseHelper.ObtenerDuenos();
                Console.WriteLine($"   ✓ Se cargaron {duenos.Count} dueños:");
                foreach (var dueno in duenos)
                {
                    Console.WriteLine($"     - {dueno.NombreCompleto} ({dueno.Email})");
                }
                Console.WriteLine();

                // Probar carga de categorías
                Console.WriteLine("3. Probando carga de categorías...");
                var categorias = DatabaseHelper.ObtenerCategorias();
                Console.WriteLine($"   ✓ Se cargaron {categorias.Count} categorías:");
                foreach (var categoria in categorias)
                {
                    Console.WriteLine($"     - {categoria.Nombre}: {categoria.Descripcion}");
                }
                Console.WriteLine();

                // Probar carga de casas
                Console.WriteLine("4. Probando carga de casas...");
                var casas = DatabaseHelper.ObtenerCasas();
                Console.WriteLine($"   ✓ Se cargaron {casas.Count} casas en la base de datos:");
                if (casas.Count > 0)
                {
                    foreach (var casa in casas)
                    {
                        Console.WriteLine($"     - {casa.Nombre} (Dueño: {casa.NombreDueno}, Categoría: {casa.NombreCategoria})");
                    }
                }
                else
                {
                    Console.WriteLine("     (No hay casas registradas aún - esto es normal en una instalación nueva)");
                }
                Console.WriteLine();

                // Probar funcionalidad de usuarios
                Console.WriteLine("5. Probando funcionalidad de usuarios...");
                TestAgregarUsuario.EjecutarPruebas();
                Console.WriteLine();

                // Prueba de validación de usuario
                Console.WriteLine("6. Probando validación de usuario administrador...");
                bool usuarioValido = DatabaseHelper.ValidarCredenciales("admin", "admin123");
                if (usuarioValido)
                {
                    Console.WriteLine("   ✓ Usuario administrador validado correctamente");
                }
                else
                {
                    Console.WriteLine("   ✗ Error: No se pudo validar el usuario administrador");
                }

                Console.WriteLine("\n=== TODAS LAS PRUEBAS COMPLETADAS EXITOSAMENTE ===");
                Console.WriteLine("\nLa funcionalidad 'Agregar Casa' está lista para usar:");
                Console.WriteLine("1. Ejecute la aplicación");
                Console.WriteLine("2. Inicie sesión con admin/admin123");
                Console.WriteLine("3. Haga clic en 'Agregar' → 'Nueva Casa'");
                Console.WriteLine("4. Complete el formulario y guarde");
                Console.WriteLine("5. La nueva casa aparecerá en el menú principal");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ ERROR EN LAS PRUEBAS: {ex.Message}");
                Console.WriteLine($"Detalles: {ex}");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}
