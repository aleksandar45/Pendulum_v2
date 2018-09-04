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
                double[] yMax = new double[7] {1,1,1,1,1,1,1};
                double[] yMin = new double[7];
                bool floatSaveFormat = true;

                string line;

                if (!File.Exists(path + "Settings.dat")) {                    
                    savePath = path + "SavedFiles";
                    if (!Directory.Exists(savePath)) {
                        Directory.CreateDirectory(path);
                    }
                    return new SettingProgram(savePath, 2.6f, 1.9f, 2.1f,yMax, yMin, floatSaveFormat);
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
                        if (String.Compare(line, 0, "ProgramSetting_OpenGlYmax", 0, 25) == 0)
                        {
                            double tempYmax = (float)Double.Parse(line.Substring(27), CultureInfo.InvariantCulture);
                            switch (line[25])
                            {
                                case '1':
                                    yMax[0] = tempYmax;
                                    break;
                                case '2':
                                    yMax[1] = tempYmax;
                                    break;
                                case '3':
                                    yMax[2] = tempYmax;
                                    break;
                                case '4':
                                    yMax[3] = tempYmax;
                                    break;
                                case '5':
                                    yMax[4] = tempYmax;
                                    break;
                                case '6':
                                    yMax[5] = tempYmax;
                                    break;
                                case '7':
                                    yMax[6] = tempYmax;
                                    break;
                            }
                            
                        }
                        if (String.Compare(line, 0, "ProgramSetting_OpenGlYmin", 0, 25) == 0)
                        {
                            double tempYmin = (float)Double.Parse(line.Substring(27), CultureInfo.InvariantCulture);
                            switch (line[25])
                            {
                                case '1':
                                    yMin[0] = tempYmin;
                                    break;
                                case '2':
                                    yMin[1] = tempYmin;
                                    break;
                                case '3':
                                    yMin[2] = tempYmin;
                                    break;
                                case '4':
                                    yMin[3] = tempYmin;
                                    break;
                                case '5':
                                    yMin[4] = tempYmin;
                                    break;
                                case '6':
                                    yMin[5] = tempYmin;
                                    break;
                                case '7':
                                    yMin[6] = tempYmin;
                                    break;
                            }

                        }
                        if (String.Compare(line, 0, "ProgramSetting_FloatSaveFormat,", 0, 30) == 0) floatSaveFormat = bool.Parse(line.Substring(31));
                    }

                }
                sr.Close();
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(path + "SavedFiles");
                    savePath = path + "SavedFiles";
                }
                // if (!System.IO.Directory.Exists(savePath)) throw new Exception("Save directory declared in Setting.dat does not exist. Create it manualy or change save path!!!");
                return new SettingProgram(savePath, battMax, battMin, battAlert,yMax,yMin, floatSaveFormat);
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
                    return new SettingEMG(6990);
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
                    //8192 = 1g = 9.81m/s^2 => 8192/9.81 = 835
                    return new SettingACC(835, offsetACC0, offsetACC1);
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
                    //32768=500deg/s=8.72rad/s =>32768/8.72 = 3755
                    return new SettingGY(3755, offsetGY0, offsetGY1);
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
                line = "ProgramSetting_BattAlert=" + settingProgram.BatteryAlertVoltage.ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_OpenGlYmax1=" + settingProgram.YMax[0].ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_OpenGlYmin1=" + settingProgram.YMin[0].ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_OpenGlYmax2=" + settingProgram.YMax[1].ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_OpenGlYmin2=" + settingProgram.YMin[1].ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_OpenGlYmax3=" + settingProgram.YMax[2].ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_OpenGlYmin3=" + settingProgram.YMin[2].ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_OpenGlYmax4=" + settingProgram.YMax[3].ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_OpenGlYmin4=" + settingProgram.YMin[3].ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_OpenGlYmax5=" + settingProgram.YMax[4].ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_OpenGlYmin5=" + settingProgram.YMin[4].ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_OpenGlYmax6=" + settingProgram.YMax[5].ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_OpenGlYmin6=" + settingProgram.YMin[5].ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_OpenGlYmax7=" + settingProgram.YMax[6].ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_OpenGlYmin7=" + settingProgram.YMin[6].ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_FloatSaveFormat=" + settingProgram.FloatSaveFormat.ToString() + newline;
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
