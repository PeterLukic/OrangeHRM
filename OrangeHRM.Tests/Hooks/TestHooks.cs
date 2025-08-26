using AventStack.ExtentReports;
using OrangeHRM.Tests.Drivers;
using OrangeHRM.Tests.Utils;
using TechTalk.SpecFlow;

namespace OrangeHRM.Tests.Hooks
{
    [Binding]
    public class TestHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;
        private readonly PlaywrightDriver _playwrightDriver;

        public TestHooks(ScenarioContext scenarioContext, FeatureContext featureContext, PlaywrightDriver playwrightDriver)
        {
            _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
            _featureContext = featureContext ?? throw new ArgumentNullException(nameof(featureContext));
            _playwrightDriver = playwrightDriver ?? throw new ArgumentNullException(nameof(playwrightDriver));
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            try
            {
                Console.WriteLine("Initializing test run...");
                // Initialize the report
                ReportingUtil.GetExtentReports();
                Console.WriteLine("Test run initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize reporting: {ex.Message}");
                // Don't throw here as reporting shouldn't break tests
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            try
            {
                Console.WriteLine("Finalizing test run...");
                // Flush the report
                ReportingUtil.FlushReport();
                Console.WriteLine("Test run finalized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to flush report: {ex.Message}");
            }
        }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            try
            {
                var featureTitle = featureContext?.FeatureInfo?.Title ?? "Unknown Feature";
                Console.WriteLine($"Starting feature: {featureTitle}");
                ReportingUtil.CreateFeature(featureTitle);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create feature in report: {ex.Message}");
            }
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            try
            {
                var scenarioTitle = _scenarioContext?.ScenarioInfo?.Title ?? "Unknown Scenario";
                Console.WriteLine($"Starting scenario: {scenarioTitle}");
                ReportingUtil.CreateScenario(scenarioTitle);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create scenario in report: {ex.Message}");
            }
        }

        [AfterStep]
        public async Task AfterStep()
        {
            try
            {
                // Check if step context is available
                if (ScenarioStepContext.Current?.StepInfo == null)
                {
                    Console.WriteLine("Step context not available, skipping step reporting");
                    return;
                }

                var stepInfo = ScenarioStepContext.Current.StepInfo;
                var stepType = stepInfo.StepDefinitionType.ToString();
                var stepText = stepInfo.Text ?? "Unknown Step";

                Console.WriteLine($"Processing step: {stepType} {stepText}");

                // Determine step status
                Status stepStatus;
                var error = _scenarioContext.TestError;
                string? screenshotPath = null;

                if (error == null)
                {
                    stepStatus = Status.Pass;
                    Console.WriteLine("Step passed");
                }
                else
                {
                    stepStatus = Status.Fail;
                    Console.WriteLine($"Step failed: {error.Message}");

                    // Capture screenshot on failure
                    try
                    {
                        var scenarioTitle = _scenarioContext?.ScenarioInfo?.Title ?? "Unknown";
                        var sanitizedTitle = SanitizeFileName(scenarioTitle);
                        screenshotPath = await ScreenshotUtil.CaptureScreenshotAsync(
                            _playwrightDriver.Page,
                            $"Error_{sanitizedTitle}_{DateTime.Now:yyyyMMdd_HHmmss}");

                        Console.WriteLine($"Error screenshot captured: {screenshotPath}");
                    }
                    catch (Exception screenshotEx)
                    {
                        Console.WriteLine($"Failed to capture error screenshot: {screenshotEx.Message}");
                    }
                }

                // Parse step type safely
                if (Enum.TryParse<StepType>(stepType, out var parsedStepType))
                {
                    // Log step with status
                    ReportingUtil.LogStep(
                        parsedStepType,
                        stepText,
                        stepStatus,
                        error?.Message ?? "");

                    // Log screenshot if captured
                    if (!string.IsNullOrEmpty(screenshotPath))
                    {
                        ReportingUtil.LogScreenshot(screenshotPath);
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to parse step type: {stepType}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to process after step: {ex.Message}");
            }
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            try
            {
                var scenarioTitle = _scenarioContext?.ScenarioInfo?.Title ?? "Unknown";
                Console.WriteLine($"Completing scenario: {scenarioTitle}");

                // Take screenshot after each scenario for documentation (only if no error)
                if (_scenarioContext.TestError == null)
                {
                    try
                    {
                        var sanitizedTitle = SanitizeFileName(scenarioTitle);
                        var screenshotPath = await ScreenshotUtil.CaptureScreenshotAsync(
                            _playwrightDriver.Page,
                            $"Final_{sanitizedTitle}_{DateTime.Now:yyyyMMdd_HHmmss}");

                        if (!string.IsNullOrEmpty(screenshotPath))
                        {
                            ReportingUtil.LogScreenshot(screenshotPath);
                            Console.WriteLine($"Final screenshot captured: {screenshotPath}");
                        }
                    }
                    catch (Exception screenshotEx)
                    {
                        Console.WriteLine($"Failed to capture final screenshot: {screenshotEx.Message}");
                    }
                }

                Console.WriteLine($"Scenario completed: {scenarioTitle}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to complete scenario: {ex.Message}");
            }
        }

        /// <summary>
        /// Sanitizes filename by removing invalid characters
        /// </summary>
        /// <param name="fileName">Original filename</param>
        /// <returns>Sanitized filename</returns>
        private static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return "Unknown";

            var invalidChars = System.IO.Path.GetInvalidFileNameChars();
            var sanitized = fileName;

            foreach (var invalidChar in invalidChars)
            {
                sanitized = sanitized.Replace(invalidChar, '_');
            }

            // Also replace spaces with underscores for better file handling
            sanitized = sanitized.Replace(' ', '_');

            // Limit length to avoid file system issues
            if (sanitized.Length > 50)
                sanitized = sanitized.Substring(0, 50);

            return sanitized;
        }
    }
}