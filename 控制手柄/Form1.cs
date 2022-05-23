using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NM.WebSocketSharp;
using Newtonsoft.Json;

namespace 控制手柄
{
    public partial class Form1 : Form
    {
        WebSocket wsClient = new WebSocket("ws://localhost:10080/cmd");
        WebSocketAutoReConnector rc;
        public Form1()
        {
            InitializeComponent();
            wsClient.OnClose += wsClient_OnClose;
            wsClient.OnError += wsClient_OnError;
            wsClient.OnMessage += wsClient_OnMessage;
            wsClient.OnOpen += wsClient_OnOpen;
           rc = new WebSocketAutoReConnector(ref wsClient, int.MaxValue, new TimeSpan(0, 0, 3));
            rc.OnConnected += rc_OnConnected;
        }

        delegate void setLogFunction(string msg);
        void SetLog(string msg)
        {
            if (this.logTextBox.InvokeRequired)
            {
                setLogFunction sf = new setLogFunction(SetLog);
                this.logTextBox.BeginInvoke(sf, msg);
                return;
            }
            this.logTextBox.AppendText(string.Format("\r\n时间:{0}\r\n信息:{1}\r\n",
                DateTime.Now.ToString("HH:mm:ss"),
                msg
                ));
        }

        void rc_OnConnected(WebSocket socket)
        {
            SetLog("已连接主机:" + socket.Url);
        }

        delegate void SetConnectBtnFunc(WebSocketState state);
        void setConnectBtn(WebSocketState state)
        {
            if (this.连接按钮.InvokeRequired)
            {
                SetConnectBtnFunc fc = new SetConnectBtnFunc(this.setConnectBtn);
                this.连接按钮.BeginInvoke(fc, state);
                return;
            }
            switch (state)
            {
                case WebSocketState.Connecting:
                     this.连接按钮.Enabled = false;
                     this.断开连接按钮.Enabled = false;
                this.连接按钮.Text = "连接中";
                this.connectStatusLed.ForeColor = Color.Yellow;
                    break;
                case WebSocketState.Open:
                     this.连接按钮.Enabled = false;
                     this.断开连接按钮.Enabled = true;
                this.连接按钮.Text = "已连接";
                this.connectStatusLed.ForeColor = Color.Green;
                    break;
                case WebSocketState.Closing:
                     this.连接按钮.Enabled = false;
                     this.断开连接按钮.Enabled = false;
                this.连接按钮.Text = "关闭中";
                this.connectStatusLed.ForeColor = Color.DarkRed;
                    break;
                case WebSocketState.Closed:
                     this.连接按钮.Enabled = true;
                     this.断开连接按钮.Enabled = false;
                this.连接按钮.Text = "连接";
                this.connectStatusLed.ForeColor = Color.OrangeRed;
                    break;
                default:
                    break;
            }
            
        }

        void wsClient_OnOpen(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            WebSocket ws = sender as WebSocket;
            setConnectBtn(ws.ReadyState);
        }

        void wsClient_OnMessage(object sender, MessageEventArgs e)
        {
            //throw new NotImplementedException();
            LogPacket packet = null;
            try
            {
                packet = Newtonsoft.Json.JsonConvert.DeserializeObject<LogPacket>(e.Data);
            }
            catch (Exception parseErr)
            {
                Console.WriteLine("解析信息错误{0} \r\n{1}", e.Data, parseErr.Message);
                return;
            }
            Utils.log(packet.LogType, packet.Msg,false, packet.Objects);
            SetLog(string.Format("远程Debug消息:{0}  {1}  {2}", packet.LogType.ToString(), packet.Time, packet.Msg));
        }

        void wsClient_OnError(object sender, ErrorEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void wsClient_OnClose(object sender, CloseEventArgs e)
        {
            //throw new NotImplementedException();
            WebSocket ws = sender as WebSocket;
            setConnectBtn(ws.ReadyState);
        }

        private void 模拟扫描747Btn_Click(object sender, EventArgs e)
        {
            DebugCommandOnWS cmd = new DebugCommandOnWS();
            cmd.Command = "simulate scan qrcode";
            cmd.Fields = new Dictionary<string, object>();
            cmd.Fields.Add("code", "747");
            string json = JsonConvert.SerializeObject(cmd);

            wsClient.Send(json);
            SetLog("已发送747条码");
        }

        private void 连接按钮_Click(object sender, EventArgs e)
        {
            //wsClient.Close();
            //wsClient.ConnectAsync();
            if (wsClient.ReadyState == WebSocketState.Open)
            {
                wsClient.Close();
            }
            wsClient.ReConnectToAsync(this.ip地址文本框.Text);
            //wsClient.ReConnectTo(this.ip地址文本框.Text);

            rc.Start();
        }

        private void 模拟扫描地榆升白胶囊Btn_Click(object sender, EventArgs e)
        {
            ///6911502722656
            DebugCommandOnWS cmd = new DebugCommandOnWS();
            cmd.Command = "simulate scan qrcode";
            cmd.Fields = new Dictionary<string, object>();
            cmd.Fields.Add("code", "6911502722656");
            string json = JsonConvert.SerializeObject(cmd);

            wsClient.Send(json);
            SetLog("已发送地榆升白胶囊的条码");
        }

        private void 模拟扫描药品库中的第1个药品_Click(object sender, EventArgs e)
        {
            DebugCommandOnWS cmd = new DebugCommandOnWS();
            cmd.Command = "simulate scan qrcode";
            cmd.Fields = new Dictionary<string, object>();
            cmd.Fields.Add("code", "6902401045076");
            string json = JsonConvert.SerializeObject(cmd);

            wsClient.Send(json);
            SetLog("已发送第一个药品的条码");
        }

        private void 模拟扫描药品库中第100个药品_Click(object sender, EventArgs e)
        {
            DebugCommandOnWS cmd = new DebugCommandOnWS();
            cmd.Command = "simulate scan qrcode";
            cmd.Fields = new Dictionary<string, object>();
            cmd.Fields.Add("code", "6919624168567");
            string json = JsonConvert.SerializeObject(cmd);

            wsClient.Send(json);
            SetLog("已发送第100个药品的条码");
        }

        private void 使用本机地址按钮_Click(object sender, EventArgs e)
        {
            this.ip地址文本框.Text = "ws://127.0.0.1:10080/cmd";
            this.连接按钮_Click(sender, e);
        }

        private void 使用vpn地址按钮_Click(object sender, EventArgs e)
        {
            this.ip地址文本框.Text = "ws://10.10.10.17:10080/cmd";
            this.连接按钮_Click(sender, e);
        }

        private void 使用192段地址按钮_Click(object sender, EventArgs e)
        {
            this.ip地址文本框.Text = "ws://192.168.2.191:10080/cmd";
            this.连接按钮_Click(sender, e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.connectStatusLed.ForeColor = Color.OrangeRed;
            ToolTip tp = new ToolTip();
            tp.ShowAlways = true;
            tp.SetToolTip(this.connectStatusLed, "连接状态指示灯,红:已断开 绿:已连接 黄:连接中");
        }

        private void 断开连接按钮_Click(object sender, EventArgs e)
        {
            this.wsClient.Close();
            this.rc.Stop();
        }

        private void 模拟扫描516Btn_Click(object sender, EventArgs e)
        {
            DebugCommandOnWS cmd = new DebugCommandOnWS();
            cmd.Command = "simulate scan qrcode";
            cmd.Fields = new Dictionary<string, object>();
            cmd.Fields.Add("code", "516");
            string json = JsonConvert.SerializeObject(cmd);

            wsClient.Send(json);
            SetLog("已发送516条码");
        }

        private void 模拟扫描风油精条码Btn_Click(object sender, EventArgs e)
        {
            //6971204040144
            DebugCommandOnWS cmd = new DebugCommandOnWS();
            cmd.Command = "simulate scan qrcode";
            cmd.Fields = new Dictionary<string, object>();
            cmd.Fields.Add("code", "6971204040144");
            string json = JsonConvert.SerializeObject(cmd);

            wsClient.Send(json);
            SetLog("已发送地风油精的条码");
        }

        private void 测试一盒一盒的取药按钮_Click(object sender, EventArgs e)
        {
            DebugCommandOnWS cmd = new DebugCommandOnWS();
            cmd.Command = "simulate medicines getting";
            cmd.Fields = new Dictionary<string, object>();
            cmd.Fields.Add("status", "start");
            string json = JsonConvert.SerializeObject(cmd);
            wsClient.Send(json);
            SetLog("已发送一盒一盒的取药测试启动命令");
        }

        private void simulateScanExitCodeBtn_Click(object sender, EventArgs e)
        {
            //6971204040144
            DebugCommandOnWS cmd = new DebugCommandOnWS();
            cmd.Command = "simulate scan qrcode";
            cmd.Fields = new Dictionary<string, object>();
            cmd.Fields.Add("code", "exit:admin");
            string json = JsonConvert.SerializeObject(cmd);

            wsClient.Send(json);
            SetLog("已发送设备退出码");
        }

        private void 模拟拟扫描8970_4个地榆升按钮_Click(object sender, EventArgs e)
        {
            DebugCommandOnWS cmd = new DebugCommandOnWS();
            cmd.Command = "simulate scan qrcode";
            cmd.Fields = new Dictionary<string, object>();
            cmd.Fields.Add("code", "8970");
            string json = JsonConvert.SerializeObject(cmd);

            wsClient.Send(json);
            SetLog("已发送8970条码");
        }

        private void 模拟扫描8975_Click(object sender, EventArgs e)
        {
            DebugCommandOnWS cmd = new DebugCommandOnWS();
            cmd.Command = "simulate scan qrcode";
            cmd.Fields = new Dictionary<string, object>();
            cmd.Fields.Add("code", "8975");
            string json = JsonConvert.SerializeObject(cmd);

            wsClient.Send(json);
            SetLog("已发送8975条码");
        }
    }
    public class DebugCommandOnWS
    {
        public string Command { get; set; }
        public Dictionary<string, object> Fields { get; set; }
    }
}
