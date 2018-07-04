/*
 * 
 * Serial port class
 * Copyright 2013
 * 
 */

using System;
using System.Threading;
using System.Diagnostics;
using System.IO.Ports;
using System.IO;

namespace PendulumApp.DeviceModel
{
    #region enum
    /// <summary>
    /// COM port parity enum
    /// </summary>
    public enum Parity { NONE = 0, ODD = 1, EVEN = 2, MARK = 3, SPACE = 4 }
    /// <summary>
    /// COM port stop bits enum
    /// </summary>
    public enum StopBits { FIRST = -1, ONE, ONE_AND_HALF, TWO }
    /// <summary>
    /// COM port data bits enum
    /// </summary>
    public enum DataBits { FIRST = -1, EIGHT, SEVEN, SIX }
    /// <summary>
    /// COM port handshake enum
    /// </summary>
    public enum HandshakeBits { FIRST = -1, None, RTS, XOnXOff }

    #endregion

    /// <summary>
    /// Serial port communication class
    /// </summary>
    public class MSerialPort
    {
        #region Variable

        delegate void CommPortCallback(byte[] buffer, int num_toread);
        delegate void CommPortLineCallback(string msg, int length);
        delegate void DebugCallbackFunction(string name);
        //Device device;
        SerialPort commPort;
        bool isOpen = false;
        public bool debug = false;
        Mutex comMutex = new Mutex();

        #endregion

        #region Constructor

        /// <summary>
        /// Serial port constructor
        /// </summary>
        /// <param name="main_screen_ptr"></param>
        public MSerialPort()
        {
            try
            {
                commPort = new SerialPort();
                commPort.Encoding = System.Text.Encoding.ASCII;
                commPort.ErrorReceived += new SerialErrorReceivedEventHandler(this.serialPort_ErrorReceived);
                //commPort.DataReceived += new SerialDataReceivedEventHandler(this.serialPort_DataReceived);
                //commPort.PinChanged += new SerialPinChangedEventHandler(this.serialPort_PinChanged);
                commPort.PortName = "COM1";
                commPort.Parity = System.IO.Ports.Parity.None;
                commPort.StopBits = System.IO.Ports.StopBits.One;
                commPort.DataBits = 8;
                commPort.BaudRate = 19200;
                commPort.ReadBufferSize = 25000;
                commPort.ReadTimeout = 500;
                commPort.WriteTimeout = 500;
                commPort.NewLine = "-";
                commPort.ReceivedBytesThreshold = 1;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());

          /*      if (debug && !main_screen.application_closing)
                    main_screen.Invoke(new DebugCallbackFunction(main_screen.DebugCallback),
                        "Serial Port creation error!" + ex.ToString());*/
            }
        }

        #endregion

        #region public functions

        /// <summary>
        /// Change COM port parameters
        /// </summary>
        /// <param name="port_name"></param>
        /// <param name="baudrate"></param>
        /// <param name="p"></param>
        /// <param name="data"></param>
        /// <param name="stop"></param>
        /// <param name="h"></param>
        public  void setCommParms(string port_name, int baudrate, Parity p, DataBits data, StopBits stop, HandshakeBits h)
        {
            try
            {
                if (commPort.IsOpen)
                    return;

                commPort.PortName = port_name;
                commPort.BaudRate = baudrate;

                switch (p)
                {
                    case Parity.NONE:
                        commPort.Parity = System.IO.Ports.Parity.None;
                        break;
                    case Parity.ODD:
                        commPort.Parity = System.IO.Ports.Parity.Odd;
                        break;
                    case Parity.EVEN:
                        commPort.Parity = System.IO.Ports.Parity.Even;
                        break;
                    case Parity.MARK:
                        commPort.Parity = System.IO.Ports.Parity.Mark;
                        break;
                    case Parity.SPACE:
                        commPort.Parity = System.IO.Ports.Parity.Space;
                        break;
                    default:
                        commPort.Parity = System.IO.Ports.Parity.None;
                        break;
                }

                switch (stop)
                {
                    case StopBits.ONE:
                        commPort.StopBits = System.IO.Ports.StopBits.One;
                        break;
                    case StopBits.ONE_AND_HALF:
                        commPort.StopBits = System.IO.Ports.StopBits.OnePointFive;
                        break;
                    case StopBits.TWO:
                        commPort.StopBits = System.IO.Ports.StopBits.Two;
                        break;
                    default:
                        commPort.StopBits = System.IO.Ports.StopBits.One;
                        break;
                }

                switch (data)
                {
                    case DataBits.EIGHT:
                        commPort.DataBits = 8;
                        break;
                    case DataBits.SEVEN:
                        commPort.DataBits = 7;
                        break;
                    case DataBits.SIX:
                        commPort.DataBits = 6;
                        break;
                    default:
                        commPort.DataBits = 8;
                        break;
                }

                switch (h)
                {
                    case HandshakeBits.RTS:
                        commPort.Handshake = Handshake.RequestToSend;
                        break;

                    case HandshakeBits.XOnXOff:
                        commPort.Handshake = Handshake.XOnXOff;
                        break;

                    case HandshakeBits.None:
                        commPort.Handshake = Handshake.None;
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());

             /*   if (debug && !main_screen.ApplicationClosing)
                    main_screen.Invoke(new DebugCallbackFunction(main_screen.DebugCallback),
                        "Serial Port set parameters error!" + ex.ToString());*/
            }
        }

        /// <summary>
        /// Send byte[] array
        /// </summary>
        /// <param name="b"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public uint put(byte[] b, uint count)
        {
            try
            {
                commPort.Write(b, 0, (int)count);
                return count;
            }
            catch
                (Exception ex)
            {
                Debug.Write(ex.ToString());

             /*   if (debug && !main_screen.ApplicationClosing)
                    main_screen.Invoke(new DebugCallbackFunction(main_screen.DebugCallback),
                        "Serial Port put data error!" + ex.ToString());
                */
                return 0;
            }
        }

        /// <summary>
        /// Open serial port
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            try
            {
                comMutex.WaitOne();

                commPort.Open();
                isOpen = commPort.IsOpen;
                comMutex.ReleaseMutex();

                if (isOpen)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());

          /*      if (debug && !main_screen.ApplicationClosing)
                    main_screen.Invoke(new DebugCallbackFunction(main_screen.DebugCallback),
                        "Serial Port open error!" + ex.ToString());
                */
                comMutex.ReleaseMutex();
                return false;
            }
        }

        /// <summary>
        /// Check if COMx port is open
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            try
            {
                return isOpen;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Close serial port
        /// </summary>
        public void Destroy()
        {
            try
            {
                comMutex.WaitOne(1000);
                commPort.Close();
                comMutex.ReleaseMutex();
            }
            catch (Exception ex)
            {
                comMutex.ReleaseMutex();
                Debug.Write(ex.ToString());

            /*    if (debug && !main_screen.ApplicationClosing)
                    main_screen.Invoke(new DebugCallbackFunction(main_screen.DebugCallback),
                        "Serial Port close error!" + ex.ToString());*/
            }

            isOpen = false;
        }

        #endregion

        #region serial port evenets

        /// <summary>
        /// Serial port received data callback function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                comMutex.WaitOne();
                String line = "";
                byte[] buffer=new byte[3];
                while (commPort.BytesToRead > 0)
                    //line += commPort.ReadExisting();
                    line += commPort.ReadLine();
                // String line = String.Format();
                /*line = String.Format("{0} T", line);
                using (StreamWriter sw = File.AppendText("file.txt"))
                {
                    sw.WriteLine(line);
                } */

                /*if (line != "")
                    main_screen.Invoke(new CommPortLineCallback(device.ProcessSerialData), line, line.Length);
                else
                {
                    line = "";
                }*/

                comMutex.ReleaseMutex();
            }
            catch (Exception ex)
            {
                comMutex.ReleaseMutex();
                Debug.Write(ex.ToString());

           /*     if (debug && !main_screen.ApplicationClosing)
                    main_screen.Invoke(new DebugCallbackFunction(main_screen.DebugCallback),
                        "Serial Port data received error!" + ex.ToString());*/
            }
        }

        /// <summary>
        /// Read data over serial port
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public int ReadData(ref byte[] buffer)
        {
            try
            {
                comMutex.WaitOne();
                int num_to_read = commPort.BytesToRead;
                int count = commPort.Read(buffer, 0, Math.Min(buffer.Length, num_to_read));
                comMutex.ReleaseMutex();

                return count;
            }
            catch (Exception ex)
            {
                comMutex.ReleaseMutex();
                Debug.Write(ex.ToString());
                return -1;
            }
        }

        /// <summary>
        /// Serial port received error callback function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPort_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());

             /*   if (debug && !main_screen.ApplicationClosing)
                    main_screen.Invoke(new DebugCallbackFunction(main_screen.DebugCallback),
                        "Serial Port Error received!" + ex.ToString());*/
            }
        }

        /// <summary>
        /// Serial port pin changed callback function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPort_PinChanged(object sender, System.IO.Ports.SerialPinChangedEventArgs e)
        {
            try
            {
                Debug.Write(e.EventType.ToString() + "\n");
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());

             /*   if (debug && !main_screen.ApplicationClosing)
                    main_screen.Invoke(new DebugCallbackFunction(main_screen.DebugCallback),
                        "Serial Port Pin changed error!" + ex.ToString());*/
            }
        }

        #endregion
    }
}
