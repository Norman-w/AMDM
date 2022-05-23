//using Modbus.Device;
using NModbus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FakeHISClient
{
    public partial class ModbusTest : Form
    {
        public ModbusTest()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IModbusSerialMaster connect = this.CreateModBusRtuConnection(new SerialPort("COM1", 9600, Parity.Even, 7, StopBits.One));
            MessageBox.Show("连接完成");
        }
        public IModbusSerialMaster CreateModBusRtuConnection(SerialPort serialPort)
        {
            IModbusSerialMaster master = null;
            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.Open();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            try
            {               
                //master = ModbusSerialMaster.CreateRtu(serialPort);
                ///同时也可以配置master的一些参数
                master.Transport.ReadTimeout = 100;//读取数据超时100ms
                master.Transport.WriteTimeout = 100;//写入数据超时100ms
                master.Transport.Retries = 3;//重试次数
                master.Transport.WaitToRetryMilliseconds = 10;//重试间隔

            }
            catch (Exception e)
            {
                throw e;
            }
            return master;
        }
    }
}
