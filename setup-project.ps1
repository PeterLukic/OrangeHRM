# PowerShell script to set up the Playwright BDD project
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Playwright BDD Project Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Function to check if command exists
function Test-CommandExists {
    param($Command)
    $null = Get-Command $Command -ErrorAction SilentlyContinue
    return $?
}

# Step 1: Clean and build the project
Write-Host "`n1. Cleaning and building project..." -ForegroundColor Yellow
try {
    dotnet clean
    Write-Host "Project cleaned successfully" -ForegroundColor Green
    
    dotnet build
    Write-Host "Project built successfully" -ForegroundColor Green
}
catch {
    Write-Host "Error building project: $_" -ForegroundColor Red
    exit 1
}

# Step 2: Check for PowerShell Core
Write-Host "`n2. Checking PowerShell installation..." -ForegroundColor Yellow
if (-not (Test-CommandExists "pwsh")) {
    Write-Host "PowerShell Core not found. Please install it:" -ForegroundColor Red
    Write-Host "  Option 1: winget install Microsoft.PowerShell" -ForegroundColor Gray
    Write-Host "  Option 2: Download from https://github.com/PowerShell/PowerShell/releases" -ForegroundColor Gray
    Write-Host "  Option 3: Use alternative installation method below" -ForegroundColor Gray
    
    # Alternative method using dotnet tool
    Write-Host "`nTrying alternative installation method..." -ForegroundColor Yellow
    try {
        dotnet tool install --global Microsoft.Playwright.CLI
        Write-Host "Playwright CLI tool installed" -ForegroundColor Green
        
        playwright install
        Write-Host "Browsers installed using Playwright CLI" -ForegroundColor Green
        
        Write-Host "`nSetup completed using alternative method!" -ForegroundColor Green
        Write-Host "You can now run: dotnet test" -ForegroundColor Cyan
        exit 0
    }
    catch {
        Write-Host "Alternative installation also failed: $_" -ForegroundColor Red
        Write-Host "Please install PowerShell Core manually and try again" -ForegroundColor Red
        exit 1
    }
}
else {
    Write-Host "PowerShell Core found" -ForegroundColor Green
}

# Step 3: Install Playwright browsers
Write-Host "`n3. Installing Playwright browsers..." -ForegroundColor Yellow

# Find the correct path for playwright.ps1
$playwrightScript = Get-ChildItem -Path "bin" -Filter "playwright.ps1" -Recurse | Select-Object -First 1

if ($playwrightScript) {
    Write-Host "Found Playwright script at: $($playwrightScript.FullName)" -ForegroundColor Gray
    
    try {
        # Change to the directory containing the script
        Push-Location -Path $playwrightScript.Directory
        
        # Run the installation
        pwsh ./playwright.ps1 install
        
        Write-Host "Playwright browsers installed successfully!" -ForegroundColor Green
        
        # Return to original directory
        Pop-Location
    }
    catch {
        Write-Host "Error installing browsers: $_" -ForegroundColor Red
        Pop-Location
        
        # Try alternative method
        Write-Host "Trying alternative installation method..." -ForegroundColor Yellow
        try {
            dotnet tool install --global Microsoft.Playwright.CLI --version 1.42.0
            playwright install
            Write-Host "Browsers installed using alternative method!" -ForegroundColor Green
        }
        catch {
            Write-Host "Alternative installation failed: $_" -ForegroundColor Red
            exit 1
        }
    }
}
else {
    Write-Host "Playwright script not found. Trying alternative method..." -ForegroundColor Yellow
    try {
        dotnet tool install --global Microsoft.Playwright.CLI --version 1.42.0
        playwright install
        Write-Host "Browsers installed using Playwright CLI!" -ForegroundColor Green
    }
    catch {
        Write-Host "Failed to install browsers: $_" -ForegroundColor Red
        exit 1
    }
}

# Step 4: Verify installation
Write-Host "`n4. Verifying installation..." -ForegroundColor Yellow
try {
    # Check if browsers are installed by looking for the chromium directory
    $playwrightDir = "$env:USERPROFILE\AppData\Local\ms-playwright"
    if (Test-Path $playwrightDir) {
        $chromiumDirs = Get-ChildItem -Path $playwrightDir -Directory | Where-Object { $_.Name -like "chromium-*" }
        if ($chromiumDirs.Count -gt 0) {
            Write-Host "Chromium browser found at: $($chromiumDirs[0].FullName)" -ForegroundColor Green
        }
        else {
            Write-Host "Warning: Chromium browser directory not found" -ForegroundColor Yellow
        }
    }
    
    # Test if playwright CLI is available
    if (Test-CommandExists "playwright") {
        Write-Host "Playwright CLI is available" -ForegroundColor Green
    }
}
catch {
    Write-Host "Verification completed with warnings" -ForegroundColor Yellow
}

# Step 5: Final instructions
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "  1. Run a single test first: dotnet test --filter 'SuccessfulLoginWithValidCredentials'" -ForegroundColor Gray
Write-Host "  2. If successful, run all tests: dotnet test" -ForegroundColor Gray
Write-Host "  3. Check the TestResults folder for reports and screenshots" -ForegroundColor Gray
Write-Host "`nTroubleshooting:" -ForegroundColor Yellow
Write-Host "  - If tests still fail, set Headless = false in AppConfig.cs to see the browser" -ForegroundColor Gray
Write-Host "  - Check the console output for detailed error messages" -ForegroundColor Gray
Write-Host "  - Ensure no antivirus software is blocking the browsers" -ForegroundColor Gray

Write-Host "`nHappy testing! 🎭" -ForegroundColor Cyan