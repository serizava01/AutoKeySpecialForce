using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public class InjectionHandler
{
    private readonly RichTextBoxLogger _messageLog;
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;
    private readonly LoginHandler _loginHandler;

    public InjectionHandler(RichTextBoxLogger messageLog, IWebDriver driver, WebDriverWait wait, LoginHandler loginHandler)
    {
        _messageLog = messageLog;
        _driver = driver;
        _wait = wait;
        _loginHandler = loginHandler;
    }
    public static string[] GetAvailableTasks()
    {
        return new string[] { "รายวัน", "เลือกทั้งหมด", "60 - 240", "300 - 480", "540 - 600", "ไอเทมคีย์กิจกรรม" };
    }
    public void InjectionCode(string task, Dictionary<string, string> keyValues,
        bool IDmode,
        bool showBrowser,
        bool isItemSelected,
        string selectedCharacter,
        string gmKey)
    {
        string[] validTasks = { "รายวัน", "เลือกทั้งหมด", "60 - 240", "300 - 480", "540 - 600", "ไอเทมคีย์กิจกรรม" };
        if (!validTasks.Contains(task))
        {
            _messageLog.AppendToRtbNoTime($"Invalid task: {task}");
            return;
        }

        _messageLog.ClearRTB();
        try
        {
            _messageLog.Line();
            _messageLog.AppendToRtbNoTime("ระบบส่งคีย์อัตโนมัติ");
            _messageLog.Line();

            string loginUrl = "https://auth.gg.in.th/authenticate_v3/Auth_Full/Login.aspx?appid=1&scope=&state=1&sourceid=1&redirecturi=http%3a%2f%2fmember.sf.in.th%2fLandingPlatform.aspx&fblogin=true";
            _driver.Navigate().GoToUrl(loginUrl);

            if (IDmode)
            {
                _loginHandler.HandleDfLogin(keyValues["username"], keyValues["password"]);
            }
            else
            {
                _loginHandler.HandleGGIDLogin(keyValues["username"], keyValues["password"]);
            }

            _messageLog.Line();
            _driver.Navigate().GoToUrl("http://member.sf.in.th/Keyword/");

            if (isItemSelected)
            {
                ScFunction(task, keyValues);
            }
            else
            {
                if (string.IsNullOrEmpty(selectedCharacter) && IsCharacterSelectionRequired(gmKey))
                {
                    selectedCharacter = ShowCharacterSelectionPopup();
                    if (string.IsNullOrEmpty(selectedCharacter))
                    {
                        throw new ArgumentException("กรุณาเลือกตัวละครก่อน");
                    }
                }
                HandleKeySubmission(gmKey, "กำลังส่งคีย์ไอเทม", selectedCharacter);
            }

        }
        catch (NoSuchElementException ex)
        {
            LogError($"ไม่พบองค์ประกอบ: {ex.Message}");
        }
        catch (TimeoutException ex)
        {
            LogError($"เวลาหมด: {ex.Message}");
        }
        catch (Exception ex)
        {
            LogError($"เกิดข้อผิดพลาด: {CleanErrorMessage(ex.Message)}");
        }
        finally
        {
            HandleDriverCleanup();
        }
    }

    public bool IsCharacterSelectionRequired(string key)
    {
        
        return key.Contains("GM") || key.Contains("Event");
    }
    public void ScFunction(string task, Dictionary<string, string> keyValues)
    {
        switch (task)
        {
            case "รายวัน":
                HandleKeySubmission(keyValues["keyDay"], "กำลังส่งคีย์ไอเทมรายวัน");
                HandleKeySubmission(keyValues["0"], "กำลังส่งคีย์(0)", isExpKey: true);
                break;
            case "60 - 240":
                HandleKeySubmission(keyValues["60"], "กำลังส่งคีย์(60)", isExpKey: true);
                HandleKeySubmission(keyValues["120"], "กำลังส่งคีย์(120)", isExpKey: true);
                HandleKeySubmission(keyValues["180"], "กำลังส่งคีย์(180)", isExpKey: true);
                HandleKeySubmission(keyValues["240"], "กำลังส่งคีย์(240)", isExpKey: true);
                break;
            case "300 - 480":
                HandleKeySubmission(keyValues["300"], "กำลังส่งคีย์(300)", isExpKey: true);
                HandleKeySubmission(keyValues["360"], "กำลังส่งคีย์(360)", isExpKey: true);
                HandleKeySubmission(keyValues["420"], "กำลังส่งคีย์(420)", isExpKey: true);
                HandleKeySubmission(keyValues["480"], "กำลังส่งคีย์(480)", isExpKey: true);
                break;
            case "540 - 600":
                HandleKeySubmission(keyValues["540"], "กำลังส่งคีย์(540)", isExpKey: true);
                HandleKeySubmission(keyValues["600"], "กำลังส่งคีย์(600)", isExpKey: true);
                break;
            case "เลือกทั้งหมด":
                HandleKeySubmission(keyValues["keyDay"], "กำลังส่งคีย์ไอเทมรายวัน");
                HandleKeySubmission(keyValues["0"], "กำลังส่งคีย์(0)", isExpKey: true);
                HandleKeySubmission(keyValues["60"], "กำลังส่งคีย์(60)", isExpKey: true);
                HandleKeySubmission(keyValues["120"], "กำลังส่งคีย์(120)", isExpKey: true);
                HandleKeySubmission(keyValues["180"], "กำลังส่งคีย์(180)", isExpKey: true);
                HandleKeySubmission(keyValues["240"], "กำลังส่งคีย์(240)", isExpKey: true);
                HandleKeySubmission(keyValues["300"], "กำลังส่งคีย์(300)", isExpKey: true);
                HandleKeySubmission(keyValues["360"], "กำลังส่งคีย์(360)", isExpKey: true);
                HandleKeySubmission(keyValues["420"], "กำลังส่งคีย์(420)", isExpKey: true);
                HandleKeySubmission(keyValues["480"], "กำลังส่งคีย์(480)", isExpKey: true);
                HandleKeySubmission(keyValues["540"], "กำลังส่งคีย์(540)", isExpKey: true);
                HandleKeySubmission(keyValues["600"], "กำลังส่งคีย์(600)", isExpKey: true);
                break;
            case "ไอเทมคีย์กิจกรรม":
                HandleActivityKey(keyValues["GMkey"]);
                break;
            default:
                throw new ArgumentException($"Invalid task: {task}");
        }
    }
    private void HandleActivityKey(string key)
    {
        try
        {
            // ส่งคีย์แรกเริ่ม
            IWebElement keyText = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[3]/div/div[1]/div[1]/div[2]/input")));
            keyText.Clear();
            keyText.SendKeys(key);
            _messageLog.AppendToRtbNoTime("กำลังส่งคีย์กิจกรรม");

            IWebElement sendKeyCodes = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[3]/div/div[1]/div[2]")));
            sendKeyCodes.Click();

            HandleAlertIfPresent();

            // ตรวจสอบประเภทคีย์
            if (CheckForElement("/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[1]/table/tbody/tr[3]/td[1]/input[1]"))
            {
                HandleExpKey();
            }
            else if (CheckForElement("/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[1]/table/tbody/tr[2]/td[1]/input[1]"))
            {
                HandleCharacterkey();
            }
            else
            {
                HandleNormalItemKey();
            }
        }
        catch (Exception ex)
        {
            LogError($"เกิดข้อผิดพลาดในการส่งคีย์กิจกรรม: {CleanErrorMessage(ex.Message)}");
        }
    }
    private bool CheckForElement(string xpath)
    {
        try
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
            return true;
        }
        catch
        {
            return false;
        }

    }
    private void HandleKeySubmission(string key, string logMessage, string selectedCharacter = null, bool isExpKey = false)
    {
        try
        {
            IWebElement keyText = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[3]/div/div[1]/div[1]/div[2]/input")));
            keyText.Clear();
            keyText.SendKeys(key);
            _messageLog.AppendToRtbNoTime(logMessage);

            IWebElement sendKeyCodes = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[3]/div/div[1]/div[2]")));
            sendKeyCodes.Click();

            HandleAlertIfPresent();

            if (isExpKey)
            {
                HandleExpKey();
            }
            else if (!string.IsNullOrEmpty(selectedCharacter))
            {
                HandleCharacterSelection(selectedCharacter);
            }
            else
            {
                HandleNormalItemKey();
            }
        }
        catch (Exception ex)
        {
            LogError($"เกิดข้อผิดพลาดในการส่งคีย์: {CleanErrorMessage(ex.Message)}");
        }
    }
    private void HandleExpKey()
    {
        _messageLog.AppendToRtbNoTime("พบคีย์ประเภท EXP");
        IWebElement checkExp2 = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[1]/table/tbody/tr[3]/td[1]/input[1]")));
        checkExp2.Click();
        _messageLog.AppendToRtbNoTime("เลือก Exp Rank 2");
        ConfirmGetItem();
    }
    private void HandleCharacterkey()
    {
        _messageLog.AppendToRtbNoTime("พบคีย์ประเภทตัวละคร");
        string selectedCharacter = ShowCharacterSelectionPopup();
        if (string.IsNullOrEmpty(selectedCharacter))
        {
            throw new ArgumentException("กรุณาเลือกตัวละครก่อน");
        }

        HandleCharacterSelection(selectedCharacter);
    }
    private void HandleCharacterSelection(string selectedCharacter)
    {
        if (characterXPaths.ContainsKey(selectedCharacter))
        {
            IWebElement characterCheckbox = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(characterXPaths[selectedCharacter])));
            characterCheckbox.Click();
            _messageLog.AppendToRtbNoTime($"เลือกตัวละคร: {selectedCharacter}");
            ConfirmGetItem();
        }
        else
        {
            throw new ArgumentException($"ไม่พบตัวละคร: {selectedCharacter}");
        }
    }
    private void HandleNormalItemKey()
    {
        _messageLog.AppendToRtbNoTime("พบคีย์ไอเทมทั่วไป");
        ConfirmGetItem();
    }
    private void ConfirmGetItem()
    {
        try
        {
            IWebElement getItem = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[2]/a[1]")));
            getItem.Click();
            _messageLog.AppendToRtb("ยืนยันการรับไอเทม");
            _messageLog.Line();

            IWebElement itemList = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div[1]/div/h1/div/span")));
            _messageLog.AppendToRtbNoTime($"{itemList.Text}");

            IWebElement endButtonList = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div[1]/div/div/a")));
            endButtonList.Click();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[3]/div/div[1]/div[1]/div[2]/input")));
            _messageLog.Line();
        }
        catch (Exception ex)
        {
            LogError($"เกิดข้อผิดพลาดในการยืนยันรับไอเทม: {CleanErrorMessage(ex.Message)}");
        }
    }
    private bool HandleAlertIfPresent()
    {
        try
        {
            IAlert alert = _driver.SwitchTo().Alert();
            string alertText = alert.Text;
            alert.Accept();
            _messageLog.AppendToRtbNoTime($"แจ้งเตือน: {alertText}");
            return true;
        }
        catch (NoAlertPresentException)
        {
            return false;
        }
        catch (Exception ex)
        {
            LogError($"เกิดข้อผิดพลาดในการจัดการ Alert: {CleanErrorMessage(ex.Message)}");
            return false;
        }
    }
    private void HandleDriverCleanup()
    {
        try
        {
            if (_driver != null)
            {
                try
                {
                    IAlert alert = _driver.SwitchTo().Alert();
                    string alertText = alert.Text;
                    alert.Accept();
                    LogError($"เกิดข้อผิดพลาด: {CleanErrorMessage(alertText)}");
                }
                catch (NoAlertPresentException) { }

                _driver.Quit();
                _messageLog.Line();
            }
        }
        catch (ObjectDisposedException ex)
        {
            LogError($"ข้อผิดพลาด: {ex.Message}");
        }
    }
    private void LogError(string message)
    {
        _messageLog.Line();
        _messageLog.AppendToRtb(message);
        _messageLog.Line();
    }
    private string CleanErrorMessage(string message)
    {
        return Regex.Replace(message, @"\(Session info:.*?\)", "").Trim();
    }
    public string ShowCharacterSelectionPopup()
    {
        
        var popupForm = new Form
        {
            Width = 300,
            Height = 200,
            Text = "เลือกตัวละคร",
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterScreen
        };

       
        var comboBox = new ComboBox
        {
            Dock = DockStyle.Top,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        comboBox.Items.AddRange(new[] { "Delta", "EID", "Forcrecon", "GIGN", "GSG9", "KSF", "PSU", "ROKMC", "SAS", "SASR", "SIAM", "SpetSnaz", "SRG", "SSD" });

     
        var okButton = new Button
        {
            Text = "ตกลง",
            Dock = DockStyle.Bottom
        };

        string selectedCharacter = null;
        okButton.Click += (sender, args) =>
        {
            selectedCharacter = comboBox.SelectedItem?.ToString();
            popupForm.Close();
        };

     
        popupForm.Controls.Add(comboBox);
        popupForm.Controls.Add(okButton);

         
        popupForm.ShowDialog();

        return selectedCharacter;
    }
    private Dictionary<string, string> characterXPaths = new Dictionary<string, string>()
    {
        { "Delta", "/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[1]/table/tbody/tr[2]/td[1]/input[1]" },
        { "EID", "/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[1]/table/tbody/tr[3]/td[1]/input[1]" },
        { "Forcrecon", "/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[1]/table/tbody/tr[4]/td[1]/input[1]" },
        { "GIGN", "/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[1]/table/tbody/tr[5]/td[1]/input[1]" },
        { "GSG9", "/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[1]/table/tbody/tr[6]/td[1]/input[1]" },
        { "KSF", "/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[1]/table/tbody/tr[7]/td[1]/input[1]" },
        { "PSU", "/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[1]/table/tbody/tr[8]/td[1]/input[1]" },
        { "ROKMC", "/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[1]/table/tbody/tr[9]/td[1]/input[1]" },
        { "SAS", "/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[1]/table/tbody/tr[10]/td[1]/input[1]" },
        { "SASR", "/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[1]/table/tbody/tr[11]/td[1]/input[1]" },
        { "SIAM", "/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[1]/table/tbody/tr[12]/td[1]/input[1]" },
        { "SpetSnaz", "/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[1]/table/tbody/tr[13]/td[1]/input[1]" },
        { "SRG", "/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[1]/table/tbody/tr[14]/td[1]/input[1]" },
        { "SSD", "/html/body/form/div[3]/div[3]/div[2]/div/div[3]/div/div/div[1]/table/tbody/tr[15]/td[1]/input[1]" }
    };

}