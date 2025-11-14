# Script de Construcción e Instalador para FlujoCajaApp
# Este script compila la aplicación y crea un instalador ejecutable

param(
    [string]$Version = "1.0.0",
    [string]$OutputPath = ".\Installer",
    [switch]$Clean = $false
)

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  FlujoCaja - Generador de Instalador" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Configuración
$ProjectName = "FlujoDeCajaApp"
$AppDisplayName = "FlujoCaja - Sistema de Gestión de Propiedades"
$Publisher = "Samara Rental"
$ProjectPath = "."
$PublishPath = ".\bin\Release\net9.0-windows\publish"
$InstallerPath = $OutputPath

# Función para mostrar errores
function Show-Error {
    param([string]$Message)
    Write-Host "ERROR: $Message" -ForegroundColor Red
    exit 1
}

# Función para mostrar información
function Show-Info {
    param([string]$Message)
    Write-Host "INFO: $Message" -ForegroundColor Green
}

# Limpiar carpetas anteriores si se solicita
if ($Clean) {
    Show-Info "Limpiando directorios anteriores..."
    if (Test-Path $PublishPath) { Remove-Item $PublishPath -Recurse -Force }
    if (Test-Path $InstallerPath) { Remove-Item $InstallerPath -Recurse -Force }
}

# Verificar que estamos en el directorio correcto
if (-not (Test-Path "$ProjectName.csproj")) {
    Show-Error "No se encontró el archivo $ProjectName.csproj. Asegúrate de ejecutar este script desde la raíz del proyecto."
}

# Verificar que dotnet está instalado
try {
    $dotnetVersion = dotnet --version
    Show-Info "Usando .NET SDK versión: $dotnetVersion"
} catch {
    Show-Error "No se encontró .NET SDK. Por favor instala .NET 9.0 SDK."
}

# Compilar y publicar la aplicación
Show-Info "Compilando la aplicación..."
try {
    dotnet clean --configuration Release
    dotnet restore
    dotnet publish --configuration Release --framework net9.0-windows --output $PublishPath --self-contained false --verbosity minimal
    
    if ($LASTEXITCODE -ne 0) {
        Show-Error "Error durante la compilación."
    }
    
    Show-Info "Compilación exitosa."
} catch {
    Show-Error "Error durante la compilación: $($_.Exception.Message)"
}

# Verificar que los archivos se publicaron correctamente
if (-not (Test-Path "$PublishPath\$ProjectName.exe")) {
    Show-Error "No se encontró el ejecutable en $PublishPath"
}

# Crear directorio del instalador
Show-Info "Creando estructura del instalador..."
New-Item -ItemType Directory -Force -Path $InstallerPath | Out-Null
New-Item -ItemType Directory -Force -Path "$InstallerPath\App" | Out-Null

# Copiar archivos de la aplicación
Show-Info "Copiando archivos de la aplicación..."
Copy-Item -Path "$PublishPath\*" -Destination "$InstallerPath\App" -Recurse -Force

# Crear script de instalación
$InstallScript = @"
@echo off
setlocal EnableDelayedExpansion

echo =====================================
echo   $AppDisplayName
echo   Instalador v$Version
echo =====================================
echo.

REM Verificar permisos de administrador
net session >nul 2>&1
if !errorLevel! == 0 (
    echo Ejecutando con permisos de administrador...
) else (
    echo ADVERTENCIA: Se recomienda ejecutar como administrador para una instalación completa.
    pause
)

REM Configuración
set "APP_NAME=$ProjectName"
set "APP_DISPLAY_NAME=$AppDisplayName"
set "INSTALL_DIR=%ProgramFiles%\FlujoCaja"
set "DESKTOP_SHORTCUT=%PUBLIC%\Desktop\FlujoCaja.lnk"
set "START_MENU_SHORTCUT=%ProgramData%\Microsoft\Windows\Start Menu\Programs\FlujoCaja.lnk"

echo Instalando en: !INSTALL_DIR!
echo.

REM Crear directorio de instalación
if exist "!INSTALL_DIR!" (
    echo Eliminando instalación anterior...
    rmdir /s /q "!INSTALL_DIR!"
)

echo Creando directorio de instalación...
mkdir "!INSTALL_DIR!"
if !errorLevel! neq 0 (
    echo ERROR: No se pudo crear el directorio de instalación.
    echo Asegúrate de ejecutar como administrador.
    pause
    exit /b 1
)

REM Copiar archivos
echo Copiando archivos de la aplicación...
xcopy /s /e /y "App\*" "!INSTALL_DIR!\"
if !errorLevel! neq 0 (
    echo ERROR: No se pudieron copiar los archivos.
    pause
    exit /b 1
)

REM Crear acceso directo en el escritorio
echo Creando acceso directo en el escritorio...
powershell -Command "& {$WshShell = New-Object -comObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('!DESKTOP_SHORTCUT!'); $Shortcut.TargetPath = '!INSTALL_DIR!\%APP_NAME%.exe'; $Shortcut.WorkingDirectory = '!INSTALL_DIR!'; $Shortcut.Description = '!APP_DISPLAY_NAME!'; $Shortcut.Save()}"

REM Crear acceso directo en el menú inicio
echo Creando acceso directo en el menú inicio...
powershell -Command "& {$WshShell = New-Object -comObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('!START_MENU_SHORTCUT!'); $Shortcut.TargetPath = '!INSTALL_DIR!\%APP_NAME%.exe'; $Shortcut.WorkingDirectory = '!INSTALL_DIR!'; $Shortcut.Description = '!APP_DISPLAY_NAME!'; $Shortcut.Save()}"

REM Registrar en Programas y características (opcional)
echo Registrando en el sistema...
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\FlujoCaja" /v "DisplayName" /d "!APP_DISPLAY_NAME!" /f >nul 2>&1
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\FlujoCaja" /v "DisplayVersion" /d "$Version" /f >nul 2>&1
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\FlujoCaja" /v "Publisher" /d "$Publisher" /f >nul 2>&1
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\FlujoCaja" /v "InstallLocation" /d "!INSTALL_DIR!" /f >nul 2>&1
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\FlujoCaja" /v "UninstallString" /d "!INSTALL_DIR!\Uninstall.bat" /f >nul 2>&1

REM Crear desinstalador
echo @echo off > "!INSTALL_DIR!\Uninstall.bat"
echo echo Desinstalando FlujoCaja... >> "!INSTALL_DIR!\Uninstall.bat"
echo rmdir /s /q "!INSTALL_DIR!" >> "!INSTALL_DIR!\Uninstall.bat"
echo del "!DESKTOP_SHORTCUT!" ^>nul 2^>^&1 >> "!INSTALL_DIR!\Uninstall.bat"
echo del "!START_MENU_SHORTCUT!" ^>nul 2^>^&1 >> "!INSTALL_DIR!\Uninstall.bat"
echo reg delete "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\FlujoCaja" /f ^>nul 2^>^&1 >> "!INSTALL_DIR!\Uninstall.bat"
echo echo FlujoCaja ha sido desinstalado. >> "!INSTALL_DIR!\Uninstall.bat"
echo pause >> "!INSTALL_DIR!\Uninstall.bat"

echo.
echo =====================================
echo ¡Instalación completada exitosamente!
echo =====================================
echo.
echo La aplicación se ha instalado en:
echo !INSTALL_DIR!
echo.
echo Se han creado accesos directos en:
echo - Escritorio
echo - Menú Inicio
echo.
echo Para desinstalar, ejecuta:
echo !INSTALL_DIR!\Uninstall.bat
echo.
echo ¿Deseas ejecutar la aplicación ahora? (S/N)
set /p choice=
if /i "!choice!"=="S" (
    start "" "!INSTALL_DIR!\%APP_NAME%.exe"
)

echo.
echo Presiona cualquier tecla para cerrar...
pause >nul
"@

# Guardar script de instalación
$InstallScript | Out-File -FilePath "$InstallerPath\Install.bat" -Encoding ASCII

# Crear archivo README para el instalador
$ReadmeContent = @"
# FlujoCaja - Sistema de Gestión de Propiedades
## Instalador v$Version

### Requisitos del Sistema:
- Windows 10 o superior
- .NET 9.0 Runtime (se puede descargar desde https://dotnet.microsoft.com/download)
- 50 MB de espacio libre en disco

### Instrucciones de Instalación:

1. **Ejecutar como Administrador:**
   - Haz clic derecho en `Install.bat`
   - Selecciona "Ejecutar como administrador"

2. **Seguir las instrucciones en pantalla**

3. **La aplicación se instalará en:**
   `C:\Program Files\FlujoCaja\`

4. **Se crearán accesos directos en:**
   - Escritorio
   - Menú Inicio

### Características de la Aplicación:
- ✅ Gestión de propiedades inmobiliarias
- ✅ Control de flujo de caja por propiedad
- ✅ Seguimiento de ingresos y gastos
- ✅ Reportes y historial de movimientos
- ✅ Vista consolidada de toda la empresa
- ✅ Sistema de justificaciones para auditoría
- ✅ Interfaz moderna y fácil de usar

### Para Desinstalar:
Ejecuta: `C:\Program Files\FlujoCaja\Uninstall.bat`

### Soporte:
Si tienes problemas con la instalación:
1. Asegúrate de tener .NET 9.0 Runtime instalado
2. Ejecuta el instalador como administrador
3. Verifica que tienes suficiente espacio en disco

### Notas Técnicas:
- La aplicación utiliza SQLite para almacenamiento local
- Se conecta a Supabase para sincronización en la nube
- Compatible con Windows 10/11 (x64)

---
Desarrollado por $Publisher
Versión: $Version
Fecha: $(Get-Date -Format 'dd/MM/yyyy')
"@

$ReadmeContent | Out-File -FilePath "$InstallerPath\README.txt" -Encoding UTF8

# Crear script de verificación de requisitos
$RequirementsCheck = @"
@echo off
echo =====================================
echo   Verificación de Requisitos
echo =====================================
echo.

echo Verificando .NET Runtime...
dotnet --list-runtimes | findstr "Microsoft.WindowsDesktop.App 9.0" >nul
if !errorLevel! == 0 (
    echo ✅ .NET 9.0 Desktop Runtime encontrado
) else (
    echo ❌ .NET 9.0 Desktop Runtime NO encontrado
    echo.
    echo Descarga e instala .NET 9.0 Runtime desde:
    echo https://dotnet.microsoft.com/download/dotnet/9.0
    echo.
    echo Busca: ".NET Desktop Runtime 9.0.x"
    echo.
    pause
    exit /b 1
)

echo.
echo Verificando espacio en disco...
for /f "tokens=3" %%a in ('dir /-c %ProgramFiles% ^| findstr /i bytes') do set free=%%a
if !free! GTR 52428800 (
    echo ✅ Espacio suficiente en disco
) else (
    echo ❌ Espacio insuficiente en disco
    echo Se requieren al menos 50 MB libres
    pause
    exit /b 1
)

echo.
echo ✅ Todos los requisitos están satisfechos
echo Puedes proceder con la instalación.
echo.
pause
"@

$RequirementsCheck | Out-File -FilePath "$InstallerPath\CheckRequirements.bat" -Encoding ASCII

# Crear autorun opcional
$AutorunContent = @"
[autorun]
open=Install.bat
icon=App\FlujoDeCajaApp.exe
label=Instalar FlujoCaja
"@

$AutorunContent | Out-File -FilePath "$InstallerPath\autorun.inf" -Encoding ASCII

# Crear archivo de información del instalador
$InstallerInfo = @"
{
    "name": "$AppDisplayName",
    "version": "$Version",
    "publisher": "$Publisher",
    "created": "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')",
    "target_framework": "net9.0-windows",
    "requires_admin": false,
    "install_size_mb": $([Math]::Round((Get-ChildItem "$InstallerPath\App" -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB, 2)),
    "files": {
        "installer": "Install.bat",
        "requirements_check": "CheckRequirements.bat",
        "uninstaller": "Created during installation",
        "readme": "README.txt"
    }
}
"@

$InstallerInfo | Out-File -FilePath "$InstallerPath\installer-info.json" -Encoding UTF8

# Mostrar resumen
Show-Info "¡Instalador creado exitosamente!"
Write-Host ""
Write-Host "📁 Ubicación del instalador: $InstallerPath" -ForegroundColor Yellow
Write-Host "📦 Archivos generados:" -ForegroundColor Yellow
Write-Host "   - Install.bat (Instalador principal)" -ForegroundColor Gray
Write-Host "   - CheckRequirements.bat (Verificador de requisitos)" -ForegroundColor Gray
Write-Host "   - README.txt (Instrucciones)" -ForegroundColor Gray
Write-Host "   - App\ (Archivos de la aplicación)" -ForegroundColor Gray
Write-Host ""
Write-Host "🚀 Para distribuir:" -ForegroundColor Green
Write-Host "   1. Comprime toda la carpeta '$InstallerPath'" -ForegroundColor Gray
Write-Host "   2. Envía el archivo ZIP al usuario final" -ForegroundColor Gray
Write-Host "   3. El usuario debe extraer y ejecutar 'Install.bat' como administrador" -ForegroundColor Gray
Write-Host ""
Write-Host "💡 Tamaño del instalador: $([Math]::Round((Get-ChildItem $InstallerPath -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB, 2)) MB" -ForegroundColor Cyan

# Preguntar si crear ZIP
$createZip = Read-Host "¿Deseas crear un archivo ZIP del instalador? (s/n)"
if ($createZip -eq "s" -or $createZip -eq "S") {
    Show-Info "Creando archivo ZIP..."
    $zipPath = "FlujoCaja-Installer-v$Version.zip"
    Compress-Archive -Path "$InstallerPath\*" -DestinationPath $zipPath -Force
    Show-Info "Archivo ZIP creado: $zipPath"
}

Write-Host ""
Write-Host "✅ ¡Proceso completado!" -ForegroundColor Green