using Microsoft.Playwright;
using OrangeHRM.Tests.Config;

namespace OrangeHRM.Tests.Pages
{
    public class LoginPage : BasePage
    {
        // Updated selectors for OrangeHRM demo site
        private string UsernameInput => "input[name='username']";
        private string PasswordInput => "input[name='password']";
        private string LoginButton => "button[type='submit']";
        private string ErrorMessage => ".oxd-alert-content-text";
        private string DashboardHeader => ".oxd-topbar-header-breadcrumb";
        private string LoadingSpinner => ".oxd-loading-spinner";

        public LoginPage(IPage page) : base(page)
        {
        }

        public async Task NavigateToLoginPageAsync()
        {
            try
            {
                Console.WriteLine($"Navigating to: {AppConfig.BaseUrl}");
                await NavigateToAsync(AppConfig.BaseUrl);

                // Wait for the page to load and username input to be visible
                await WaitForSelectorAsync(UsernameInput);
                Console.WriteLine("Login page loaded successfully");

                // Take a small pause to ensure page is fully rendered
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failed: {ex.Message}");
                throw;
            }
        }

        public async Task EnterUsernameAsync(string username)
        {
            try
            {
                Console.WriteLine($"Entering username: {username}");
                await WaitForSelectorAsync(UsernameInput);
                await FillAsync(UsernameInput, username);
                Console.WriteLine("Username entered successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to enter username: {ex.Message}");
                throw;
            }
        }

        public async Task EnterPasswordAsync(string password)
        {
            try
            {
                Console.WriteLine("Entering password...");
                await WaitForSelectorAsync(PasswordInput);
                await FillAsync(PasswordInput, password);
                Console.WriteLine("Password entered successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to enter password: {ex.Message}");
                throw;
            }
        }

        public async Task ClickLoginButtonAsync()
        {
            try
            {
                Console.WriteLine("Clicking login button...");
                await WaitForSelectorAsync(LoginButton);
                await ClickAsync(LoginButton);
                Console.WriteLine("Login button clicked");

                // Wait for either success or failure response
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to click login button: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> IsErrorMessageDisplayedAsync()
        {
            try
            {
                // Wait a bit for error message to appear if it's going to
                await Task.Delay(1000);

                var isVisible = await IsVisibleAsync(ErrorMessage);
                Console.WriteLine($"Error message visible: {isVisible}");
                return isVisible;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking error message visibility: {ex.Message}");
                return false;
            }
        }

        public async Task<string> GetErrorMessageAsync()
        {
            try
            {
                if (await IsErrorMessageDisplayedAsync())
                {
                    var message = await GetTextAsync(ErrorMessage);
                    Console.WriteLine($"Error message text: {message}");
                    return message ?? "";
                }
                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get error message: {ex.Message}");
                return "";
            }
        }

        public async Task<bool> IsLoggedInAsync()
        {
            try
            {
                // Wait for page transition
                await Task.Delay(3000);

                // Check multiple indicators of successful login
                var isDashboardVisible = await IsVisibleAsync(DashboardHeader);
                var currentUrl = Page.Url;
                var isOnDashboard = currentUrl.Contains("dashboard") || currentUrl.Contains("index");

                Console.WriteLine($"Dashboard header visible: {isDashboardVisible}");
                Console.WriteLine($"Current URL: {currentUrl}");
                Console.WriteLine($"Is on dashboard page: {isOnDashboard}");

                return isDashboardVisible || isOnDashboard;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking login status: {ex.Message}");
                return false;
            }
        }

        public async Task LoginAsync(string username, string password)
        {
            try
            {
                Console.WriteLine($"Performing login with username: {username}");
                await EnterUsernameAsync(username);
                await EnterPasswordAsync(password);
                await ClickLoginButtonAsync();
                Console.WriteLine("Login attempt completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
                throw;
            }
        }

        public async Task LoginWithDefaultCredentialsAsync()
        {
            await LoginAsync(AppConfig.Username, AppConfig.Password);
        }
    }
}