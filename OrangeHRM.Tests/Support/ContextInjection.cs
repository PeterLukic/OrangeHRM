using BoDi;
using OrangeHRM.Tests.Drivers;
using TechTalk.SpecFlow;

namespace OrangeHRM.Tests.Support
{
    [Binding]
    public class ContextInjection
    {
        private readonly IObjectContainer _objectContainer;

        public ContextInjection(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario(Order = 0)]
        public async Task RegisterDependencies()
        {
            try
            {
                Console.WriteLine("Initializing Playwright driver for scenario...");

                // Create and initialize PlaywrightDriver
                var driver = new PlaywrightDriver();
                await driver.InitializeAsync();

                // Register the initialized driver
                _objectContainer.RegisterInstanceAs(driver);

                Console.WriteLine("Playwright driver registered successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to register dependencies: {ex.Message}");
                throw;
            }
        }

        [AfterScenario(Order = 1000)]
        public async Task CleanupDependencies()
        {
            try
            {
                Console.WriteLine("Cleaning up Playwright driver...");

                if (_objectContainer.IsRegistered<PlaywrightDriver>())
                {
                    var driver = _objectContainer.Resolve<PlaywrightDriver>();
                    await driver.DisposeAsync();
                }

                Console.WriteLine("Playwright driver cleanup completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during cleanup: {ex.Message}");
            }
        }
    }
}