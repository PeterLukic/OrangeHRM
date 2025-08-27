using FluentAssertions;
using Microsoft.Playwright;
using NUnit.Framework;
using OrangeHRM.Tests.Config;
using OrangeHRM.Tests.Drivers;
using OrangeHRM.Tests.Pages;
using OrangeHRM.Tests.Utils;
using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using AventStack.ExtentReports;

namespace OrangeHRM.Tests.StepDefinitions
{
    [Binding]
    public class LoginSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly PlaywrightDriver _playwrightDriver;
        private LoginPage _loginPage;

        public LoginSteps(ScenarioContext scenarioContext, PlaywrightDriver playwrightDriver)
        {
            _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
            _playwrightDriver = playwrightDriver ?? throw new ArgumentNullException(nameof(playwrightDriver));
            _loginPage = new LoginPage(_playwrightDriver.Page);
        }

        [Given(@"I am on the OrangeHRM login page")]
        public async Task GivenIAmOnTheOrangeHRMLoginPage()
        {
            var stepName = "I am on the OrangeHRM login page";
            ReportingUtil.CreateStep(StepType.Given, stepName);

            try
            {
                Console.WriteLine("Navigating to OrangeHRM login page...");
                await _loginPage.NavigateToLoginPageAsync();
                Console.WriteLine("Successfully navigated to login page");

                // Log success to report
                ReportingUtil.LogStep(StepType.Given, stepName, Status.Pass, "Successfully navigated to OrangeHRM login page");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to navigate to login page: {ex.Message}");
                ReportingUtil.LogStep(StepType.Given, stepName, Status.Fail, ex.Message);
                throw;
            }
        }

        [When(@"I enter valid username and password")]
        public async Task WhenIEnterValidUsernameAndPassword()
        {
            var stepName = "I enter valid username and password";
            ReportingUtil.CreateStep(StepType.When, stepName);

            try
            {
                Console.WriteLine("Entering valid credentials...");
                await _loginPage.EnterUsernameAsync(AppConfig.Username);
                await _loginPage.EnterPasswordAsync(AppConfig.Password);
                Console.WriteLine("Credentials entered successfully");

                // Log success to report
                ReportingUtil.LogStep(StepType.When, stepName, Status.Pass, $"Entered username: {AppConfig.Username} and password");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to enter credentials: {ex.Message}");
                ReportingUtil.LogStep(StepType.When, stepName, Status.Fail, ex.Message);
                throw;
            }
        }

        [When(@"I enter username ""(.*)"" and password ""(.*)""")]
        public async Task WhenIEnterUsernameAndPassword(string username, string password)
        {
            var stepName = $"I enter username \"{username}\" and password \"{password}\"";
            ReportingUtil.CreateStep(StepType.When, stepName);

            try
            {
                Console.WriteLine($"Entering username: {username}");
                await _loginPage.EnterUsernameAsync(username);
                await _loginPage.EnterPasswordAsync(password);
                Console.WriteLine("Custom credentials entered successfully");

                // Log success to report (don't show actual password)
                ReportingUtil.LogStep(StepType.When, stepName, Status.Pass, $"Entered username: {username} and password: [HIDDEN]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to enter custom credentials: {ex.Message}");
                ReportingUtil.LogStep(StepType.When, stepName, Status.Fail, ex.Message);
                throw;
            }
        }

        [When(@"I click the login button")]
        public async Task WhenIClickTheLoginButton()
        {
            var stepName = "I click the login button";
            ReportingUtil.CreateStep(StepType.When, stepName);

            try
            {
                Console.WriteLine("Clicking login button...");
                await _loginPage.ClickLoginButtonAsync();

                // Wait a bit for the page to respond
                await Task.Delay(2000);
                Console.WriteLine("Login button clicked successfully");

                // Log success to report
                ReportingUtil.LogStep(StepType.When, stepName, Status.Pass, "Login button clicked and waiting for response");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to click login button: {ex.Message}");
                ReportingUtil.LogStep(StepType.When, stepName, Status.Fail, ex.Message);
                throw;
            }
        }

        [Then(@"I should be logged in successfully")]
        public async Task ThenIShouldBeLoggedInSuccessfully()
        {
            var stepName = "I should be logged in successfully";
            ReportingUtil.CreateStep(StepType.Then, stepName);

            try
            {
                Console.WriteLine("Checking if user is logged in...");

                // Wait for either dashboard or error message
                await Task.Delay(3000);

                var isLoggedIn = await _loginPage.IsLoggedInAsync();
                Console.WriteLine($"Login status: {isLoggedIn}");

                isLoggedIn.Should().BeTrue("User should be logged in successfully");

                // Take screenshot of successful login
                var screenshotPath = await ScreenshotUtil.CaptureScreenshotAsync(
                    _playwrightDriver.Page,
                    $"SuccessfulLogin_{DateTime.Now:yyyyMMdd_HHmmss}");

                if (!string.IsNullOrEmpty(screenshotPath))
                {
                    ReportingUtil.LogScreenshot(screenshotPath);
                }

                // Log success to report
                ReportingUtil.LogStep(StepType.Then, stepName, Status.Pass, "User successfully logged in and redirected to dashboard");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to verify login success: {ex.Message}");

                // Capture failure screenshot
                var screenshotPath = await ScreenshotUtil.CaptureScreenshotAsync(
                    _playwrightDriver.Page,
                    $"LoginFailure_{DateTime.Now:yyyyMMdd_HHmmss}");

                if (!string.IsNullOrEmpty(screenshotPath))
                {
                    ReportingUtil.LogScreenshot(screenshotPath);
                }

                ReportingUtil.LogStep(StepType.Then, stepName, Status.Fail, ex.Message);
                throw;
            }
        }

        [Then(@"I should see an error message")]
        public async Task ThenIShouldSeeAnErrorMessage()
        {
            var stepName = "I should see an error message";
            ReportingUtil.CreateStep(StepType.Then, stepName);

            try
            {
                Console.WriteLine("Checking for error message...");

                // Wait for error message to appear
                await Task.Delay(2000);

                var isErrorDisplayed = await _loginPage.IsErrorMessageDisplayedAsync();
                Console.WriteLine($"Error message displayed: {isErrorDisplayed}");

                string errorMessage = "";
                if (isErrorDisplayed)
                {
                    errorMessage = await _loginPage.GetErrorMessageAsync();
                    Console.WriteLine($"Error message: {errorMessage}");
                }

                // Take screenshot of error state
                var screenshotPath = await ScreenshotUtil.CaptureScreenshotAsync(
                    _playwrightDriver.Page,
                    $"ErrorMessage_{DateTime.Now:yyyyMMdd_HHmmss}");

                if (!string.IsNullOrEmpty(screenshotPath))
                {
                    ReportingUtil.LogScreenshot(screenshotPath);
                }

                isErrorDisplayed.Should().BeTrue("Error message should be displayed for invalid credentials");

                // Log success to report
                ReportingUtil.LogStep(StepType.Then, stepName, Status.Pass, $"Error message displayed successfully: {errorMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to check error message: {ex.Message}");
                ReportingUtil.LogStep(StepType.Then, stepName, Status.Fail, ex.Message);
                throw;
            }
        }

        [Then(@"I should see an error message for empty fields")]
        public async Task ThenIShouldSeeAnErrorMessageForEmptyFields()
        {
            var stepName = "I should see an error message for empty fields";
            ReportingUtil.CreateStep(StepType.Then, stepName);

            try
            {
                Console.WriteLine("Checking for empty fields error message...");
                await Task.Delay(2000);

                var isErrorDisplayed = await _loginPage.IsErrorMessageDisplayedAsync();
                Console.WriteLine($"Empty fields error displayed: {isErrorDisplayed}");

                isErrorDisplayed.Should().BeTrue("Error message should be displayed for empty fields");

                // Log success to report
                ReportingUtil.LogStep(StepType.Then, stepName, Status.Pass, "Empty fields validation message displayed correctly");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to check empty fields error: {ex.Message}");
                ReportingUtil.LogStep(StepType.Then, stepName, Status.Fail, ex.Message);
                throw;
            }
        }

        [Then(@"the login should be ""(.*)""")]
        public async Task ThenTheLoginShouldBe(string result)
        {
            var stepName = $"the login should be \"{result}\"";
            ReportingUtil.CreateStep(StepType.Then, stepName);

            try
            {
                Console.WriteLine($"Verifying login result should be: {result}");
                await Task.Delay(3000);

                if (result.Equals("successful", StringComparison.OrdinalIgnoreCase))
                {
                    var isLoggedIn = await _loginPage.IsLoggedInAsync();
                    Console.WriteLine($"Login successful verification: {isLoggedIn}");
                    isLoggedIn.Should().BeTrue("User should be logged in successfully");

                    // Log success to report
                    ReportingUtil.LogStep(StepType.Then, stepName, Status.Pass, "Login was successful as expected");
                }
                else
                {
                    var isErrorDisplayed = await _loginPage.IsErrorMessageDisplayedAsync();
                    Console.WriteLine($"Login failure verification: {isErrorDisplayed}");
                    isErrorDisplayed.Should().BeTrue("Error message should be displayed for invalid credentials");

                    // Log success to report
                    ReportingUtil.LogStep(StepType.Then, stepName, Status.Pass, "Login failed as expected with proper error message");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to verify login result: {ex.Message}");
                ReportingUtil.LogStep(StepType.Then, stepName, Status.Fail, ex.Message);
                throw;
            }
        }
    }
}