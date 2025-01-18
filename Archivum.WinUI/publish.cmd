@echo off
setlocal enabledelayedexpansion
cd %~dp0
set VERSION=0.0.0.0

dotnet publish -c Release -f net9.0-windows10.0.19041.0 ^
 --self-contained ^
 -o bin/exe ^
 /p:Platform=x64 ^
 /p:PublishSingleFile=true ^
 /p:EnableMsixTooling=true ^
 /p:SelfContained=true ^
 /p:WindowsPackageType=None ^
 /p:GenerateDocumentationFile=false ^
 /p:DebugType=embedded

if errorlevel 1 exit /b

mv -f bin\exe\Archivum.WinUI.exe bin\Archivum.WinUI.exe

if exist ..\Archivum.WinUI.%VERSION%.zip (
    del ..\Archivum.WinUI.%VERSION%.zip
)

start 7za a -tzip -mx=9 -mmt=%NUMBER_OF_PROCESSORS% ^
 ..\Archivum.WinUI.%VERSION%.zip ^
 .\bin\Archivum.WinUI.exe ^
 README.txt
