using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using PendulumApp.ViewModel;
using PendulumApp.Dialogs.DialogService;

namespace PendulumApp.Dialogs.DialogSaveFile
{
    public class DialogSaveFileViewModel: DialogViewModelBase
    {
        public DialogSaveFileViewModel(string path, string fileName): base("Save file")
        {
            this.okCommand = new DelegateCommand(OnOKClicked);
            this.cancelCommand = new DelegateCommand(OnCancelClicked);
            SavePathTextBlockText = path;
            FileNameTextBoxText = fileName;
        }
        private string _savePathTextBlockText = "";
        public string SavePathTextBlockText
        {
            get
            {
                return _savePathTextBlockText;
            }
            set
            {
                _savePathTextBlockText = value;
                OnPropertyChanged("SavePathTextBlockText");
            }
        }

        private string _fileNameTextBoxText = "";
        public string FileNameTextBoxText
        {
            get
            {
                return _fileNameTextBoxText;
            }
            set
            {
                _fileNameTextBoxText = value;
                OnPropertyChanged("FileNameTextBoxText");
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
                if (FileNameTextBoxText.Length > 0)
                {
                    this.CloseDialogWithResult(parameter as Window, new DialogResultSaveFile(FileNameTextBoxText, true));
                }
                
            }
            catch (Exception)
            {               
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
                this.CloseDialogWithResult(parameter as Window, new DialogResultSaveFile(FileNameTextBoxText, false));
            }
            catch (Exception)
            {                
            }

        }
        #endregion
    }
}
