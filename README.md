# C# Playwright BDD Framework

This is a test automation framework built with C#, Playwright, and SpecFlow for BDD-style testing. The framework is designed to test the OrangeHRM application, with a focus on the login functionality.

## Features

- **BDD Approach**: Uses SpecFlow for behavior-driven development
- **Page Object Model**: Implements the Page Object pattern for better maintainability
- **Parallel Execution**: Configured to run tests in parallel for faster execution
- **Reporting**: Integrated with ExtentReports for detailed test reports
- **Screenshot Capture**: Automatically captures screenshots on test failures
- **Configuration Management**: Centralized configuration for test settings

## Prerequisites

- Visual Studio 2022 or later
- .NET 8.0 SDK
- SpecFlow extension for Visual Studio

## Project Structure

```
OrangeHRM.Tests/
├── Config/                  # Configuration files
│   └── AppConfig.cs         # Application configuration
├── Drivers/                 # Browser drivers
│   └── PlaywrightDriver.cs  # Playwright driver implementation
├── Features/                # BDD feature files
│   └── Login.feature        # Login feature scenarios
├── Hooks/                   # SpecFlow hooks
│   └── TestHooks.cs         # Test setup and teardown hooks
├── Pages/                   # Page Object Models
│   ├── BasePage.cs          # Base page with common methods
│   └── LoginPage.cs         # Login page object
├── StepDefinitions/         # Step definition files
│   └── LoginSteps.cs        # Login step implementations
├── Support/                 # Support files
│   └── ContextInjection.cs  # Dependency injection setup
├── Utils/                   # Utility classes
│   ├── ReportingUtil.cs     # Reporting utilities
│   └── ScreenshotUtil.cs    # Screenshot utilities
├── AssemblyInfo.cs          # Assembly configuration
├── specflow.json            # SpecFlow configuration
└── OrangeHRM.Tests.csproj   # Project file
```

## Setup Instructions

1. Clone the repository
2. Open the solution in Visual Studio
3. Restore NuGet packages
4. Install Playwright browsers by running the following command in the Package Manager Console:

```
pwsh bin\Debug\net8.0\playwright.ps1 install
```

## Running Tests

### From Visual Studio

1. Open Test Explorer (Test > Test Explorer)
2. Click "Run All" to execute all tests

### From Command Line

```
dotnet test
```

### Running Tests in Parallel

The framework is configured to run tests in parallel by default. The number of parallel workers is set to 3 in the `AssemblyInfo.cs` file. You can modify this value based on your system capabilities.

## Configuration

Test configuration is managed in the `AppConfig.cs` file. You can modify the following settings:

- **BaseUrl**: The URL of the application under test
- **Username**: Default username for login
- **Password**: Default password for login
- **DefaultTimeout**: Default timeout for Playwright actions (in milliseconds)
- **Browser**: Browser to use for testing (chromium, firefox, webkit)
- **Headless**: Whether to run tests in headless mode

## Reports

Test reports are generated using ExtentReports and can be found in the `TestResults/Report` directory after test execution. Screenshots are saved in the `TestResults/Screenshots` directory.

## Adding New Tests

1. Create a new feature file in the `Features` directory
2. Implement step definitions in the `StepDefinitions` directory
3. Create page objects in the `Pages` directory if needed

## Best Practices

- Keep scenarios focused on business requirements
- Maintain page objects for better maintainability
- Use tags to categorize and filter tests
- Keep step definitions simple and reusable
- Use scenario outlines for data-driven testing

## Troubleshooting

- If tests fail with browser-related errors, ensure Playwright browsers are installed correctly
- For parallel execution issues, try reducing the number of parallel workers
- Check the generated reports and screenshots for detailed error information