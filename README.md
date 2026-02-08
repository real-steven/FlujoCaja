# 🏠 Sistema de Flujo de Caja - WPF

Sistema de gestión de flujo de caja para propiedades de alquiler con integración a Supabase y Azure AI.

## 📋 Sprint 1 - Login y Menú Principal (COMPLETADO)

### ✅ Funcionalidades Implementadas

- ✅ Autenticación con Supabase Auth
- ✅ Pantalla de login moderna con gradientes
- ✅ Menú principal con tarjetas de propiedades
- ✅ Búsqueda en tiempo real de casas
- ✅ Visualización de balance actual por casa
- ✅ Arquitectura MVVM completa
- ✅ Estilos modernos con Material Design

---

## 🚀 Requisitos Previos

### Software necesario:
- **.NET 9 SDK** (descarga: https://dotnet.microsoft.com/download)
- **Visual Studio 2022** o **Visual Studio Code** con extensión C#
- **Cuenta de Supabase** (gratuita: https://supabase.com)

### Servicios cloud:
- **Supabase Project** (para base de datos y autenticación)
- **Azure Document Intelligence** (opcional, para Sprint 4)

---

## ⚙️ Configuración Paso a Paso

### 1. Clonar/Abrir el Proyecto

```powershell
cd c:\Users\titen\OneDrive\Desktop\FlujoCajaSprint1\FlujoCajaSprint1\FlujoCajaActual\FlujoCajaWpf
```

### 2. Configurar Supabase

#### 2.1 Crear Proyecto en Supabase
1. Ve a https://supabase.com y crea una cuenta
2. Crea un nuevo proyecto
3. Espera a que se inicialice (2-3 minutos)

#### 2.2 Obtener Credenciales
1. En el dashboard de Supabase, ve a **Settings** → **API**
2. Copia:
   - **Project URL** (ejemplo: `https://xyzproject.supabase.co`)
   - **anon public key** (clave larga que empieza con `eyJ...`)

#### 2.3 Configurar appsettings.json

Edita el archivo `appsettings.json` en la raíz del proyecto:

```json
{
  "Supabase": {
    "Url": "https://TU_PROYECTO.supabase.co",
    "Key": "eyJ...TU_ANON_KEY_AQUI"
  },
  "Azure": {
    "DocumentIntelligence": {
      "Endpoint": "AZURE_ENDPOINT_AQUI",
      "ApiKey": "AZURE_KEY_AQUI"
    }
  }
}
```

### 3. Crear Base de Datos en Supabase

#### 3.1 Ejecutar Script SQL
1. En Supabase dashboard, ve a **SQL Editor**
2. Crea un nuevo query
3. Copia todo el contenido de `Scripts/InitDatabase.sql`
4. Pega y ejecuta (Run)
5. Verifica que muestre "Success. No rows returned"

#### 3.2 Verificar Tablas Creadas
En **Table Editor**, deberías ver:
- ✅ casas
- ✅ duenos
- ✅ categorias
- ✅ categorias_movimientos
- ✅ movimientos
- ✅ facturas
- ✅ user_profiles

### 4. Crear Usuario de Prueba

#### Opción A: Desde Supabase Auth (Recomendado)
1. En Supabase, ve a **Authentication** → **Users**
2. Clic en **Add User** → **Create new user**
3. Email: `test@example.com`
4. Password: `Test123456`
5. Confirma el email automáticamente
6. Guarda

#### Opción B: Desde SQL
```sql
-- Ejecutar en SQL Editor
INSERT INTO auth.users (email, encrypted_password, email_confirmed_at)
VALUES ('test@example.com', crypt('Test123456', gen_salt('bf')), NOW());
```

### 5. Habilitar Email Confirmation (Opcional)

Para desarrollo local, desactiva la confirmación de email:

1. Ve a **Authentication** → **Settings**
2. En **Email Auth**, desactiva:
   - ❌ Enable email confirmations
   - ❌ Secure email change

---

## ▶️ Ejecutar la Aplicación

### Desde Visual Studio Code:

```powershell
dotnet run
```

### Desde Visual Studio 2022:

1. Abre `FlujoCajaWpf.csproj`
2. Presiona **F5** o clic en ▶️ Start

### Credenciales de prueba:

```
Email: test@example.com
Password: Test123456
```

---

## 🧪 Probar Funcionalidades

### Test 1: Login
1. Ingresar email y contraseña
2. Verificar que carga el menú principal
3. ✅ Exitoso si se muestra el header con logo

### Test 2: Ver Casas
1. El menú debe mostrar tarjetas de casas (si las hay)
2. Si no hay casas, verás mensaje "📭 No hay casas registradas"

### Test 3: Búsqueda
1. Escribir en el campo de búsqueda
2. Verificar que filtra en tiempo real

### Test 4: Cerrar Sesión
1. Clic en "🚪 Cerrar Sesión"
2. Confirmar diálogo
3. Volver a pantalla de login

---

## 📊 Agregar Datos de Prueba

### Insertar una casa de prueba:

```sql
-- En Supabase SQL Editor
INSERT INTO duenos (nombre, correo) 
VALUES ('María González', 'maria@example.com');

INSERT INTO casas (nombre, direccion, pais, moneda, balance_actual, dueno_id)
VALUES (
  'Casa Playa Sámara',
  'Avenida Principal 123',
  'Costa Rica',
  'USD',
  5000.00,
  (SELECT id FROM duenos WHERE nombre = 'María González' LIMIT 1)
);
```

Refresca la aplicación (F5) y deberías ver la casa en el menú.

---

## 🐛 Solución de Problemas

### Error: "Error al conectar con Supabase"

**Causa:** Credenciales incorrectas o sin conexión a internet

**Solución:**
1. Verifica que `appsettings.json` tenga URL y Key correctos
2. Verifica conexión a internet
3. Revisa la consola de Output en VS para ver logs

### Error: "Credenciales inválidas" al hacer login

**Causa:** Usuario no existe o contraseña incorrecta

**Solución:**
1. Verifica que creaste el usuario en Supabase Auth
2. Confirma que el email está verificado
3. Prueba con: `test@example.com` / `Test123456`

### No se ven las casas en el menú

**Causa:** No hay datos en la tabla `casas`

**Solución:**
1. Ejecuta el script SQL de datos de prueba (arriba)
2. Verifica en Supabase Table Editor que la tabla `casas` tiene registros
3. Verifica que las casas tengan `activa = true`

### Error: "The type or namespace 'Supabase' could not be found"

**Causa:** Paquetes NuGet no restaurados

**Solución:**
```powershell
dotnet restore
dotnet build
```

---

## 📁 Estructura del Proyecto

```
FlujoCajaWpf/
├── Commands/              # ICommand implementations
│   └── RelayCommand.cs
├── Converters/            # Value converters para XAML
│   └── ValueConverters.cs
├── Data/                  # Helpers de Supabase
│   ├── SupabaseHelper.cs
│   ├── SupabaseAuthHelper.cs
│   └── SupabaseCasaHelper.cs
├── Models/                # Modelos de datos
│   ├── Usuario.cs
│   ├── Casa.cs
│   ├── CasaSupabase.cs
│   └── Propiedad.cs
├── Resources/             # Imágenes y recursos
│   └── LogoSamaraRental.PNG
├── Scripts/               # Scripts SQL
│   └── InitDatabase.sql
├── Services/              # Servicios auxiliares
│   └── NavigationService.cs
├── Styles/                # Estilos XAML
│   ├── Colors.xaml
│   ├── Buttons.xaml
│   └── TextBoxes.xaml
├── ViewModels/            # ViewModels MVVM
│   ├── Base/
│   │   └── ViewModelBase.cs
│   ├── LoginViewModel.cs
│   └── MenuPrincipalViewModel.cs
├── Views/                 # Ventanas XAML
│   ├── LoginWindow.xaml
│   └── MenuPrincipalWindow.xaml
├── App.xaml               # Aplicación principal
├── appsettings.json       # Configuración (credenciales)
└── FlujoCajaWpf.csproj    # Proyecto
```

---

## 🎨 Paleta de Colores

| Color | Hex | Uso |
|-------|-----|-----|
| Verde Principal | `#2E7D32` | Botones primarios, header |
| Verde Claro | `#66BB6A` | Highlights |
| Verde Oscuro | `#1B5E20` | Gradientes |
| Rojo Acento | `#C62828` | Errores, balances negativos |
| Gris Fondo | `#F5F5F5` | Fondo de aplicación |
| Blanco | `#FFFFFF` | Tarjetas, contenedores |

---

## 🔐 Seguridad

⚠️ **IMPORTANTE:** 

- **NUNCA** subas `appsettings.json` con credenciales reales a Git
- Agrega `appsettings.json` a `.gitignore`
- Para producción, usa variables de entorno

```gitignore
# .gitignore
appsettings.json
appsettings.*.json
```

---

## 📝 Próximos Sprints

### Sprint 2: CRUD de Dueños, Categorías y Casas
- Gestionar dueños
- Gestionar categorías
- Agregar/editar/eliminar casas

### Sprint 3: Movimientos y Balance
- Ver detalle de casa con movimientos
- Agregar ingresos/egresos
- Cálculo automático de balance

### Sprint 4: Facturas con Azure AI
- Subir facturas (PDF/imagen)
- Procesamiento con OCR
- Extracción automática de datos

### Sprint 5: Reportes y Mejoras
- Resumen consolidado
- Exportar a Excel
- Notificaciones en tiempo real

---

## 🆘 Soporte

Si tienes problemas:

1. **Revisa los logs:** La consola de Output muestra mensajes útiles
2. **Verifica Supabase:** Dashboard → Logs para ver errores de API
3. **Consulta documentación:** 
   - Supabase: https://supabase.com/docs
   - WPF: https://learn.microsoft.com/wpf

---

## 📄 Licencia

Sistema privado de gestión - Playa Sámara © 2025

---

**✨ ¡Sprint 1 completado exitosamente!**

🎯 Siguiente paso: Configura Supabase y prueba el login.
