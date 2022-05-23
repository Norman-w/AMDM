using System;
using System.Diagnostics;
using System.IO.Ports;
using NModbus.IO;

namespace NModbus.Serial
{
    /// <summary>
    ///     Concrete Implementor - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    public class SerialPortAdapter : IStreamResource
    {
        private const string NewLine = "\r\n";
        private SerialPort _serialPort;

        public SerialPortAdapter(SerialPort serialPort)
        {
            Debug.Assert(serialPort != null, "Argument serialPort cannot be null.");

            _serialPort = serialPort;
            _serialPort.NewLine = NewLine;
            _serialPort.DataReceived += _serialPort_DataReceived;
        }

        void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public int InfiniteTimeout
        {
            get { return SerialPort.InfiniteTimeout; }
        }

        public int ReadTimeout
        {
            get { return _serialPort.ReadTimeout; }
            set { _serialPort.ReadTimeout = value; }
        }

        public int WriteTimeout
        {
            get { return _serialPort.WriteTimeout; }
            set { _serialPort.WriteTimeout = value; }
        }

        public void DiscardInBuffer()
        {
            try
            {
                _serialPort.DiscardInBuffer();
            }
            catch (Exception err)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("在执行SerialPortAdapter的DiscardInBuffer函数中发生错误:{0}", err.Message);
                return ;
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                return _serialPort.Read(buffer, offset, count);
            }
            catch (Exception err)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("在执行SerialPortAdapter的Read函数中发生错误:{0}",err.Message);
                return 1;
            }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            try
            {
                _serialPort.Write(buffer, offset, count);
            }
            catch (Exception err)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("在执行SerialPortAdapter的Write函数中发生错误:{0}", err.Message);
                return;
            }
            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_serialPort != null)
	{
                    _serialPort.Dispose();
	}
                _serialPort = null;
            }
        }
    }
}
