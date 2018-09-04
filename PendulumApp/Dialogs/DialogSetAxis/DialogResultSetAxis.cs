using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PendulumApp.Dialogs.DialogService;

namespace PendulumApp.Dialogs.DialogSetAxis
{
    public class DialogResultSetAxis: DialogResult
    {
        public DialogResultSetAxis(double ymax, double ymin,bool okClicked)
        {
            YMax = ymax;
            YMin = ymin;
            OKClicked = okClicked;
        }

        public bool OKClicked
        {
            get;
            private set;
        }

        public double YMax
        {
            get;
            private set;
        }

        public double YMin
        {
            get;
            private set;
        }
    }
}
