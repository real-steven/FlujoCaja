using System.Data.SQLite;
using System.IO;

namespace FlujoDeCajaApp.Data
{
    /// <summary>
    /// Clase helper para manejo de la base de datos SQLite
    /// Contiene métodos para inicialización y operaciones básicas
    /// </summary>
    public static class DatabaseHelper
    {
        // Ruta del archivo de base de datos
        private static readonly string rutaBaseDatos = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FlujoDeCajaApp",
            "cuenta_propiedades.db"
        );

        // Cadena de conexión a la base de datos
        private static readonly string cadenaConexion = $"Data Source={rutaBaseDatos};Version=3;";

        /// <summary>
        /// Inicializa la base de datos creando el archivo y las tablas necesarias
        /// </summary>
        public static void InicializarBaseDatos()
        {
            try
            {
                Console.WriteLine($"Inicializando base de datos en: {rutaBaseDatos}");
                
                // Crear directorio si no existe
                string? directorio = Path.GetDirectoryName(rutaBaseDatos);
                if (!string.IsNullOrEmpty(directorio) && !Directory.Exists(directorio))
                {
                    Console.WriteLine($"Creando directorio: {directorio}");
                    Directory.CreateDirectory(directorio);
                }

                // Crear base de datos y tabla de usuarios
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    
                    // Crear tabla de usuarios si no existe
                    string sqlCrearTablaUsuarios = @"
                        CREATE TABLE IF NOT EXISTS Usuarios (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Usuario TEXT UNIQUE NOT NULL,
                            Contrasena TEXT NOT NULL,
                            Correo TEXT,
                            Rol TEXT NOT NULL,
                            FechaCreacion TEXT DEFAULT CURRENT_TIMESTAMP,
                            Activo INTEGER DEFAULT 1
                        )";
                    
                    using (var comando = new SQLiteCommand(sqlCrearTablaUsuarios, conexion))
                    {
                        comando.ExecuteNonQuery();
                    }

                    // Crear tabla de dueños si no existe
                    string sqlCrearTablaDuenos = @"
                        CREATE TABLE IF NOT EXISTS Duenos (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Nombre TEXT NOT NULL,
                            Apellido TEXT NOT NULL,
                            Identificacion TEXT,
                            Telefono TEXT,
                            Email TEXT,
                            Direccion TEXT,
                            FechaCreacion TEXT DEFAULT CURRENT_TIMESTAMP,
                            Activo INTEGER DEFAULT 1
                        )";
                    
                    using (var comando = new SQLiteCommand(sqlCrearTablaDuenos, conexion))
                    {
                        comando.ExecuteNonQuery();
                    }

                    // Agregar la columna Identificacion si no existe (para compatibilidad con BD existentes)
                    try
                    {
                        string sqlAgregarColumna = "ALTER TABLE Duenos ADD COLUMN Identificacion TEXT";
                        using (var comando = new SQLiteCommand(sqlAgregarColumna, conexion))
                        {
                            comando.ExecuteNonQuery();
                        }
                    }
                    catch
                    {
                        // La columna ya existe, continuar
                    }

                    // Crear tabla de categorías si no existe
                    string sqlCrearTablaCategorias = @"
                        CREATE TABLE IF NOT EXISTS Categorias (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Nombre TEXT UNIQUE NOT NULL,
                            Descripcion TEXT,
                            FechaCreacion TEXT DEFAULT CURRENT_TIMESTAMP,
                            Activo INTEGER DEFAULT 1
                        )";
                    
                    using (var comando = new SQLiteCommand(sqlCrearTablaCategorias, conexion))
                    {
                        comando.ExecuteNonQuery();
                    }

                    // Crear tabla de casas si no existe
                    string sqlCrearTablaCasas = @"
                        CREATE TABLE IF NOT EXISTS Casas (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Nombre TEXT NOT NULL,
                            DuenoId INTEGER NOT NULL,
                            CategoriaId INTEGER NOT NULL,
                            RutaImagen TEXT,
                            FechaCreacion TEXT DEFAULT CURRENT_TIMESTAMP,
                            Activo INTEGER DEFAULT 1,
                            FOREIGN KEY (DuenoId) REFERENCES Duenos(Id),
                            FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id)
                        )";
                    
                    using (var comando = new SQLiteCommand(sqlCrearTablaCasas, conexion))
                    {
                        comando.ExecuteNonQuery();
                    }

                    // Crear tabla de movimientos si no existe
                    string sqlCrearTablaMovimientos = @"
                        CREATE TABLE IF NOT EXISTS Movimientos (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            CasaId INTEGER NOT NULL,
                            Fecha TEXT NOT NULL,
                            Descripcion TEXT NOT NULL,
                            Monto DECIMAL(10,2) NOT NULL,
                            Categoria TEXT NOT NULL,
                            FechaCreacion TEXT DEFAULT CURRENT_TIMESTAMP,
                            Activo INTEGER DEFAULT 1,
                            FOREIGN KEY (CasaId) REFERENCES Casas(Id)
                        )";
                    
                    using (var comando = new SQLiteCommand(sqlCrearTablaMovimientos, conexion))
                    {
                        comando.ExecuteNonQuery();
                    }

                    // Agregar la columna Correo en Usuarios si no existe (para compatibilidad con BD existentes)
                    try
                    {
                        string sqlAgregarColumnaCorreo = "ALTER TABLE Usuarios ADD COLUMN Correo TEXT";
                        using (var comando = new SQLiteCommand(sqlAgregarColumnaCorreo, conexion))
                        {
                            comando.ExecuteNonQuery();
                        }
                    }
                    catch
                    {
                        // La columna ya existe, continuar
                    }

                    // Insertar usuario administrador por defecto si no existe
                    InsertarUsuarioAdminPorDefecto(conexion);
                    
                    // Insertar datos de ejemplo para dueños y categorías
                    InsertarDatosEjemplo(conexion);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al inicializar la base de datos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Inserta el usuario administrador por defecto si no existe
        /// </summary>
        private static void InsertarUsuarioAdminPorDefecto(SQLiteConnection conexion)
        {
            try
            {
                // Verificar si ya existe el usuario admin
                string sqlVerificar = "SELECT COUNT(*) FROM Usuarios WHERE Usuario = @usuario";
                using (var comandoVerificar = new SQLiteCommand(sqlVerificar, conexion))
                {
                    comandoVerificar.Parameters.AddWithValue("@usuario", "admin");
                    long cantidad = (long)comandoVerificar.ExecuteScalar();

                    // Si no existe, insertarlo
                    if (cantidad == 0)
                    {
                        string sqlInsertar = @"
                            INSERT INTO Usuarios (Usuario, Contrasena, Rol) 
                            VALUES (@usuario, @contrasena, @rol)";
                        
                        using (var comandoInsertar = new SQLiteCommand(sqlInsertar, conexion))
                        {
                            comandoInsertar.Parameters.AddWithValue("@usuario", "admin");
                            comandoInsertar.Parameters.AddWithValue("@contrasena", "admin");
                            comandoInsertar.Parameters.AddWithValue("@rol", "Administrador");
                            comandoInsertar.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al insertar usuario administrador: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Valida las credenciales de un usuario
        /// </summary>
        /// <param name="usuario">Nombre de usuario</param>
        /// <param name="contrasena">Contraseña</param>
        /// <returns>True si las credenciales son válidas, False en caso contrario</returns>
        public static bool ValidarCredenciales(string usuario, string contrasena)
        {
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    
                    string sql = @"
                        SELECT COUNT(*) FROM Usuarios 
                        WHERE Usuario = @usuario AND Contrasena = @contrasena AND Activo = 1";
                    
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@usuario", usuario);
                        comando.Parameters.AddWithValue("@contrasena", contrasena);
                        
                        long cantidad = (long)comando.ExecuteScalar();
                        return cantidad > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al validar credenciales: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene la información completa de un usuario por su nombre de usuario
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario</param>
        /// <returns>Información del usuario o null si no existe</returns>
        public static Dictionary<string, object>? ObtenerUsuario(string nombreUsuario)
        {
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    
                    string sql = @"
                        SELECT Id, Usuario, Rol, FechaCreacion, Activo 
                        FROM Usuarios 
                        WHERE Usuario = @usuario AND Activo = 1";
                    
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@usuario", nombreUsuario);
                        
                        using (var lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                return new Dictionary<string, object>
                                {
                                    ["Id"] = lector["Id"],
                                    ["Usuario"] = lector["Usuario"],
                                    ["Rol"] = lector["Rol"],
                                    ["FechaCreacion"] = lector["FechaCreacion"],
                                    ["Activo"] = lector["Activo"]
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener usuario: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene la cadena de conexión para uso en otros módulos
        /// </summary>
        /// <returns>Cadena de conexión SQLite</returns>
        public static string ObtenerCadenaConexion()
        {
            return cadenaConexion;
        }

        /// <summary>
        /// Inserta datos de ejemplo para dueños y categorías
        /// </summary>
        private static void InsertarDatosEjemplo(SQLiteConnection conexion)
        {
            try
            {
                // Insertar dueños de ejemplo si no existen
                string sqlVerificarDuenos = "SELECT COUNT(*) FROM Duenos";
                using (var comandoVerificar = new SQLiteCommand(sqlVerificarDuenos, conexion))
                {
                    long cantidadDuenos = (long)comandoVerificar.ExecuteScalar();
                    
                    if (cantidadDuenos == 0)
                    {
                        string[] duenosEjemplo = {
                            "('Juan', 'Pérez', '+506 8888-1111', 'juan.perez@email.com', 'San José, Costa Rica')",
                            "('María', 'González', '+506 8888-2222', 'maria.gonzalez@email.com', 'Cartago, Costa Rica')",
                            "('Carlos', 'Rodríguez', '+506 8888-3333', 'carlos.rodriguez@email.com', 'Alajuela, Costa Rica')",
                            "('Ana', 'Jiménez', '+506 8888-4444', 'ana.jimenez@email.com', 'Heredia, Costa Rica')",
                            "('Luis', 'Morales', '+506 8888-5555', 'luis.morales@email.com', 'Puntarenas, Costa Rica')"
                        };
                        
                        foreach (string dueno in duenosEjemplo)
                        {
                            string sqlInsertar = $"INSERT INTO Duenos (Nombre, Apellido, Telefono, Email, Direccion) VALUES {dueno}";
                            using (var comando = new SQLiteCommand(sqlInsertar, conexion))
                            {
                                comando.ExecuteNonQuery();
                            }
                        }
                    }
                }

                // Insertar categorías de ejemplo si no existen
                string sqlVerificarCategorias = "SELECT COUNT(*) FROM Categorias";
                using (var comandoVerificar = new SQLiteCommand(sqlVerificarCategorias, conexion))
                {
                    long cantidadCategorias = (long)comandoVerificar.ExecuteScalar();
                    
                    if (cantidadCategorias == 0)
                    {
                        string[] categoriasEjemplo = {
                            "('Casa de Playa', 'Propiedades ubicadas cerca del mar')",
                            "('Casa de Montaña', 'Propiedades en zonas montañosas')",
                            "('Casa Urbana', 'Propiedades en zonas urbanas y centros de ciudad')",
                            "('Casa Rural', 'Propiedades en zonas rurales y campestres')",
                            "('Villa de Lujo', 'Propiedades de alta gama con amenidades exclusivas')",
                            "('Cabaña', 'Estructuras rústicas y acogedoras')",
                            "('Apartamento', 'Unidades en edificios o condominios')"
                        };
                        
                        foreach (string categoria in categoriasEjemplo)
                        {
                            string sqlInsertar = $"INSERT INTO Categorias (Nombre, Descripcion) VALUES {categoria}";
                            using (var comando = new SQLiteCommand(sqlInsertar, conexion))
                            {
                                comando.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al insertar datos de ejemplo: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los dueños activos de la base de datos
        /// </summary>
        /// <returns>Lista de dueños con Id, Nombre completo</returns>
        public static List<(int Id, string NombreCompleto, string Telefono, string Email)> ObtenerDuenos()
        {
            var duenos = new List<(int Id, string NombreCompleto, string Telefono, string Email)>();
            
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = @"
                        SELECT Id, Nombre, Apellido, Telefono, Email 
                        FROM Duenos 
                        WHERE Activo = 1 
                        ORDER BY Nombre, Apellido";
                    
                    using (var comando = new SQLiteCommand(sql, conexion))
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string nombre = reader.GetString(1);
                            string apellido = reader.GetString(2);
                            string telefono = reader.IsDBNull(3) ? "" : reader.GetString(3);
                            string email = reader.IsDBNull(4) ? "" : reader.GetString(4);
                            
                            duenos.Add((id, $"{nombre} {apellido}", telefono, email));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener dueños: {ex.Message}", ex);
            }
            
            return duenos;
        }

        /// <summary>
        /// Obtiene todas las categorías activas de la base de datos
        /// </summary>
        /// <returns>Lista de categorías con Id, Nombre y Descripción</returns>
        public static List<(int Id, string Nombre, string Descripcion)> ObtenerCategorias()
        {
            var categorias = new List<(int Id, string Nombre, string Descripcion)>();
            
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = @"
                        SELECT Id, Nombre, Descripcion 
                        FROM Categorias 
                        WHERE Activo = 1 
                        ORDER BY Nombre";
                    
                    using (var comando = new SQLiteCommand(sql, conexion))
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string nombre = reader.GetString(1);
                            string descripcion = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            
                            categorias.Add((id, nombre, descripcion));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener categorías: {ex.Message}", ex);
            }
            
            return categorias;
        }

        /// <summary>
        /// Guarda una nueva casa en la base de datos
        /// </summary>
        /// <param name="nombre">Nombre de la casa</param>
        /// <param name="duenoId">ID del dueño</param>
        /// <param name="categoriaId">ID de la categoría</param>
        /// <param name="rutaImagen">Ruta relativa de la imagen</param>
        /// <returns>ID de la casa creada</returns>
        public static int GuardarCasa(string nombre, int duenoId, int categoriaId, string rutaImagen)
        {
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = @"
                        INSERT INTO Casas (Nombre, DuenoId, CategoriaId, RutaImagen, FechaCreacion) 
                        VALUES (@nombre, @duenoId, @categoriaId, @rutaImagen, @fechaCreacion);
                        SELECT last_insert_rowid();";
                    
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@nombre", nombre);
                        comando.Parameters.AddWithValue("@duenoId", duenoId);
                        comando.Parameters.AddWithValue("@categoriaId", categoriaId);
                        comando.Parameters.AddWithValue("@rutaImagen", rutaImagen);
                        comando.Parameters.AddWithValue("@fechaCreacion", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        
                        object resultado = comando.ExecuteScalar();
                        return Convert.ToInt32(resultado);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar casa: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene todas las casas con información de dueño y categoría
        /// </summary>
        /// <returns>Lista de casas con toda la información</returns>
        public static List<(int Id, string Nombre, string NombreDueno, string NombreCategoria, string RutaImagen, DateTime FechaCreacion)> ObtenerCasas()
        {
            var casas = new List<(int Id, string Nombre, string NombreDueno, string NombreCategoria, string RutaImagen, DateTime FechaCreacion)>();
            
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = @"
                        SELECT c.Id, c.Nombre, 
                               (d.Nombre || ' ' || d.Apellido) as NombreDueno,
                               cat.Nombre as NombreCategoria,
                               c.RutaImagen, c.FechaCreacion
                        FROM Casas c
                        INNER JOIN Duenos d ON c.DuenoId = d.Id
                        INNER JOIN Categorias cat ON c.CategoriaId = cat.Id
                        WHERE c.Activo = 1
                        ORDER BY c.FechaCreacion DESC";
                    
                    using (var comando = new SQLiteCommand(sql, conexion))
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string nombre = reader.GetString(1);
                            string nombreDueno = reader.GetString(2);
                            string nombreCategoria = reader.GetString(3);
                            string rutaImagen = reader.IsDBNull(4) ? "" : reader.GetString(4);
                            DateTime fechaCreacion = DateTime.Parse(reader.GetString(5));
                            
                            casas.Add((id, nombre, nombreDueno, nombreCategoria, rutaImagen, fechaCreacion));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener casas: {ex.Message}", ex);
            }
            
            return casas;
        }

        /// <summary>
        /// Obtiene todas las casas inactivas con información de dueño y categoría
        /// </summary>
        /// <returns>Lista de casas inactivas con toda la información</returns>
        public static List<(int Id, string Nombre, string NombreDueno, string NombreCategoria, string RutaImagen, DateTime FechaCreacion)> ObtenerCasasInactivas()
        {
            var casas = new List<(int Id, string Nombre, string NombreDueno, string NombreCategoria, string RutaImagen, DateTime FechaCreacion)>();
            
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = @"
                        SELECT c.Id, c.Nombre, 
                               (d.Nombre || ' ' || d.Apellido) as NombreDueno,
                               cat.Nombre as NombreCategoria,
                               c.RutaImagen, c.FechaCreacion
                        FROM Casas c
                        INNER JOIN Duenos d ON c.DuenoId = d.Id
                        INNER JOIN Categorias cat ON c.CategoriaId = cat.Id
                        WHERE c.Activo = 0
                        ORDER BY c.FechaCreacion DESC";
                    
                    using (var comando = new SQLiteCommand(sql, conexion))
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string nombre = reader.GetString(1);
                            string nombreDueno = reader.GetString(2);
                            string nombreCategoria = reader.GetString(3);
                            string rutaImagen = reader.IsDBNull(4) ? "" : reader.GetString(4);
                            DateTime fechaCreacion = DateTime.Parse(reader.GetString(5));
                            
                            casas.Add((id, nombre, nombreDueno, nombreCategoria, rutaImagen, fechaCreacion));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener casas inactivas: {ex.Message}", ex);
            }
            
            return casas;
        }

        /// <summary>
        /// Desactiva una casa específica cambiando su estado a inactivo
        /// </summary>
        /// <param name="casaId">ID de la casa a desactivar</param>
        /// <returns>True si se desactivó correctamente, False en caso contrario</returns>
        public static bool DesactivarCasa(int casaId)
        {
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = "UPDATE Casas SET Activo = 0 WHERE Id = @casaId";
                    
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@casaId", casaId);
                        int filasAfectadas = comando.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al desactivar casa: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Reactiva una casa específica cambiando su estado a activo
        /// </summary>
        /// <param name="casaId">ID de la casa a reactivar</param>
        /// <returns>True si se reactivó correctamente, False en caso contrario</returns>
        public static bool ReactivarCasa(int casaId)
        {
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = "UPDATE Casas SET Activo = 1 WHERE Id = @casaId";
                    
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@casaId", casaId);
                        int filasAfectadas = comando.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al reactivar casa: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los usuarios activos de la base de datos
        /// </summary>
        /// <returns>Lista de usuarios con Id, Nombre de usuario, Correo, Rol y Fecha de creación</returns>
        public static List<(int Id, string Usuario, string Correo, string Rol, DateTime FechaCreacion)> ObtenerUsuarios()
        {
            var usuarios = new List<(int Id, string Usuario, string Correo, string Rol, DateTime FechaCreacion)>();
            
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    
                    string sql = @"
                        SELECT Id, Usuario, COALESCE(Correo, '') as Correo, Rol, FechaCreacion 
                        FROM Usuarios 
                        WHERE Activo = 1 
                        ORDER BY FechaCreacion DESC";
                    
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        using (var reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = Convert.ToInt32(reader["Id"]);
                                string usuario = reader["Usuario"].ToString() ?? "";
                                string correo = reader["Correo"].ToString() ?? "";
                                string rol = reader["Rol"].ToString() ?? "";
                                DateTime fechaCreacion = DateTime.Parse(reader["FechaCreacion"].ToString() ?? DateTime.Now.ToString());
                                
                                usuarios.Add((id, usuario, correo, rol, fechaCreacion));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener usuarios: {ex.Message}", ex);
            }
            
            return usuarios;
        }

        /// <summary>
        /// Valida el formato de un correo electrónico
        /// </summary>
        /// <param name="correo">Correo a validar</param>
        /// <returns>True si el formato es válido, False en caso contrario</returns>
        public static bool ValidarFormatoCorreo(string correo)
        {
            try
            {
                var direccion = new System.Net.Mail.MailAddress(correo);
                return direccion.Address == correo;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Guarda una categoría en la base de datos
        /// </summary>
        /// <param name="nombre">Nombre de la categoría</param>
        /// <param name="descripcion">Descripción de la categoría</param>
        /// <returns>ID de la categoría creada o -1 si hay error</returns>
        public static int GuardarCategoria(string nombre, string descripcion)
        {
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = "INSERT INTO Categorias (Nombre, Descripcion) VALUES (@nombre, @descripcion)";
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@nombre", nombre);
                        comando.Parameters.AddWithValue("@descripcion", descripcion);
                        comando.ExecuteNonQuery();
                        return (int)conexion.LastInsertRowId;
                    }
                }
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Guarda un usuario en la base de datos
        /// </summary>
        /// <param name="usuario">Nombre de usuario</param>
        /// <param name="contrasena">Contraseña</param>
        /// <param name="correo">Correo del usuario</param>
        /// <param name="rol">Rol del usuario</param>
        /// <returns>ID del usuario creado o -1 si hay error</returns>
        public static int GuardarUsuario(string usuario, string contrasena, string correo, string rol)
        {
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = "INSERT INTO Usuarios (Usuario, Contrasena, Correo, Rol) VALUES (@usuario, @contrasena, @correo, @rol)";
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@usuario", usuario);
                        comando.Parameters.AddWithValue("@contrasena", contrasena);
                        comando.Parameters.AddWithValue("@correo", correo);
                        comando.Parameters.AddWithValue("@rol", rol);
                        comando.ExecuteNonQuery();
                        return (int)conexion.LastInsertRowId;
                    }
                }
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Guarda un dueño en la base de datos
        /// </summary>
        /// <param name="nombre">Nombre del dueño</param>
        /// <param name="apellido">Apellido del dueño</param>
        /// <param name="identificacion">Identificación del dueño</param>
        /// <param name="correo">Correo del dueño</param>
        /// <param name="telefono">Teléfono del dueño</param>
        /// <returns>ID del dueño creado o -1 si hay error</returns>
        public static int GuardarDueno(string nombre, string apellido, string identificacion, string correo, string telefono)
        {
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = "INSERT INTO Duenos (Nombre, Apellido, Telefono, Email, Identificacion) VALUES (@nombre, @apellido, @telefono, @email, @identificacion)";
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@nombre", nombre);
                        comando.Parameters.AddWithValue("@apellido", apellido);
                        comando.Parameters.AddWithValue("@telefono", telefono);
                        comando.Parameters.AddWithValue("@email", correo);
                        comando.Parameters.AddWithValue("@identificacion", identificacion);
                        comando.ExecuteNonQuery();
                        return (int)conexion.LastInsertRowId;
                    }
                }
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Verifica si existe una categoría con el nombre especificado
        /// </summary>
        /// <param name="nombre">Nombre de la categoría</param>
        /// <returns>True si existe, False en caso contrario</returns>
        public static bool ExisteCategoriaConNombre(string nombre)
        {
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = "SELECT COUNT(*) FROM Categorias WHERE Nombre = @nombre";
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@nombre", nombre);
                        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica si existe un usuario con el nombre especificado
        /// </summary>
        /// <param name="usuario">Nombre de usuario</param>
        /// <returns>True si existe, False en caso contrario</returns>
        public static bool ExisteUsuarioConNombre(string usuario)
        {
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = "SELECT COUNT(*) FROM Usuarios WHERE Usuario = @usuario";
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@usuario", usuario);
                        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica si existe un usuario con el correo especificado
        /// </summary>
        /// <param name="correo">Correo del usuario</param>
        /// <returns>True si existe, False en caso contrario</returns>
        public static bool ExisteUsuarioConCorreo(string correo)
        {
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = "SELECT COUNT(*) FROM Usuarios WHERE Correo = @correo";
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@correo", correo);
                        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica si existe un dueño con la identificación especificada
        /// </summary>
        /// <param name="identificacion">Identificación del dueño</param>
        /// <returns>True si existe, False en caso contrario</returns>
        public static bool ExisteDuenoConIdentificacion(string identificacion)
        {
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = "SELECT COUNT(*) FROM Duenos WHERE Identificacion = @identificacion";
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@identificacion", identificacion);
                        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica si existe un dueño con el email especificado
        /// </summary>
        /// <param name="email">Email del dueño</param>
        /// <returns>True si existe, False en caso contrario</returns>
        public static bool ExisteDuenoConEmail(string email)
        {
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = "SELECT COUNT(*) FROM Duenos WHERE Email = @email";
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@email", email);
                        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Guarda un movimiento en la base de datos
        /// </summary>
        /// <param name="casaId">ID de la casa</param>
        /// <param name="fecha">Fecha del movimiento</param>
        /// <param name="descripcion">Descripción del movimiento</param>
        /// <param name="monto">Monto del movimiento (positivo para ingresos, negativo para gastos)</param>
        /// <param name="categoria">Categoría del movimiento</param>
        /// <returns>ID del movimiento creado o -1 si hay error</returns>
        public static int GuardarMovimiento(int casaId, DateTime fecha, string descripcion, decimal monto, string categoria)
        {
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = "INSERT INTO Movimientos (CasaId, Fecha, Descripcion, Monto, Categoria) VALUES (@casaId, @fecha, @descripcion, @monto, @categoria)";
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@casaId", casaId);
                        comando.Parameters.AddWithValue("@fecha", fecha.ToString("yyyy-MM-dd"));
                        comando.Parameters.AddWithValue("@descripcion", descripcion);
                        comando.Parameters.AddWithValue("@monto", monto);
                        comando.Parameters.AddWithValue("@categoria", categoria);
                        comando.ExecuteNonQuery();
                        return (int)conexion.LastInsertRowId;
                    }
                }
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Obtiene los movimientos de una casa filtrados por mes y año
        /// </summary>
        /// <param name="casaId">ID de la casa</param>
        /// <param name="año">Año a filtrar</param>
        /// <param name="mes">Mes a filtrar (1-12)</param>
        /// <returns>Lista de movimientos</returns>
        public static List<(int Id, DateTime Fecha, string Descripcion, decimal Monto, string Categoria)> ObtenerMovimientosPorMes(int casaId, int año, int mes)
        {
            var movimientos = new List<(int Id, DateTime Fecha, string Descripcion, decimal Monto, string Categoria)>();
            
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = @"
                        SELECT Id, Fecha, Descripcion, Monto, Categoria 
                        FROM Movimientos 
                        WHERE CasaId = @casaId AND Activo = 1 
                        AND strftime('%Y', Fecha) = @año 
                        AND strftime('%m', Fecha) = @mes
                        ORDER BY Fecha DESC";
                    
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@casaId", casaId);
                        comando.Parameters.AddWithValue("@año", año.ToString());
                        comando.Parameters.AddWithValue("@mes", mes.ToString("00"));
                        
                        using (var reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                movimientos.Add((
                                    reader.GetInt32(0),
                                    DateTime.Parse(reader.GetString(1)),
                                    reader.GetString(2),
                                    reader.GetDecimal(3),
                                    reader.GetString(4)
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener movimientos: {ex.Message}", ex);
            }
            
            return movimientos;
        }

        /// <summary>
        /// Actualiza un movimiento existente
        /// </summary>
        /// <param name="id">ID del movimiento</param>
        /// <param name="fecha">Nueva fecha</param>
        /// <param name="descripcion">Nueva descripción</param>
        /// <param name="monto">Nuevo monto</param>
        /// <param name="categoria">Nueva categoría</param>
        /// <returns>True si se actualizó correctamente, False en caso contrario</returns>
        public static bool ActualizarMovimiento(int id, DateTime fecha, string descripcion, decimal monto, string categoria)
        {
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = "UPDATE Movimientos SET Fecha = @fecha, Descripcion = @descripcion, Monto = @monto, Categoria = @categoria WHERE Id = @id";
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@id", id);
                        comando.Parameters.AddWithValue("@fecha", fecha.ToString("yyyy-MM-dd"));
                        comando.Parameters.AddWithValue("@descripcion", descripcion);
                        comando.Parameters.AddWithValue("@monto", monto);
                        comando.Parameters.AddWithValue("@categoria", categoria);
                        return comando.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Elimina (inactiva) un movimiento
        /// </summary>
        /// <param name="id">ID del movimiento</param>
        /// <returns>True si se eliminó correctamente, False en caso contrario</returns>
        public static bool EliminarMovimiento(int id)
        {
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = "UPDATE Movimientos SET Activo = 0 WHERE Id = @id";
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@id", id);
                        return comando.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene el balance anterior (hasta el mes anterior al especificado)
        /// </summary>
        /// <param name="casaId">ID de la casa</param>
        /// <param name="año">Año actual</param>
        /// <param name="mes">Mes actual</param>
        /// <returns>Balance anterior</returns>
        public static decimal ObtenerBalanceAnterior(int casaId, int año, int mes)
        {
            try
            {
                using (var conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();
                    string sql = @"
                        SELECT COALESCE(SUM(Monto), 0) 
                        FROM Movimientos 
                        WHERE CasaId = @casaId AND Activo = 1 
                        AND (strftime('%Y', Fecha) < @año 
                             OR (strftime('%Y', Fecha) = @año AND strftime('%m', Fecha) < @mes))";
                    
                    using (var comando = new SQLiteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@casaId", casaId);
                        comando.Parameters.AddWithValue("@año", año.ToString());
                        comando.Parameters.AddWithValue("@mes", mes.ToString("00"));
                        
                        var result = comando.ExecuteScalar();
                        return Convert.ToDecimal(result);
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Obtiene las categorías más utilizadas para movimientos
        /// </summary>
        /// <returns>Lista de categorías</returns>
        public static List<string> ObtenerCategoriasMovimientos()
        {
            var categorias = new List<string>
            {
                "Alquiler",
                "Mantenimiento",
                "Servicios Públicos",
                "Reparaciones",
                "Limpieza",
                "Seguros",
                "Impuestos",
                "Comisiones",
                "Otros Ingresos",
                "Otros Gastos"
            };
            
            return categorias;
        }
    }
}
