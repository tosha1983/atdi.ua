using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlU.Controls
{
    /// <summary>
    /// Логика взаимодействия для RsReceiverPanel.xaml
    /// </summary>
    public partial class RsReceiverPanel : UserControl
    {
        Equipment.RsReceiver_v2 rcv = MainWindow.Rcvr;
        Helpers.Helper help = new Helpers.Helper();
        public RsReceiverPanel()
        {
            InitializeComponent();
            this.DataContext = rcv;
            rcv.PropertyChanged += UniqueData_PropertyChanged;
            PrintScreen.DataContext = App.Sett.Equipments_Settings.RuSReceiver;

        }
        private void UniqueData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsRuning")
            {
                bool df = false;
                bool pscan = false;
                for (int i = 0; i < rcv.UniqueData.LoadedInstrOption.Count; i++)
                {
                    if (rcv.UniqueData.LoadedInstrOption[i].Type.ToLower().Contains("df"))
                    {
                        df = true;
                    }
                    if (rcv.UniqueData.LoadedInstrOption[i].Type.ToLower().Contains("ps"))
                    {
                        pscan = true;
                    }
                }
                if (df)
                {
                    DFPanel.Visibility = Visibility.Visible;
                    DFPanel.IsEnabled = df;
                }
                else
                {
                    DFPanel.Visibility = Visibility.Collapsed;
                    DFPanel.IsEnabled = df;
                }
                if (pscan)
                {
                    PSCANPanel.Visibility = Visibility.Visible;
                    PSCANPanel.IsEnabled = pscan;
                }
                else
                {
                    PSCANPanel.Visibility = Visibility.Collapsed;
                    PSCANPanel.IsEnabled = pscan;
                }
            }
        }


        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string t = "FFM";
            try
            {
                // ... Get TabControl reference.
                var item = sender as TabControl;
                // ... Set Title to selected tab header.
                var selected = item.SelectedItem as TabItem;
                t = selected.Header.ToString();
            }
            catch { }

            if (t == "FFM") { rcv.Mode = rcv.UniqueData.Modes.Where(x => x.Mode == t).First(); /*rcv.TelnetDM += rcv.SetStop;*/ }
            else if (t == "DF") { rcv.Mode = rcv.UniqueData.Modes.Where(x => x.Mode == t).First(); /*rcv.TelnetDM += rcv.SetStop; */}
            else if (t == "PSCAN") { rcv.Mode = rcv.UniqueData.Modes.Where(x => x.Mode == t).First();  /*rcv.TelnetDM += rcv.SetRun;*/ }
            //rcv.TelnetDM += rcv.SetMeasAppl;
            //rcv.TelnetDM += rcv.SetFreqMode;
            rcv.SetLevelFromMode();

        }
        private void Decimal_PreKeyDownDecimal(object sender, TextCompositionEventArgs e)
        {
            help.CheckIsDecimal((TextBox)sender, e);
        }
        #region доп кнопки

        private void PrintScreen_Click(object sender, RoutedEventArgs e)
        {
            if (App.Lic)
            {
                //if (rcv.IsRuning)
                //{
                bool fromname = false;
                if (App.Sett.Equipments_Settings.RuSReceiver.PrintScreenType == 0)
                {
                    DialogWindow dialog = new DialogWindow(this.FindResource("EnterFileName").ToString(), "ControlU");
                    dialog.Width = 250;
                    dialog.ResponseText = rcv.ScreenName;
                    if (dialog.ShowDialog() == true)
                    {
                        fromname = true;
                        rcv.ScreenName = dialog.ResponseText;
                    }
                }
                else if (App.Sett.Equipments_Settings.RuSReceiver.PrintScreenType == 1)
                {
                    int ind = 1;
                    string[] Files = help.GetFileNames(App.Sett.Screen_Settings.ScreenFolder, "Screen_*.*");
                    if (Files.Length > 0)
                    {
                        for (int i = 0; i < Files.Length; i++)
                        {
                            int f = 0;
                            int.TryParse(Files[i].Replace("Screen_", ""), out f);
                            ind = Math.Max(ind, f);
                        }
                        ind++;
                    }
                    rcv.ScreenName = "Screen_" + string.Format("{0:00000}", ind);
                }
                else if (App.Sett.Equipments_Settings.RuSReceiver.PrintScreenType == 2)
                {
                    rcv.ScreenName = MainWindow.gps.LocalTime.ToString("yyyy-MM-dd H.mm.ss.fff");
                }
                else if (App.Sett.Equipments_Settings.RuSReceiver.PrintScreenType == 3)
                {
                    Controls.FreqConverter fcdp = new Controls.FreqConverter();
                    rcv.ScreenName = (string)fcdp.Convert(rcv.FreqCentr, null, null, null) + " " + MainWindow.gps.LocalTime.ToString("yyyy-MM-dd H.mm.ss.fff");
                }
                else if (App.Sett.Equipments_Settings.RuSReceiver.PrintScreenType == 4)
                {
                    Controls.FreqConverter fcdp = new Controls.FreqConverter();
                    rcv.ScreenName = MainWindow.gps.LocalTime.ToString("yyyy-MM-dd H.mm.ss.fff") + " " + (string)fcdp.Convert(rcv.FreqCentr, null, null, null);
                }
                if ((App.Sett.Equipments_Settings.RuSReceiver.PrintScreenType == 0 && fromname) || App.Sett.Equipments_Settings.RuSReceiver.PrintScreenType != 0)
                {
                    Equipment.PrintScreen ps = new Equipment.PrintScreen();
                    ps.InstrType = (int)Equipment.InstrumentType.RuSReceiver;
                    if (rcv.Trace1Type.UI != new Equipment.AllTraceTypes().TraceTypes[6].UI)
                    {
                        ps.Trace1State = true;
                        ps.Trace1Legend = rcv.Trace1Type.UI + "(" + rcv.Detector.UI + ")";
                        if (rcv.Trace1Type.Parameter == "1")
                        { ps.Trace1Legend += "[" + rcv.Trace1AveragedList.NumberOfSweeps.ToString() + "/" + rcv.Trace1AveragedList.AveragingCount.ToString() + "]"; }
                        if (rcv.Trace1Type.UI == "2")
                        { ps.Trace1Legend += "[" + rcv.Trace1TrackedList.NumberOfSweeps.ToString() + "/" + rcv.Trace1TrackedList.TrackingCount.ToString() + "]"; }
                        ps.Trace1 = rcv.Trace1;
                    }
                    if (rcv.Trace2Type.UI != new Equipment.AllTraceTypes().TraceTypes[6].UI)
                    {
                        ps.Trace2State = true;
                        ps.Trace2Legend = rcv.Trace2Type.UI + "(" + rcv.Detector.UI + ")";
                        if (rcv.Trace2Type.Parameter == "1")
                        { ps.Trace2Legend += "[" + rcv.Trace2AveragedList.NumberOfSweeps.ToString() + "/" + rcv.Trace2AveragedList.AveragingCount.ToString() + "]"; }
                        if (rcv.Trace2Type.UI == "2")
                        { ps.Trace2Legend += "[" + rcv.Trace2TrackedList.NumberOfSweeps.ToString() + "/" + rcv.Trace2TrackedList.TrackingCount.ToString() + "]"; }
                        ps.Trace2 = rcv.Trace2;
                    }
                    if (rcv.Trace3Type.UI != new Equipment.AllTraceTypes().TraceTypes[6].UI)
                    {
                        ps.Trace3State = true;
                        ps.Trace3Legend = rcv.Trace3Type.UI + "(" + rcv.Detector.UI + ")";
                        if (rcv.Trace3Type.Parameter == "1")
                        { ps.Trace3Legend += "[" + rcv.Trace3AveragedList.NumberOfSweeps.ToString() + "/" + rcv.Trace3AveragedList.AveragingCount.ToString() + "]"; }
                        if (rcv.Trace3Type.UI == "2")
                        { ps.Trace3Legend += "[" + rcv.Trace3TrackedList.NumberOfSweeps.ToString() + "/" + rcv.Trace3TrackedList.TrackingCount.ToString() + "]"; }
                        ps.Trace3 = rcv.Trace3;
                    }
                    if (rcv.ChannelPowerState)
                    {
                        ps.ChannelPower = rcv.ChannelPowerState;
                        ps.ChannelPowerBW = rcv.ChannelPowerBW;
                        ps.ChannelPowerResult = rcv.ChannelPowerResult;
                    }
                    ps.ActualPoints = rcv.Trace1.Length;// rcv.TracePoints;
                    ps.FreqStart = rcv.FreqStart;
                    ps.FreqStop = rcv.FreqStop;

                    ps.RefLevel = rcv.RefLevel;
                    ps.Range = rcv.Range;
                    ps.LevelUnit = rcv.LevelUnitStr;
                    ps.DateTime = MainWindow.gps.LocalTime;
                    ps.SWT = rcv.MeasTime;
                    //ps.Att = rcv.ATTStr;
                    if (rcv.Mode.Mode == "FFM")
                        ps.RBW = rcv.FFMStep;
                    if (rcv.Mode.Mode == "DF")
                        ps.RBW = rcv.FFMStep;
                    if (rcv.Mode.Mode == "PSCAN")
                        ps.RBW = rcv.UniqueData.PSCANStepBW[rcv.PScanStepInd];
                    ps.Mode = rcv.Mode.Mode;

                    ps.FreqCentr = rcv.FFMFreqCentr;
                    ps.FreqSpan = rcv.FreqSpan;
                    ps.InstrManufacrure = Equipment.InstrManufacrures.RuS.UIShort;
                    ps.InstrModel = rcv.InstrModel;
                    ps.Location = new Point((double)MainWindow.gps.LongitudeDecimal, (double)MainWindow.gps.LatitudeDecimal);// MainWindow.gps.LatitudeStr + " " + MainWindow.gps.LongitudeStr;

                    //ps.OverLoad = "RFOverload";
                    ps.Markers = rcv.Markers;
                    //ps.inTMarkers = rcv.TMarkers;


                    ps.FilePath = App.Sett.Screen_Settings.ScreenFolder + "\\" + rcv.ScreenName;//  MainWindow.gps.LocalTime.ToString();
                    ps.drawImageToPath();
                    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = this.FindResource("FileSaved").ToString().Replace("*NAME*", rcv.ScreenName);
                }
            }
            else
            {
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = this.FindResource("LicenseFail").ToString();
            }
            //rcv.TelnetDM += rcv.Gettr;
        }
        private void PrintScreenType_Click(object sender, RoutedEventArgs e)
        {
            App.Sett.Equipments_Settings.RuSReceiver.PrintScreenType = int.Parse((string)((MenuItem)sender).Tag);
            App.Sett.SaveEquipments();
        }

        private void PrintScreenTypeMenu_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = PrintScreen_Btn;// (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }
        private void OpenScreenFolder_click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (System.IO.Directory.Exists(@App.Sett.Screen_Settings.ScreenFolder))
                {

                    if (rcv.ScreenName == "")
                    { System.Diagnostics.Process.Start(@App.Sett.Screen_Settings.ScreenFolder); }
                    else
                    {
                        string argument = "/select, \"" + App.Sett.Screen_Settings.ScreenFolder + @"\" + rcv.ScreenName + "." + @App.Sett.Screen_Settings.SaveScreenImageFormat.ToString() + "\"";
                        System.Diagnostics.Process.Start("explorer.exe", argument);
                    }
                }
                else
                {
                    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = this.FindResource("ScreenShotPathFail").ToString();//"Путь сохранения скриншотов не задан!";
                }
            }
            catch { /*OKMessageBox Message = new OKMessageBox("Путь сохранения скриншотов не задан", "Предупреждение"); Message.ShowDialog();*/ }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (rcv.UniqueData.SaveAllData)
            {
                rcv.SaveAllData = !rcv.SaveAllData;
            }
        }
        private void ShowWaterfall_Click(object sender, RoutedEventArgs e)
        {
            if (rcv.UniqueData.SaveAllData)
            {
                rcv.ShowWaterfall = !rcv.ShowWaterfall;
            }
        }
        private void Replay_click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.OpenFileDialog OF = new System.Windows.Forms.OpenFileDialog();
                OF.InitialDirectory =App.Sett.Screen_Settings.ScreenFolder;
                OF.Filter = "(*.dat)|*.dat";
                if (OF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    rcv.LoadFileReplay(OF.FileName);
                }
            }
            catch (Exception exp)
            { }
        }
        private void StopReplay_Click(object sender, RoutedEventArgs e)
        {
            rcv.Replay = !rcv.Replay;
        }


        private void PresetGui(object sender, RoutedEventArgs e)
        {
            rcv.TelnetDM += rcv.Preset;
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Global
        private void FFMMeasMode_Click(object sender, RoutedEventArgs e)
        {
            rcv.MeasMode = !rcv.MeasMode;
            rcv.SetMeasMode(rcv.MeasMode);
        }
        private void MeasTimeDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.MeasTime -= help.PlMntime(rcv.MeasTime);
            rcv.SetMeasTime(rcv.MeasTime);
        }

        private void MeasTimeUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.MeasTime += help.PlMntime(rcv.MeasTime);
            rcv.SetMeasTime(rcv.MeasTime);
        }

        #region Selectivity
        private void SelectivityDn_Click(object sender, RoutedEventArgs e)
        {
            int i = rcv.UniqueData.SelectivityModes.IndexOf(rcv.SelectivityMode);
            i--;
            if (i < 0)
            {
                i = rcv.UniqueData.SelectivityModes.Count - 1;
            }
            rcv.SetSelectivity(rcv.UniqueData.SelectivityModes[i]);


            //rcv.TelnetDM += rcv.SetSelectivityDn;
        }
        private void SelectivityUp_Click(object sender, RoutedEventArgs e)
        {
            int i = rcv.UniqueData.SelectivityModes.IndexOf(rcv.SelectivityMode);
            i++;
            if (i >= rcv.UniqueData.SelectivityModes.Count)
            {
                i = 0;
            }
            rcv.SetSelectivity(rcv.UniqueData.SelectivityModes[i]);
            //rcv.TelnetDM += rcv.SetSelectivityUp;
        }
        #endregion Selectivity
        #endregion Global

        #region FFM
        #region FreqCentr
        private void FFMFreqCentr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rcv.FFMFreqCentrToSet = help.KeyDownDecimal(rcv.FFMFreqCentr / 1000000, (TextBox)sender, e) * 1000000;
                rcv.SetFFMFreqCentr(rcv.FFMFreqCentrToSet);
            }
        }
        private void FFMFreqCentrDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.FFMFreqCentrToSet -= rcv.FreqSpan / 100;
            rcv.SetFFMFreqCentr(rcv.FFMFreqCentrToSet);
        }
        private void FFMFreqCentrUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.FFMFreqCentrToSet += rcv.FreqSpan / 100;
            rcv.SetFFMFreqCentr(rcv.FFMFreqCentrToSet);
        }
        #endregion FreqCentr

        #region FreqSpan
        private void FFMFreqSpanDn_Click(object sender, RoutedEventArgs e)
        {
            int ind = rcv.FFMFreqSpanInd;
            if (ind > 0)
            {
                ind--;
            }
            rcv.SetFFMFreqSpanFromIndex(ind);
        }
        private void FFMFreqSpanUp_Click(object sender, RoutedEventArgs e)
        {
            int ind = rcv.FFMFreqSpanInd;
            if (ind < rcv.UniqueData.FFMSpanBW.Length - 1)
            {
                ind++;
            }
            rcv.SetFFMFreqSpanFromIndex(ind);
        }
        #endregion FreqSpan

        #region Step
        private void FFMStepDn_Click(object sender, RoutedEventArgs e)
        {
            int ind = rcv.FFMStepInd;
            if (ind > 0)
            {
                ind--;
            }
            rcv.SetFFMStepFromIndex(ind);
            rcv.FFMStepAuto = false;
        }

        private void FFMStepUp_Click(object sender, RoutedEventArgs e)
        {
            int ind = rcv.FFMStepInd;
            if (ind < rcv.UniqueData.FFMStepBW.Length - 1)
            {
                ind++;
            }
            rcv.SetFFMStepFromIndex(ind);
            rcv.FFMStepAuto = false;
        }
        private void FFMStepAuto_Changed(object sender, RoutedEventArgs e)
        {
            rcv.SetFFMStepAuto(rcv.FFMStepAuto);
            //rcv.TelnetDM += rcv.SetFFMStepAuto;
        }
        #endregion Step

        #region FFTMode
        private void FFMFFTModeDn_Click(object sender, RoutedEventArgs e)
        {
            int i = rcv.UniqueData.FFTModes.IndexOf(rcv.FFMFFTMode);
            i++;
            if (i >= rcv.UniqueData.FFTModes.Count)
            {
                i = 0;
            }
            rcv.SetFFMFFTMode(rcv.UniqueData.FFTModes[i]);
        }
        private void FFMFFTModeUp_Click(object sender, RoutedEventArgs e)
        {
            int i = rcv.UniqueData.FFTModes.IndexOf(rcv.FFMFFTMode);
            i--;
            if (i < 0)
            {
                i = rcv.UniqueData.FFTModes.Count - 1;
            }

            rcv.SetFFMFFTMode(rcv.UniqueData.FFTModes[i]);
        }
        #endregion FFTMode

        #region Level
        private void FFMRefLevelDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.FFMRefLevel--;
            rcv.SetFFMRefLevel(rcv.FFMRefLevel);
        }
        private void FFMRefLevelUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.FFMRefLevel++;
            rcv.SetFFMRefLevel(rcv.FFMRefLevel);
        }
        private void FFMRefLevel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            rcv.FFMRefLevel += (decimal)help.MouseWheelOutStep(sender, e);
            rcv.SetFFMRefLevel(rcv.FFMRefLevel);
        }

        private void FFMRangeLevelDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.FFMRangeLevel -= rcv.UniqueData.RangeLevelStep;
            rcv.SetFFMRangeLevel(rcv.FFMRangeLevel);
        }
        private void FFMRangeLevelUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.FFMRangeLevel += rcv.UniqueData.RangeLevelStep;
            rcv.SetFFMRangeLevel(rcv.FFMRangeLevel);
        }
        private void FFMRangeLevel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            rcv.FFMRangeLevel += (decimal)help.MouseWheelOutStep(sender, e) * rcv.UniqueData.RangeLevelStep;
            rcv.SetFFMRangeLevel(rcv.FFMRangeLevel);
        }
        #endregion Level

        #region DemodFreqCentr пофиксить

        private void DemodFreqCentrFFM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rcv.FFMFreqCentrToSet = help.KeyDownDecimal(rcv.FFMFreqCentr / 1000000, (TextBox)sender, e) * 1000000;
                rcv.SetFFMFreqCentr(rcv.FFMFreqCentrToSet);
            }
        }
        private void DemodFreqCentrDF_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rcv.FFMFreqCentrToSet = help.KeyDownDecimal(rcv.FFMFreqCentr / 1000000, (TextBox)sender, e) * 1000000;
                rcv.SetFFMFreqCentr(rcv.FFMFreqCentrToSet);
            }
        }
        private void DemodFreqCentrDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.FFMDemodFreq -= rcv.FreqSpan / 100;
            rcv.SetDemodFreq(rcv.FFMDemodFreq);
            //rcv.TelnetDM += rcv.SetDemodFreqDn;
        }
        private void DemodFreqCentrUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.FFMDemodFreq += rcv.FreqSpan / 100;
            rcv.SetDemodFreq(rcv.FFMDemodFreq);
        }
        #endregion DemodFreqCentr
        #endregion FFM
        #region PSCAN
        #region Freq
        private void PSCAN_FreqStartDn_Click(object sender, RoutedEventArgs e)
        {
            if (rcv.PScanFreqStart >= rcv.MinFreqReceiver - rcv.PScanFreqStart + help.helpFreqMinus(rcv.PScanFreqStart))
            {
                rcv.PScanFreqStart = help.helpFreqMinus(rcv.PScanFreqStart);
            }
            rcv.SetPScanFreqStart(rcv.PScanFreqStart);
        }
        private void PSCAN_FreqStartUp_Click(object sender, RoutedEventArgs e)
        {
            if (help.helpFreqPlus(rcv.PScanFreqStart) <= rcv.MaxFreqReceiver)
            {
                rcv.PScanFreqStart = help.helpFreqPlus(rcv.PScanFreqStart);
            }
            rcv.SetPScanFreqStart(rcv.PScanFreqStart);
        }
        private void PSCAN_FreqStart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rcv.PScanFreqStart = help.KeyDownDecimal(rcv.PScanFreqStart / 1000000, (TextBox)sender, e) * 1000000;
                rcv.SetPScanFreqStart(rcv.PScanFreqStart);
            }
        }

        private void PSCAN_FreqStopDn_Click(object sender, RoutedEventArgs e)
        {
            if (rcv.PScanFreqStop >= rcv.MinFreqReceiver - rcv.PScanFreqStop + help.helpFreqMinus(rcv.PScanFreqStop))
            {
                rcv.PScanFreqStop = help.helpFreqMinus(rcv.PScanFreqStop);
            }
            rcv.SetPSCANFreqStop(rcv.PScanFreqStop);
        }
        private void PSCAN_FreqStopUp_Click(object sender, RoutedEventArgs e)
        {
            if (rcv.PScanFreqStop <= rcv.MaxFreqReceiver + rcv.PScanFreqStop - help.helpFreqPlus(rcv.PScanFreqStop))
            {
                rcv.PScanFreqStop = help.helpFreqPlus(rcv.PScanFreqStop);
            }
            rcv.SetPSCANFreqStop(rcv.PScanFreqStop);
        }
        private void PSCAN_FreqStop_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rcv.PScanFreqStop = help.KeyDownDecimal(rcv.PScanFreqStop / 1000000, (TextBox)sender, e) * 1000000;
                rcv.SetPSCANFreqStop(rcv.PScanFreqStop);
            }
        }

        private void PSCAN_FreqCentrDn_Click(object sender, RoutedEventArgs e)
        {
            if (rcv.PScanFreqCentr >= rcv.MinFreqReceiver - rcv.PScanFreqCentr + help.helpFreqMinus(rcv.PScanFreqCentr))
            {
                rcv.PScanFreqCentr = help.helpFreqMinus(rcv.PScanFreqCentr);
            }
            rcv.SetPSCANFreqCentr(rcv.PScanFreqCentr);
        }
        private void PSCAN_FreqCentrUp_Click(object sender, RoutedEventArgs e)
        {
            if (rcv.PScanFreqCentr <= rcv.MaxFreqReceiver + rcv.PScanFreqCentr - help.helpFreqPlus(rcv.PScanFreqCentr))
            {
                rcv.PScanFreqCentr = help.helpFreqPlus(rcv.PScanFreqCentr);
            }
            rcv.SetPSCANFreqCentr(rcv.PScanFreqCentr);
        }
        private void PSCAN_FreqCentr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rcv.PScanFreqCentr = help.KeyDownDecimal(rcv.PScanFreqCentr / 1000000, (TextBox)sender, e) * 1000000;
                rcv.SetPSCANFreqCentr(rcv.PScanFreqCentr);
            }
        }

        private void PSCAN_FreqSpanDn_Click(object sender, RoutedEventArgs e)
        {
            if (rcv.PScanFreqCentr - help.helpFreqMinus(rcv.PScanFreqSpan) / 2 >= rcv.MinFreqReceiver && help.helpFreqMinus(rcv.PScanFreqSpan) >= rcv.UniqueData.PSCANStepBW[rcv.PScanStepInd])
            {
                rcv.PScanFreqSpan = help.helpFreqMinus(rcv.PScanFreqSpan);
            }
            rcv.SetPSCANFreqSpan(rcv.PScanFreqSpan);
        }
        private void PSCAN_FreqSpanUp_Click(object sender, RoutedEventArgs e)
        {
            if (rcv.PScanFreqCentr + help.helpFreqPlus(rcv.PScanFreqSpan) / 2 <= rcv.MaxFreqReceiver)
            {
                rcv.PScanFreqSpan = help.helpFreqPlus(rcv.PScanFreqSpan);
            }
            rcv.SetPSCANFreqSpan(rcv.PScanFreqSpan);
        }
        private void PSCAN_FreqSpan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rcv.PScanFreqSpan = help.KeyDownDecimal(rcv.PScanFreqSpan / 1000000, (TextBox)sender, e) * 1000000;
                rcv.SetPSCANFreqSpan(rcv.PScanFreqSpan);
            }
        }
        #endregion Freq

        private void PSCANRUN_Click(object sender, RoutedEventArgs e)
        {
            rcv.PScanRun = true;
        }
        private void PSCANSTOP_Click(object sender, RoutedEventArgs e)
        {
            rcv.PScanRun = false;
        }

        private void PScanStepDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.PScanStepInd--;
            rcv.SetPScanStepIndex(rcv.PScanStepInd);
        }
        private void PScanStepUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.PScanStepInd++;
            rcv.SetPScanStepIndex(rcv.PScanStepInd);
        }

        private void PScanFFTModeDn_Click(object sender, RoutedEventArgs e)
        {
            int i = rcv.UniqueData.FFTModes.IndexOf(rcv.PScanFFTMode);
            i--;
            if (i < 0)
            {
                i = rcv.UniqueData.FFTModes.Count - 1;
            }
            rcv.PScanFFTMode = rcv.UniqueData.FFTModes[i];
            rcv.SetPScanFFTMode(rcv.PScanFFTMode);
        }

        private void PScanFFTModeUp_Click(object sender, RoutedEventArgs e)
        {
            int i = rcv.UniqueData.FFTModes.IndexOf(rcv.PScanFFTMode);
            i++;
            if (i >= rcv.UniqueData.FFTModes.Count)
            {
                i = 0;
            }
            rcv.PScanFFTMode = rcv.UniqueData.FFTModes[i];
            rcv.SetPScanFFTMode(rcv.PScanFFTMode);
        }

        private void PScanRefLevelDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.PScanRefLevel--;
            rcv.SetPScanRefLevel(rcv.PScanRefLevel);
        }
        private void PScanRefLevelUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.PScanRefLevel++;
            rcv.SetPScanRefLevel(rcv.PScanRefLevel);
        }
        private void PScanRefLevel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            rcv.PScanRefLevel += (decimal)help.MouseWheelOutStep(sender, e);
            rcv.SetPScanRefLevel(rcv.PScanRefLevel);
        }
        private void PScanRangeLevelDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.PScanRangeLevel -= rcv.UniqueData.RangeLevelStep;
            rcv.SetPScanRangeLevel(rcv.PScanRangeLevel);
        }
        private void PScanRangeLevelUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.PScanRangeLevel += rcv.UniqueData.RangeLevelStep;
            rcv.SetPScanRangeLevel(rcv.PScanRangeLevel);
        }
        private void PScanRangeLevel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            rcv.PScanRangeLevel += (decimal)help.MouseWheelOutStep(sender, e) * rcv.UniqueData.RangeLevelStep;
            rcv.SetPScanRangeLevel(rcv.PScanRangeLevel);
        }
        #endregion PSCAN

        #region Trace       
        #region Trace 1      
        private void Trace1Reset_Click(object sender, RoutedEventArgs e)
        {
            rcv.Trace1Reset = true;
        }
        private void Trace1AverageMinus_Click(object sender, RoutedEventArgs e)
        {
            rcv.Trace1AveragedList.AveragingCount--;
        }
        private void Trace1AveragePlus_Click(object sender, RoutedEventArgs e)
        {
            rcv.Trace1AveragedList.AveragingCount++;
        }
        private void Trace1Average_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rcv.Trace1AveragedList.AveragingCount = help.KeyDownUInt(rcv.Trace1AveragedList.AveragingCount, (TextBox)sender, e);
            }
        }

        private void Trace1TrackingMinus_Click(object sender, RoutedEventArgs e)
        {
            rcv.Trace1TrackedList.TrackingCount--;
        }
        private void Trace1TrackingPlus_Click(object sender, RoutedEventArgs e)
        {
            rcv.Trace1TrackedList.TrackingCount++;
        }
        private void Trace1Tracking_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rcv.Trace1TrackedList.TrackingCount = help.KeyDownUInt(rcv.Trace1TrackedList.TrackingCount, (TextBox)sender, e);
            }
        }
        #endregion Trace 1
        #region Trace 2      
        private void Trace2Reset_Click(object sender, RoutedEventArgs e)
        {
            rcv.Trace2Reset = true;
        }
        private void Trace2AverageMinus_Click(object sender, RoutedEventArgs e)
        {
            rcv.Trace2AveragedList.AveragingCount--;
        }
        private void Trace2AveragePlus_Click(object sender, RoutedEventArgs e)
        {
            rcv.Trace2AveragedList.AveragingCount++;
        }
        private void Trace2Average_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rcv.Trace2AveragedList.AveragingCount = help.KeyDownUInt(rcv.Trace2AveragedList.AveragingCount, (TextBox)sender, e);
            }
        }

        private void Trace2TrackingMinus_Click(object sender, RoutedEventArgs e)
        {
            rcv.Trace2TrackedList.TrackingCount--;
        }
        private void Trace2TrackingPlus_Click(object sender, RoutedEventArgs e)
        {
            rcv.Trace2TrackedList.TrackingCount++;
        }
        private void Trace2Tracking_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rcv.Trace2TrackedList.TrackingCount = help.KeyDownUInt(rcv.Trace2TrackedList.TrackingCount, (TextBox)sender, e);
            }
        }
        #endregion Trace 2
        #region Trace 3   
        private void Trace3Reset_Click(object sender, RoutedEventArgs e)
        {
            rcv.Trace3Reset = true;
        }
        private void Trace3AverageMinus_Click(object sender, RoutedEventArgs e)
        {
            rcv.Trace3AveragedList.AveragingCount--;
        }
        private void Trace3AveragePlus_Click(object sender, RoutedEventArgs e)
        {
            rcv.Trace3AveragedList.AveragingCount++;
        }
        private void Trace3Average_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rcv.Trace3AveragedList.AveragingCount = help.KeyDownUInt(rcv.Trace3AveragedList.AveragingCount, (TextBox)sender, e);
            }
        }

        private void Trace3TrackingMinus_Click(object sender, RoutedEventArgs e)
        {
            rcv.Trace3TrackedList.TrackingCount--;
        }
        private void Trace3TrackingPlus_Click(object sender, RoutedEventArgs e)
        {
            rcv.Trace3TrackedList.TrackingCount++;
        }
        private void Trace3Tracking_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rcv.Trace3TrackedList.TrackingCount = help.KeyDownUInt(rcv.Trace3TrackedList.TrackingCount, (TextBox)sender, e);
            }
        }
        #endregion Trace 3
        #endregion Trace

        #region 
        private void MidFreqCentrDF_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rcv.FFMFreqCentrToSet = help.KeyDownDecimal(rcv.FFMFreqCentr / 1000000, (TextBox)sender, e) * 1000000;
                rcv.SetFFMFreqCentr(rcv.FFMFreqCentrToSet);
            }
        }

        #endregion

        #region DF
        private void ScrollDFGui_up(object sender, RoutedEventArgs e)
        {
            ScrollDFGui.LineUp();
        }
        private void ScrollDFGui_down(object sender, RoutedEventArgs e)
        {
            ScrollDFGui.LineDown();
        }
        private void DFSQUMode_Click(object sender, RoutedEventArgs e)
        {
            int i = rcv.DFSQUModeIndex;
            i++;
            if (i > rcv.UniqueData.DFSQUModes.Count - 1) i = 0;
            rcv.SetDFSQUModeInde(i);
        }
        private void DFSQUDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.TelnetDM += rcv.SetDFSQUDn;
        }

        private void DFSQUUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.TelnetDM += rcv.SetDFSQUUp;
        }
        private void DFQualSQUDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.TelnetDM += rcv.SetDFQualSQUDn;
        }

        private void DFQualSQUUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.TelnetDM += rcv.SetDFQualSQUUp;
        }
        private void DFMeasureTimeDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.TelnetDM += rcv.SetDFMeasureTimeDn;
        }

        private void DFMeasureTimeUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.TelnetDM += rcv.SetDFMeasureTimeUp;
        }
        private void DFBWDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.DFBandwidthAuto = false;
            //проверить rcv.DFBandwidthInd--
            int ind = rcv.DFBandwidthInd;
            if (ind > 0)
            {
                ind--;
            }
            rcv.SetDFBandwidtIndex(ind);

            //rcv.TelnetDM += rcv.SetDFBandwidthDn;
        }

        private void DFBWUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.DFBandwidthAuto = false;
            //проверить rcv.DFBandwidthInd++
            int ind = rcv.DFBandwidthInd;
            if (ind < rcv.UniqueData.DFBW.Length - 1)
            {
                ind++;
            }
            rcv.SetDFBandwidtIndex(ind);
            //rcv.TelnetDM += rcv.SetDFBandwidthUp;
        }

        private void DFBWAuto_Checked(object sender, RoutedEventArgs e)
        {
            rcv.SetDFBandwidthAuto(rcv.DFBandwidthAuto);
        }

        private void DFBWAuto_Unchecked(object sender, RoutedEventArgs e)
        {
            rcv.SetDFBandwidthAuto(rcv.DFBandwidthAuto);
        }
        #endregion




        #region Global

        private void RfModeDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.TelnetDM += rcv.SetRfModeDn;
        }
        private void RfModeUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.TelnetDM += rcv.SetRfModeUp;
        }
        private void LevelUnit_Click(object sender, RoutedEventArgs e)
        {
            rcv.LevelUnitInd++;
        }
        private void Antennas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            rcv.SetAntenna(((ComboBox)sender).SelectedIndex);
        }
        #endregion

        #region Markers
        //=====================================================================================================
        //=====================================================================================================
        #region Markers
        private void MarkerState_Click(object sender, RoutedEventArgs e)
        {
            Equipment.Marker m = (Equipment.Marker)((Button)sender).Tag;
            rcv.SetMarkerState(m, !m.StateNew);
        }
        private void MarkerFreqMinus_Click(object sender, RoutedEventArgs e)
        {
            Equipment.Marker m = (Equipment.Marker)((RepeatButton)sender).Tag;
            m.IndexOnTrace--;
            rcv.SetMarkerFromIndex(m, m.IndexOnTrace);

        }
        private void MarkerFreqPlus_Click(object sender, RoutedEventArgs e)
        {
            Equipment.Marker m = (Equipment.Marker)((RepeatButton)sender).Tag;
            m.IndexOnTrace++;
            rcv.SetMarkerFromIndex(m, m.IndexOnTrace);
        }
        private void MarkerFreq_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Equipment.Marker m = (Equipment.Marker)((TextBox)sender).Tag;
            if (m.MarkerType == 0 || m.MarkerType == 3)//M/M+nDb
            {
                help.CheckIsDecimal((TextBox)sender, e);
            }
            else if (m.MarkerType == 1)//D
            {
                //e.Handled = true;
                //help.CheckIsDecimal((TextBox)sender, e);
                help.CheckIsDecimalWithMinus((TextBox)sender, e);
            }
            else if (m.MarkerType == 2)//T
            {

            }
        }
        private void MarkerFreq_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Equipment.Marker m = (Equipment.Marker)((TextBox)sender).Tag;
                decimal f = m.Freq;
                if (m.MarkerType == 0)
                { f = help.KeyDownDecimal(m.Freq / 1000000, (TextBox)sender, e) * 1000000; }
                else if (m.MarkerType == 1)
                {
                    f = m.MarkerParent.Freq + help.KeyDownDecimal(m.Funk1 / 1000000, (TextBox)sender, e) * 1000000;
                    int r = 0;
                }
                rcv.SetMarkerFromFreq(m, f);
            }
        }
        private void MarkerPeakSearch_Click(object sender, RoutedEventArgs e)
        {
            Equipment.Marker m = (Equipment.Marker)((Button)sender).Tag;
            rcv.SetMarkerPeakSearch(m);
        }
        private void MarkerType_Click(object sender, RoutedEventArgs e)
        {
            Equipment.Marker m = (Equipment.Marker)((Button)sender).Tag;
            rcv.SetMarkerChangeType(m);
        }


        private void ScrollMarkersUp_Click(object sender, RoutedEventArgs e)
        {
            Markers_ScrollViewer.LineUp();
        }
        private void ScrollMarkersDown_Click(object sender, RoutedEventArgs e)
        {
            Markers_ScrollViewer.LineDown();
        }
        private void AllMarkersOff(object sender, RoutedEventArgs e)
        {

        }
        #region ndb
        private void NdBBWMinus_Click(object sender, RoutedEventArgs e)
        {
            rcv.NdBLevel -= 0.1m;
            rcv.Markers[0].Funk2 = rcv.NdBLevel;
        }

        private void NdBBWPlus_Click(object sender, RoutedEventArgs e)
        {
            rcv.NdBLevel += 0.1m;
            rcv.Markers[0].Funk2 = rcv.NdBLevel;
        }
        private void NdBLevel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rcv.NdBLevel = help.KeyDownDecimal(rcv.NdBLevel, (TextBox)sender, e);
                rcv.Markers[0].Funk2 = rcv.NdBLevel;
            }
        }
        private void NdBLevel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            rcv.NdBLevel += ((decimal)help.MouseWheelOutStep(sender, e)) / 10;
            rcv.Markers[0].Funk2 = rcv.NdBLevel;
        }
        private void ndB_Expander_ExpandedChanged(object sender, RoutedEventArgs e)
        {
            rcv.SetMarkerNdB();
            //Expander exp = (Expander)sender;
            //SH.NdBState = exp.IsExpanded;
        }
        #endregion
        #region ChannelPower
        private void ChannelPowerBWMinus_Click(object sender, RoutedEventArgs e)
        {
            rcv.ChannelPowerBW -= rcv.FreqSpan / 100;
        }

        private void ChannelPowerBWPlus_Click(object sender, RoutedEventArgs e)
        {
            rcv.ChannelPowerBW += rcv.FreqSpan / 100;
        }
        private void ChannelPowerBW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rcv.ChannelPowerBW = help.KeyDownDecimal(rcv.ChannelPowerBW / 1000000, (TextBox)sender, e) * 1000000;
            }
        }
        private void ChannelPowerBW_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            rcv.ChannelPowerBW += ((decimal)help.MouseWheelOutStep(sender, e)) * rcv.FreqSpan / 100;
        }
        #endregion ChannelPower
        #endregion Markers





        private void Uni_GotMouseCapture(object sender, MouseEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.SelectAll();
        }
        private void Uni_PreKeyDownDecimal(object sender, TextCompositionEventArgs e)
        {
            help.CheckIsDecimal((TextBox)sender, e);
        }
        private void Uni_PreKeyDownInteger(object sender, TextCompositionEventArgs e)
        {
            help.CheckIsInt((TextBox)sender, e);
        }







        #endregion


        private void ANTENNA_Click(object sender, RoutedEventArgs e)
        {
            if (rcv.AntennaSelected == 0)
            {
                rcv.AntennaSelected = 1;
            }
            else
            {
                rcv.AntennaSelected = 0;
            }
            rcv.SetAntenna(rcv.AntennaSelected);
        }

        
    }
}
