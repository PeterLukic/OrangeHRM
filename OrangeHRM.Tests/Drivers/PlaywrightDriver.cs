using Microsoft.Playwright;
using OrangeHRM.Tests.Config;

namespace OrangeHRM.Tests.Drivers
{
    public class PlaywrightDriver : IDisposable
    {
        private readonly Task<IPlaywright> _playwrightTask;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _page;

        public IPage Page => _page ?? throw new InvalidOperationException("Page not initialized");

        public PlaywrightDriver()
        {
            _playwrightTask = Microsoft.Playwright.Playwright.CreateAsync();
            InitializeAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeAsync()
        {
            var playwright = await _playwrightTask;

            // Choose browser based on configuration
            _browser = AppConfig.Browser.ToLower() switch
            {
                "chromium" => await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = AppConfig.Headless }),
                "firefox" => await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = AppConfig.Headless }),
                "webkit" => await playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions { Headless = AppConfig.Headless }),
                _ => await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = AppConfig.Headless })
            };

            // Create a new browser context with viewport and user agent
            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
                AcceptDownloads = true,
                HasTouch = false,
                Locale = "en-US",
                TimezoneId = "UTC"
            });

            // Create a new page
            _page = await _context.NewPageAsync();

            // Set default timeout
            _page.SetDefaultTimeout(AppConfig.DefaultTimeout);
        }

        public async Task<IPage> GetNewPageAsync()
        {
            if (_context == null)
            {
                throw new InvalidOperationException("Browser context not initialized");
            }
            return await _context.NewPageAsync();
        }

        public void Dispose()
        {
            try
            {
                _page?.CloseAsync().GetAwaiter().GetResult();
                _context?.CloseAsync().GetAwaiter().GetResult();
                _browser?.CloseAsync().GetAwaiter().GetResult();
                _playwrightTask.Result?.Dispose();
            }
            catch (Exception)
            {
                // Suppress disposal exceptions to prevent issues during cleanup
            }
        }
    }
}