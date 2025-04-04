using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public class RichTextBoxLogger
{
    private RichTextBox rtb_01;
    public bool IsLogMessages { get; set; } = true;

    public RichTextBoxLogger(RichTextBox richTextBox)
    {
        rtb_01 = richTextBox;
    }

    public void AppendToRtb(string text)
    {
        if (!IsLogMessages) return;

        if (rtb_01.InvokeRequired)
        {
            rtb_01.Invoke((MethodInvoker)delegate
            {
                string timeStampedMessage = $"{DateTime.Now:hh:mm:ss} {text}";
                rtb_01.AppendText(CleanErrorMessage(timeStampedMessage + Environment.NewLine));
                rtb_01.ScrollToCaret();
            });
        }
        else
        {
            rtb_01.AppendText(CleanErrorMessage(text + Environment.NewLine));
            rtb_01.ScrollToCaret();
        }
    }

    public void AppendToRtbNoTime(string text)
    {
        if (!IsLogMessages) return;

        if (rtb_01.InvokeRequired)
        {
            rtb_01.Invoke((MethodInvoker)delegate
            {
                rtb_01.AppendText(CleanErrorMessage($"{text + Environment.NewLine}"));
                rtb_01.ScrollToCaret();
            });
        }
        else
        {
            rtb_01.AppendText(CleanErrorMessage( text + Environment.NewLine));
            rtb_01.ScrollToCaret();
        }
    }

    public void Line()
    {
        if (!IsLogMessages) return;

        if (rtb_01.InvokeRequired)
        {
            rtb_01.Invoke(new Action(() =>
            {
                rtb_01.AppendText(CleanErrorMessage($" ===============================================" + Environment.NewLine));
                rtb_01.ScrollToCaret();
            }));
        }
        else
        {
            rtb_01.AppendText(CleanErrorMessage($" ===============================================" + Environment.NewLine));
            rtb_01.ScrollToCaret();
        }
    }

    public void ClearRTB()
    {
        if (rtb_01.InvokeRequired)
        {
            rtb_01.Invoke(new Action(() =>
            {
                rtb_01.Clear();
            }));
        }
        else
        {
            rtb_01.Clear();
        }
    }
    private string CleanErrorMessage(string message)
    {
        return Regex.Replace(message, @"\(Session info:.*?\)", "").Trim() + Environment.NewLine;
    }
}



//    private RichTextBoxLogger _Messagelog; ประกาศตัวแปรในคลาส 
//    _Messagelog = new RichTextBoxLogger(Richtextbox);  ตั้งค่าใช้งานเชื่อมต่อเข้ากับ RichTextBox เอาไว้ใน  Loadforme หรือ หลังจาก InitializeComponent();