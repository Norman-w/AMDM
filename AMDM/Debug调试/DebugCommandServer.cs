using AMDM_Domain;
using Newtonsoft.Json;
using NM.WebSocketSharp;
using NM.WebSocketSharp.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 2022年1月25日22:37:33  创建这个是为了让界面更简洁,不会出一些没有用的调试代码,外界的调试代码都从这里被接收,然后启动相应的内容.
 * 比如扫描二维码,发送到二维码扫描器,发送一些命令 给amdm相应的模块进行处理.
 * 
*/



namespace AMDM
{
    public class WS : WebSocketBehavior
    { }
    /// <summary>
    /// 调试时命令的接收器,使用ws来接收命令
    /// </summary>
    public class DebugCommandServer
    {
        void logOutput(LogPacket packet)
        {
            if (this.wsServer!= null && this.wsServer.WebSocketServices!= null)
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(packet);
                wsServer.WebSocketServices.Broadcast(json);
            }
        }
        LogOutputFuncDelegate logOutputFunc;
        public DebugSettingClass Setting { get; set; }
        /// <summary>
        /// 要绑定的方法的类型
        /// </summary>
        public enum BindingFuncTypeEnum
        {
            QRCordProcessFunc,
        }
        #region global state
        WebSocketServer wsServer = new WebSocketServer(10080, false);
        #endregion
        public DebugCommandServer()
        {
            this.Setting = new DebugSettingClass();
            App.AutoMedicinesGettingTester = new AutoMedicinesGettingTester();
            logOutputFunc = new LogOutputFuncDelegate(this.logOutput);
            wsServer.AllowForwardedRequest = true;
            wsServer.AddWebSocketService<WS>("/cmd", onInitWebsocketService);
            wsServer.Start();
            //this.ProcessQRCodeMessageFunctions = new List<Action<string>>();
        }
        public bool DebuggerConnected { get; set; }
        void onInitWebsocketService(WebSocketBehavior newServerObject)
        {
            newServerObject.OnOpen = OnOpen;
            newServerObject.OnMessage = OnMessage;
            newServerObject.OnError = OnError;
            newServerObject.OnClose = OnClose;
        }
        void Speak(string msg)
        {
            if (App.TTSSpeaker != null)
            {
                try
                {
                    App.TTSSpeaker.Speak(msg);
                }
                catch (Exception err)
                {
                    Utils.LogError("在调试控制面板服务器中语音播放错误", err.Message);
                }
            }
        }
        void OnOpen(object s, EventArgs e)
        {
            App.DebugOutputFunctions.Add(logOutputFunc);
            DebuggerConnected = true;
            this.Setting.IgnoreDeliveriedChecking = true;
            this.Setting.IgnoreMedicinesBulketCoverChecking = true;
            this.Setting.MedicineGettingTestPerGridGrubTimes = 1;
            this.Setting.SkipDeliveryRecordPaperRealPrinting = true;
            Speak("调试器控制面板已插入");
        }
        void OnMessage(object s, MessageEventArgs e)
        {
            try
            {
                DebugCommandOnWS cmd = JsonConvert.DeserializeObject<DebugCommandOnWS>(e.Data);
                switch (cmd.Command.ToLower())
                {
                    case "simulate scan qrcode":
                        if (cmd.Fields.ContainsKey("code"))
                        {
                            //foreach (var item in this.ProcessQRCodeMessageFunctions)
                            //{
                            //    if (item != null && cmd.Fields != null)
                            //    {
                            //        item(string.Format("{0}", cmd.Fields["code"]));
                            //    }
                            //}
                            if (App.ICCardReaderAndCodeScanner2in1ReceivedData!= null)
                            {
                                App.ICCardReaderAndCodeScanner2in1ReceivedData(string.Format("{0}", cmd.Fields["code"]));
                            }
                        }
                        break;
                    case "simulate medicines getting":
                        if (true)
                        {
                            if (cmd.Fields.ContainsKey("status"))
                            {
                                if ((string)cmd.Fields["status"] == "start")
                                {
                                    if (App.AutoMedicinesGettingTester != null && App.ICCardReaderAndCodeScanner2in1ReceivedData != null)
                                    {
                                        string newOrder = App.AutoMedicinesGettingTester.CreateNewOrder();
                                        App.AutoMedicinesGettingTester.Working = true;
                                        App.ICCardReaderAndCodeScanner2in1ReceivedData(newOrder);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

            }
        }
        void OnError(object s, EventArgs e)
        {
            Speak("调试器控制面板发生错误");
        }
        void OnClose(object s, EventArgs e)
        {
            App.DebugOutputFunctions.Remove(logOutputFunc);
            DebuggerConnected = false;
            Speak("调试器控制面板已拔出");
        }
        #region 私有
        /// <summary>
        /// 处理扫描条码的方法合集
        /// </summary>
        //public List<Action<string>> ProcessQRCodeMessageFunctions;
        #endregion
    }
    public class DebugCommandOnWS
    {
        public string Command { get; set; }
        public Dictionary<string,object> Fields { get; set; }
    }
}
