using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.IO.Ports;
using System.Windows.Media;
using PendulumApp.ViewModel.OpenGLRender;
using PendulumApp.Settings;

namespace PendulumApp.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(OpenGLDispatcher openGLDispatcher, SettingProgram settingProgramData, SettingEMG settingEMGData, SettingACC settingACCData, SettingGY settingGYData) {

            this.OpenGLDispatcher = openGLDispatcher;
            PlayCommand = new DelegateCommand(PlayExecute, CanPlayExecute);
            RecordCommand = new DelegateCommand(RecordExecute, CanRecordExecute);
            CalibrateCommand = new DelegateCommand(CalibrateExecute, CanCalibrateExecute);
            SetAxis1Command = new DelegateCommand(SetAxis1Execute);
            SetAxis2Command = new DelegateCommand(SetAxis2Execute);
            SetAxis3Command = new DelegateCommand(SetAxis3Execute);
            SetAxis4Command = new DelegateCommand(SetAxis4Execute);
            SetAxis5Command = new DelegateCommand(SetAxis5Execute);
            SetAxis6Command = new DelegateCommand(SetAxis6Execute);
            WindowClosing = new DelegateCommand(WindowClosingExecute);

            string[] ports = SerialPort.GetPortNames();
            _comboBoxCOMPorts = new List<string>();
            foreach (string port in ports)
            {
                _comboBoxCOMPorts.Add(port);
                _comboboxSelectedItem = port;
            }

            SettingProgramData = settingProgramData;
            SettingEMGData = settingEMGData;
            SettingACCData = settingACCData;
            SettingGYData = settingGYData;

            openGLDispatcher.setScaleFactors(settingEMGData.Scale, settingACCData.Scale, settingGYData.Scale);
            for(int i = 0; i < 6; i++)
            {
                openGLDispatcher.setAxisValues(i + 1, settingProgramData.YMax[i], settingProgramData.YMin[i]);
            }                       

            ToogleButtonUser1IsChecked = true;
            ToggleButtonAutoZoomIsChecked = false;
        }


        #region BUTTON_PLAY

        private bool _playing = false;
        public bool Playing
        {
            get
            {
                return _playing;
            }
            set
            {
                _playing = value;
                OnPropertyChanged("Playing");
            }
        }

        private string _buttonPlayTooltip;
        public string ButtonPlayTooltip
        {
            get
            {
                return _buttonPlayTooltip ?? "Play";
            }
            set
            {
                _buttonPlayTooltip = value;
                OnPropertyChanged("ButtonPlayTooltip");
            }
        }

        private bool _buttonPlayIsEnabled = true;
        public bool ButtonPlayIsEnabled
        {
            get
            {
                return _buttonPlayIsEnabled;
            }
            set
            {
                _buttonPlayIsEnabled = value;
                if (_buttonPlayIsEnabled == true)
                {
                    PlayImageOpacity = 1.0f;
                }
                else
                {
                    PlayImageOpacity = 0.5f;
                }
                OnPropertyChanged("ButtonPlayIsEnabled");
            }
        }

        private float _playImageOpacity = 1.0F;
        public float PlayImageOpacity
        {
            get
            {
                return _playImageOpacity;
            }
            set
            {
                _playImageOpacity = value;
                OnPropertyChanged("PlayImageOpacity");
            }
        }

        public delegate bool DevicePlayExecute(bool playing);
        public event DevicePlayExecute devicePlayExecute;
        public ICommand PlayCommand { get; set; }
        private void PlayExecute(object obj)
        {

            if (devicePlayExecute(Playing))
            {
                if (Playing)
                {
                    ButtonPlayTooltip = "Play";
                    Playing = false;
                }
                else
                {
                    ButtonPlayTooltip = "Stop playing";
                    Playing = true;
                }

            }

        }
        private bool CanPlayExecute(object obj)
        {
            return true;
        }

        #endregion

        #region BUTTON_RECORD

        private bool _recording = false;
        public bool Recording
        {
            get { return _recording; }
            set
            {
                _recording = value;
                OnPropertyChanged("Recording");
            }
        }

        private string _buttonRecordTooltip;
        public string ButtonRecordTooltip
        {
            get
            {
                return _buttonRecordTooltip ?? "Record";
            }
            set
            {
                _buttonRecordTooltip = value;
                OnPropertyChanged("ButtonRecordTooltip");
            }
        }

        private bool _buttonRecordIsEnabled = false;
        public bool ButtonRecordIsEnabled
        {
            get
            {
                return _buttonRecordIsEnabled;
            }
            set
            {
                _buttonRecordIsEnabled = value;
                if (_buttonRecordIsEnabled == true)
                {
                    RecordImageOpacity = 1.0f;
                }
                else
                {
                    RecordImageOpacity = 0.5f;
                }
                OnPropertyChanged("ButtonRecordIsEnabled");
            }
        }

        private float _recordImageOpacity = 0.5F;
        public float RecordImageOpacity
        {
            get
            {
                return _recordImageOpacity;
            }
            set
            {
                _recordImageOpacity = value;
                OnPropertyChanged("RecordImageOpacity");
            }
        }

        public delegate bool DeviceRecordExecute(bool recording);
        public event DeviceRecordExecute deviceRecordExecute;

        public ICommand RecordCommand { get; set; }
        private void RecordExecute(object obj)
        {
            if (deviceRecordExecute(Recording))          //check if procedure is executed properly
            {
                if (Recording)
                {
                    ButtonRecordTooltip = "Start recording";
                    Recording = false;
                }
                else
                {
                    Recording = true;
                    ButtonRecordTooltip = "Stop recording";
                }
            }
        }
        private bool CanRecordExecute(object obj)
        {
            return true;
        }
        #endregion

        #region BUTTON_CALIBRATE

        private bool _buttonCalibrateIsEnabled = false;
        public bool ButtonCalibrateIsEnabled
        {
            get
            {
                return _buttonCalibrateIsEnabled;
            }
            set
            {
                _buttonCalibrateIsEnabled = value;
                if (_buttonCalibrateIsEnabled == true)
                {
                    CalibrateImageOpacity = 1.0f;
                }
                else
                {
                    CalibrateImageOpacity = 0.5f;
                }
                OnPropertyChanged("ButtonCalibrateIsEnabled");
            }
        }
        private float _calibrateImageOpacity = 0.5F;
        public float CalibrateImageOpacity
        {
            get
            {
                return _calibrateImageOpacity;
            }
            set
            {
                _calibrateImageOpacity = value;
                OnPropertyChanged("CalibrateImageOpacity");
            }
        }
        private string _buttonCalibrateTooltip;
        public string ButtonCalibrateTooltip
        {
            get
            {
                return _buttonCalibrateTooltip ?? "Calibrate";
            }
            set
            {
                _buttonCalibrateTooltip = value;
                OnPropertyChanged("ButtonCalibrateTooltip");
            }
        }

        public delegate bool DeviceCalibrateExecute();
        public event DeviceCalibrateExecute deviceCalibrateExecute;
        public ICommand CalibrateCommand { get; set; }
        private void CalibrateExecute(object obj)
        {
            deviceCalibrateExecute();
        }
        private bool CanCalibrateExecute(object obj)
        {
            return true;
        }
        #endregion

        #region TOGGLE BUTTON AUTO ZOOM
        private string _toggleButtonAutoZoomToolTip = "Auto zoom on";
        public string ToggleButtonAutoZoomToolTip
        {
            get
            {
                return _toggleButtonAutoZoomToolTip;
            }
            set
            {
                _toggleButtonAutoZoomToolTip = value;
                OnPropertyChanged("ToggleButtonAutoZoomToolTip");
            }
        }

        private bool _toggleButtonAutoZoomIsChecked = false;
        public bool ToggleButtonAutoZoomIsChecked
        {
            get
            {
                return _toggleButtonAutoZoomIsChecked;
            }
            set
            {
                _toggleButtonAutoZoomIsChecked = value;
                OnPropertyChanged("ToggleButtonAutoZoomIsChecked");
                if (_toggleButtonAutoZoomIsChecked)
                {
                    for (int i = 1; i <= 6; i++)
                    {
                        OpenGLDispatcher.setAutoZoom(i,true);
                    }
                    ToggleButtonAutoZoomToolTip = "Auto zoom off";
                }
                else
                {
                    for (int i = 1; i <= 6; i++)
                    {
                        OpenGLDispatcher.setAutoZoom(i, false);
                    }
                    ToggleButtonAutoZoomToolTip = "Auto zoom on";
                }
            }
        }
        #endregion

        #region CONNECTION LABEL
        private bool _comConnectionStatus = false;
        public bool COMConnectionStatus {
            get
            {
                return _comConnectionStatus;
            }
            set
            {
                _comConnectionStatus = value;
                if (_comConnectionStatus)
                {
                    COMConnectionLabelText = "COM: Connected";
                    COMConnectionLabelColor = Brushes.LawnGreen;

                    ButtonRecordIsEnabled = true;
                    RecordImageOpacity = 1.0f;
                    ButtonCalibrateIsEnabled = true;
                    CalibrateImageOpacity = 1.0f;
                }
                else
                {
                    COMConnectionLabelText = "COM: Disconnected";
                    COMConnectionLabelColor = Brushes.Red;

                    ButtonRecordIsEnabled = false;
                    RecordImageOpacity = 0.5f;
                    ButtonCalibrateIsEnabled = false;
                    CalibrateImageOpacity = 0.5f;
                }
            }
        }
        private bool _bleConnectionStatus = false;
        public bool BLEConnectionStatus
        {
            get
            {
                return _bleConnectionStatus;
            }
            set
            {
                _bleConnectionStatus = value;
                if (_bleConnectionStatus)
                {
                    BLEConnectionLabelText = "BLE: Connected";
                    BLEConnectionLabelColor = Brushes.LawnGreen;
                }
                else
                {
                    BLEConnectionLabelText = "BLE: Disconnected";
                    BLEConnectionLabelColor = Brushes.Red;
                }
            }
        }

        private Brush _comConnectionLabelColor = Brushes.Red;
        public Brush COMConnectionLabelColor
        {
            get
            {
                return _comConnectionLabelColor;
            }
            set
            {
                _comConnectionLabelColor = value;
                OnPropertyChanged("COMConnectionLabelColor");
            }
        }
        private Brush _bleConnectionLabelColor = Brushes.Red;
        public Brush BLEConnectionLabelColor
        {
            get
            {
                return _bleConnectionLabelColor;
            }
            set
            {
                _bleConnectionLabelColor = value;
                OnPropertyChanged("BLEConnectionLabelColor");
            }
        }
        private string _comConnectionLabelText;
        public string COMConnectionLabelText
        {
            get
            {
                return _comConnectionLabelText ?? "COM: Disconnected";
            }
            set
            {
                _comConnectionLabelText = value;
                OnPropertyChanged("COMConnectionLabelText");
            }
        }
        private string _bleConnectionLabelText;
        public string BLEConnectionLabelText
        {
            get
            {
                return _bleConnectionLabelText ?? "BLE: Disconnected";
            }
            set
            {
                _bleConnectionLabelText = value;
                OnPropertyChanged("BLEConnectionLabelText");
            }
        }
        #endregion

        #region COMBOBOX_PROFILE
        private bool _comboboxCOMPortsIsEnabled = true;
        public bool ComboboxCOMPortsIsEnabled
        {
            get
            {
                return _comboboxCOMPortsIsEnabled;
            }
            set
            {
                _comboboxCOMPortsIsEnabled = value;
                OnPropertyChanged("ComboboxCOMPortsIsEnabled");
            }
        }
        private List<string> _comboBoxCOMPorts;
        public List<string> ComboBoxCOMPorts
        {
            get
            {
                return _comboBoxCOMPorts;
            }
            set
            {
                _comboBoxCOMPorts = value;
                OnPropertyChanged("ComboBoxCOMPorts");
            }
        }
        private String _comboboxSelectedItem;
        public String ComboboxSelectedItem
        {
            get
            {
                return _comboboxSelectedItem;
            }
            set
            {
                _comboboxSelectedItem = value;
                OnPropertyChanged("ComboboxSelectedItem");
            }
        }
        #endregion

        #region ACC/EMG/BAT STATUSES
        private bool _accGy0Status = false;
        public bool AccGy0Status
        {
            get
            {
                return _accGy0Status;
            }
            set
            {
                _accGy0Status = value;
                if (_accGy0Status)
                {
                    AccGy0StatusLabelText = "AccGy0 = OK";
                    AccGy0StatusLabelColor = Brushes.LawnGreen;
                }
                else
                {
                    AccGy0StatusLabelText = "AccGy0 = FAIL";
                    AccGy0StatusLabelColor = Brushes.Red;
                }
            }
        }
        private bool _accGy1Status = false;
        public bool AccGy1Status
        {
            get
            {
                return _accGy1Status;
            }
            set
            {
                _accGy1Status = value;
                if (_accGy1Status)
                {
                    AccGy1StatusLabelText = "AccGy1 = OK";
                    AccGy1StatusLabelColor = Brushes.LawnGreen;
                }
                else
                {
                    AccGy1StatusLabelText = "AccGy1 = FAIL";
                    AccGy1StatusLabelColor = Brushes.Red;
                }
            }
        }
        private bool _emgStatus = false;
        public bool EMGStatus
        {
            get
            {
                return _emgStatus;
            }
            set
            {
                _emgStatus = value;
                if (_emgStatus)
                {
                    EMGStatusLabelText = "EMG = OK";
                    EMGStatusLabelColor = Brushes.LawnGreen;
                }
                else
                {
                    EMGStatusLabelText = "EMG = FAIL";
                    EMGStatusLabelColor = Brushes.Red;
                }
            }
        }

        private Brush _accGy0StatusLabelColor = Brushes.Red;
        public Brush AccGy0StatusLabelColor {
            get
            {
                return _accGy0StatusLabelColor;
            }
            set
            {
                _accGy0StatusLabelColor = value;
                OnPropertyChanged("AccGy0StatusLabelColor");
            }
        }
        private string _accGy0StatusLabelText;
        public string AccGy0StatusLabelText
        {
            get
            {
                return _accGy0StatusLabelText ?? "AccGy0 = FAIL";
            }
            set
            {
                _accGy0StatusLabelText = value;
                OnPropertyChanged("AccGy0StatusLabelText");
            }
        }

        private Brush _accGy1StatusLabelColor = Brushes.Red;
        public Brush AccGy1StatusLabelColor
        {
            get
            {
                return _accGy1StatusLabelColor;
            }
            set
            {
                _accGy1StatusLabelColor = value;
                OnPropertyChanged("AccGy1StatusLabelColor");
            }
        }
        private string _accGy1StatusLabelText;
        public string AccGy1StatusLabelText
        {
            get
            {
                return _accGy1StatusLabelText ?? "AccGy1 = FAIL";
            }
            set
            {
                _accGy1StatusLabelText = value;
                OnPropertyChanged("AccGy1StatusLabelText");
            }
        }

        private Brush _emgStatusLabelColor = Brushes.Red;
        public Brush EMGStatusLabelColor
        {
            get
            {
                return _emgStatusLabelColor;
            }
            set
            {
                _emgStatusLabelColor = value;
                OnPropertyChanged("EMGStatusLabelColor");
            }
        }
        private string _emgStatusLabelText;
        public string EMGStatusLabelText
        {
            get
            {
                return _emgStatusLabelText ?? "EMG = FAIL";
            }
            set
            {
                _emgStatusLabelText = value;
                OnPropertyChanged("EMGStatusLabelText");
            }
        }

        private bool _batteryLowLevel = false;
        public bool BatteryLowLevel
        {
            get
            {
                return _batteryLowLevel;
            }
            set
            {
                _batteryLowLevel = value;
                if (_batteryLowLevel)
                {
                    BatteryStatusLabelColor = Brushes.Red;
                }
                else
                {
                    BatteryStatusLabelColor = Brushes.LawnGreen;
                }
            }
        }
        private Brush _batteryStatusLabelColor = Brushes.Red;
        public Brush BatteryStatusLabelColor
        {
            get
            {
                return _batteryStatusLabelColor;
            }
            set
            {
                _batteryStatusLabelColor = value;
                OnPropertyChanged("BatteryStatusLabelColor");
            }
        }
        private string _batteryStatusTextLabel = "Battery: N/A";
        public string BatteryStatusTextLabel
        {
            get
            {
                return _batteryStatusTextLabel;
            }
            set
            {
                _batteryStatusTextLabel = value;
                OnPropertyChanged("BatteryStatusTextLabel");
            }
        }

        #endregion

        #region OPENGL
        public OpenGLDispatcher OpenGLDispatcher
        {
            get;
            private set;
        }
        #endregion

        #region TOGGLE/TAB BUTTONS
        private bool _toogleButtonUser1IsChecked = true;
        public bool ToogleButtonUser1IsChecked
        {
            get
            {
                return _toogleButtonUser1IsChecked;
            }
            set
            {
                _toogleButtonUser1IsChecked = value;
                if (_toogleButtonUser1IsChecked)
                {
                    ToogleButtonUser2IsChecked = false;
                    ButtonSetAxis1IsEnabled = true;
                    ButtonSetAxis2IsEnabled = true;
                    ButtonSetAxis3IsEnabled = true;
                    ButtonSetAxis4IsEnabled = true;
                    ButtonSetAxis5IsEnabled = false;
                    ButtonSetAxis6IsEnabled = false;

                    LabelDisplay1Tittle = "EMG1 (mV)";
                    LabelDisplay2Tittle = "EMG2 (mV)";
                    LabelDisplay3Tittle = "ACC Thigh/Transverse (m/s^2)";
                    LabelDisplay4Tittle = "ACC Shank/Transverse (m/s^2)";
                    LabelDisplay5Tittle = "";
                    LabelDisplay6Tittle = "";
                    
                    OpenGLDispatcher.enableDisplay(1);                   
                    OpenGLDispatcher.linkDisplay(1, "EMG", 1);
                    OpenGLDispatcher.setAxisValues(1, SettingProgramData.YMax[0], SettingProgramData.YMin[0]);

                    OpenGLDispatcher.enableDisplay(2);                    
                    OpenGLDispatcher.linkDisplay(2, "EMG", 2);
                    OpenGLDispatcher.setAxisValues(2, SettingProgramData.YMax[1], SettingProgramData.YMin[1]);

                    OpenGLDispatcher.enableDisplay(3);                    
                    OpenGLDispatcher.linkDisplay(3, "ACC", 1);         //ACC0z 
                    OpenGLDispatcher.setAxisValues(3, SettingProgramData.YMax[2], SettingProgramData.YMin[2]);

                    OpenGLDispatcher.enableDisplay(4);                    
                    OpenGLDispatcher.linkDisplay(4, "ACC", 2);         //ACC1z
                    OpenGLDispatcher.setAxisValues(4, SettingProgramData.YMax[3], SettingProgramData.YMin[3]);

                    OpenGLDispatcher.disableDisplay(5);
                    OpenGLDispatcher.disableDisplay(6);

                }
                OnPropertyChanged("ToogleButtonUser1IsChecked");
            }
        }
        private bool _toogleButtonUser2IsChecked = false;
        public bool ToogleButtonUser2IsChecked
        {
            get
            {
                return _toogleButtonUser2IsChecked;
            }
            set
            {
                _toogleButtonUser2IsChecked = value;
                if (_toogleButtonUser2IsChecked)
                {
                    ToogleButtonUser1IsChecked = false;
                    ButtonSetAxis1IsEnabled = true;
                    ButtonSetAxis2IsEnabled = true;
                    ButtonSetAxis3IsEnabled = true;
                    ButtonSetAxis4IsEnabled = false;
                    ButtonSetAxis5IsEnabled = false;
                    ButtonSetAxis6IsEnabled = false;

                    LabelDisplay1Tittle = "GY Thigh/Sagital (rad/s)";
                    LabelDisplay2Tittle = "GY Shank/Sagital (rad/s)";
                    LabelDisplay3Tittle = "GY Shank/Transverse (rad/s)";
                    LabelDisplay4Tittle = "";
                    LabelDisplay5Tittle = "";
                    LabelDisplay6Tittle = "";
                   

                    OpenGLDispatcher.enableDisplay(1);
                    OpenGLDispatcher.linkDisplay(1, "GY", 1);          //GY0
                    OpenGLDispatcher.setAxisValues(1, SettingProgramData.YMax[4], SettingProgramData.YMin[4]);

                    OpenGLDispatcher.enableDisplay(2);
                    OpenGLDispatcher.linkDisplay(2, "GY", 2);          //GY0
                    OpenGLDispatcher.setAxisValues(2, SettingProgramData.YMax[5], SettingProgramData.YMin[5]);

                    OpenGLDispatcher.enableDisplay(3);
                    OpenGLDispatcher.linkDisplay(3, "GY", 3);          //GY1
                    OpenGLDispatcher.setAxisValues(3, SettingProgramData.YMax[6], SettingProgramData.YMin[6]);

                    OpenGLDispatcher.disableDisplay(4);
                    OpenGLDispatcher.disableDisplay(5);
                    OpenGLDispatcher.disableDisplay(6);

                }
                OnPropertyChanged("ToogleButtonUser2IsChecked");
            }
        }
        #endregion

        #region SET_AXIS BUTTONS 
        private bool _buttonSetAxis1IsVisible = true;
        public bool ButtonSetAxis1IsVisible
        {
            get
            {
                return _buttonSetAxis1IsVisible;
            }
            set
            {
                _buttonSetAxis1IsVisible = value;
                OnPropertyChanged("ButtonSetAxis1IsVisible");
            }
        }
        private bool _buttonSetAxis1IsEnabled = true;
        public bool ButtonSetAxis1IsEnabled
        {
            get
            {
                return _buttonSetAxis1IsEnabled;
            }
            set
            {
                _buttonSetAxis1IsEnabled = value;
                if (_buttonSetAxis1IsEnabled == true)
                {
                    ButtonSetAxis1IsVisible = true;
                }
                else
                {
                    ButtonSetAxis1IsVisible = false;
                }
                OnPropertyChanged("ButtonSetAxis1IsEnabled");
            }
        }
        private bool _buttonSetAxis2IsVisible = true;
        public bool ButtonSetAxis2IsVisible
        {
            get
            {
                return _buttonSetAxis2IsVisible;
            }
            set
            {
                _buttonSetAxis2IsVisible = value;
                OnPropertyChanged("ButtonSetAxis2IsVisible");
            }
        }
        private bool _buttonSetAxis2IsEnabled = true;
        public bool ButtonSetAxis2IsEnabled
        {
            get
            {
                return _buttonSetAxis2IsEnabled;
            }
            set
            {
                _buttonSetAxis2IsEnabled = value;
                if (_buttonSetAxis2IsEnabled == true)
                {
                    ButtonSetAxis2IsVisible = true;
                }
                else
                {
                    ButtonSetAxis2IsVisible = false;
                }
                OnPropertyChanged("ButtonSetAxis2IsEnabled");
            }
        }
        private bool _buttonSetAxis3IsVisible = true;
        public bool ButtonSetAxis3IsVisible
        {
            get
            {
                return _buttonSetAxis3IsVisible;
            }
            set
            {
                _buttonSetAxis3IsVisible = value;
                OnPropertyChanged("ButtonSetAxis3IsVisible");
            }
        }
        private bool _buttonSetAxis3IsEnabled = true;
        public bool ButtonSetAxis3IsEnabled
        {
            get
            {
                return _buttonSetAxis3IsEnabled;
            }
            set
            {
                _buttonSetAxis3IsEnabled = value;
                if (_buttonSetAxis3IsEnabled == true)
                {
                    ButtonSetAxis3IsVisible = true;
                }
                else
                {
                    ButtonSetAxis3IsVisible = false;
                }
                OnPropertyChanged("ButtonSetAxis3IsEnabled");
            }
        }
        private bool _buttonSetAxis4IsVisible = true;
        public bool ButtonSetAxis4IsVisible
        {
            get
            {
                return _buttonSetAxis4IsVisible;
            }
            set
            {
                _buttonSetAxis4IsVisible = value;
                OnPropertyChanged("ButtonSetAxis4IsVisible");
            }
        }
        private bool _buttonSetAxis4IsEnabled = true;
        public bool ButtonSetAxis4IsEnabled
        {
            get
            {
                return _buttonSetAxis4IsEnabled;
            }
            set
            {
                _buttonSetAxis4IsEnabled = value;
                if (_buttonSetAxis4IsEnabled == true)
                {
                    ButtonSetAxis4IsVisible = true;
                }
                else
                {
                    ButtonSetAxis4IsVisible = false;
                }
                OnPropertyChanged("ButtonSetAxis4IsEnabled");
            }
        }
        private bool _buttonSetAxis5IsVisible = true;
        public bool ButtonSetAxis5IsVisible
        {
            get
            {
                return _buttonSetAxis5IsVisible;
            }
            set
            {
                _buttonSetAxis5IsVisible = value;
                OnPropertyChanged("ButtonSetAxis5IsVisible");
            }
        }
        private bool _buttonSetAxis5IsEnabled = true;
        public bool ButtonSetAxis5IsEnabled
        {
            get
            {
                return _buttonSetAxis5IsEnabled;
            }
            set
            {
                _buttonSetAxis5IsEnabled = value;
                if (_buttonSetAxis5IsEnabled == true)
                {
                    ButtonSetAxis5IsVisible = true;
                }
                else
                {
                    ButtonSetAxis5IsVisible = false;
                }
                OnPropertyChanged("ButtonSetAxis5IsEnabled");
            }
        }
        private bool _buttonSetAxis6IsVisible = true;
        public bool ButtonSetAxis6IsVisible
        {
            get
            {
                return _buttonSetAxis6IsVisible;
            }
            set
            {
                _buttonSetAxis6IsVisible = value;
                OnPropertyChanged("ButtonSetAxis6IsVisible");
            }
        }
        private bool _buttonSetAxis6IsEnabled = true;
        public bool ButtonSetAxis6IsEnabled
        {
            get
            {
                return _buttonSetAxis6IsEnabled;
            }
            set
            {
                _buttonSetAxis6IsEnabled = value;
                if (_buttonSetAxis6IsEnabled == true)
                {
                    ButtonSetAxis6IsVisible = true;
                }
                else
                {
                    ButtonSetAxis6IsVisible = false;
                }
                OnPropertyChanged("ButtonSetAxis6IsEnabled");
            }
        }

        public ICommand SetAxis1Command { get; set; }
        private void SetAxis1Execute(object obj)
        {
            Dialogs.DialogService.DialogViewModelBase vm = new Dialogs.DialogSetAxis.DialogSetAxisViewModel();
            Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(vm);

            Dialogs.DialogSetAxis.DialogResultSetAxis resultSetAxis = result as Dialogs.DialogSetAxis.DialogResultSetAxis;
            if (resultSetAxis.OKClicked)
            {
                OpenGLDispatcher.setAxisValues(1, resultSetAxis.YMax, resultSetAxis.YMin);
            }
            if (ToogleButtonUser1IsChecked)
            {
                SettingProgramData.YMax[0] = resultSetAxis.YMax;
                SettingProgramData.YMin[0] = resultSetAxis.YMin;
            }
            else
            {
                SettingProgramData.YMax[4] = resultSetAxis.YMax;
                SettingProgramData.YMin[4] = resultSetAxis.YMin;
            }
        }
        public ICommand SetAxis2Command { get; set; }
        private void SetAxis2Execute(object obj)
        {
            Dialogs.DialogService.DialogViewModelBase vm = new Dialogs.DialogSetAxis.DialogSetAxisViewModel();
            Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(vm);
            Dialogs.DialogSetAxis.DialogResultSetAxis resultSetAxis = result as Dialogs.DialogSetAxis.DialogResultSetAxis;

            if (resultSetAxis.OKClicked)
            {
                OpenGLDispatcher.setAxisValues(2, resultSetAxis.YMax, resultSetAxis.YMin);
            }
            if (ToogleButtonUser1IsChecked)
            {
                SettingProgramData.YMax[1] = resultSetAxis.YMax;
                SettingProgramData.YMin[1] = resultSetAxis.YMin;
            }
            else
            {
                SettingProgramData.YMax[5] = resultSetAxis.YMax;
                SettingProgramData.YMin[5] = resultSetAxis.YMin;
            }
            
        }
        public ICommand SetAxis3Command { get; set; }
        private void SetAxis3Execute(object obj)
        {
            Dialogs.DialogService.DialogViewModelBase vm = new Dialogs.DialogSetAxis.DialogSetAxisViewModel();
            Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(vm);
            Dialogs.DialogSetAxis.DialogResultSetAxis resultSetAxis = result as Dialogs.DialogSetAxis.DialogResultSetAxis;

            if (resultSetAxis.OKClicked)
            {
                OpenGLDispatcher.setAxisValues(3, resultSetAxis.YMax, resultSetAxis.YMin);
            }
            if (ToogleButtonUser1IsChecked)
            {
                SettingProgramData.YMax[2] = resultSetAxis.YMax;
                SettingProgramData.YMin[2] = resultSetAxis.YMin;
            }
            else
            {
                SettingProgramData.YMax[6] = resultSetAxis.YMax;
                SettingProgramData.YMin[6] = resultSetAxis.YMin;
            }
            
        }
        public ICommand SetAxis4Command { get; set; }
        private void SetAxis4Execute(object obj)
        {
            Dialogs.DialogService.DialogViewModelBase vm = new Dialogs.DialogSetAxis.DialogSetAxisViewModel();
            Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(vm);
            Dialogs.DialogSetAxis.DialogResultSetAxis resultSetAxis = result as Dialogs.DialogSetAxis.DialogResultSetAxis;

            if (resultSetAxis.OKClicked)
            {
                OpenGLDispatcher.setAxisValues(4, resultSetAxis.YMax, resultSetAxis.YMin);
            }
            SettingProgramData.YMax[3] = resultSetAxis.YMax;
            SettingProgramData.YMin[3] = resultSetAxis.YMin;
        }
        public ICommand SetAxis5Command { get; set; }
        private void SetAxis5Execute(object obj)
        {
            Dialogs.DialogService.DialogViewModelBase vm = new Dialogs.DialogSetAxis.DialogSetAxisViewModel();
            Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(vm);
            Dialogs.DialogSetAxis.DialogResultSetAxis resultSetAxis = result as Dialogs.DialogSetAxis.DialogResultSetAxis;

            if (resultSetAxis.OKClicked)
            {
                OpenGLDispatcher.setAxisValues(5, resultSetAxis.YMax, resultSetAxis.YMin);
            }
            SettingProgramData.YMax[4] = resultSetAxis.YMax;
            SettingProgramData.YMin[4] = resultSetAxis.YMin;
        }
        public ICommand SetAxis6Command { get; set; }
        private void SetAxis6Execute(object obj)
        {
            Dialogs.DialogService.DialogViewModelBase vm = new Dialogs.DialogSetAxis.DialogSetAxisViewModel();
            Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(vm);
            Dialogs.DialogSetAxis.DialogResultSetAxis resultSetAxis = result as Dialogs.DialogSetAxis.DialogResultSetAxis;

            if (resultSetAxis.OKClicked)
            {
                OpenGLDispatcher.setAxisValues(6, resultSetAxis.YMax, resultSetAxis.YMin);
            }
            SettingProgramData.YMax[5] = resultSetAxis.YMax;
            SettingProgramData.YMin[5] = resultSetAxis.YMin;

        }


        #endregion

        #region DISPLAY_TITLES
        private string _labelDisplay1Tittle;
        public string LabelDisplay1Tittle {
            get
            {
                return _labelDisplay1Tittle;
            }
            set
            {
                _labelDisplay1Tittle = value;
                OnPropertyChanged("LabelDisplay1Tittle");
            }
        }
        private string _labelDisplay2Tittle;
        public string LabelDisplay2Tittle
        {
            get
            {
                return _labelDisplay2Tittle;
            }
            set
            {
                _labelDisplay2Tittle = value;
                OnPropertyChanged("LabelDisplay2Tittle");
            }
        }
        private string _labelDisplay3Tittle;
        public string LabelDisplay3Tittle
        {
            get
            {
                return _labelDisplay3Tittle;
            }
            set
            {
                _labelDisplay3Tittle = value;
                OnPropertyChanged("LabelDisplay3Tittle");
            }
        }
        private string _labelDisplay4Tittle;
        public string LabelDisplay4Tittle
        {
            get
            {
                return _labelDisplay4Tittle;
            }
            set
            {
                _labelDisplay4Tittle = value;
                OnPropertyChanged("LabelDisplay4Tittle");
            }
        }
        private string _labelDisplay5Tittle;
        public string LabelDisplay5Tittle
        {
            get
            {
                return _labelDisplay5Tittle;
            }
            set
            {
                _labelDisplay5Tittle = value;
                OnPropertyChanged("LabelDisplay5Tittle");
            }
        }
        private string _labelDisplay6Tittle;
        public string LabelDisplay6Tittle
        {
            get
            {
                return _labelDisplay6Tittle;
            }
            set
            {
                _labelDisplay6Tittle = value;
                OnPropertyChanged("LabelDisplay6Tittle");
            }
        }

        #endregion

        #region SETTING_DATA
        public SettingProgram SettingProgramData { get; set; }
        public SettingEMG SettingEMGData { get; set; }
        public SettingACC SettingACCData { get; set; }
        public SettingGY SettingGYData { get; set; }
        #endregion

        #region WINDOW_CLOSING
        public event EventHandler WindowClosingEventHandler;
        public ICommand WindowClosing { get; private set; }
        private void WindowClosingExecute(object obj)
        {
            
            SettingService.StoreSetting(SettingProgramData, SettingEMGData, SettingACCData, SettingGYData);
            WindowClosingEventHandler(null, null);
            OpenGLDispatcher.dispose();
            Log log = new Log();
            log.LogMessageToFile("Program closed!!!");
            
        }
        #endregion
    }
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public DelegateCommand(Action<object> execute,
                       Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
