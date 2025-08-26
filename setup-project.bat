@echo off
echo ========================================
echo   Playwright BDD Project Setup
echo ========================================

echo.
echo 1. Cleaning and building project...
call dotnet clean
if %errorlevel% neq 0 (
    echo Error: Failed to clean project
    pause
    exit /b 1
)

call dotnet build
if %errorlevel% neq 0 (
    echo Error: Failed to build project
    pause
    exit /b 1
)

echo Project built successfully

echo.
echo 2. Installing Playwright browsers...

REM Try method 1: Using pwsh
echo Trying PowerShell method...
pwsh -Command "& { try { .\bin\Debug\net8.0\playwright.ps1 install } catch { exit 1 } }"
if %errorlevel% equ 0 (
    echo Browsers installed successfully using PowerShell!
    goto :success
)

REM Method 1 failed, try method 2: dotnet tool
echo PowerShell method failed, trying dotnet tool method...
call dotnet tool install --global Microsoft.Playwright.CLI
if %errorlevel% neq 0 (
    echo Warning: Failed to install Playwright CLI tool
) else (
    call playwright install
    if %errorlevel% equ 0 (
        echo Browsers installed successfully using dotnet tool!
        goto :success
    )
)

REM Method 2 failed, try method 3: Manual pwsh
echo Trying direct PowerShell Core installation...
for /r %%i in (playwright.ps1) do (
    if exist "%%i" (
        pushd "%%~pi"
        pwsh ./playwright.ps1 install
        if %errorlevel% equ 0 (
            echo Browsers installed successfully!
            popd
            goto :success
        )
        popd
    )
)

echo.
echo ERROR: All installation methods failed!
echo.
echo Please try one of these manual methods:
echo 1. Install PowerShell Core: winget install Microsoft.PowerShell
echo 2. Then run: pwsh bin\Debug\net8.0\playwright.ps1 install
echo 3. Or install globally: dotnet tool install --global Microsoft.Playwright.CLI
echo 4. Then run: playwright install
echo.
pause
exit /b 1

:success
echo.
echo ========================================
echo   Setup Complete!
echo ========================================
echo.
echo Next steps:
echo   1. Run single test: dotnet test --filter "SuccessfulLoginWithValidCredentials"
echo   2. If successful, run all: dotnet test
echo   3. Check TestResults folder for reports
echo.
echo Troubleshooting:
echo   - Set Headless = false in AppConfig.cs to see browser
echo   - Check console output for errors
echo   - Disable antivirus temporarily if browsers fail to launch
echo.
echo Happy testing! 🎭
pause