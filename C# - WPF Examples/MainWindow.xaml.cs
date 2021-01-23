using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Oven_App_2.Windows;
using Oven_App_2.UserControls;
using System.IO;
using System.Collections.ObjectModel;
using Oven_App_2.Classes;
using TP = Oven_App_2.Classes.TemperatureProfile;

namespace Oven_App_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private SettingsWindow _settingsWindow = new SettingsWindow();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Updates the UI when a setting is changed
        /// </summary>
        /// <param name="settingName"></param>
        private void Instance_OnSettingChanged(SettingsWindow.ChangedSetting settingName)
        {
            var _profile = new TemperatureProfile();
            switch (settingName)
            {
                case SettingsWindow.ChangedSetting.SpectroIPs:
                    SpectroListPanel.Children.Clear();
                    ScriptHandler.GlobalScriptHandler.AddAllSpectros();
                    foreach (var spectro in ScriptHandler.GlobalScriptHandler.Spectros)
                        SpectroListPanel.Children.Add(new SpectroUIElement(spectro));
                    break;
                case SettingsWindow.ChangedSetting.TempCalSelection:
                    TempCalPreviewGraph.Visibility = Properties.Settings.Default.TempCalSelected ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case SettingsWindow.ChangedSetting.LampCompSelection:
                    LampCompPreviewGraph.Visibility = Properties.Settings.Default.LampCompSelected ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case SettingsWindow.ChangedSetting.TempCompSelection:
                    TempCompPreviewGraph.Visibility = Properties.Settings.Default.TempCompSelected ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case SettingsWindow.ChangedSetting.TempCheckSelection:
                    TempCheckPreviewGraph.Visibility = Properties.Settings.Default.TempCheckSelected ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case SettingsWindow.ChangedSetting.DriftCheckSelection:
                    DriftCheckPreviewGraph.Visibility = Properties.Settings.Default.DriftCheckSelected ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case SettingsWindow.ChangedSetting.CustomSelection:
                    CustomPreviewGraph.Visibility = Properties.Settings.Default.CustomSelected ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case SettingsWindow.ChangedSetting.TempCalProfile:
                    _profile.Load(Properties.Settings.Default.TempCalProfileURI);
                    TempCalPreviewGraph.SetGraphData(_profile.GetGraphData());
                    break;
                case SettingsWindow.ChangedSetting.LampCompProfile:
                    _profile.Load(Properties.Settings.Default.LampCompProfileURI);
                    LampCompPreviewGraph.SetGraphData(_profile.GetGraphData());
                    break;
                case SettingsWindow.ChangedSetting.TempCompProfile:
                    _profile.Load(Properties.Settings.Default.TempCompProfileURI);
                    TempCompPreviewGraph.SetGraphData(_profile.GetGraphData());
                    break;
                case SettingsWindow.ChangedSetting.TempCheckProfile:
                    _profile.Load(Properties.Settings.Default.TempCheckProfileURI);
                    TempCheckPreviewGraph.SetGraphData(_profile.GetGraphData());
                    break;
                case SettingsWindow.ChangedSetting.DriftCheckProfile:
                    _profile.Load(Properties.Settings.Default.DriftCheckProfileURI);
                    DriftCheckPreviewGraph.SetGraphData(_profile.GetGraphData());
                    break;
                case SettingsWindow.ChangedSetting.CustomProfile:
                    _profile.Load(Properties.Settings.Default.CustomProfileURI);
                    CustomPreviewGraph.SetGraphData(_profile.GetGraphData());
                    break;
                case SettingsWindow.ChangedSetting.All:
                    foreach (int i in Enum.GetValues(typeof(SettingsWindow.ChangedSetting)))
                        if (i != -1)
                            Instance_OnSettingChanged((SettingsWindow.ChangedSetting)i);
                    break;

            }


            if (PreviewGrid.Children.OfType<UIElement>().Count(c => c.Visibility == Visibility.Visible) > 3)
                PreviewGrid.Rows = 2;
            else
                PreviewGrid.Rows = 1;
        }

        /// <summary>
        /// Updates the UI when the oven connection times out
        /// </summary>
        private void GlobalWatlow_WatlowTimedOut()
        {
            Dispatcher.Invoke(() => { TimedOutText = $"Timeout on {Watlow.GlobalWatlow.GetPortName()}"; });
        }

        /// <summary>
        /// Updates the graph and time display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlobalTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (GlobalTimer.Instance.TimeRemaining == TimeSpan.Zero)
                    TimeRemaining = "";
                else
                    TimeRemaining = $"Time Remaining: {GlobalTimer.Instance.TimeRemaining.ToString()}";

                var dataArray = _currentProfile.GetGraphData();

                //Temperature data
                dataArray[3] = new Graph.GraphPointSet()
                {
                    Points = new ObservableCollection<TemperatureProfile.ObservablePoint>
                    {
                        new TemperatureProfile.ObservablePoint(_currentProfile.ProfileMinTime.TotalMinutes, Watlow.GlobalWatlow.LastTemperature),
                        new TemperatureProfile.ObservablePoint(GlobalTimer.Instance.TimeElapsed.TotalMinutes, Watlow.GlobalWatlow.LastTemperature),
                        new TemperatureProfile.ObservablePoint(GlobalTimer.Instance.TimeElapsed.TotalMinutes, _currentProfile.ProfileMinTemperature)
                    },
                    LineColor = Brushes.Blue,
                    StrokeWeight = 2
                };

                //Title
                CurrentStepGraph.Title = _currentProfile.Name;

                //Update
                CurrentStepGraph.SetGraphData(dataArray);
            });
        }

        //Closes the settings window
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _settingsWindow.ApplicationClosing = true;
            _settingsWindow.Close();
            foreach (var spectro in ScriptHandler.GlobalScriptHandler.Spectros)
                if (spectro != null)
                    spectro.Device.disconnect();
            Watlow.GlobalWatlow.ClosePort();
        }

        //Handles the settings window
        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            _settingsWindow.Show();
            _settingsWindow.Focus();
        }

        //Loads in the spectros
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _settingsWindow.OnSettingChanged += Instance_OnSettingChanged;
            GlobalTimer.Instance.Elapsed += GlobalTimer_Elapsed;

            ScriptHandler.GlobalScriptHandler.StepStarted += GlobalScriptHandler_StepStarted;
            ScriptHandler.GlobalScriptHandler.StatusChanged += GlobalScriptHandler_StatusChanged;

            Watlow.GlobalWatlow.PortClosed += GlobalWatlow_PortClosed;
            Watlow.GlobalWatlow.PortOpened += GlobalWatlow_PortOpened;
            Watlow.GlobalWatlow.WatlowTimedOut += GlobalWatlow_WatlowTimedOut;
            Watlow.GlobalWatlow.CommandSent += GlobalWatlow_CommandSent;
            Watlow.GlobalWatlow.ResponseRecieved += GlobalWatlow_ResponseRecieved;
            Watlow.GlobalWatlow.StatusChanged += GlobalWatlow_StatusChanged;

            Instance_OnSettingChanged(SettingsWindow.ChangedSetting.All);
        }

        //Clears the logged responses from watlow
        private void ClearWatlowConversation(object sender, RoutedEventArgs e)
        {
            WatlowConversation = "";
        }

        //Connects the spectros
        private void ConnectSpectro(object sender, MouseButtonEventArgs e)
        {
            foreach (var spectro in ScriptHandler.GlobalScriptHandler.Spectros)
                spectro.ConnectAsync();
        }

        //Opens or closes the Oven port
        private void ConnectOven(object sender, MouseButtonEventArgs e)
        {
            if (Watlow.GlobalWatlow.IsOpen())
                Watlow.GlobalWatlow.ClosePort();
            else
            {
                OvenConnectTextblock.IsEnabled = false;
                Watlow.GlobalWatlow.OpenPort();
            }
        }

        //Handles the oven connecting
        private void GlobalWatlow_PortOpened(bool success)
        {
            Dispatcher.Invoke(() =>
            {
                OvenConnectTextblock.IsEnabled = true;
                if (success)
                    OvenConnectTextblock.Text = "Disconnect Watlow Series 942";
            });
        }

        //Handles the oven closing
        private void GlobalWatlow_PortClosed()
        {
            Dispatcher.Invoke(() =>
            {
                OvenConnectTextblock.Text = "Connect Watlow Series 942";
                CurrentTemperature = "";
                CurrentMode = "";
                CurrentError1 = "";
                CurrentError2 = "";
                TimedOutText = "";
            });
        }

        private void GlobalWatlow_CommandSent(string command)
        {
            LastCommandSent = $"Sent: {command}";
            WatlowConversation += $"S> {command}\r";
        }

        private void GlobalWatlow_ResponseRecieved(string response)
        {
            LastResponseRecieved = $"Recieved: {response}";
            WatlowConversation += $"R> {response}\r";
        }

        private void GlobalWatlow_StatusChanged(string status, Watlow.ResponseType type)
        {
            switch (type)
            {
                case Watlow.ResponseType.ERR:
                    CurrentError1 = $"Error 1: {status}";
                    TimedOutText = "";
                    break;
                case Watlow.ResponseType.ER2:
                    CurrentError2 = $"Error 2: {status}";
                    TimedOutText = "";
                    break;
                case Watlow.ResponseType.MODE:
                    CurrentMode = $"Mode: {status}";
                    TimedOutText = "";
                    break;
                case Watlow.ResponseType.TEMPERATURE:
                    CurrentTemperature = $"{status} °C";
                    TimedOutText = "";
                    break;
                default:
                    break;
            }
        }

        #region Bound Properties

        private string _timedOutText = "";
        public string TimedOutText
        {
            get { return _timedOutText; }
            set { Set(ref _timedOutText, value); }
        }

        private string _currentStepName = "";
        public string CurrentStepName
        {
            get { return _currentStepName; }
            set { Set(ref _currentStepName, value); }
        }

        private string _status = "";
        public string Status
        {
            get { return _status; }
            set { Set(ref _status, value); }
        }

        private string _lastCommandSent = "";
        public string LastCommandSent
        {
            get { return _lastCommandSent; }
            set { Set(ref _lastCommandSent, value); }
        }

        private string _lastResponseRecieved = "";
        public string LastResponseRecieved
        {
            get { return _lastResponseRecieved; }
            set { Set(ref _lastResponseRecieved, value); }
        }

        private string _watlowConversation = "";
        public string WatlowConversation
        {
            get { return _watlowConversation; }
            set { Set(ref _watlowConversation, value); }
        }

        private string _timeRemaining = "";
        public string TimeRemaining
        {
            get { return _timeRemaining; }
            set { Set(ref _timeRemaining, value); }
        }

        private string _currentTemperature = "";
        public string CurrentTemperature
        {
            get { return _currentTemperature; }
            set { Set(ref _currentTemperature, value); }
        }

        private string _currentMode = "";
        public string CurrentMode
        {
            get { return _currentMode; }
            set { Set(ref _currentMode, value); }
        }

        private string _currentError1 = "";
        public string CurrentError1
        {
            get { return _currentError1; }
            set { Set(ref _currentError1, value); }
        }

        private string _currentError2 = "";
        public string CurrentError2
        {
            get { return _currentError2; }
            set { Set(ref _currentError2, value); }
        }

        #endregion

        #region Script Handler

        private TemperatureProfile _currentProfile = new TemperatureProfile();

        //Updates the UI to reflect the current step
        private void GlobalScriptHandler_StepStarted(ScriptHandler.Step step)
        {
            Dispatcher.Invoke(() =>
            {
                RunScriptsButton.IsEnabled = false;
                StopScriptsButton.IsEnabled = true;
                CurrentStepName = $"Running {step}";
                PreviewGrid.Visibility = Visibility.Collapsed;
                CurrentStepGraph.Visibility = Visibility.Visible;
            });

            switch (step)
            {
                case ScriptHandler.Step.TempCal:
                    Dispatcher.Invoke(() =>
                    {
                        _currentProfile = TP.GetProfile(Properties.Settings.Default.TempCalProfileURI);

                        _currentProfile.Name = "TempCal";

                        CurrentStepGraph.Title = _currentProfile.Name;
                        CurrentStepGraph.SetGraphData(_currentProfile.GetGraphData());
                    });
                    break;
                case ScriptHandler.Step.LampComp:
                    Dispatcher.Invoke(() =>
                    {
                        _currentProfile = TP.GetProfile(Properties.Settings.Default.LampCompProfileURI);

                        _currentProfile.Name = "Lamp Comp";

                        CurrentStepGraph.Title = _currentProfile.Name;
                        CurrentStepGraph.SetGraphData(_currentProfile.GetGraphData());
                    });
                    break;
                case ScriptHandler.Step.TempComp:
                    Dispatcher.Invoke(() =>
                    {
                        _currentProfile = TP.GetProfile(Properties.Settings.Default.TempCompProfileURI);

                        _currentProfile.Name = "Temp Comp";

                        CurrentStepGraph.Title = _currentProfile.Name;
                        CurrentStepGraph.SetGraphData(_currentProfile.GetGraphData());
                    });
                    break;
                case ScriptHandler.Step.TempCheck:
                    Dispatcher.Invoke(() =>
                    {
                        _currentProfile = TP.GetProfile(Properties.Settings.Default.TempCheckProfileURI);

                        _currentProfile.Name = "Temp Check";

                        CurrentStepGraph.Title = _currentProfile.Name;
                        CurrentStepGraph.SetGraphData(_currentProfile.GetGraphData());
                    });
                    break;
                case ScriptHandler.Step.DriftCheck:
                    Dispatcher.Invoke(() =>
                    {
                        _currentProfile = TP.GetProfile(Properties.Settings.Default.DriftCheckProfileURI);

                        _currentProfile.Name = "Drift Check";

                        CurrentStepGraph.Title = _currentProfile.Name;
                        CurrentStepGraph.SetGraphData(_currentProfile.GetGraphData());
                    });
                    break;
                case ScriptHandler.Step.Custom:
                    Dispatcher.Invoke(() =>
                    {
                        _currentProfile = TP.GetProfile(Properties.Settings.Default.CustomProfileURI);

                        _currentProfile.Name = "Custom";

                        CurrentStepGraph.Title = _currentProfile.Name;
                        CurrentStepGraph.SetGraphData(_currentProfile.GetGraphData());
                    });
                    break;
            }
        }

        //Updates the UI when theres a status change
        private void GlobalScriptHandler_StatusChanged(ScriptHandler.Status status, string args = "")
        {
            Dispatcher.Invoke(() =>
            {
                switch (status)
                {
                    case ScriptHandler.Status.CheckingOvenConnection:
                        Status = "Checking oven connection";
                        break;
                    case ScriptHandler.Status.CheckingSpectroConnection:
                        Status = "Checking spectro connection";
                        break;
                    case ScriptHandler.Status.SendingOvenProfile:
                        Status = "Sending profile to oven";
                        break;
                    case ScriptHandler.Status.SettingUpScript:
                        Status = "Setting up scripts";
                        break;
                    case ScriptHandler.Status.WaitingToStartScript:
                        Status = "Waiting to start script";
                        break;
                    case ScriptHandler.Status.RunningScript:
                        Status = "Running script";
                        break;
                    case ScriptHandler.Status.WaitingForProfileEnd:
                        Status = "Waiting for profile to end";
                        break;
                    case ScriptHandler.Status.Cancled:
                        RunScriptsButton.IsEnabled = true;
                        StopScriptsButton.IsEnabled = false;
                        TimeRemaining = "";
                        Status = "Script Canceled";
                        PreviewGrid.Visibility = Visibility.Visible;
                        CurrentStepGraph.Visibility = Visibility.Collapsed;
                        break;
                    case ScriptHandler.Status.Finished:
                        RunScriptsButton.IsEnabled = true;
                        StopScriptsButton.IsEnabled = false;
                        TimeRemaining = "";
                        Status = "Finished";
                        PreviewGrid.Visibility = Visibility.Visible;
                        CurrentStepGraph.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        break;
                }
            });
        }


        //Handles the Run button press
        private void RunTests(object sender, RoutedEventArgs e)
        {
            ScriptHandler.GlobalScriptHandler.RunAll();
            RunScriptsButton.IsEnabled = false;
        }

        //Handles the Stop button press
        private void StopTests(object sender, RoutedEventArgs e)
        {
            ScriptHandler.GlobalScriptHandler.CancleAll();
        }

        #endregion

        #region Implements INotifyPropertyChanged
        protected bool Set<t>(ref t field, t value, [CallerMemberName]string propertyName = "")
        {
            if (field == null || EqualityComparer<t>.Default.Equals(field, value)) { return false; }
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        private void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
