@echo off

cd ..

dotnet run --project "GigMarket.Api" -- --seed-users

pause