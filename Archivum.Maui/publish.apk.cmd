@echo off
setlocal enabledelayedexpansion
cd %~dp0
set VERSION=0.0.0.0

dotnet publish -c Release -f net9.0-android -r android-arm64 ^
 -o bin\apk ^
 -p:AndroidPackageFormats=apk

if errorlevel 1 exit /b

mv -f bin\apk\pm.booth.kaihatsuikka.archivum.maui-Signed.apk bin\archivum.maui.%VERSION%.apk

if exist ..\Archivum.Maui.%VERSION%.apk.zip (
    del ..\Archivum.Maui.%VERSION%.apk.zip
)

start 7za a -tzip -mx=9 -mmt=%NUMBER_OF_PROCESSORS% ^
 ..\Archivum.Maui.%VERSION%.apk.zip ^
 .\bin\Archivum.Maui.%VERSION%.apk ^
 README.txt
