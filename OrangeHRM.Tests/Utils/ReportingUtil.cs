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
            htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Dark;
            htmlReporter.Config.DocumentTitle = "OrangeHRM Test Report";
            htmlReporter.Config.ReportName = "OrangeHRM Automation Test Report";

            _extent = new ExtentReports();
            _extent.AttachReporter(htmlReporter);

            _extent.AddSystemInfo("Environment", "Test");
            _extent.AddSystemInfo("Browser", "Chromium");
            _extent.AddSystemInfo("OS", Environment.OSVersion.ToString());
        }

        public static void CreateFeature(string featureName)
        {
            _feature = GetExtentReports().CreateTest<Feature>(featureName);
        }

        public static void CreateScenario(string scenarioName)
        {
            _scenario = _feature.CreateNode<Scenario>(scenarioName);
        }

        public static void CreateStep(StepType stepType, string stepName)
        {
            switch (stepType)
            {
                case StepType.Given:
                    _scenario.CreateNode<Given>(stepName);
                    break;
                case StepType.When:
                    _scenario.CreateNode<When>(stepName);
                    break;
                case StepType.Then:
                    _scenario.CreateNode<Then>(stepName);
                    break;
                case StepType.And:
                    _scenario.CreateNode<And>(stepName);
                    break;
                default:
                    _scenario.CreateNode<Given>(stepName);
                    break;
            }
        }

        public static void LogStep(StepType stepType, string stepName, Status status, string details = "")
        {
            ExtentTest step = null;

            switch (stepType)
            {
                case StepType.Given:
                    step = _scenario.CreateNode<Given>(stepName);
                    break;
                case StepType.When:
                    step = _scenario.CreateNode<When>(stepName);
                    break;
                case StepType.Then:
                    step = _scenario.CreateNode<Then>(stepName);
                    break;
                case StepType.And:
                    step = _scenario.CreateNode<And>(stepName);
                    break;
                default:
                    step = _scenario.CreateNode<Given>(stepName);
                    break;
            }

            step.Log(status, details);
        }

        public static void LogScreenshot(string screenshotPath)
        {
            _scenario.AddScreenCaptureFromPath(screenshotPath);
        }

        public static void FlushReport()
        {
            _extent?.Flush();
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