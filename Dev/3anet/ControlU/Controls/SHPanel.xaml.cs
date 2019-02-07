using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Логика взаимодействия для SHPanel.xaml
    /// </summary>
    public partial class SHPanel : UserControl
    {
        Equipment.SignalHound SH = MainWindow.SHReceiver;
        Helpers.Helper help;
        public SHPanel()
        {
            help = new Helpers.Helper();
            InitializeComponent();
            //this.DataContext = Sett;
            MainGrid.DataContext = SH;
            PrintScreen.DataContext = App.Sett.Equipments_Settings.RuSReceiver;
        }

        #region доп кнопки
        private void PrintScreen_Click(object sender, RoutedEventArgs e)
        {
            if (App.Lic)
            {
                bool fromname = false;
                if (App.Sett.Equipments_Settings.SignalHound.PrintScreenType == 0)
                {
                    DialogWindow dialog = new DialogWindow(this.FindResource("EnterFileName").ToString(), "ControlU");
                    dialog.Width = 250;
                    dialog.ResponseText = SH.ScreenName;
                    if (dialog.ShowDialog() == true)
                    {
                        fromname = true;
                        SH.ScreenName = dialog.ResponseText;
                    }
                }
                else if (App.Sett.Equipments_Settings.SignalHound.PrintScreenType == 1)
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
                    SH.ScreenName = "Screen_" + string.Format("{0:00000}", ind);
                }
                else if (App.Sett.Equipments_Settings.SignalHound.PrintScreenType == 2)
                {
                    SH.ScreenName = MainWindow.gps.LocalTime.ToString("yyyy-MM-dd H.mm.ss.fff");
                }
                else if (App.Sett.Equipments_Settings.SignalHound.PrintScreenType == 3)
                {
                    Controls.FreqConverterDecimal fcdp = new Controls.FreqConverterDecimal();
                    SH.ScreenName = (string)fcdp.Convert(SH.FreqCentr, null, null, null) + " " + MainWindow.gps.LocalTime.ToString("yyyy-MM-dd H.mm.ss.fff");
                }
                else if (App.Sett.Equipments_Settings.SignalHound.PrintScreenType == 4)
                {
                    Controls.FreqConverterDecimal fcdp = new Controls.FreqConverterDecimal();
                    SH.ScreenName = MainWindow.gps.LocalTime.ToString("yyyy-MM-dd H.mm.ss.fff") + " " + (string)fcdp.Convert(SH.FreqCentr, null, null, null);
                }
                if ((App.Sett.Equipments_Settings.SignalHound.PrintScreenType == 0 && fromname) || App.Sett.Equipments_Settings.SignalHound.PrintScreenType != 0)
                {
                    Equipment.PrintScreen ps = new Equipment.PrintScreen();
                    ps.InstrType = (int)Equipment.InstrumentType.SignalHound;

                    if (SH.Trace1Type.UI != new Equipment.AllTraceTypes().TraceTypes[6].UI)
                    {
                        ps.Trace1State = true;
                        ps.Trace1Legend = SH.Trace1Type.UI + "(" + SH.Detector.UI + ")";
                        if (SH.Trace1Type.Parameter == "1")
                        { ps.Trace1Legend += "[" + SH.Trace1AveragedList.NumberOfSweeps.ToString() + "/" + SH.Trace1AveragedList.AveragingCount.ToString() + "]"; }
                        if (SH.Trace1Type.UI == "2")
                        { ps.Trace1Legend += "[" + SH.Trace1TrackedList.NumberOfSweeps.ToString() + "/" + SH.Trace1TrackedList.TrackingCount.ToString() + "]"; }
                        ps.Trace1 = SH.Trace1;
                    }
                    if (SH.Trace2Type.UI != new Equipment.AllTraceTypes().TraceTypes[6].UI)
                    {
                        ps.Trace2State = true;
                        ps.Trace2Legend = SH.Trace2Type.UI + "(" + SH.Detector.UI + ")";
                        if (SH.Trace2Type.Parameter == "1")
                        { ps.Trace2Legend += "[" + SH.Trace2AveragedList.NumberOfSweeps.ToString() + "/" + SH.Trace2AveragedList.AveragingCount.ToString() + "]"; }
                        if (SH.Trace2Type.UI == "2")
                        { ps.Trace2Legend += "[" + SH.Trace2TrackedList.NumberOfSweeps.ToString() + "/" + SH.Trace2TrackedList.TrackingCount.ToString() + "]"; }
                        ps.Trace2 = SH.Trace2;
                    }
                    if (SH.Trace3Type.UI != new Equipment.AllTraceTypes().TraceTypes[6].UI)
                    {
                        ps.Trace3State = true;
                        ps.Trace3Legend = SH.Trace3Type.UI + "(" + SH.Detector.UI + ")";
                        if (SH.Trace3Type.Parameter == "1")
                        { ps.Trace3Legend += "[" + SH.Trace3AveragedList.NumberOfSweeps.ToString() + "/" + SH.Trace3AveragedList.AveragingCount.ToString() + "]"; }
                        if (SH.Trace3Type.UI == "2")
                        { ps.Trace3Legend += "[" + SH.Trace3TrackedList.NumberOfSweeps.ToString() + "/" + SH.Trace3TrackedList.TrackingCount.ToString() + "]"; }
                        ps.Trace3 = SH.Trace3;
                    }
                    ps.ActualPoints = SH.Trace1.Length;
                    ps.RefLevel = SH.RefLevel;
                    ps.Range = SH.Range;
                    ps.LevelUnit = SH.LevelUnit;
                    ps.Att = SH.AttSelected.UI;
                    ps.RBW = SH.RBW;
                    ps.VBW = SH.VBW;
                    ps.SWT = SH.SweepTime;
                    ps.Mode = SH.DeviceMode.UI;
                    ps.PreAmp = SH.GainSelected.UI;
                    ps.InstrManufacrure = "Signal Hound";
                    ps.InstrModel = SH.Device_Type;
                    ps.Location = new Point((double)MainWindow.gps.LongitudeDecimal, (double)MainWindow.gps.LatitudeDecimal);
                    ps.FreqStart = SH.FreqStart;
                    ps.FreqStop = SH.FreqStop;
                    ps.FreqCentr = SH.FreqCentr;
                    ps.FreqSpan = SH.FreqSpan;
                    ps.Freq_CentrSpan_StartStop = SH.Freq_CentrSpan_StartStop;
                    ps.OverLoad = SH.RFOverload ? "RF Overload" : "";
                    ps.DateTime = MainWindow.gps.LocalTime;
                    ps.Markers = SH.Markers;

                    if (SH.ChannelPowerState)
                    {
                        ps.ChannelPower = SH.ChannelPowerState;
                        ps.ChannelPowerBW = SH.ChannelPowerBW;
                        ps.ChannelPowerResult = SH.ChannelPowerResult;
                    }
                    ps.FilePath = App.Sett.Screen_Settings.ScreenFolder + "\\" + SH.ScreenName;//  MainWindow.gps.LocalTime.ToString();
                    ps.drawImageToPath();
                    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = this.FindResource("FileSaved").ToString().Replace("*NAME*", SH.ScreenName);
                }
            }
            else { ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = this.FindResource("LicenseFail").ToString(); }
        }
        private void PrintScreenType_Click(object sender, RoutedEventArgs e)
        {
            App.Sett.Equipments_Settings.SignalHound.PrintScreenType = int.Parse((string)((MenuItem)sender).Tag);
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
                    if (SH.ScreenName == "")
                    { System.Diagnostics.Process.Start(@App.Sett.Screen_Settings.ScreenFolder); }
                    else
                    {
                        string argument = "/select, \"" + App.Sett.Screen_Settings.ScreenFolder + @"\" + SH.ScreenName + "." + @App.Sett.Screen_Settings.SaveScreenImageFormat.ToString() + "\"";
                        System.Diagnostics.Process.Start("explorer.exe", argument);
                    }
                }
                else
                {
                    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = this.FindResource("ScreenShotPathFail").ToString();//"Путь сохранения скриншотов не задан!";
                }
            }
            catch { }
        }
        private void PresetGui(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region FREQ
        private void FreqCentrMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.FreqCentr -= SH.FreqSpan / 100;
            SH.SetFreqCentrSqeeping(SH.FreqCentr);
        }
        private void FreqCentrPlus_Click(object sender, RoutedEventArgs e)
        {
            SH.FreqCentr += SH.FreqSpan / 100;
            SH.SetFreqCentrSqeeping(SH.FreqCentr);
        }
        private void FreqCentr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SH.FreqCentr = help.KeyDownDecimal(SH.FreqCentr / 1000000, (TextBox)sender, e) * 1000000;
                SH.SetFreqCentrSqeeping(SH.FreqCentr);
            }
        }
        private void FreqCentr_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            SH.FreqCentr += ((decimal)help.MouseWheelOutStep(sender, e)) * SH.FreqSpan / 100;
            SH.SetFreqCentrSqeeping(SH.FreqCentr);
        }
        ///////////////////////////////////////
        private void FreqSpanMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.FreqSpan -= SH.FreqSpan / 100;
            SH.SetFreqSpanSqeeping(SH.FreqSpan);
        }
        private void FreqSpanPlus_Click(object sender, RoutedEventArgs e)
        {
            SH.FreqSpan += SH.FreqSpan / 100;
            SH.SetFreqSpanSqeeping(SH.FreqSpan);
        }
        private void FreqSpan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SH.FreqSpan = help.KeyDownDecimal(SH.FreqSpan / 1000000, (TextBox)sender, e) * 1000000;
                SH.SetFreqSpanSqeeping(SH.FreqSpan);
            }
        }
        private void FreqSpan_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            SH.FreqSpan += ((decimal)help.MouseWheelOutStep(sender, e)) * SH.FreqSpan / 100;
            SH.SetFreqSpanSqeeping(SH.FreqSpan);
        }
        ///////////////////////////////////////
        private void FreqStartMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.FreqStart -= SH.FreqSpan / 100;
            SH.SetFreqStartSqeeping(SH.FreqStart);
        }
        private void FreqStartPlus_Click(object sender, RoutedEventArgs e)
        {
            SH.FreqStart += SH.FreqSpan / 100;
            SH.SetFreqStartSqeeping(SH.FreqStart);
        }
        private void FreqStart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SH.FreqStart = help.KeyDownDecimal(SH.FreqSpan / 1000000, (TextBox)sender, e) * 1000000;
                SH.SetFreqStartSqeeping(SH.FreqStart);
            }
        }
        private void FreqStart_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            SH.FreqStart += ((decimal)help.MouseWheelOutStep(sender, e)) * SH.FreqSpan / 100;
            SH.SetFreqStartSqeeping(SH.FreqStart);
        }
        ///////////////////////////////////////
        private void FreqStopMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.FreqStop -= SH.FreqSpan / 100;
            SH.SetFreqStopSqeeping(SH.FreqStop);
        }
        private void FreqStopPlus_Click(object sender, RoutedEventArgs e)
        {
            SH.FreqStop += SH.FreqSpan / 100;
            SH.SetFreqStopSqeeping(SH.FreqStop);
        }
        private void FreqStop_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SH.FreqStop = help.KeyDownDecimal(SH.FreqSpan / 1000000, (TextBox)sender, e) * 1000000;
                SH.SetFreqStopSqeeping(SH.FreqStop);
            }
        }
        private void FreqStop_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            SH.FreqStop += ((decimal)help.MouseWheelOutStep(sender, e)) * SH.FreqSpan / 100;
            SH.SetFreqStopSqeeping(SH.FreqStop);
        }
        #endregion FREQ

        #region RBW / VBW
        private void RBWMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.RBWIndex--;
            SH.SetRBWFromIndex(SH.RBWIndex);
        }
        private void RBWPlus_Click(object sender, RoutedEventArgs e)
        {
            SH.RBWIndex++;
            SH.SetRBWFromIndex(SH.RBWIndex);
        }
        private void AutoRBW_CheckChanged(object sender, RoutedEventArgs e)
        {

        }
        ///////////////////////////////////////
        private void VBWMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.AutoVBW = false;
            SH.VBWIndex--;
            SH.SetVBWFromIndex(SH.VBWIndex);
        }
        private void VBWPlus_Click(object sender, RoutedEventArgs e)
        {
            SH.AutoVBW = false;
            SH.VBWIndex++;
            SH.SetVBWFromIndex(SH.VBWIndex);
        }
        private void AutoVBW_CheckChanged(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Sweep
        private void SweepTimeMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.SweepTime -= help.PlMntime(SH.SweepTime);
            SH.SetSweepTime(SH.SweepTime);
        }
        private void SweepTimePlus_Click(object sender, RoutedEventArgs e)
        {
            SH.SweepTime += help.PlMntime(SH.SweepTime);
            SH.SetSweepTime(SH.SweepTime);
        }
        private void SweepTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string temp = (sender as System.Windows.Controls.TextBox).Text;
                string t = string.Empty;
                foreach (char c in temp)
                {
                    if (Char.IsDigit(c) || c == '.' || c == ',')
                    {
                        t += c;
                    }
                }
                SH.SweepTime = decimal.Parse(t.Replace(",", "."), CultureInfo.InvariantCulture);
                SH.SetSweepTime(SH.SweepTime);
            }

        }
        private void SweepTime_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int step = help.MouseWheelOutStep(sender, e);
            if (step > 0) SH.SweepTime += help.PlMntime(SH.SweepTime);
            if (step < 0) SH.SweepTime -= help.PlMntime(SH.SweepTime);
            SH.SetSweepTime(SH.SweepTime);
        }
        private void AutoSweepTime_CheckChanged(object sender, RoutedEventArgs e)
        {

        }
        private void RealTime_Click(object sender, RoutedEventArgs e)
        {
            SH.DeviceModeIndex = ((ComboBox)sender).SelectedIndex;
            SH.SetDeviceMode(SH.BBDeviceMode);
            //SHReceiver.SH_dm += SHReceiver.SetMode;
            //SHReceiver.SetMode(Equipment.SignalHound.BB_Mode.BB_REAL_TIME);
        }
        private void DeviceModeIndex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        #endregion

        #region Amplitude
        private void RefLevelMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.RefLevel--;
            SH.SetRefLevel(SH.RefLevel);
        }
        private void RefLevelPlus_Click(object sender, RoutedEventArgs e)
        {
            SH.RefLevel++;
            SH.SetRefLevel(SH.RefLevel);
        }
        private void RefLevel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            SH.RefLevel += (decimal)help.MouseWheelOutStep(sender, e);
            SH.SetRefLevel(SH.RefLevel);
        }
        private void RangeMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.Range -= 10;
        }
        private void RangePlus_Click(object sender, RoutedEventArgs e)
        {
            SH.Range += 10;
        }
        private void Range_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            SH.Range += (decimal)help.MouseWheelOutStep(sender, e) * 10;
        }
        private void AmpUnitGui_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void AmpUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void GainMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.GainIndex--;
            SH.SetGain(SH.Gain);
        }

        private void GainPlus_Click(object sender, RoutedEventArgs e)
        {
            SH.GainIndex++;
            SH.SetGain(SH.Gain);
        }
        private void Gain_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            SH.GainIndex += help.MouseWheelOutStep(sender, e);
            SH.SetGain(SH.Gain);
        }
        private void AttMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.AttIndex--;
            SH.SetAtt(SH.Att);
        }
        private void AttPlus_Click(object sender, RoutedEventArgs e)
        {
            SH.AttIndex++;
            SH.SetAtt(SH.Att);
        }
        private void Att_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            SH.AttIndex += help.MouseWheelOutStep(sender, e);
            SH.SetAtt(SH.Att);
        }
        #endregion Amplitude

        #region Trace       
        private void VideoUnitTypeIndex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SH.VideoUnitTypeIndex = ((ComboBox)sender).SelectedIndex;
            SH.SetVideoUnit(SH.VideoUnit);
        }
        private void DetectorTypeIndex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SH.DetectorTypeIndex = ((ComboBox)sender).SelectedIndex;
            SH.SetDetector(SH.BBDetector);
        }

        #region Trace 1      
        private void Trace1Reset_Click(object sender, RoutedEventArgs e)
        {
            SH.Trace1Reset = true;
        }
        private void Trace1AverageMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.Trace1AveragedList.AveragingCount--;
        }
        private void Trace1AveragePlus_Click(object sender, RoutedEventArgs e)
        {
            SH.Trace1AveragedList.AveragingCount++;
        }
        private void Trace1Average_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SH.Trace1AveragedList.AveragingCount = help.KeyDownUInt(SH.Trace1AveragedList.AveragingCount, (TextBox)sender, e);
            }
        }

        private void Trace1TrackingMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.Trace1TrackedList.TrackingCount--;
        }
        private void Trace1TrackingPlus_Click(object sender, RoutedEventArgs e)
        {
            SH.Trace1TrackedList.TrackingCount++;
        }
        private void Trace1Tracking_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SH.Trace1TrackedList.TrackingCount = help.KeyDownUInt(SH.Trace1TrackedList.TrackingCount, (TextBox)sender, e);
            }
        }
        #endregion Trace 1
        #region Trace 2      
        private void Trace2Reset_Click(object sender, RoutedEventArgs e)
        {
            SH.Trace2Reset = true;
        }
        private void Trace2AverageMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.Trace2AveragedList.AveragingCount--;
        }
        private void Trace2AveragePlus_Click(object sender, RoutedEventArgs e)
        {
            SH.Trace2AveragedList.AveragingCount++;
        }
        private void Trace2Average_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SH.Trace2AveragedList.AveragingCount = help.KeyDownUInt(SH.Trace2AveragedList.AveragingCount, (TextBox)sender, e);
            }
        }

        private void Trace2TrackingMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.Trace2TrackedList.TrackingCount--;
        }
        private void Trace2TrackingPlus_Click(object sender, RoutedEventArgs e)
        {
            SH.Trace2TrackedList.TrackingCount++;
        }
        private void Trace2Tracking_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SH.Trace2TrackedList.TrackingCount = help.KeyDownUInt(SH.Trace2TrackedList.TrackingCount, (TextBox)sender, e);
            }
        }
        #endregion Trace 2
        #region Trace 3   
        private void Trace3Reset_Click(object sender, RoutedEventArgs e)
        {
            SH.Trace3Reset = true;
        }
        private void Trace3AverageMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.Trace3AveragedList.AveragingCount--;
        }
        private void Trace3AveragePlus_Click(object sender, RoutedEventArgs e)
        {
            SH.Trace3AveragedList.AveragingCount++;
        }
        private void Trace3Average_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SH.Trace3AveragedList.AveragingCount = help.KeyDownUInt(SH.Trace3AveragedList.AveragingCount, (TextBox)sender, e);
            }
        }

        private void Trace3TrackingMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.Trace3TrackedList.TrackingCount--;
        }
        private void Trace3TrackingPlus_Click(object sender, RoutedEventArgs e)
        {
            SH.Trace3TrackedList.TrackingCount++;
        }
        private void Trace3Tracking_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SH.Trace3TrackedList.TrackingCount = help.KeyDownUInt(SH.Trace3TrackedList.TrackingCount, (TextBox)sender, e);
            }
        }
        #endregion Trace 3
        #endregion Trace

        #region Markers
        private void MarkerState_Click(object sender, RoutedEventArgs e)
        {
            Equipment.Marker m = (Equipment.Marker)((Button)sender).Tag;
            SH.SetMarkerState(m, !m.StateNew);
        }
        private void MarkerFreqMinus_Click(object sender, RoutedEventArgs e)
        {
            Equipment.Marker m = (Equipment.Marker)((RepeatButton)sender).Tag;
            m.IndexOnTrace--;
            SH.SetMarkerFromIndex(m, m.IndexOnTrace);

        }
        private void MarkerFreqPlus_Click(object sender, RoutedEventArgs e)
        {
            Equipment.Marker m = (Equipment.Marker)((RepeatButton)sender).Tag;
            m.IndexOnTrace++;
            SH.SetMarkerFromIndex(m, m.IndexOnTrace);
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
                SH.SetMarkerFromFreq(m, f);
            }
        }
        private void MarkerPeakSearch_Click(object sender, RoutedEventArgs e)
        {
            Equipment.Marker m = (Equipment.Marker)((Button)sender).Tag;
            SH.SetMarkerPeakSearch(m);
        }
        private void MarkerType_Click(object sender, RoutedEventArgs e)
        {
            Equipment.Marker m = (Equipment.Marker)((Button)sender).Tag;
            SH.SetMarkerChangeType(m);
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
            SH.NdBLevel -= 0.1m;
            SH.Markers[0].Funk2 = SH.NdBLevel;
        }

        private void NdBBWPlus_Click(object sender, RoutedEventArgs e)
        {
            SH.NdBLevel += 0.1m;
            SH.Markers[0].Funk2 = SH.NdBLevel;
        }
        private void NdBLevel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SH.NdBLevel = help.KeyDownDecimal(SH.NdBLevel, (TextBox)sender, e);
                SH.Markers[0].Funk2 = SH.NdBLevel;
            }
        }
        private void NdBLevel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            SH.NdBLevel += ((decimal)help.MouseWheelOutStep(sender, e)) / 10;
            SH.Markers[0].Funk2 = SH.NdBLevel;
        }
        private void ndB_Expander_ExpandedChanged(object sender, RoutedEventArgs e)
        {
            SH.SetMarkerNdB();
            //Expander exp = (Expander)sender;
            //SH.NdBState = exp.IsExpanded;
        }

        #endregion
        #endregion Markers

        #region Meas
        private void ChannelPowerBWMinus_Click(object sender, RoutedEventArgs e)
        {
            SH.ChannelPowerBW -= SH.FreqSpan / 100;
        }

        private void ChannelPowerBWPlus_Click(object sender, RoutedEventArgs e)
        {
            SH.ChannelPowerBW += SH.FreqSpan / 100;
        }
        private void ChannelPowerBW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SH.ChannelPowerBW = help.KeyDownDecimal(SH.ChannelPowerBW / 1000000, (TextBox)sender, e) * 1000000;
            }
        }
        private void ChannelPowerBW_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            SH.ChannelPowerBW += ((decimal)help.MouseWheelOutStep(sender, e)) * SH.FreqSpan / 100;
        }
        #endregion Meas

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


    }
}
