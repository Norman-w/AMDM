using AutoUpdate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace FakeHISServer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Console.WriteLine("检查是否需要更新");
            if (!Debugger.IsAttached)
            {
                Console.WriteLine("正在检查新版本");
                bool needUpdate = AutoUpdater.InjectAndCheckNeedUpdate(Application.StartupPath, Application.ExecutablePath, true);
                if (needUpdate)
                {
                    Console.WriteLine("需要更新,本进程将退出!一会再见");
                    return;
                }
            }
            Application.Run(new MainForm());
        }
    }
}
