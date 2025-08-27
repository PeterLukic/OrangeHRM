using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace OrangeHRM.Tests.Utils
{
    public class ReportingUtil
    {
        private static ExtentReports _extent;
        private static readonly ConcurrentDictionary<string, ExtentTest> _features = new ConcurrentDictionary<string, ExtentTest>();
        private static readonly ConcurrentDictionary<int, ExtentTest> _scenarios = new ConcurrentDictionary<int, ExtentTest>();
        private static readonly ConcurrentDictionary<int, ExtentTest> _currentSteps = new ConcurrentDictionary<int, ExtentTest>();
        private static readonly object _lockObject = new object();

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
            lock (_lockObject)
            {
                if (_extent != null) return;
                
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
        }

        public static void CreateFeature(string featureName)
        {
            try
            {
                if (_extent == null)
                {
                    InitializeReport();
                }

                var feature = _extent.CreateTest<Feature>(featureName);
                _features[featureName] = feature;
                Console.WriteLine($"[REPORT] Created feature: {featureName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPORT] Error creating feature: {ex.Message}");
            }
        }

        public static void CreateScenario(string scenarioName, string featureName)
        {
            try
            {
                if (!_features.TryGetValue(featureName, out var feature))
                {
                    CreateFeature(featureName);
                    feature = _features[featureName];
                }

                var scenario = feature.CreateNode<Scenario>(scenarioName);
                int threadId = Thread.CurrentThread.ManagedThreadId;
                _scenarios[threadId] = scenario;
                Console.WriteLine($"[REPORT] Created scenario: {scenarioName} on thread {threadId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPORT] Error creating scenario: {ex.Message}");
            }
        }

        // Updated method to properly create and track steps
        public static void LogStep(StepType stepType, string stepName, Status status, string details = "")
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (!_scenarios.TryGetValue(threadId, out var scenario))
            {
                Console.WriteLine($"[REPORT] Warning: Scenario is null for step: {stepName} on thread {threadId}");
                return;
            }

            try
            {
                // Create the step node based on type
                ExtentTest step = stepType switch
                {
                    StepType.Given => scenario.CreateNode<Given>(stepName),
                    StepType.When => scenario.CreateNode<When>(stepName),
                    StepType.Then => scenario.CreateNode<Then>(stepName),
                    StepType.And => scenario.CreateNode<And>(stepName),
                    _ => scenario.CreateNode<Given>(stepName)
                };

                // Store reference for screenshots/additional logging
                _currentSteps[threadId] = step;

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

                Console.WriteLine($"[REPORT] Logged step: {stepType} - {stepName} - {status} on thread {threadId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPORT] Error logging step: {ex.Message}");
            }
        }

        // Alternative method for creating steps without immediate status (if needed)
        public static void CreateStep(StepType stepType, string stepName)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            
            if (!_scenarios.TryGetValue(threadId, out var scenario))
            {
                Console.WriteLine($"[REPORT] Warning: Scenario is null for step creation: {stepName} on thread {threadId}");
                return;
            }

            try
            {
                ExtentTest step = stepType switch
                {
                    StepType.Given => scenario.CreateNode<Given>(stepName),
                    StepType.When => scenario.CreateNode<When>(stepName),
                    StepType.Then => scenario.CreateNode<Then>(stepName),
                    StepType.And => scenario.CreateNode<And>(stepName),
                    _ => scenario.CreateNode<Given>(stepName)
                };
                
                _currentSteps[threadId] = step;

                Console.WriteLine($"[REPORT] Created step node: {stepType} - {stepName} on thread {threadId}");
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
                int threadId = Thread.CurrentThread.ManagedThreadId;
                
                if (_currentSteps.TryGetValue(threadId, out var currentStep))
                {
                    // Add screenshot to current step
                    currentStep.AddScreenCaptureFromPath(screenshotPath);
                    Console.WriteLine($"[REPORT] Added screenshot to current step: {screenshotPath} on thread {threadId}");
                }
                else if (_scenarios.TryGetValue(threadId, out var scenario))
                {
                    // Fallback: add to scenario
                    scenario.AddScreenCaptureFromPath(screenshotPath);
                    Console.WriteLine($"[REPORT] Added screenshot to scenario: {screenshotPath} on thread {threadId}");
                }
                else
                {
                    Console.WriteLine($"[REPORT] Warning: No active step or scenario for screenshot: {screenshotPath} on thread {threadId}");
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
                int threadId = Thread.CurrentThread.ManagedThreadId;
                
                if (_currentSteps.TryGetValue(threadId, out var currentStep))
                {
                    currentStep.Info(info);
                    Console.WriteLine($"[REPORT] Logged info to step on thread {threadId}");
                }
                else if (_scenarios.TryGetValue(threadId, out var scenario))
                {
                    scenario.Info(info);
                    Console.WriteLine($"[REPORT] Logged info to scenario on thread {threadId}");
                }
                else
                {
                    Console.WriteLine($"[REPORT] Warning: No active step or scenario for logging info on thread {threadId}");
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
                int threadId = Thread.CurrentThread.ManagedThreadId;
                
                if (_currentSteps.TryGetValue(threadId, out var currentStep))
                {
                    currentStep.Fail($"<strong>Error:</strong> {errorMessage}");
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        currentStep.Fail($"<strong>Stack Trace:</strong><br/><pre>{stackTrace}</pre>");
                    }
                    Console.WriteLine($"[REPORT] Logged failure to step on thread {threadId}");
                }
                else if (_scenarios.TryGetValue(threadId, out var scenario))
                {
                    scenario.Fail($"<strong>Error:</strong> {errorMessage}");
                    Console.WriteLine($"[REPORT] Logged failure to scenario on thread {threadId}");
                }
                else
                {
                    Console.WriteLine($"[REPORT] Warning: No active step or scenario for logging failure on thread {threadId}");
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
            int threadId = Thread.CurrentThread.ManagedThreadId;
            _currentSteps.TryRemove(threadId, out _);
            Console.WriteLine($"[REPORT] Reset current references for thread {threadId}");
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