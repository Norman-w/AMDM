using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace LogServerService
{
    static class Program
    {
        #region 全局变量,根据自己的使用情况直接修改该包围块中的内容即可
        
        /// <summary>
        /// 服务的名称.
        /// </summary>
        public static string ServiceName = "LogServerService";
        /// <summary>
        /// 服务显示名称
        /// </summary>
        public static string ServiceDisplayName = "LogServerService";
        /// <summary>
        /// 服务描述
        /// </summary>
        public static string ServiceDesc = "潮咖医疗付药机看门狗程序,用以检测付药机程序的状态及自动启动付药机程序";
        /// <summary>
        /// 服务的exe要安装到哪个文件夹下,生成了服务以后直接把文件复制到目标位置,然后安装的时候使用这个位置.
        /// </summary>
        public static string ServiceInstallPath = "c:\\付药机看门狗服务";
        /// <summary>
        /// 用于运行console模式的时候,使用的服务相关处理情况日志目录,为保证日志文件不会过大,通常只在服务启动错误的时候使用.
        /// </summary>
        public static string ConsoleTestLogFilePath = "c:\\windows服务部署器日志.log";

        #endregion
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string[] args)
        {
            #region 不参数启动exe文件的时候就是启动服务的进程
            if (args == null || args.Length == 0)
            {
                try
                {
                    ServiceBase[] serviceToRun = new ServiceBase[] { new LogServerService() };
                    ServiceBase.Run(serviceToRun);
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllText(ConsoleTestLogFilePath, "\r\n服务启动失败,启动时间：" + DateTime.Now.ToString() + "\r\n错误信息:" + ex.Message);
                }
            }
            #endregion
            #region 带参数启动exe的时候就是运行安装部署器,在项目的属性->调试->启动选项->命令行参数 内 添加任意参数即可.可以根据具体的情况确认是否严格校验参数或指定不同的参数对应的功能.
            else
            {
            //开始标记的位置,可供goto使用
            StartLocation:
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("********************************************************");
                Console.ResetColor();
                Console.WriteLine("当前时间:{0}", DateTime.Now);
                Console.WriteLine("1:删除服务(如存在)+安装服务+启动服务");
                Console.WriteLine("2:安装服务");
                Console.WriteLine("3:卸载服务");
                Console.WriteLine("4:服务状态检查");
                Console.WriteLine("5:启动服务");
                Console.WriteLine("6:停止服务");
                Console.WriteLine("7:删除服务(使用sc delete 服务名)");
                Console.WriteLine("8:调试服务逻辑代码");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("********************************************************");
                Console.ResetColor();
                ConsoleKey key = Console.ReadKey().Key;
                #region 按键1自动部署服务 如果存在服务,卸载,然后重新安装,安装后启动.
                if (key == ConsoleKey.NumPad1 || key == ConsoleKey.D1)
                {
                    if (ServiceConfigurator.IsServiceExisted(ServiceName))
                    {
                        ServiceConfigurator.UnInstallService(ServiceName);
                    }
                    if (!ServiceConfigurator.IsServiceExisted(ServiceName))
                    {
                        ServiceConfigurator.InstallService(ServiceName, ServiceInstallPath, ServiceDisplayName, ServiceDesc);
                    }
                    ServiceConfigurator.StartService(ServiceName);
                    goto StartLocation;
                }
                #endregion
                #region 按键2的安装服务
                else if (key == ConsoleKey.NumPad2 || key == ConsoleKey.D2)
                {
                    if (!ServiceConfigurator.IsServiceExisted(ServiceName))
                    {
                        ServiceConfigurator.InstallService(ServiceName, ServiceInstallPath, ServiceDisplayName, ServiceDesc);
                    }
                    else
                    {
                        Console.WriteLine("\n服务已存在......");
                    }
                    goto StartLocation;
                }
                #endregion
                #region 按键3的卸载服务
                else if (key == ConsoleKey.NumPad3 || key == ConsoleKey.D3)
                {
                    if (ServiceConfigurator.IsServiceExisted(ServiceName))
                    {
                        ServiceConfigurator.UnInstallService(ServiceName);
                    }
                    else
                    {
                        Console.WriteLine("\n服务不存在......");
                    }
                    goto StartLocation;
                }
                #endregion
                #region 按键4的查看服务状态
                else if (key == ConsoleKey.NumPad4 || key == ConsoleKey.D4)
                {
                    if (!ServiceConfigurator.IsServiceExisted(ServiceName))
                    {
                        Console.WriteLine("\n服务不存在......");
                    }
                    else
                    {
                        Console.WriteLine("\n服务状态：" + ServiceConfigurator.GetServiceStatus(ServiceName).ToString());
                    }
                    goto StartLocation;
                }
                #endregion
                #region 按键5的启动服务
                else if (key == ConsoleKey.NumPad5 || key == ConsoleKey.D5)
                {
                    ServiceConfigurator.StartService(ServiceName);
                    Console.WriteLine("执行启动后的服务状态：" + ServiceConfigurator.GetServiceStatus(ServiceName).ToString());
                    goto StartLocation;
                }
                #endregion
                #region 按键6的停止服务
                else if (key == ConsoleKey.NumPad6 || key == ConsoleKey.D6)
                {
                    ServiceConfigurator.StopService(ServiceName);
                    Console.WriteLine("执行停止后的服务状态：" + ServiceConfigurator.GetServiceStatus(ServiceName).ToString());
                    goto StartLocation;
                }
                #endregion
                #region 按键7的删除服务使用sc
                else if (key == ConsoleKey.NumPad7 || key == ConsoleKey.D7)
                {
                    Console.WriteLine("正在使用sc命令删除服务......");
                    Process p = new Process();
                    p.StartInfo.FileName = @"C:\WINDOWS\system32\cmd.exe ";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    p.StandardInput.WriteLine(string.Format("net stop {0}", ServiceName));
                    p.StandardInput.WriteLine(string.Format("sc delete {0}", ServiceName));
                    p.StandardInput.WriteLine("exit");
                    p.WaitForExit();
                    p.Close();
                    p.Dispose();

                    //Process pr = new Process();
                    //pr.StartInfo.FileName = "sc";
                    //pr.StartInfo.Arguments = string.Format(" delete {0}", ServiceName);
                    //pr.Start();
                    if (ServiceConfigurator.IsServiceExisted(ServiceName) == false)
                    {
                        Console.WriteLine("执行sc删除服务命令完成");
                    }
                    else
                    {
                        Console.WriteLine("执行sc删除服务命令失败");
                    }
                    goto StartLocation;
                }
                #endregion
                #region 按键8的调试服务逻辑代码
                else if (key == ConsoleKey.NumPad8 || key == ConsoleKey.D8)
                {
                    LogServer server = new LogServer();
                    server.Start();
                    goto StartLocation;
                }
                #endregion
                #region 其他无效的按键输入
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("无效的输入");
                    Console.ResetColor();
                    goto StartLocation;
                }
                #endregion

            }
            #endregion
        }
    }
}
