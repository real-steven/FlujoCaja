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

echo Creando instalador...

REM Crear instalador principal
echo @echo off > "!OUTPUT_DIR!\Install.bat"
echo setlocal EnableDelayedExpansion >> "!OUTPUT_DIR!\Install.bat"
echo. >> "!OUTPUT_DIR!\Install.bat"
echo echo ===================================== >> "!OUTPUT_DIR!\Install.bat"
echo echo   FlujoCaja - Sistema de Gestión >> "!OUTPUT_DIR!\Install.bat"
echo echo   Instalador v!VERSION! >> "!OUTPUT_DIR!\Install.bat"
echo echo ===================================== >> "!OUTPUT_DIR!\Install.bat"
echo echo. >> "!OUTPUT_DIR!\Install.bat"
echo. >> "!OUTPUT_DIR!\Install.bat"
echo set "INSTALL_DIR=%%ProgramFiles%%\FlujoCaja" >> "!OUTPUT_DIR!\Install.bat"
echo. >> "!OUTPUT_DIR!\Install.bat"
echo echo Instalando en: ^^!INSTALL_DIR^^! >> "!OUTPUT_DIR!\Install.bat"
echo echo. >> "!OUTPUT_DIR!\Install.bat"
echo. >> "!OUTPUT_DIR!\Install.bat"
echo if exist "^^!INSTALL_DIR^^!" ^( >> "!OUTPUT_DIR!\Install.bat"
echo     echo Eliminando instalación anterior... >> "!OUTPUT_DIR!\Install.bat"
echo     rmdir /s /q "^^!INSTALL_DIR^^!" >> "!OUTPUT_DIR!\Install.bat"
echo ^) >> "!OUTPUT_DIR!\Install.bat"
echo. >> "!OUTPUT_DIR!\Install.bat"
echo echo Creando directorio... >> "!OUTPUT_DIR!\Install.bat"
echo mkdir "^^!INSTALL_DIR^^!" >> "!OUTPUT_DIR!\Install.bat"
echo. >> "!OUTPUT_DIR!\Install.bat"
echo echo Copiando archivos... >> "!OUTPUT_DIR!\Install.bat"
echo xcopy /s /e /y "%%~dp0App\*" "^^!INSTALL_DIR^^!\" >> "!OUTPUT_DIR!\Install.bat"
echo. >> "!OUTPUT_DIR!\Install.bat"
echo if ^^!errorLevel^^! neq 0 ^( >> "!OUTPUT_DIR!\Install.bat"
echo     echo ERROR: No se pudieron copiar los archivos >> "!OUTPUT_DIR!\Install.bat"
echo     pause >> "!OUTPUT_DIR!\Install.bat"
echo     exit /b 1 >> "!OUTPUT_DIR!\Install.bat"
echo ^) >> "!OUTPUT_DIR!\Install.bat"
echo. >> "!OUTPUT_DIR!\Install.bat"
echo echo Creando acceso directo... >> "!OUTPUT_DIR!\Install.bat"
echo powershell -Command "^^^$WshShell = New-Object -comObject WScript.Shell; ^^^$Shortcut = ^^^$WshShell.CreateShortcut('^^^$env:PUBLIC\Desktop\FlujoCaja.lnk'); ^^^$Shortcut.TargetPath = '^^!INSTALL_DIR^^!\FlujoDeCajaApp.exe'; ^^^$Shortcut.WorkingDirectory = '^^!INSTALL_DIR^^!'; ^^^$Shortcut.Description = 'FlujoCaja - Sistema de Gestión'; ^^^$Shortcut.Save()" >> "!OUTPUT_DIR!\Install.bat"
echo. >> "!OUTPUT_DIR!\Install.bat"
echo echo. >> "!OUTPUT_DIR!\Install.bat"
echo echo ===================================== >> "!OUTPUT_DIR!\Install.bat"
echo echo ¡Instalación completada! >> "!OUTPUT_DIR!\Install.bat"
echo echo ===================================== >> "!OUTPUT_DIR!\Install.bat"
echo echo. >> "!OUTPUT_DIR!\Install.bat"
echo echo Se ha creado un acceso directo en el escritorio. >> "!OUTPUT_DIR!\Install.bat"
echo echo. >> "!OUTPUT_DIR!\Install.bat"
echo echo ¿Ejecutar FlujoCaja ahora? ^(S/N^) >> "!OUTPUT_DIR!\Install.bat"
echo set /p choice= >> "!OUTPUT_DIR!\Install.bat"
echo if /i "^^!choice^^!"=="S" start "" "^^!INSTALL_DIR^^!\FlujoDeCajaApp.exe" >> "!OUTPUT_DIR!\Install.bat"
echo pause >> "!OUTPUT_DIR!\Install.bat"

REM Crear archivo README
echo # FlujoCaja - Instalador v!VERSION! > "!OUTPUT_DIR!\README.txt"
echo. >> "!OUTPUT_DIR!\README.txt"
echo ## Instrucciones de Instalación: >> "!OUTPUT_DIR!\README.txt"
echo. >> "!OUTPUT_DIR!\README.txt"
echo 1. Ejecuta Install.bat como administrador >> "!OUTPUT_DIR!\README.txt"
echo 2. Sigue las instrucciones en pantalla >> "!OUTPUT_DIR!\README.txt"
echo 3. La aplicación se instalará en C:\Program Files\FlujoCaja >> "!OUTPUT_DIR!\README.txt"
echo. >> "!OUTPUT_DIR!\README.txt"
echo ## Requisitos: >> "!OUTPUT_DIR!\README.txt"
echo - Windows 10 o superior >> "!OUTPUT_DIR!\README.txt"
echo - .NET 9.0 Runtime >> "!OUTPUT_DIR!\README.txt"
echo. >> "!OUTPUT_DIR!\README.txt"
echo Si .NET no está instalado, descárgalo desde: >> "!OUTPUT_DIR!\README.txt"
echo https://dotnet.microsoft.com/download/dotnet/9.0 >> "!OUTPUT_DIR!\README.txt"

echo ✅ Instalador creado en la carpeta: !OUTPUT_DIR!
echo.
echo 📁 Archivos generados:
echo    - Install.bat (ejecutar como administrador)
echo    - README.txt (instrucciones)
echo    - App\ (archivos de la aplicación)
echo.
echo 🚀 Para distribuir:
echo    1. Comprime la carpeta '!OUTPUT_DIR!'
echo    2. Envía el ZIP al usuario final
echo    3. El usuario extrae y ejecuta Install.bat
echo.
echo ✅ ¡Listo para distribuir!
echo.
pause