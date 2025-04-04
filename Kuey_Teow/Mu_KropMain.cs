using FileDownloader;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ReaLTaiizor.Controls;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using static ReaLTaiizor.Controls.ExtendedPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Kuey_Teow
{
    public partial class Mu_KropMain : Form
    {
        private EX_File ex;
        private string CustomDll = "Data.dll";
        private string CustomFolder = "data";


        private bool IsMessages = false;
        private bool IsLogMessages = true;
         
        private ScreenCapture screenCapture;
        private OCRProcessor ocrProcessor;

        public RichTextBoxLogger _Messagelog;
        public IWebDriver _driver;
 


        private string downloadUrl = "https://serizava01.github.io/jubilant-system/tessdata.zip";
        private string updatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Downlad.zip");
        private string extractPath = Application.StartupPath;
  

       
        
        public Mu_KropMain()
        {
            InitializeComponent();
            _Messagelog = new RichTextBoxLogger(rtb_01);
             
            Loads();
            ex = new EX_File("Data.Resources.", rtb_01, CustomDll, CustomFolder);
            cbx_Program.Items.AddRange(new string[] { "AutoSupply_Box" });
            cbx_ItemCode.Items.AddRange(new string[] { "รายวัน", "เลือกทั้งหมด", "60 - 240", "300 - 480", "540 - 600", "ไอเทมคีย์กิจกรรม" });
            cbx_character.Items.AddRange(new string[] { "delta", "EID", "Forcrecon", "GIGN",
                "GSG9", "KSF", "PSU", "ROKMC", "SAS", "SASR", "SIAM","SpetSnaz","SRG","SSD" });
       
            screenCapture = new ScreenCapture();
            ocrProcessor = new OCRProcessor();
            FileDownloadManager downloader = new FileDownloadManager(progressBar1);
            
        }
          
        #region Config
        public void XjCode()
        {
            string selectedTask = cbx_ItemCode.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedTask))
            {
                MessageBox.Show("กรุณาเลือกงานก่อน", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Dictionary<string, string> keyValues = new Dictionary<string, string>
            {
                { "username", txt_Username.Text },
                { "password", txt_Password.Text },
                { "keyDay", txt_keyDay.Text },
                { "0", txt_0.Text },
                { "60", txt_60.Text },
                { "120", txt_120.Text },
                { "180", txt_180.Text },
                { "240", txt_240.Text },
                { "300", txt_300.Text },
                { "360", txt_360.Text },
                { "420", txt_420.Text },
                { "480", txt_480.Text },
                { "540", txt_540.Text },
                { "600", txt_600.Text },
                { "GMkey", txt_GMkey.Text }
            };

            
            IWebDriver driver = WebDriverFactory.CreateChromeDriver(isHeadless: !cb_show.Checked);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            LoginHandler loginHandler = new LoginHandler(_Messagelog, driver);
            InjectionHandler injectionHandler = new InjectionHandler(_Messagelog, driver, wait, loginHandler);

            
            bool isCharacterSelectionRequired = injectionHandler.IsCharacterSelectionRequired(txt_GMkey.Text);
            string selectedCharacter = cbx_character.Text;

            if (isCharacterSelectionRequired && string.IsNullOrEmpty(selectedCharacter))
            {
                 
                selectedCharacter = injectionHandler.ShowCharacterSelectionPopup();

                if (string.IsNullOrEmpty(selectedCharacter))
                {
                    MessageBox.Show("กรุณาเลือกตัวละครก่อน", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            
            injectionHandler.InjectionCode(
                task: selectedTask,
                keyValues: keyValues,
                IDmode: !cb_swit.Checked,
                showBrowser: cb_show.Checked,
                isItemSelected: RB_items.Checked,
                selectedCharacter: selectedCharacter,
                gmKey: txt_GMkey.Text
            );
        }
        public void Quest()
        {
             
         
            _driver = WebDriverFactory.CreateChromeDriver(isHeadless: !cb_show.Checked);
            LoginHandler loginHandler = new LoginHandler(_Messagelog, _driver);
            QuestHandler questHandler = new QuestHandler(_Messagelog, loginHandler, _driver, this);

            if (_driver == null)
            {
                MessageBox.Show("ไม่สามารถสร้าง WebDriver ได้");
                return;
            }
            Dictionary<string, string> keyValues = new Dictionary<string, string>
            {
                { "username", txt_Username.Text },
                { "password", txt_Password.Text }
             
            };
            questHandler.Quest_Start(
            useDfLogin: !cb_swit.Checked,  
            showBrowser: cb_show.Checked,  
            keyValues: keyValues,  
            IsEventQuestm: cb_EventQuest.Checked,
            IsDaily_Quest: cb_Daily_Quest.Checked,
            IsPrivilege: cb_Privilege.Checked
            );

        }
     
        public void StartGame(bool IDmode)
        { 
            IWebDriver driver = WebDriverFactory.CreateChromeDriver(false);
            LoginHandler LoginHanders = new LoginHandler(_Messagelog, driver);
            
            try
            {
               _Messagelog. ClearRTB();
                _Messagelog.Line();
                _Messagelog.AppendToRtbNoTime("ระบบล็อคอินอัตโนมัติเริ่มทำงาน !");
                _Messagelog.Line();
                driver.Navigate().GoToUrl("https://auth.gg.in.th/authenticate_v3/Auth_Full/Login.aspx?appid=1&scope=&state=1&sourceid=1&redirecturi=https://sf.gg.in.th/newauthen/landingplatform.aspx&fblogin=true");

                if (IDmode)
                {
                    
                    LoginHanders.HandleGGIDLogin(txt_Username.Text, txt_Password.Text);
                     
                }
                else
                {
                    LoginHanders.HandleDfLogin(txt_Username.Text, txt_Password.Text);
                }
                Thread.Sleep(100);
                SendKeys.SendWait("{LEFT}");
                Thread.Sleep(200);
                SendKeys.SendWait("{ENTER}");
                Thread.Sleep(100);

            }
            catch (Exception ex)
            {
                _Messagelog.AppendToRtb($"Error: {ex.Message}");
                _Messagelog.Line();
                driver.Quit();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            finally
            {

            }

            _Messagelog.AppendToRtb("กำลังเริ่มเกม!");
            _Messagelog.Line();
            _Messagelog.AppendToRtb("นับถอยหลัง 5 วิ");
            Thread.Sleep(5000);
            driver.Quit();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
        public void ORC_Config()
        {
            //string patgs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            //Dictionary<string, string[]> folderFileMap = new Dictionary<string, string[]>
            //{
            //    {$"{patgs}/x86", new string[] { "Tesseract.dll", "tesseract50.dll", "leptonica-1.82.0.dll" } }
            //};


            string capturedImagePath = screenCapture.CaptureSelectedArea();

            if (!string.IsNullOrEmpty(capturedImagePath))
            {
                string extractedText = ocrProcessor.ExtractTextFromImage(capturedImagePath);
               _Messagelog. ClearRTB();
                _Messagelog.AppendToRtbNoTime($"รหัสที่ถอดได้: {extractedText}");
                txt_GMkey.Text = extractedText;
            }
            else
            {
                txt_GMkey.Text = "การจับภาพล้มเหลว";
            }
        }
        public void ORC_Check()
        {


            string baseFolder = AppDomain.CurrentDomain.BaseDirectory;
            Dictionary<string, string[]> folderFileMap = new Dictionary<string, string[]>
            {
                {Path.Combine(baseFolder, "tessdata"), new string[] { "eng.traineddata", "equ.traineddata", "osd.traineddata" } },
                {Path.Combine(baseFolder, "data"), new string[] { "Tesseract.dll" } }
            };
            foreach (var entry in folderFileMap)
            {
                string folder = entry.Key;
                string[] filesToCheck = entry.Value;

                if (Directory.Exists(folder))
                {
                    foreach (var fileName in filesToCheck)
                    {
                        string filePath = Path.Combine(folder, fileName);
                        if (File.Exists(filePath))
                        {
                            FileInfo fileInfo = new FileInfo(filePath);
                            if (fileInfo.Length > 0)
                            {
                                //_Messagelog.AppendToRtbNoTime($"พบไฟล์: {fileName} (ขนาด: {fileInfo.Length} bytes)");

                            }
                            else
                            {
                                _Messagelog.AppendToRtbNoTime($"พบไฟล์: {fileName} แต่ไฟล์ว่างเปล่า!");

                            }
                        }
                        else
                        {
                            _Messagelog.AppendToRtbNoTime($"ไม่พบไฟล์: {fileName}");
                            if (!Work2.IsBusy)
                            {
                                Work2.RunWorkerAsync("Download");
                            }
                        }
                    }
                }
                else
                {
                    _Messagelog.AppendToRtbNoTime($"ไม่พบโฟลเดอร์: {folder}");
                    _Messagelog.AppendToRtbNoTime("กำลังทำการเริ่มดาวน์โหลดส่วมเสริมเพิ่ม");
                    if (!Work2.IsBusy)
                    {
                        Work2.RunWorkerAsync("Download");
                    }


                }
            }




        }
        public void GetStringonEditor()
        {
            string installPath = "";
            installPath = (string)Registry.GetValue(
            @"HKEY_CURRENT_USER\Software\Dragonfly\SpecialForce",
            "InstallPath",
            null
        );


            if (!string.IsNullOrEmpty(installPath))
            {
                _Messagelog.AppendToRtbNoTime($"โฟลเดอร์เกม: {installPath}");
                 
               
            }
            else
            {
                _Messagelog.AppendToRtbNoTime($"ไม่พบค่า InstallPath ใน Registry");
            }

            string FileDumpName = "*.dmp";
            // ลบไฟล์จาก Program folder
            if (Directory.Exists(installPath))
            {
                foreach (string dumpFile in Directory.GetFiles(installPath, FileDumpName))
                {
                    _Messagelog.AppendToRtbNoTime($"พบไฟล์Errorของเกม: {dumpFile} ถูกลบแล้ว");
                    File.Delete(dumpFile);
                }
            }
        }
        #endregion
        #region SetupDriverConfig
       
        //Config FileAndFolder
        public void deletItems()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string folderProgram = Path.Combine(baseDir, "Program");
            string folderData = Path.Combine(baseDir, "data");
            string folderSelenium = Path.Combine(baseDir, "selenium-manager");

            string[] extensions = { "*.exe", "*.dll", "*.bat", "*.dmp" };

            string[] hiddenFiles = {  "Kuey_Teow.exe.config", "Kuey_Teow.pdb", "launcher.exe.config" };

            string[] deleteFiles = { "ReaLTaiizor.xml", "Tesseract.dll","Downlad.zip", "System.Drawing.Common.xml", "System.Drawing.Common.pdb", "Patagames.Ocr.xml",
        "System.IO.Pipelines.xml",  "AutoClick supply Box.exe", "Luncher.pdb", "ICSharpCode.SharpZipLib.pdb",
        "Discord.Net.Commands.xml", "Discord.Net.Core.xml", "Discord.Net.Interactions.xml", "Discord.Net.Rest.xml", "Discord.Net.Webhook.xml",
        "Discord.Net.WebSocket.xml", "ICSharpCode.SharpZipLib.xml", "Microsoft.Bcl.AsyncInterfaces.xml", "Microsoft.Extensions.DependencyInjection.Abstractions.xml",
        "Newtonsoft.Json.xml", "System.Buffers.xml", "System.Collections.Immutable.xml", "System.Interactive.Async.xml", "System.Linq.Async.xml",
        "System.Memory.xml", "System.Numerics.Vectors.xml", "System.Reactive.xml", "System.Runtime.CompilerServices.Unsafe.xml", "System.Threading.Tasks.Extensions.xml",
        "System.ValueTuple.xml", "WebDriver.Support.xml", "WebDriver.xml","System.Text.Encodings.Web.xml", "System.Text.Json.xml", "Microsoft.Win32.Registry.xml", "System.Security.Principal.Windows.xml",
        "System.Diagnostics.PerformanceCounter.xml", "Hardware.Info.xml", "System.Security.AccessControl.xml", "System.CodeDom.xml", "AutoClick.exe"
    };
            string[] excludeDlls = { "Ionic.Zip.dll", "ICSharpCode.SharpZipLib.dll", "tesseract.dll", "ReaLTaiizor.dll" };
            string[] tempFiles = { "Data.pdb" };
            string[] seleniumFolders = { "linux", "macos" };
            GetStringonEditor();
            // ปิดโปรแกรมที่รันอยู่
            foreach (Process proc in Process.GetProcesses())
            {
                string procName = proc.ProcessName.ToLower();
                string exeName = procName + ".exe";

                if (deleteFiles.Contains(exeName, StringComparer.OrdinalIgnoreCase))
                {
                    try
                    {
                        _Messagelog.AppendToRtb($"ปิดโปรแกรมที่รันอยู่: {exeName}");
                        proc.Kill();
                        proc.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"กรุณาปิดโปรแกรม {exeName} ก่อน\n\n{ex.Message}", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            // ลบไฟล์จาก Program folder
            if (Directory.Exists(folderProgram))
            {
                foreach (string ext in extensions)
                {
                    foreach (string file in Directory.GetFiles(folderProgram, ext))
                    {
                        _Messagelog.AppendToRtbNoTime($"เคลียร์ไฟล์: {file}");
                        File.Delete(file);
                    }
                }
            }

            // ซ่อนไฟล์ที่กำหนด
            foreach (string file in hiddenFiles.Select(f => Path.Combine(baseDir, f)))
            {
                if (File.Exists(file))
                {
                    File.SetAttributes(file, File.GetAttributes(file) | FileAttributes.Hidden);
                }
            }

            // ลบไฟล์ที่ระบุ
            if (Directory.Exists(baseDir))
            {
                foreach (string file in deleteFiles)
                {
                    string path1 = Path.Combine(baseDir, file);
                    if (File.Exists(path1))
                    {
                        _Messagelog.AppendToRtb($"ลบไฟล์: {path1}");
                        File.Delete(path1);
                    }
                }
            }

            foreach (string file in deleteFiles)
            {
                string path2 = Path.Combine(baseDir, file);
                if (File.Exists(path2))
                {
                    _Messagelog.AppendToRtb($"ลบไฟล์: {path2}");
                    File.Delete(path2);
                }
            }

            // ลบโฟลเดอร์ selenium ที่ไม่ต้องการ
            if (Directory.Exists(folderSelenium))
            {
                foreach (string folder in seleniumFolders)
                {
                    string path = Path.Combine(folderSelenium, folder);
                    if (Directory.Exists(path))
                    {
                        _Messagelog.AppendToRtb($"ลบโฟลเดอร์: {path}");
                        Directory.Delete(path, true);
                    }
                }
            }

            // ลบ temp files
            if (Directory.Exists(folderData))
            {
                foreach (string file in tempFiles.Select(f => Path.Combine(folderData, f)))
                {
                    if (File.Exists(file))
                    {
                        _Messagelog.AppendToRtb($"ลบไฟล์: {file}");
                        File.Delete(file);
                    }
                }
            }

            // ลบ .dll ที่ไม่อยู่ในรายการยกเว้น
            if (Directory.Exists(baseDir))
            {
                try
                {
                    foreach (string file in Directory.GetFiles(baseDir, "*.dll"))
                    {
                        if (!excludeDlls.Contains(Path.GetFileName(file), StringComparer.OrdinalIgnoreCase))
                        {
                            _Messagelog.AppendToRtbNoTime($"ลบไฟล์ .dll: {file}");
                            File.Delete(file);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _Messagelog.AppendToRtbNoTime($"เกิดข้อผิดพลาด: {ex.Message}");
                }
            }
        }

        public static void HideFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
                directoryInfo.Attributes |= FileAttributes.Hidden;
            }
            else
            {

            }
        }
       
        #endregion
        #region Setup Save And Loads
        public void Loads()
        {
            //โหลดคีย์เวิดที่เคยบันทึกไว้

            cb_EbID.Checked = Kuey_Teow.Properties.Settings.Default.CK_SaveID;
            cb_show.Checked = Kuey_Teow.Properties.Settings.Default.webShow;
            txt_Time.Text = Kuey_Teow.Properties.Settings.Default.Time;
            txt_keyDay.Text = Kuey_Teow.Properties.Settings.Default.Keydaytime;
            txt_0.Text = Kuey_Teow.Properties.Settings.Default.ZeroTime;
            txt_60.Text = Kuey_Teow.Properties.Settings.Default.SixtyTime;
            txt_120.Text = Kuey_Teow.Properties.Settings.Default.onetwenty;
            txt_180.Text = Kuey_Teow.Properties.Settings.Default.oneeighty;
            txt_240.Text = Kuey_Teow.Properties.Settings.Default.twoforty;
            txt_300.Text = Kuey_Teow.Properties.Settings.Default.Threehundred;
            txt_360.Text = Kuey_Teow.Properties.Settings.Default.ThreeSixty;
            txt_420.Text = Kuey_Teow.Properties.Settings.Default.fortytwenty;
            txt_480.Text = Kuey_Teow.Properties.Settings.Default.foryeighty;
            txt_540.Text = Kuey_Teow.Properties.Settings.Default.fiveforty;
            txt_600.Text = Kuey_Teow.Properties.Settings.Default.Sixhundred;
            txt_Username.Text = Kuey_Teow.Properties.Settings.Default.Username01;
            txt_Password.Text = Kuey_Teow.Properties.Settings.Default.Password01;
            cb_swit.Checked = Kuey_Teow.Properties.Settings.Default.SwitLogin1;
            cb_Daily_Quest.Checked = Kuey_Teow.Properties.Settings.Default.cb_dalys;
            cb_Privilege.Checked = Kuey_Teow.Properties.Settings.Default.cb_pv;
            cb_EventQuest.Checked = Kuey_Teow.Properties.Settings.Default.cb_event;




        }
        public void SaveSettings()
        {
            //เซฟคีย์เวิร์ดที่ ดึงมาจากหน้าเว็ป


            Kuey_Teow.Properties.Settings.Default.CK_SaveID = cb_EbID.Checked;
            Kuey_Teow.Properties.Settings.Default.Time = txt_Time.Text;
            Kuey_Teow.Properties.Settings.Default.Keydaytime = txt_keyDay.Text;
            Kuey_Teow.Properties.Settings.Default.ZeroTime = txt_0.Text;
            Kuey_Teow.Properties.Settings.Default.SixtyTime = txt_60.Text;
            Kuey_Teow.Properties.Settings.Default.onetwenty = txt_120.Text;
            Kuey_Teow.Properties.Settings.Default.oneeighty = txt_180.Text;
            Kuey_Teow.Properties.Settings.Default.twoforty = txt_240.Text;
            Kuey_Teow.Properties.Settings.Default.Threehundred = txt_300.Text;
            Kuey_Teow.Properties.Settings.Default.ThreeSixty = txt_360.Text;
            Kuey_Teow.Properties.Settings.Default.fortytwenty = txt_420.Text;
            Kuey_Teow.Properties.Settings.Default.foryeighty = txt_480.Text;
            Kuey_Teow.Properties.Settings.Default.fiveforty = txt_540.Text;
            Kuey_Teow.Properties.Settings.Default.Sixhundred = txt_600.Text;
            Kuey_Teow.Properties.Settings.Default.webShow = cb_show.Checked;
            Kuey_Teow.Properties.Settings.Default.cb_dalys = cb_Daily_Quest.Checked;
            Kuey_Teow.Properties.Settings.Default.cb_event = cb_EventQuest.Checked;
            Kuey_Teow.Properties.Settings.Default.cb_pv = cb_Privilege.Checked;

            //Get_CodeSF.Properties.Settings.Default.LoginSaveDF1 = rb_DfLogin.Checked;
            // Get_CodeSF.Properties.Settings.Default.LoginSaveGG1 = rb_GGID.Checked;
            Kuey_Teow.Properties.Settings.Default.Save();

        }
        public void SaveID()
        {
            if (rb_ID1.Checked)
            {

                cb_swit.Checked = Kuey_Teow.Properties.Settings.Default.SwitLogin1;
                txt_Username.Text = Kuey_Teow.Properties.Settings.Default.Username01;
                txt_Password.Text = Kuey_Teow.Properties.Settings.Default.Password01;

            }
            else if (rb_ID2.Checked)
            {

                cb_swit.Checked = Kuey_Teow.Properties.Settings.Default.SwitLogin2;
                txt_Username.Text = Kuey_Teow.Properties.Settings.Default.Username02;
                txt_Password.Text = Kuey_Teow.Properties.Settings.Default.Password02;

            }
            else if (rb_ID3.Checked)
            {

                cb_swit.Checked = Kuey_Teow.Properties.Settings.Default.SwitLogin3;
                txt_Username.Text = Kuey_Teow.Properties.Settings.Default.Username03;
                txt_Password.Text = Kuey_Teow.Properties.Settings.Default.Password03;
            }
            else if (rb_ID4.Checked)
            {

                cb_swit.Checked = Kuey_Teow.Properties.Settings.Default.SwitLogin4;
                txt_Username.Text = Kuey_Teow.Properties.Settings.Default.Username04;
                txt_Password.Text = Kuey_Teow.Properties.Settings.Default.Password04;

            }
        }
        #endregion
        #region DownloadFile
        private void DownloadFile()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompletedCallback);

                    Invoke(new Action(() =>
                    {
                        progressBar1.Visible = true;
                        progressBar1.Value = 0;
                        _Messagelog.AppendToRtbNoTime("กำลังดาวน์โหลดไฟล์เสริม...");
                    }));

                    client.DownloadFileAsync(new Uri(downloadUrl), updatePath);
                }
            }
            catch (Exception ex)
            {
                _Messagelog.AppendToRtbNoTime("เกิดข้อผิดพลาดขณะดาวน์โหลด: " + ex.Message);
            }
        }
        private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
        private void DownloadFileCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                string extractPath = AppDomain.CurrentDomain.BaseDirectory;

                
                if (!workExtractFile.IsBusy)
                {
                    workExtractFile.RunWorkerAsync(extractPath);
                }
            }
            catch (Exception ex)
            {
                _Messagelog.AppendToRtbNoTime("เกิดข้อผิดพลาดขณะขยายไฟล์: " + ex.Message);
            }
            finally
            {
                Invoke(new Action(() => progressBar1.Visible = false));
            }
        }
        private void ExtractZipFile(string zipPath, string extractPath)
        {
            try
            {
                if (!File.Exists(zipPath))
                {
                    throw new FileNotFoundException("ไม่พบไฟล์ ZIP ที่ต้องการขยาย", zipPath);
                }

                using (FileStream fs = File.OpenRead(zipPath))
                using (ZipFile zf = new ZipFile(fs))
                {
                    int totalEntries = (int)zf.Count;
                    int extractedEntries = 0;

                    Invoke(new Action(() =>
                    {
                        progressBar1.Visible = true;
                        progressBar1.Value = 0;
                    }));

                    foreach (ZipEntry entry in zf)
                    {
                        if (!entry.IsFile)
                        {
                            continue;
                        }

                        string entryFileName = entry.Name;
                        byte[] buffer = new byte[4096];
                        string fullZipToPath = Path.Combine(extractPath, entryFileName);
                        string directoryName = Path.GetDirectoryName(fullZipToPath);

                        if (!string.IsNullOrEmpty(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }

                        _Messagelog.AppendToRtbNoTime($"กำลังขยายไฟล์ไปที่: {fullZipToPath}");

                        try
                        {
                            using (FileStream streamWriter = File.Create(fullZipToPath))
                            {
                                Stream zipStream = zf.GetInputStream(entry);
                                StreamUtils.Copy(zipStream, streamWriter, buffer);
                            }
                        }
                        catch (Exception ex)
                        {
                            _Messagelog.AppendToRtbNoTime("เกิดข้อผิดพลาดขณะคัดลอกไฟล์: " + ex.Message);
                        }

                        extractedEntries++;

                        Invoke(new Action(() =>
                        {
                            progressBar1.Value = (int)((double)extractedEntries / totalEntries * 100);
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                _Messagelog.AppendToRtbNoTime("เกิดข้อผิดพลาดขณะขยายไฟล์: " + ex.Message);
            }
            finally
            {
                Invoke(new Action(() => progressBar1.Visible = false));
            }
        }
        #endregion
        private void groupBox2_Click(object sender, EventArgs e)
        {

        }

        private void foreverForm1_Click(object sender, EventArgs e)
        {

        }

        private void Mu_KropMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
            deletItems();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void Mu_KropMain_Load(object sender, EventArgs e)
        {
            
            if (!workExtractFile.IsBusy)
            {
                ORC_Check();
                deletItems();
                
            }
            
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
           
            Application.Exit();
        }

        private void Work0_DoWork(object sender, DoWorkEventArgs e)
        {
            string task = (string)e.Argument;
            switch (task)
            {
                case "ItemCode":
                    ItemCodeExtractor extractor = new ItemCodeExtractor(_Messagelog, this);
                    extractor.ExtractItemCodes();
                    break;
                case "KeyItem":
                    XjCode();
                    break;
                case "Quest_Event":
                    Quest();
                    break;
                case "StartGame":
                    StartGame(cb_swit.Checked);
                    break;
                case "DownloadFile":
                    DownloadFile();
                    break;
                case "ORC_Config":
                    ORC_Config();
                    break;
                default:
                    throw new ArgumentException("Invalid task");

            }
        }

        private void btn_GetItemcode_Click(object sender, EventArgs e)
        {
            if (!Work0.IsBusy)
            {
                Work0.RunWorkerAsync("ItemCode");

            }
            else
            {
                MessageBox.Show("Error");
            }
        }

        private void chatButtonLeft1_Click(object sender, EventArgs e)
        {
             
        }

        private void Work2_DoWork(object sender, DoWorkEventArgs e)
        {
            string task = (string)e.Argument;
            switch (task)
            {
                case "ORC_Cap":
                    ORC_Config();
                    break;
                case "Download":
                    DownloadFile();
                    break;
                default:
                    throw new ArgumentException("Invalid task");

            }
        }

        private void workExtractFile_DoWork(object sender, DoWorkEventArgs e)
        {
            string extractPath = e.Argument as string;
            ExtractZipFile(updatePath, extractPath);
        }

        private void RB_items_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_items.Checked)
            {
                cbx_character.Enabled = false;
            }
        }

        private void rb_Exp_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Exp.Checked)
            {
                cbx_character.Enabled = false;

            }

        }

        private void RB_character_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_character.Checked)
            {
                cbx_character.Enabled = true;

            }
        }

        private void rb_ID1_CheckedChanged(object sender, EventArgs e)
        {
            SaveID();
        }

        private void rb_ID2_CheckedChanged(object sender, EventArgs e)
        {
            SaveID();
        }

        private void rb_ID3_CheckedChanged(object sender, EventArgs e)
        {
            SaveID();
        }

        private void rb_ID4_CheckedChanged(object sender, EventArgs e)
        {
            SaveID();
        }

        private void btn_SaveID_Click(object sender, EventArgs e)
        {
            if (rb_ID1.Checked)
            {

                Kuey_Teow.Properties.Settings.Default.SwitLogin1 = cb_swit.Checked;
                Kuey_Teow.Properties.Settings.Default.Username01 = txt_Username.Text;
                Kuey_Teow.Properties.Settings.Default.Password01 = txt_Password.Text;
                Kuey_Teow.Properties.Settings.Default.Save();
            }
            else if (rb_ID2.Checked)
            {

                Kuey_Teow.Properties.Settings.Default.SwitLogin2 = cb_swit.Checked;
                Kuey_Teow.Properties.Settings.Default.Username02 = txt_Username.Text;
                Kuey_Teow.Properties.Settings.Default.Password02 = txt_Password.Text;
                Kuey_Teow.Properties.Settings.Default.Save();
            }
            else if (rb_ID3.Checked)
            {

                Kuey_Teow.Properties.Settings.Default.SwitLogin3 = cb_swit.Checked;
                Kuey_Teow.Properties.Settings.Default.Username03 = txt_Username.Text;
                Kuey_Teow.Properties.Settings.Default.Password03 = txt_Password.Text;
                Kuey_Teow.Properties.Settings.Default.Save();
            }
            else if (rb_ID4.Checked)
            {

                Kuey_Teow.Properties.Settings.Default.SwitLogin4 = cb_swit.Checked;
                Kuey_Teow.Properties.Settings.Default.Username04 = txt_Username.Text;
                Kuey_Teow.Properties.Settings.Default.Password04 = txt_Password.Text;
                Kuey_Teow.Properties.Settings.Default.Save();
            }
        }

        private void btn_injunctionCode_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(cbx_ItemCode.Text))
            {
                MessageBox.Show("กรุณาเลือกไอเทมก่อน", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (!Work0.IsBusy)
            {
                Work0.RunWorkerAsync("KeyItem");
            }
            else
            {
                _Messagelog.AppendToRtbNoTime("Error: BackgroundWorker กำลังทำงานอยู่");
            }
        }

        private void btn_Cap_Click(object sender, EventArgs e)
        {
            if (!Work0.IsBusy)
            {
                Work0.RunWorkerAsync("ORC_Config");

            }
            else
            {
                _Messagelog.AppendToRtbNoTime("กำลังทำงานอยู่");
            }
        }

        private void btn_StartProgram_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbx_Program.Text))
            {
                MessageBox.Show("กรุณาเลือกไอเทมก่อน", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (!exFileRun.IsBusy)
                {
                    exFileRun.RunWorkerAsync(cbx_Program.Text);
                }
                else
                {
                    _Messagelog.AppendToRtbNoTime("กำลังทำงานอยู่");
                }
            }
        }

        private void btn_StartGame_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_Username.Text))
            {
                MessageBox.Show("กรุณากรอกไอดีพาสเวิร์ดก่อน", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Question);

            }
            else
            {
                if (!Work0.IsBusy)
                {
                    Work0.RunWorkerAsync("StartGame");


                }
                else
                {
                    _Messagelog.AppendToRtbNoTime("กำลังทำงานอยู่");
                }
            }
        }

        private void btn_Coppy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(
           $"Day = {txt_Time.Text}\n\n" +
           $"Key = {txt_keyDay.Text}\n " +
           $"0   = {txt_0.Text}\n" +
           $"60  = {txt_60.Text}\n" +
           $"120 = {txt_120.Text}\n" +
           $"180 = {txt_180.Text}\n" +
           $"240 = {txt_240.Text}\n" +
           $"300 = {txt_300.Text}\n" +
           $"360 = {txt_360.Text}\n" +
           $"420 = {txt_420.Text}\n" +
           $"480 = {txt_480.Text}\n" +
           $"540 = {txt_540.Text}\n" +
           $"600 = {txt_600.Text}\n\n"
           );

        }

        private void btn_StartQuest_Click(object sender, EventArgs e)
        {
            if (!new List<ReaLTaiizor.Controls.CheckBox> {
                cb_Daily_Quest,
                cb_EventQuest,
                cb_Privilege
                 }.Any(cb => cb.Checked))
            {
                MessageBox.Show("กรุณาเลือก Checkbox อย่างน้อย 1 ตัวก่อน", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            };

            if (!Work0.IsBusy)
            {
                Work0.RunWorkerAsync("Quest_Event");

            }
            else
            {
                MessageBox.Show("รอการทำงานสิ้นสุดก่อนค่อยกดเริ่มใหม่");
            }
        }

        private void Work0_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _Messagelog.AppendToRtbNoTime("สิ้นสุดการทำงาน");
            _Messagelog.Line();

        }

        private void cb_EbID_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_EbID.Checked)
            {
                txt_Username.Enabled = false;
                txt_Password.Enabled = false;
            }
            else
            {
                txt_Username.Enabled = true;
                txt_Password.Enabled = true;
            }
        }

        private void exFileRun_DoWork(object sender, DoWorkEventArgs e)
        {
            string task = (string)e.Argument;
           
            switch (task)
            {
                case "AutoSupply_Box":
                    ex.ExEStart("AutoClick.exe", "AutoClick.exe", true, IsLogMessages, IsMessages);
                    break;
                default:
                    _Messagelog.AppendToRtbNoTime($"ไม่มีฟังก์ชัน {task}");
                    break;



            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void rtb_01_TextChanged(object sender, EventArgs e)
        {

        }

        private void linkLabelEdit1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("Chrome", "https://www.facebook.com/PLAMSsE");
        }

        private void linkLabelEdit2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("Chrome", "https://github.com/serizava01");
        }
    }
}
