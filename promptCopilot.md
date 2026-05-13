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

### ✅ SPRINT 6: Panel de Resumen Consolidado (COMPLETADO)
**Objetivo:** Dashboard con KPIs y gráficos

**Implementado:**
- ResumenConsolidadoWindow.xaml / ResumenConsolidadoControl.xaml con cards de métricas y filtros básicos.
- Filtros por Casa, rango de fechas y tipo de movimiento.
- Totales de Ingresos, Gastos y Balance.
- Gráficos de comparación de Ingresos vs Gastos y evolución del balance.
- Integración con `SupabaseMovimientoHelper` para cargar movimientos y balances.
- Ventana de reporte generada desde `DetalleCasaWindow`.

**Notas:**
- El panel de resumen está disponible en el menú principal.
- Se mantiene la arquitectura MVVM y el estilo visual del proyecto.

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

### ✅ SPRINT 7: Reportes, Exportación y Correo (EN PROGRESO - ~90%)
**Objetivo:** Generar reportes en PDF/Excel, enviarlos por correo y registrar logs de envío

---

#### ✅ Historia 16: Generar reporte mensual en pantalla (CERRADA)
- [x] `ResumenConsolidadoWindow.xaml/.cs` — Dashboard con KPIs (total casas, balances USD/CRC, DataGrid por casa)
- [x] `ReporteFormatoDialog.xaml/.cs` — Diálogo selector de formato (PDF / Excel)
- [x] Filtros de año y mes en ResumenConsolidado
- [x] `BalanceMensual.cs` — Modelo para cálculo de balance por período
- [x] Integrado en `MenuPrincipalWindow` como opción de navegación

**Librerías instaladas:** `QuestPDF v2024.12.4`, `ClosedXML v0.102.3`, `LiveChartsCore v2.0.0-rc6.1`

---

#### ✅ Historia 17: Exportar reporte en PDF y Excel (CERRADA)
- [x] `ReporteService.cs` (Services/) con:
  - `GenerarPdfAsync()` — PDF con tabla de movimientos, resumen de totales, colores y header corporativo (QuestPDF)
  - `GenerarExcelAsync()` — Excel con cabeceras formateadas, filas alternadas, hipervínculos a imágenes, resumen final (ClosedXML)
  - `DatosReporte` record con CasaNombre, DuenoNombre, Moneda, MesAnio, Movimientos
- [x] Flujo en `DetalleCasaWindow.xaml.cs`:
  - Botón "📄 Reporte" → `ReporteFormatoDialog` → `SaveFileDialog` → Generar → Abrir archivo
  - Luego ofrece `EnviarReporteDialog` para envío por correo
- [x] Columna "Imagen adjunta" en Excel como hipervínculo a URL de Supabase Storage

---

#### ✅ Historia 18: Envío de reportes por correo electrónico (IMPLEMENTADO)
- [x] `EmailService.cs` (Services/) — Servicio de envío vía SDK oficial de **Resend** (`Resend v0.2.2`):
  - `EnviarAsync()` con destinatarios múltiples y adjuntos
  - `EsEmailValido()` con regex compilado
  - `CrearAttachment()` verificando existencia del archivo
- [x] `EnviarReporteDialog.xaml/.cs` — Diálogo completo de envío:
  - Lista de destinatarios con checkboxes (email principal + correos adicionales de BD)
  - Agregar correo nuevo con opción de guardar en BD
  - Vista previa de asunto y cuerpo HTML
  - Indicador de archivos adjuntos (PDF y/o Excel)
  - Estado de envío con mensajes de éxito/error
- [x] `EnviarReporteViewModel.cs` — ViewModel MVVM completo con:
  - `CorreoItem` seleccionable (ObservableCollection)
  - Preview dinámico de destinatarios, asunto y mensaje
  - Confirmación antes de enviar
  - `BuildHtmlBody()` — Plantilla HTML corporativa con colores del sistema
- [x] `SupabaseCorreoCasaHelper.cs` — CRUD de correos por casa:
  - `ObtenerCorreosPorCasaAsync()`, `InsertarCorreoAsync()`, `EliminarCorreoAsync()`
  - `ActualizarEmailPrincipalAsync()` — Actualiza email_principal en tabla casas
- [x] `CorreoCasaSupabase.cs` — Modelo tabla `correos_casa`
- [x] `Casa.cs` / `CasaSupabase.cs` — Propiedad `EmailPrincipal` mapeada a columna `email_principal`
- [x] `Scripts/06_agregar_email_casas_y_tabla_correos.sql` — Migración: columna `email_principal` en casas + tabla `correos_casa` con RLS
- [x] `appsettings.json` — Sección `Resend` con `ApiKey` y `From` configurados
- [x] `HistorialWindow.xaml/.cs` — Interfaz de historial de envíos de correo con filtros y paginación

**Pendiente de validación:**
- [ ] Prueba end-to-end real de envío con Resend
- [ ] Confirmar que el dominio `From` esté verificado para producción
- [ ] Verificar el flujo con email del dueño y correos adicionales

---

#### ✅ Historia 19: Registrar log de envíos de reportes (IMPLEMENTADO)
- [x] `Scripts/08_create_log_correos_table.sql` — Script de creación de la tabla `log_correos`
- [x] `Models/LogCorreoSupabase.cs` — Modelo Supabase para log de correos
- [x] `Data/SupabaseLogCorreoHelper.cs` — Helper de logs de correo con registro y consulta
- [x] `Views/HistorialWindow.xaml/.cs` — Pestaña e interfaz de historial de correos con filtros por usuario, casa y estado
- [x] `EnviarReporteViewModel.EnviarAsync()` — Registra envíos exitosos y errores en el log
- [x] `SupabaseLogCorreoHelper.ObtenerLogsAsync()` — Consulta con paginación y filtros
- [x] `SupabaseLogCorreoHelper.ObtenerUsuariosAsync()` / `ObtenerCasasAsync()` — Filtros dinámicos para el historial

**Notas:**
- El log de envíos de correo ya está integrado en `HistorialWindow`.
- La funcionalidad de almacenamiento de logs está implementada y solo requiere revisión final de datos.

**Prioridad:** Alta (esta pieza ya está en código, resta validar y cerrar Sprint 7)

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

### 🔄 SPRINT 9: Facturas IA y Privacidad de Reportes (PENDIENTE)
**Objetivo:** Integrar carga de facturas con IA, revisión humana y reportes PDF privados manteniendo el estilo actual.

**Parte 1 — Privacidad y reportes PDF**
1. Cambiar el bucket de fotos/facturas a privado en Supabase Storage. No usar URL pública directa.
2. Crear o actualizar políticas RLS y reglas de storage para que solo el cliente autenticado y los procesos autorizados puedan leer las facturas.
3. Agregar soporte de URLs firmadas/autenticadas en `SupabaseStorageHelper.cs` para descarga temporal desde la app.
4. Modificar la generación de reportes para eliminar la exportación directa a Excel. Solo se generará PDF.
5. Mantener el flujo de datos de la tabla de la app y generar un PDF de tabla + PDF de fotos (o un solo PDF de fotos con tabla incluida). Cada movimiento debe:
   - ir numerado
   - coincidir con el orden por fecha de la tabla
   - mostrar los datos principales de la app
   - mostrar la imagen debajo en el mismo orden
6. Actualizar `ReporteService.cs` para soportar:
   - PDF de tabla de movimientos con formato corporativo
   - PDF de fotos con plantilla tabular + galería ordenada
   - URLs firmadas de imágenes privadas cuando se renderiza el PDF dentro de la app
7. Adaptar `DetalleCasaWindow.xaml.cs` y botones de reporte:
   - `Generar reporte PDF` (cambia de Excel+PDF a solo PDF)
   - `Generar reporte de fotos` o `PDF de facturas` con plantilla de imágenes
8. Documentar los cambios en Supabase, incluída la creación del bucket privado y el uso de service_role para n8n si es necesario.

**Parte 2 — Movimientos IA**
1. Cambiar pestañas de `DetalleCasaWindow` a: `Resumen`, `Movimientos`, `Movimientos IA`, `Detalles`.
2. Crear en UI una nueva pestaña `Movimientos IA` basada en la estructura actual de movimientos.
3. Agregar en esa pestaña botones superiores:
   - `Seleccionar todos`
   - `Borrar seleccionados`
   - `Aceptar seleccionados`
   - `Importar facturas IA`
4. Añadir checkbox por fila para selección individual y mantener el estado seleccionado en la tabla.
5. Crear ventana emergente/modal con dos áreas drag & drop:
   - una para `Ingresos`
   - otra para `Egresos`
   - cada área debe permitir también seleccionar carpeta/archivos vía explorador
6. Al soltar archivos:
   - subir cada factura al bucket privado de `facturas`
   - enviar a n8n el archivo y metadatos (casaId, mes, año, tipo sugerido)
   - recibir datos parseados: fecha, monto, categoría, descripción, proveedor, tipo_movimiento, raw_json
7. Guardar los resultados en una tabla temporal de revisión tipo `movimientos_ia_temporales` o `facturas_pendientes` con campos:
   - `casaid`, `hoja_mensual_id`, `mes`, `anio`
   - `fecha`, `monto`, `categoria`, `descripcion`, `tipo_movimiento`
   - `factura_url`, `estado`, `raw_json`, `usuario_creador`, `fecha_creacion`
8. Mostrar en la pestaña IA los resultados automáticos:
   - filas normales para lecturas correctas
   - filas con sombra roja cuando el parseo es parcial o falla
   - imagen adjunta visible
   - botón `Reintentar` en cada fila para volver a enviar sólo esa factura a n8n
   - botón `Cambiar foto` para reemplazar el archivo y reintentar
9. Agregar validaciones visuales con colores y mensajes de error si faltan campos importantes.
10. Al aceptar uno o varios movimientos:
    - crear entradas en `movimientos` usando la hoja mensual correcta
    - marcar los movimientos originales como `IA` con un detalle pequeño (por ejemplo `Descripcion += " (IA)"` o campo `Origen`)
    - actualizar la fila temporal a `aprobado` y guardar `usuario_aprobo`/`fecha_aprobacion`
11. Permitir eliminar los temporales seleccionados con `Borrar seleccionados`.
12. Mantener el diseño actual: botones amarillos para acciones, verdes para aceptar; cards blancas con sombras; tipografía y esquema de colores consistentes.

**Parte 3 — Panel de usuarios administrador**
1. Agregar botón `Agregar Usuarios` en `MenuPrincipalWindow.xaml` visible solo para el usuario administrador.
2. Detectar el rol admin desde el usuario actual y el DTO `Usuario` o el registro de `usuarios`.
3. Crear una nueva ventana/pestaña `Usuarios` con:
   - lista de usuarios existentes
   - botón `Nuevo usuario`
   - acción `Enviar correo de cambio de contraseña`
   - acción `Eliminar usuario`
4. Implementar la creación de usuario usando Supabase Auth y la tabla `usuarios`:
   - crear auth user en Supabase
   - guardar metadata local (`nombre`, `email`, `rol`, `activo`)
   - enviar correo con Resend o usar el workflow de reseteo de Supabase si está disponible
5. Implementar la eliminación/desactivación de usuarios usando `activo = false` y/o Supabase Auth API si se necesita eliminar del auth.
6. Restringir el acceso de esta ventana solo a admin y mantener el botón oculto para usuarios normales.
7. Mantener el estilo visual actual y soportar modo claro/oscuro.

**Parte 4 — Cambios específicos en Supabase / Resend / n8n**
1. Supabase Storage:
   - convertir bucket de facturas/fotos a privado
   - validar que `CasasFotos` y cualquier bucket nuevo tengan RLS correctas
   - crear bucket privado `facturas` o `facturas_ia`
2. Resend:
   - configurar `From` con un dominio verificado
   - usar Resend para correos de cambio de contraseña
   - en appsettings.json guardar la sección Resend actualizada
3. n8n:
   - crear webhook seguro con token
   - configurar nodo que descargue facturas desde Supabase con service_role o credenciales seguras
   - procesar OCR/IA y devolver JSON normalizado
   - opcional: insertar directamente en la tabla temporal si lo quieres automatizar
4. Documentar todas las claves de servicio, endpoints y buckets nuevos en el README y `promptCopilot.md`.

**Notas de ejecución:**
- Usar nuevos helpers en `Data/` para no mezclar lógica con los helpers existentes.
- Crear nuevas vistas/controles específicos para `Movimientos IA` y `Usuarios`.
- No romper el flujo actual de `DetalleCasaWindow` ni la navegación de `MenuPrincipalWindow`.
- Mantener consistencia de colores, tarjetas y estilo de botones como en el resto del sistema.

**Prioridad:** Alta

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

**Estado Actual del Proyecto (Última Actualización: Marzo 2026):**
- Sprint 1: ✅ 100% Completado (Autenticación y Menú Principal)
- Sprint 2: ✅ 100% Completado (CRUD Básico y Gestión)
- Sprint 3: ✅ 100% Completado (Historial, Auditoría y Detalle de Casa)
- Sprint 4: ✅ 100% Completado (Tutorial del Sistema)
- Sprint 5: ✅ 100% Completado (Panel de Casas Inactivas)
- Sprint 6: ✅ 100% Completado (ResumenConsolidadoWindow con KPIs y filtros)
- Sprint 7: 🔄 ~75% En Progreso:
  - Historia 16 (Reporte en pantalla): ✅ CERRADA
  - Historia 17 (Exportar PDF/Excel): ✅ CERRADA
- Historia 18 (Envío por correo): ✅ Implementado, falta validación end-to-end
- Historia 19 (Log de envíos): ✅ Implementado en código y en UI; revisar datos
- Siguiente: Validar el log de envíos y completar Sprint 7

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
- duenos, categorias, casas (con columna `email_principal`), categorias_movimientos
- hojas_mensuales (cierres mensuales)
- movimientos (con auditoría de usuario_creador_id, usuario_modificador_id)
- notas_casa (SERIAL autoincrement)
- fotos_casa (SERIAL autoincrement)
- auditoria (con JSONB para datos anteriores/nuevos)
- correos_casa (correos adicionales por casa para envío de reportes)
- preferencias_usuario (modo oscuro por usuario)
- **PENDIENTE:** log_envios_reportes (historial de envíos de correo — Historia 19)

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
