using System;
using System.Reflection;
using System.Windows.Forms;
using AutoUpdate;
using   System.Diagnostics;
using System.Collections.Generic;
using System.Security.Cryptography;

/*
 * 明天要使用libvlcsharp的winform以及字幕显示的3.6.1版本 摄像头考虑也使用他 但是好像失败了  不行就用dshow试试  还是算了 节省时间
 * 明天再完善交互 取药中页面等
 */

namespace FakeHISClient
{
    static class Program
    {
        static Program()
        {
            var sss =Assembly.GetExecutingAssembly().GetReferencedAssemblies()[9];
            var ssw = Assembly.GetAssembly(sss.GetType());
            var ss = AppDomain.CurrentDomain.GetAssemblies();
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            //var edm = 27;
            //var cosValue = edm * Math.PI / 180;
            //var a = Math.Cos(27*Math.PI/180) * 80;


            Console.WriteLine("检查是否需要更新");
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
            //Application.Run(new FormDirect());
            //Application.Run(new TestWindowLessCameraCaputer());

            #region 485 MODBUS通讯测试
            //苹果搭配PARALLELS DESKTOP,WIN7系统不支持CH340这样的芯片,usb驱动不起来 Monterey 不知道是不是PD破戒版本的问题还是苹果系统的问题或者USB串口转换线设备的问题
            /*
             * 但是在调试的时候又不能一直开着win7系统,这样的话 webstorm就不能在苹果上用了.而且要把测试机上接好长的线接到PLC那边.
             * 但是PLC那边的设备有网络,MAC下PD里的win7也有网络.所以就用网络和开发机进行连接和通讯.
             * 试过下面几个方法,最终使用NMODBUS里面的网络接口方式.但是要注意的是,初始化socket的时候 需要指定socket的一些参数 比如要使用stream方式连接,并且建立连接方式是tcp.不能使用默认参数
             * 
             */
            #region 可以用,可再构造能力强,但是操作特别复杂的方式,需要自己手写TCP服务器相关消息处理和MODBUS协议 所以此方法放弃.虽然能跑的通
            //可以借助这个方式给串口发送消息,然后监听串口或者PLC,也可以像现在这样 2022年3月3日14:10:44 485-TTL-网口  监听网口消息的方式来进行调试时候使用.平时不建议使用
            //System.Net.Sockets.TcpClient tcpClient = new System.Net.Sockets.TcpClient();
            //tcpClient.Connect("192.168.2.192", 20108);
            //List<byte> buffer = new List<byte>();
            //buffer.Add(Byte.Parse("64", System.Globalization.NumberStyles.HexNumber));
            //buffer.Add(Byte.Parse("03", System.Globalization.NumberStyles.HexNumber));
            //buffer.Add(Byte.Parse("10", System.Globalization.NumberStyles.HexNumber));
            //buffer.Add(Byte.Parse("64", System.Globalization.NumberStyles.HexNumber));
            //buffer.Add(Byte.Parse("00", System.Globalization.NumberStyles.HexNumber));
            //buffer.Add(Byte.Parse("0A", System.Globalization.NumberStyles.HexNumber));
            //buffer.Add(Byte.Parse("89", System.Globalization.NumberStyles.HexNumber));
            //buffer.Add(Byte.Parse("27", System.Globalization.NumberStyles.HexNumber));

            //tcpClient.GetStream().Write(buffer.ToArray(), 0, buffer.Count);
            ////System.Threading.Thread.Sleep(1000);
            //byte[] readBuffer =  new byte[1024];
            //tcpClient.GetStream().Read(readBuffer, 0, 100);
            #endregion



            #region 不可以用的方式,使用EasyModbus的IP地址和端口号进行连接的方式 由于EasyModbus连接的时候不可以指定串口的RTS使能,也不能使用自定义的网口参数进行连接 所以此种方式 废废
            //EasyModbus.ModbusClient client = new EasyModbus.ModbusClient("192.168.2.192", 20108);
            //client.Connect();
            //System.Threading.Thread.Sleep(1000);
            //var ret = client.ReadHoldingRegisters(4196, 100);
            #endregion

            #region 可以用的NModbus方式,使用TCP转串口的方式连接   硬件连接方式: PLC->485转TTL->USR232-T(TTL转RS485网口服务器)->交换机->路由器
            //System.Net.Sockets.Socket socket = new System.Net.Sockets.Socket(System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            //socket.Connect("192.168.2.192", 20108);


            //NModbus.ModbusFactory fc = new NModbus.ModbusFactory();
            //var master = fc.CreateMaster(socket);
            //var red = master.ReadHoldingRegisters(100, 4196, 100);
            #endregion

            #endregion

            #region 主程序
            
            Console.WriteLine("初始化主程序");
            MainForm mform = new MainForm();
            mform.Init();
            Console.WriteLine("初始化完成");
            Application.Run(mform);
            #endregion

            #region 自动布局器测试
            //var aform = new GridAutoLayouter();
            //Application.Run(aform);
            #endregion

            
            //Application.Run(new DirectShow.FormMain());
            ////Application.Run(new VlcPlayer.VlcPlayerTest());
            //Stock stockForm = new Stock();
            //stockForm.Init(0);
            //Application.Run(stockForm);

            #region 2021年12月7日14:24:54   modbus测试
            //ModbusTest testForm = new ModbusTest();
            //Application.Run(testForm);
            #endregion
        }


        
        //static S7Client client = new S7Client();
        //static void Main(string[] args)
        //{
        //    var res = client.ConnectTo("192.168.1.75", 0, 1);//连接PLC的IP地址
        //    if (res == 0)
        //    {
        //        while (true)
        //        {
        //            byte[] db3Buffer = new byte[4];
        //            var result = client.DBRead(1, 2, db3Buffer.Length, db3Buffer);//读取DB127.DBD178位置数据到db3Buffer当中
        //            if (result == 0)//判断操作成功标识，成功0，否则不成功
        //            {
        //                double db3dbd4 = S7.GetRealAt(db3Buffer, 0);//将获取的数据进行转换成Real类型，需要确定PLC当中此数据为Real类型方可
        //                Console.WriteLine("DB127.DBD178: " + db3dbd4);
        //            }


        //            db3Buffer = new byte[4];

        //            db3Buffer.SetRealAt(0, 345f);//将 345 数据存入buffer当中后续写入到PLC
        //            //此方法为扩展方法，也可以写成
        //            //S7.SetRealAt(db3Buffer, 0, 345);
        //            result = client.DBWrite(1, 2, db3Buffer.Length, db3Buffer);//将buffer数据写入PLC位置DB127.DBD54，类型为Real
        //            if (result != 0)
        //            {
        //                Console.WriteLine("Error: " + client.ErrorText(result));
        //            }

        //            Thread.Sleep(100);
        //        }
        //    }
        //    Console.ReadKey();
        //}
    }
}
