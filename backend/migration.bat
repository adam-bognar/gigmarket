@echo off
setlocal

echo ===================================================
echo       GigMarket EF Core Automator
echo ===================================================
echo.

set /p MigrationName="Enter the name for the new migration: "

if "%MigrationName%"=="" (
    echo [ERROR] You must provide a migration name!
    pause
    exit /b
)

echo.
echo [1/2] Creating migration '%MigrationName%'...
dotnet ef migrations add "%MigrationName%" -p "GigMarket.Infrastructure" -s "GigMarket.Api"

if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Failed to create migration. Fix your C# errors and try again.
    pause
    exit /b %errorlevel%
)

echo.
echo [2/2] Updating the database...
dotnet ef database update -p "GigMarket.Infrastructure" -s "GigMarket.Api"

if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Failed to update the database.
    pause
    exit /b %errorlevel%
)

echo.
echo ===================================================
echo       Success! Database is up to date.
echo ===================================================
pause