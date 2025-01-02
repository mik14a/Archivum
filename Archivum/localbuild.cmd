setlocal

set APP_DIR=D:\Apps\Archivum

dotnet publish -c Release -f net9.0-windows10.0.19041.0 ^
 --self-contained ^
 -o %APP_DIR% ^
 /p:Platform=x64 ^
 /p:PublishSingleFile=true ^
 /p:EnableMsixTooling=true ^
 /p:SelfContained=true ^
 /p:WindowsPackageType=None ^
 /p:GenerateDocumentationFile=false ^
 /p:DebugType=embedded

if errorlevel 1 (
    echo Build failed
    exit /b 1
)

start %APP_DIR%

endlocal
