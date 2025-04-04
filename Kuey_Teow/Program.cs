using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kuey_Teow
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static Mutex mutex = new Mutex(true, "Kuey_Teow");
        [STAThread]
        static void Main()
        {
             
            //if (mutex.WaitOne(TimeSpan.Zero, true))
            //{
            //    Application.EnableVisualStyles();
            //    Application.SetCompatibleTextRenderingDefault(false);
            //    Application.Run(new mu_krop());

            //}
            //else
            //{
            //    MessageBox.Show("โปรแกรมถูกเปิดอยูแล้ว", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            string processName = Process.GetCurrentProcess().ProcessName;
            var runningProcesses = Process.GetProcessesByName(processName);

            if (runningProcesses.Length > 20) // ต้องเป็น 2 ไม่ใช่ >= 2
            {
                MessageBox.Show("มีโปรแกรมเปิดอยู่แล้ว 20 อินสแตนซ์!", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Mu_KropMain());
            //mutex.ReleaseMutex();
        }
    }
}
