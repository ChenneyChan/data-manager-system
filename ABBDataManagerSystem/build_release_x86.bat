@echo off
setlocal

REM ============================================================
REM  ABBDataManagerSystem - Release x86 build script
REM  Uses MSBuild from Visual Studio 18 Community
REM ============================================================

set "MSBUILD=D:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"
set "PROJECT=%~dp0ABBDataManagerSystem.csproj"

echo.
echo [1/2] Restore NuGet packages ...
"%MSBUILD%" "%PROJECT%" /t:Restore /p:Platform=x86 /p:Configuration=Release /v:minimal /nologo
if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Restore failed, exit code: %errorlevel%
    exit /b %errorlevel%
)

echo.
echo [2/2] Build ...
"%MSBUILD%" "%PROJECT%" /t:Build /p:Platform=x86 /p:Configuration=Release /v:minimal /nologo
if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Build failed, exit code: %errorlevel%
    exit /b %errorlevel%
)

echo.
echo [OK] Build succeeded.
echo Output: %~dp0bin\x86\Release\net6.0-windows\
endlocal
