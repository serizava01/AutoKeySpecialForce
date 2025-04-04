using Kuey_Teow;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;


public class QuestHandler
{
    private readonly RichTextBoxLogger _messageLog;
    private readonly IWebDriver _driver;
    private readonly LoginHandler _loginHandler;
    private readonly WebDriverWait _wait;
    private readonly Mu_KropMain _mainForm;


    public QuestHandler(RichTextBoxLogger messageLog, LoginHandler loginHandler, IWebDriver driver, Mu_KropMain mainForm)
    {
        _messageLog = messageLog;
        _loginHandler = loginHandler;
        _driver = driver;
        _mainForm = mainForm; 
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
    }
    public void Quest_Start(bool useDfLogin, bool showBrowser, Dictionary<string, string> keyValues, bool IsEventQuestm, bool IsDaily_Quest, bool IsPrivilege)
    {
        if (_driver == null)
        {
            _messageLog.AppendToRtbNoTime("Driver is null. Cannot start quest.");
            return;
        }

        try
        {
            _messageLog.ClearRTB();
            _messageLog.Line();
            _messageLog.AppendToRtbNoTime("EventQuest เริ่มทำงาน");
            _messageLog.Line();

            _driver.Navigate().GoToUrl("https://auth.gg.in.th/authenticate_v3/Auth_Full/Login.aspx?appid=1&scope=&state=1&sourceid=1&redirecturi=http%3a%2f%2fmember.sf.in.th%2fLandingPlatform.aspx&fblogin=true");
            _messageLog.AppendToRtbNoTime("ระบบรับไอเทม อีเว้น อัตโนมัติ เริ่มทำงาน !");

            if (useDfLogin)
            {
                _loginHandler.HandleDfLogin(keyValues["username"], keyValues["password"]);
            }
            else
            {
                _loginHandler.HandleGGIDLogin(keyValues["username"], keyValues["password"]);
            }

            if (IsEventQuestm)
            {
                _driver.Navigate().GoToUrl("http://member.sf.in.th/PIMS/Event_PlayGame_Accumulate.aspx");
                _messageLog.AppendToRtbNoTime("โหมดรับเควสอีเว้น");
                HandleAllEvents(_driver);
            }

            if (IsDaily_Quest)
            {
                _driver.Url = "http://member.sf.in.th/PIMS/Event_Mission.aspx";
                _messageLog.AppendToRtbNoTime("โหมดรับเควสรายวัน");
                CheckQuest(_driver);
            }

            if (IsPrivilege)
            {
                _driver.Url = "http://member.sf.in.th/Privilege/GetItems.aspx";
                _messageLog.AppendToRtbNoTime("โหมดรับ Privilege");
                StatusButtonPvCheck(_driver);
            }
        }
        catch (Exception ex)
        {
            _messageLog.AppendToRtbNoTime($"เกิดข้อผิดพลาด: {ex.Message}");
        }
        finally
        {
            if (_driver != null)
            {
                _driver.Quit();
            }
        }
    }
    public void HandleAllEvents(IWebDriver driver)
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

        try
        {

            string eventXPath = "/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[4]/a";
            IReadOnlyCollection<IWebElement> eventElements = driver.FindElements(By.XPath(eventXPath));


            int eventCount = eventElements.Count;

            _messageLog.Line();
            _messageLog.AppendToRtbNoTime($"พบอีเว้นทั้งหมด {eventCount} รายการ");
            _messageLog.Line();

            for (int i = 1; i <= eventCount; i++)
            {
                try
                {

                    string ButtonEvent = $"/html/body/form/div[3]/div[3]/div/div/div[4]/div[{i}]/div[4]/a";
                    string txt_Quest = $"/html/body/form/div[3]/div[3]/div/div/div[4]/div[{i}]/div[1]";


                    wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(ButtonEvent)));
                    IWebElement eventElement = driver.FindElement(By.XPath(ButtonEvent));
                    IWebElement Quest_Text = driver.FindElement(By.XPath(txt_Quest));
                    _messageLog.Line();
                    _messageLog.AppendToRtbNoTime($"กิจกรรมที่ {i}");

                    string NameQuest = Quest_Text.Text;
                    _messageLog.AppendToRtbNoTime($"ชื่อเควส: {NameQuest}");
                    eventElement.Click();

                    wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[5]")));
                    string statusButtonXPath = "/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[5]";
                    IWebElement statusButton = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(statusButtonXPath)));

                    StatusButtonitemAndExp(driver, statusButton, NameQuest);


                    wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[3]/ul/li[13]/a")));
                    IWebElement GOEventList = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[3]/ul/li[13]/a")));
                    GOEventList.Click();
                    _messageLog.AppendToRtbNoTime("กลับไปหน้าเช็ค Event");
                    _messageLog.Line();
                }
                catch (NoSuchElementException)
                {

                    _messageLog.AppendToRtbNoTime($"ไม่พบกิจกรรมที่ {i}");
                }
                catch (StaleElementReferenceException)
                {

                    _messageLog.AppendToRtbNoTime("เกิด Stale Element Reference Exception: ค้นหา element ใหม่");
                    i--;
                }
                catch (Exception ex)
                {
                    _messageLog.AppendToRtbNoTime($"เกิดข้อผิดพลาดในการดำเนินการกิจกรรมที่ {i}: {ex.Message}");
                    driver.Navigate().GoToUrl("http://member.sf.in.th/PIMS/Event_PlayGame_Accumulate.aspx");
                }
            }
        }
        catch (Exception ex)
        {
            _messageLog.AppendToRtbNoTime($"เกิดข้อผิดพลาดในการดึงกิจกรรม: {ex.Message}");
        }
    }
    public void CheckQuest(IWebDriver driver)
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

        string QuestXpath = "/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[4]/a";

        IReadOnlyCollection<IWebElement> QuestElement = driver.FindElements(By.XPath(QuestXpath));
        int QuestCount = QuestElement.Count();
        _messageLog.AppendToRtbNoTime($"เจอภารกิจ: {QuestCount} ภารกิจ");
        _messageLog.AppendToRtbNoTime("T = Team\nS = Single");
        for (int i = 1; i <= QuestCount; i++)
        {
            try
            {
                string Questbutton = $"/html/body/form/div[3]/div[3]/div/div/div[4]/div[{i}]/div[4]/a";
                string QuestnameTitile = $"/html/body/form/div[3]/div[3]/div/div/div[4]/div[{i}]/div[1]";

                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(Questbutton)));
                IWebElement QuestButtonElement = driver.FindElement(By.XPath(Questbutton));
                IWebElement QuestTitle = driver.FindElement(By.XPath(QuestnameTitile));

                string QuestNameTitles = QuestTitle.Text;
                _messageLog.Line();
                _messageLog.AppendToRtbNoTime($"เควสที่ {i}");
                _messageLog.AppendToRtbNoTime($"ชื่อเควส: {QuestNameTitles}");

                QuestButtonElement.Click();

                string Button = $"/html/body/form/div[3]/div[3]/div/div/div[4]/div[2]/div[3]";

                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(Button)));
                IWebElement buttonStatus = driver.FindElement(By.XPath(Button));

                StatusButtonQuestCheck(driver, buttonStatus, QuestNameTitles);
                _messageLog.Line();


            }
            catch (NoSuchElementException)
            {

                _messageLog.AppendToRtbNoTime($"ไม่พบกิจกรรมที่ {i}");
            }
            catch (StaleElementReferenceException)
            {

                _messageLog.AppendToRtbNoTime("เกิด Stale Element Reference Exception: ค้นหา element ใหม่");
                i--;
            }
            catch (Exception ex)
            {
                _messageLog.AppendToRtbNoTime($"เกิดข้อผิดพลาดในการดำเนินการกิจกรรมที่ {i}: {ex.Message}");
                driver.Navigate().GoToUrl("http://member.sf.in.th/PIMS/Event_Mission.aspx");
            }

        }
    }
    public void StatusButtonPvCheck(IWebDriver driver)
    {
        string buttonPickupItem = "/html/body/form/div[3]/div[3]/div[2]/div/div[5]/div/div/div[2]/a[1]";
        string listitem = "/html/body/form/div[3]/div[3]/div[1]/div/h1/div/span";
        ///html/body/form/div[3]/div[3]/div[2]/div/div[5]/div/div/div[1]/blockquote/div/span
        string buttonback = "/html/body/form/div[3]/div[3]/div[2]/div/div[4]/div/div/div[2]/a";
        try
        {
            var buttonpickup = Element(driver, buttonPickupItem);
            buttonpickup.Click();
            _messageLog.AppendToRtbNoTime($"พบไอเทมที่สามารถรับได้");

            var Lisitem = Element(driver, listitem);
            string lis = Lisitem.Text;
            _messageLog.AppendToRtbNoTime($"ไอเทมที่ได้รับ\n{lis}");

        }
        catch (NoSuchElementException)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(buttonback)));
                var workbuttonback = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(buttonback)));
                workbuttonback.Click();
                _messageLog.AppendToRtbNoTime("ไม่พบไอเทมที่รับได้");
            }
            catch (ElementNotSelectableException ex)
            {
                _messageLog.AppendToRtbNoTime($"ไม่พบปุ่มไดๆบนเว็ป: {ex.Message}");

            }


        }

    }
    private bool HasChoice(IWebDriver driver)
    {
        try
        {
            IReadOnlyCollection<IWebElement> choices = driver.FindElements(By.XPath("/html/body/form/div[3]/div[3]/div[2]/div/div[5]/div/div/div[1]/table/tbody/tr/td[1]/input[1]"));
            return choices.Count > 0;
        }
        catch
        {
            return false;
        }
    }
    public void StatusButtonitemAndExp(IWebDriver driver, IWebElement statusButton, string eventName)
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        try
        {
            if (statusButton.Text.Contains("ทำกิจกรรมเพิ่ม"))
            {

                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[1]/p[1]")));
                IWebElement txt_TimeToplay = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[1]/p[1]"));
                IWebElement Cout_TimeToplay = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[1]/p[2]"));
                string Txt_TimeToplays = txt_TimeToplay.Text;
                string cout_TimetoPlays = Cout_TimeToplay.Text;

                IWebElement txt_YouPlayTime = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[2]/p[1]"));
                IWebElement cout_YouPlayTime = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[2]/p[2]"));
                string txt_YouplayTimes = txt_YouPlayTime.Text;
                string cout_YouplayTimes = cout_YouPlayTime.Text;

                IWebElement txt_TimeEvent = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[3]/p[1]"));
                IWebElement cout_TimeEvent = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[3]/p[2]"));
                string txt_TimeEvents = txt_TimeEvent.Text;
                string cout_TimeEvents = cout_TimeEvent.Text;

                IWebElement txt_statusEvent = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[4]/p[1]"));
                IWebElement StatusEvent = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[4]/p[2]/font/b"));
                string txt_statusEvents = txt_statusEvent.Text;
                string StatusEvents = StatusEvent.Text;


                _messageLog.AppendToRtbNoTime($"{Txt_TimeToplays}: {cout_TimetoPlays}");
                _messageLog.AppendToRtbNoTime($"{txt_YouplayTimes}: {cout_YouplayTimes}");
                _messageLog.AppendToRtbNoTime($"{txt_TimeEvents}: {cout_TimeEvents}");
                _messageLog.AppendToRtbNoTime($"{txt_statusEvents}: {StatusEvents}");

                _messageLog.AppendToRtbNoTime($"ผลลัพธ์: {statusButton.Text}");
                driver.Navigate().GoToUrl("http://member.sf.in.th/PIMS/Event_PlayGame_Accumulate.aspx");

            }
            else if (statusButton.Text.Contains("รับไอเทมไปแล้ว"))
            {

                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[1]/p[1]")));
                IWebElement txt_TimeToplay = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[1]/p[1]"));
                IWebElement Cout_TimeToplay = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[1]/p[2]"));
                string Txt_TimeToplays = txt_TimeToplay.Text;
                string cout_TimetoPlays = Cout_TimeToplay.Text;

                IWebElement txt_YouPlayTime = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[2]/p[1]"));
                IWebElement cout_YouPlayTime = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[2]/p[2]"));
                string txt_YouplayTimes = txt_YouPlayTime.Text;
                string cout_YouplayTimes = cout_YouPlayTime.Text;

                IWebElement txt_TimeEvent = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[3]/p[1]"));
                IWebElement cout_TimeEvent = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[3]/p[2]"));
                string txt_TimeEvents = txt_TimeEvent.Text;
                string cout_TimeEvents = cout_TimeEvent.Text;

                IWebElement txt_statusEvent = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[4]/p[1]"));
                IWebElement StatusEvent = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[4]/p[2]/font/b"));
                string txt_statusEvents = txt_statusEvent.Text;
                string StatusEvents = StatusEvent.Text;


                _messageLog.AppendToRtbNoTime($"{Txt_TimeToplays}: {cout_TimetoPlays}");
                _messageLog.AppendToRtbNoTime($"{txt_YouplayTimes}: {cout_YouplayTimes}");
                _messageLog.AppendToRtbNoTime($"{txt_TimeEvents}: {cout_TimeEvents}");
                _messageLog.AppendToRtbNoTime($"{txt_statusEvents}: {StatusEvents}");
                _messageLog.AppendToRtbNoTime($"ผลลัพธ์: {statusButton.Text}");
                driver.Navigate().GoToUrl("http://member.sf.in.th/PIMS/Event_PlayGame_Accumulate.aspx");

            }
            else if (statusButton.Text.Contains("รับไอเทม"))
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[1]/p[1]")));
                IWebElement txt_TimeToplay = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[1]/p[1]"));
                IWebElement Cout_TimeToplay = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[1]/p[2]"));
                string Txt_TimeToplays = txt_TimeToplay.Text;
                string cout_TimetoPlays = Cout_TimeToplay.Text;

                IWebElement txt_YouPlayTime = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[2]/p[1]"));
                IWebElement cout_YouPlayTime = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[2]/p[2]"));
                string txt_YouplayTimes = txt_YouPlayTime.Text;
                string cout_YouplayTimes = cout_YouPlayTime.Text;

                IWebElement txt_TimeEvent = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[3]/p[1]"));
                IWebElement cout_TimeEvent = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[3]/p[2]"));
                string txt_TimeEvents = txt_TimeEvent.Text;
                string cout_TimeEvents = cout_TimeEvent.Text;

                IWebElement txt_statusEvent = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[4]/p[1]"));
                IWebElement StatusEvent = driver.FindElement(By.XPath("/html/body/form/div[3]/div[3]/div/div/div[4]/div/div[4]/p[2]/font/b"));
                string txt_statusEvents = txt_statusEvent.Text;
                string StatusEvents = StatusEvent.Text;

                _messageLog.AppendToRtbNoTime($"{Txt_TimeToplays}: {cout_TimetoPlays}");
                _messageLog.AppendToRtbNoTime($"{txt_YouplayTimes}: {cout_YouplayTimes}");
                _messageLog.AppendToRtbNoTime($"{txt_TimeEvents}: {cout_TimeEvents}");
                _messageLog.AppendToRtbNoTime($"{txt_statusEvents}: {StatusEvents}");
                _messageLog.AppendToRtbNoTime($"ผลลัพธ์: {statusButton.Text}");
                statusButton.Click();

                if (HasChoice(driver))
                {
                    HandleTimeTaskExp(driver, eventName, statusButton);
                }
                else
                {
                    HandleTimeTaskItem(driver, eventName, statusButton);
                }

                driver.Navigate().GoToUrl("http://member.sf.in.th/PIMS/Event_PlayGame_Accumulate.aspx");

            }
        }
        catch (Exception ex)
        {
            _messageLog.AppendToRtb($"เกิดข้อผิดพลาดในการจัดการสถานะ: {ex.Message}");
            driver.Quit();


        }
    }
    private void HandleTimeTaskItem(IWebDriver driver, string eventName, IWebElement statusButton)
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        try
        {
            _messageLog.AppendToRtbNoTime($"เควส: {eventName}");
            _messageLog.AppendToRtbNoTime("พบเป็นเควสรับไอเทม");
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div[2]/div/div[5]/div/div/div[2]/a[1]")));
            IWebElement confirmButton = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div[2]/div/div[5]/div/div/div[2]/a[1]")));
            confirmButton.Click();
            _messageLog.AppendToRtbNoTime($"ยืนยันการรับไอเทมสำเร็จ!");
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div[1]/div/h1/div")));
            IWebElement listitem = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div[1]/div/h1/div")));
            string GiveitemFor = listitem.Text;
            _messageLog.AppendToRtbNoTime($"\n(รายชื่อไอเทมที่ได้้รับ)\n{GiveitemFor}\n");
            _messageLog.AppendToRtbNoTime("กำลับไปเช็คหน้าเควส");


        }
        catch (Exception ex)
        {
            _messageLog.AppendToRtbNoTime($"เกิดข้อผิดพลาดใน HandleTimeTaskItem: {ex.Message}");
        }
    }
    private void HandleTimeTaskExp(IWebDriver driver, string eventName, IWebElement statusButton)
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        try
        {
            _messageLog.AppendToRtbNoTime($"เควส: {eventName}");
            _messageLog.AppendToRtbNoTime("ตรวจพบเป็นเควสEXP");
            IWebElement expChoice = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div[2]/div/div[5]/div/div/div[1]/table/tbody/tr[2]/td[1]/input[1]")));
            expChoice.Click();
            _messageLog.AppendToRtbNoTime($"กำลังเลือกช้อยส์ Exp");

            // ยืนยันการรับไอเทม
            IWebElement confirmButton = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div[2]/div/div[5]/div/div/div[2]/a[1]")));
            confirmButton.Click();
            _messageLog.AppendToRtbNoTime($"ยืนยันการรับไอเทมสำเร็จ!");


            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div[1]/div/h1/div")));
            IWebElement listitem = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/form/div[3]/div[3]/div[1]/div/h1/div")));
            string GiveitemFor = listitem.Text;
            _messageLog.AppendToRtbNoTime($"\n(รายชื่อไอเทมที่ได้้รับ)\n{GiveitemFor}\n");
            _messageLog.AppendToRtbNoTime("กำลับไปเช็คหน้าเควส");


        }
        catch (Exception ex)
        {
            _messageLog.AppendToRtbNoTime($"เกิดข้อผิดพลาดใน HandleTimeTaskExp: {ex.Message}");
        }
    }
    public void StatusButtonQuestCheck(IWebDriver driver, IWebElement statusButton, string QuestName)
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        if (statusButton.Text.Contains("ยังไม่ถึงเกณฑ์"))
        {
            string txt_ModeTitile = $"/html/body/form/div[3]/div[3]/div/div/div[4]/div[1]/div[2]/div[1]/p";
            string txt_ModeText = $"/html/body/form/div[3]/div[3]/div/div/div[4]/div[1]/div[2]/div[3]/p";

            string txt_cout = $"/html/body/form/div[3]/div[3]/div/div/div[4]/div[1]/div[5]/div[1]/p";
            string txt_coutQuest = "/html/body/form/div[3]/div[3]/div/div/div[4]/div[1]/div[5]/div[3]/p";

            string txt_YouQuestCout = "/html/body/form/div[3]/div[3]/div/div/div[4]/div[2]/div[1]/p[2]";

            string BlackToQuestList = "/html/body/form/div[3]/div[3]/div/div/div[3]/ul/li[15]/a";

            IWebElement txt_ModeTitles = driver.FindElement(By.XPath(txt_ModeTitile));
            IWebElement txt_ModeTexts = driver.FindElement(By.XPath(txt_ModeText));

            IWebElement txt_couts = driver.FindElement(By.XPath(txt_cout));
            IWebElement txt_coutQuests = driver.FindElement(By.XPath(txt_coutQuest));

            IWebElement YouCoutQuest = driver.FindElement(By.XPath(txt_YouQuestCout));

            IWebElement BlackToListQuests = driver.FindElement(By.XPath(BlackToQuestList));


            string txt_ModeTItlese = txt_ModeTitles.Text;
            string txt_modeTextse = txt_ModeTexts.Text;

            string txt_Coutse = txt_couts.Text;
            string txt_CoutQuestse = txt_coutQuests.Text;

            string txt_YouCout = YouCoutQuest.Text;

            _messageLog.AppendToRtbNoTime($"{txt_ModeTItlese}: {txt_modeTextse}");
            _messageLog.AppendToRtbNoTime($"{txt_Coutse}: {txt_CoutQuestse}");
            _messageLog.AppendToRtbNoTime($"{txt_YouCout}");
            _messageLog.AppendToRtbNoTime($"ผลลัพธ์: {statusButton.Text}");

            driver.Navigate().GoToUrl("http://member.sf.in.th/PIMS/Event_Mission.aspx");

        }
        else if (statusButton.Text.Contains("รับไอเทมไปแล้ว"))
        {
            string txt_ModeTitile = $"/html/body/form/div[3]/div[3]/div/div/div[4]/div[1]/div[2]/div[1]/p";
            string txt_ModeText = $"/html/body/form/div[3]/div[3]/div/div/div[4]/div[1]/div[2]/div[3]/p";

            string txt_cout = $"/html/body/form/div[3]/div[3]/div/div/div[4]/div[1]/div[5]/div[1]/p";
            string txt_coutQuest = "/html/body/form/div[3]/div[3]/div/div/div[4]/div[1]/div[5]/div[3]/p";

            string txt_YouQuestCout = "/html/body/form/div[3]/div[3]/div/div/div[4]/div[2]/div[1]/p[2]";



            IWebElement txt_ModeTitles = driver.FindElement(By.XPath(txt_ModeTitile));
            IWebElement txt_ModeTexts = driver.FindElement(By.XPath(txt_ModeText));

            IWebElement txt_couts = driver.FindElement(By.XPath(txt_cout));
            IWebElement txt_coutQuests = driver.FindElement(By.XPath(txt_coutQuest));

            IWebElement YouCoutQuest = driver.FindElement(By.XPath(txt_YouQuestCout));




            string txt_ModeTItlese = txt_ModeTitles.Text;
            string txt_modeTextse = txt_ModeTexts.Text;

            string txt_Coutse = txt_couts.Text;
            string txt_CoutQuestse = txt_coutQuests.Text;

            string txt_YouCout = YouCoutQuest.Text;

            _messageLog.AppendToRtbNoTime($"{txt_ModeTItlese}: {txt_modeTextse}");
            _messageLog.AppendToRtbNoTime($"{txt_Coutse}: {txt_CoutQuestse}");
            _messageLog.AppendToRtbNoTime($"{txt_YouCout}");
            _messageLog.AppendToRtbNoTime($"ผลลัพธ์: {statusButton.Text}");

            driver.Navigate().GoToUrl("http://member.sf.in.th/PIMS/Event_Mission.aspx");


        }
        else if (statusButton.Text.Contains("รับไอเทม"))
        {
            string txt_ModeTitile = $"/html/body/form/div[3]/div[3]/div/div/div[4]/div[1]/div[2]/div[1]/p";
            string txt_ModeText = $"/html/body/form/div[3]/div[3]/div/div/div[4]/div[1]/div[2]/div[3]/p";

            string txt_cout = $"/html/body/form/div[3]/div[3]/div/div/div[4]/div[1]/div[5]/div[1]/p";
            string txt_coutQuest = "/html/body/form/div[3]/div[3]/div/div/div[4]/div[1]/div[5]/div[3]/p";

            string txt_YouQuestCout = "/html/body/form/div[3]/div[3]/div/div/div[4]/div[2]/div[1]/p[2]";



            IWebElement txt_ModeTitles = driver.FindElement(By.XPath(txt_ModeTitile));
            IWebElement txt_ModeTexts = driver.FindElement(By.XPath(txt_ModeText));

            IWebElement txt_couts = driver.FindElement(By.XPath(txt_cout));
            IWebElement txt_coutQuests = driver.FindElement(By.XPath(txt_coutQuest));

            IWebElement YouCoutQuest = driver.FindElement(By.XPath(txt_YouQuestCout));




            string txt_ModeTItlese = txt_ModeTitles.Text;
            string txt_modeTextse = txt_ModeTexts.Text;

            string txt_Coutse = txt_couts.Text;
            string txt_CoutQuestse = txt_coutQuests.Text;

            string txt_YouCout = YouCoutQuest.Text;

            _messageLog.AppendToRtbNoTime($"{txt_ModeTItlese}: {txt_modeTextse}");
            _messageLog.AppendToRtbNoTime($"{txt_Coutse}: {txt_CoutQuestse}");
            _messageLog.AppendToRtbNoTime($"{txt_YouCout}");
            _messageLog.AppendToRtbNoTime($"ผลลัพธ์: {statusButton.Text}");
            statusButton.Click();
            if (HasChoice(driver))
            {
                HandleTimeTaskExp(driver, QuestName, statusButton);
            }
            else
            {
                HandleTimeTaskItem(driver, QuestName, statusButton);
            }

            driver.Navigate().GoToUrl("http://member.sf.in.th/PIMS/Event_Mission.aspx");
        }
    }
    public IWebElement Element(IWebDriver driver, string Xpath)
    {
        return driver.FindElement(By.XPath(Xpath));

    }


}
