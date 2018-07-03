using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PendulumApp.Settings
{
    class SettingACC
    {
        public SettingACC(int scale, List<int> offsetACC0, List<int> offsetACC1)
        {
            Scale = scale;
            OffsetACC0 = offsetACC0;
            OffsetACC1 = offsetACC1;
        }
        public int Scale
        {
            get;
            private set;
        }
        public List<int> OffsetACC0
        {
            get;
            private set;
        }
        public List<int> OffsetACC1
        {
            get;
            private set;
        }
        
    }
}
