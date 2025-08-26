using FluentAssertions;
using Microsoft.Playwright;
using NUnit.Framework;
using OrangeHRM.Tests.Config;
using OrangeHRM.Tests.Drivers;
using OrangeHRM.Tests.Pages;
using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

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
            _scenarioContext = scenarioContext;
            _playwrightDriver = playwrightDriver;
            _loginPage = new LoginPage(_playwrightDriver.Page);
        }

        [Given(@"I am on the OrangeHRM login page")]
        public async Task GivenIAmOnTheOrangeHRMLoginPage()
        {
            await _loginPage.NavigateToLoginPageAsync();
        }

        [When(@"I enter valid username and password")]
        public async Task WhenIEnterValidUsernameAndPassword()
        {
            await _loginPage.EnterUsernameAsync(AppConfig.Username);
            await _loginPage.EnterPasswordAsync(AppConfig.Password);
        }

        [When(@"I enter username ""(.*)"" and password ""(.*)""")]
        public async Task WhenIEnterUsernameAndPassword(string username, string password)
        {
            await _loginPage.EnterUsernameAsync(username);
            await _loginPage.EnterPasswordAsync(password);
        }

        [When(@"I click the login button")]
        public async Task WhenIClickTheLoginButton()
        {
            await _loginPage.ClickLoginButtonAsync();
        }

        [Then(@"I should be logged in successfully")]
        public async Task ThenIShouldBeLoggedInSuccessfully()
        {
            var isLoggedIn = await _loginPage.IsLoggedInAsync();
            isLoggedIn.Should().BeTrue("User should be logged in successfully");
        }

        [Then(@"I should see an error message")]
        public async Task ThenIShouldSeeAnErrorMessage()
        {
            var isErrorDisplayed = await _loginPage.IsErrorMessageDisplayedAsync();
            isErrorDisplayed.Should().BeTrue("Error message should be displayed for invalid credentials");
        }

        [Then(@"I should see an error message for empty fields")]
        public async Task ThenIShouldSeeAnErrorMessageForEmptyFields()
        {
            var isErrorDisplayed = await _loginPage.IsErrorMessageDisplayedAsync();
            isErrorDisplayed.Should().BeTrue("Error message should be displayed for empty fields");
        }

        [Then(@"the login should be ""(.*)""")]
        public async Task ThenTheLoginShouldBe(string result)
        {
            if (result.Equals("successful", StringComparison.OrdinalIgnoreCase))
            {
                var isLoggedIn = await _loginPage.IsLoggedInAsync();
                isLoggedIn.Should().BeTrue("User should be logged in successfully");
            }
            else
            {
                var isErrorDisplayed = await _loginPage.IsErrorMessageDisplayedAsync();
                isErrorDisplayed.Should().BeTrue("Error message should be displayed for invalid credentials");
            }
        }
    }
}