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
        public void RegisterDependencies() 
        {
            // Register PlaywrightDriver as a scenario-scoped dependency 
            // This ensures each scenario gets its own browser instance 
            _objectContainer.RegisterFactoryAs<PlaywrightDriver>(container => new PlaywrightDriver());
        } 
    } 
}