using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
/*
 * 2021年12月10日10:16:43 完成了服务的构建和自动安装.
 * 该LogServerService方案中,定义项目为控制台应用程序,如果使用vs进行调试的时候,需要增加随意一个调试参数可启动服务部署工具(在项目属性->调试->启动选项->命令行参数中设置)
 * 免去了使用命令行安装的麻烦.后续可以考虑具体使用哪个命令能启动调试.
 * 本类中的代码或者LogServer(正式服务载体)如果改动,直接vs调试然后安装即可.如果要调试本类中的代码,使用vs的调试菜单中的附加到进程,附加到进程的时候需要勾选 显示所有用户的进程 能看到为 LogServerService.exe的进程 可以启动调试
 * 调试时,OnStart的代码并不能捕获到,所以在Onstart中启动LogServer,LogServer会启动backgroundworker,在dowork函数中执行具体的逻辑.
 * dowork工作时,如果Setting.xml中设置的是Debugging模式,只有在等待vs调试附加到该进程后才会走正式工作代码(因为服务的onstart根本捕获不到,所以用此法等待调试器调试它).
 * 如果调试都完成,Setting.xml中的debugging要设置为false,否则服务不会正常工作一直等待调试.
 */
namespace LogServerService
{
    partial class LogServerService : ServiceBase
    {
        LogServer server;
        public LogServerService()
        {
            InitializeComponent();            
        }

        protected override void OnStart(string[] args)
        {
            server = new LogServer();
            server.Start();
        }

        protected override void OnStop()
        {
            server.Stop();
        }
    }
}
