using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace PendulumApp.Settings
{
    class SettingService
    {                
        public static SettingProgram LoadSettingProgram()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!path.EndsWith("\\")) path += "\\";
            path += "PendulumApp\\";
            try
            {
                string savePath = "";
                float battMax = 0.0f;
                float battMin = 0.0f;
                float battAlert = 0.0f;    

                string line;

                if (!File.Exists(path + "Settings.dat")) {                    
                    savePath = path + "SavedFiles";
                    if (!Directory.Exists(savePath)) {
                        Directory.CreateDirectory(path);
                    }
                    return new SettingProgram(savePath, 2.6f, 1.9f, 2.1f);
                }
                StreamReader sr = File.OpenText(path + "Settings.dat");

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (String.Compare(line, 0, "ProgramSetting", 0, 14) == 0)
                    {
                        if (String.Compare(line, 0, "ProgramSetting_SavePath", 0, 23) == 0) savePath = line.Substring(24);
                        if (String.Compare(line, 0, "ProgramSetting_BattMax", 0, 22) == 0) battMax = (float) Double.Parse(line.Substring(23), CultureInfo.InvariantCulture);
                        if (String.Compare(line, 0, "ProgramSetting_BattMin", 0, 22) == 0) battMin = (float) Double.Parse(line.Substring(23), CultureInfo.InvariantCulture);
                        if (String.Compare(line, 0, "ProgramSetting_BattAlert", 0, 24) == 0) battAlert = (float) Double.Parse(line.Substring(25), CultureInfo.InvariantCulture);                        
                    }

                }
                sr.Close();
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(path + "SavedFiles");
                    savePath = path + "SavedFiles";
                }
                // if (!System.IO.Directory.Exists(savePath)) throw new Exception("Save directory declared in Setting.dat does not exist. Create it manualy or change save path!!!");
                return new SettingProgram(savePath, battMax, battMin, battAlert);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public static SettingEMG LoadSettingEMG()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!path.EndsWith("\\")) path += "\\";
            path += "PendulumApp\\";
            try
            {
                int scale = 0;                

                string line;
                if (!File.Exists(path + "Settings.dat"))
                {                  
                    return new SettingEMG(1);
                }
                System.IO.StreamReader sr = System.IO.File.OpenText(path + "Settings.dat");

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (String.Compare(line, 0, "EMGSetting", 0, 10) == 0)
                    {
                        if (String.Compare(line, 0, "EMGSetting_Scale=", 0, 17) == 0) scale = Int32.Parse(line.Substring(17));                        

                    }
                }
                sr.Close();

                return new SettingEMG(scale);


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static SettingACC LoadSettingACC()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!path.EndsWith("\\")) path += "\\";
            path += "PendulumApp\\";
            try
            {
                int scale = 0;
                List<int> offsetACC0 = new List<int>();
                List<int> offsetACC1 = new List<int>();

                string line;
                if (!File.Exists(path + "Settings.dat"))
                {
                    offsetACC0.Add(0);
                    offsetACC0.Add(0);
                    offsetACC0.Add(0);
                    offsetACC1.Add(0);
                    offsetACC1.Add(0);
                    offsetACC1.Add(0);

                    return new SettingACC(1, offsetACC0, offsetACC1);
                }
                System.IO.StreamReader sr = System.IO.File.OpenText(path + "Settings.dat");

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (String.Compare(line, 0, "ACCSetting", 0, 10) == 0)
                    {
                        if (String.Compare(line, 0, "ACCSetting_Scale=", 0, 17) == 0) scale = Int32.Parse(line.Substring(17));
                        if (String.Compare(line, 0, "ACCSetting_OffsetACC0x=", 0, 23) == 0) offsetACC0.Add(Int32.Parse(line.Substring(23)));
                        if (String.Compare(line, 0, "ACCSetting_OffsetACC0y=", 0, 23) == 0) offsetACC0.Add(Int32.Parse(line.Substring(23)));
                        if (String.Compare(line, 0, "ACCSetting_OffsetACC0z=", 0, 23) == 0) offsetACC0.Add(Int32.Parse(line.Substring(23)));                        
                        if (String.Compare(line, 0, "ACCSetting_OffsetACC1x=", 0, 23) == 0) offsetACC1.Add(Int32.Parse(line.Substring(23)));
                        if (String.Compare(line, 0, "ACCSetting_OffsetACC1y=", 0, 23) == 0) offsetACC1.Add(Int32.Parse(line.Substring(23)));
                        if (String.Compare(line, 0, "ACCSetting_OffsetACC1z=", 0, 23) == 0) offsetACC1.Add(Int32.Parse(line.Substring(23)));
                    }
                }
                sr.Close();

                return new SettingACC(scale, offsetACC0, offsetACC1);


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static SettingGY LoadSettingGY()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!path.EndsWith("\\")) path += "\\";
            path += "PendulumApp\\";
            try
            {
                int scale = 0;
                List<int> offsetGY0 = new List<int>();
                List<int> offsetGY1 = new List<int>();

                string line;
                if (!File.Exists(path + "Settings.dat"))
                {
                    offsetGY0.Add(0);
                    offsetGY0.Add(0);
                    offsetGY0.Add(0);
                    offsetGY1.Add(0);
                    offsetGY1.Add(0);
                    offsetGY1.Add(0);

                    return new SettingGY(1, offsetGY0, offsetGY1);
                }
                System.IO.StreamReader sr = System.IO.File.OpenText(path + "Settings.dat");

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (String.Compare(line, 0, "GYSetting", 0, 9) == 0)
                    {
                        if (String.Compare(line, 0, "GYSetting_Scale=", 0, 16) == 0) scale = Int32.Parse(line.Substring(16));
                        if (String.Compare(line, 0, "GYSetting_OffsetGY0x=", 0, 21) == 0) offsetGY0.Add(Int32.Parse(line.Substring(21)));
                        if (String.Compare(line, 0, "GYSetting_OffsetGY0y=", 0, 21) == 0) offsetGY0.Add(Int32.Parse(line.Substring(21)));
                        if (String.Compare(line, 0, "GYSetting_OffsetGY0z=", 0, 21) == 0) offsetGY0.Add(Int32.Parse(line.Substring(21)));
                        if (String.Compare(line, 0, "GYSetting_OffsetGY1x=", 0, 21) == 0) offsetGY1.Add(Int32.Parse(line.Substring(21)));
                        if (String.Compare(line, 0, "GYSetting_OffsetGY1y=", 0, 21) == 0) offsetGY1.Add(Int32.Parse(line.Substring(21)));
                        if (String.Compare(line, 0, "GYSetting_OffsetGY1z=", 0, 21) == 0) offsetGY1.Add(Int32.Parse(line.Substring(21)));
                    }
                }
                sr.Close();

                return new SettingGY(scale, offsetGY0, offsetGY1);


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static void StoreSetting(SettingProgram settingProgram, SettingEMG settingEMG, SettingACC settingACC, SettingGY settingGY)
        {

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!path.EndsWith("\\")) path += "\\";
            path += "PendulumApp\\";
            StreamWriter sw = File.CreateText(path + "Settings.dat");

            try
            {
                string line = String.Format("//ProgramSetting");
                string newline = sw.NewLine;
                sw.WriteLine(line);
                line = "ProgramSetting_SavePath=" + settingProgram.SavePath;
                sw.WriteLine(line);
                line = "ProgramSetting_BattMax=" + settingProgram.MaxBatteryVoltage.ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_BattMin=" + settingProgram.MinBatteryVoltage.ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_BattAlert=" + settingProgram.BatteryAlertVoltage.ToString(CultureInfo.InvariantCulture) + newline;
                sw.WriteLine(line);

                line = String.Format("//EMGSetting");
                sw.WriteLine(line);
                line = String.Format("EMGSetting_Scale={0}", settingEMG.Scale);
                sw.WriteLine(line + newline);

                line = String.Format("//ACCSetting");
                sw.WriteLine(line);
                line = String.Format("ACCSetting_Scale={0}", settingACC.Scale);
                sw.WriteLine(line);
                line = String.Format("ACCSetting_OffsetACC0x={0}", settingACC.OffsetACC0.ElementAt(0));
                sw.WriteLine(line);
                line = String.Format("ACCSetting_OffsetACC0y={0}", settingACC.OffsetACC0.ElementAt(1));
                sw.WriteLine(line);
                line = String.Format("ACCSetting_OffsetACC0z={0}", settingACC.OffsetACC0.ElementAt(2));
                sw.WriteLine(line);
                line = String.Format("ACCSetting_OffsetACC1x={0}", settingACC.OffsetACC1.ElementAt(0));
                sw.WriteLine(line);
                line = String.Format("ACCSetting_OffsetACC1y={0}", settingACC.OffsetACC1.ElementAt(1));
                sw.WriteLine(line);
                line = String.Format("ACCSetting_OffsetACC1z={0}", settingACC.OffsetACC1.ElementAt(2));
                sw.WriteLine(line + newline);

                line = String.Format("//GYSetting");
                sw.WriteLine(line);
                line = String.Format("GYSetting_Scale={0}", settingGY.Scale);
                sw.WriteLine(line);
                line = String.Format("GYSetting_OffsetGY0x={0}", settingGY.OffsetGY0.ElementAt(0));
                sw.WriteLine(line);
                line = String.Format("GYSetting_OffsetGY0y={0}", settingGY.OffsetGY0.ElementAt(1));
                sw.WriteLine(line);
                line = String.Format("GYSetting_OffsetGY0z={0}", settingGY.OffsetGY0.ElementAt(2));
                sw.WriteLine(line);
                line = String.Format("GYSetting_OffsetGY1x={0}", settingGY.OffsetGY1.ElementAt(0));
                sw.WriteLine(line);
                line = String.Format("GYSetting_OffsetGY1y={0}", settingGY.OffsetGY1.ElementAt(1));
                sw.WriteLine(line);
                line = String.Format("GYSetting_OffsetGY1z={0}", settingGY.OffsetGY1.ElementAt(2));
                sw.WriteLine(line + newline);

                sw.Close();
            }
            catch (Exception ex) {
                Log log = new Log();
                log.LogMessageToFile("Settings/SettingService/StoreSetting:" + ex.Message);
                Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Error, ex.Message);
                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
            }
            finally
            {
                sw.Close();

            }

        }
    }
}
