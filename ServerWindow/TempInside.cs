using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Threading;

namespace ServerWindow
{
    class TempInside : Program
    {
        public static SerialPort serialPort = new SerialPort();

        public int TempInsideValue = 0;
        public void  Connect()
        {
            try
            {
                List<string> names = ComPortNames("1A86", "7523");
                foreach (String s in SerialPort.GetPortNames())
                {
                    if (names.Contains(s))
                    {
                        serialPort.PortName = s;
                        serialPort.BaudRate = 9600;
                        serialPort.Parity = Parity.None;
                        serialPort.StopBits = StopBits.One;
                        serialPort.DataBits = 8;
                        serialPort.ReadBufferSize = 64;
                        serialPort.WriteTimeout = 100;
                        serialPort.ReadTimeout = 100;
                        serialPort.Open();
                        serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Не удалось установить соединение с датчиком температуры");
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
            var message = sp.ReadLine();
            // var resultBuffer = Encoding.ASCII.GetBytes(message + "\r\n");
            //Console.WriteLine(message);
                message = message.Substring(31);
                message = message.Substring(0, message.Length - 8);
                TempInsideValue = Convert.ToInt32(message);
            }
            catch
            {

            }
        }

        List<string> ComPortNames(String VID, String PID)
        {
            String pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> comports = new List<string>();
            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
            foreach (String s3 in rk2.GetSubKeyNames())
            {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (String s in rk3.GetSubKeyNames())
                {
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (String s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            comports.Add((string)rk6.GetValue("PortName"));
                        }
                    }
                }
            }
            return comports;
        }
    }
}