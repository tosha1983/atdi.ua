using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Controls.Primitives;

namespace ControlU.Controls
{
    /// <summary>
    /// Логика взаимодействия для AnPanel_v2.xaml
    /// </summary>
    public partial class AnPanel_v2 : UserControl
    {
        Equipment.Analyzer an = MainWindow.An;
        Helpers.Helper help;

        public AnPanel_v2()
        {
            help = new Helpers.Helper();
            InitializeComponent();
            MainGrid.DataContext = an;
            PrintScreen.DataContext = App.Sett.Equipments_Settings.SpectrumAnalyzer;
        }
        #region доп кнопки
        private void PrintScreen_Click(object sender, RoutedEventArgs e)
        {
            if (App.Lic)
            {
                bool fromname = false;
                if (App.Sett.Equipments_Settings.SpectrumAnalyzer.PrintScreenType == 0)
                {
                    DialogWindow dialog = new DialogWindow(this.FindResource("EnterFileName").ToString(), "ControlU");
                    dialog.Width = 250;
                    dialog.ResponseText = an.ScreenName;
                    if (dialog.ShowDialog() == true)
                    {
                        fromname = true;
                        an.ScreenName = dialog.ResponseText;
                    }
                }
                else if (App.Sett.Equipments_Settings.SpectrumAnalyzer.PrintScreenType == 1)
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
                    an.ScreenName = "Screen_" + string.Format("{0:00000}", ind);
                }
                else if (App.Sett.Equipments_Settings.SpectrumAnalyzer.PrintScreenType == 2)
                {
                    an.ScreenName = MainWindow.gps.LocalTime.ToString("yyyy-MM-dd H.mm.ss.fff");
                }
                else if (App.Sett.Equipments_Settings.SpectrumAnalyzer.PrintScreenType == 3)
                {
                    Controls.FreqConverterDecimal fcdp = new Controls.FreqConverterDecimal();
                    an.ScreenName = (string)fcdp.Convert(an.FreqCentr, null, null, null) + " " + MainWindow.gps.LocalTime.ToString("yyyy-MM-dd H.mm.ss.fff");
                }
                else if (App.Sett.Equipments_Settings.SpectrumAnalyzer.PrintScreenType == 4)
                {
                    Controls.FreqConverterDecimal fcdp = new Controls.FreqConverterDecimal();
                    an.ScreenName = MainWindow.gps.LocalTime.ToString("yyyy-MM-dd H.mm.ss.fff") + " " + (string)fcdp.Convert(an.FreqCentr, null, null, null);
                }
                if ((App.Sett.Equipments_Settings.SpectrumAnalyzer.PrintScreenType == 0 && fromname) || App.Sett.Equipments_Settings.SpectrumAnalyzer.PrintScreenType != 0)
                {
                    if (App.Sett.Screen_Settings.SaveScreenFromInstr == true)
                    {
                        an.GetScreen();
                    }
                    else
                    {
                        Equipment.PrintScreen ps = new Equipment.PrintScreen();
                        ps.InstrType = (int)Equipment.InstrumentType.SpectrumAnalyzer;
                        if (an.Trace1Type.UI != new Equipment.AllTraceTypes().TraceTypes[6].UI)
                        {
                            ps.Trace1State = true;
                            ps.Trace1Legend = an.Trace1Type.UI + "(" + an.Trace1Detector.UI + ")";
                            if (an.Trace1Type.UI == "Avarege")
                            { ps.Trace1Legend += "[" + an.NumberOfSweeps.ToString() + "/" + an.AveragingCount.ToString() + "]"; }
                            ps.Trace1 = an.Trace1;
                        }
                        if (an.Trace2Type.UI != new Equipment.AllTraceTypes().TraceTypes[6].UI)
                        {
                            ps.Trace2State = true;
                            ps.Trace2Legend = an.Trace2Type.UI + "(" + an.Trace2Detector.UI + ")";
                            if (an.Trace2Type.UI == "Avarege")
                            { ps.Trace2Legend += "[" + an.NumberOfSweeps.ToString() + "/" + an.AveragingCount.ToString() + "]"; }
                            ps.Trace2 = an.Trace2;
                        }
                        if (an.Trace3Type.UI != new Equipment.AllTraceTypes().TraceTypes[6].UI)
                        {
                            ps.Trace3State = true;
                            ps.Trace3Legend = an.Trace3Type.UI + "(" + an.Trace3Detector.UI + ")";
                            if (an.Trace3Type.UI == "Avarege")
                            { ps.Trace3Legend += "[" + an.NumberOfSweeps.ToString() + "/" + an.AveragingCount.ToString() + "]"; }
                            ps.Trace3 = an.Trace3;
                        }
                        ps.ActualPoints = an.Trace1.Length;
                        ps.RefLevel = an.RefLevel;
                        ps.Range = an.Range;
                        ps.LevelUnit = an.LevelUnit;
                        ps.Att = ((int)an.AttLevel).ToString() + " dB";
                        ps.RBW = an.RBW;
                        ps.VBW = an.VBW;
                        ps.SWT = an.SweepTime;
                        ps.Mode = an.SweepTypeSelected.UI;
                        ps.PreAmp = an.PreAmp ? "ON" : "OFF";
                        ps.InstrManufacrure = an.UniqueData.InstrManufacrureData.UIShort;
                        ps.InstrModel = an.UniqueData.InstrModel;
                        ps.Location = new Point((double)MainWindow.gps.LongitudeDecimal, (double)MainWindow.gps.LatitudeDecimal);
                        ps.FreqStart = an.FreqStart;
                        ps.FreqStop = an.FreqStop;
                        ps.FreqCentr = an.FreqCentr;
                        ps.FreqSpan = an.FreqSpan;
                        ps.Freq_CentrSpan_StartStop = an.Freq_CentrSpan_StartStop;
                        ps.OverLoad = an.PowerRegisterStr;
                        ps.DateTime = MainWindow.gps.LocalTime;
                        ps.Markers = an.Markers;

                        if (an.ChannelPowerState)
                        {
                            ps.ChannelPower = an.ChannelPowerState;
                            ps.ChannelPowerBW = an.ChannelPowerBW;
                            ps.ChannelPowerResult = an.ChannelPowerResult;
                        }
                        ps.FilePath = App.Sett.Screen_Settings.ScreenFolder + "\\" + an.ScreenName;//
                        ps.drawImageToPath();
                    }
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = this.FindResource("FileSaved").ToString().Replace("*NAME*", an.ScreenName);
                }
            }
            else
            {
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = this.FindResource("LicenseFail").ToString();
            }
        }
        private void PrintScreenType_Click(object sender, RoutedEventArgs e)
        {
            App.Sett.Equipments_Settings.SpectrumAnalyzer.PrintScreenType = int.Parse((string)((MenuItem)sender).Tag);
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
                    if (an.ScreenName == "")
                    { System.Diagnostics.Process.Start(@App.Sett.Screen_Settings.ScreenFolder); }
                    else
                    {
                        string argument = "/select, \"" + App.Sett.Screen_Settings.ScreenFolder + @"\" + an.ScreenName + "." + @App.Sett.Screen_Settings.SaveScreenImageFormat.ToString() + "\"";
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
        private void Preset(object sender, RoutedEventArgs e)
        {
            an.SetPreset();
        }
        private void InstrOff_Click(object sender, RoutedEventArgs e)
        {
            an.ThisInstrShutdown();
        }
        #endregion доп кнопки

        #region FREQ
        private void FreqCentrMinus_Click(object sender, RoutedEventArgs e)
        {
            an.FreqCentr -= an.FreqSpan / 100;
            an.SetFreqCentr(an.FreqCentr);
        }
        private void FreqCentrPlus_Click(object sender, RoutedEventArgs e)
        {
            an.FreqCentr += an.FreqSpan / 100;
            an.SetFreqCentr(an.FreqCentr);
        }
        private void FreqCentr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                an.FreqCentr = help.KeyDownDecimal(an.FreqCentr / 1000000, (TextBox)sender, e) * 1000000;
                an.SetFreqCentr(an.FreqCentr);
            }
        }
        private void FreqCentr_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            an.FreqCentr += ((decimal)help.MouseWheelOutStep(sender, e)) * an.FreqSpan / 100;
            an.SetFreqCentr(an.FreqCentr);
        }
        ///////////////////////////////////////
        private void FreqSpanMinus_Click(object sender, RoutedEventArgs e)
        {
            an.FreqSpan -= an.FreqSpan / 100;
            an.SetFreqSpan(an.FreqSpan);
        }
        private void FreqSpanPlus_Click(object sender, RoutedEventArgs e)
        {
            an.FreqSpan += an.FreqSpan / 100;
            an.SetFreqSpan(an.FreqSpan);
        }
        private void FreqSpan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                an.FreqSpan = help.KeyDownDecimal(an.FreqSpan / 1000000, (TextBox)sender, e) * 1000000;
                an.SetFreqSpan(an.FreqSpan);
            }
        }
        private void FreqSpan_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            an.FreqSpan += ((decimal)help.MouseWheelOutStep(sender, e)) * an.FreqSpan / 100;
            an.SetFreqSpan(an.FreqSpan);
        }
        ///////////////////////////////////////
        private void FreqStartMinus_Click(object sender, RoutedEventArgs e)
        {
            an.FreqStart -= an.FreqSpan / 100;
            an.SetFreqStart(an.FreqStart);
        }
        private void FreqStartPlus_Click(object sender, RoutedEventArgs e)
        {
            an.FreqStart += an.FreqSpan / 100;
            an.SetFreqStart(an.FreqStart);
        }
        private void FreqStart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                an.FreqStart = help.KeyDownDecimal(an.FreqSpan / 1000000, (TextBox)sender, e) * 1000000;
                an.SetFreqStart(an.FreqStart);
            }
        }
        private void FreqStart_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            an.FreqStart += ((decimal)help.MouseWheelOutStep(sender, e)) * an.FreqSpan / 100;
            an.SetFreqStart(an.FreqStart);
        }
        ///////////////////////////////////////
        private void FreqStopMinus_Click(object sender, RoutedEventArgs e)
        {
            an.FreqStop -= an.FreqSpan / 100;
            an.SetFreqStop(an.FreqStop);
        }
        private void FreqStopPlus_Click(object sender, RoutedEventArgs e)
        {
            an.FreqStop += an.FreqSpan / 100;
            an.SetFreqStop(an.FreqStop);
        }
        private void FreqStop_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                an.FreqStop = help.KeyDownDecimal(an.FreqSpan / 1000000, (TextBox)sender, e) * 1000000;
                an.SetFreqStop(an.FreqStop);
            }
        }
        private void FreqStop_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            an.FreqStop += ((decimal)help.MouseWheelOutStep(sender, e)) * an.FreqSpan / 100;
            an.SetFreqStop(an.FreqStop);
        }
        #endregion FREQ

        #region RBW / VBW
        private void RBWMinus_Click(object sender, RoutedEventArgs e)
        {
            an.RBWIndex--;
            an.SetRBWFromIndex(an.RBWIndex);
        }
        private void RBWPlus_Click(object sender, RoutedEventArgs e)
        {
            an.RBWIndex++;
            an.SetRBWFromIndex(an.RBWIndex);
        }
        private void AutoRBW_CheckChanged(object sender, RoutedEventArgs e)
        {
            an.SetAutoRBW(an.AutoRBW);
        }
        ///////////////////////////////////////
        private void VBWMinus_Click(object sender, RoutedEventArgs e)
        {
            an.AutoVBW = false;
            an.VBWIndex--;
            an.SetVBWFromIndex(an.VBWIndex);
        }
        private void VBWPlus_Click(object sender, RoutedEventArgs e)
        {
            an.AutoVBW = false;
            an.VBWIndex++;
            an.SetVBWFromIndex(an.VBWIndex);
        }
        private void AutoVBW_CheckChanged(object sender, RoutedEventArgs e)
        {
            an.SetAutoVBW(an.AutoVBW);
        }
        #endregion

        #region Sweep

        private void SweepTimeMinus_Click(object sender, RoutedEventArgs e)
        {
            an.SweepTime -= help.PlMntime(an.SweepTime);
            an.SetSweepTime(an.SweepTime);
        }
        private void SweepTimePlus_Click(object sender, RoutedEventArgs e)
        {
            an.SweepTime += help.PlMntime(an.SweepTime);
            an.SetSweepTime(an.SweepTime);
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
                an.SweepTime = decimal.Parse(t.Replace(",", "."), CultureInfo.InvariantCulture);
                an.SetSweepTime(an.SweepTime);
            }
        }
        private void SweepTime_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int step = help.MouseWheelOutStep(sender, e);
            if (step > 0) an.SweepTime += help.PlMntime(an.SweepTime);
            if (step < 0) an.SweepTime -= help.PlMntime(an.SweepTime);
            an.SetSweepTime(an.SweepTime);
        }
        private void AutoSweepTime_CheckChanged(object sender, RoutedEventArgs e)
        {
            an.SetAutoSweepTime(an.AutoSweepTime);
        }
        private void SweepType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            an.SweepTypeSelected = (Equipment.Analyzer.SwpType)((ComboBox)sender).SelectedItem;
            an.SetAutoSweepType(an.SweepTypeSelected);
        }
        private void SweepPointsMinus_Click(object sender, RoutedEventArgs e)
        {
            an.SweepPointsIndex--;
            an.SetSweepPoints(an.SweepPoints);
        }
        private void SweepPointsPlus_Click(object sender, RoutedEventArgs e)
        {
            an.SweepPointsIndex++;
            an.SetSweepPoints(an.SweepPoints);
        }
        #endregion Sweep

        #region Amplitude
        private void RefLevelMinus_Click(object sender, RoutedEventArgs e)
        {
            an.RefLevel--;
            an.SetRefLevel(an.RefLevel);
        }
        private void RefLevelPlus_Click(object sender, RoutedEventArgs e)
        {
            an.RefLevel++;
            an.SetRefLevel(an.RefLevel);
        }
        private void RefLevel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            an.RefLevel += (decimal)help.MouseWheelOutStep(sender, e);
            an.SetRefLevel(an.RefLevel);
        }
        private void RangeMinus_Click(object sender, RoutedEventArgs e)
        {
            an.RangeIndex--;
            an.SetRangeFromIndex(an.RangeIndex);
        }
        private void RangePlus_Click(object sender, RoutedEventArgs e)
        {
            an.RangeIndex++;
            an.SetRangeFromIndex(an.RangeIndex);
        }
        private void Range_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            an.RangeIndex += help.MouseWheelOutStep(sender, e);
            an.SetRangeFromIndex(an.RangeIndex);
        }

        private void AmpUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            an.LevelUnitIndex = ((ComboBox)sender).SelectedIndex;
            an.SetLevelUnitFromIndex(an.LevelUnitIndex);
        }

        private void AttMinus_Click(object sender, RoutedEventArgs e)
        {
            an.AttLevel -= an.UniqueData.AttStep;
            an.SetAttLevel(an.AttLevel);
        }
        private void AttPlus_Click(object sender, RoutedEventArgs e)
        {
            an.AttLevel += an.UniqueData.AttStep;
            an.SetAttLevel(an.AttLevel);
        }
        private void Att_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            an.AttLevel += help.MouseWheelOutStep(sender, e) * an.UniqueData.AttStep;
            an.SetAttLevel(an.AttLevel);
        }
        private void AttAuto_CheckChanged(object sender, RoutedEventArgs e)
        {
            an.SetAutoAttLevel(an.AttAuto);
        }

        private void PreAmp_CheckChanged(object sender, RoutedEventArgs e)
        {
            an.SetPreAmp(an.PreAmp);
        }
        #endregion Amplitude

        #region Trace
        private void TraceReset_Click(object sender, RoutedEventArgs e)
        {
            int tr = int.Parse(((Button)sender).Tag.ToString());
            an.TraceNumberToReset = tr;
            an.SetResetTrace(an.TraceNumberToReset);
        }

        private void TraceAverageMinus_Click(object sender, RoutedEventArgs e)
        {
            an.AveragingCount--;
            an.SetAverageCount(an.AveragingCount);
        }
        private void TraceAveragePlus_Click(object sender, RoutedEventArgs e)
        {
            an.AveragingCount++;
            an.SetAverageCount(an.AveragingCount);
        }
        private void traceAverage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                an.AveragingCount = help.KeyDownUInt(an.AveragingCount, (TextBox)sender, e);
            }
        }
        private void AveragingCount_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            an.AveragingCount += help.MouseWheelOutStep(sender, e);
            an.SetAverageCount(an.AveragingCount);
        }

        #region Trace1
        private void Trace1Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            an.SetTraceType(1, an.Trace1Type);
        }
        private void Detector_1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            an.SetDetectorType(1, an.Trace1Detector);
        }
        #endregion Trace1

        #region Trace2
        private void Trace2Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            an.SetTraceType(2, an.Trace2Type);
        }
        private void Detector_2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            an.SetDetectorType(2, an.Trace2Detector);
        }
        #endregion Trace2

        #region Trace2
        private void Trace3Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            an.SetTraceType(3, an.Trace3Type);
        }
        private void Detector_3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            an.SetDetectorType(3, an.Trace3Detector);
        }
        #endregion Trace2
        #endregion Trace

        #region Markers
        private void MarkerState_Click(object sender, RoutedEventArgs e)
        {
            Equipment.Marker m = (Equipment.Marker)((Button)sender).Tag;

            an.SetMarkerState(m, !m.StateNew);
        }
        private void MarkerFreqMinus_Click(object sender, RoutedEventArgs e)
        {
            Equipment.Marker m = (Equipment.Marker)((RepeatButton)sender).Tag;
            m.IndexOnTrace--;
            an.SetMarkerFromIndex(m, m.IndexOnTrace);

        }
        private void MarkerFreqPlus_Click(object sender, RoutedEventArgs e)
        {
            Equipment.Marker m = (Equipment.Marker)((RepeatButton)sender).Tag;
            m.IndexOnTrace++;
            an.SetMarkerFromIndex(m, m.IndexOnTrace);
        }
        private void MarkerFreq_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Equipment.Marker m = (Equipment.Marker)((TextBox)sender).Tag;
            if (m.MarkerType == 0 || m.MarkerType == 3 || m.MarkerType == 4)//M/M+nDb
            {
                help.CheckIsDecimal((TextBox)sender, e);
            }
            else if (m.MarkerType == 1)//D
            {
                //e.Handled = true;
                //help.CheckIsDecimal((TextBox)sender, e);
                help.CheckIsDecimalWithMinus((TextBox)sender, e);
            }
        }
        private void MarkerFreq_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Equipment.Marker m = (Equipment.Marker)((TextBox)sender).Tag;
                decimal f = m.Freq;
                if (m.MarkerType == 0 || m.MarkerType == 3 || m.MarkerType == 4)
                { f = help.KeyDownDecimal(m.Freq / 1000000, (TextBox)sender, e) * 1000000; }
                else if (m.MarkerType == 1)
                {
                    f = m.MarkerParent.Freq + help.KeyDownDecimal(m.Funk1 / 1000000, (TextBox)sender, e) * 1000000;
                    int r = 0;
                }
                an.SetMarkerFromFreq(m, f);
            }
        }
        //private void MarkerFreq_MouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    Equipment.Marker m = (Equipment.Marker)((TextBox)sender).Tag;
        //    m.IndexOnTrace += help.MouseWheelOutStep(sender, e);
        //    an.SetMarkerFromIndex(m, m.IndexOnTrace);
        //}
        private void MarkerPeakSearch_Click(object sender, RoutedEventArgs e)
        {
            Equipment.Marker m = (Equipment.Marker)((Button)sender).Tag;
            an.SetMarkerPeakSearch(m);
        }
        private void MarkerType_Click(object sender, RoutedEventArgs e)
        {
            Equipment.Marker m = (Equipment.Marker)((Button)sender).Tag;
            an.SetMarkerChangeType(m);
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
            an.SetAllMarkerOff();
        }
        #region ndb
        private void ndB_ExpandedChanged(object sender, RoutedEventArgs e)
        {
            an.SetMarkerNdB();
            //Expander exp = (Expander)sender;
            //an.NdBState = exp.IsExpanded;
        }

        private void NdBBWMinus_Click(object sender, RoutedEventArgs e)
        {
            an.NdBLevel -= 0.1m;
            an.SetNdBLevel(an.NdBLevel);
        }
        private void NdBBWPlus_Click(object sender, RoutedEventArgs e)
        {
            an.NdBLevel += 0.1m;
            an.SetNdBLevel(an.NdBLevel);
        }
        private void NdBLevel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                an.NdBLevel = help.KeyDownDecimal(an.NdBLevel, (TextBox)sender, e);
                an.SetNdBLevel(an.NdBLevel);
            }
        }
        private void NdBLevel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            an.NdBLevel += ((decimal)help.MouseWheelOutStep(sender, e)) / 10;
            an.SetNdBLevel(an.NdBLevel);
        }
        #endregion ndb

        #region obw
        private void OBW_Expanded(object sender, RoutedEventArgs e)
        {
            an.SetMarkerOBW();
        }
        private void POBWMinus_Click(object sender, RoutedEventArgs e)
        {
            an.OBWPercent -= 0.1m;
            an.SetOBWPercent(an.OBWPercent);
        }
        private void POBWPlus_Click(object sender, RoutedEventArgs e)
        {
            an.OBWPercent += 0.1m;
            an.SetOBWPercent(an.OBWPercent);
        }
        private void OBWPercent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                an.OBWPercent = help.KeyDownDecimal(an.OBWPercent, (TextBox)sender, e);
                an.SetOBWPercent(an.OBWPercent);
            }
        }
        private void OBWPercent_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            an.OBWPercent += ((decimal)help.MouseWheelOutStep(sender, e)) / 10;
            an.SetOBWPercent(an.OBWPercent);
        }

        private void OBWChnlBWMinus_Click(object sender, RoutedEventArgs e)
        {
            an.OBWChnlBW -= an.FreqSpan / 100;
            an.SetOBWChnlBW(an.OBWChnlBW);
        }
        private void OBWChnlBWPlus_Click(object sender, RoutedEventArgs e)
        {
            an.OBWChnlBW += an.FreqSpan / 100;
            an.SetOBWChnlBW(an.OBWChnlBW);
        }
        private void OBWChnlBW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                an.OBWChnlBW = help.KeyDownDecimal(an.OBWChnlBW, (TextBox)sender, e);
                an.SetOBWChnlBW(an.OBWChnlBW);
            }
        }
        private void OBWChnlBW_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            an.OBWChnlBW += ((decimal)help.MouseWheelOutStep(sender, e)) * an.FreqSpan / 100;
            an.SetOBWChnlBW(an.OBWChnlBW);
        }
        #endregion obw

        #endregion Markers

        #region Meas
        #region ChannelPower
        private void ChannelPower_ExpandedChanged(object sender, RoutedEventArgs e)
        {
            an.SetChannelPower(an.ChannelPowerState);
        }
        private void ChannelPowerBWMinus_Click(object sender, RoutedEventArgs e)
        {
            an.ChannelPowerBW -= an.FreqSpan / 100;
            an.SetChannelPowerBW(an.ChannelPowerBW);
        }
        private void ChannelPowerBWPlus_Click(object sender, RoutedEventArgs e)
        {
            an.ChannelPowerBW += an.FreqSpan / 100;
            an.SetChannelPowerBW(an.ChannelPowerBW);
        }
        private void ChannelPowerBW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                an.ChannelPowerBW = help.KeyDownDecimal(an.ChannelPowerBW / 1000000, (TextBox)sender, e) * 1000000;
                an.SetChannelPowerBW(an.ChannelPowerBW);
            }
        }
        private void ChannelPowerBW_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            an.ChannelPowerBW += ((decimal)help.MouseWheelOutStep(sender, e)) * an.FreqSpan / 100;
            an.SetChannelPowerBW(an.ChannelPowerBW);
        }
        #endregion ChannelPower
        #region Transducer
        private void ActivateTransducer_Changed(object sender, RoutedEventArgs e)
        {
            Equipment.Analyzer.Transducer tr = (Equipment.Analyzer.Transducer)((CheckBox)sender).Tag;
            an.SetSelectedTransducer(tr);
        }
        private void ActivateTransducer_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            Equipment.Analyzer.Transducer tr = (Equipment.Analyzer.Transducer)(cb).Tag;

            an.SetSelectedTransducer(tr);
        }
        #endregion Transducer
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
