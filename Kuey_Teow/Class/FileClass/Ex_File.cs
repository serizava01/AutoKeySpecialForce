using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

public class EX_File
{
    private readonly string _resourceBase;
    private readonly string _dllFileName;
    private readonly string _dllPath;
    private readonly RichTextBox _logTextBox;
    private readonly string _folder;
    private bool HideFIle = false;
     public EX_File(string resourceBase, RichTextBox logTextBox, string dllFileName, string folder)
     {
        _resourceBase = resourceBase;
        _dllFileName = dllFileName;
        _folder = folder;
        _dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _folder, _dllFileName);
        _logTextBox = logTextBox;
         
     }

    
    public  void Showresource()
    {
         
        Assembly resourceAssembly = Assembly.LoadFrom(_dllPath);
        string[] resourceNames = resourceAssembly.GetManifestResourceNames();

        string message = "Resources in DLL:\n\n";
        foreach (string resourceName in resourceNames)
        {
            
            message += resourceName + "\n";
        }

        MessageBox.Show(message, $"�����ª�������� ", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }  


    private void RunProgram(string programName, bool logToTextBox = false, bool showMessageBox = false)
    {
        string outputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Program", programName);
        FileInfo fileInfo = new FileInfo(programName);
        try
        {
            Process.Start(outputFilePath);
            if (logToTextBox)
            {
                LogMessage($"����� '{programName}' (��Ҵ: {fileInfo.Length} bytes) ������ӧҹ ���������.", false, false);
            }
        }
        catch (Exception ex)
        {
            LogMessage($"�Դ��ͼԴ��Ҵ㹡������������ '{programName}' (��Ҵ: {fileInfo.Length} bytes): {ex.Message}", logToTextBox, showMessageBox);
        }
    }

    public void ExEStart(string exResource, string programName, bool shouldRun = false, bool logToTextBox = false, bool showMessageBox = false)
    {
        Assembly assembly = Assembly.LoadFile(_dllPath);
        string resourceName = _resourceBase + exResource;

        ExtractResource(assembly, resourceName, programName, logToTextBox, showMessageBox, false);

        if (shouldRun)
        {
            RunProgram(programName, logToTextBox, showMessageBox);
        }
    }

    private void ExtractResource(Assembly assembly, string resourceName, string outputFileName, bool logToTextBox, bool showMessageBox, bool hideFIle)
    {
        hideFIle = HideFIle;
        FileInfo fileInfo = new FileInfo(outputFileName);
        try
        {
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream != null)
                {
                    string outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Program");
                    string outputFilePath = Path.Combine(outputDirectory, outputFileName);

                    if (!Directory.Exists(outputDirectory))
                    {
                        try
                        {
                            Directory.CreateDirectory(outputDirectory);
                            LogMessage($"���ҧ������ '{outputDirectory}' ���������.", logToTextBox, showMessageBox);
                        }
                        catch (Exception ex)
                        {
                            LogMessage($"�Դ��ͼԴ��Ҵ㹡�����ҧ������ '{outputDirectory}': {ex.Message}", logToTextBox, showMessageBox);
                            return;
                        }
                    }

                    if (File.Exists(outputFilePath))
                    {
                        LogMessage($"��� '{outputFileName}'(��Ҵ: {fileInfo.Length} bytes) ����������. ���Թ��õ���.", logToTextBox, showMessageBox);
                    }
                    else
                    {
                        using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                        {
                            resourceStream.CopyTo(fileStream);
                        }

                        try
                        {
                            if (hideFIle)
                            {
                                File.SetAttributes(outputFilePath, File.GetAttributes(outputFilePath) | FileAttributes.Hidden);
                                LogMessage($"�ӡ�ë�͹��� '{outputFileName}' ��������� ���ѧ����������", logToTextBox, showMessageBox);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogMessage($"���ѭ������ǡѺ��ë�͹��� '{outputFileName}': {ex.Message}", logToTextBox, showMessageBox);
                        }
                    }
                }
                else
                {
                    LogMessage($"��辺�����ի����: {resourceName}", logToTextBox, showMessageBox);
                }
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessage($"�����Ҷ֧�١����ʸ�������Ҷ֧��� '{outputFileName}': {ex.Message}", logToTextBox, showMessageBox);
        }
        catch (Exception ex)
        {
            LogMessage($"���ѭ������ǡѺ������ҧ�����ի���� '{resourceName}': {ex.Message}", logToTextBox, showMessageBox);
        }
    }

    private void LogMessage(string message, bool logToTextBox = false, bool showMessageBox = false)
    {
         
        if (showMessageBox)
        {
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;  
        }

         
        if (logToTextBox && _logTextBox != null)
        {
            if (_logTextBox.InvokeRequired)
            {
                _logTextBox.Invoke((MethodInvoker)delegate {_logTextBox.AppendText(CleanErrorMessage(message + Environment.NewLine)); });
            }
            else
            {
                _logTextBox.AppendText(CleanErrorMessage(message + Environment.NewLine));
                _logTextBox.ScrollToCaret();
            }
        }

    }
    private string CleanErrorMessage(string message)
    {
        return Regex.Replace(message, @"\(Session info:.*?\)", "").Trim();
    }
} 


//string customDll = "CustomData.dll";
//string customFolder = "customFolder";
//EX_File exFile = new EX_File("Data.Resources.", richTextBox, customDll, customFolder);