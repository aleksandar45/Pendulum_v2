using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PendulumApp.Settings
{
    class SettingEMG
    {
        public SettingEMG(int scale) {
            Scale = scale;
        }
        public int Scale
        {
            get;
            private set;
        }
    }
}
