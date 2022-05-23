using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
/// <summary>
    /// 服务配置器
    /// </summary>
public class ServiceConfigurator
{
    #region 检查服务是否存在

    /// <summary>
    /// 检查服务是否存在
    /// </summary>
    /// <param name="serviceName"></param>
    /// <returns></returns>
    public static bool IsServiceExisted(string serviceName)
    {
        ServiceController[] services = ServiceController.GetServices();
        foreach (ServiceController s in services)
        {
            if (s.ServiceName == serviceName)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region 启动和停止服务

    /// <summary>
    /// 启动服务
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <param name="checkStartResultTimes">服务启动命令执行后,执行多少次检查服务启动状态</param>
    /// <param name="perTimeCheckStartResultDelay">服务启动命令执行后,每隔多久检测一次服务是否在运行的状态</param>
    public static void StartService(string serviceName, int checkStartResultTimes = 100, int perTimeCheckStartResultDelay = 100)
    {
        if (IsServiceExisted(serviceName))
        {
            System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName);
            if (service.Status != System.ServiceProcess.ServiceControllerStatus.Running &&
                service.Status != System.ServiceProcess.ServiceControllerStatus.StartPending)
            {
                service.Start();
                for (int i = 0; i < checkStartResultTimes; i++)
                {
                    service.Refresh();
                    System.Threading.Thread.Sleep(perTimeCheckStartResultDelay);
                    if (service.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("----服务启动成功----");
                        Console.ResetColor();
                        break;
                    }
                    if (i == checkStartResultTimes - 1)
                    {
                        throw new Exception("启动服务发生错误,服务名:" + serviceName);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 停止服务
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    public static void StopService(string serviceName)
    {
        if (IsServiceExisted(serviceName))
        {
            System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName);
            if (service.Status != System.ServiceProcess.ServiceControllerStatus.Stopped &&
                service.Status != System.ServiceProcess.ServiceControllerStatus.StopPending)
            {
                service.Stop();
                for (int i = 0; i < 60; i++)
                {
                    service.Refresh();
                    System.Threading.Thread.Sleep(1000);
                    if (service.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
                    {
                        Console.WriteLine("服务已停止");
                        break;
                    }
                    if (i == 59)
                    {
                        throw new Exception("停止服务发生错误,服务名:" + serviceName);
                    }
                }
            }
        }
    }
    #endregion

    #region 获取服务状态

    /// <summary>
    /// 获取服务状态
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <returns></returns>
    public static ServiceControllerStatus GetServiceStatus(string serviceName)
    {
        System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName);
        return service.Status;
    }
    #endregion

    #region 安装和卸载服务

    #region 获取所有的引用的文件
    /// <summary>
    /// 获取当前应用程序引用的所有的dll文件,以便能复制到服务安装目录下
    /// </summary>
    /// <returns></returns>
    static List<string> GetImportedDllFilds()
    {
        Assembly asm = Assembly.GetExecutingAssembly();
        AssemblyName[] names = asm.GetReferencedAssemblies();
        List<string> files = new List<string>();
        for (int i = 0; i < names.Length; i++)
        {
            AssemblyName currentName = names[i];
            Assembly currentAsm = Assembly.Load(currentName);
            string currentFileName = currentAsm.Location;
            files.Add(currentFileName);
        }
        return files;
    }
    #endregion
    /// <summary>
    /// 配置服务(安装或者卸载)
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <param name="serviceSrcDir">要把服务的应用程序安装到哪个文件夹下面,以后启动服务的时候从那里直接启动</param>
    /// <param name="displayName">服务在服务控制台中显示的名字,可以直接使用服务名称也可以指定其他的名称如中文名</param>
    /// <param name="desc">服务详情的描述</param>
    public static void InstallService(string serviceName, string serviceSrcDir, string displayName, string desc)
    {
        TransactedInstaller installer = BuidInstaller(serviceName, displayName, desc);
        //服务的真正要放置的目录.
        string installService2FilePath = null;
        try
        {
            #region 复制文件到目标位置然后再进行安装
            string debuggingFilePath = Assembly.GetEntryAssembly().Location;
            string debuggingFileName = System.IO.Path.GetFileName(debuggingFilePath);
            string debuggingFileDir = System.IO.Path.GetDirectoryName(debuggingFilePath);
            if (serviceSrcDir.EndsWith("\\"))
            {
                serviceSrcDir = System.IO.Path.GetDirectoryName(serviceSrcDir);
            }
            #region 没有目标文件夹自动创建
            if (System.IO.Directory.Exists(serviceSrcDir) == false)
            {
                System.IO.Directory.CreateDirectory(serviceSrcDir);
            }
            #endregion
            installService2FilePath = string.Format("{0}\\{1}", serviceSrcDir, debuggingFileName);
            System.IO.File.Copy(debuggingFileName, installService2FilePath, true);

            #region 拷贝所有的引用文件
            List<string> dllFiles = GetImportedDllFilds();
            foreach (var file in dllFiles)
            {
                if (file.ToLower().Contains("c:\\windows\\") == true)
                {
                    //windows目录下的引用文件不需要拷贝.
                    continue;
                }
                string fileName = System.IO.Path.GetFileName(file);
                string newDestFileName = string.Format("{0}\\{1}", serviceSrcDir, fileName);
                System.IO.File.Copy(file, newDestFileName, true);
            }
            #endregion
            #endregion
        }
        catch (Exception copyFilesErr)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("复制文件到目标文件夹时发生错误:{0}", copyFilesErr.Message);
            Console.ResetColor();
        }
        
        installer.Context.Parameters["assemblypath"] = installService2FilePath;
        //ti.Context.Parameters["assemblypath"] = Assembly.GetEntryAssembly().Location;
        installer.Install(new Hashtable());
        Console.WriteLine("安装命令启动完成");
    }
    /// <summary>
    /// 卸载服务
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    public static void UnInstallService(string serviceName)
    {
        TransactedInstaller installer = BuidInstaller(serviceName);
        installer.Uninstall(null);
        Console.WriteLine("卸载命令启动完成");
    }
    #endregion

    #region 安装和卸载服务时,使用的安装器对象构建

    /// <summary>
    /// 构建安装器,在卸载时,直接指定第一参数即可
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <param name="displayName">服务在服务管理器中的显示名称</param>
    /// <param name="desc">服务相关描述</param>
    static TransactedInstaller BuidInstaller(string serviceName, string displayName = null, string desc = null)
    {
        TransactedInstaller ti = new TransactedInstaller();
        ti.Installers.Add(new ServiceProcessInstaller
        {
            Account = ServiceAccount.LocalSystem
        });
        ti.Installers.Add(new ServiceInstaller
        {
            DisplayName = (displayName == null ? serviceName : displayName),
            ServiceName = serviceName,
            Description = desc == null ? "安装服务时未指定服务描述" : desc,
            //ServicesDependedOn = new string[] { "Netlogon" },//此服务依赖的其他服务
            StartType = ServiceStartMode.Automatic//运行方式
        });
        ti.Context = new InstallContext();
        return ti;
    }
    #endregion
}