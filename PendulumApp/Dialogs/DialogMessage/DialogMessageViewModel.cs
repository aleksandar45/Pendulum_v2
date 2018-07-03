using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using PendulumApp.Dialogs.DialogService;
using PendulumApp.ViewModel;
using System.Windows;

namespace PendulumApp.Dialogs.DialogMessage
{
    public class DialogMessageViewModel : DialogViewModelBase
    {
        public string Message
        {
            get;
            private set;
        }
        private ICommand okCommand = null;
        public ICommand OKCommand
        {
            get { return okCommand; }
            set { okCommand = value; }
        }

        public string ImageSource
        {
            get;
            private set;

        }

        public DialogMessageViewModel(DialogImageTypeEnum imageType,string message): base(ModifyMessage(imageType))
        {
            this.Message = message;
            this.okCommand = new DelegateCommand(OnOKClicked);
            switch (imageType) { 
                case DialogImageTypeEnum.Error:
                    ImageSource = "/PendulumApp;component/Images/Error.png";                    
                    break;
                case DialogImageTypeEnum.Info:
                    ImageSource = "/PendulumApp;component/Images/Info.png";                    
                    break;
                case DialogImageTypeEnum.Warning:
                    ImageSource = "/PendulumApp;component/Images/Warning.png";                    
                    break;
            }
        }
        private static string ModifyMessage(DialogImageTypeEnum imageType) { 
            switch (imageType) { 
                case DialogImageTypeEnum.Error:                    
                    return "Error";
                case DialogImageTypeEnum.Info:
                    return "Info";
                case DialogImageTypeEnum.Warning:
                    return "Warning";
                default:
                    return "Error";
            }
        }
        private void OnOKClicked(object parameter)
        {
            this.CloseDialogWithResult(parameter as Window, new DialogResultMessage());

        }
    }
}
