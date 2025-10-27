@echo off
setlocal enabledelayedexpansion

set COVERAGE_DIR=coverage
set REPORT_DIR=%COVERAGE_DIR%\report
set FILTER_FILE=coverage-filters.txt

rmdir /s /q %COVERAGE_DIR%
mkdir %COVERAGE_DIR%

echo Searching test projects

for /r %%f in (*.csproj) do (
    echo %%f | findstr /i "test" >nul
    if !errorlevel! == 0 (
        echo Running tests for %%f...
        dotnet test "%%f" --collect:"XPlat Code Coverage" --results-directory %COVERAGE_DIR% --no-build
    )
)

REM Converte patterns do coverage-filters.txt para argumentos -filefilters
set FILTERS=
for /f "usebackq delims=" %%l in ("%FILTER_FILE%") do (
    set FILTERS=!FILTERS! -filefilters:-%%l
)

echo Generating report...
dotnet tool install --global dotnet-reportgenerator-globaltool >nul 2>&1
reportgenerator -reports:%COVERAGE_DIR%\**\coverage.cobertura.xml -targetdir:%REPORT_DIR% -reporttypes:Html %FILTERS%

echo Report ready: %REPORT_DIR%\index.html
pause