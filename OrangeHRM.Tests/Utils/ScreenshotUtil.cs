using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OrangeHRM.Tests.Utils
{
    public static class ScreenshotUtil
    {
        public static async Task<string> CaptureScreenshotAsync(IPage page, string screenshotName)
        {
            var screenshotPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults", "Screenshots");
            Directory.CreateDirectory(screenshotPath);

            var fileName = $"{screenshotName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            var fullPath = Path.Combine(screenshotPath, fileName);

            await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = fullPath,
                FullPage = true
            });

            return fullPath;
        }
    }
}