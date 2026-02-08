# 📋 Documentación Detallada de Sprints - Sistema de Flujo de Caja

## 🎯 Objetivo General
Sistema de gestión de flujo de caja para múltiples propiedades (casas/apartamentos) con seguimiento de ingresos, egresos, y generación de reportes.

---

## ✅ SPRINT 1 - Autenticación y Menú Principal (COMPLETADO)

### 📦 Funcionalidades Implementadas

#### 1. **Sistema de Autenticación**
- **LoginWindow.xaml** (550x850px)
  - Formulario de login con email y contraseña
  - Integración con Supabase Auth (auth.users)
  - Validación de credenciales
  - Logo centrado (150px)
  - Diseño con gradiente azul (#1E3A8A → #3B82F6)
  - Botón "Iniciar Sesión" con estilo primario

- **SupabaseAuthHelper.cs**
  - `SignInAsync(email, password)` - Autenticación de usuarios
  - `SignOutAsync()` - Cierre de sesión
  - `GetCurrentUser()` - Obtener usuario actual
  - Manejo de sesión con Supabase

#### 2. **Menú Principal**
- **MenuPrincipalWindow.xaml**
  - Header con logo, título y botón "Cerrar Sesión"
  - Barra de búsqueda (filtro por nombre, categoría, dueño)
  - Grid de tarjetas de propiedades con:
    - Nombre de la propiedad
    - Estado (Activa/Inactiva) con badge de color
    - Categoría (Residencial, Comercial, Vacacional, etc.)
    - Moneda (USD, CRC, EUR)
    - Nombre del dueño principal
    - Botón 📝 para abrir notas
  - Mensaje "No hay casas registradas" cuando no hay datos
  - Loading overlay durante carga de datos

- **MenuPrincipalViewModel.cs**
  - Carga de casas desde Supabase
  - Filtrado en tiempo real por búsqueda
  - Comando para abrir detalle de casa
  - Comando para cerrar sesión
  - Comando para abrir popup de notas
  - Propiedades observables para UI reactiva

#### 3. **Popup de Notas**
- **NotasPopup.xaml** (600x500px)
  - Ventana modal con fondo transparente
  - Header con título de la casa y botón cerrar
  - Sección de dueño principal (solo lectura)
  - TextBox grande para notas (180px altura)
  - Botones: Cancelar y Guardar
  - Diseño centrado con sombra

- **NotasViewModel.cs**
  - Carga de notas actuales
  - Actualización de notas en Supabase
  - Validación y manejo de errores

#### 4. **Modelos de Datos**

**CasaSupabase.cs** - Mapeo directo con tabla `casas`
```csharp
int Id (PK)
string Nombre
bool Activo
int DuenoId (FK → duenos)
int CategoriaId (FK → categorias)
string? RutaImagen
string Moneda
string? Notas
DateTime FechaCreacion
DateTime? FechaModificacion
```

**Casa.cs** - Modelo de UI
```csharp
int Id
string Nombre
int DuenoId
string? DuenoNombre (JOIN)
int CategoriaId
string? CategoriaNombre (JOIN)
string Moneda
string? Notas
bool Activo
```

**DuenoSupabase.cs** - Mapeo con tabla `duenos`
```csharp
int Id (PK)
string Nombre
string Apellido
string? Telefono
string? Email
string? NombreCompleto
```

**CategoriaSupabase.cs** - Mapeo con tabla `categorias`
```csharp
int Id (PK)
string Nombre
string? Descripcion
```

**Propiedad.cs** - Modelo extendido para tarjetas
```csharp
int Id
string Nombre
string Moneda
string? CategoriaNombre
bool Activo
string DuenoNombre
string? Notas
string EstadoTexto
string ColorEstado
```

**Usuario.cs** - Usuario autenticado
```csharp
string Id (UUID de auth.users)
string Email
DateTime CreatedAt
```

#### 5. **Helpers de Supabase**

**SupabaseCasaHelper.cs**
- `ObtenerTodasCasasAsync()` - Carga casas + JOIN duenos + categorias
- `ObtenerCasaPorIdAsync(id)` - Obtener casa específica
- `InsertarCasaAsync(casa)` - Crear nueva casa
- `ActualizarCasaAsync(casa)` - Actualizar casa existente
- `DesactivarCasaAsync(id)` - Soft delete
- `EliminarCasaAsync(id)` - Hard delete

**Estrategia de JOIN:**
- Ejecuta 3 queries en paralelo (casas, duenos, categorias)
- Crea diccionarios por ID
- Realiza JOIN en memoria (C#)
- Asigna DuenoNombre y CategoriaNombre a cada Casa

#### 6. **Base de Datos Supabase**

**Tabla `duenos`**
```sql
id SERIAL PRIMARY KEY
nombre VARCHAR(100) NOT NULL
apellido VARCHAR(100) NOT NULL
telefono VARCHAR(20)
email VARCHAR(255)
fechacreacion TIMESTAMPTZ DEFAULT now()
fechamodificacion TIMESTAMPTZ
```

**Tabla `categorias`**
```sql
id SERIAL PRIMARY KEY
nombre VARCHAR(50) NOT NULL UNIQUE
descripcion TEXT
fechacreacion TIMESTAMPTZ DEFAULT now()
```

**Tabla `casas`**
```sql
id SERIAL PRIMARY KEY
nombre VARCHAR(200) NOT NULL
activo BOOLEAN DEFAULT true
duenoid INT NOT NULL REFERENCES duenos(id) RESTRICT
categoriaid INT NOT NULL REFERENCES categorias(id) RESTRICT
rutaimagen TEXT
moneda VARCHAR(3) DEFAULT 'USD'
notas TEXT
fechacreacion TIMESTAMPTZ DEFAULT now()
fechamodificacion TIMESTAMPTZ
usuario_creador_id UUID REFERENCES auth.users(id)
usuario_modificador_id UUID REFERENCES auth.users(id)
```

**Índices:**
- `idx_casas_activo` en `activo`
- `idx_casas_duenoid` en `duenoid`
- `idx_casas_categoriaid` en `categoriaid`
- `idx_casas_notas` GIN full-text search en `notas`

**Triggers:**
- `actualizar_fechamodificacion` en UPDATE de casas

### 🎨 Estilos y Colores
- **PrimaryBlue:** #1E3A8A (Azul oscuro)
- **LightBlue:** #3B82F6 (Azul medio)
- **AccentYellow:** #F59E0B (Amarillo acento)
- **BackgroundGray:** #D1D5DB (Gris claro)
- **TextPrimary:** #1F2937 (Gris muy oscuro)
- **TextSecondary:** #6B7280 (Gris medio)
- **BorderColor:** #E5E7EB (Gris muy claro)
- **Success:** #10B981 (Verde)

**Estilos de Botones:**
- `PrimaryButton` - Azul con hover
- `OutlineButton` - Borde azul con hover

### 🔧 Configuración

**appsettings.json**
```json
{
  "Supabase": {
    "Url": "https://txytwyrujgdnfbrrjgvz.supabase.co",
    "Key": "[SUPABASE_ANON_KEY]"
  }
}
```

### ✅ Completado
- ✅ Login con Supabase Auth
- ✅ Menú principal con tarjetas
- ✅ Carga de casas con JOIN (duenos + categorias)
- ✅ Búsqueda y filtrado
- ✅ Popup de notas editable
- ✅ Integración completa con base de datos
- ✅ Manejo de estados (loading, vacío, error)
- ✅ UI moderna y responsiva

---

## 🚧 SPRINT 2 - CRUD de Dueños, Categorías y Casas (PENDIENTE)

### 📦 Funcionalidades a Implementar

#### 1. **Gestión de Dueños**
- **GestionDuenosWindow.xaml**
  - Lista de dueños en DataGrid
  - Botones: Agregar, Editar, Eliminar
  - Búsqueda por nombre/apellido/email
  - Filtros avanzados

- **AgregarDuenoWindow.xaml**
  - Formulario modal para crear/editar dueño
  - Campos:
    - Nombre (requerido)
    - Apellido (requerido)
    - Teléfono (opcional)
    - Email (opcional, validación formato)
  - Validaciones en tiempo real
  - Botones: Guardar, Cancelar

#### 2. **Gestión de Categorías**
- **GestionCategoriasWindow.xaml**
  - Lista de categorías
  - CRUD completo
  - Validación de nombre único

- **AgregarCategoriaWindow.xaml**
  - Nombre de categoría
  - Descripción
  - Color asociado (opcional)

#### 3. **Gestión de Casas**
- **AgregarCasaWindow.xaml**
  - Formulario completo para casa
  - Campos:
    - Nombre (requerido)
    - Dueño (ComboBox con búsqueda)
    - Categoría (ComboBox)
    - Moneda (USD, CRC, EUR)
    - Imagen (opcional, upload a Supabase Storage)
    - Notas
    - Estado (Activa/Inactiva)
  
- **EditarCasaWindow.xaml**
  - Mismo formulario que agregar
  - Pre-cargado con datos existentes
  - Validación antes de guardar

#### 4. **Menú Principal - Acciones**
- Botón "➕ Nueva Casa" en header
- Botón "⚙️ Gestión" con dropdown:
  - Gestión de Dueños
  - Gestión de Categorías
  - Gestión de Casas
- Click en tarjeta abre detalle de casa

### 🗂️ ViewModels Necesarios
- `GestionDuenosViewModel.cs`
- `AgregarDuenoViewModel.cs`
- `GestionCategoriasViewModel.cs`
- `AgregarCategoriaViewModel.cs`
- `AgregarCasaViewModel.cs`
- `EditarCasaViewModel.cs`

### 🔨 Helpers a Crear
- `SupabaseDuenoHelper.cs`
  - CRUD completo de dueños
  - Validación de email
- `SupabaseCategoriaHelper.cs`
  - CRUD de categorías
  - Validación de nombre único

### 📸 Supabase Storage
- Bucket `imagenes-casas`
- Upload de imágenes
- Generación de URLs públicas
- Compresión de imágenes (opcional)

---

## 🚧 SPRINT 3 - Movimientos y Balance (PENDIENTE)

### 📦 Funcionalidades a Implementar

#### 1. **Tabla de Movimientos**

**Tabla `categorias_movimientos`**
```sql
id SERIAL PRIMARY KEY
nombre VARCHAR(100) NOT NULL UNIQUE (ej: "Alquiler", "Servicios", "Reparaciones")
tipo VARCHAR(10) NOT NULL CHECK (tipo IN ('ingreso', 'egreso'))
descripcion TEXT
fechacreacion TIMESTAMPTZ DEFAULT now()
```

**Tabla `movimientos`**
```sql
id SERIAL PRIMARY KEY
casaid INT NOT NULL REFERENCES casas(id) CASCADE
categoria_movimiento_id INT NOT NULL REFERENCES categorias_movimientos(id) RESTRICT
tipo VARCHAR(10) NOT NULL CHECK (tipo IN ('ingreso', 'egreso'))
monto DECIMAL(15,2) NOT NULL CHECK (monto > 0)
fecha DATE NOT NULL DEFAULT CURRENT_DATE
descripcion TEXT
factura_url TEXT
fechacreacion TIMESTAMPTZ DEFAULT now()
fechamodificacion TIMESTAMPTZ
usuario_creador_id UUID REFERENCES auth.users(id)
```

**Índices:**
- `idx_movimientos_casaid` en `casaid`
- `idx_movimientos_fecha` en `fecha`
- `idx_movimientos_tipo` en `tipo`

#### 2. **Vista de Detalle de Casa**
- **DetalleCasaWindow.xaml**
  - Header con información de la casa
  - Tabs:
    - **Resumen**: Balance actual, gráficos
    - **Movimientos**: Lista de ingresos/egresos
    - **Historial**: Timeline de cambios
  
- **Panel de Resumen**
  - Balance total (ingresos - egresos)
  - Total ingresos del mes
  - Total egresos del mes
  - Gráfico de barras (últimos 6 meses)
  - Gráfico de pastel (categorías de gastos)

- **Panel de Movimientos**
  - DataGrid con movimientos
  - Filtros:
    - Rango de fechas
    - Tipo (Ingreso/Egreso/Todos)
    - Categoría
  - Búsqueda por descripción
  - Botones: Agregar, Editar, Eliminar
  - Exportar a Excel/PDF

#### 3. **Gestión de Movimientos**
- **AgregarMovimientoWindow.xaml**
  - Formulario modal
  - Campos:
    - Casa (ComboBox)
    - Tipo (Ingreso/Egreso)
    - Categoría (ComboBox dinámico según tipo)
    - Monto (validación numérica)
    - Fecha (DatePicker)
    - Descripción
    - Adjuntar factura (opcional)
  - Cálculo automático de balance

#### 4. **Categorías de Movimientos**
- **GestionCategoriasMovimientosWindow.xaml**
  - CRUD de categorías de movimientos
  - Separación por tipo (ingreso/egreso)
  - Ejemplos predefinidos:
    - **Ingresos:** Alquiler, Depósito, Otros Ingresos
    - **Egresos:** Servicios Públicos, Reparaciones, Mantenimiento, Impuestos, Seguros

### 🔨 Helpers a Crear
- `SupabaseMovimientoHelper.cs`
  - CRUD de movimientos
  - Cálculo de balance por casa
  - Obtener movimientos por rango de fechas
  - Estadísticas por categoría
- `SupabaseCategoriaMovimientoHelper.cs`
  - CRUD de categorías de movimientos

### 📊 Funciones PostgreSQL
```sql
-- Calcular balance de una casa hasta una fecha
CREATE OR REPLACE FUNCTION calcular_balance_casa(casa_id INT, hasta_fecha DATE)
RETURNS DECIMAL(15,2) AS $$
  SELECT COALESCE(
    SUM(CASE WHEN tipo = 'ingreso' THEN monto ELSE -monto END), 
    0
  )
  FROM movimientos
  WHERE casaid = casa_id AND fecha <= hasta_fecha;
$$ LANGUAGE SQL STABLE;
```

### 📈 Librerías para Gráficos
- **LiveCharts2** para gráficos WPF
  - Instalación: `dotnet add package LiveChartsCore.SkiaSharpView.WPF`
  - Gráficos de barras para balances mensuales
  - Gráficos de pastel para distribución de gastos

---

## 🚧 SPRINT 4 - Escaneo de Facturas con Azure AI (PENDIENTE)

### 📦 Funcionalidades a Implementar

#### 1. **Azure AI Document Intelligence**
- Integración con Azure Cognitive Services
- **Tabla `facturas`**
```sql
id SERIAL PRIMARY KEY
movimientoid INT REFERENCES movimientos(id) CASCADE
archivo_url TEXT NOT NULL
datos_extraidos JSONB
monto_extraido DECIMAL(15,2)
fecha_extraida DATE
proveedor_extraido VARCHAR(200)
confianza DECIMAL(3,2) (0.00 a 1.00)
procesado BOOLEAN DEFAULT false
fechacreacion TIMESTAMPTZ DEFAULT now()
```

#### 2. **Ventana de Escaneo**
- **EscanearFacturaWindow.xaml**
  - Arrastrar y soltar imagen/PDF
  - Preview de la factura
  - Botón "Escanear con IA"
  - Resultados extraídos:
    - Monto detectado
    - Fecha detectada
    - Proveedor detectado
    - Nivel de confianza
  - Botones: Confirmar, Editar, Cancelar
  - Auto-creación de movimiento con datos

#### 3. **Azure AI Document Intelligence Setup**
- Crear recurso en Azure Portal
- Configurar keys en appsettings.json
```json
{
  "Azure": {
    "DocumentIntelligence": {
      "Endpoint": "https://[nombre].cognitiveservices.azure.com/",
      "Key": "[AZURE_KEY]"
    }
  }
}
```

#### 4. **Procesamiento**
- Upload de factura a Supabase Storage bucket `facturas`
- Envío a Azure Document Intelligence API
- Extracción de campos:
  - Total/Monto
  - Fecha de emisión
  - Nombre del proveedor
  - Items individuales
- Guardado de JSON completo en `datos_extraidos`
- Validación manual opcional

### 🔨 Helpers a Crear
- `AzureDocumentIntelligenceHelper.cs`
  - `AnalizarFacturaAsync(fileStream)`
  - `ExtraerDatosFacturaAsync(imageUrl)`
  - Parsing de respuesta JSON
- `SupabaseFacturaHelper.cs`
  - CRUD de facturas
  - Vincular factura con movimiento

### 📦 NuGet Packages
```bash
dotnet add package Azure.AI.FormRecognizer
dotnet add package Azure.Storage.Blobs
```

### 🎯 Flujo de Usuario
1. Usuario hace clic en "📷 Escanear Factura"
2. Selecciona o arrastra imagen/PDF
3. Sistema sube a Supabase Storage
4. Envía a Azure AI para procesamiento
5. Muestra resultados extraídos
6. Usuario confirma o edita datos
7. Crea movimiento automáticamente
8. Vincula factura con movimiento

---

## 🚧 SPRINT 5 - Reportes y Mejoras Finales (PENDIENTE)

### 📦 Funcionalidades a Implementar

#### 1. **Panel de Reportes**
- **ReportesWindow.xaml**
  - Selector de tipo de reporte:
    - Resumen Consolidado (todas las casas)
    - Reporte por Casa
    - Reporte por Categoría
    - Reporte de Rentabilidad
  - Filtros:
    - Rango de fechas
    - Casas específicas
    - Moneda
  - Botones: Generar, Exportar, Imprimir

#### 2. **Reporte Consolidado**
- Balance total de todas las casas
- Ingresos totales
- Egresos totales
- Rentabilidad neta
- Gráfico de barras comparativo por casa
- Top 5 casas más rentables
- Top 5 casas con mayores gastos

#### 3. **Reporte por Casa**
- Detalle completo de una casa
- Balance histórico (últimos 12 meses)
- Desglose de ingresos por categoría
- Desglose de egresos por categoría
- Proyección de ingresos futuros
- Lista de movimientos del período

#### 4. **Exportación**
- **Excel:** usando EPPlus o ClosedXML
- **PDF:** usando QuestPDF o iTextSharp
- **CSV:** para análisis externo
- Plantillas personalizables

#### 5. **Dashboard Principal**
- Widgets en MenuPrincipalWindow:
  - Total casas activas
  - Balance consolidado
  - Ingresos del mes
  - Egresos del mes
  - Alertas (pagos pendientes, etc.)

#### 6. **Mejoras UX/UI**
- Animaciones suaves con Storyboards
- Tooltips informativos
- Confirmaciones elegantes
- Notificaciones toast
- Tema oscuro (opcional)
- Configuración de usuario

#### 7. **Funciones Adicionales**
- **Respaldo de Base de Datos**
  - Exportar datos completos
  - Importar desde respaldo
- **Historial de Cambios (Auditoría)**
  - Tabla `auditoria` con todos los cambios
  - Quién, cuándo, qué cambió
- **Configuración**
  - Moneda predeterminada
  - Formato de fecha
  - Idioma (opcional)

### 🗂️ Función PostgreSQL para Resumen
```sql
CREATE OR REPLACE FUNCTION obtener_resumen_consolidado(mes INT, anio INT)
RETURNS TABLE (
  casa_id INT,
  casa_nombre VARCHAR,
  total_ingresos DECIMAL,
  total_egresos DECIMAL,
  balance DECIMAL
) AS $$
BEGIN
  RETURN QUERY
  SELECT 
    c.id,
    c.nombre,
    COALESCE(SUM(CASE WHEN m.tipo = 'ingreso' THEN m.monto ELSE 0 END), 0),
    COALESCE(SUM(CASE WHEN m.tipo = 'egreso' THEN m.monto ELSE 0 END), 0),
    COALESCE(SUM(CASE WHEN m.tipo = 'ingreso' THEN m.monto ELSE -m.monto END), 0)
  FROM casas c
  LEFT JOIN movimientos m ON c.id = m.casaid
  WHERE EXTRACT(MONTH FROM m.fecha) = mes AND EXTRACT(YEAR FROM m.fecha) = anio
  GROUP BY c.id, c.nombre;
END;
$$ LANGUAGE plpgsql;
```

### 📦 NuGet Packages
```bash
dotnet add package EPPlus  # Para Excel
dotnet add package QuestPDF  # Para PDF
dotnet add package Newtonsoft.Json  # Para JSON
```

---

## 🎯 Stack Tecnológico Completo

### Frontend
- **WPF .NET 9** - Framework de UI
- **MVVM Pattern** - Arquitectura
- **XAML** - Markup para UI
- **LiveCharts2** - Gráficos

### Backend/Database
- **Supabase** - Backend as a Service
  - PostgreSQL - Base de datos
  - Postgrest - API REST automática
  - Supabase Auth - Autenticación
  - Supabase Storage - Almacenamiento de archivos
  - Realtime - Suscripciones en tiempo real
- **Azure AI Document Intelligence** - OCR de facturas

### Librerías .NET
- **Supabase.Client** - Cliente oficial de Supabase
- **Postgrest** - Cliente REST para PostgreSQL
- **LiveChartsCore.SkiaSharpView.WPF** - Gráficos
- **Azure.AI.FormRecognizer** - Escaneo de facturas
- **EPPlus** - Generación de Excel
- **QuestPDF** - Generación de PDF

### Herramientas de Desarrollo
- **Visual Studio 2022** - IDE
- **Git** - Control de versiones
- **Supabase Dashboard** - Gestión de base de datos
- **Azure Portal** - Gestión de servicios Azure

---

## 📝 Notas Importantes

### Decisiones de Diseño
1. **IDs Integer vs UUID:** Se decidió usar `SERIAL` (integers) en lugar de UUIDs por simplicidad y rendimiento
2. **JOIN en C# vs SQL:** Debido a limitaciones de Supabase Postgrest, los JOINs se realizan en memoria
3. **Soft Delete:** Las casas usan campo `activo` en lugar de eliminar registros
4. **Auditoría:** Campos `usuario_creador_id` y `usuario_modificador_id` referencian `auth.users`

### Seguridad
- **RLS (Row Level Security):** Activado en todas las tablas
- **Policies:** Solo usuarios autenticados pueden acceder
- **Storage Rules:** Bucket facturas solo para usuarios autenticados
- **API Keys:** Nunca commitear keys en Git

### Rendimiento
- **Índices:** Creados en columnas más consultadas
- **Full-text Search:** Índice GIN en campo `notas`
- **Paginación:** Implementar para listas grandes (Sprint 2+)
- **Caché:** Considerar caché local para datos frecuentes

### Testing
- Crear datos de prueba con script `InitDatabase_v2.sql`
- 5 registros de ejemplo por tabla
- Probar con diferentes escenarios (casas sin movimientos, etc.)

---

## 🚀 Próximos Pasos Inmediatos

### Para continuar con Sprint 2:
1. Crear `GestionDuenosWindow.xaml` con DataGrid
2. Implementar `SupabaseDuenoHelper.cs`
3. Crear formulario `AgregarDuenoWindow.xaml`
4. Repetir para Categorías
5. Crear formulario completo de Casas con upload de imágenes
6. Configurar Supabase Storage bucket para imágenes

### Preparación para Sprint 3:
1. Crear tablas `categorias_movimientos` y `movimientos`
2. Insertar categorías predefinidas
3. Diseñar UI de DetalleCasaWindow
4. Investigar LiveCharts2 para gráficos

### Preparación para Sprint 4:
1. Crear cuenta Azure
2. Provisionar recurso Document Intelligence
3. Probar API con facturas de ejemplo
4. Configurar Supabase Storage para facturas

---

**Última actualización:** Diciembre 7, 2025  
**Estado actual:** Sprint 1 completado, listo para Sprint 2
