using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PendulumApp.Settings
{
    class SettingGY
    {
        public SettingGY(int scale, List<int> offsetGY0, List<int> offsetGY1)
        {
            Scale = scale;
            OffsetGY0 = offsetGY0;
            OffsetGY1 = offsetGY1;
        }
        public int Scale
        {
            get;
            private set;
        }
        public List<int> OffsetGY0
        {
            get;
            private set;
        }
        public List<int> OffsetGY1
        {
            get;
            private set;
        }
    }
}
