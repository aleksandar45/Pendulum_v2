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
        DispatcherTimer connectionTimer;

        string TAG = "DeviceModel/Device/";
        bool record;
        bool calibrate;
        ulong counter;
        bool connected = false;
        Int16 batteryValue;
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

                connectionTimer = new DispatcherTimer();
                connectionTimer.Tick += new EventHandler(ConnectionTimerTimeout);
                connectionTimer.Interval = new TimeSpan(0, 0, 0, 0, 400);  //20 miliseconds timer period
                connectionTimer.Start();
            }

            catch (Exception ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "Device:" + ex.Message);
            }
            finally { }
        }
        public void ConnectionTimerTimeout(object sender, EventArgs e) {
            if (mainWindowViewModel.Playing)
            {
                if (!connected)
                {
                    mainWindowViewModel.COMConnectionStatus = false;
                    mainWindowViewModel.BLEConnectionStatus = false;
                    mainWindowViewModel.BatteryLowLevel = true;
                    mainWindowViewModel.BatteryStatusTextLabel = "Battery: N/A";
                }
                else
                {
                    mainWindowViewModel.COMConnectionStatus = true;
                    mainWindowViewModel.BatteryStatusTextLabel = "Battery: " + batteryValue.ToString() + "%";
                    if(batteryValue < 15)
                    {
                        mainWindowViewModel.BatteryLowLevel = true;
                    }
                    else
                    {
                        mainWindowViewModel.BatteryLowLevel = false;
                    }
                }
                connected = false;
            }
           

        }
        public void ParseRingBuffers(object sender, EventArgs e) {

            byte[] comBuffer = new byte[8192];
            byte[] tmpBuffer = new byte[100];
            byte[] dataBuffer = new byte[100];

            int emgCH1, emgCH2;
            int x, y, z;
            int status;
            Int32 int32_temp;
            Int16 int16_temp;

            try
            {
                int count = comPort.ReadData(ref comBuffer);
                if (comPortRB.WriteSpace() < count)
                {
                    throw new Exception("Not enough space in ring buffer");
                }
                if (count > 0 && (comPortRB.WriteSpace() >= count))
                    comPortRB.Write(comBuffer, count);

                /*if (count > 0 && (comPortRB.ReadSpace() < count))
                    comPortRB.Write(comBuffer, count);*/
                if ((record) && (recorded_data.Count > 1500000))  //1500000))
                {
                    recorded_data.Clear();
                    recorded_packets.Clear();
                    parseTimer.Stop();
                    throw new Exception("Size of recorded file exceeds limit \n ");                   

                }
                if (comPortRB.ReadSpace() >= 104)
                {
                    while (comPortRB.ReadSpace() > 1)
                    {
                        comPortRB.Read(tmpBuffer, 1);

                        if (tmpBuffer[0] == 0xFF)               // 0xff
                        {
                            comPortRB.Read(tmpBuffer, 1);

                            if (tmpBuffer[0] == 0xFF)           // 0xff
                            {
                                comPortRB.Read(tmpBuffer, 1);

                                if (tmpBuffer[0] == 0xFF)       // 0xff
                                {
                                    comPortRB.Read(tmpBuffer, 2);
                                    if ((tmpBuffer[0] == 'D') && (tmpBuffer[1] == 44))     // 
                                    {
                                        comPortRB.Read(dataBuffer, 44);
                                        comPortRB.Read(tmpBuffer, 3);
                                        if ((tmpBuffer[0] == 'E') && (tmpBuffer[1] == 'N') && (tmpBuffer[2] == 'D'))
                                        {
                                            connected = true;
                                            if (mainWindowViewModel.COMConnectionStatus == false)
                                            {
                                                mainWindowViewModel.COMConnectionStatus = true;
                                            }
                                            for (int i = 0; i < 30; i += 6)
                                            {
                                                // channel I                    
                                                int32_temp = (Int32)(dataBuffer[i] * 65536 + dataBuffer[i + 1] * 256 + dataBuffer[i + 2]);
                                                if ((int32_temp & 0x800000) == 0x800000)
                                                    glD.emgChannels.ElementAt(0).intArray[glD.emgIndex] = int32_temp - 16777216;
                                                else
                                                    glD.emgChannels.ElementAt(0).intArray[glD.emgIndex] = int32_temp;

                                                // channel II                    
                                                int32_temp = (Int32)(dataBuffer[i + 3] * 65536 + dataBuffer[i + 4] * 256 + dataBuffer[i + 5]);
                                                if ((int32_temp & 0x800000) == 0x800000)
                                                    glD.emgChannels.ElementAt(1).intArray[glD.emgIndex] = int32_temp - 16777216;
                                                else
                                                    glD.emgChannels.ElementAt(1).intArray[glD.emgIndex] = int32_temp;

                                                glD.emgIndex++;
                                            }

                                            //ACC0
                                            int16_temp = (Int16)(dataBuffer[30] * 256 + dataBuffer[31]);
                                            glD.accChannels.ElementAt(0).intArray[glD.accIndex] = int16_temp;

                                            //ACC1
                                            int16_temp = (Int16)(dataBuffer[32] * 256 + dataBuffer[33]);
                                            glD.accChannels.ElementAt(1).intArray[glD.accIndex] = int16_temp;

                                            glD.accIndex++;

                                            //GY0_0
                                            int16_temp = (Int16)(dataBuffer[34] * 256 + dataBuffer[35]);
                                            glD.gyChannels.ElementAt(0).intArray[glD.gyIndex] = int16_temp;

                                            //GY0_1
                                            int16_temp = (Int16)(dataBuffer[36] * 256 + dataBuffer[37]);
                                            glD.gyChannels.ElementAt(1).intArray[glD.gyIndex] = int16_temp;

                                            //GY1
                                            int16_temp = (Int16)(dataBuffer[38] * 256 + dataBuffer[39]);
                                            glD.gyChannels.ElementAt(2).intArray[glD.gyIndex] = int16_temp;

                                            glD.gyIndex++;

                                            //BATT
                                            batteryValue = (Int16)(dataBuffer[40] * 256 + dataBuffer[41]);                                            

                                            if (((dataBuffer[42] & 0x01) == 0x01) && (mainWindowViewModel.AccGy0Status == true))
                                            {
                                                mainWindowViewModel.AccGy0Status = false;

                                            }
                                            else if (((dataBuffer[42] & 0x01) == 0x00) && (mainWindowViewModel.AccGy0Status == false))
                                            {
                                                mainWindowViewModel.AccGy0Status = true;
                                            }
                                            if (((dataBuffer[42] & 0x02) == 0x01) && (mainWindowViewModel.AccGy1Status == true))
                                            {
                                                mainWindowViewModel.AccGy1Status = false;

                                            }
                                            else if (((dataBuffer[42] & 0x02) == 0x00) && (mainWindowViewModel.AccGy1Status == false))
                                            {
                                                mainWindowViewModel.AccGy1Status = true;
                                            }
                                            if (((dataBuffer[42] & 0x04) == 0x01) && (mainWindowViewModel.EMGStatus == true))
                                            {
                                                mainWindowViewModel.EMGStatus = false;

                                            }
                                            else if (((dataBuffer[42] & 0x04) == 0x00) && (mainWindowViewModel.EMGStatus == false))
                                            {
                                                mainWindowViewModel.EMGStatus = true;
                                            }
                                            if (((dataBuffer[43] & 0x01) == 0x01) && (mainWindowViewModel.BLEConnectionStatus == false))
                                            {
                                                mainWindowViewModel.BLEConnectionStatus = true;

                                            }
                                            else if (((dataBuffer[43] & 0x01) == 0x00) && (mainWindowViewModel.BLEConnectionStatus == true))
                                            {
                                                mainWindowViewModel.BLEConnectionStatus = false;
                                            }

                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                glD.renderDisplays();

            }
            catch (Exception ex)
            {
                Log l = new Log();
                l.LogMessageToFile(TAG + "ParseRingBuffers:" + ex.ToString());
                Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Error, ex.Message);
                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
                parseTimer.Stop();
            }

        }
        public bool StartStopPlaying(bool playing)
        {
            try
            {
                if (playing)
                {
                    mainWindowViewModel.ComboboxCOMPortsIsEnabled = true;
                    mainWindowViewModel.COMConnectionStatus = false;
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
                       
                        comPort.setCommParms(port, 115200, Parity.NONE, DataBits.EIGHT, StopBits.ONE, HandshakeBits.None);
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
