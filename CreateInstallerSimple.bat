@echo off
setlocal EnableDelayedExpansion

echo =====================================
echo   FlujoCaja - Generador de Instalador
echo =====================================
echo.

REM Verificar que estamos en el directorio correcto
if not exist "FlujoDeCajaApp.csproj" (
    echo ERROR: No se encontró FlujoDeCajaApp.csproj
    echo Ejecuta este script desde la raíz del proyecto.
    pause
    exit /b 1
)

REM Configuración
set "VERSION=1.0.0"
set "OUTPUT_DIR=Installer"
set "PUBLISH_DIR=bin\Release\net9.0-windows\publish"

echo Verificando .NET SDK...
dotnet --version >nul 2>&1
if !errorLevel! neq 0 (
    echo ERROR: No se encontró .NET SDK
    echo Descarga e instala .NET 9.0 SDK desde:
    echo https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo ✅ .NET SDK encontrado
echo.

REM Limpiar compilaciones anteriores
echo Limpiando compilaciones anteriores...
if exist "!PUBLISH_DIR!" rmdir /s /q "!PUBLISH_DIR!"
if exist "!OUTPUT_DIR!" rmdir /s /q "!OUTPUT_DIR!"

echo Compilando aplicación...
dotnet clean --configuration Release >nul
dotnet restore >nul
dotnet publish --configuration Release --framework net9.0-windows --output "!PUBLISH_DIR!" --self-contained false >nul

if !errorLevel! neq 0 (
    echo ERROR: Error durante la compilación
    pause
    exit /b 1
)

echo ✅ Compilación exitosa
echo.

REM Verificar ejecutable
if not exist "!PUBLISH_DIR!\FlujoDeCajaApp.exe" (
    echo ERROR: No se encontró el ejecutable compilado
    pause
    exit /b 1
)

echo Creando estructura del instalador...
mkdir "!OUTPUT_DIR!"
mkdir "!OUTPUT_DIR!\App"

echo Copiando archivos...
xcopy /s /e /y "!PUBLISH_DIR!\*" "!OUTPUT_DIR!\App\"

echo Creando instalador mejorado...

REM Crear script de instalación mejorado
(
echo @echo off
echo setlocal EnableDelayedExpansion
echo.
echo echo =====================================
echo echo   FlujoCaja - Sistema de Gestión
echo echo   Instalador v!VERSION!
echo echo =====================================
echo echo.
echo.
echo REM Verificar que se está ejecutando como administrador
echo net session ^>nul 2^>^&1
echo if %%errorLevel%% neq 0 ^(
echo     echo ERROR: Ejecuta este instalador como administrador
echo     echo Clic derecho -^> Ejecutar como administrador
echo     pause
echo     exit /b 1
echo ^)
echo.
echo set "INSTALL_DIR=%%ProgramFiles%%\FlujoCaja"
echo set "CURRENT_DIR=%%~dp0"
echo.
echo echo Instalando en: %%INSTALL_DIR%%
echo echo.
echo.
echo REM Eliminar instalación anterior si existe
echo if exist "%%INSTALL_DIR%%" ^(
echo     echo Eliminando instalación anterior...
echo     rmdir /s /q "%%INSTALL_DIR%%"
echo ^)
echo.
echo echo Creando directorio de instalación...
echo mkdir "%%INSTALL_DIR%%"
echo if %%errorLevel%% neq 0 ^(
echo     echo ERROR: No se pudo crear el directorio de instalación
echo     pause
echo     exit /b 1
echo ^)
echo.
echo echo Copiando archivos de la aplicación...
echo xcopy /s /e /y "%%CURRENT_DIR%%App\*" "%%INSTALL_DIR%%\"
echo if %%errorLevel%% neq 0 ^(
echo     echo ERROR: No se pudieron copiar los archivos
echo     pause
echo     exit /b 1
echo ^)
echo.
echo echo Creando acceso directo en el escritorio...
echo set "SHORTCUT_PATH=%%PUBLIC%%\Desktop\FlujoCaja.lnk"
echo if exist "%%SHORTCUT_PATH%%" del "%%SHORTCUT_PATH%%"
echo.
echo REM Crear acceso directo usando VBScript
echo echo Set oWS = WScript.CreateObject^("WScript.Shell"^) ^> "%%TEMP%%\CreateShortcut.vbs"
echo echo sLinkFile = "%%PUBLIC%%\Desktop\FlujoCaja.lnk" ^>^> "%%TEMP%%\CreateShortcut.vbs"
echo echo Set oLink = oWS.CreateShortcut^(sLinkFile^) ^>^> "%%TEMP%%\CreateShortcut.vbs"
echo echo oLink.TargetPath = "%%INSTALL_DIR%%\FlujoDeCajaApp.exe" ^>^> "%%TEMP%%\CreateShortcut.vbs"
echo echo oLink.WorkingDirectory = "%%INSTALL_DIR%%" ^>^> "%%TEMP%%\CreateShortcut.vbs"
echo echo oLink.Description = "FlujoCaja - Sistema de Gestión" ^>^> "%%TEMP%%\CreateShortcut.vbs"
echo echo oLink.Save ^>^> "%%TEMP%%\CreateShortcut.vbs"
echo.
echo cscript "%%TEMP%%\CreateShortcut.vbs" ^>nul
echo del "%%TEMP%%\CreateShortcut.vbs"
echo.
echo echo Creando entrada en Programas y características...
echo reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\FlujoCaja" /v "DisplayName" /t REG_SZ /d "FlujoCaja - Sistema de Gestión" /f ^>nul
echo reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\FlujoCaja" /v "DisplayVersion" /t REG_SZ /d "!VERSION!" /f ^>nul
echo reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\FlujoCaja" /v "InstallLocation" /t REG_SZ /d "%%INSTALL_DIR%%" /f ^>nul
echo reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\FlujoCaja" /v "UninstallString" /t REG_SZ /d "%%INSTALL_DIR%%\Uninstall.bat" /f ^>nul
echo.
echo echo.
echo echo =====================================
echo echo ¡Instalación completada exitosamente!
echo echo =====================================
echo echo.
echo echo La aplicación se ha instalado en:
echo echo %%INSTALL_DIR%%
echo echo.
echo echo Se ha creado un acceso directo en el escritorio.
echo echo.
echo echo ¿Ejecutar FlujoCaja ahora? ^(S/N^)
echo set /p choice=
echo if /i "%%choice%%"=="S" start "" "%%INSTALL_DIR%%\FlujoDeCajaApp.exe"
echo if /i "%%choice%%"=="s" start "" "%%INSTALL_DIR%%\FlujoDeCajaApp.exe"
echo.
echo pause
) > "!OUTPUT_DIR!\Install.bat"

REM Crear desinstalador
(
echo @echo off
echo echo Desinstalando FlujoCaja...
echo.
echo set "INSTALL_DIR=%%ProgramFiles%%\FlujoCaja"
echo.
echo REM Cerrar aplicación si está ejecutándose
echo taskkill /f /im FlujoDeCajaApp.exe ^>nul 2^>^&1
echo.
echo REM Eliminar acceso directo
echo if exist "%%PUBLIC%%\Desktop\FlujoCaja.lnk" del "%%PUBLIC%%\Desktop\FlujoCaja.lnk"
echo.
echo REM Eliminar archivos
echo if exist "%%INSTALL_DIR%%" rmdir /s /q "%%INSTALL_DIR%%"
echo.
echo REM Eliminar entrada del registro
echo reg delete "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\FlujoCaja" /f ^>nul 2^>^&1
echo.
echo echo FlujoCaja ha sido desinstalado.
echo pause
) > "!OUTPUT_DIR!\App\Uninstall.bat"

REM Crear archivo README mejorado
(
echo # FlujoCaja - Instalador v!VERSION!
echo # Generado el %date% a las %time%
echo.
echo ## Instrucciones de Instalación:
echo.
echo 1. **IMPORTANTE**: Clic derecho en Install.bat y seleccionar "Ejecutar como administrador"
echo 2. Sigue las instrucciones en pantalla
echo 3. La aplicación se instalará en C:\Program Files\FlujoCaja
echo 4. Se creará un acceso directo en el escritorio
echo.
echo ## Requisitos del Sistema:
echo - Windows 10 o superior
echo - .NET 9.0 Runtime
echo - Permisos de administrador para la instalación
echo.
echo ## Si .NET no está instalado:
echo Descarga .NET 9.0 Runtime desde:
echo https://dotnet.microsoft.com/download/dotnet/9.0
echo.
echo ## Para Desinstalar:
echo 1. Ir a C:\Program Files\FlujoCaja
echo 2. Ejecutar Uninstall.bat como administrador
echo.
echo ## Soporte:
echo Si tienes problemas con la instalación, verifica:
echo - Que tienes permisos de administrador
echo - Que .NET 9.0 está instalado
echo - Que no hay antivirus bloqueando la instalación
) > "!OUTPUT_DIR!\README.txt"

echo ✅ Instalador mejorado creado en la carpeta: !OUTPUT_DIR!
echo.
echo 📁 Archivos generados:
echo    - Install.bat (ejecutar como administrador)
echo    - README.txt (instrucciones detalladas)
echo    - App\ (archivos de la aplicación + desinstalador)
echo.
echo 🔧 Mejoras incluidas:
echo    ✅ Verificación de permisos de administrador
echo    ✅ Validación de errores en cada paso
echo    ✅ Creación de acceso directo con VBScript (más compatible)
echo    ✅ Registro en Programas y características
echo    ✅ Desinstalador incluido
echo.
echo 🚀 Para distribuir:
echo    1. Comprime la carpeta '!OUTPUT_DIR!'
echo    2. Envía el ZIP al usuario final
echo    3. El usuario extrae y ejecuta Install.bat COMO ADMINISTRADOR
echo.
echo ✅ ¡Listo para distribuir!
echo.
pause