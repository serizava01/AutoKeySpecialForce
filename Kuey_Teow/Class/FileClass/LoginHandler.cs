using Kuey_Teow;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Text.RegularExpressions;
using System.Threading;

public class LoginHandler
{
    private readonly RichTextBoxLogger _messageLog;
    private readonly IWebDriver _driver;

    public LoginHandler(RichTextBoxLogger messageLog, IWebDriver driver)
    {
        _messageLog = messageLog;
        _driver = driver;
    }
    public void LoginCheck(string username, string password, bool IDmode)
    {
        
        if (IDmode)
        {
            HandleDfLogin(username, password);
        }
        else
        {
            HandleGGIDLogin(username, password);
        }
    }
    public void HandleGGIDLogin(string username, string password)
    {
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        try
        {
            Thread.Sleep(1000);
            _messageLog.AppendToRtb("เริ่มการล็อคอิน GG ID...");
            ClickElement(wait, By.XPath("/html/body/form/div[4]/div[2]/div[2]/div/ul/li[1]/a"));

            _messageLog.AppendToRtb("โหลดข้อมูลเว็ปล็อคอินไอดีแบบ GG ID");
            SendKeysToElement(wait, By.XPath("/html/body/form/div[4]/div[2]/div[2]/div/div[2]/div[1]/ul/div/div[1]/div[1]/input"), username);
            _messageLog.AppendToRtb("ส่งข้อมูลไอดีสู่เว็ปไซต์");

            SendKeysToElement(wait, By.XPath("/html/body/form/div[4]/div[2]/div[2]/div/div[2]/div[1]/ul/div/div[1]/div[2]/input"), password);
            _messageLog.AppendToRtb("ส่งข้อมูลพาสเวิร์ดสู่เว็ปไซต์");

            ClickElement(wait, By.XPath("/html/body/form/div[4]/div[2]/div[2]/div/div[2]/div[1]/ul/div/div[2]"));
            _messageLog.AppendToRtbNoTime("กำลังโหลดอยู่รอสักครู่...");

            ClickElement(wait, By.XPath("/html/body/form/div[4]/div[2]/div[2]/div/div[2]/div[6]"));
            _messageLog.AppendToRtb("ล็อคอิน GG ID สำเร็จ!");
        }
        catch (Exception ex)
        {
            string cleanedMessage = Regex.Replace(ex.Message, @"\(Session info:.*?\)", "").Trim();
            _messageLog.AppendToRtb($"เกิดข้อผิดพลาดในการล็อคอิน GG ID: {cleanedMessage}");
            _driver.Quit();
        }
    }
    public void HandleDfLogin(string username, string password)
    {
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        try
        {
            Thread.Sleep(1000);
            SendKeysToElement(wait, By.XPath("/html/body/form/div[4]/div[2]/div[2]/div/div[2]/div[2]/ul/div/div[1]/div[1]/input[1]"), username);
            _messageLog.AppendToRtb("ส่งข้อมูลไอดีสู่เข้าสู่เว็ปไซต์");

            SendKeysToElement(wait, By.XPath("/html/body/form/div[4]/div[2]/div[2]/div/div[2]/div[2]/ul/div/div[1]/div[2]/input"), password);
            _messageLog.AppendToRtb("ส่งข้อมูลสู่พาสเวิร์ดเข้าสู่เว็ปไซต์");

            ClickElement(wait, By.XPath("/html/body/form/div[4]/div[2]/div[2]/div/div[2]/div[2]/ul/div/div[2]"));
            _messageLog.AppendToRtb("กำลังโหลดข้อมูลสักครู่....");

            ClickElement(wait, By.XPath("/html/body/form/div[4]/div[2]/div[2]/div/div[2]/div[6]"));
            _messageLog.AppendToRtb("ล็อคอินสำเร็จ");
        }
        catch (Exception ex)
        {
            _messageLog.AppendToRtb($"เกิดข้อผิดพลาดในการล็อคอิน Df: {ex.Message}");
            _driver.Quit();
        }
    }
    private void SendKeysToElement(WebDriverWait wait, By by, string text)
    {
        IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(by));
        element.SendKeys(text);
        Thread.Sleep(100);
    }
    private void ClickElement(WebDriverWait wait, By by)
    {
        IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(by));
        element.Click();
    }
}