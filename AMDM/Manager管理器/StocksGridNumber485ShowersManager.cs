using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

/*
 * 所有药仓的485显示屏的管理器 2021年11月15日09:05:01 
 * 目前问题是,多个药仓的时候,如果都用485 电脑的口不够用,而且药仓之间中继的线会很多,待确认方案
 */
namespace AMDM.Manager
{
    public class StocksGridNumber485ShowersManager :IDisposable
    {
        SerialPort serialPort;
        public StocksGridNumber485ShowersManager()
        {
        }
        public bool Init(string portName, int baudRate, Parity parity, int dataBits)
        {
            this.serialPort = new SerialPort(portName, baudRate, parity, dataBits);
            //this.serialPort.DataReceived += serialPort_DataReceived;
            return true;
        }

        //void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //}
        public bool Open()
        {
            try
            {
                this.serialPort.Open();
                return true;
            }
            catch (Exception err)
            {
                throw err;
            }
        }
        /// <summary>
        /// 串口打开是否正常
        /// </summary>
        public bool IsOpen { get { if (this.serialPort == null) return false; return this.serialPort.IsOpen; } set{} }
        public bool Close()
        {
            try
            {
                this.serialPort.Close();
                return true;
            }
            catch (Exception closeErr)
            {
                Console.WriteLine(closeErr.Message);
                return false;
            }
        }
        /// <summary>
        /// 在指定的药仓上显示数字(使用三位数数字显示屏)
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public bool ShowGridNumber(int stockIndex, int number)
        {
            string numberStr = string.Format("{0}", number);
            numberStr = numberStr.PadLeft(3, '0');
            string stockIndexStr = string.Format("{0}", stockIndex+1);
            stockIndexStr = stockIndexStr.PadLeft(3, '0');

            string command = string.Format("${0},{1}#", stockIndexStr, numberStr);
            try
            {
                serialPort.Write(command);
                return true;
            }
            catch (Exception sendErr)
            {
                string msg = string.Format("发送失败!! 发送给药仓{0}的 内容为:{1} \r\n报文全文:\r\n{2} \r\n请检查连接,错误内容:{3}", stockIndexStr, numberStr, command,sendErr.Message);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ResetColor();
                return false;
            }
        }
        /// <summary>
        /// 清空显示药槽编号
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <returns></returns>
        public bool ClearGridNumber(int stockIndex)
        {
            string stockIndexStr = string.Format("{0}", stockIndex + 1);
            string command = string.Format("${0},#", stockIndexStr);
            try
            {
                serialPort.Write(command);
                return true;
            }
            catch (Exception sendErr)
            {
                string msg = string.Format("发送失败!! 发送给药仓{0}的 内容为:{1} \r\n报文全文:\r\n{2} \r\n请检查连接,错误内容:{3}", stockIndexStr, "清空显示", command, sendErr.Message);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ResetColor();
                return false;
            }
        }
        public void Dispose()
        {
            if (serialPort!= null)
            {
                //serialPort.DataReceived -= serialPort_DataReceived;
                serialPort.Dispose();
            }
        }
    }
}
