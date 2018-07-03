using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PendulumApp.Settings
{
    class SettingProgram
    {
        public SettingProgram(string savePath, float maxBattVoltage, float minBattVoltage, float alertBattVoltage)
        {
            SavePath = savePath;
            MaxBatteryVoltage = maxBattVoltage;
            MinBatteryVoltage = minBattVoltage;
            BatteryAlertVoltage = alertBattVoltage;
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
    }
}
