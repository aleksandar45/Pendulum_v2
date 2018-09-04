using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PendulumApp.Dialogs.DialogService;

namespace PendulumApp.Dialogs.DialogSaveFile
{
    public class DialogResultSaveFile: DialogResult
    {
        public DialogResultSaveFile(string fileName,bool okClicked)
        {
            FileName = fileName;
            OKClicked = okClicked;
        }

        public bool OKClicked
        {
            get;
            private set;
        }


        public string FileName
        {
            get;
            private set;
        }
    }
}
