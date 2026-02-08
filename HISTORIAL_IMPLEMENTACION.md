# 📊 Sistema de Historial / Auditoría - Implementación Completa

## ✅ **LO QUE YA ESTÁ IMPLEMENTADO**

### 1. **Modelos de Datos**
- ✅ `AuditoriaSupabase.cs` - Mapeo con tabla Supabase
- ✅ `Auditoria.cs` - Modelo para UI con propiedades calculadas

### 2. **Helper de Auditoría**
- ✅ `SupabaseAuditoriaHelper.cs`
  - `RegistrarAccionAsync()` - Registra cualquier acción
  - `ObtenerAuditoriasPorModuloAsync()` - Con paginación
  - `DeshacerMovimientoAsync()` - Deshacer movimientos
  - `ObtenerUsuariosAsync()` - Lista de usuarios

### 3. **Interfaz de Usuario**
- ✅ `HistorialWindow.xaml` - Ventana con tabs
  - Tab Casas (funcional)
  - Tab Movimientos (funcional con botón deshacer)
  - Tab Dueños (pendiente)
  - Tab Categorías (pendiente)
- ✅ Paginación estilo Excel (50 registros por página)
- ✅ Filtros por usuario, acción y búsqueda
- ✅ Botón deshacer con icono ↶ y confirmación

### 4. **Integración**
- ✅ Botón "📜 Historial" en MenuPrincipal conectado

---

## 🔧 **LO QUE FALTA POR HACER**

### **PASO 1: Crear tabla en Supabase** ⚠️ **REQUERIDO**

**Esto es OBLIGATORIO para que funcione el historial.**

> **⚠️ IMPORTANTE:** Ejecutar el script SQL es SOLO EL PRIMER PASO. Después debes hacer el PASO 2 para que el sistema realmente registre las acciones.

#### **Instrucciones detalladas:**

**1. Abre tu proyecto de Supabase en el navegador:**
   - Ve a: https://supabase.com/dashboard
   - Inicia sesión si no lo has hecho
   - Selecciona tu proyecto (el que usas para FlujoCaja)

**2. Ve al SQL Editor:**
   - En el menú lateral izquierdo, busca y haz click en "SQL Editor"
   - O ve directamente a: https://supabase.com/dashboard/project/TU-PROJECT-ID/sql

**3. Abre el archivo del script:**
   - En VS Code, abre: `Scripts/04_create_auditoria_table.sql`
   - Selecciona TODO el contenido (Ctrl+A)
   - Cópialo (Ctrl+C)

**4. Pega y ejecuta el script:**
   - En Supabase SQL Editor, haz click en "+ New query" (botón arriba a la derecha)
   - Pega el código que copiaste (Ctrl+V)
   - Haz click en el botón "RUN" (abajo a la derecha, o F5)
   - Deberías ver: "Success. No rows returned"

**5. Verifica que la tabla fue creada:**
   - En el menú lateral, ve a "Table Editor"
   - Deberías ver una nueva tabla llamada `auditoria`
   - Haz click en ella para ver su estructura
   - Debe tener estas columnas:
     - id (int4)
     - usuario_email (varchar)
     - modulo (varchar)
     - tipo_accion (varchar)
     - entidad_id (int4)
     - entidad_nombre (varchar)
     - descripcion (text)
     - datos_anteriores (jsonb)
     - datos_nuevos (jsonb)
     - fecha (timestamptz)

**¿Qué hace este script?**
- Crea la tabla `auditoria` para guardar todas las acciones
- Crea índices para que las búsquedas sean rápidas
- Configura permisos para que usuarios autenticados puedan leer/escribir

**Si ves algún error:**
- Revisa que hayas copiado TODO el script
- Verifica que estés en el proyecto correcto
- Si la tabla ya existe, primero elimínala: `DROP TABLE IF EXISTS auditoria;`

---

### **PASO 2: Integrar registro de auditoría en acciones existentes**

> **📌 ESTO ES ESENCIAL:** Si solo ejecutas el script SQL, la tabla existirá PERO ESTARÁ VACÍA porque el código no está registrando nada todavía. Debes agregar código en cada lugar donde se crea/edita/elimina algo.

**¿Qué significa "integrar"?**
- Cada vez que el usuario crea una casa → llamar a `RegistrarAccionAsync()`
- Cada vez que edita un movimiento → llamar a `RegistrarAccionAsync()`
- Cada vez que elimina algo → llamar a `RegistrarAccionAsync()`
- etc.

**Ejemplo práctico:**

#### **A. Gestión de Casas**

**Archivo:** `Views/AgregarCasaWindow.xaml.cs`

```csharp
// Al CREAR casa exitosamente
var user = SupabaseAuthHelper.GetCurrentUser();
await SupabaseAuditoriaHelper.RegistrarAccionAsync(
    user?.Email ?? "desconocido",
    "casa",
    "crear",
    resultado.Data.Id,
    resultado.Data.Nombre,
    $"Creó nueva casa: {resultado.Data.Nombre}"
);
```

**Archivo:** `Views/DetalleCasaWindow.xaml.cs` o donde se edite casa

```csharp
// Al EDITAR casa
var user = SupabaseAuthHelper.GetCurrentUser();
await SupabaseAuditoriaHelper.RegistrarAccionAsync(
    user?.Email ?? "desconocido",
    "casa",
    "editar",
    casa.Id,
    casa.Nombre,
    $"Editó casa: Cambió {camposModificados}" // Describir qué cambió
);

// Al ELIMINAR/DESACTIVAR casa
await SupabaseAuditoriaHelper.RegistrarAccionAsync(
    user?.Email ?? "desconocido",
    "casa",
    "desactivar",
    casa.Id,
    casa.Nombre,
    $"Desactivó casa: {casa.Nombre}"
);
```

---

#### **B. Gestión de Movimientos**

**Archivo:** `Views/AgregarMovimientoWindow.xaml.cs`

```csharp
// Al CREAR movimiento
var user = SupabaseAuthHelper.GetCurrentUser();
await SupabaseAuditoriaHelper.RegistrarAccionAsync(
    user?.Email ?? "desconocido",
    "movimiento",
    "crear",
    nuevoMovimiento.Id,
    _casa.Nombre,
    $"{tipo}: {monto:C} - {categoria} en {_casa.Nombre}",
    datosNuevos: new {
        casa = _casa.Nombre,
        tipo = tipo,
        monto = monto,
        categoria = categoria,
        descripcion = descripcion
    }
);
```

**Archivo:** `Views/DetalleCasaWindow.xaml.cs` (método de eliminar movimiento)

```csharp
// Al ELIMINAR movimiento
var user = SupabaseAuthHelper.GetCurrentUser();
await SupabaseAuditoriaHelper.RegistrarAccionAsync(
    user?.Email ?? "desconocido",
    "movimiento",
    "eliminar",
    movimiento.Id,
    _casa.Nombre,
    $"Eliminó {movimiento.Tipo}: {movimiento.Monto:C} - {movimiento.CategoriaNombre}"
);
```

---

#### **C. Gestión de Dueños**

**Archivo:** `Views/Controls/GestionDuenosControl.xaml.cs`

```csharp
// Al CREAR dueño
var user = SupabaseAuthHelper.GetCurrentUser();
await SupabaseAuditoriaHelper.RegistrarAccionAsync(
    user?.Email ?? "desconocido",
    "dueno",
    "crear",
    dueno.Id,
    $"{dueno.Nombre} {dueno.Apellido}",
    $"Creó dueño: {dueno.Nombre} {dueno.Apellido}"
);

// Al EDITAR dueño
await SupabaseAuditoriaHelper.RegistrarAccionAsync(
    user?.Email ?? "desconocido",
    "dueno",
    "editar",
    dueno.Id,
    $"{dueno.Nombre} {dueno.Apellido}",
    $"Editó dueño: {cambios}"
);

// Al ELIMINAR dueño
await SupabaseAuditoriaHelper.RegistrarAccionAsync(
    user?.Email ?? "desconocido",
    "dueno",
    "eliminar",
    dueno.Id,
    $"{dueno.Nombre} {dueno.Apellido}",
    $"Eliminó dueño: {dueno.Nombre} {dueno.Apellido}"
);
```

---

#### **D. Gestión de Categorías**

**Archivo:** `Views/Controls/GestionCategoriasControl.xaml.cs`

```csharp
// Al CREAR categoría
var user = SupabaseAuthHelper.GetCurrentUser();
await SupabaseAuditoriaHelper.RegistrarAccionAsync(
    user?.Email ?? "desconocido",
    "categoria",
    "crear",
    categoria.Id,
    categoria.Nombre,
    $"Creó categoría: {categoria.Nombre}"
);

// Similar para editar y eliminar
```

**Archivo:** `Views/Controls/GestionCategoriasMovimientosControl.xaml.cs`

```csharp
// Similar para categorías de movimientos
// Usar modulo "categoria_movimiento"
```

---

## 🎯 **CÓMO USAR EL SISTEMA**

### **Ver Historial:**
1. Abrir MenuPrincipal
2. Click en botón "📜 Historial"
3. Seleccionar tab (Casas / Movimientos)
4. Usar filtros para buscar
5. Navegar con paginación

### **Deshacer Movimiento:**
1. Ir a tab Movimientos
2. Encontrar movimiento a deshacer (debe ser tipo "crear")
3. Click en botón ↶ (rojo)
4. Confirmar en diálogo
5. El movimiento se eliminará y quedará registrado como "deshacer"

---

## 📋 **CHECKLIST DE IMPLEMENTACIÓN**

- [ ] **1. Ejecutar script SQL en Supabase**
- [ ] **2. Agregar auditoría en AgregarCasaWindow (crear)**
- [ ] **3. Agregar auditoría en DetalleCasaWindow (editar/eliminar casa)**
- [ ] **4. Agregar auditoría en AgregarMovimientoWindow (crear)**
- [ ] **5. Agregar auditoría en DetalleCasaWindow (eliminar movimiento)**
- [ ] **6. Agregar auditoría en GestionDuenosControl (crear/editar/eliminar)**
- [ ] **7. Agregar auditoría en GestionCategoriasControl (crear/editar/eliminar)**
- [ ] **8. Agregar auditoría en GestionCategoriasMovimientosControl**
- [ ] **9. Probar cada acción y verificar en Historial**
- [ ] **10. Probar botón deshacer en movimientos**

---

## 🔍 **FORMATO DE DESCRIPCIÓN SUGERIDO**

### Casas:
- Crear: "Creó nueva casa: Villa Paraíso"
- Editar: "Editó Villa Paraíso: Cambió moneda de CRC a USD"
- Eliminar: "Eliminó casa: Villa Paraíso"
- Activar: "Activó casa: Villa Paraíso"
- Desactivar: "Desactivó casa: Villa Paraíso"

### Movimientos:
- Crear: "Ingreso: ₡500,000 - Alquiler en Villa Paraíso"
- Editar: "Editó movimiento de Villa Paraíso: Cambió monto de ₡500,000 a ₡550,000"
- Eliminar: "Eliminó Ingreso: ₡500,000 - Alquiler"

### Dueños:
- Crear: "Creó dueño: Juan Pérez"
- Editar: "Editó Juan Pérez: Cambió teléfono"
- Eliminar: "Eliminó dueño: Juan Pérez"

### Categorías:
- Crear: "Creó categoría: Mantenimiento"
- Editar: "Editó categoría Mantenimiento: Cambió descripción"
- Eliminar: "Eliminó categoría: Mantenimiento"

---

## 💡 **MEJORAS FUTURAS**

1. **Comparación de cambios:** Mostrar diff visual de datos_anteriores vs datos_nuevos
2. **Exportar historial:** PDF o Excel con filtros aplicados
3. **Deshacer más acciones:** Ediciones, eliminaciones de casas, etc.
4. **Notificaciones:** Alertas cuando otro usuario hace cambios
5. **Tabs Dueños y Categorías:** Implementar funcionalidad completa
6. **Búsqueda avanzada:** Por rango de fechas, por entidad específica
7. **Dashboard de actividad:** Gráficos de acciones por usuario/día

---

**✨ El sistema de historial está listo para usar una vez ejecutes el script SQL y agregues las llamadas de auditoría!**
