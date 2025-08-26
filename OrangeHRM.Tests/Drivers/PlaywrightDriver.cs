using Microsoft.Playwright;
using OrangeHRM.Tests.Config;

namespace OrangeHRM.Tests.Drivers
{
    public class PlaywrightDriver : IDisposable
    {
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _page;
        private bool _disposed = false;

        public IPage Page => _page ?? throw new InvalidOperationException("Page not initialized. Call InitializeAsync first.");

        public async Task InitializeAsync()
        {
            if (_playwright != null) return; // Already initialized

            try
            {
                // Create Playwright instance
                _playwright = await Microsoft.Playwright.Playwright.CreateAsync();

                // Launch browser
                var launchOptions = new BrowserTypeLaunchOptions
                {
                    Headless = AppConfig.Headless,
                    Timeout = AppConfig.DefaultTimeout,
                    SlowMo = AppConfig.Headless ? 0 : 100 // Add slight delay for debugging in non-headless mode
                };

                _browser = AppConfig.Browser.ToLower() switch
                {
                    "chromium" => await _playwright.Chromium.LaunchAsync(launchOptions),
                    "firefox" => await _playwright.Firefox.LaunchAsync(launchOptions),
                    "webkit" => await _playwright.Webkit.LaunchAsync(launchOptions),
                    _ => await _playwright.Chromium.LaunchAsync(launchOptions)
                };

                // Create browser context
                _context = await _browser.NewContextAsync(new BrowserNewContextOptions
                {
                    ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
                    AcceptDownloads = true,
                    HasTouch = false,
                    Locale = "en-US",
                    TimezoneId = "UTC",
                    IgnoreHTTPSErrors = true // Ignore HTTPS errors for demo sites
                });

                // Create page
                _page = await _context.NewPageAsync();
                _page.SetDefaultTimeout(AppConfig.DefaultTimeout);
                _page.SetDefaultNavigationTimeout(AppConfig.DefaultTimeout);

                Console.WriteLine("Playwright driver initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize Playwright driver: {ex.Message}");
                await DisposeAsync();
                throw;
            }
        }

        public async Task<IPage> GetNewPageAsync()
        {
            if (_context == null)
            {
                throw new InvalidOperationException("Browser context not initialized");
            }
            return await _context.NewPageAsync();
        }

        public async Task DisposeAsync()
        {
            if (_disposed) return;

            try
            {
                if (_page != null)
                {
                    await _page.CloseAsync();
                    _page = null;
                }

                if (_context != null)
                {
                    await _context.CloseAsync();
                    _context = null;
                }

                if (_browser != null)
                {
                    await _browser.CloseAsync();
                    _browser = null;
                }

                _playwright?.Dispose();
                _playwright = null;

                _disposed = true;
                Console.WriteLine("Playwright driver disposed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Playwright driver disposal: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                // Synchronous disposal for IDisposable
                Task.Run(async () => await DisposeAsync()).GetAwaiter().GetResult();
            }
        }
    }
}