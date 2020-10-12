using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace ControlU.Controls
{
    /// <summary>
    /// Логика взаимодействия для DBPanel.xaml
    /// </summary>
    public partial class DBPanel : UserControl, INotifyPropertyChanged
    {
        string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        #region ATDI
        public bool ATDI_ShowSpectrum
        {
            get { return _ATDI_ShowSpectrum; }
            set { _ATDI_ShowSpectrum = value; OnPropertyChanged("ATDI_ShowSpectrum"); }
        }
        private bool _ATDI_ShowSpectrum = false;
        public System.Windows.Media.Imaging.BitmapImage ATDI_SpectrImg
        {
            get { return _ATDI_SpectrImg; }
            set { _ATDI_SpectrImg = value; OnPropertyChanged("ATDI_SpectrImg"); }
        }
        private System.Windows.Media.Imaging.BitmapImage _ATDI_SpectrImg;
        public MeasurementResult ATDI_SpectrData
        {
            get { return _ATDI_SpectrData; }
            set { _ATDI_SpectrData = value; OnPropertyChanged("ATDI_SpectrData"); }
        }
        private MeasurementResult _ATDI_SpectrData = new MeasurementResult() { };

        public object ATDI_ResultData;

        #endregion
        //public decimal[] ShowTracePoints
        //{
        //    get { return _ShowTracePoints; }
        //    set { _ShowTracePoints = value; OnPropertyChanged("ShowTracePoints"); }
        //}
        //private decimal[] _ShowTracePoints = new decimal[5] { 1, 5, 20, 50, 10 };


        public DBPanel()
        {
            InitializeComponent();
            MainGrid.DataContext = MainWindow.db_v2;
            WRLSMacBinding_DB.DataContext = MainWindow.db_v2;
            //maingrid.DataContext = this;


            Atdi_TabItem.DataContext = this;
            ATDI_ButtonPanel.DataContext = MainWindow.db_v2;
            ATDI_TasksData.DataContext = MainWindow.db_v2;

        }


        private void wrls_delete_binding(object sender, RoutedEventArgs e)
        {
            ////Get the clicked MenuItem
            //var menuItem = (MenuItem)sender;

            ////Get the ContextMenu to which the menuItem belongs
            //var contextMenu = (ContextMenu)menuItem.Parent;

            ////Find the placementTarget
            //var item = (DataGrid)contextMenu.PlacementTarget;

            ////Get the underlying item, that you cast to your object that is bound
            ////to the DataGrid (and has subject and state as property)
            //DB.WRLSMacBinding WRLSMacB = (DB.WRLSMacBinding)item.SelectedCells[0].Item;

            ////Remove the toDeleteFromBindedList object from your ObservableCollection
            ////yourObservableCollection.Remove(toDeleteFromBindedList);
            //MainWindow.db_v2.WRLSMacBindingDelete(WRLSMacB);

        }
        private void ShowSpectrum_Click(object sender, RoutedEventArgs e)
        {
            string selectedTag = (string)((Button)sender).Tag;
            if (selectedTag == "ATDI")
            { ATDI_ShowSpectrum = !ATDI_ShowSpectrum; }
            if (ATDI_ShowSpectrum)
            {
                Data_ColumnDefinition.Width = new GridLength(5, GridUnitType.Star);
                ResData_ColumnDefinition.Width = new GridLength(5, GridUnitType.Star);
            }
            else
            {
                ResData_ColumnDefinition.Width = new GridLength(0, GridUnitType.Pixel);
            }
        }




        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }




        #region ATDI////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ATDI_DownloadTasks_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.db_v2.ServerIsLoaded && MainWindow.atdi.RabbitIsUsed == false)
                MainWindow.atdi.Initialize();
        }
        private void ATDI_UploadTasks_Click(object sender, RoutedEventArgs e)
        {
            //string t = "";
            //foreach (DB.localatdi_meas_task ttask in MainWindow.db_v2.AtdiTasks)
            //{
            //    if (ttask.task_state == 2)
            //    {
            //        t = ttask.task_id;
            //    }
            //}
            //DB.localatdi_meas_task task = MainWindow.db_v2.AtdiTasks.Where(x => x.task_id == t).First();
            //List<Atdi.AppServer.Contracts.Sdrns.MeasSdrResults> r = MainWindow.db_v2.SendMeasDataFromResult(task);
            //MainWindow.atdi.SendResult(r);

        }
        private void ATDI_SendResult_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.db_v2.ServerIsLoaded && MainWindow.atdi.RabbitIsUsed == false)
                MainWindow.atdi.Initialize();
            if (sender is Button)
            {
                if (((Button)sender).Tag is DB.localatdi_meas_task)
                {
                    DB.localatdi_meas_task res = (DB.localatdi_meas_task)((Button)sender).Tag;
                    ATDI.AtdiDataConverter dc = new ATDI.AtdiDataConverter();
                    Atdi.DataModels.Sdrns.Device.MeasResults measResult = dc.ConvertToATDI(res);
                    DB.localatdi_result_state_data resdata = new DB.localatdi_result_state_data()
                    {
                        SaveInDB = false,
                        ResultId = measResult.ResultId,
                        DeliveryConfirmation = -1,
                        ResponseReceived = DateTime.MinValue,
                        ResultSended = DateTime.MinValue,
                    };
                    if (MainWindow.db_v2.ATDI_AddNewResultInfoToDB_v2(resdata, true, res.task_id))
                        res.ResultsInfo.Add(resdata);

                    MainWindow.atdi.SendResult(measResult, ref resdata);

                    MainWindow.db_v2.ATDI_UpdateResultInfoToDB_v2(res.ResultsInfo.ToArray(), true, res.task_id);
                }
                else if (((Button)sender).Tag is DB.localatdi_unknown_result)
                {
                    DB.localatdi_unknown_result res = (DB.localatdi_unknown_result)((Button)sender).Tag;
                    ATDI.AtdiDataConverter dc = new ATDI.AtdiDataConverter();
                    Atdi.DataModels.Sdrns.Device.MeasResults measResult = dc.ConvertToATDI(res);
                    DB.localatdi_result_state_data resdata = new DB.localatdi_result_state_data()
                    {
                        SaveInDB = false,
                        ResultId = measResult.ResultId,
                        DeliveryConfirmation = -1,
                        ResponseReceived = DateTime.MinValue,
                        ResultSended = DateTime.MinValue,
                    };
                    if (MainWindow.db_v2.ATDI_AddNewResultInfoToDB_v2(resdata, false, res.id))
                    {
                        res.ResultsInfo.Add(resdata);
                        //Array.Resize(ref res.ResultsInfo, res.ResultsInfo.Length + 1);
                        //res.ResultsInfo[res.ResultsInfo.Length - 1] = resdata;
                    }
                        
                    MainWindow.atdi.SendResult(measResult, ref resdata);

                    MainWindow.db_v2.ATDI_UpdateResultInfoToDB_v2(res.ResultsInfo.ToArray(), false, res.id);
                }
            }

        }
        private void ATDI_TaskState_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.db_v2.MeasMonState == false)
            {
                Button bt = (Button)sender;
                foreach (DB.localatdi_meas_task task in MainWindow.db_v2.AtdiTasks)
                {
                    if (task.task_id == (string)bt.Tag)
                    {
                        if (task.task_state == 0) { task.task_state = 1; }
                        else if (task.task_state == 1) { task.task_state = 2; }
                        else if (task.task_state == 2) { task.task_state = 0; }
                        MainWindow.db_v2.updateATDITaskStateInMeasTasks_v2((string)bt.Tag, task.task_state);
                    }
                }
            }
            else
            {
                if (MainWindow.db_v2.MeasMonState)
                    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("DB_TaskState_Warning").ToString();
            }
            //MainWindow.db_v2.OnPropertyChanged("UpdateTaskState");
        }
        private void DataInfoMenu_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);// (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }
        private void ATDI_DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is DB.localatdi_meas_task)
            {
                DB.localatdi_meas_task task = (DB.localatdi_meas_task)((Button)sender).Tag;
                Controls.Messages.Confirm msg = new Controls.Messages.Confirm
                (
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("Warning").ToString(),
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("DB_Task_Delete").ToString().Replace("*TaskNumber*", task.task_id)
                );
                msg.PropertyChanged += (Sender, Event) =>
                {
                    if (Event.PropertyName.Contains("Confirm"))
                    {
                        if (MainWindow.db_v2.DeleteTask(task))
                            ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("DB_Task_Deleted").ToString().Replace("*TaskNumber*", task.task_id);
                    }
                    else if (Event.PropertyName.Contains("Cancel"))
                    {
                        //System.Windows.MessageBox.Show("Cancel");
                    }
                };
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.ShowMessages_SP.Children.Add(msg);
            }
            else if (((Button)sender).Tag is DB.localatdi_unknown_result)
            {
                DB.localatdi_unknown_result unk = (DB.localatdi_unknown_result)((Button)sender).Tag;
                MonthToStringConverter msc = new MonthToStringConverter();
                string mm = (string)msc.Convert(unk.time_start.Month, null, null, null);
                if (unk.id != MainWindow.db_v2.AtdiUnknownResult.id)
                {
                    
                    Controls.Messages.Confirm msg = new Controls.Messages.Confirm
                     (
                     ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("Warning").ToString(),
                     ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("DB_Task_Delete").ToString().Replace("*TaskNumber*", mm + " " + unk.time_start.Year.ToString())
                     );
                    msg.PropertyChanged += (Sender, Event) =>
                    {
                        if (Event.PropertyName.Contains("Confirm"))
                        {
                            //if (MainWindow.db_v2.DeleteTask(unk))
                            //    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("DB_Task_Deleted").ToString().Replace("*TaskNumber*", task.task_id);
                        }
                    };
                    ((SplashWindow)App.Current.MainWindow).m_mainWindow.ShowMessages_SP.Children.Add(msg);
                }
                else
                {
                    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("DB_ImpossibleToDdelete").ToString().Replace("*TaskNumber*", mm + " " + unk.time_start.Year.ToString());
                }
            }
        }
        private void ATDI_DeleteResults_Click(object sender, RoutedEventArgs e)
        {            
            string id = "";
            if (((Button)sender).Tag is DB.localatdi_meas_task)
            {
                id = ((DB.localatdi_meas_task)((Button)sender).Tag).task_id;
            }
            else if (((Button)sender).Tag is DB.localatdi_unknown_result)
            {
                id = ((DB.localatdi_unknown_result)((Button)sender).Tag).id;
            }
            else if (((Button)sender).Tag is DB.Track)
            {
                id = ((DB.Track)((Button)sender).Tag).table_name;
                if (MainWindow.db_v2.MeasTrackState && id == MainWindow.db_v2.TracksDataSelected.table_name)
                {
                    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message =
                        ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("DBPanel_ResultCannotBeDeleted").ToString();
                    id = "";
                }
            }
            if (id != "")
            {
                Controls.Messages.Confirm msg = new Controls.Messages.Confirm
                  (
                  ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("Warning").ToString(),
                  ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("DB_Results_Delete").ToString().Replace("*TaskNumber*", id)
                  );
                msg.PropertyChanged += (Sender, Event) =>
                {
                    if (Event.PropertyName.Contains("Confirm"))
                    {
                        if (MainWindow.db_v2.DeleteResults(((Button)sender).Tag)) //if (MainWindow.db_v2.DeleteResults(type, id))
                            ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message =
                            ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("DB_Results_Deleted").ToString().Replace("*TaskNumber*", id);
                    }
                    else if (Event.PropertyName.Contains("Cancel"))
                    {
                    }
                };
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.ShowMessages_SP.Children.Add(msg);
            }
        }
        private void ATDI_SpectrumResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ATDI_ShowSpectrum == true)
            {
                //try
                //{
                DataGrid dataGrid = sender as DataGrid;
                if (dataGrid.SelectedItem is DB.localatdi_result_item)
                {
                    ATDI_ResultData = (DB.localatdi_result_item)dataGrid.SelectedItem;

                    ATDI_DataProcessingSpectrResult(ATDI_ResultData);
                    ATDI_DrawSpectrResult(ATDI_ResultData);
                }
                else if (dataGrid.SelectedItem is DB.TrackData)
                {
                    ATDI_ResultData = (DB.TrackData)dataGrid.SelectedItem;

                    ATDI_DataProcessingSpectrResult(ATDI_ResultData);
                }
                //}
                //catch (Exception exp)
                //{
                //    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "DBPanel", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                //}
            }
        }
        //private void ATDI_DataProcessingTrackResult(object sender)
        //{
        //    if (sender is DB.localatdi_result_item)
        //    {
        //        try
        //        {
        //            #region
        //            DB.localatdi_result_item ri = (DB.localatdi_result_item)sender;// SelectedCells[0].Item;
        //            decimal fc = 0;
        //            if (ri.freq_centr_perm != 0)
        //            { fc = ri.freq_centr_perm; }
        //            else { fc = ri.spec_data.FreqCentr; }

        //            decimal step = ri.spec_data.Trace[10].freq - ri.spec_data.Trace[9].freq;
        //            ATDI_SpectrData = new MeasurementResult()
        //            {
        //                //MarkerM1 = new MeasurementMarker() { Type = "M1", Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[0]].freq, Level = ri.spec_data.Trace[ri.bw_data.NdBResult[0]].level },
        //                //MarkerT1 = new MeasurementMarker() { Type = "T1", Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[1]].freq, Level = ri.spec_data.Trace[ri.bw_data.NdBResult[1]].level },
        //                //MarkerT2 = new MeasurementMarker() { Type = "T2", Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[2]].freq, Level = ri.spec_data.Trace[ri.bw_data.NdBResult[2]].level },
        //                STRENGTH = ri.meas_strength,
        //                FreqPermision = fc,
        //                //FreqMeasured = (ri.spec_data.Trace[ri.bw_data.NdBResult[1]].freq + ri.spec_data.Trace[ri.bw_data.NdBResult[2]].freq) / 2,
        //                //BWMeasured = ri.spec_data.Trace[ri.bw_data.NdBResult[2]].freq - ri.spec_data.Trace[ri.bw_data.NdBResult[1]].freq,
        //                //RelativeDeviation = Math.Round(((Math.Abs(((ri.spec_data.FreqStart * 2 + step * ri.bw_data.NdBResult[1] + step * ri.bw_data.NdBResult[2]) / 2) - (fc))) / (fc / 1000000)), 3),
        //                InformationBlocks = new ObservableCollection<DB.local3GPPSystemInformationBlock>(ri.station_sys_info.information_blocks),
        //                level_measurements_car = ri.level_results
        //            };
        //            if (ri.bw_data.NdBResult[0] > -1 && ri.bw_data.NdBResult[0] < ri.spec_data.Trace.Length)
        //                ATDI_SpectrData.MarkerM1 = new MeasurementMarker() { Type = "M1", Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[0]].freq, Level = ri.spec_data.Trace[ri.bw_data.NdBResult[0]].level };
        //            if (ri.bw_data.NdBResult[1] > -1 && ri.bw_data.NdBResult[1] < ri.spec_data.Trace.Length)
        //                ATDI_SpectrData.MarkerT1 = new MeasurementMarker() { Type = "T1", Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[1]].freq, Level = ri.spec_data.Trace[ri.bw_data.NdBResult[1]].level };
        //            if (ri.bw_data.NdBResult[2] > -1 && ri.bw_data.NdBResult[2] < ri.spec_data.Trace.Length)
        //                ATDI_SpectrData.MarkerT2 = new MeasurementMarker() { Type = "T2", Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[2]].freq, Level = ri.spec_data.Trace[ri.bw_data.NdBResult[2]].level };
        //            if (ri.bw_data.NdBResult[1] > -1 && ri.bw_data.NdBResult[1] < ri.spec_data.Trace.Length && ri.bw_data.NdBResult[2] > -1 && ri.bw_data.NdBResult[2] < ri.spec_data.Trace.Length)
        //            {
        //                ATDI_SpectrData.FreqMeasured = (ri.spec_data.Trace[ri.bw_data.NdBResult[1]].freq + ri.spec_data.Trace[ri.bw_data.NdBResult[2]].freq) / 2;
        //                ATDI_SpectrData.BWMeasured = ri.spec_data.Trace[ri.bw_data.NdBResult[2]].freq - ri.spec_data.Trace[ri.bw_data.NdBResult[1]].freq;
        //                ATDI_SpectrData.RelativeDeviation = Math.Round(((Math.Abs(((ri.spec_data.FreqStart * 2 + step * ri.bw_data.NdBResult[1] + step * ri.bw_data.NdBResult[2]) / 2) - (fc))) / (fc / 1000000)), 3);
        //            }
        //            #endregion
        //        }
        //        catch (Exception exp)
        //        {
        //            MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "DBPanel", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
        //        }
        //    }
        //}
        private void ATDI_DataProcessingSpectrResult(object sender)
        {
            if (sender is DB.localatdi_result_item)
            {
                try
                {
                    #region
                    DB.localatdi_result_item ri = (DB.localatdi_result_item)sender;// SelectedCells[0].Item;
                    decimal fc = 0;
                    if (ri.freq_centr_perm != 0)
                    { fc = ri.freq_centr_perm; }
                    else { fc = ri.spec_data.FreqCentr; }

                    decimal step = ri.spec_data.Trace[10].freq - ri.spec_data.Trace[9].freq;
                    ATDI_SpectrData = new MeasurementResult()
                    {
                        //MarkerM1 = new MeasurementMarker() { Type = "M1", Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[0]].freq, Level = ri.spec_data.Trace[ri.bw_data.NdBResult[0]].level },
                        //MarkerT1 = new MeasurementMarker() { Type = "T1", Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[1]].freq, Level = ri.spec_data.Trace[ri.bw_data.NdBResult[1]].level },
                        //MarkerT2 = new MeasurementMarker() { Type = "T2", Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[2]].freq, Level = ri.spec_data.Trace[ri.bw_data.NdBResult[2]].level },
                        STRENGTH = ri.meas_strength,
                        FreqPermision = fc,
                        //FreqMeasured = (ri.spec_data.Trace[ri.bw_data.NdBResult[1]].freq + ri.spec_data.Trace[ri.bw_data.NdBResult[2]].freq) / 2,
                        //BWMeasured = ri.spec_data.Trace[ri.bw_data.NdBResult[2]].freq - ri.spec_data.Trace[ri.bw_data.NdBResult[1]].freq,
                        //RelativeDeviation = Math.Round(((Math.Abs(((ri.spec_data.FreqStart * 2 + step * ri.bw_data.NdBResult[1] + step * ri.bw_data.NdBResult[2]) / 2) - (fc))) / (fc / 1000000)), 3),
                        InformationBlocks = new ObservableCollection<DB.local3GPPSystemInformationBlock>(ri.station_sys_info.information_blocks),
                        level_measurements_car = ri.level_results
                    };
                    if (ri.bw_data.NdBResult[0] > -1 && ri.bw_data.NdBResult[0] < ri.spec_data.Trace.Length)
                        ATDI_SpectrData.MarkerM1 = new MeasurementMarker() { Type = "M1", Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[0]].freq, Level = ri.spec_data.Trace[ri.bw_data.NdBResult[0]].level };
                    if (ri.bw_data.NdBResult[1] > -1 && ri.bw_data.NdBResult[1] < ri.spec_data.Trace.Length)
                        ATDI_SpectrData.MarkerT1 = new MeasurementMarker() { Type = "T1", Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[1]].freq, Level = ri.spec_data.Trace[ri.bw_data.NdBResult[1]].level };
                    if (ri.bw_data.NdBResult[2] > -1 && ri.bw_data.NdBResult[2] < ri.spec_data.Trace.Length)
                        ATDI_SpectrData.MarkerT2 = new MeasurementMarker() { Type = "T2", Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[2]].freq, Level = ri.spec_data.Trace[ri.bw_data.NdBResult[2]].level };
                    if (ri.bw_data.NdBResult[1] > -1 && ri.bw_data.NdBResult[1] < ri.spec_data.Trace.Length && ri.bw_data.NdBResult[2] > -1 && ri.bw_data.NdBResult[2] < ri.spec_data.Trace.Length)
                    {
                        ATDI_SpectrData.FreqMeasured = (ri.spec_data.Trace[ri.bw_data.NdBResult[1]].freq + ri.spec_data.Trace[ri.bw_data.NdBResult[2]].freq) / 2;
                        ATDI_SpectrData.BWMeasured = ri.spec_data.Trace[ri.bw_data.NdBResult[2]].freq - ri.spec_data.Trace[ri.bw_data.NdBResult[1]].freq;
                        ATDI_SpectrData.RelativeDeviation = Math.Round(((Math.Abs(((ri.spec_data.FreqStart * 2 + step * ri.bw_data.NdBResult[1] + step * ri.bw_data.NdBResult[2]) / 2) - (fc))) / (fc / 1000000)), 3);
                    }
                    #endregion
                }
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "DBPanel", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
            }
            else if (sender is DB.TrackData)
            {
                DB.TrackData ri = (DB.TrackData)sender;// SelectedCells[0].Item;
                ATDI_SpectrData = new MeasurementResult()
                {
                    //MarkerM1 = new MeasurementMarker() { Type = "M1", Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[0]].freq, Level = ri.spec_data.Trace[ri.bw_data.NdBResult[0]].level },
                    //MarkerT1 = new MeasurementMarker() { Type = "T1", Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[1]].freq, Level = ri.spec_data.Trace[ri.bw_data.NdBResult[1]].level },
                    //MarkerT2 = new MeasurementMarker() { Type = "T2", Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[2]].freq, Level = ri.spec_data.Trace[ri.bw_data.NdBResult[2]].level },
                    //FreqMeasured = (ri.spec_data.Trace[ri.bw_data.NdBResult[1]].freq + ri.spec_data.Trace[ri.bw_data.NdBResult[2]].freq) / 2,
                    //BWMeasured = ri.spec_data.Trace[ri.bw_data.NdBResult[2]].freq - ri.spec_data.Trace[ri.bw_data.NdBResult[1]].freq,
                    //RelativeDeviation = Math.Round(((Math.Abs(((ri.spec_data.FreqStart * 2 + step * ri.bw_data.NdBResult[1] + step * ri.bw_data.NdBResult[2]) / 2) - (fc))) / (fc / 1000000)), 3),
                    InformationBlocks = new ObservableCollection<DB.local3GPPSystemInformationBlock>(ri.information_blocks),
                    level_measurements_car = ri.level_results
                };
            }
        }
        private void ATDI_SpectrData_InformationBlock_Select_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            ATDI_SpectrData.SelectedInformationBlock = bt.Tag.ToString();
        }
        private void ATDI_SpectrImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ATDI_ShowSpectrum == true)
            {
                try
                {
                    ATDI_DrawSpectrResult(ATDI_ResultData);
                }
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "DBPanel", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
            }
        }

        private void ATDI_DrawSpectrResult(object sender)
        {
            if (sender is DB.localatdi_result_item)
            {
                try
                {
                    #region
                    Equipment.PrintScreen ps = new Equipment.PrintScreen() { };
                    DB.localatdi_result_item ri = (DB.localatdi_result_item)sender;// SelectedCells[0].Item;
                                                                                   //ShowTracePoints = ri.trace;
                    double maxlevel = -1000, minlevel = 1000;
                    Equipment.tracepoint[] points = ri.spec_data.Trace;
                    for (int y = 0; y < ri.spec_data.Trace.Length; y++)
                    {
                        //points[y] = new Equipment.tracepoint() { freq = ri.trace_freq_start + ri.trace_freq_step * y, level = ri.trace[y] };
                        if (ri.spec_data.Trace[y].level > maxlevel) maxlevel = ri.spec_data.Trace[y].level;
                        if (ri.spec_data.Trace[y].level < minlevel) minlevel = ri.spec_data.Trace[y].level;
                    }
                    maxlevel += 10; minlevel -= 10;
                    ps.FreqStart = ri.spec_data.FreqStart;
                    ps.FreqStop = ri.spec_data.FreqStop;

                    ps.Trace1 = ri.spec_data.Trace;
                    ps.Trace1State = true;

                    Equipment.LocalMeasurement lm = new Equipment.LocalMeasurement();
                    //ps.Trace2State = true;
                    //try
                    //{
                    //    long ttt = DateTime.Now.Ticks;
                    //    ps.Trace2 = lm.ChangeFreqGrid_v2(ref points, ri.spec_data.FreqStart, points[2].freq - points[1].freq, ri.spec_data.FreqStart, ri.spec_data.FreqSpan / 500, 501, -158);
                    //    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = ((double)(DateTime.Now.Ticks - ttt) / 10000).ToString() + "\r\n" +
                    //        ps.Trace1.Length.ToString() + "  " + ps.Trace2.Length.ToString();
                    //}
                    //catch { }

                    ps.InstrType = 1;
                    ps.ActualPoints = ri.spec_data.Trace.Length;
                    //if (ri.sources_trace_level_type == false)
                    ps.LevelUnit = "dBm";
                    //else if (ri.sources_trace_level_type == false)
                    //    ps.LevelUnit = "dBµV";
                    ps.RefLevel = (decimal)maxlevel;
                    ps.Range = (decimal)(maxlevel - minlevel);

                    ps.Markers = new ObservableCollection<Equipment.Marker>() {
                        new Equipment.Marker()
                        {
                            State = true,
                            StateNew = true,
                            Index = 1,
                            MarkerType = 3,
                            MarkerTypeNew = 3,
                            FunctionDataType = 2,
                            Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[0]].freq,//+ points[2].freq - points[1].freq * ri.trace_m1_index,
                            Level = ri.spec_data.Trace[ri.bw_data.NdBResult[0]].level//ri.trace[ri.trace_m1_index].level,
                        }
                        };
                    //Equipment.LocalMeasurement lm = new Equipment.LocalMeasurement();
                    //int[] ttt = lm.GetMeasNDB(points, ri.SourcesM1MarkerIndex, 30, (points[points.Length - 1].Freq - points[0].Freq) * 0.95m);
                    //if (ttt[0] > 0)
                    //{
                    //    ps.inMarkers.Add(
                    //    new Equipment.Marker()
                    //    {
                    //        State = true,
                    //        MarkerType = "M",
                    //        NameInLegend = "M2" + ri.SourcesTrace[ttt[0]].ToString(),
                    //        Freq = (double)(ri.SourcesFreqStart + ri.SourcesFreqStep * ttt[0]),
                    //        Level = (double)ri.SourcesTrace[ttt[0]],
                    //    });
                    //}
                    if (ri.bw_data.NdBResult[1] > -1 && ri.bw_data.NdBResult[1] < ri.spec_data.Trace.Length && ri.bw_data.NdBResult[2] > -1 && ri.bw_data.NdBResult[2] < ri.spec_data.Trace.Length)
                    {
                        ps.Markers[0].TMarkers[0].State = true;
                        ps.Markers[0].TMarkers[0].StateNew = true;
                        ps.Markers[0].TMarkers[0].Index = 1;
                        ps.Markers[0].TMarkers[0].MarkerType = 2;
                        ps.Markers[0].TMarkers[0].MarkerTypeNew = 2;
                        ps.Markers[0].TMarkers[0].FunctionDataType = 3;
                        ps.Markers[0].TMarkers[0].Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[1]].freq;// ri.trace_freq_start + ri.trace_freq_step * ri.trace_t1_index;
                        ps.Markers[0].TMarkers[0].Level = ri.spec_data.Trace[ri.bw_data.NdBResult[1]].level;// ri.trace[ri.trace_t1_index].level;
                        ps.Markers[0].TMarkers[1].State = true;
                        ps.Markers[0].TMarkers[1].StateNew = true;
                        ps.Markers[0].TMarkers[1].Index = 2;
                        ps.Markers[0].TMarkers[1].MarkerType = 2;
                        ps.Markers[0].TMarkers[1].MarkerTypeNew = 2;
                        ps.Markers[0].TMarkers[1].FunctionDataType = 3;
                        ps.Markers[0].TMarkers[1].Freq = ri.spec_data.Trace[ri.bw_data.NdBResult[2]].freq;
                        ps.Markers[0].TMarkers[1].Level = ri.spec_data.Trace[ri.bw_data.NdBResult[2]].level;

                    }


                    ps.Trace1State = true;
                    System.Drawing.Bitmap Img = ps.drawImageSpectr((int)ATDI_SpectrResultGrid.ActualWidth - 1, (int)ATDI_SpectrResultGrid.ActualHeight - 1);

                    using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
                    {
                        Img.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                        memory.Position = 0;
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        ATDI_SpectrImg = bitmapImage;
                    }
                    #endregion


                }
                catch (Exception exp)
                {
                    //MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "DBPanel", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
            }
        }
        #endregion

        private void TaskItemsDataGrid_RowDetailsVisibilityChanged(object sender, DataGridRowDetailsEventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            Grid gr = e.DetailsElement as Grid;
            //Expander innerExpander = gr.Children.OfType<Expander>().Where(x => x.Name == "res_exp").FirstOrDefault();

            Expander innerExpander = gr.Children.OfType<Expander>().Where(x => x.Name == "res_exp").FirstOrDefault();
            innerExpander.Visibility = Visibility.Collapsed;
            DataGrid innerDataGrid = (DataGrid)innerExpander.Content;
            ///DataGrid innerDataGrid = e.DetailsElement as DataGrid;

            DB.localatdi_station item = (DB.localatdi_station)dg.SelectedItem;
            string meastaskid = "";
            for (int i = 0; i < MainWindow.db_v2.AtdiTasks.Count; i++)
            {
                for (int j = 0; j < MainWindow.db_v2.AtdiTasks[i].data_from_tech.Count; j++)
                {
                    for (int k = 0; k < MainWindow.db_v2.AtdiTasks[i].data_from_tech[j].TaskItems.Count; k++)
                    {
                        if (MainWindow.db_v2.AtdiTasks[i].data_from_tech[j].TaskItems[k] == item)
                        { meastaskid = MainWindow.db_v2.AtdiTasks[i].task_id; }
                    }
                }
            }


            ObservableCollection<DB.localatdi_result_item> rirows = new ObservableCollection<DB.localatdi_result_item>() { };
            for (int i = 0; i < MainWindow.db_v2.AtdiTasks.Count; i++)
            {
                if (MainWindow.db_v2.AtdiTasks[i].task_id == meastaskid)
                    for (int j = 0; j < MainWindow.db_v2.AtdiTasks[i].data_from_tech.Count; j++)
                    {
                        if (item.standard.Contains(MainWindow.db_v2.AtdiTasks[i].data_from_tech[j].tech))
                        {
                            for (int k = 0; k < MainWindow.db_v2.AtdiTasks[i].data_from_tech[j].ResultItems.Count; k++)
                            {
                                DB.localatdi_result_item ri = MainWindow.db_v2.AtdiTasks[i].data_from_tech[j].ResultItems[k];
                                if (ri.id_station == item.id)
                                {
                                    for (int sec = 0; sec < item.sectors.Count; sec++)
                                    {
                                        if (ri.id_sector == item.sectors[sec].sector_id)
                                        {
                                            for (int l = 0; l < item.sectors[sec].frequencies.Length; l++)
                                            {
                                                if (ri.id_frequency == item.sectors[sec].frequencies[l].id)
                                                {
                                                    rirows.Add(ri);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
            }
            if (rirows.Count > 0)
            {
                innerExpander.Visibility = Visibility.Visible;
                innerDataGrid.ItemsSource = rirows;
            }
            //else { innerExpander.Visibility = Visibility.Collapsed; }
        }

        private void SaveTrackToCSV_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is DB.Track)
            {
                //id = ((DB.Track)((Button)sender).Tag).table_name;

                SaveTrackData sdt = new SaveTrackData((DB.Track)((Button)sender).Tag);
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.ShowGlobalUC(sdt, false, false);
            }
        }
    }
    public class MeasurementResult : INotifyPropertyChanged
    {
        public MeasurementMarker MarkerM1
        {
            get { return _MarkerM1; }
            set { _MarkerM1 = value; OnPropertyChanged("MarkerM1"); }
        }
        private MeasurementMarker _MarkerM1 = new MeasurementMarker();

        public MeasurementMarker MarkerT1
        {
            get { return _MarkerT1; }
            set { _MarkerT1 = value; OnPropertyChanged("MarkerT1"); }
        }
        private MeasurementMarker _MarkerT1 = new MeasurementMarker();

        public MeasurementMarker MarkerT2
        {
            get { return _MarkerT2; }
            set { _MarkerT2 = value; OnPropertyChanged("MarkerT2"); }
        }
        private MeasurementMarker _MarkerT2 = new MeasurementMarker();

        public decimal STRENGTH
        {
            get { return _STRENGTH; }
            set { _STRENGTH = value; OnPropertyChanged("STRENGTH"); }
        }
        private decimal _STRENGTH = -1000;

        public decimal FreqPermision
        {
            get { return _FreqPermision; }
            set { _FreqPermision = value; OnPropertyChanged("FreqPermision"); }
        }
        private decimal _FreqPermision = -1000;

        public decimal FreqMeasured
        {
            get { return _FreqMeasured; }
            set { _FreqMeasured = value; OnPropertyChanged("FreqMeasured"); }
        }
        private decimal _FreqMeasured = -1000;

        public decimal RelativeDeviation
        {
            get { return _RelativeDeviation; }
            set { _RelativeDeviation = value; OnPropertyChanged("RelativeDeviation"); }
        }
        private decimal _RelativeDeviation = -1000;

        public decimal BWPermision
        {
            get { return _BWPermision; }
            set { _BWPermision = value; OnPropertyChanged("BWPermision"); }
        }
        private decimal _BWPermision = -1000;

        public decimal BWMeasured
        {
            get { return _BWMeasured; }
            set { _BWMeasured = value; OnPropertyChanged("BWMeasured"); }
        }
        private decimal _BWMeasured = -1000;




        public ObservableCollection<DB.local3GPPSystemInformationBlock> InformationBlocks
        {
            get { return _InformationBlocks; }
            set { _InformationBlocks = value; OnPropertyChanged("InformationBlocks"); }
        }
        public ObservableCollection<DB.local3GPPSystemInformationBlock> _InformationBlocks = new ObservableCollection<DB.local3GPPSystemInformationBlock>() { };

        public string SelectedInformationBlock
        {
            get { return _SelectedInformationBlock; }
            set { _SelectedInformationBlock = value; OnPropertyChanged("SelectedInformationBlock"); }
        }
        private string _SelectedInformationBlock = "";

        public ObservableCollection<DB.localatdi_level_meas_result> level_measurements_car
        {
            get { return _level_measurements_car; }
            set { _level_measurements_car = value; OnPropertyChanged("level_measurements_car"); }
        }
        private ObservableCollection<DB.localatdi_level_meas_result> _level_measurements_car = new ObservableCollection<DB.localatdi_level_meas_result>() { };


        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class MeasurementMarker : INotifyPropertyChanged
    {
        public string Type
        {
            get { return _Type; }
            set { _Type = value; OnPropertyChanged("Type"); }
        }
        private string _Type = string.Empty;

        public decimal Freq
        {
            get { return _Freq; }
            set { _Freq = value; OnPropertyChanged("Freq"); }
        }
        private decimal _Freq = -1000;

        public double Level
        {
            get { return _Level; }
            set { _Level = value; OnPropertyChanged("Level"); }
        }
        private double _Level = -1000;

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ImageConverter : IValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new BitmapImage(new Uri(value.ToString()));
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    [ValueConversion(typeof(decimal[]), typeof(Geometry))]
    public class SpectrumToPathConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal[] points = (decimal[])value;
            if (points != null && points.Length > 0)
            {
                decimal start = points[0];
                List<LineSegment> segments = new List<LineSegment>();
                decimal min = points.Min(), max = points.Max();
                decimal dout0 = MainWindow.help.MAP(points[0], max, min, min, max);
                for (int i = 0; i < points.Length; i++)
                {
                    decimal dout = MainWindow.help.MAP(points[i], max, min, min, max);
                    segments.Add(new LineSegment(new Point(i, (double)dout), true));
                }
                PathFigure figure = new PathFigure(new Point(0, (double)dout0), segments, false); //true if closed
                PathGeometry geometry = new PathGeometry();
                geometry.Figures.Add(figure);
                return geometry;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
    [ValueConversion(typeof(decimal[]), typeof(Geometry))]
    public class SpectrumGridToPathConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal[] points = (decimal[])value;
            if (points != null && points.Length > 0)
            {
                decimal start = points[0];
                List<LineSegment> segments = new List<LineSegment>();
                decimal min = points.Min(), max = points.Max();
                decimal dout0 = MainWindow.help.MAP(points[0], max, min, min, max);
                for (int i = 0; i < 110; i += 10)
                {
                    segments.Add(new LineSegment(new Point(i, 0), false));
                    segments.Add(new LineSegment(new Point(i, 100), true));
                }
                segments.Add(new LineSegment(new Point(0, 0), false));
                segments.Add(new LineSegment(new Point(100, 0), true));

                segments.Add(new LineSegment(new Point(0, 100), false));
                segments.Add(new LineSegment(new Point(100, 100), true));
                decimal range = max - min;
                for (int i = 0; i < 11; i++)
                {
                    segments.Add(new LineSegment(new Point(0, 10 * i), false));
                    segments.Add(new LineSegment(new Point(100, 10 * i), true));
                }
                FormattedText text = new FormattedText("Text to display",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Tahoma"),
                    14,
                    Brushes.Black);
                Geometry textgeometry = text.BuildGeometry(new Point(5, 5));
                FormattedText text1 = new FormattedText("Text to display",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Tahoma"),
                    14,
                    Brushes.Black);
                Geometry textgeometry1 = text1.BuildGeometry(new Point(50, 5));

                //PathGeometry g1 = new PathGeometry();
                //g1.Figures.Add(new PathFigure(new Point(10, 10), PathGeometry.CreateFromGeometry(textgeometry), true));
                //PathGeometry pathGeometry = PathGeometry.CreateFromGeometry(textgeometry);
                PathFigure figure = new PathFigure(new Point(0, 0), segments, false); //true if closed

                PathGeometry geometry = PathGeometry.CreateFromGeometry(textgeometry);
                geometry = PathGeometry.CreateFromGeometry(textgeometry1);
                geometry.Figures.Add(figure);
                return geometry;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
    public class LevelPlacementItem : ContentControl
    {
        public LevelPlacementItem()
        {
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
        }
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            double X = arrangeBounds.Width * .1 * PozX;
            double Y = arrangeBounds.Height * .1 * PozY;
            RenderTransform = new TranslateTransform(X, Y);
            return base.ArrangeOverride(arrangeBounds);
        }

        public double PozX
        {
            get { return (double)GetValue(PozXProperty); }
            set { SetValue(PozXProperty, value); }
        }

        public static readonly DependencyProperty PozXProperty =
            DependencyProperty.Register("PozX", typeof(double), typeof(PolarPlacementItem), new PropertyMetadata(0d, OnPozXPropertyChanged));

        private static void OnPozXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PolarPlacementItem).InvalidateArrange();
        }

        public double PozY
        {
            get { return (double)GetValue(PozYProperty); }
            set { SetValue(PozYProperty, value); }
        }

        public static readonly DependencyProperty PozYProperty =
            DependencyProperty.Register("PozY", typeof(double), typeof(PolarPlacementItem), new PropertyMetadata(0d, OnPozYPropertyChanged));

        private static void OnPozYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PolarPlacementItem).InvalidateArrange();
        }

    }
}
