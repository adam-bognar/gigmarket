@echo off

set SERVER=localhost
set DATABASE=GigMarket
set LOGIN_TIMEOUT=30
set QUERY_TIMEOUT=30

sqlcmd -S "%SERVER%" -d "%DATABASE%" -E -N -C -l %LOGIN_TIMEOUT% -t %QUERY_TIMEOUT% -i "seed_gigs.sql"

pause