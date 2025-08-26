using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeHRM.Tests.Config
{
    public static class AppConfig
    {
        public static string BaseUrl => "https://opensource-demo.orangehrmlive.com/web/index.php/auth/login";
        public static string Username => "Admin";
        public static string Password => "admin123";
        public static int DefaultTimeout => 3000; // 3 seconds
        public static string Browser => "chromium"; // Options: chromium, firefox, webkit
        public static bool Headless => false;
    }
}