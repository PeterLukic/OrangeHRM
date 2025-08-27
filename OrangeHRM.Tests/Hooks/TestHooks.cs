using AventStack.ExtentReports;
using Microsoft.Playwright;
using OrangeHRM.Tests.Drivers;
using OrangeHRM.Tests.Utils;
using System;
using System.Threading.Tasks;
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
                Console.WriteLine("=== INITIALIZING TEST RUN ===");
                ReportingUtil.GetExtentReports();
                Console.WriteLine("=== TEST RUN INITIALIZED ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize reporting: {ex.Message}");
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            try
            {
                Console.WriteLine("=== FINALIZING TEST RUN ===");
                ReportingUtil.FlushReport();
                Console.WriteLine("=== TEST RUN FINALIZED ===");
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
                Console.WriteLine($"=== STARTING FEATURE: {featureTitle} ===");
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
                var featureTitle = _featureContext?.FeatureInfo?.Title ?? "Unknown Feature";
                Console.WriteLine($"=== STARTING SCENARIO: {scenarioTitle} IN FEATURE: {featureTitle} ===");
                ReportingUtil.CreateScenario(scenarioTitle, featureTitle);
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
                // Get current step information
                var stepContext = ScenarioStepContext.Current;
                if (stepContext?.StepInfo == null)
                {
                    Console.WriteLine("⚠️ Step context not available");
                    return;
                }

                var stepInfo = stepContext.StepInfo;
                var stepType = stepInfo.StepDefinitionType.ToString();
                var stepText = stepInfo.Text ?? "Unknown Step";

                Console.WriteLine($"📝 Processing step: {stepType} {stepText}");

                // Determine step status
                Status stepStatus;
                var error = _scenarioContext.TestError;
                string? screenshotPath = null;

                if (error == null)
                {
                    stepStatus = Status.Pass;
                    Console.WriteLine($"✅ Step PASSED: {stepText}");
                }
                else
                {
                    stepStatus = Status.Fail;
                    Console.WriteLine($"❌ Step FAILED: {stepText} - {error.Message}");

                    // Capture screenshot on failure
                    try
                    {
                        var scenarioTitle = _scenarioContext?.ScenarioInfo?.Title ?? "Unknown";
                        var sanitizedTitle = SanitizeFileName(scenarioTitle);
                        screenshotPath = await ScreenshotUtil.CaptureScreenshotAsync(
                            _playwrightDriver.Page,
                            $"Error_{sanitizedTitle}_{DateTime.Now:yyyyMMdd_HHmmss}");

                        Console.WriteLine($"📸 Error screenshot captured: {screenshotPath}");
                    }
                    catch (Exception screenshotEx)
                    {
                        Console.WriteLine($"📸 Failed to capture error screenshot: {screenshotEx.Message}");
                    }
                }

                // Parse and log step to report
                if (Enum.TryParse<StepType>(stepType, out var parsedStepType))
                {
                    // Enhanced step details
                    var stepDetails = error != null
                        ? $"Error: {error.Message}"
                        : "Completed successfully";

                    // Log step with enhanced information
                    ReportingUtil.LogStep(
                        parsedStepType,
                        stepText,
                        stepStatus,
                        stepDetails);

                    // Add screenshot if captured
                    if (!string.IsNullOrEmpty(screenshotPath))
                    {
                        ReportingUtil.LogScreenshot(screenshotPath);
                    }

                    // Add additional context information
                    if (stepStatus == Status.Pass)
                    {
                        ReportingUtil.LogStepInfo($"Step executed successfully at {DateTime.Now:HH:mm:ss.fff}");
                    }
                    else
                    {
                        ReportingUtil.LogStepFailure(error?.Message ?? "Unknown error", error?.StackTrace ?? "");
                    }

                    Console.WriteLine($"📊 Step logged to report: {stepType} - {stepStatus}");
                }
                else
                {
                    Console.WriteLine($"⚠️ Failed to parse step type: {stepType}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to process after step: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            try
            {
                var scenarioTitle = _scenarioContext?.ScenarioInfo?.Title ?? "Unknown";
                var scenarioStatus = _scenarioContext.TestError == null ? "PASSED" : "FAILED";

                Console.WriteLine($"=== COMPLETING SCENARIO: {scenarioTitle} - {scenarioStatus} ===");

                // Take final screenshot if scenario passed
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
                            Console.WriteLine($"📸 Final screenshot captured: {screenshotPath}");
                        }
                    }
                    catch (Exception screenshotEx)
                    {
                        Console.WriteLine($"📸 Failed to capture final screenshot: {screenshotEx.Message}");
                    }
                }

                // Reset current references for next scenario
                ReportingUtil.ResetCurrentReferences();

                Console.WriteLine($"=== SCENARIO COMPLETED: {scenarioTitle} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to complete scenario: {ex.Message}");
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