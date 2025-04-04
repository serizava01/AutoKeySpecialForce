using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

public static class WebDriverFactory
{
    public static IWebDriver CreateChromeDriver(bool isHeadless)
    {
        ChromeOptions chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--width=1920");
        chromeOptions.AddArgument("--height=1080");
        chromeOptions.AddArgument("--no-proxy-server");
        chromeOptions.AddArgument("mute-audio");

        var driverService = ChromeDriverService.CreateDefaultService();
        driverService.HideCommandPromptWindow = true;

        if (isHeadless)
        {
            chromeOptions.AddArgument("--headless");
        }

        chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
        //chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
        //chromeOptions.AddArgument("--disable-gpu");
        //chromeOptions.AddArgument("--no-sandbox");

        return new ChromeDriver(driverService, chromeOptions);
    }
}