using Microsoft.Playwright;
using OrangeHRM.Tests.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeHRM.Tests.Pages
{
    public class LoginPage : BasePage
    {
        // Selectors
        private string UsernameInput => "input[name='username']";
        private string PasswordInput => "input[name='password']";
        private string LoginButton => "button[type='submit']";
        private string ErrorMessage => ".oxd-alert-content-text";
        private string DashboardHeader => ".oxd-topbar-header-title";

        public LoginPage(IPage page) : base(page)
        {
        }

        public async Task NavigateToLoginPageAsync()
        {
            await NavigateToAsync(AppConfig.BaseUrl);
            await WaitForSelectorAsync(UsernameInput);
        }

        public async Task EnterUsernameAsync(string username)
        {
            await FillAsync(UsernameInput, username);
        }

        public async Task EnterPasswordAsync(string password)
        {
            await FillAsync(PasswordInput, password);
        }

        public async Task ClickLoginButtonAsync()
        {
            await ClickAsync(LoginButton);
        }

        public async Task<bool> IsErrorMessageDisplayedAsync()
        {
            return await IsVisibleAsync(ErrorMessage);
        }

        public async Task<string> GetErrorMessageAsync()
        {
            return await GetTextAsync(ErrorMessage);
        }

        public async Task<bool> IsLoggedInAsync()
        {
            return await IsVisibleAsync(DashboardHeader);
        }

        public async Task LoginAsync(string username, string password)
        {
            await EnterUsernameAsync(username);
            await EnterPasswordAsync(password);
            await ClickLoginButtonAsync();
        }

        public async Task LoginWithDefaultCredentialsAsync()
        {
            await LoginAsync(AppConfig.Username, AppConfig.Password);
        }
    }
}