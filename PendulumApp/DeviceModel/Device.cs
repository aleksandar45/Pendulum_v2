using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.IO.Ports;
using PendulumApp.ViewModel;
using PendulumApp.ViewModel.OpenGLRender;


namespace PendulumApp.DeviceModel
{
    class Device
    {
        MainWindowViewModel mainWindowViewModel;
        OpenGLDispatcher glD;
        DispatcherTimer parseTimer;

        string TAG = "DeviceModel/Device/";
        bool record;
        bool calibrate;
        ulong counter;
        MSerialPort comPort;
        RingBufferByte comPortRB;

        List<Int32> recorded_data;
        List<Int32> recorded_packets;

        public Device(MainWindowViewModel mainWindowViewModel) {
            try
            {
                this.mainWindowViewModel = mainWindowViewModel;
                this.mainWindowViewModel.WindowClosingEventHandler += dispose;
                this.mainWindowViewModel.devicePlayExecute += StartStopPlaying;
                this.mainWindowViewModel.deviceRecordExecute += StartStopRecording;
                this.mainWindowViewModel.deviceCalibrateExecute += Calibrate;                           

                glD = mainWindowViewModel.OpenGLDispatcher;
                recorded_data = new List<Int32>(1380000);
                recorded_packets = new List<Int32>(30000);

                comPortRB = new RingBufferByte(16384);
                comPort = new MSerialPort();

                parseTimer = new DispatcherTimer();
                parseTimer.Tick += new EventHandler(ParseRingBuffers);
                parseTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);  //20 miliseconds timer period
                parseTimer.Start();
            }

            catch (Exception ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "Device:" + ex.Message);
            }
            finally { }
        }

        public void ParseRingBuffers(object sender, EventArgs e) {

        }
        public bool StartStopPlaying(bool playing)
        {
            try
            {
                if (playing)
                {
                    mainWindowViewModel.ComboboxCOMPortsIsEnabled = true;
                    if (comPort.IsOpen())
                    {
                        byte[] b = new byte[3];
                        b[0] = (byte)'+';
                        b[1] = (byte)'S';
                        b[2] = (byte)'-';
                        comPort.put(b, 3);

                        comPort.Destroy();
                        return true;
                    }
                    else
                    {
                        throw new Exception("COM port busy!!! Disconnect device and try again \n");                                                
                    }

                }
                else
                {
                    if (mainWindowViewModel.ComboBoxCOMPorts.Count > 0)
                    {
                        string port = mainWindowViewModel.ComboboxSelectedItem;

                        glD.emgIndex = 0;
                        glD.accIndex = 0;
                        glD.gyIndex = 0;
                       
                        comPort.setCommParms(port, 460800, Parity.NONE, DataBits.EIGHT, StopBits.ONE, HandshakeBits.None);
                        comPort.Open();

                        if (comPort.IsOpen())
                        {
                            mainWindowViewModel.ComboboxCOMPortsIsEnabled = false;
                            byte[] b = new byte[3];
                            b[0] = (byte)'+';
                            b[1] = (byte)'T';
                            b[2] = (byte)'-';
                            comPort.put(b, 3);                            
                            return true;
                        }
                        else
                        {
                            throw new Exception("COM port busy \n ");                           
                        }
                    }
                    else
                    {
                        throw new Exception("COM port busy \n ");                        
                    }

                }
                
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "StartStopPlaying:" + ex.Message);
                Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Error, ex.Message);
                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
                return false;
            }
        }
        public bool StartStopRecording(bool recording)
        {
            try
            {
                if (recording)
                {
                    counter = 0;
                    record = false;
                    recorded_packets.Clear();
                }
                else
                {
                    counter = 0;
                    record = true;
                    recorded_packets.Clear();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "StartStopRecording:" + ex.Message);
                Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Error, ex.Message);
                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
                return false;
            }
        }
        public bool Calibrate()
        {
            calibrate = true;
            return true;
        }

        private int ThreeByteToInt(ref byte[] source)
        {
            Int32 result = 0;


            result = source[0];
            result = result << 8;
            result |= (source[1] & 0x00FF);
            result = result << 8;
            result |= (source[2] & 0x00FF);
            if ((result >> 23) > 0)
            {
                result = result - 16777216;
            }

            return result;
        }
        private int TwoByteToInt(ref byte[] source)
        {
            int result = 0;

            result = source[0];
            result = result << 8;
            result |= (source[1] & 0x00FF);
            if ((result >> 15) > 0)
            {
                result = result - 65536;
            }
            return result;
        }

        public void dispose(object sender, EventArgs e)
        {

        }
    }
}
