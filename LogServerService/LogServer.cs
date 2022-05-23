using EasyNamedPipe;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
/*
 * 2021年12月9日09:24:41  因为程序可能会有未知的原因被关闭了,或者是直接卡死了.所以需要使用日志服务器记录客户端发来的日志文件.
 * 该日志服务器直接安装在客户机,一定要保证该日志服务器的稳定,并且应该做成windows服务,每次启动电脑就启动服务.服务启动后如果没有检测到付药机应用的进程的话就启动.
 * 正常应该每隔多久推送一次日志信息,如果日志信息迟迟没有到来,那就说明客户端可能是掉了或者卡死了.
 * 
 * 在bw_dowork函数中,检测setting设置,如果是debug模式,只有在被vs调试的时候才会进入到正式的逻辑代码.否则一直等待.
 * 
 * 2021年12月10日14:49:41 目前代码告一段落,准备整理和构建后台服务器的逻辑和启动web后台相关的开发.在此期间同步测试付药客户机的取药功能,收集错误信息,准备把具体关闭系统的原因找出来.
 * 使用日志服务器和看门狗不是最终的解决方案,但是看门狗是应该有的.最终的解决方案是保证前端运行程序的稳定.所以需要现在多做测试找出问题根源.
 * 目前可预见的情况可能跟内存占用过高,显卡不能有效的分配视频播放内容.内存占用过高与摄像头方案可能有很大的关系.
 * 
 * 2021年12月10日14:52:48  系统架构设计思维导图 走起
 */
namespace LogServerService
{
    /// <summary>
    /// 带重启客户端功能的日志服务器
    /// </summary>
    public class LogServer
    {
        #region 全局变量
        /// <summary>
        /// 要被启动的目标客户端的当前状态.
        /// </summary>
        ClientAppStatusEnum clientAppStatus = ClientAppStatusEnum.NotFound;
        /// <summary>
        /// 设置文件的路径
        /// </summary>
        string settingFilePath;
        /// <summary>
        /// 全局设置
        /// </summary>
        Setting setting;
        /// <summary>
        /// 后台工作线程,不使用主线程工作,防止阻塞和过高的系统资源占用
        /// </summary>
        BackgroundWorker bw = new BackgroundWorker();

        DateTime lastHeartbeatsTime_ = DateTime.MinValue;
        public DateTime lastHeartbeatsTime
        {
            get
            {
                return lastHeartbeatsTime_;
            }
            set
            {
                lastHeartbeatsTime_ = value;
            }
        }
        MysqlClient sqlClient;
        /// <summary>
        /// 认为等待程序启动以后多久能发过来心跳,如果为-1,就是一直等待
        /// </summary>
        //int waitFirstHeartbeatsTimeout = 10000;

        /// <summary>
        /// 管道消息服务器
        /// </summary>
        PipeServer pipeServer;
        #endregion
        #region 构造函数
        public LogServer()
        {
            string exeFilePath = Assembly.GetEntryAssembly().Location;
            string exeFileDir = System.IO.Path.GetDirectoryName(exeFilePath);
            this.settingFilePath = string.Format("{0}\\setting.xml", exeFileDir);
            string defaultLogFilePath = string.Format("{0}\\defaultLogFile.log", exeFileDir);
            #region 如果设置文件不存在,创建设置并保存
            if (System.IO.File.Exists(this.settingFilePath) == false || LoadSetting() == false)
            {
                setting = new Setting();
                setting.Debugging = true;
                setting.LogFilePath = defaultLogFilePath;
                setting.Mode = LogModeEnum.Log2File;
                setting.LogEachLevelFields = new SerializableDictionary<LogLevel, List<LogFieldEnum>>();
                List<LogFieldEnum> defaultLogFields = new List<LogFieldEnum>(){LogFieldEnum.Time, LogFieldEnum.Message, LogFieldEnum.Params, LogFieldEnum.Title };
                foreach (var item in Enum.GetValues(typeof(LogLevel)))
                {
                    setting.LogEachLevelFields.Add((LogLevel)item, defaultLogFields);
                }
                setting.ClientAppPath = string.Format("{0}\\客户端程序.exe", exeFileDir);
                #region 初始化数据库连接
                setting.MysqlSetting = new Setting.MysqlSettingClass();
                setting.MysqlSetting.Ip = "127.0.0.1";
                setting.MysqlSetting.User = "root";
                setting.MysqlSetting.Pass = "password";
                setting.MysqlSetting.Port = 3306;
                setting.MysqlSetting.Database = "amdm_log_server";
                setting.MysqlSetting.LogTableName = "logrecord";
                #endregion
                bool saveSettingRet = SaveSetting();
                if (saveSettingRet == false)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("!!!未读取到设置,且初始化设置错误,当前运行在默认配置下,如此问题重复出现将始终不能按照您的设置执行程序");
                    Console.ResetColor();
                }
            }
            #endregion
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;

            #region 初始化管道服务器
            this.pipeServer = new PipeServer("LogServerService");
            this.pipeServer.OnClientConnected += pipeServer_OnClientConnected;
            this.pipeServer.OnClientDisconnected += pipeServer_OnClientDisconnected;
            this.pipeServer.OnClientMessage += pipeServer_OnClientMessage;
            this.pipeServer.OnServerClose += pipeServer_OnServerClose;
            #endregion

            #region 初始化mysql服务器
            this.sqlClient = new MysqlClient(setting.MysqlSetting.Ip, setting.MysqlSetting.User, setting.MysqlSetting.Pass, setting.MysqlSetting.Database, setting.MysqlSetting.Port);
            #endregion
        }

        #region 管道消息服务器的事件
        
        void pipeServer_OnServerClose(PipeServer sender, EventArgs e)
        {
            //throw new NotImplementedException();
            string title = "管道消息服务器被关闭,这通常是不应该的";
            string msg = "请检查关闭具体原因.通常该管道应伴随计算机的启动一直存在";
            Log2LogServerLogFile(title, msg);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(title);
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        void pipeServer_OnClientMessage(ServerServicePipe sender, PipeEventArgs e)
        {
            if (e == null || e.Length < 1 || e.Bytes == null || e.Bytes.Length < 1)
            {
                return;
            }
            else
            {
                //有效的数据,转换为心跳包.
                string msg = Encoding.UTF8.GetString(e.Bytes);
                try
                {
                    //PipeMessageBase msgBase = DeserializeFromXmlString<PipeMessageBase>(msg);
                    PipeMessagePackageBase msgBase = JsonConvert.DeserializeObject<PipeMessagePackageBase>(msg);
                    if (msgBase == null)
                    {
                        return;
                    }
                    switch (msgBase.PackageType)
                    {
                        case PipeMessagePackageType.Auth:
                            break;
                        case PipeMessagePackageType.Heartbeats:
                            #region 心跳消息的处理

                            //PipeMessage_Heartbeats heartBeatsMsg = DeserializeFromXmlString<PipeMessage_Heartbeats>(msg);
                            PipeMessage_Heartbeats heartBeatsMsg = JsonConvert.DeserializeObject<PipeMessage_Heartbeats>(msg);
                            if (heartBeatsMsg == null)
                            {
                                return;
                            }
                            else
                            {
                                #region 正常的收到心跳包了.
                                if (this.clientAppStatus != ClientAppStatusEnum.Connected && this.clientAppStatus != ClientAppStatusEnum.Running)
                                {
                                    //当状态不是已经连接的话,接收到的心跳无效.
                                    return;
                                }
                                this.lastHeartbeatsTime = heartBeatsMsg.HeartbeatsTime;
                                //Log2LogServerLogStorage(new LogRecord() { Level = PipeMessage_Log.LogLevel.Info, Message = msg, LevelName = PipeMessage_Log.LogLevel.Info, Params = sender == null ? null : sender.ClientId, Time = DateTime.Now, Title = "客户端心跳" });
                                #endregion
                            }
                            
                            #endregion
                            break;
                        case PipeMessagePackageType.Message:
                            #region 普通消息的处理
                            PipeMessage_Log message = JsonConvert.DeserializeObject<PipeMessage_Log>(msg);
                            if (message == null)
                            {
                                return;
                            }
                            else
                            {
                                #region 正常的消息结构
                                LogLevel level = LogLevel.Info;
                                //string msgParam = null;
                                try
                                {
                                    level = (LogLevel)Enum.Parse(typeof(LogLevel), message.Level.ToString());
                                }
                                catch (Exception parseLogLevelErr)
                                {
                                    throw new NotImplementedException(string.Format("收到客户端消息类型你为message的消息,解析msg的level出错,错误内容:{0}", parseLogLevelErr.Message));
                                    //msgParam = parseLogLevelErr.Message;
                                }
                                
                                Log2LogServerLogStorage(new LogRecord()
                                    {
                                         Level = level, LevelName = level, Message = message.Message, Title = message.Title, Time = DateTime.Now, 
                                         //Params = msgParam
                                    });
                                #endregion
                            }
                            #endregion
                            break;
                        case PipeMessagePackageType.File:
                            break;
                        case PipeMessagePackageType.Object:
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception jsonParseErr)
                {
                    Log2LogServerLogStorage(new LogRecord() { Level = LogLevel.Error, Message = jsonParseErr.Message, LevelName = LogLevel.Error, Params = msg, Time = DateTime.Now, Title = "反序列化客户机发过来的消息失败" });
                }
            }
        }

        void pipeServer_OnClientDisconnected(ServerServicePipe sender, EventArgs e)
        {
            //throw new NotImplementedException();
            this.clientAppStatus = ClientAppStatusEnum.Disconnected;
            Log2LogServerLogStorage(new LogRecord() { Title = "客户端从管道服务器断开", Time = DateTime.Now, Params = null, LevelName = LogLevel.Info, Message = null, Level = LogLevel.Info });
        }

        void pipeServer_OnClientConnected(ServerServicePipe sender, EventArgs e)
        {
            this.clientAppStatus = ClientAppStatusEnum.Connected;
            Log2LogServerLogStorage(new LogRecord() { Title = "客户端已连接管道服务器", Time = DateTime.Now, Params = sender == null ? null : sender.ClientId, LevelName = LogLevel.Info, Message = null, Level = LogLevel.Info });
            //throw new NotImplementedException();
        }
        #endregion


        #endregion
        #region 工作线程
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            string startedMsg =string.Format("\r\n进入到服务bw_DoWork当前时间:{0}\r\n调试模式?:{1}\r\n配置文件路径:{2}\r\n日志文件路径:{3}\r\n",
                DateTime.Now,setting.Debugging, this.settingFilePath, this.setting.LogFilePath);
            Console.WriteLine(startedMsg);
            Console.ResetColor();
            //System.IO.File.AppendAllText(setting.LogFilePath, startedMsg);
            Log2LogServerLogFile("线程已启动", startedMsg);
            while (bw.CancellationPending == false)
            {
                //2021年12月10日10:41:12  调试的时候 只有当调试器介入了的时候,才会正常运行逻辑代码.非调试正常部署服务的时候这个要去掉.
                if (setting.Debugging && System.Diagnostics.Debugger.IsAttached == false)
                {
                    System.Threading.Thread.Sleep(17);
                    continue;
                }
                break;
            }

            #region 正常代码的前置测试代码
            #endregion
            #region 正常代码
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("此消息来自服务执行逻辑{0},即将执行查找客户端操作,路径{1}", DateTime.Now, setting.ClientAppPath);
            Console.ResetColor();
            //System.Threading.Thread.Sleep(5000);
            Log2LogServerLogFile("已进入正常逻辑", string.Format("准备查找客户端文件{0}", setting.ClientAppPath));
            //System.IO.File.AppendAllText(setting.LogFilePath, "进入到服务正常代码" + DateTime.Now.ToString() + "\r\n");
            StartLocation:
            #region 查找客户端文件,如果不存在,一直查找.
            FindClientApp();
            
            if (bw.CancellationPending)
            {
                return;
            }
            this.clientAppStatus = ClientAppStatusEnum.Startting;
            Log2LogServerLogFile("客户端已找到,准备启动", string.Format("客户端程序地址:{0}", setting.ClientAppPath));
            #endregion
            #region 找到程序以后启动app
            //测试  改一下要启动的程序
            //setting.ClientAppPath = @"\\Mac\Home\Documents\Visual Studio 2008\Projects\测试\FakeHISClient\阻塞和假死程序需要被看门狗处理\bin\Debug\阻塞和假死程序需要被看门狗处理.exe";
            StartClientApp(setting.ClientAppPath);
            string appProcessName = System.IO.Path.GetFileNameWithoutExtension(setting.ClientAppPath);
            if (bw.CancellationPending)
            {
                return;
            }
            
            #endregion
            #region 启动app以后检测是否已经启动了进程
            WaitClientApp();
            if (bw.CancellationPending == true)
            {
                return;
            }
            if (this.clientAppStatus < ClientAppStatusEnum.Started)
            {
                goto StartLocation;
            }
            Log2LogServerLogFile("客户端已启动", string.Format("客户端当前状态{0}", this.clientAppStatus.ToString()));
            #endregion
            #region 检查Running,也就是监测系统有没有收到客户机发来的 第一个心跳包
            var sss = this.lastHeartbeatsTime;
            WaitFirstHeartbeats();
            if (bw.CancellationPending)
            {
                return;
            }
            if (this.clientAppStatus < ClientAppStatusEnum.Running)
            {
                //没等到第一个心跳,死种
                //StopAllClientApp(System.IO.Path.GetFileNameWithoutExtension(setting.ClientAppPath), 3000);
                Log2LogServerLogFile("启动了没有心跳能力的客户端", "正在销毁进程重新生产");
                StopAllClientApp(appProcessName, setting.StopOneAppByCloseMainWindowMethodTimeoutMS);
                if (bw.CancellationPending == false)
                {
                    goto StartLocation;
                }
            }
            #endregion
            #region 如果已经有了心跳了 一直等待和监听客户机的心跳 一旦发生中断,重启
            bool isNoHeartbeatsNeedRestart = false;
            while (bw.CancellationPending == false)
            {
                //上一次心跳距离现在已经过去了多少毫秒.
                double pastedTime = (DateTime.Now- lastHeartbeatsTime).TotalMilliseconds;
                if (pastedTime > setting.PerHeartbeatsValidSpanMS)
                {
                    //超过心跳有效时间了,关闭程序并且直接开启新的任务
                    Log2LogServerLogStorage(new LogRecord() {
                        Level = LogLevel.Error,
                        LevelName = LogLevel.Error,
                        Message = string.Format("上次心跳时间:{0},发现需抢救时,已经过{1}毫秒 正在施救(重建进程)", lastHeartbeatsTime, pastedTime),
                        Time = DateTime.Now,
                        Params = null,
                        Title = "客户端心跳超时施救"
                    });
                    isNoHeartbeatsNeedRestart = true;
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(setting.RetryCheckValidHeartbeatsDelayMS);
                }
            }
            if (isNoHeartbeatsNeedRestart)
            {
                StopAllClientApp(appProcessName, setting.StopOneAppByCloseMainWindowMethodTimeoutMS);
                if (bw.CancellationPending == false)
                {
                    goto StartLocation;
                }
            }
            //看上次的心跳是什么时候,如果超时了,关掉,重新开
            #endregion
                        

            #endregion
            //throw new NotImplementedException();
        }
        #endregion
        #region 私有函数
        #region 启动和停止目标应用程序
        void FindClientApp()
        {//不超时的,一直获取客户端文件.除非标记为退出应用.
            //string ret = null;
            while (bw.CancellationPending == false)
            {
                //之前记录的文件的地址,如果发生改变了,记录到日志文件中.
                string oldFile = setting.ClientAppPath;
                //一直找app,如果找不到,休息1秒再加载配置文件,然后再找.
                if (System.IO.File.Exists(setting.ClientAppPath) == false)
                {
                    System.Threading.Thread.Sleep(setting.RetryLoadSettingDelayMS);
                    bool reLoadSettingRet = LoadSetting();
                    if (reLoadSettingRet == false)
                    {
                        Log2LogServerLogFile("重新读取设置文件错误", string.Format("未找到客户端文件,路径:{0},尝试重新读取设置时发生读取设置错误", setting.ClientAppPath));
                    }
                    else//读取设置文件成功
                    {
                        if (string.IsNullOrEmpty(setting.ClientAppPath))
                        {
                            Log2LogServerLogFile("设置文件错误", string.Format("设置文件中的客户端应用程序为空"));
                        }
                        else
                        {
                            if (setting.ClientAppPath.ToLower() != oldFile.ToLower())
                            {
                                bool newFileExist = System.IO.File.Exists(setting.ClientAppPath);
                                //设置文件中的客户端文件有变更
                                Log2LogServerLogFile("获取客户端程序时,检测到位置发生变化", string.Format("原位置:{0} 新位置:{1} 新位置是否有效?:{2}", oldFile, setting.ClientAppPath, newFileExist));
                                if (newFileExist)
                                {
                                    //ret = setting.ClientAppPath;
                                    break;
                                }
                                else
                                {
                                    //新设置的文件路径还是不存在.
                                }
                            }
                            else
                            {
                                //还是原来那个文件,不用记录log
                            }
                        }
                    }
                }
                else
                {
                    //ret = setting.ClientAppPath;
                    break;
                }
            }
            //return ret;
        }

        bool Log2LogServerLogFile(string title, string msg)
        {
            return Log2LogServerLogFile(string.Format("时间:{0} 信息:{1} 详细信息:{2}\r\n", DateTime.Now, title, msg == null? null: msg.TrimEnd('\0')));
        }
        /// <summary>
        /// 记录日志文件到设定的默认日志文件中,如果日志文件不存在,将存储在c盘根目录下.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool Log2LogServerLogFile(string logLine)
        {
            try
            {
                logLine = logLine.TrimEnd('\0');
                logLine = string.Format("{0}\r\n", logLine);
                string defaultLogFilePath = System.IO.Path.GetDirectoryName(setting.LogFilePath);
                if (System.IO.Directory.Exists(defaultLogFilePath) == false)
                {
                    System.IO.Directory.CreateDirectory(defaultLogFilePath);
                }
            }
            catch (Exception err)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("设置中的日志文件所在的文件夹不存在或无效,尝试创建文件夹时发生错误:{0}", err.Message);
                setting.LogFilePath = "C:\\LogServerServiceDefaultLogFile.log";
                Console.WriteLine("将使用默认存储在{0}的文件作为日志服务器的默认基础信息记录文件", setting.LogFilePath);
                Console.ResetColor();
            }
            try
            {
                System.IO.File.AppendAllText(setting.LogFilePath, logLine);
                return true;
            }
            catch (Exception err)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("记录消息到日志文件发生错误:{0} 待记录信息:{1}", err.Message, logLine);
                Console.ResetColor();
            }
            return false;
        }
        /// <summary>
        /// 将数据存储到设置中的目标存储位置.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool Log2LogServerLogStorage(LogRecord record)
        {
            if (record == null)
            {
                return false;
            }
            if (record.Message != null)
            {
                record.Message = record.Message.TrimEnd('\0');
            }
            if (setting.Mode == LogModeEnum.Log2File)
            {
                string log = buildLogLine(record);
                Log2LogServerLogFile(log);
            }
            else if (setting.Mode == LogModeEnum.Log2Mysql)
            {
                try
                {
                    //保存记录到数据库服务器
                    SqlInsertRecordParamsV2<LogRecord> pr = new SqlInsertRecordParamsV2<LogRecord>(setting.MysqlSetting.LogTableName, record, "*", "id", "level", "levelname");
                    this.sqlClient.InsertDataV2<LogRecord>(pr);
                }
                catch (Exception log2SqlErr)
                {
                    //如果保存到数据库发生错误了以后 记录日志到文件
                    Log2LogServerLogFile("mysql记录日志数据错误", string.Format("错误信息:{0}", log2SqlErr.Message));
                    Log2LogServerLogFile(buildLogLine(record));
                }
            }
            return false;
        }
        string buildLogLine(LogRecord record)
        {
            string log = string.Format("时间:{0} 标题:{1} 消息:{2} 级别{3} 参数:{4}", record.Time, record.Title, record.Message, record.LevelName.ToString(), record.Params);
            return log;
        }
        void StartClientApp(string exeFile)
        {
            //是哟线程或者是bat文件启动客户端程序
            //Process pr = new Process();
            //pr.StartInfo = new ProcessStartInfo();
            //pr.StartInfo.FileName = setting.ClientAppPath;
            //pr.Start();
            //Process.Start(exeFile);
            ServiceSessionUtility.CreateProcess(exeFile, null, 1);//string path=@"C:\Users\Administrator\Test.exe";

            //ServiceSessionUtility.CreateProcess
        }
        void WaitClientApp()
        {
            DateTime start = DateTime.Now;
            string appName = System.IO.Path.GetFileNameWithoutExtension(setting.ClientAppPath);
            while (bw.CancellationPending == false)
            {
                if ((DateTime.Now-start).TotalMilliseconds > setting.WaitClientStartedTimeout)
                {
                    //超时了.
                    break;
                }
                else if (IsExistProcess(appName))
                {
                    this.clientAppStatus = ClientAppStatusEnum.Started;
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(setting.RetryWaitClientDelayMS);
                }
            }
        }
        void WaitFirstHeartbeats()
        {
            //bool timeout = true;
            DateTime start = DateTime.Now;
            while (bw.CancellationPending == false)
            {
                if (setting.WaitFirstHeartbeatsTimeout <= 0)
                {
                    //一直等
                    if (this.lastHeartbeatsTime == DateTime.MinValue)
                    {
                        System.Threading.Thread.Sleep(setting.RetryWaitFirstHeartbeatsDelayMS);
                    }
                    else
                    {
                        //timeout = false;
                        if (this.clientAppStatus< ClientAppStatusEnum.Running)
                        {
                            this.clientAppStatus = ClientAppStatusEnum.Running;   
                        }
                        break;
                    }
                }
                else
                {
                    //只等待固定的超时时间
                    if ((DateTime.Now - start).TotalMilliseconds > setting.WaitFirstHeartbeatsTimeout)
                    {
                        Log2LogServerLogFile("等待第一次心跳超时", string.Format("启动程序后已经等待客户端发来心跳超过{0}毫秒,但是仍然为等到第一次心跳,正在作废原有进程并重生", setting.WaitFirstHeartbeatsTimeout));
                        //timeout = true;
                        break;
                    }
                    else
                    {
                        if (this.lastHeartbeatsTime == DateTime.MinValue)
                        {
                            System.Threading.Thread.Sleep(setting.RetryWaitFirstHeartbeatsDelayMS);
                        }
                        else
                        {
                            if (this.clientAppStatus < ClientAppStatusEnum.Running)
                            {
                                this.clientAppStatus = ClientAppStatusEnum.Running;
                            }
                            break;
                        }
                    }
                }
            }
            //return timeout;
        }
        void StopAllClientApp(string appName, int closeByCloseMainWindowsMethodTimeout)
        {
            DateTime start = DateTime.Now;
            #region 走使用closemainwindows这种友好的方式关闭
            List<Process> processs = GetProcessByName(appName);
            if (processs.Count == 0)
            {
                //没有获取到 应该是自己已经挂掉了,但是不要return 要设置完了状态为stop并且清空上次的心跳时间才能退出
            }
            else
            {

                foreach (var p in processs)
                {
                    try
                    {
                        StopOneClientApp(p, closeByCloseMainWindowsMethodTimeout);
                    }
                    catch (Exception err)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("停止客户端的时候发生了错误:{0}", err.Message);
                        Console.ResetColor();
                    }
                }
            }
            
            #endregion

            this.clientAppStatus = ClientAppStatusEnum.Stoped;
            this.lastHeartbeatsTime = DateTime.MinValue;
            int currentCount = GetProcessByName(appName).Count;
            Log2LogServerLogFile("关闭所有已存在客户端完成", String.Format("当前存在的该名称进程数量:{0}", currentCount));
        }
        /// <summary>
        /// 关闭一个进程,如果有多个同名的进程的时候需要多次调用.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="closeByCloseMainWindowsMethodTimeout"></param>
        void StopOneClientApp(Process p, int closeByCloseMainWindowsMethodTimeout)
        {
            int oldCount = GetProcessByName(p.ProcessName).Count;
            DateTime start = DateTime.Now;
            p.CloseMainWindow();
            bool timeout = true;
            #region 等待超时,在等待期间一直重新获取进程
            while (
                    bw.CancellationPending == false &&
                    (DateTime.Now - start).TotalMilliseconds < closeByCloseMainWindowsMethodTimeout
                    )
            {

                List<Process> currentList = GetProcessByName(p.ProcessName);
                if (currentList.Count + 1 <= oldCount)
                {
                    timeout = false;
                    break;//已经关闭完了,关闭下一个
                }
                else
                {
                    System.Threading.Thread.Sleep(setting.RetryWaitFriendlyClosedDelayMS);
                }
            }
            #endregion
            #region 超时了,直接给他干掉
            if (timeout)
            {
                Log2LogServerLogFile("强制关闭", "使用友好退出方式退出客户端程序失败,尝试使用强制关闭方式");
                if (p != null)
                {
                    p.Kill();
                }
            }
            
            #endregion
            else
            {

            }
            //再检查一遍
            while (
                        bw.CancellationPending == false)
            {

                List<Process> currentList = GetProcessByName(p.ProcessName);
                if (currentList.Count + 1 <= oldCount)
                {
                    break;//已经关闭完了,关闭下一个
                }
                else
                {
                    System.Threading.Thread.Sleep(setting.RetryWaitProcessClosedDelayMS);
                }
            }
            Log2LogServerLogFile("关闭客户端完成", string.Format("超时?:{0} 执行关闭前该名称线程数:{1}", timeout, oldCount));
        }
        #endregion
        #region 配置相关方法 保存 读取 序列化xml 反序列化xml
        /// <summary>
        /// 保存设置文件
        /// </summary>
        /// <returns></returns>
        public bool SaveSetting()
        {
            if (string.IsNullOrEmpty(this.settingFilePath) == true)
            {
                return false;
            }
            try
            {
                SerializeToXml<Setting>(settingFilePath, setting);
            }
            catch (Exception saveSettingErr)
            {
                Console.WriteLine("保存设置文件错误:{0}", saveSettingErr.Message);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 读取设置文件
        /// </summary>
        /// <returns></returns>
        public bool LoadSetting()
        {
            if (System.IO.File.Exists(settingFilePath) == false)
            {
                return false;
            }
            try
            {
                this.setting = DeserializeFromXml<Setting>(settingFilePath);
                return this.setting != null;
            }
            catch (Exception loadFileErr)
            {
                Console.WriteLine("读取xml设置错误:{0}", loadFileErr.Message);
                return false;
            }
        }
        /// <summary>
        /// XML序列化某一类型到指定的文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        public static void SerializeToXml<T>(string filePath, T obj)
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
                {
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    xs.Serialize(writer, obj);
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 把对象序列化成文本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static string SerializeToXmlString<T>(T obj)
        {
            string ret = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(ms))
                    {
                        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                        xs.Serialize(writer, obj);
                        ret = Encoding.UTF8.GetString(ms.GetBuffer());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("在SerializeToXmlString时发生错误:{0}", ex.Message);
                Console.ResetColor();
            }
            return ret;
        }
        /// <summary>
        /// 从某一XML文件反序列化到某一类型
        /// </summary>
        /// <param name="filePath">待反序列化的XML文件名称</param>
        /// <param name="type">反序列化出的</param>
        /// <returns></returns>
        public static T DeserializeFromXml<T>(string filePath)
        {
            try
            {
                if (!System.IO.File.Exists(filePath))
                    throw new ArgumentNullException(filePath + " not Exists");

                using (System.IO.StreamReader reader = new System.IO.StreamReader(filePath))
                {
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    T ret = (T)xs.Deserialize(reader);
                    return ret;
                }
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
        /// <summary>
        /// 从文本反序列化到对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T DeserializeFromXmlString<T>(string content)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    using( System.IO.StreamReader reader = new System.IO.StreamReader(ms))
                    {
                        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                        T ret = (T)xs.Deserialize(reader);
                        return ret;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("在DeserializeFromXmlString发生错误:{0}", ex.Message);
                Console.ResetColor();
                return default(T);
            }
        }
        #endregion
        #region 检查应用程序是否已经启动
        /// <summary>
        /// 检查应用程序是否已经启动
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        bool IsExistProcess(string processName)
        {
            Process[] MyProcesses = Process.GetProcesses();
            foreach (Process MyProcess in MyProcesses)
            {
                if (MyProcess.ProcessName.ToLower().CompareTo(processName.ToLower()) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取符合该名称的所有进程
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        List<Process> GetProcessByName(string processName)
        {
            List<Process> rets = new List<Process>();
            Process[] MyProcesses = Process.GetProcesses();
            foreach (Process MyProcess in MyProcesses)
            {
                if (MyProcess.ProcessName.ToLower().CompareTo(processName.ToLower()) == 0)
                {
                    rets.Add(MyProcess);
                }
            }
            return rets;
        }
        #endregion
        #endregion
        #region 公共函数
        /// <summary>
        /// 启动服务器
        /// </summary>
        public void Start()
        {
            bw.RunWorkerAsync();
            this.pipeServer.Start();
        }
        /// <summary>
        /// 停止服务器
        /// </summary>
        public void Stop()
        {
            if (bw!=null)
            {
                bw.CancelAsync();
            }
            if (this.pipeServer!= null)
            {
                this.pipeServer.Stop();
            }
        }
        #endregion
        #region 回调函数

        #endregion
    }
}