using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeHRM.Tests.Pages
{
    public abstract class BasePage
    {
        protected readonly IPage Page;

        protected BasePage(IPage page)
        {
            Page = page;
        }

        public async Task NavigateToAsync(string url)
        {
            await Page.GotoAsync(url);
        }

        protected async Task ClickAsync(string selector)
        {
            await Page.ClickAsync(selector);
        }

        protected async Task FillAsync(string selector, string value)
        {
            await Page.FillAsync(selector, value);
        }

        protected async Task<string> GetTextAsync(string selector)
        {
            return await Page.TextContentAsync(selector);
        }

        protected async Task<bool> IsVisibleAsync(string selector)
        {
            var element = await Page.QuerySelectorAsync(selector);
            return element != null && await element.IsVisibleAsync();
        }

        protected async Task WaitForSelectorAsync(string selector)
        {
            await Page.WaitForSelectorAsync(selector);
        }
    }
}