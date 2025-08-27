using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using System;
using System.IO;

namespace OrangeHRM.Tests.Utils
{
    public class ReportingUtil
    {
        private static ExtentReports _extent;
        private static ExtentTest _feature;
        private static ExtentTest _scenario;
        private static ExtentTest _currentStep; // Add this to track current step

        public static ExtentReports GetExtentReports()
        {
            if (_extent == null)
            {
                InitializeReport();
            }
            return _extent;
        }

        private static void InitializeReport()
        {
            var reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults", "Report");
            Directory.CreateDirectory(reportPath);

            var htmlReporter = new ExtentHtmlReporter(Path.Combine(reportPath, $"TestReport_{DateTime.Now:yyyyMMdd_HHmmss}.html"));

            // Enhanced configuration for better step visibility
            htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Dark;
            htmlReporter.Config.DocumentTitle = "OrangeHRM Test Report";
            htmlReporter.Config.ReportName = "OrangeHRM Automation Test Report";
            //htmlReporter.Config.TimeStampFormat = "yyyy-MM-dd HH:mm:ss";

            // Enable step details
            htmlReporter.Config.EnableTimeline = true;

            _extent = new ExtentReports();
            _extent.AttachReporter(htmlReporter);

            _extent.AddSystemInfo("Environment", "Test");
            _extent.AddSystemInfo("Browser", "Chromium");
            _extent.AddSystemInfo("OS", Environment.OSVersion.ToString());
            _extent.AddSystemInfo("Framework", ".NET 8.0");
            _extent.AddSystemInfo("Test Framework", "NUnit + SpecFlow");
        }

        public static void CreateFeature(string featureName)
        {
            _feature = GetExtentReports().CreateTest<Feature>(featureName);
            Console.WriteLine($"[REPORT] Created feature: {featureName}");
        }

        public static void CreateScenario(string scenarioName)
        {
            if (_feature == null)
            {
                Console.WriteLine("[REPORT] Warning: Feature is null, creating default feature");
                CreateFeature("Unknown Feature");
            }

            _scenario = _feature.CreateNode<Scenario>(scenarioName);
            Console.WriteLine($"[REPORT] Created scenario: {scenarioName}");
        }

        // Updated method to properly create and track steps
        public static void LogStep(StepType stepType, string stepName, Status status, string details = "")
        {
            if (_scenario == null)
            {
                Console.WriteLine($"[REPORT] Warning: Scenario is null for step: {stepName}");
                return;
            }

            try
            {
                // Create the step node based on type
                ExtentTest step = stepType switch
                {
                    StepType.Given => _scenario.CreateNode<Given>(stepName),
                    StepType.When => _scenario.CreateNode<When>(stepName),
                    StepType.Then => _scenario.CreateNode<Then>(stepName),
                    StepType.And => _scenario.CreateNode<And>(stepName),
                    _ => _scenario.CreateNode<Given>(stepName)
                };

                // Store reference for screenshots/additional logging
                _currentStep = step;

                // Log the step with status and details
                if (!string.IsNullOrEmpty(details))
                {
                    step.Log(status, $"{stepName}<br/><strong>Details:</strong> {details}");
                }
                else
                {
                    step.Log(status, stepName);
                }

                // Add timestamp
                step.Info($"<i>Executed at: {DateTime.Now:HH:mm:ss.fff}</i>");

                Console.WriteLine($"[REPORT] Logged step: {stepType} - {stepName} - {status}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPORT] Error logging step: {ex.Message}");
            }
        }

        // Alternative method for creating steps without immediate status (if needed)
        public static void CreateStep(StepType stepType, string stepName)
        {
            if (_scenario == null)
            {
                Console.WriteLine($"[REPORT] Warning: Scenario is null for step creation: {stepName}");
                return;
            }

            try
            {
                _currentStep = stepType switch
                {
                    StepType.Given => _scenario.CreateNode<Given>(stepName),
                    StepType.When => _scenario.CreateNode<When>(stepName),
                    StepType.Then => _scenario.CreateNode<Then>(stepName),
                    StepType.And => _scenario.CreateNode<And>(stepName),
                    _ => _scenario.CreateNode<Given>(stepName)
                };

                Console.WriteLine($"[REPORT] Created step node: {stepType} - {stepName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPORT] Error creating step: {ex.Message}");
            }
        }

        public static void LogScreenshot(string screenshotPath)
        {
            try
            {
                if (_currentStep != null)
                {
                    // Add screenshot to current step
                    _currentStep.AddScreenCaptureFromPath(screenshotPath);
                    Console.WriteLine($"[REPORT] Added screenshot to current step: {screenshotPath}");
                }
                else if (_scenario != null)
                {
                    // Fallback: add to scenario
                    _scenario.AddScreenCaptureFromPath(screenshotPath);
                    Console.WriteLine($"[REPORT] Added screenshot to scenario: {screenshotPath}");
                }
                else
                {
                    Console.WriteLine($"[REPORT] Warning: No active step or scenario for screenshot: {screenshotPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPORT] Error adding screenshot: {ex.Message}");
            }
        }

        // Method to log additional info to current step
        public static void LogStepInfo(string info)
        {
            try
            {
                if (_currentStep != null)
                {
                    _currentStep.Info(info);
                }
                else if (_scenario != null)
                {
                    _scenario.Info(info);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPORT] Error logging step info: {ex.Message}");
            }
        }

        // Method to log step failure details
        public static void LogStepFailure(string errorMessage, string stackTrace = "")
        {
            try
            {
                if (_currentStep != null)
                {
                    _currentStep.Fail($"<strong>Error:</strong> {errorMessage}");
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        _currentStep.Fail($"<strong>Stack Trace:</strong><br/><pre>{stackTrace}</pre>");
                    }
                }
                else if (_scenario != null)
                {
                    _scenario.Fail($"<strong>Error:</strong> {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPORT] Error logging failure: {ex.Message}");
            }
        }

        public static void FlushReport()
        {
            try
            {
                _extent?.Flush();
                Console.WriteLine("[REPORT] Report flushed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPORT] Error flushing report: {ex.Message}");
            }
        }

        // Clean up references (optional, for memory management)
        public static void ResetCurrentReferences()
        {
            _currentStep = null;
        }
    }

    public enum StepType
    {
        Given,
        When,
        Then,
        And
    }
}