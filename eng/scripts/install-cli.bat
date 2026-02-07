@echo off
setlocal

set SOLUTION_DIR=%~dp0..\..
set PROJECT_DIR=%SOLUTION_DIR%\src\TestId.Cli

echo Packing TestId.Cli...
dotnet pack "%PROJECT_DIR%\TestId.Cli.csproj" -c Release -o "%PROJECT_DIR%\nupkg"
if %ERRORLEVEL% neq 0 (
    echo Failed to pack TestId.Cli.
    exit /b 1
)

echo Uninstalling existing TestId.Cli tool (if any)...
dotnet tool uninstall -g TestId.Cli 2>nul

echo Installing TestId.Cli as a global tool...
dotnet tool install -g TestId.Cli --add-source "%PROJECT_DIR%\nupkg"
if %ERRORLEVEL% neq 0 (
    echo Failed to install TestId.Cli.
    exit /b 1
)

echo TestId.Cli installed successfully. Run 'testid' to use it.
