@echo off
setlocal

echo ===================================================
echo       GigMarket Database Wipe Automator
echo ===================================================
echo.

echo [1/2] Dropping the database...
dotnet ef database drop --force -p "GigMarket.Infrastructure" -s "GigMarket.Api"

if %errorlevel% neq 0 (
	echo.
	echo [ERROR] Failed to drop the database.
	pause
	exit /b %errorlevel%
)

echo.
echo [2/2] Recreating the database with latest migrations...
dotnet ef database update -p "GigMarket.Infrastructure" -s "GigMarket.Api"

if %errorlevel% neq 0 (
	echo.
	echo [ERROR] Failed to recreate/update the database.
	pause
	exit /b %errorlevel%
)

echo.
echo ===================================================
echo       Success! Database has been wiped and rebuilt.
echo ===================================================
pause
