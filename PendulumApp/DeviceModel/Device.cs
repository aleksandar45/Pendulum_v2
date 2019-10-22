using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.IO;
using System.IO.Ports;
using System.Globalization;
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
                    mainWindowViewModel.AccGy0Status = false;
                    mainWindowViewModel.AccGy1Status = false;
                    mainWindowViewModel.EMGStatus = false;
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
            else
            {
                string[] ports = SerialPort.GetPortNames();

                if (ports.Length != mainWindowViewModel.ComboBoxCOMPorts.Count)
                {
                    List<string>  _comboBoxCOMPorts = new List<string>();                    
                    foreach (string port in ports)
                    {
                        _comboBoxCOMPorts.Add(port);                        
                    }
                    mainWindowViewModel.ComboBoxCOMPorts = _comboBoxCOMPorts;
                    mainWindowViewModel.ComboboxSelectedItem = ports[ports.Length-1];
                }
                else
                {
                    bool comPortChanged = false;
                    for (int i = 0; i < ports.Length; i++) {
                        if (String.Compare(ports[i], mainWindowViewModel.ComboBoxCOMPorts.ElementAt(i)) != 0)
                        {
                            comPortChanged = true;
                        }
                    }
                    if (comPortChanged)
                    {
                        List<string> _comboBoxCOMPorts = new List<string>();
                        foreach (string port in ports)
                        {
                            _comboBoxCOMPorts.Add(port);
                        }
                        mainWindowViewModel.ComboBoxCOMPorts = _comboBoxCOMPorts;
                        mainWindowViewModel.ComboboxSelectedItem = ports[ports.Length - 1];
                    }
                }
                                
            }

        }
        public void ParseRingBuffers(object sender, EventArgs e) {

            byte[] comBuffer = new byte[8192];
            byte[] tmpBuffer = new byte[100];
            byte[] dataBuffer = new byte[100];
            MainWindowViewModel mWVM = mainWindowViewModel;

            int emgCH1, emgCH2;
            int x, y, z;           
            Int32 int32_temp;
            Int16 int16_temp;
            Int16 int16_tempAdd;

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

                                            if (record)
                                            {
                                                recorded_packets.Add(recorded_data.Count);
                                            }

                                            for (int i = 0; i < 30; i += 6)
                                            {
                                                // channel I                    
                                                int32_temp = (Int32)(dataBuffer[i] * 65536 + dataBuffer[i + 1] * 256 + dataBuffer[i + 2]);
                                                if ((int32_temp & 0x800000) == 0x800000)
                                                {
                                                    glD.emgChannels.ElementAt(0).intArray[glD.emgIndex] = (int32_temp - 16777216);
                                                    emgCH1 = (int32_temp - 16777216);
                                                }
                                                else
                                                {
                                                    glD.emgChannels.ElementAt(0).intArray[glD.emgIndex] = int32_temp;
                                                    emgCH1 = int32_temp;
                                                }


                                                // channel II                    
                                                int32_temp = (Int32)(dataBuffer[i + 3] * 65536 + dataBuffer[i + 4] * 256 + dataBuffer[i + 5]);
                                                if ((int32_temp & 0x800000) == 0x800000)
                                                {
                                                    glD.emgChannels.ElementAt(1).intArray[glD.emgIndex] = int32_temp - 16777216;
                                                    emgCH2 = int32_temp - 16777216;
                                                }
                                                else
                                                {
                                                    glD.emgChannels.ElementAt(1).intArray[glD.emgIndex] = int32_temp;
                                                    emgCH2 = int32_temp;
                                                }



                                                glD.emgIndex++;

                                                if (record)
                                                {
                                                    recorded_data.Add(emgCH1);
                                                    recorded_data.Add(emgCH2);
                                                }
                                            }

                                            //ACC0x
                                            int16_temp = (Int16)(dataBuffer[30] * 256 + dataBuffer[31]);
                                            glD.accChannels.ElementAt(0).intArray[glD.accIndex] = int16_temp - mWVM.SettingACCData.OffsetACC0.ElementAt(0);
                                            x = int16_temp - mWVM.SettingACCData.OffsetACC0.ElementAt(0);
                                            if (record)
                                            {
                                                recorded_data.Add(x);
                                            }
                                            if (calibrate)
                                            {
                                                mWVM.SettingACCData.OffsetACC0.Clear();
                                                mWVM.SettingACCData.OffsetACC0.Add(int16_temp);
                                                mWVM.SettingACCData.OffsetACC0.Add(0);
                                                mWVM.SettingACCData.OffsetACC0.Add(0);
                                            }

                                            //ACC1y
                                            int16_temp = (Int16)(dataBuffer[32] * 256 + dataBuffer[33]);
                                            glD.accChannels.ElementAt(1).intArray[glD.accIndex] = int16_temp - mWVM.SettingACCData.OffsetACC1.ElementAt(1);
                                            y = int16_temp - mWVM.SettingACCData.OffsetACC1.ElementAt(1);
                                            if (record)
                                            {
                                                recorded_data.Add(y);
                                            }
                                            if (calibrate)
                                            {
                                                mWVM.SettingACCData.OffsetACC1.Clear();
                                                mWVM.SettingACCData.OffsetACC1.Add(0);
                                                mWVM.SettingACCData.OffsetACC1.Add(int16_temp);
                                                mWVM.SettingACCData.OffsetACC1.Add(0);
                                            }

                                            glD.accIndex++;

                                            //GY0 (y)
                                            int16_temp = (Int16)(dataBuffer[34] * 256 + dataBuffer[35]);
                                            glD.gyChannels.ElementAt(0).intArray[glD.gyIndex] = int16_temp - mWVM.SettingGYData.OffsetGY0.ElementAt(1);
                                            y = int16_temp - mWVM.SettingGYData.OffsetGY0.ElementAt(1);
                                            if (record)
                                            {
                                                recorded_data.Add(y);
                                            }
                                            if (calibrate)
                                            {
                                                mWVM.SettingGYData.OffsetGY0.Clear();
                                                mWVM.SettingGYData.OffsetGY0.Add(0);
                                                mWVM.SettingGYData.OffsetGY0.Add(int16_temp);
                                                mWVM.SettingGYData.OffsetGY0.Add(0);
                                            }

                                            //GY1_0 (x)
                                            int16_temp = (Int16)(dataBuffer[36] * 256 + dataBuffer[37]);
                                            glD.gyChannels.ElementAt(1).intArray[glD.gyIndex] = int16_temp - mWVM.SettingGYData.OffsetGY1.ElementAt(0);
                                            x = int16_temp - mWVM.SettingGYData.OffsetGY1.ElementAt(0);
                                            if (record)
                                            {
                                                recorded_data.Add(x);
                                            }

                                            //GY1_1 (z)
                                            int16_tempAdd = (Int16)(dataBuffer[38] * 256 + dataBuffer[39]);
                                            glD.gyChannels.ElementAt(2).intArray[glD.gyIndex] = int16_tempAdd - mWVM.SettingGYData.OffsetGY1.ElementAt(2);
                                            z = int16_tempAdd - mWVM.SettingGYData.OffsetGY1.ElementAt(2);
                                            if (record)
                                            {
                                                recorded_data.Add(z);
                                            }
                                            if (calibrate)
                                            {
                                                mWVM.SettingGYData.OffsetGY1.Clear();
                                                mWVM.SettingGYData.OffsetGY1.Add(int16_temp);
                                                mWVM.SettingGYData.OffsetGY1.Add(0);
                                                mWVM.SettingGYData.OffsetGY1.Add(int16_tempAdd);
                                                calibrate = false;
                                            }

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
                                            if (mainWindowViewModel.BLEConnectionStatus == false)
                                            {
                                                mainWindowViewModel.BLEConnectionStatus = true;

                                            }
                                           

                                        }

                                    }
                                    if ((tmpBuffer[0] == 'I') && (tmpBuffer[1] == 44))     // 
                                    {
                                        comPortRB.Read(tmpBuffer, 44);
                                        comPortRB.Read(tmpBuffer, 3);
                                        if ((tmpBuffer[0] == 'E') && (tmpBuffer[1] == 'N') && (tmpBuffer[2] == 'D'))
                                        {
                                            connected = true;
                                            if (mainWindowViewModel.COMConnectionStatus == false)
                                            {
                                                mainWindowViewModel.COMConnectionStatus = true;
                                            }
                                            if (mainWindowViewModel.BLEConnectionStatus == true)
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
                    mainWindowViewModel.BLEConnectionStatus = false;
                    mainWindowViewModel.BatteryLowLevel = true;
                    mainWindowViewModel.AccGy0Status = false;
                    mainWindowViewModel.AccGy1Status = false;
                    mainWindowViewModel.EMGStatus = false;
                    mainWindowViewModel.BatteryStatusTextLabel = "Battery: N/A";
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
                    SaveData();
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
        private void SaveData()
        {
            try
            {
                MainWindowViewModel mWVM = mainWindowViewModel;
                string saveFileName = string.Format("{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now);
                string savePath = mainWindowViewModel.SettingProgramData.SavePath;

                Dialogs.DialogService.DialogViewModelBase vm = new Dialogs.DialogSaveFile.DialogSaveFileViewModel(savePath, saveFileName);
                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(vm);
                Dialogs.DialogSaveFile.DialogResultSaveFile saveFileResult = result as Dialogs.DialogSaveFile.DialogResultSaveFile;

                saveFileName = saveFileResult.FileName + ".txt";
                //Stream myStream;
                /* SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                 saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
                 saveFileDialog1.FilterIndex = 2;
                 saveFileDialog1.RestoreDirectory = true;*/
                if (saveFileResult.OKClicked)
                {

                    if (!savePath.EndsWith("\\")) savePath += "\\";
                    string directoryName = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                    if (!Directory.Exists(savePath + directoryName))
                    {
                        Directory.CreateDirectory(savePath + directoryName);
                    }
                    savePath = savePath + directoryName + "\\";


                   
                    using (StreamWriter writer = new StreamWriter(savePath + saveFileName)) //myStream = saveFileDialog1.OpenFile()) != null)
                    {
                        Int32 index;
                        String line;
                        int i, j;

                        int  ACC0x,  ACC1y, GY0y, GY1x, GY1z;

                        line = String.Format("EMG_CH1    EMG_CH2    ACC0x(thigh/transverse)    ACC1y(shank/transverse)    GY0y(thigh/sagital)    GY1x(shank/sagital)    GY1z(shank/transverse)    \n");
                        writer.WriteLine(line);
                        line = String.Format("EMG_CH1    EMG_CH2    ACC0x    ACC1y    GY0y    GY1x    GY1z    \n");
                        writer.WriteLine(line);
                        for (i = 0; i < recorded_packets.Count; i++)
                        {
                            index = recorded_packets.ElementAt(i);

                            index = index + 5 * 2;
                            ACC0x = recorded_data.ElementAt(index + 0);
                            ACC1y = recorded_data.ElementAt(index + 1);

                            GY0y = recorded_data.ElementAt(index + 2);
                            GY1x = recorded_data.ElementAt(index + 3);
                            GY1z = recorded_data.ElementAt(index + 4);                            


                            index = recorded_packets.ElementAt(i);
                            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                            culture.NumberFormat.NumberDecimalSeparator = ".";
                            for (j = 0; j < 5; j++)
                            {
                                if (mainWindowViewModel.SettingProgramData.FloatSaveFormat)
                                {
                                    line = String.Format(new CultureInfo("en-GB"), "{0:#0.####}   {1:#0.####}    {2:#0.####}    {3:#0.##}    {4:#0.##}    {5:#0.####}    {6:#0.####}    ",
                                    recorded_data.ElementAt(index + j * 2) / (float) mWVM.SettingEMGData.Scale, recorded_data.ElementAt(index + j * 2 + 1) / (float) mWVM.SettingEMGData.Scale, ACC0x / (float) mWVM.SettingACCData.Scale, ACC1y / (float)mWVM.SettingACCData.Scale, GY0y / (float) mWVM.SettingGYData.Scale, GY1x / (float) mWVM.SettingGYData.Scale, GY1z / (float) mWVM.SettingGYData.Scale);
                                }
                                else
                                {
                                    line = String.Format("{0}    {1}    {2}    {3}    {4}    {5}    {6}",
                                    recorded_data.ElementAt(index + j * 2), recorded_data.ElementAt(index + j * 2 + 1), ACC0x, ACC1y, GY0y, GY1x, GY1z);
                                }

                                writer.WriteLine(line);
                            }
                        }
                        writer.Close();
                    }
                   
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "SaveData:" + ex.Message);
                Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Error, ex.Message);
                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
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
            if (mainWindowViewModel.Playing) {
                StartStopPlaying(true);
            }
        }
    }
}
