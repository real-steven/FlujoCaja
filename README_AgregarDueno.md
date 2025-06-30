# Samara Rentals - Funcionalidad "Agregar Dueño"

## Descripción
Esta implementación añade la funcionalidad completa para agregar nuevos dueños al sistema de gestión de propiedades de Samara Rentals.

## Características Implementadas

### 1. Formulario de Agregar Dueño (`AgregarDuenoForm`)

#### Estructura del Formulario:
- **Encabezado**: "Apartado de Agregación" con logo de Samara Rentals
- **Dropdown Tipo de Entidad**: Selecciona "Dueño" por defecto (preparado para futuras entidades)
- **Campos Obligatorios**:
  - Nombre Completo (TextBox con placeholder)
  - Identificación (TextBox con placeholder)
  - Correo Electrónico (TextBox con placeholder y validación de formato)
  - Número Telefónico (TextBox con placeholder)
- **Botón Guardar**: Centrado, se habilita solo cuando todos los campos son válidos
- **Botón Regresar**: En la esquina superior derecha
- **Mensaje de Éxito**: Verde, aparece temporalmente al guardar exitosamente

#### Características de UX/UI:
- **Placeholders dinámicos**: Texto de ayuda que desaparece al hacer foco
- **Validación en tiempo real**: El botón se habilita/deshabilita automáticamente
- **Diseño moderno**: Alineado con el sistema existente
- **Responsive**: Formulario centrado y bien distribuido
- **Feedback visual**: Colores y mensajes claros

### 2. Validaciones Implementadas

#### Validaciones de Campos:
- **Nombre Completo**: Obligatorio, debe tener al menos nombre y apellido
- **Identificación**: Obligatorio, único en el sistema
- **Correo Electrónico**: Obligatorio, formato válido, único en el sistema
- **Número Telefónico**: Obligatorio

#### Validaciones de Negocio:
- **Identificación única**: Verifica que no exista otro dueño con la misma identificación
- **Email único**: Verifica que no exista otro dueño con el mismo correo
- **Formato de email**: Validación con expresión regular
- **Separación automática**: Divide el nombre completo en nombre y apellido(s)

### 3. Base de Datos

#### Tabla Duenos Actualizada:
```sql
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
)
```

#### Métodos Nuevos en DatabaseHelper:
- `GuardarDueno()`: Inserta un nuevo dueño en la base de datos
- `ExisteDuenoConIdentificacion()`: Verifica unicidad de identificación
- `ExisteDuenoConEmail()`: Verifica unicidad de email
- Soporte para migración automática (agrega columna Identificacion si no existe)

### 4. Integración con el Sistema

#### Acceso desde el Menú Principal:
1. **Botón "Agregar"** → **"Nuevo Dueño"**
2. Se abre el formulario modal `AgregarDuenoForm`
3. Al guardar exitosamente, se muestra mensaje de confirmación
4. Los datos están disponibles inmediatamente para nuevas casas

#### Flujo de Uso:
1. **Acceso**: Menú Principal → Agregar → Nuevo Dueño
2. **Llenar formulario**:
   - Tipo de entidad: "Dueño" (preseleccionado)
   - Nombre completo: Ej. "Juan Carlos Pérez García"
   - Identificación: Ej. "123456789"
   - Correo: Ej. "juan.perez@email.com"
   - Teléfono: Ej. "+506 8888-9999"
3. **Validación**: Se valida en tiempo real, botón se habilita automáticamente
4. **Guardar**: Clic en "Guardar"
5. **Confirmación**: Mensaje verde "Dueño agregado correctamente"
6. **Formulario limpio**: Se limpia para agregar otro dueño
7. **Regresar**: Botón "Regresar" para volver al menú

## Archivos Creados/Modificados

### Nuevos Archivos:
- `Formularios/AgregarDuenoForm.cs` - Lógica del formulario
- `Formularios/AgregarDuenoForm.Designer.cs` - Diseño del formulario
- `README_AgregarDueno.md` - Esta documentación

### Archivos Modificados:
- `Data/DatabaseHelper.cs` - Métodos para CRUD de dueños y validaciones
- `Formularios/PanelAgregar.cs` - Integración con el formulario de dueños

## Detalles Técnicos

### Validación de Email
```csharp
string patron = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
return Regex.IsMatch(correo, patron);
```

### Separación de Nombre y Apellido
```csharp
private (string nombre, string apellido) SepararNombreApellido(string nombreCompleto)
{
    var partes = nombreCompleto.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
    string nombre = partes[0];
    string apellido = string.Join(" ", partes.Skip(1));
    return (nombre, apellido);
}
```

### Placeholders Dinámicos
- Texto gris cuando está vacío/sin foco
- Texto negro cuando tiene contenido/con foco
- Se limpia automáticamente al hacer clic

### Timer para Mensaje de Éxito
- Duración: 3 segundos
- Color verde
- Centrado automáticamente
- Se oculta automáticamente

## Manejo de Errores

### Errores Controlados:
- **Campos vacíos**: Mensaje específico por campo
- **Formato de email inválido**: "El formato del correo electrónico no es válido"
- **Identificación duplicada**: "Ya existe un dueño con esa identificación"
- **Email duplicado**: "Ya existe un dueño con ese correo electrónico"
- **Errores de BD**: Mensajes técnicos para debugging

### Robustez:
- Try-catch en todas las operaciones críticas
- Validación antes de operaciones de BD
- Rollback automático en caso de error
- Logging en consola para debugging

## Características Futuras Preparadas

### Extensibilidad:
- **Dropdown de tipos**: Preparado para "Categoría", "Usuario", etc.
- **Arquitectura modular**: Fácil agregar nuevos tipos de entidades
- **Validaciones configurables**: Pueden extenderse por tipo
- **Base de datos escalable**: Estructura preparada para más campos

### Mejoras Futuras Sugeridas:
1. **Foto del dueño**: Agregar campo para imagen
2. **Dirección detallada**: Campos separados para dirección completa
3. **Documentos**: Subir documentos de identificación
4. **Historial**: Ver historial de cambios
5. **Búsqueda avanzada**: Filtros en listado de dueños
6. **Edición**: Modificar dueños existentes
7. **Inactivación**: Marcar como inactivo en lugar de eliminar

## Pruebas Recomendadas

### Casos de Prueba:
1. **Campos vacíos**: Verificar que no se pueda guardar
2. **Email inválido**: Probar formatos incorrectos
3. **Duplicados**: Intentar crear dueños con misma identificación/email
4. **Nombre simple**: Probar con un solo nombre
5. **Caracteres especiales**: Probar en todos los campos
6. **Longitudes extremas**: Textos muy largos
7. **Regresar sin guardar**: Verificar que se cierre correctamente

### Datos de Prueba:
```
Nombre: María Elena Rodríguez Jiménez
ID: 987654321
Email: maria.rodriguez@test.com
Teléfono: +506 7777-8888
```

---

**Autor**: GitHub Copilot  
**Fecha**: Diciembre 2024  
**Versión**: 1.0.0  
**Dependencias**: .NET 9.0, SQLite, WinForms
