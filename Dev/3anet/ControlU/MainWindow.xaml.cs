using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace ControlU
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public Visibility ATDIVis
        {
            get { return _ATDIVis; }
            set { _ATDIVis = value; OnPropertyChanged("ATDIVis"); }
        }
        Visibility _ATDIVis = Visibility.Visible;

        public delegate void ReadyToShowDelegate(object sender, EventArgs args);
        public event ReadyToShowDelegate ReadyToShow;
        private DispatcherTimer timer;


        string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static Helpers.ExeptionProcessing exp = App.exp;
        public static Equipment.IdentificationData IdfData = new Equipment.IdentificationData();
        public static Equipment.Analyzer An = new Equipment.Analyzer();
        public static Equipment.RsReceiver_v2 Rcvr = new Equipment.RsReceiver_v2();
        public static Equipment.TSMxReceiver tsmx = new Equipment.TSMxReceiver();
        public static Equipment.SignalHound SHReceiver = new Equipment.SignalHound();
        public static Equipment.GPSNMEA gps = new Equipment.GPSNMEA();
        public static Equipment.RCRomes RCR = new Equipment.RCRomes();
        public static Helpers.Helper help = new Helpers.Helper();
        public static DB.NpgsqlDB_v2 db_v2 = new DB.NpgsqlDB_v2();
        public static DB.MyATDI atdi = new DB.MyATDI();
        public static Equipment.SomeMeasData amd = new Equipment.SomeMeasData();

        public static Equipment.RBS6K rbs = new Equipment.RBS6K() { };

        #region Controls
        public bool GlobalUC = false;
        public Controls.SettingsControl SetCtrl;

        #endregion
        #region Message
        private ObservableRangeCollection<string> _AllMessages = new ObservableRangeCollection<string>();
        public ObservableRangeCollection<string> AllMessages
        {
            get { return _AllMessages; }
            set { _AllMessages = value; OnPropertyChanged("AllMessages"); }
        }
        private string _Message = "";
        public string Message
        {
            get { return _Message; }
            set
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    _Message = value;
                    AllMessages.Add(_Message);
                    Controls.Messages.Message msg = new Controls.Messages.Message(_Message);
                    ShowMessages_SP.Children.Add(msg);
                    OnPropertyChanged("Message");
                });

            }
        }
        #endregion
        #region Error Mes

        System.Timers.Timer ErrMesTmr;
        private bool _VisibilityErrMes = false;
        public bool VisibilityErrMes
        {
            get { return _VisibilityErrMes; }
            set
            {
                if (_VisibilityErrMes != value)
                {
                    _VisibilityErrMes = value;
                    OnPropertyChanged("VisibilityErrMes");
                    ErrMesTmr = new System.Timers.Timer(5000);

                    ErrMesTmr.AutoReset = false;
                    ErrMesTmr.Elapsed += ErrMesAutoClose;
                    ErrMesTmr.Enabled = true;

                    //ErrMesTmr.Start();
                }
            }
        }
        private void ErrMesAutoClose(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (VisibilityErrMes == true)
            { VisibilityErrMes = false; ErrMesTmr.Elapsed -= ErrMesAutoClose; ErrMesTmr.Stop(); }
        }
        private bool _VisibilityAllErrMes = false;
        public bool VisibilityAllErrMes
        {
            get { return _VisibilityAllErrMes; }
            set { if (_VisibilityAllErrMes != value) { _VisibilityAllErrMes = value; OnPropertyChanged("VisibilityAllErrMes"); } }
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            SetStyleColor();
            #region Lang
            App.Language = new CultureInfo(App.Sett.GlogalApps_Settings.UILanguage);

            App.LanguageChanged += LanguageChanged;
            CultureInfo currLang = App.Language;
            //Заполняем меню смены языка:
            Menu_language.Items.Clear();
            foreach (var lang in App.Languages)
            {
                MenuItem menuLang = new MenuItem();
                menuLang.Header = lang.DisplayName;
                menuLang.Tag = lang;
                menuLang.IsChecked = lang.Equals(currLang);
                menuLang.Click += ChangeLanguageClick;
                Menu_language.Items.Add(menuLang);
            }
            #endregion
            #region InitializeClasses

            db_v2.Load();

            #endregion
            SelectedUser_Gui.DataContext = App.Sett.UsersApps_Settings;
            MenuDevice_GridGui.DataContext = gps;
            ErrMesBorder.DataContext = this;
            AllErrMesBorder.DataContext = this;
            ErrorLogs_ic.DataContext = exp;
            Global_TabControl.DataContext = App.Sett;
            Receiver_Grid.DataContext = Rcvr;
            exp.PropertyChanged += DrawExceptionText_PropertyChanged;
            db_v2.PropertyChanged += DB_PropertyChanged;
            StartApp();

            //RBS_TabItem.DataContext = rbs;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

        }
        #region мои обработчики пнопок закрытия ресайза и т.д.
        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            //System.Windows.MessageBox.Show("");
        }

        private void buttonRestore_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); //Application.Current.Shutdown();
        }

        #region максимизация окна (костыли)

        private bool mRestoreIfMove = false;
        void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr mWindowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(mWindowHandle).AddHook(new HwndSourceHook(WindowProc));
        }

        private static System.IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    break;
            }
            return IntPtr.Zero;
        }

        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {
            POINT lMousePosition;
            GetCursorPos(out lMousePosition);

            IntPtr lPrimaryScreen = MonitorFromPoint(new POINT(0, 0), MonitorOptions.MONITOR_DEFAULTTOPRIMARY);
            MONITORINFO lPrimaryScreenInfo = new MONITORINFO();
            if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
            {
                return;
            }
            IntPtr lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);

            MINMAXINFO lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            if (lPrimaryScreen.Equals(lCurrentScreen) == true)
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcWork.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcWork.Right - lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcWork.Bottom - lPrimaryScreenInfo.rcWork.Top;
            }
            else
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcMonitor.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcMonitor.Right - lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcMonitor.Bottom - lPrimaryScreenInfo.rcMonitor.Top;
            }

            Marshal.StructureToPtr(lMmi, lParam, true);
        }

        private void SwitchWindowState()
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    {
                        WindowState = WindowState.Maximized;
                        break;
                    }
                case WindowState.Maximized:
                    {
                        WindowState = WindowState.Normal;
                        break;
                    }
            }
        }

        private void TitleBar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if ((ResizeMode == ResizeMode.CanResize) || (ResizeMode == ResizeMode.CanResizeWithGrip))
                {
                    SwitchWindowState();
                }
                return;
            }
            else if (WindowState == WindowState.Maximized)
            {
                mRestoreIfMove = true;
                return;
            }
            DragMove();
        }

        private void TitleBar_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mRestoreIfMove = false;
        }

        private void TitleBar_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (mRestoreIfMove)
            {
                mRestoreIfMove = false;

                double percentHorizontal = e.GetPosition(this).X / ActualWidth;
                double targetHorizontal = RestoreBounds.Width * percentHorizontal;

                double percentVertical = e.GetPosition(this).Y / ActualHeight;
                double targetVertical = RestoreBounds.Height * percentVertical;

                WindowState = WindowState.Normal;

                POINT lMousePosition;
                GetCursorPos(out lMousePosition);

                Left = lMousePosition.X - targetHorizontal;
                Top = lMousePosition.Y - targetVertical;

                //e.Handled = false;
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
                //try { DragMove(); } catch { }

            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }

        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }
        #endregion
        void OnSizeSouth(object sender, MouseButtonEventArgs e) { OnSize(sender, SizingAction.South); }
        void OnSizeNorth(object sender, MouseButtonEventArgs e) { OnSize(sender, SizingAction.North); }
        void OnSizeEast(object sender, MouseButtonEventArgs e) { OnSize(sender, SizingAction.East); }
        void OnSizeWest(object sender, MouseButtonEventArgs e) { OnSize(sender, SizingAction.West); }
        void OnSizeNorthWest(object sender, MouseButtonEventArgs e) { OnSize(sender, SizingAction.NorthWest); }
        void OnSizeNorthEast(object sender, MouseButtonEventArgs e) { OnSize(sender, SizingAction.NorthEast); }
        void OnSizeSouthEast(object sender, MouseButtonEventArgs e) { OnSize(sender, SizingAction.SouthEast); }
        void OnSizeSouthWest(object sender, MouseButtonEventArgs e) { OnSize(sender, SizingAction.SouthWest); }

        void OnSize(object sender, SizingAction action)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {

                if (this.WindowState == WindowState.Normal)
                    DragSize(GetWindowHandle(this), action);
            }
        }
        const int WM_SYSCOMMAND = 0x112;
        const int SC_SIZE = 0xF000;
        const int SC_KEYMENU = 0xF100;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        void DragSize(IntPtr handle, SizingAction sizingAction)
        {
            SendMessage(handle, WM_SYSCOMMAND, (IntPtr)(SC_SIZE + sizingAction), IntPtr.Zero);
            SendMessage(handle, 514, IntPtr.Zero, IntPtr.Zero);
        }
        public IntPtr GetWindowHandle(Window window)
        {
            WindowInteropHelper helper = new WindowInteropHelper(window);
            return helper.Handle;
        }
        enum SizingAction
        {
            North = 3,
            South = 6,
            East = 2,
            West = 1,
            NorthEast = 5,
            NorthWest = 4,
            SouthEast = 8,
            SouthWest = 7
        }
        #endregion

        #region Style
        private void ChangeStyle_Click(object sender, RoutedEventArgs e)
        {
            App.Sett.GlogalApps_Settings.SelectedStyle++;
            if (App.Sett.GlogalApps_Settings.SelectedStyle > 1) App.Sett.GlogalApps_Settings.SelectedStyle = 0;
            SetStyleColor();
        }
        private void SetStyleColor()
        {
            if (App.Sett.GlogalApps_Settings.SelectedStyle == 0)
            {
                Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary() { Source = new Uri("/Theme/ColorsDark.xaml", UriKind.Relative) };
            }
            else if (App.Sett.GlogalApps_Settings.SelectedStyle == 1)
            {
                Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary() { Source = new Uri("/Theme/ColorsLight.xaml", UriKind.Relative) };
            }
            OnPropertyChanged("StyleChanged");
        }
        #endregion
        #region обработчики окна
        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            if (ReadyToShow != null)
            {
                ReadyToShow(this, null);
            }
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string msg = this.FindResource("m_ExitMessege").ToString();
            MessageBoxResult result =
              MessageBox.Show(
                msg,
                "ControlU",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
            {
                // If user doesn't want to close, cancel closure
                e.Cancel = true;
            }
            else if (result == MessageBoxResult.Yes)
            {
                //e.Cancel = true;
                if (AnyDevieceRunning())
                {
                    e.Cancel = true;
                    Message = "Отключите устройства перед закрытием программы";
                }
                else
                {
                    OnPropertyChanged("MainWindow_Closing");
                    atdi.ClosConnection();
                    Thread.Sleep(500);

                    App.Sett.GlogalApps_Settings.MainWindowLeft = this.Left;
                    App.Sett.GlogalApps_Settings.MainWindowTop = this.Top;
                    App.Sett.GlogalApps_Settings.MainWindowWidth = this.Width;
                    App.Sett.GlogalApps_Settings.MainWindowHeight = this.Height;
                    App.Sett.SaveGlogalApps();

                    //await StopAllDeviece();
                    StopAllDeviece();
                    App.Sett.SaveAll();
                }
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        public void StartApp()
        {
            try
            {
                //грохнем процесс рудика он же romes он же vicom
                System.Diagnostics.Process[] ps2 = System.Diagnostics.Process.GetProcessesByName("RuSWorkerDllLoaderPhysicalLayer"); //Имя процесса
                foreach (System.Diagnostics.Process p1 in ps2)
                {
                    p1.Kill();
                }
            }
            catch (Exception e)
            {
                exp.ExceptionData = new ExData() { ex = e, ClassName = "MainWindow", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
        }
        private bool AnyDevieceRunning()
        {
            bool alldev = true;
            int Running = 0;
            if (App.Sett.Equipments_Settings.SpectrumAnalyzer.UseEquipment == true && (An.Run == true || An.IsRuning == true)) Running++;
            if (App.Sett.Equipments_Settings.RuSReceiver.UseEquipment == true && (Rcvr.Run == true || Rcvr.IsRuning == true)) Running++;
            if (App.Sett.Equipments_Settings.RuSTSMx.UseEquipment == true && (tsmx.Run == true || tsmx.IsRuning == true)) Running++;
            if (App.Sett.Equipments_Settings.SignalHound.UseEquipment == true && (SHReceiver.Run == true || SHReceiver.IsRuning == true)) Running++;
            if (Running > 0) alldev = true;
            else if (Running == 0)
            {
                alldev = false;
            }
            return alldev;
        }
        private void StopAllDeviece()
        {
            bool alldev = true;
            if (App.Sett.Equipments_Settings.SpectrumAnalyzer.UseEquipment == true && An.Run == true)
            {
                An.Run = false;
            }
            if (App.Sett.Equipments_Settings.RuSReceiver.UseEquipment == true && Rcvr.Run == true)
            {
                Rcvr.Run = false;
            }
            if (App.Sett.Equipments_Settings.RuSTSMx.UseEquipment == true && tsmx.Run == true)
            {
                tsmx.Run = false;
            }
            if (App.Sett.Equipments_Settings.RuSRomesRC.UseEquipment == true && RCR.Run == true)
            {
                RCR.Run = false;
            }
            if (App.Sett.Equipments_Settings.SignalHound.UseEquipment == true && SHReceiver.Run == true)
            {
                SHReceiver.Run = false;
            }
            //bool alldev = true;
            //while (alldev)
            //{
            //    int Running = 0;
            //    if (An.Run == true || An.IsRuning == true) Running++;
            //    if (Rcvr.Run == true || Rcvr.IsRuning == true) Running++;
            //    if (tsmx.Run == true || tsmx.IsRuning == true) Running++;
            //    if (SHReceiver.Run == true || SHReceiver.IsRuning == true) Running++;
            //    if (Running > 0) alldev = true;
            //    else if (Running == 0)
            //    {
            //        alldev = false;
            //        Thread.Sleep(500);
            //    }
            //    Thread.Sleep(500);
            //}
        }
        #endregion

        #region db Loaded
        private void DB_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ServerIsLoaded")
            {
                if (db_v2.ServerIsLoaded == true)
                {
                    if (db_v2.ATDIConnectionData_Selsected.owner_id == App.Sett.ATDIConnection_Settings.Selected.owner_id &&
                          db_v2.ATDIConnectionData_Selsected.product_key == App.Sett.ATDIConnection_Settings.Selected.product_key)
                    {
                        //Atdi.SDR.Server.Bus.Classes.GlobalInit.MainRabbitMQServices =
                        //    "host=" + db_v2.ATDIConnectionData_Selsected.rabbit_host_name +
                        //    "; username=" + db_v2.ATDIConnectionData_Selsected.rabbit_user_name +
                        //    "; password=" + db_v2.ATDIConnectionData_Selsected.rabbit_password;
                        //Atdi.SDR.Server.Bus.Classes.GlobalInit.Template_SENSORS_List_ = db_v2.ATDIConnectionData_Selsected.sensor_queue;
                        //Atdi.SDR.Server.Bus.Classes.GlobalInit.Template_MEAS_TASK_Main_List_APPServer = db_v2.ATDIConnectionData_Selsected.task_queue;
                        //Atdi.SDR.Server.Bus.Classes.GlobalInit.Res_Template_MEAS_TASK_Main_List_APPServer = db_v2.ATDIConnectionData_Selsected.result_queue;
                        atdi.NameSensor = db_v2.Check(db_v2.ATDIConnectionData_Selsected.owner_id, db_v2.ATDIConnectionData_Selsected.product_key); //db_v2.ATDIConnectionData_Selsected.owner_id;
                                                                                                                                                    //atdi.NameTechId = db_v2.ATDIConnectionData_Selsected.sensor_equipment_tech_id;


                        //atdi.Initialize();

                    }
                }
                else
                    atdi.NameSensor = db_v2.Check(db_v2.ATDIConnectionData_Selsected.owner_id, db_v2.ATDIConnectionData_Selsected.product_key);
            }
        }
        #endregion




        #region MainMenu
        private void ExitfromMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SettingsfromMenu_Click(object sender, RoutedEventArgs e)
        {
            if (SetCtrl == null)
            {
                SetCtrl = new Controls.SettingsControl();
                SetCtrl.Width = 800;
                SetCtrl.Height = 500;
                ShowGlobalUC(SetCtrl, true, false);
            }
        }
        private void SaveAppSettings_Click(object sender, RoutedEventArgs e)
        {
            App.Sett.SaveBackupAllSettings();
        }
        private void LoadAppSettings_Click(object sender, RoutedEventArgs e)
        {
            App.Sett.RestoreBackupAllSettings();
        }

        private void UserMode_Click(object sender, RoutedEventArgs e)
        {
            //if (UserMode == Visibility.Collapsed) { UserMode = Visibility.Visible; }
            //else if (UserMode == Visibility.Visible) { UserMode = Visibility.Collapsed; }
        }
        private void AllLicenseAgreement_Click(object sender, RoutedEventArgs e)
        {
            Controls.AllLicenseAgreement ala = new Controls.AllLicenseAgreement();
            ShowGlobalUC(ala, true, false);
        }

        #endregion
        #region Errore
        private void DrawExceptionText_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ExceptionData" /* && FMeas.Settings.MainWindowSettings.Default.ExceptionShow == true*/)
            {
                App.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ExceptionTitleMessage.Text = exp.ExceptionData.ClassName + "  Method: " + exp.ExceptionData.AdditionalInformation;
                    ExceptionTextMessage.Text = exp.ExceptionText;
                    //AllErrMes += DateTime.Now.ToString() + "   " + exp.ExceptionData.ClassName + "  Method: " + exp.ExceptionData.AdditionalInformation + ":\r\n" +
                    //exp.ExceptionText;
                    VisibilityErrMes = true;
                }));
            }
        }
        private void CloseErrMesBorder_ButtonClick(object sender, RoutedEventArgs e)
        {
            VisibilityErrMes = false; /*ErrMesTmr.Stop();*/
        }
        private void CloseAllErrMesBorder_ButtonClick(object sender, RoutedEventArgs e)
        {
            VisibilityAllErrMes = false;
        }
        private void OpenAllErrMesBorder_ButtonClick(object sender, RoutedEventArgs e)
        {
            VisibilityAllErrMes = true;
        }
        #endregion
        #region Lang
        private void LanguageChanged(Object sender, EventArgs e)
        {
            CultureInfo currLang = App.Language;

            //Отмечаем нужный пункт смены языка как выбранный язык
            foreach (MenuItem i in Menu_language.Items)
            {
                CultureInfo ci = i.Tag as CultureInfo;
                i.IsChecked = ci != null && ci.Equals(currLang);
            }
        }
        private void ChangeLanguageClick(Object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                CultureInfo lang = mi.Tag as CultureInfo;
                if (lang != null)
                {
                    App.Language = lang;
                }
            }
        }
        #endregion
        #region Messages
        public void AddMesages(FrameworkElement el)
        {
            bool exist = false;
            foreach (UIElement spvolume in Messages.Children)
            {
                if (spvolume is UserControl)
                {
                    if (((UserControl)spvolume).Name == el.Name)
                    { exist = true; }
                }
            }
            if (!exist) { Messages.Children.Add(el); }
            //    //bool exist = false;
            //    //for (int i = 0; i < Messages.Children.Count; i++)
            //    //{
            //    //    if(Messages.Children[i])
            //    //}
            //    if (Messages.FindName(el.Name) is Controls.DeletePlan)
            //{

            //}
            //else { Messages.Children.Add(el); }
        }

        #endregion

        #region Вызов и закрытие всяких контролов поверх MainWindowGrid
        public void ShowGlobalUC(UserControl UCin, bool BlurIsEnabled, bool MainGridIsEnabled)
        {
            if (!GlobalUC)
            {
                GlobalUC = true;
                //AnSpec_Control.Visibility = Visibility.Hidden;
                //RsReceiverSpectrum_Control.Visibility = Visibility.Hidden;
                //MVSJammingPanel_cntrl.MVSRsReceiverSpectrum_Control.Visibility = Visibility.Hidden;
                //MeasDVBv2_Control.SpecPanel.Visibility = Visibility.Hidden;
                //MobileMon_control.MobileMonSpectrum_uc.Visibility = Visibility.Hidden;
                //TSMxSpectrum_Control.Visibility = Visibility.Hidden;
            }
            //UCin.Name = "UC";
            if (BlurIsEnabled)
            {
                var blur = new BlurEffect();
                blur.Radius = 10;
                DataGrid.Effect = blur;
            }
            DataGrid.IsEnabled = MainGridIsEnabled;

            MainGrid.Children.Add(UCin);
            //}
        }
        public void CloseGlobalUC(UserControl UCin)
        {
            MainGrid.Children.Remove(UCin);
            if (GlobalUC && MainGrid.Children.Count == 1)
            {
                DataGrid.IsEnabled = true;
                DataGrid.Effect = null;
                GlobalUC = false;
                //AnSpec_Control.Visibility = Visibility.Visible;
                //RsReceiverSpectrum_Control.Visibility = Visibility.Visible;
                //MVSJammingPanel_cntrl.MVSRsReceiverSpectrum_Control.Visibility = Visibility.Visible;
                //MeasDVBv2_Control.SpecPanel.Visibility = Visibility.Visible;
                //MobileMon_control.MobileMonSpectrum_uc.Visibility = Visibility.Visible;
                //TSMxSpectrum_Control.Visibility = Visibility.Visible;
            }
            UCin = null;
            //}
        }

        #endregion
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //private void plus(object sender, RoutedEventArgs e)
        //{
        //    Rcvr.Ind++;
        //    ind.Text = Rcvr.Ind.ToString();
        //}

        //private void minus(object sender, RoutedEventArgs e)
        //{
        //    Rcvr.Ind--;
        //    ind.Text = Rcvr.Ind.ToString();
        //}

        //private void gnssrundom(object sender, RoutedEventArgs e)
        //{
        //    //db_v2.add();
        //    //atdi.RegisterSensor();
        //    //App.Sett.SaveAll();
        //    //Message = DateTime.Now.ToLongTimeString();
        //    //atdi.SendReg();
        //    Random r = new Random();

        //    gps.LatitudeDecimal += ((decimal)r.Next(-100, 100)) / 10000;
        //    gps.LongitudeDecimal += ((decimal)r.Next(-100, 100)) / 10000;
        //}


    }

}
