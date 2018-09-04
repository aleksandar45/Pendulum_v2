using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using PendulumApp.ViewModel;
using PendulumApp.Dialogs.DialogService;

namespace PendulumApp.Dialogs.DialogSetAxis
{
    public class DialogSetAxisViewModel : DialogViewModelBase
    {
        public DialogSetAxisViewModel() : base("Set axis")
        {
            this.okCommand = new DelegateCommand(OnOKClicked);
            this.cancelCommand = new DelegateCommand(OnCancelClicked);
        }

        private string _yMaxTextBoxText = "";
        public string YMaxTextBoxText
        {
            get
            {
                return _yMaxTextBoxText;
            }
            set
            {
                _yMaxTextBoxText = value;
                OnPropertyChanged("YMaxTextBoxText");
            }
        }

        private string _yMinTextBoxText = "";
        public string YMinTextBoxText
        {
            get
            {
                return _yMinTextBoxText;
            }
            set
            {
                _yMinTextBoxText = value;
                OnPropertyChanged("YMinTextBoxText");
            }
        }

        private string _warningMessageTextBlock = "";
        public string WarningMessageTextBlock
        {
            get
            {
                return _warningMessageTextBlock;
            }
            set
            {
                _warningMessageTextBlock = value;
                OnPropertyChanged("WarningMessageTextBlock");
            }
        }

        #region BUTTON_OK
        private ICommand okCommand = null;
        public ICommand OKCommand
        {
            get { return okCommand; }
            set { okCommand = value; }
        }
        private void OnOKClicked(object parameter)
        {
            try
            {
                double yMax, yMin;
                yMax = Double.Parse(_yMaxTextBoxText, System.Globalization.NumberStyles.AllowDecimalPoint);
                yMin = Double.Parse(_yMinTextBoxText, System.Globalization.NumberStyles.Any);
                if (yMax <= yMin)
                {
                    WarningMessageTextBlock = "Ymax must be greater than Ymin!";
                    return;
                }
                this.CloseDialogWithResult(parameter as Window, new DialogResultSetAxis(yMax,yMin, true));
            }
            catch(Exception)
            {
                WarningMessageTextBlock = "Number format is not OK!";
            }
            
        }
        #endregion

        #region BUTTON_CANCEL
        private ICommand cancelCommand = null;
        public ICommand CancelCommand
        {
            get { return cancelCommand; }
            set { cancelCommand = value; }
        }
        private void OnCancelClicked(object parameter)
        {
            try
            {                
                this.CloseDialogWithResult(parameter as Window, new DialogResultSetAxis(0, 0, false));
            }
            catch (Exception)
            {
                WarningMessageTextBlock = "Somthing is wrong in code behind";
            }

        }
        #endregion
    }
}
