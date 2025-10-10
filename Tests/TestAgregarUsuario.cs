using System;
using FlujoDeCajaApp.Data;

namespace FlujoDeCajaApp.Tests
{
    /// <summary>
    /// Pruebas básicas para verificar que el formulario de agregar usuario funcione correctamente
    /// </summary>
    public static class TestAgregarUsuario
    {
        /// <summary>
        /// Ejecuta todas las pruebas básicas
        /// </summary>
        public static void EjecutarPruebas()
        {
            Console.WriteLine("=== INICIANDO PRUEBAS DEL FORMULARIO AGREGAR USUARIO ===");
            
            try
            {
                // Inicializar base de datos
                DatabaseHelper.InicializarBaseDatos();
                Console.WriteLine("✓ Base de datos inicializada correctamente");
                
                // Prueba 1: Validar formato de correo
                TestValidarFormatoCorreo();
                
                // Prueba 2: Verificar duplicados
                TestVerificarDuplicados();
                
                // Prueba 3: Guardar usuario
                TestGuardarUsuario();
                
                Console.WriteLine("\n=== TODAS LAS PRUEBAS COMPLETADAS EXITOSAMENTE ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error durante las pruebas: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Prueba la validación de formato de correo
        /// </summary>
        private static void TestValidarFormatoCorreo()
        {
            Console.WriteLine("\n--- Prueba: Validar Formato de Correo ---");
            
            // Casos válidos
            string[] correosValidos = { "test@gmail.com", "usuario.test@empresa.co.cr", "admin@samara.com" };
            foreach (string correo in correosValidos)
            {
                bool esValido = DatabaseHelper.ValidarFormatoCorreo(correo);
                Console.WriteLine($"✓ {correo}: {(esValido ? "VÁLIDO" : "INVÁLIDO")}");
                if (!esValido) throw new Exception($"Correo válido marcado como inválido: {correo}");
            }
            
            // Casos inválidos
            string[] correosInvalidos = { "correo-invalido", "@gmail.com", "test@", "test.com" };
            foreach (string correo in correosInvalidos)
            {
                bool esValido = DatabaseHelper.ValidarFormatoCorreo(correo);
                Console.WriteLine($"✓ {correo}: {(esValido ? "VÁLIDO" : "INVÁLIDO")}");
                if (esValido) throw new Exception($"Correo inválido marcado como válido: {correo}");
            }
        }
        
        /// <summary>
        /// Prueba la verificación de duplicados
        /// </summary>
        private static void TestVerificarDuplicados()
        {
            Console.WriteLine("\n--- Prueba: Verificar Duplicados ---");
            
            // Crear usuario de prueba
            string usuarioPrueba = "test_user_" + DateTime.Now.Ticks;
            string correoPrueba = $"test_{DateTime.Now.Ticks}@test.com";
            
            // Verificar que no existe
            bool existeAntes = DatabaseHelper.ExisteUsuarioConNombre(usuarioPrueba);
            Console.WriteLine($"✓ Usuario antes de crear: {(existeAntes ? "EXISTE" : "NO EXISTE")}");
            if (existeAntes) throw new Exception("Usuario de prueba ya existe antes de crearlo");
            
            // Crear usuario
            int id = DatabaseHelper.GuardarUsuario(usuarioPrueba, "password123", correoPrueba, "Usuario");
            Console.WriteLine($"✓ Usuario creado con ID: {id}");
            if (id <= 0) throw new Exception("No se pudo crear el usuario de prueba");
            
            // Verificar que ahora existe
            bool existeDespues = DatabaseHelper.ExisteUsuarioConNombre(usuarioPrueba);
            Console.WriteLine($"✓ Usuario después de crear: {(existeDespues ? "EXISTE" : "NO EXISTE")}");
            if (!existeDespues) throw new Exception("Usuario no se encuentra después de crearlo");
            
            // Verificar duplicado de correo
            bool existeCorreo = DatabaseHelper.ExisteUsuarioConCorreo(correoPrueba);
            Console.WriteLine($"✓ Correo después de crear: {(existeCorreo ? "EXISTE" : "NO EXISTE")}");
            if (!existeCorreo) throw new Exception("Correo no se encuentra después de crear usuario");
        }
        
        /// <summary>
        /// Prueba guardar un usuario completo
        /// </summary>
        private static void TestGuardarUsuario()
        {
            Console.WriteLine("\n--- Prueba: Guardar Usuario ---");
            
            string usuario = "usuario_completo_" + DateTime.Now.Ticks;
            string contrasena = "contraseña_segura_123";
            string correo = $"completo_{DateTime.Now.Ticks}@samara.com";
            string rol = "Usuario";
            
            int id = DatabaseHelper.GuardarUsuario(usuario, contrasena, correo, rol);
            Console.WriteLine($"✓ Usuario guardado con ID: {id}");
            
            if (id <= 0)
            {
                throw new Exception("No se pudo guardar el usuario");
            }
            
            // Verificar que se guardó correctamente
            bool existe = DatabaseHelper.ExisteUsuarioConNombre(usuario);
            bool existeCorreo = DatabaseHelper.ExisteUsuarioConCorreo(correo);
            
            Console.WriteLine($"✓ Usuario existe: {existe}");
            Console.WriteLine($"✓ Correo existe: {existeCorreo}");
            
            if (!existe || !existeCorreo)
            {
                throw new Exception("Usuario no se guardó correctamente");
            }
        }
    }
}
