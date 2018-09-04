using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PendulumApp.Settings
{
    class SettingProgram
    {
        public SettingProgram(string savePath, float maxBattVoltage, float minBattVoltage, float alertBattVoltage, double[] yMax, double[] yMin,bool floatSaveFormat)
        {
            SavePath = savePath;
            MaxBatteryVoltage = maxBattVoltage;
            MinBatteryVoltage = minBattVoltage;
            BatteryAlertVoltage = alertBattVoltage;
            YMax = yMax;
            YMin = yMin;
            FloatSaveFormat = floatSaveFormat;
        }
        public string SavePath
        {
            get;
            private set;
        }
        public float MaxBatteryVoltage
        {
            get;
            private set;
        }
        public float MinBatteryVoltage
        {
            get;
            private set;
        }
        public float BatteryAlertVoltage
        {
            get;
            private set;
        }
        public double[] YMax
        {
            get;
            private set;
        }
        public double[] YMin
        {
            get;
            private set;
        }
        public bool FloatSaveFormat
        {
            get;
            private set;
        }
    }
}
