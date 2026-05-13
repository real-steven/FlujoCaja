# 🏠 Sistema de Flujo de Caja — Samara Rentals

Aplicación de escritorio para la **gestión financiera de propiedades de alquiler**. Permite controlar ingresos, gastos y balances de múltiples casas, con análisis automático de facturas mediante Inteligencia Artificial, generación de reportes en PDF/Excel y envío de reportes por correo electrónico.

---

## 📋 Índice

1. [¿Qué hace el sistema?](#-qué-hace-el-sistema)
2. [Tecnologías utilizadas](#-tecnologías-utilizadas)
3. [Arquitectura del proyecto](#-arquitectura-del-proyecto)
4. [Módulos y pantallas](#-módulos-y-pantallas)
5. [Base de datos (Supabase)](#-base-de-datos-supabase)
6. [Módulo de IA con n8n](#-módulo-de-ia-con-n8n)
7. [Almacenamiento de archivos](#-almacenamiento-de-archivos)
8. [Envío de correos (Resend)](#-envío-de-correos-resend)
9. [Autenticación y roles](#-autenticación-y-roles)
10. [Configuración del proyecto](#-configuración-del-proyecto)
11. [Estructura de archivos](#-estructura-de-archivos)
12. [Requisitos del sistema](#-requisitos-del-sistema)

---

## 🎯 ¿Qué hace el sistema?

El sistema está diseñado para propietarios y administradores de propiedades de alquiler que necesitan:

- **Centralizar** el control financiero de múltiples casas en una sola aplicación
- **Registrar** ingresos y gastos mensualmente por propiedad
- **Importar facturas** automáticamente usando IA para extraer datos (monto, fecha, descripción)
- **Generar reportes** en PDF con resumen de movimientos y enviarlos por correo
- **Llevar auditoría** de todos los cambios realizados en el sistema
- **Gestionar usuarios** con diferentes niveles de acceso (admin / usuario)

### Principales funcionalidades

| Funcionalidad | Descripción |
|---|---|
| Gestión de casas | Crear, editar y desactivar propiedades con moneda propia (USD/CRC) |
| Movimientos financieros | Ingresos y gastos por mes, con categorías personalizadas |
| Importación IA | Sube una foto de factura → la IA extrae los datos automáticamente |
| Reportes PDF | Genera tabla de movimientos mensual lista para enviar por correo |
| Auditoría completa | Historial de todos los cambios con opción de deshacer |
| Administración | Panel de gestión de usuarios con control de roles y accesos |
| Resumen global | Dashboard con KPIs consolidados de todas las propiedades |

---

## 💻 Tecnologías utilizadas

### Aplicación

| Tecnología | Versión | Uso |
|---|---|---|
| **WPF** (Windows Presentation Foundation) | .NET 9.0 | Framework de interfaz gráfica |
| **C#** | 12 | Lenguaje principal |
| **XAML** | — | Definición de interfaces |

### Backend y Base de datos

| Tecnología | Versión | Uso |
|---|---|---|
| **Supabase** | 1.1.1 | Backend completo (DB, Auth, Storage) |
| **PostgreSQL** | (via Supabase) | Base de datos relacional |
| **Supabase Auth** | — | Autenticación de usuarios |
| **Supabase Storage** | — | Almacenamiento de imágenes y facturas |

### Servicios externos

| Servicio | Uso |
|---|---|
| **n8n** | Orquestación del flujo de análisis IA de facturas |
| **Groq (Llama 4 Scout)** | Modelo de IA con visión que analiza imágenes de facturas vía n8n |
| **Resend** | Envío de correos electrónicos (reportes y contraseñas temporales) |

### Paquetes NuGet

| Paquete | Versión | Uso |
|---|---|---|
| `Supabase` | 1.1.1 | SDK de Supabase para .NET |
| `QuestPDF` | 2024.12.4 | Generación de reportes PDF |
| `ClosedXML` | 0.102.3 | Generación de reportes Excel |
| `LiveChartsCore.SkiaSharpView.WPF` | 2.0.0-rc6.1 | Gráficos interactivos |

---

## 🏗️ Arquitectura del proyecto

El proyecto sigue el patrón **MVVM** (Model-View-ViewModel) con code-behind donde sea necesario para operaciones complejas de UI.

```
┌─────────────────────────────────────────────────────┐
│                    PRESENTACIÓN                      │
│   Views/ (XAML + code-behind)   ViewModels/          │
│   LoginWindow, DetalleCasaWindow, GestionWindow...   │
└────────────────────┬────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────┐
│                   LÓGICA DE DATOS                    │
│   Data/ (Helpers de Supabase)                        │
│   SupabaseCasaHelper, SupabaseMovimientoHelper...    │
└────────────────────┬────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────┐
│                 BACKEND (SUPABASE)                   │
│   PostgreSQL  |  Auth  |  Storage                    │
│               +                                      │
│   n8n Webhook → IA (Groq) → JSON de factura          │
└─────────────────────────────────────────────────────┘
```

### Patrón Repository (Data Layer)

| Helper | Responsabilidad |
|---|---|
| `SupabaseHelper` | Inicialización del cliente Supabase |
| `SupabaseAuthHelper` | Login, logout, sesión, cambio de contraseña |
| `SupabaseAdminHelper` | Crear/editar/desactivar usuarios (solo admin) |
| `SupabaseCasaHelper` | CRUD de propiedades |
| `SupabaseDuenoHelper` | CRUD de propietarios |
| `SupabaseMovimientoHelper` | CRUD de movimientos financieros |
| `SupabaseMovimientoIAHelper` | Facturas procesadas por IA |
| `SupabaseHojaMensualHelper` | Hojas mensuales por casa |
| `SupabaseStorageHelper` | Subida y acceso a archivos e imágenes |
| `SupabaseAuditoriaHelper` | Registro de todas las acciones |
| `SupabaseLogCorreoHelper` | Historial de correos enviados |
| `SupabaseNotaHelper` | Notas por propiedad |
| `SupabaseFotoCasaHelper` | Fotos de propiedades |
| `SupabasePreferenciasHelper` | Preferencias del usuario (tema oscuro) |

---

## 📱 Módulos y pantallas

### 1. Login (`LoginWindow`)

- Autenticación con email y contraseña vía Supabase Auth
- **Olvidé mi contraseña**: genera y envía contraseña temporal por correo (Resend)
- **Botón de soporte**: muestra datos de contacto del administrador
- Recuerda el último email utilizado
- Soporte de tema oscuro/claro

### 2. Menú Principal (`MenuPrincipalWindow`)

- Grid de tarjetas de propiedades con imagen, dueño, moneda y balance
- Buscador en tiempo real por nombre, dueño o categoría
- Avatar del usuario (foto de perfil o iniciales)
- Botones: Gestión, Resumen, Historial, Inactivas, Tutorial, Admin (solo admins)

### 3. Detalle de Casa (`DetalleCasaWindow`)

Ventana principal de trabajo, con **5 pestañas**:

**Resumen** — información general, selector de mes/año, balance y KPIs

**Movimientos** — tabla del mes, agregar/editar/eliminar, adjuntar comprobantes. El formulario muestra la moneda de la casa (`$ USD` o `₡ CRC`)

**Movimientos IA** — drag & drop de facturas, tabla con estado de análisis, edición antes de aprobar. Los movimientos aprobados van siempre al mes seleccionado en los filtros, sin importar la fecha de la factura

**Notas** — notas libres por propiedad con fecha

**Fotos** — galería con upload a Supabase Storage y preview

### 4. Gestión (`GestionWindow`)

CRUD de catálogos: Dueños / Casas / Categorías de propiedades / Categorías de movimientos

### 5. Historial (`HistorialWindow`)

- 5 tabs de auditoría: Casas / Movimientos / Dueños / Categorías / Logs de correos
- Paginación, filtros, búsqueda de texto libre
- Botón **Deshacer** para movimientos recientes
- Ver datos JSON anteriores y nuevos

### 6. Resumen Consolidado (`ResumenConsolidadoWindow`)

- KPIs globales por moneda (USD / CRC)
- Balance, ingresos y gastos consolidados
- Casa con mayor ingreso del período

### 7. Casas Inactivas (`InactivasWindow`)

- Lista de casas desactivadas con filtros
- Botón para reactivar

### 8. Administración de Usuarios (`AdminUsuariosWindow`)

Solo visible para administradores.

| Acción | Descripción |
|---|---|
| Nuevo Usuario | Crea usuario directamente activo (sin email de verificación) |
| Cambiar Rol | Alterna entre `admin` y `usuario` |
| Cambiar Correo | Actualiza el email de usuarios con rol `usuario` (emergencia: acceso perdido) |
| Desactivar/Activar | Controla el acceso al sistema |

Un admin no puede editar el correo ni el rol de otro administrador.

---

## 🗄️ Base de datos (Supabase)

### Tablas principales

| Tabla | Descripción |
|---|---|
| `auth.users` | Tabla nativa de Supabase Auth (email, password hash) |
| `usuarios` | Perfil extendido: nombre, apellido, rol, activo, foto_perfil |
| `casas` | Propiedades: nombre, dueño, categoría, moneda, activo |
| `duenos` | Propietarios de las casas |
| `categorias` | Clasificaciones de propiedades |
| `categorias_movimientos` | Tipos de movimientos (Arriendo, Mantenimiento, etc.) |
| `hojas_mensuales` | Una hoja por mes/año por casa |
| `movimientos` | Ingresos y gastos reales de cada hoja mensual |
| `movimientos_ia_temporales` | Facturas en proceso de análisis IA |
| `notas_casa` | Notas libres asociadas a cada propiedad |
| `fotos_casa` | URLs de fotos de propiedades |
| `auditoria` | Registro de todas las acciones del sistema |
| `log_correos` | Historial de correos enviados |
| `preferencias_usuario` | Configuración por usuario (tema oscuro, etc.) |

### Hojas mensuales

Cada casa tiene una **hoja por mes/año**. Los movimientos pertenecen a una hoja. Al crear una casa nueva, se generan automáticamente hojas para todos los meses del año actual y el próximo mes.

### Seguridad

- **Row Level Security (RLS)** en todas las tablas sensibles
- **Anon Key** para operaciones normales autenticadas
- **Service Role Key** solo para operaciones admin: crear usuarios, resetear contraseñas, cambiar emails

---

## 🤖 Módulo de IA con n8n

Permite subir una foto de factura (JPG, PNG, PDF, WebP) y que la IA extraiga automáticamente los datos.

### Flujo completo

```
1. Usuario arrastra factura al dialog
         ↓
2. Se sube al bucket privado "facturas" en Supabase Storage
         ↓
3. Se genera una URL firmada con validez de 24 horas
         ↓
4. Se inserta un registro en movimientos_ia_temporales  [estado: pendiente]
         ↓
5. POST al webhook de n8n con imagenBase64, tipo, casaId, mes, anio...
         ↓
6. n8n procesa la imagen con Llama 4 Scout (Groq, con visión)
         ↓
7. n8n retorna JSON: { fecha, monto, descripcion, categoria, estado }
         ↓
8. El sistema actualiza el registro IA con los datos extraídos
         ↓
9. La tabla IA se actualiza en tiempo real mientras el dialog está abierto
         ↓
10. El usuario revisa, edita si es necesario, y aprueba
         ↓
11. Se crean los movimientos reales y los registros IA se marcan "aprobado"
```

### Configuración en appsettings.json

```json
"N8n": {
  "WebhookUrl": "https://tu-instancia.app.n8n.cloud/webhook/factura-ia",
  "Token": "tu-token-secreto"
}
```

El workflow de n8n debe:
1. Recibir el POST con header `Authorization: Bearer <Token>`
2. Enviar la imagen al modelo de visión (ej. `meta-llama/llama-4-scout-17b-16e-instruct`)
3. Retornar JSON con: `fecha`, `monto`, `descripcion`, `categoria`, `estado` ("ok" o "parcial")

### Estados de movimientos IA

| Estado | Significado |
|---|---|
| `pendiente` | Datos listos, esperando revisión del usuario |
| `parcial` | La IA no pudo leer todos los datos |
| `error` | Error en el proceso |
| `aprobado` | El usuario aprobó y se creó el movimiento real |

---

## 📦 Almacenamiento de archivos

| Bucket | Acceso | Uso |
|---|---|---|
| `CasasFotos` | Público | Fotos de portada de propiedades |
| `facturas` | **Privado** | Facturas subidas para análisis IA |
| `avatares` | Público | Fotos de perfil de usuarios |

Las **facturas** solo son accesibles mediante **URLs firmadas** con expiración de 24 horas.

---

## 📧 Envío de correos (Resend)

### Casos de uso

| Caso | Descripción |
|---|---|
| Reportes mensuales | PDF del mes enviado a destinatarios configurables |
| Contraseña temporal | Se genera y envía al usuario cuando olvida su contraseña |

### Configuración

```json
"Resend": {
  "ApiKey": "re_...",
  "From": "reporte@tu-dominio.com"
}
```

Todos los envíos quedan registrados en la tabla `log_correos` con estado y detalle de errores.

---

## 🔐 Autenticación y roles

### Roles del sistema

| Rol | Permisos |
|---|---|
| `admin` | Acceso total + panel de administración de usuarios |
| `usuario` | Acceso a casas, movimientos y reportes (sin panel admin) |

### Recuperación de acceso

- **Olvidó contraseña**: clic en "Olvidé contraseña" → contraseña temporal por correo
- **Perdió acceso al correo también**: un admin puede cambiarle el email desde el panel de usuarios, luego el usuario hace el paso anterior

---

## ⚙️ Configuración del proyecto

```json
{
  "Supabase": {
    "Url": "https://tu-proyecto.supabase.co",
    "AnonKey": "eyJ...",
    "ServiceRoleKey": "eyJ..."
  },
  "N8n": {
    "WebhookUrl": "https://tu-instancia.app.n8n.cloud/webhook/factura-ia",
    "Token": "tu-token-secreto"
  },
  "Resend": {
    "ApiKey": "re_...",
    "From": "reporte@tu-dominio.com"
  }
}
```

> ⚠️ Nunca subas `appsettings.json` con credenciales reales a un repositorio público.

---

## 📁 Estructura de archivos

```
FlujoCaja/
├── App.xaml / App.xaml.cs              # Punto de entrada
├── appsettings.json                    # Configuración y credenciales
├── FlujoCajaWpf.csproj                 # Proyecto .NET 9.0 WPF
│
├── Views/                              # Pantallas y diálogos
│   ├── LoginWindow
│   ├── MenuPrincipalWindow
│   ├── DetalleCasaWindow
│   ├── GestionWindow
│   ├── HistorialWindow
│   ├── AdminUsuariosWindow
│   ├── ResumenConsolidadoWindow
│   ├── InactivasWindow
│   ├── ImportarFacturasDialog
│   ├── EnviarReporteDialog
│   ├── AgregarMovimientoWindow
│   ├── CustomMessageBox
│   └── ...otros diálogos
│
├── Models/                             # Modelos de datos (UI + Supabase)
├── Data/                               # Helpers de acceso a Supabase (17+)
├── ViewModels/                         # Lógica de presentación MVVM
├── Commands/                           # RelayCommand
├── Converters/                         # Convertidores de valores XAML
├── Styles/                             # Temas claro/oscuro
├── Resources/                          # Logos e imágenes
└── Scripts/                            # Scripts SQL de inicialización
```

---

## 🖥️ Requisitos del sistema

| Requisito | Detalle |
|---|---|
| **Sistema operativo** | Windows 10 / 11 (64-bit) |
| **Runtime** | .NET 9.0 Desktop Runtime |
| **Conexión** | Internet (para Supabase, n8n y Resend) |
| **Resolución mínima** | 1280 × 720 |

### Para desarrollo

- Visual Studio 2022 o VS Code con extensión C#
- .NET 9.0 SDK
- Cuenta en [Supabase](https://supabase.com) con proyecto configurado
- (Opcional) Cuenta en [n8n](https://n8n.io) para el módulo IA
- (Opcional) Cuenta en [Resend](https://resend.com) para envío de correos
