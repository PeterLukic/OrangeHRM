namespace OrangeHRM.Tests.Config
{
    public static class AppConfig
    {
        public static string BaseUrl => "https://opensource-demo.orangehrmlive.com/web/index.php/auth/login";
        public static string Username => "Admin";
        public static string Password => "admin123";
        public static int DefaultTimeout => 30000; // Increased to 30 seconds
        public static string Browser => "chromium"; // Options: chromium, firefox, webkit
        public static bool Headless => false;
    }
}