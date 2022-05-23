using System;
using System.Reflection;
using System.Windows.Forms;
using AutoUpdate;
using   System.Diagnostics;
using MyCode;

/*
 * 明天要使用libvlcsharp的winform以及字幕显示的3.6.1版本 摄像头考虑也使用他 但是好像失败了  不行就用dshow试试  还是算了 节省时间
 * 明天再完善交互 取药中页面等
 */

namespace AMDM
{
    static class Program
    {
        static Program()
        {
            //string password = "123123";
            ////md5 = MD5.Create(password).ToString();
            //byte[] result = System.Text.Encoding.Default.GetBytes(password);    //tbPass为输入密码的文本框
            //System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            //byte[] output = md5.ComputeHash(result);
            //string md5String = BitConverter.ToString(output).Replace("-", "");  //tbMd5pass为输出加密文本的文本框

        //    var sss =Assembly.GetExecutingAssembly().GetReferencedAssemblies()[9];
        //    var ssw = Assembly.GetAssembly(sss.GetType());
        //    var ss = AppDomain.CurrentDomain.GetAssemblies();
            //Console.WriteLine("开始检查dll");
            //DllLoader.Load();
            //Console.WriteLine("结束检查dll");
        }
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Utils.LogBug("amdm的program函数被加载了");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Console.WriteLine("检查是否需要更新");
            //if (!Debugger.IsAttached)
            //{
            //    Console.WriteLine("正在检查新版本");
            //    bool needUpdate = AutoUpdater.InjectAndCheckNeedUpdate(Application.StartupPath, Application.ExecutablePath, true);
            //    if (needUpdate)
            //    {
            //        Console.WriteLine("需要更新");
            //        return;
            //    }
            //}
            InitializerForm iform = new InitializerForm();
            //iform.ClientSize = new Size(100, 100);
            iform.StartPosition = FormStartPosition.CenterScreen;
            iform.WindowState = FormWindowState.Maximized;
            Application.Run(iform);
            //AMDM.ExpirationDataSelectForm eform = new ExpirationDataSelectForm();
            //eform.WindowState = FormWindowState.Maximized;
            //FormAutoSizer fs = new FormAutoSizer(eform);
            //fs.TurnOnAutoSize();
            //Application.Run(eform);
        }
    }
}
