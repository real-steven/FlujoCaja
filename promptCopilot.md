# 📋 CONTEXTO COMPLETO DEL PROYECTO - FLUJO DE CAJA WPF

## 1️⃣ CONTEXTO DETALLADO DEL PROYECTO

### 🎯 Información General
- **Nombre**: FlujoCajaWpf - Sistema de Gestión de Flujo de Caja para Propiedades
- **Tecnología**: WPF .NET 9.0 (Windows Presentation Foundation)
- **Backend**: Supabase v1.1.1 (PostgreSQL + Auth + Storage)
- **Arquitectura**: MVVM (Model-View-ViewModel)
- **Lenguaje**: C# con XAML
- **IDE**: Visual Studio Code

### 🏗️ Estructura del Proyecto
```
FlujoCajaWpf/
├── Views/
│   ├── MenuPrincipalWindow.xaml          # Ventana principal con navegación
│   ├── LoginWindow.xaml                   # Ventana de autenticación
│   ├── AgregarWindow.xaml                 # Contenedor de formularios CRUD
│   ├── GestionWindow.xaml                 # Ventana de gestión de entidades
│   ├── DetalleCasaWindow.xaml             # Detalle completo de cada casa
│   ├── HistorialWindow.xaml               # Sistema de auditoría
│   ├── InactivasWindow.xaml               # Casas inactivas
│   ├── TutorialWindow.xaml                # Tutorial del sistema
│   └── Controls/
│       ├── AgregarCasaControl.xaml        # Formulario Casas (8 campos)
│       ├── AgregarDuenoControl.xaml       # Formulario Dueños (4 campos)
│       ├── AgregarCategoriaPropiedadControl.xaml
│       ├── AgregarCategoriaMovimientoControl.xaml
│       ├── GestionCasasControl.xaml       # DataGrid de casas
│       ├── GestionDuenosControl.xaml      # DataGrid de dueños
│       ├── GestionCategoriasControl.xaml  # DataGrid de categorías
│       ├── GestionCategoriasMovimientosControl.xaml
│       └── TutorialControl.xaml           # Contenido del tutorial
├── ViewModels/
│   ├── LoginViewModel.cs
│   └── MenuPrincipalViewModel.cs
├── Models/
│   ├── Usuario.cs
│   ├── PerfilUsuario.cs
│   ├── Casa.cs / CasaSupabase.cs
│   ├── Dueno.cs / DuenoSupabase.cs
│   ├── Categoria.cs / CategoriaSupabase.cs
│   ├── CategoriaMovimientoSupabase.cs
│   ├── Movimiento.cs / MovimientoSupabase.cs
│   ├── Nota.cs / NotaSupabase.cs
│   ├── Foto.cs / FotoSupabase.cs
│   └── Propiedad.cs                       # Modelo extendido para UI
├── Data/
│   ├── SupabaseHelper.cs                 # Cliente principal
│   ├── SupabaseAuthHelper.cs             # Autenticación
│   ├── SupabaseCasaHelper.cs
│   ├── SupabaseDuenoHelper.cs
│   ├── SupabaseCategoriaHelper.cs
│   ├── SupabaseCategoriaMovimientoHelper.cs
│   ├── SupabaseMovimientoHelper.cs       # CRUD de movimientos
│   ├── SupabaseHojaMensualHelper.cs      # Gestión de hojas mensuales
│   ├── SupabaseNotaHelper.cs             # CRUD de notas
│   ├── SupabaseFotoHelper.cs             # CRUD de fotos
│   ├── SupabaseStorageHelper.cs          # Gestión de imágenes
│   └── SupabaseAuditoriaHelper.cs        # Sistema de auditoría
├── Commands/
│   └── RelayCommand.cs
├── Converters/
│   └── ValueConverters.cs
├── Scripts/
│   ├── InitDatabase_v2.sql               # Script maestro de base de datos
│   ├── 04_create_auditoria_table.sql     # Tabla de auditoría
│   └── AgregarNotasCasa.sql              # Tabla de notas
└── appsettings.json                       # Configuración Supabase
```

### 🗄️ Base de Datos Supabase

**Tablas principales:**
1. **usuarios**
   - id (uuid, PK)
   - auth_id (uuid, FK a auth.users)
   - nombre, apellido, email, telefono
   - rol (admin/usuario)
   - activo (boolean)

2. **duenos**
   - id (bigint, PK)
   - nombre, apellido, telefono, email
   - NombreCompleto, activo
   - fecha_creacion, fecha_actualizacion

3. **categorias**
   - id (integer, PK)
   - nombre, descripcion
   - fechacreacion, activo

4. **casas**
   - id (integer, PK)
   - nombre, duenoid, categoriaid
   - moneda (USD/CRC/EUR), activa, notas
   - rutaimagen
   - fechacreacion

5. **categorias_movimientos**
   - id (integer, PK)
   - nombre, tipo (Ingreso/Gasto), descripcion
   - activo, fechacreacion

6. **hojas_mensuales**
   - id (integer, PK)
   - casaid, mes, anio
   - cerrada, fechacreacion
   - UNIQUE (casaid, mes, anio)

7. **movimientos**
   - id (integer, PK)
   - casaid, hoja_mensual_id
   - fecha, descripcion, monto, categoria
   - tipo_movimiento (Ingreso/Gasto)
   - usuario_creador_id, usuario_modificador_id
   - fechacreacion, fechamodificacion
   - activo

8. **notas_casa**
   - id (integer SERIAL, PK)
   - casaid, contenido
   - fechacreacion

9. **fotos_casa**
   - id (integer SERIAL, PK)
   - casaid, url, nombre_archivo
   - fechacreacion

10. **auditoria**
    - id (SERIAL, PK)
    - usuario_email, modulo, tipo_accion
    - entidad_id, entidad_nombre
    - descripcion
    - datos_anteriores (JSONB), datos_nuevos (JSONB)
    - fecha

**Storage Buckets:**
- `CasasFotos` - Almacenamiento de fotos de propiedades
- `FotosCasas` - Galería de fotos adicionales

### 🎨 Sistema de Diseño Visual

**Paleta de Colores:**
- Background principal: `#F3F4F6` (gris claro)
- Sidebar: `#202355` (azul oscuro)
- Cards: `White` con sombras
- Texto: `Black` (todos los textos)
- Bordes: `#E5E7EB`
- Focus/Hover: `#3B82F6` (azul)
- Botón Guardar: `#10B981` (verde)
- Botón Limpiar: `#F59E0B` (amarillo/naranja)
- Estado Activo: `#F59E0B` (amarillo)

**Tipografía:**
- Títulos principales: 28px Bold
- Subtítulos/Secciones: 14-16px Bold
- Labels: 13-14px SemiBold
- Texto normal: 13px

**Componentes:**
- Cards: CornerRadius 12px, DropShadowEffect
- Inputs: Height 38-45px, CornerRadius 6px, Padding 12-15px
- Botones: CornerRadius 8-10px, con sombras de color
- Separadores: 1-2px height, color #E5E7EB

**Responsive Design:**
- Grid columns con MinWidth 200px
- HorizontalAlignment="Stretch"
- Formularios adaptativos sin MaxWidth fijo

### 🔧 Configuración Técnica

**appsettings.json:**
```json
{
  "Supabase": {
    "Url": "https://[proyecto].supabase.co",
    "Key": "[anon-key]"
  }
}
```

**NuGet Packages:**
- Supabase v1.1.1
- Microsoft.Extensions.Configuration
- Microsoft.Extensions.Configuration.Json

---

## 2️⃣ SPRINTS - ROADMAP DEL PROYECTO

### ✅ SPRINT 0: Configuración Inicial (COMPLETADO)
- [x] Crear proyecto WPF .NET 9
- [x] Configurar Supabase client
- [x] Estructurar carpetas MVVM
- [x] Configurar appsettings.json
- [x] Crear modelos base

### ✅ SPRINT 1: Autenticación y Menú Principal (COMPLETADO)
**Objetivo:** Sistema de login y navegación principal

**Implementado:**
- [x] LoginWindow.xaml
  - Diseño moderno con logo
  - Campos usuario/contraseña
  - Validación con Supabase Auth
  - Mensajes de error
  
- [x] LoginViewModel.cs
  - Command LoginCommand
  - Integración SupabaseAuthHelper
  - Navegación a MenuPrincipal

- [x] MenuPrincipalWindow.xaml
  - Sidebar con 4 botones de navegación:
    1. 📊 Panel de Agregación
    2. 📅 Historial
    3. 💤 Inactivas
    4. 📈 Resumen Consolidado
  - Área de contenido dinámico
  - Header con nombre de usuario
  - Background: #F3F4F6

- [x] MenuPrincipalViewModel.cs
  - Commands de navegación
  - Gestión de UserControls dinámicos

**Estado:** 100% funcional y probado

---

### ✅ SPRINT 2: Panel de Agregación - CRUD Básico (100% COMPLETADO)
**Objetivo:** Crear, visualizar, editar, eliminar y gestionar entidades principales

**Implementado:**

#### AgregarWindow.xaml (Contenedor Principal)
- [x] Diseño con sidebar (220px) + área de contenido
- [x] Sidebar Background: `#202355`
- [x] 4 botones de navegación:
  - 🏠 Nueva Casa
  - 👤 Nuevo Dueño
  - 🏷️ Categoría Propiedad
  - 💰 Categoría Movimiento
- [x] Botón activo con Background `#F59E0B`
- [x] Carga dinámica de UserControls

#### 1. AgregarCasaControl.xaml (Formulario Completo)
**Campos:**
- [x] Nombre de la Casa (TextBox)
- [x] Dueño (ComboBox → tabla duenos)
- [x] Categoría (ComboBox → tabla categorias_propiedades)
- [x] Moneda (ComboBox: USD, CRC, EUR)
- [x] Estado (CheckBox: Casa Activa)
- [x] Imagen (FileDialog + Preview + Upload a Storage)
- [x] Notas (TextBox multiline)

**Diseño:**
- Card blanco con sombra
- Secciones compactas con emojis inline
- Sin cajas de iconos decorativas
- Separadores de 1px
- Espaciado reducido (12px entre secciones)
- Altura inputs: 38px
- Padding: 12px
- Botones: Limpiar (amarillo) + Guardar (verde)

**Code-behind:**
- [x] Guardar_Click → SupabaseCasaHelper.InsertarCasa()
- [x] Limpiar_Click → Resetear campos
- [x] SeleccionarImagen_Click → OpenFileDialog
- [x] CargarDuenos() → Llenar ComboBox
- [x] CargarCategorias() → Llenar ComboBox
- [x] Upload de imagen a Storage bucket "casas-imagenes"

**Estado:** ✅ Funcional, diseño optimizado

#### 2. AgregarDuenoControl.xaml
**Campos:**
- [x] Nombre (TextBox)
- [x] Apellido (TextBox)
- [x] Teléfono (TextBox - opcional)
- [x] Email (TextBox - opcional)

**Diseño:**
- Card con sombra
- 2 secciones: "Información Personal" + "Información de Contacto"
- Grid 2 columnas (Nombre/Apellido, Teléfono/Email)
- Iconos decorativos en secciones (32x32)
- Espaciado estándar (20px)
- Altura inputs: 45px
- Botones: Limpiar (amarillo) + Guardar (verde)

**Code-behind:**
- [x] Guardar_Click → SupabaseDuenoHelper.InsertarDueno()
- [x] Limpiar_Click → Resetear campos

**Estado:** ✅ Funcional

#### 3. AgregarCategoriaPropiedadControl.xaml
**Campos:**
- [x] Nombre (TextBox)
- [x] Descripción (TextBox multiline)

**Diseño:**
- Card simple con sombra
- 1 sección: "Información de Categoría"
- Iconos decorativos
- Espaciado estándar
- Botones: Limpiar (amarillo) + Guardar (verde)

**Code-behind:**
- [x] Guardar_Click → SupabaseCategoriaHelper.InsertarCategoria()
- [x] Limpiar_Click → Resetear campos

**Estado:** ✅ Funcional

#### 4. AgregarCategoriaMovimientoControl.xaml
**Campos:**
- [x] Nombre (TextBox)
- [x] Tipo (ComboBox: Ingreso/Gasto)
- [x] Descripción (TextBox multiline)

**Diseño:**
- Card con sombra
- 1 sección moderadamente reducida
- Espaciado: 15px
- Altura inputs: 38px
- Botones: Limpiar (amarillo) + Guardar (verde)

**Code-behind:**
- [x] Guardar_Click → SupabaseCategoriaMovimientoHelper.InsertarCategoria()
- [x] Limpiar_Click → Resetear campos

**Estado:** ✅ Funcional

#### Estilos Compartidos (Todos los Formularios)
**TextBox Template:**
```xaml
- Border con CornerRadius 6px
- ScrollViewer con VerticalAlignment="Center"
- Margin en lugar de Padding para centrado vertical
- Focus: BorderBrush #3B82F6, BorderThickness 2px
```

**ComboBox Template:**
```xaml
- ToggleButton personalizado
- ContentPresenter con Margin="12,0,30,0"
- ItemContainerStyle:
  - Padding 12,8
  - Hover: Background #EFF6FF
  - Selected: Background #DBEAFE
- Focus: BorderBrush #3B82F6, BorderThickness 2px
```

**Botones:**
```xaml
Limpiar:
- Background: #F59E0B
- Foreground: White
- Padding: 15,10 (Casas) / 25,14 (otros)
- MinWidth: 120px (Casas) / 180px (otros)
- FontSize: 13px (Casas) / 15px (otros)
- CornerRadius: 8px
- Hover: #D97706

Guardar:
- Background: #10B981
- Foreground: White
- Padding: 20,10 (Casas) / 35,14 (otros)
- MinWidth: 150px (Casas) / 220px (otros)
- FontSize: 14px (Casas) / 16px (otros)
- CornerRadius: 10px
- Hover: #059669
```

#### Helpers de Supabase (Data Layer)
- [x] SupabaseCasaHelper.cs
  - InsertarCasa(CasaSupabase casa)
  - ActualizarCasa(CasaSupabase casa)
  - ObtenerCasas(Guid usuarioId)
  
- [x] SupabaseDuenoHelper.cs
  - InsertarDueno(DuenoSupabase dueno)
  - ObtenerDuenos(Guid usuarioId)
  
- [x] SupabaseCategoriaHelper.cs
  - InsertarCategoria(CategoriaSupabase categoria)
  - ObtenerCategorias(Guid usuarioId)
  
- [x] SupabaseCategoriaMovimientoHelper.cs
  - InsertarCategoriaMovimiento(CategoriaMovimientoSupabase categoria)
  - ObtenerCategoriasMovimiento(Guid usuarioId)
  
- [x] SupabaseStorageHelper.cs
  - SubirImagen(byte[] imageBytes, string fileName)
  - ObtenerUrlPublica(string fileName)

**Pendiente en Sprint 2:**
- [x] Crear bucket "casas-imagenes" en Supabase Storage ✅ (CasasFotos)
- [x] Implementar ventanas de Edición/Eliminación con DataGrid ✅
- [x] Validaciones avanzadas (campos requeridos, formatos) ✅
- [x] Mensajes de confirmación con diseño personalizado ✅

**Progreso:** 100% completado ✅

**Archivos creados/modificados en Sprint 2:**
- Views/CustomMessageBox.xaml y .cs - Diálogos personalizados
- Views/GestionWindow.xaml y .cs - Ventana principal de gestión
- Views/EditarDuenoWindow.xaml y .cs - Edición de dueños
- Views/EditarCasaWindow.xaml y .cs - Edición de casas
- Views/Controls/GestionDuenosControl.xaml y .cs - Grid dueños
- Views/Controls/GestionCasasControl.xaml y .cs - Grid casas
- Views/Controls/GestionCategoriasControl.xaml y .cs - Grid categorías
- Views/Controls/GestionCategoriasMovimientosControl.xaml y .cs - Grid cat. movimientos
- Data/SupabaseStorageHelper.cs - Actualizado para bucket CasasFotos
- Data/SupabaseDuenoHelper.cs - Agregado ObtenerDuenosAsync()
- Data/SupabaseCasaHelper.cs - Agregado ObtenerCasasAsync()
- Data/SupabaseCategoriaHelper.cs - Agregados métodos CRUD completos
- Data/SupabaseCategoriaMovimientoHelper.cs - Agregados métodos CRUD completos
- Views/MenuPrincipalWindow.xaml y .cs - Agregado botón "⚙️ Gestión"
- Todos los formularios actualizados con CustomMessageBox y validaciones mejoradas

---

### ✅ SPRINT 3: Panel de Historial y Detalle de Casa (100% COMPLETADO)
**Objetivo:** Visualizar y gestionar movimientos financieros, historial de auditoría y detalles completos de cada casa

**Implementado:**

#### DetalleCasaWindow.xaml (Ventana de Detalle Completo)
- [x] Sistema de pestañas (Resumen, Movimientos, Detalles, Notas, Fotos)
- [x] Pestaña Resumen:
  - Estado general con balance actual
  - Últimos 5 movimientos
  - Detalle mensual con filtrado por hoja mensual
  - Gráfico de salud financiera (indicador visual)
- [x] Pestaña Movimientos:
  - DataGrid completo con filtros (tipo, búsqueda)
  - Agregar, editar, eliminar movimientos
  - Vista de ingresos y gastos
- [x] Pestaña Detalles:
  - Timeline anual con balance mes a mes
  - Evolución del balance usando hojas mensuales
- [x] Pestaña Notas:
  - Layout de 3 tarjetas por fila (WrapPanel)
  - Agregar, editar, eliminar notas
  - Cards compactas (220px) con fecha y contenido
- [x] Pestaña Fotos:
  - Galería de fotos con grid responsive
  - Subir y eliminar fotos
  - Vista previa de imágenes

#### HistorialWindow.xaml (Sistema de Auditoría)
- [x] Sistema de pestañas (Casas, Movimientos, Dueños, Categorías)
- [x] DataGrid de auditoría con columnas:
  - Usuario, Módulo, Acción, Entidad, Descripción, Fecha
- [x] Filtros:
  - Por usuario (ComboBox)
  - Por módulo (ComboBox)
  - Por tipo de acción (ComboBox)
  - Por rango de fechas (DatePickers)
- [x] Paginación:
  - 20 registros por página
  - Navegación con botones Anterior/Siguiente
  - Indicador de página actual
- [x] Botón "Deshacer" para movimientos:
  - Restaura estado anterior desde datos_anteriores JSONB
  - Confirmación antes de ejecutar
- [x] Registro automático en:
  - Crear/Editar/Eliminar casas
  - Crear/Editar/Eliminar movimientos
  - Activar/Desactivar casas

#### Modelos Nuevos
- [x] Movimiento.cs / MovimientoSupabase.cs
- [x] Nota.cs / NotaSupabase.cs (PrimaryKey autoincrement)
- [x] Foto.cs / FotoSupabase.cs
- [x] RegistroAuditoria.cs
- [x] Propiedad.cs (modelo extendido para UI con AlertaFinanciera, ColorAlerta, MostrarAlerta)

#### Helpers Nuevos
- [x] SupabaseMovimientoHelper.cs
  - ObtenerMovimientosPorCasaAsync() con filtrado por hoja_mensual_id
  - ObtenerBalanceCasaAsync() con cálculo correcto (Ingreso - Math.Abs(Gasto))
  - ObtenerMovimientosPorMesAsync() usando hojas mensuales
  - InsertarMovimientoAsync(), ActualizarMovimientoAsync(), EliminarMovimientoAsync()
  - Todos los tipos comparados como "Ingreso"/"Gasto" (case-sensitive)
- [x] SupabaseHojaMensualHelper.cs
  - ObtenerHojaPorPeriodoAsync() con múltiples Where() para Supabase
  - ObtenerMesesDisponiblesAsync()
  - CrearHojaMensualAsync()
- [x] SupabaseNotaHelper.cs
  - InsertarNotaAsync(), ActualizarNotaAsync(), EliminarNotaAsync()
  - ObtenerNotasPorCasaAsync()
  - Configurado con PrimaryKey(false) para autoincrement
- [x] SupabaseFotoHelper.cs
  - InsertarFotoAsync(), EliminarFotoAsync()
  - ObtenerFotosPorCasaAsync()
- [x] SupabaseAuditoriaHelper.cs
  - RegistrarAuditoriaAsync() con JSONB para datos anteriores/nuevos
  - ObtenerAuditoriasAsync() con filtros y paginación
  - ObtenerUsuariosDistintosAsync(), ObtenerModulosDistintosAsync()

#### Indicadores de Salud Financiera
- [x] Badge en MenuPrincipal para cada casa:
  - 🔴 CRÍTICO: Balance ≤ ₡0
  - 🟡 ATENCIÓN: Balance entre ₡1 y ₡1,000
  - Sin badge: Balance > ₡1,000 (saludable)
- [x] Modelo Propiedad con propiedades:
  - AlertaFinanciera (string): "CRÍTICO" o "ATENCIÓN"
  - ColorAlerta (string): "#DC2626" (rojo) o "#F59E0B" (amarillo)
  - MostrarAlerta (bool): Computed property

#### Correcciones Críticas
- [x] Tipo de movimiento: "Ingreso" y "Gasto" (no "ingreso"/"egreso")
- [x] Balance calculation: totalIngresos - Math.Abs(totalEgresos)
  - Gastos almacenados como negativos en DB
- [x] Filtrado por hoja_mensual_id en lugar de fecha
- [x] NotaSupabase PrimaryKey("id", false) para autoincrement
- [x] Sequence reset script para notas_casa

**Progreso:** 100% completado ✅

**Estado:** Sistema de detalle de casa, historial de auditoría y alertas financieras completamente funcionales

---

### 🔄 SPRINT 4: Tutorial del Sistema (100% COMPLETADO)
**Objetivo:** Proporcionar guía completa para nuevos usuarios

**Implementado:**
- [x] TutorialWindow.xaml - Ventana de tutorial
- [x] TutorialControl.xaml - Contenido del tutorial con:
  - Introducción al sistema
  - Guía de módulo Panel Principal
  - Guía de módulo Gestión
  - Guía de módulo Panel de Agregación
  - Guía de módulo Detalle de Casa
  - Guía de módulo Historial
  - Consejos de uso
  - Créditos del equipo:
    - 💻 Programador Principal: Steven Venegas
    - 🤝 Equipo: Andrés, Felipe, Daniela
- [x] Botón "📚 Tutorial" en MenuPrincipalWindow
- [x] Comando AbrirTutorialCommand en MenuPrincipalViewModel

**Progreso:** 100% completado ✅

---

### 🔄 SPRINT 5: Panel de Casas Inactivas (100% COMPLETADO)
**Objetivo:** Gestionar propiedades desactivadas

**Implementado:**
- [x] InactivasWindow.xaml - Ventana de casas inactivas
- [x] DataGrid con casas inactivas (activa = false)
- [x] Botón: Reactivar Casa
- [x] Botón: Ver Historial
- [x] Filtros por Dueño y Categoría
- [x] SupabaseCasaHelper:
  - ActivarCasaAsync(int casaId)
  - DesactivarCasaAsync(int casaId)
  - ObtenerCasasInactivasAsync()

**Progreso:** 100% completado ✅

---

### 🔄 SPRINT 6: Panel de Resumen Consolidado (PENDIENTE)
  - Filtros por:
    - Casa
    - Rango de fechas
    - Tipo (Ingreso/Gasto)
    - Categoría
  - Botones: Agregar Movimiento, Editar, Eliminar
  - Totales: Ingresos, Gastos, Balance

- [ ] AgregarMovimientoWindow.xaml
  - ComboBox Casa
  - ComboBox Categoría Movimiento
  - TextBox Monto
  - DatePicker Fecha
  - TextBox Descripción
  - CheckBox Recurrente

- [ ] SupabaseMovimientoHelper.cs
  - InsertarMovimiento()
  - ActualizarMovimiento()
  - EliminarMovimiento()
  - ObtenerMovimientosPorCasa()
  - ObtenerMovimientosPorFecha()

**Modelo de Datos:**
```csharp
public class Movimiento
{
    public Guid Id { get; set; }
    public Guid CasaId { get; set; }
    public Guid CategoriaMovimientoId { get; set; }
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; } // "Ingreso" o "Gasto"
    public string Descripcion { get; set; }
    public bool EsRecurrente { get; set; }
    public Guid UsuarioId { get; set; }
**Progreso:** 100% completado ✅

---

### 🔄 SPRINT 6: Panel de Resumen Consolidado (PENDIENTE)
**Objetivo:** Dashboard con KPIs y gráficos

**Funcionalidades a Implementar:**
- [ ] ResumenConsolidadoControl.xaml
  - Cards con métricas:
    - Total Casas Activas
    - Total Ingresos del Mes
    - Total Gastos del Mes
    - Balance Neto
    - Casa con Mayor Ingreso
    - Casa con Mayor Gasto
  
  - Gráficos (LiveCharts o similar):
    - Gráfico de barras: Ingresos vs Gastos por mes
    - Gráfico de pastel: Distribución de gastos por categoría
    - Gráfico de líneas: Evolución del balance

- [ ] Filtros:
  - Rango de fechas
  - Por Casa específica
  - Por Categoría

**Librerías Sugeridas:**
- LiveCharts2 para WPF
- ScottPlot

**Prioridad:** Media-Baja

---

### 🔄 SPRINT 6: Mejoras UX y Validaciones (PENDIENTE)
**Objetivo:** Pulir experiencia de usuario

**Tareas:**
- [ ] Implementar validaciones visuales en tiempo real
  - Campos requeridos con borde rojo
  - Mensajes de error bajo los inputs
  - Validación de formato email
  - Validación de formato teléfono

- [ ] Diálogos personalizados
  - Confirmación de eliminación
  - Confirmación de guardado exitoso
  - Alertas de error

- [ ] Loading states
  - Spinners durante operaciones async
  - Deshabilitar botones mientras se procesa

- [ ] Búsqueda y autocompletado
  - Búsqueda en tiempo real en DataGrids
  - Autocompletado en ComboBox

**Prioridad:** Media

---

### 🔄 SPRINT 7: Reportes y Exportación (FUTURO)
**Objetivo:** Generar reportes en PDF/Excel

**Funcionalidades:**
- [ ] Reporte de Ingresos/Gastos por Casa
- [ ] Reporte Consolidado Mensual
- [ ] Exportar a Excel
- [ ] Exportar a PDF
- [ ] Imprimir reportes

**Librerías Sugeridas:**
- iTextSharp para PDF
- EPPlus para Excel

**Prioridad:** Baja

---

### 🔄 SPRINT 8: Gestión de Usuarios (FUTURO)
**Objetivo:** Panel de administración de usuarios

**Funcionalidades:**
- [ ] Ver lista de usuarios
- [ ] Crear/Editar/Eliminar usuarios
- [ ] Asignar roles (Admin/Usuario)
- [ ] Activar/Desactivar usuarios
- [ ] Registro de actividad

**Prioridad:** Baja

---

## 3️⃣ PROMPT INICIAL PARA NUEVA CONVERSACIÓN

**IMPORTANTE:** Copia y pega exactamente este prompt al inicio de la nueva conversación con GitHub Copilot:

---

```
Hola Copilot, voy a continuar trabajando en el proyecto FlujoCajaWpf. 

ANTES DE HACER CUALQUIER COSA, lee completamente el archivo promptCopilot.md que está en la raíz del proyecto FlujoCajaWpf. Este archivo contiene:
1. Todo el contexto técnico del proyecto
2. El estado actual de cada Sprint
3. La arquitectura y estructura completa
4. El sistema de diseño visual
5. Los sprints completados y pendientes

Una vez que hayas leído y comprendido toda la información del archivo promptCopilot.md, confirma que estás listo para continuar trabajando indicando:
- Qué Sprint está actualmente en progreso
- Cuál es el siguiente Sprint a implementar
- Un breve resumen del estado actual del proyecto

NO IMPLEMENTES NADA TODAVÍA. Solo confirma que has leído y comprendido el contexto del proyecto.

Esperando tu confirmación...
```

---

### 📝 Notas Importantes para la Continuación

**Estado Actual del Proyecto (Última Actualización: Enero 2026):**
- Sprint 1: ✅ 100% Completado (Autenticación y Menú Principal)
- Sprint 2: ✅ 100% Completado (CRUD Básico y Gestión)
- Sprint 3: ✅ 100% Completado (Historial, Auditoría y Detalle de Casa)
- Sprint 4: ✅ 100% Completado (Tutorial del Sistema)
- Sprint 5: ✅ 100% Completado (Panel de Casas Inactivas)
- Siguiente: Sprint 6 (Panel de Resumen Consolidado - Dashboard KPIs)

**Características Principales Implementadas:**
1. ✅ Sistema de autenticación con Supabase Auth
2. ✅ CRUD completo de Casas, Dueños, Categorías
3. ✅ Gestión de movimientos con hojas mensuales
4. ✅ Sistema de auditoría con paginación y filtros
5. ✅ Detalle completo de casas con 5 pestañas
6. ✅ Indicadores de salud financiera con badges
7. ✅ Notas y fotos por casa
8. ✅ Tutorial integrado con créditos del equipo
9. ✅ Panel de casas inactivas

**Decisiones de Diseño Aplicadas:**
1. ✅ Todos los textos en color negro
2. ✅ Botón Limpiar en amarillo (#F59E0B) en todos los formularios
3. ✅ Sidebar con fondo azul oscuro (#202355)
4. ✅ Formularios responsive con cards y sombras
5. ✅ Badges de alerta financiera: 🔴 CRÍTICO (≤₡0), 🟡 ATENCIÓN (₡1-₡1,000)
6. ✅ Timeline anual con evolución del balance
7. ✅ WrapPanel de 3 notas por fila (220px cada una)
8. ✅ Panel de Agregación eliminado - Todo CRUD en módulo Gestión
9. ✅ Sistema de categorías flexible en movimientos:
   - ComboBox editable (IsEditable=true) permite escribir texto libre
   - CheckBox opcional "💾 Guardar categoría para uso futuro"
   - Categorías ad-hoc: se usan una vez sin guardar en BD
   - Categorías guardadas: aparecen en ComboBox para usos futuros

**Convenciones de Código:**
- Async/await para todas las operaciones Supabase
- Tipos de movimiento: "Ingreso" y "Gasto" (case-sensitive)
- Gastos almacenados como negativos en DB, usar Math.Abs() en cálculos
- Filtrado por hoja_mensual_id, no por fecha directamente
- PrimaryKey(false) para campos autoincrement
- JSONB para datos_anteriores y datos_nuevos en auditoría
- try-catch con MessageBox.Show para errores
- Usuario actual obtenido de SupabaseAuthHelper.ObtenerUsuarioActual()
- Navegación mediante UserControls dinámicos en ContentControl

**Configuración Requerida (Manual):**
1. Ejecutar InitDatabase_v2.sql en Supabase SQL Editor
2. Crear bucket "CasasFotos" en Supabase Storage (si no existe)
3. Crear bucket "FotosCasas" en Supabase Storage para galería adicional
4. Configurar políticas de Storage para permitir upload/read públicos
5. Verificar que todas las tablas tengan RLS (Row Level Security) configurado
6. Crear usuarios en Supabase Dashboard → Authentication → Users

**Tablas Principales Implementadas:**
- duenos, categorias, casas, categorias_movimientos
- hojas_mensuales (cierres mensuales)
- movimientos (con auditoría de usuario_creador_id, usuario_modificador_id)
- notas_casa (SERIAL autoincrement)
- fotos_casa (SERIAL autoincrement)
- auditoria (con JSONB para datos anteriores/nuevos)

**Comandos de Desarrollo:**
```bash
# Compilar
dotnet build

# Ejecutar
dotnet run

# Limpiar
dotnet clean

# Restaurar paquetes
dotnet restore
```

**Archivos Clave a Revisar:**
- `appsettings.json` - Credenciales Supabase
- `Data/SupabaseHelper.cs` - Cliente Supabase inicializado
- `Views/MenuPrincipalWindow.xaml` - Navegación principal
- `Views/AgregarWindow.xaml` - Contenedor CRUD
- `Views/Controls/Agregar*.xaml` - Formularios individuales

---

## 🎯 OBJETIVO FINAL DEL PROYECTO

Crear una aplicación de escritorio WPF robusta y moderna para la gestión completa del flujo de caja de múltiples propiedades inmobiliarias, con las siguientes capacidades:

1. **Autenticación segura** mediante Supabase Auth
2. **CRUD completo** de entidades (Casas, Dueños, Categorías, Movimientos)
3. **Gestión de imágenes** en la nube (Storage)
4. **Historial detallado** de movimientos financieros
5. **Dashboard con KPIs** y visualizaciones
6. **Reportes exportables** (PDF/Excel)
7. **Gestión de usuarios** multi-tenant
8. **Diseño moderno y responsive**

**Público objetivo:** Administradores de propiedades, inversores inmobiliarios, propietarios con múltiples inmuebles.

---

## 📞 SOPORTE Y RECURSOS

**Documentación Oficial:**
- WPF: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/
- Supabase C#: https://supabase.com/docs/reference/csharp/introduction
- .NET 9: https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9

**Stack Overflow Tags:**
- #wpf
- #dotnet
- #supabase
- #csharp

---

**FIN DEL DOCUMENTO**

Este archivo debe servir como referencia única y completa para retomar el desarrollo del proyecto en cualquier momento. Actualízalo conforme avances en los Sprints.
