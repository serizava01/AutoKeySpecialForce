using Kuey_Teow;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Windows.Forms;

public class ItemCodeExtractor
{
    private readonly RichTextBoxLogger _messageLog;
    private readonly Mu_KropMain _mainForm;  

    public ItemCodeExtractor(RichTextBoxLogger messageLog, Mu_KropMain mainForm)
    {
        _messageLog = messageLog;
        _mainForm = mainForm;  
    }

    public void ExtractItemCodes()
    {
        IWebDriver _driver = null;

        try
        {
            // สร้าง WebDriver
            _driver = WebDriverFactory.CreateChromeDriver(!_mainForm.cb_show.Checked); 
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
            _messageLog.ClearRTB();
            _messageLog.Line();
            _messageLog.AppendToRtbNoTime("ระบบดึงคีย์อัตโนมัติเริ่มทำงาน!");
            _messageLog.Line();

            _driver.Navigate().GoToUrl("http://keyword.gg.in.th/");
            _messageLog.AppendToRtb("โหลดข้อมูลหน้าเว็ป");

            // เปิดหน้าไอเทมโค๊ดรายวัน
            IWebElement openCode = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[5]/div/div[1]/div[12]/div[1]/div[3]/a")));
            _messageLog.AppendToRtb("กำลังโหลดไอเทมโค๊ดรายวัน");
            openCode.Click();

            // ปิดแท็บใหม่แล้วกลับไปยังแท็บเดิม
            SwitchToOriginalTab(_driver);

            _messageLog.Line();
            _driver.Navigate().GoToUrl("http://keyword.gg.in.th/");
            _messageLog.AppendToRtb("โหลดหน้าคีย์ไอเทม");
            _messageLog.Line();

            // ดึงคีย์ไอเทมแต่ละรายการ
            ExtractKey(_driver, wait, "/html/body/div[5]/div/div[1]/div[12]/div[1]/div[3]/div[1]", "คีย์รายวัน", _mainForm.txt_keyDay);
            ExtractKey(_driver, wait, "/html/body/div[5]/div/div[1]/div[11]/div[1]/div[3]/div[1]", "คีย์ 0 นาที", _mainForm.txt_0);
            ExtractKey(_driver, wait, "/html/body/div[5]/div/div[1]/div[10]/div[1]/div[3]/div[1]", "คีย์ 60 นาที", _mainForm.txt_60);
            ExtractKey(_driver, wait, "/html/body/div[5]/div/div[1]/div[9]/div[1]/div[3]/div[1]", "คีย์ 120 นาที", _mainForm.txt_120);
            ExtractKey(_driver, wait, "/html/body/div[5]/div/div[1]/div[8]/div[1]/div[3]/div[1]", "คีย์ 180 นาที", _mainForm.txt_180);
            ExtractKey(_driver, wait, "/html/body/div[5]/div/div[1]/div[7]/div[1]/div[3]/div[1]", "คีย์ 240 นาที", _mainForm.txt_240);
            ExtractKey(_driver, wait, "/html/body/div[5]/div/div[1]/div[6]/div[1]/div[3]/div[1]", "คีย์ 300 นาที", _mainForm.txt_300);
            ExtractKey(_driver, wait, "/html/body/div[5]/div/div[1]/div[5]/div[1]/div[3]/div[1]", "คีย์ 360 นาที", _mainForm.txt_360);
            ExtractKey(_driver, wait, "/html/body/div[5]/div/div[1]/div[4]/div[1]/div[3]/div[1]", "คีย์ 420 นาที", _mainForm.txt_420);
            ExtractKey(_driver, wait, "/html/body/div[5]/div/div[1]/div[3]/div[1]/div[3]/div[1]", "คีย์ 480 นาที", _mainForm.txt_480);
            ExtractKey(_driver, wait, "/html/body/div[5]/div/div[1]/div[2]/div[1]/div[3]/div[1]", "คีย์ 540 นาที", _mainForm.txt_540);
            ExtractKey(_driver, wait, "/html/body/div[5]/div/div[1]/div[1]/div[1]/div[3]/div[1]", "คีย์ 600 นาที", _mainForm.txt_600);
            _messageLog.Line();
            
        }
        catch (Exception ex)
        {
            _messageLog.AppendToRtb($"เกิดข้อผิดพลาด: {ex.Message}");
        }
        finally
        {
            HandleAlerts(_driver);
            Finalize(_driver);
        }
    }

    private void ExtractKey(IWebDriver driver, 
        WebDriverWait wait, 
        string xPath, 
        string keyLabel,
        ReaLTaiizor.Controls.ForeverTextBox textBox)
    {
        try
        {
            IWebElement keyElement = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xPath)));
            string keyText = keyElement.Text;
            _messageLog.AppendToRtb($"{keyLabel} {keyText}");

            if (textBox.InvokeRequired)
            {
                textBox.Invoke((MethodInvoker)delegate
                {
                    textBox.Text = keyText;
                });
            }
            else
            {
                textBox.Text = keyText;
            }
        }
        catch (Exception ex)
        {
            _messageLog.AppendToRtb($"เกิดข้อผิดพลาดขณะดึง {keyLabel}: {ex.Message}");
        }
    }

    private void SwitchToOriginalTab(IWebDriver driver)
    {
        var tabs = driver.WindowHandles;
        if (tabs.Count > 1)
        {
            driver.SwitchTo().Window(tabs[1]);
            driver.Close();
            driver.SwitchTo().Window(tabs[0]);
        }
    }

    private void HandleAlerts(IWebDriver driver)
    {
        try
        {
            IAlert alert = driver.SwitchTo().Alert();
            _messageLog.AppendToRtb(alert.Text);
        }
        catch (NoAlertPresentException)
        {
            // ไม่มี Alert
        }
    }

    private void Finalize(IWebDriver driver)
    {
        try
        {
            if (driver != null)
            {
                driver.Quit();
            }

            if (_mainForm.txt_Time.InvokeRequired)
            {
                _mainForm.txt_Time.Invoke((MethodInvoker)delegate
                {
                    _mainForm.txt_Time.Text = DateTime.Now.ToString("dd-MM-yyyy");
                });
            }
            else
            {
                _mainForm.txt_Time.Text = DateTime.Now.ToString("dd-MM-yyyy");
            }
        }
        catch (Exception ex)
        {
            _messageLog.AppendToRtb($"เกิดข้อผิดพลาดขณะปิด WebDriver: {ex.Message}");
        }
        finally
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}