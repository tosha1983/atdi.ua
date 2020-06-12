using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;
using XICSM.ICSControlClient.Models.WcfDataApadters;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using MP = XICSM.ICSControlClient.WpfControls.Maps;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;
using System.Windows;
using FRM = System.Windows.Forms;
using FM = XICSM.ICSControlClient.Forms;
using ICSM;
using System.Windows.Controls;
using System.Collections;
using XICSM.ICSControlClient.Models;
using System.Windows.Input;
using System.Configuration;
using System.Globalization;
using TR = System.Threading;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using Atdi.Common;
using Atdi.Contracts.WcfServices.Sdrn.Server;
using XICSM.ICSControlClient.ViewModels.Reports;

namespace XICSM.ICSControlClient.ViewModels
{
    public class GroupeEmissionWithStationProtocolModelViewModel : WpfViewModelBase
    {
        private DataSynchronizationProcessViewModel _currentProtocol;
        private IList _currentProtocolDetails;
        private GroupeEmissionProtocolDataFilter _dataFilter;
        private bool _isEnabledPrintAllCommand = false;

        private DataSynchronizationProcessDataAdapter _protocols;
        private DataSynchronizationProcessProtocolDataAdapter _protocolDetails;

        public WpfCommand FilterApplyCommand { get; set; }
        public WpfCommand PrintSelectedCommand { get; set; }
        public WpfCommand PrintAllCommand { get; set; }
        public WpfCommand FilterTRBSCommand { get; set; }

        public DataSynchronizationProcessDataAdapter Protocols => this._protocols;
        public DataSynchronizationProcessProtocolDataAdapter ProtocolDetails => this._protocolDetails;

        public GroupeEmissionWithStationProtocolModelViewModel()
        {
            this._protocols = new DataSynchronizationProcessDataAdapter();
            this._protocolDetails = new DataSynchronizationProcessProtocolDataAdapter();
            this._dataFilter = new GroupeEmissionProtocolDataFilter();
            this.FilterApplyCommand = new WpfCommand(this.OnFilterApplyCommand);
            this.FilterTRBSCommand = new WpfCommand(this.OnFilterTRBSCommand);
            this.PrintSelectedCommand = new WpfCommand(this.OnPrintSelectedCommand);
            this.PrintAllCommand = new WpfCommand(this.OnPrintAllCommand);
            IsEnabledPrintAllCommand = false;
            this.ReloadData();
        }
        public DataSynchronizationProcessViewModel CurrentProtocol
        {
            get => this._currentProtocol;
            set => this.Set(ref this._currentProtocol, value, () => { this.ReloadDataDetail(); });
        }
        public IList CurrentProtocolDetails
        {
            get => this._currentProtocolDetails;
            set
            {
                this._currentProtocolDetails = value;
                CheckEnablePrintCommand();
            }
        }
        public GroupeEmissionProtocolDataFilter DataFilter
        {
            get => this._dataFilter;
            set => this.Set(ref this._dataFilter, value);
        }
        public bool IsEnabledPrintAllCommand
        {
            get => this._isEnabledPrintAllCommand;
            set => this.Set(ref this._isEnabledPrintAllCommand, value);
        }
        private void ReloadData()
        {
            var sdrProtocols = SVC.SdrnsControllerWcfClientIeStation.GetProtocols();
            this._protocols.Source = sdrProtocols;
        }
        private void ReloadDataDetail()
        {
            if (this._currentProtocol != null)
            this._protocolDetails.Source = this._currentProtocol.DetailProtocols;
        }
        private void CheckEnablePrintCommand()
        {
            if (PluginHelper.ConvertStringToDouble(this._dataFilter.FreqStart).HasValue || PluginHelper.ConvertStringToDouble(this._dataFilter.FreqStop).HasValue || (ConvertToShort(this._dataFilter.DateMeasYear).HasValue && ConvertToShort(this._dataFilter.DateMeasMonth).HasValue))
                IsEnabledPrintAllCommand = true;
            else
                IsEnabledPrintAllCommand = false;
        }

        private void OnFilterTRBSCommand(object parameter)
        {

            var _selectPeriodForm = new FM.SelectPeriodForm();
            _selectPeriodForm.ShowDialog();
            if (_selectPeriodForm.IsPresOK)
            {
                FRM.FolderBrowserDialog folderDialog = new FRM.FolderBrowserDialog();
                if (folderDialog.ShowDialog() == FRM.DialogResult.OK)
                {
                    if (!string.IsNullOrEmpty(folderDialog.SelectedPath))
                    {
                        var selectYear = _selectPeriodForm.CurrentYear;
                        var selectMonth = _selectPeriodForm.CurrentMonth;

                        var sdrProtocols = SVC.SdrnsControllerWcfClientIeStation.GetDetailProtocolsByParameters(new DateTime(selectYear, selectMonth, 1, 0, 0, 0, 1), new DateTime(selectYear, selectMonth, System.DateTime.DaysInMonth(selectYear, selectMonth), 23, 59, 59, 1));

                        var prepareDataForReport = Report.PrepareData(sdrProtocols.ToList());
                        if ((prepareDataForReport != null) && (prepareDataForReport.Length > 0))
                        {
                            Report.Save(folderDialog.SelectedPath, selectYear.ToString(), _selectPeriodForm.CurrentMonthName, prepareDataForReport);
                            MessageBox.Show("Звіт сформовано!");
                        }
                    }
                }
            }
        }
        private void OnFilterApplyCommand(object parameter)
        {
            var sdrProtocols = SVC.SdrnsControllerWcfClientIeStation.GetProtocolsByParameters(null,
                this._dataFilter.CreatedBy,
                this._dataFilter.DateCreated,
                null,
                null,
                ConvertToShort(this._dataFilter.DateMeasDay),
                ConvertToShort(this._dataFilter.DateMeasMonth),
                ConvertToShort(this._dataFilter.DateMeasYear),
                PluginHelper.ConvertStringToDouble(this._dataFilter.FreqStart),
                PluginHelper.ConvertStringToDouble(this._dataFilter.FreqStop),
                this._dataFilter.Probability,
                this._dataFilter.Standard,
                this._dataFilter.Province,
                this._dataFilter.Owner,
                this._dataFilter.PermissionNumber,
                this._dataFilter.PermissionStart,
                this._dataFilter.PermissionStop,
                "");
            this._protocols.Source = sdrProtocols;
        }
        private void OnPrintAllCommand(object parameter)
        {
            if ((ConvertToShort(_dataFilter.DateMeasYear).HasValue && ConvertToShort(_dataFilter.DateMeasMonth).HasValue) || PluginHelper.ConvertStringToDouble(_dataFilter.FreqStart).HasValue || PluginHelper.ConvertStringToDouble(_dataFilter.FreqStop).HasValue)
            {
                if (this._protocols.Source != null && this._protocols.Source.Length > 0)
                {
                    FRM.FolderBrowserDialog folderDialog = new FRM.FolderBrowserDialog();
                    if (folderDialog.ShowDialog() == FRM.DialogResult.OK)
                    {
                        if (!string.IsNullOrEmpty(folderDialog.SelectedPath))
                        {
                            foreach (DataSynchronizationProcessProtocolsViewModel row in this._currentProtocolDetails)
                            {
                                PrintRow(row, folderDialog.SelectedPath, true);
                            }
                            MessageBox.Show("Процедура формирования отчетов успешно завершена!");
                        }
                    }
                }

            }
            else
                MessageBox.Show("You must set the filter by date or frequency");
        }
        private void OnPrintSelectedCommand(object parameter)
        {
            foreach (DataSynchronizationProcessProtocolsViewModel row in this._currentProtocolDetails)
            {
                PrintRow(row, System.IO.Path.GetTempFileName()+".rtf", false);
            }
            MessageBox.Show("Процедура формирования отчетов успешно завершена!");
        }

        private void PrintRow(DataSynchronizationProcessProtocolsViewModel row, string selectedPath, bool isPacketProcess)
        {
            var buildSpectrogram = new BuildSpectrogram();
            // заполненеие таблицы XPROTOCOL_REPORT
            IMRecordset rs = new IMRecordset("XPROTOCOL_REPORT", IMRecordset.Mode.ReadWrite);
            rs.Select("ID,DATE_CREATED,STANDARD_NAME,OWNER_NAME,PERMISSION_NUMBER,PERMISSION_START,PERMISSION_STOP,ADDRESS,LONGITUDE,LATITUDE,SENSOR_LON,SENSOR_LAT,SENSOR_NAME,DATE_MEAS,S_FREQ_MHZ,S_BW,FREQ_MHZ,BW,LEVEL_DBM,DESIG_EMISSION,GLOBAL_SID,CREATED_BY,VISN");
            rs.Open();
            var id = IM.AllocID("XPROTOCOL_REPORT", 1, -1);
            rs.AddNew();
            rs.Put("ID", id);
            rs.Put("DATE_CREATED", row.DateCreated);
            rs.Put("STANDARD_NAME", row.StandardName);
            rs.Put("OWNER_NAME", row.OwnerName);
            rs.Put("PERMISSION_NUMBER", row.PermissionNumber);
            rs.Put("PERMISSION_START", row.PermissionStart);
            rs.Put("PERMISSION_STOP", row.PermissionStop);
            if (row.StatusMeas == "A")
            {
                rs.Put("VISN", "Параметри РЕЗ відповідають умовам дозволу на експлуатацію");
            }
            else
            {
                rs.Put("VISN", "Робота РЕЗ не зафіксована");
            }
            rs.Put("ADDRESS", row.Address);
            if (row.Longitude.HasValue)
            {
                rs.Put("LONGITUDE", ConvertCoordinates.DecToDmsToString(row.Longitude.Value, Coordinates.EnumCoordLine.Lon));
                rs.Put("SENSOR_LON", ConvertCoordinates.DecToDmsToString(row.Longitude.Value, Coordinates.EnumCoordLine.Lon));
            }
            if (row.Latitude.HasValue)
            {
                rs.Put("LATITUDE", ConvertCoordinates.DecToDmsToString(row.Latitude.Value, Coordinates.EnumCoordLine.Lat));
                rs.Put("SENSOR_LAT", ConvertCoordinates.DecToDmsToString(row.Latitude.Value, Coordinates.EnumCoordLine.Lat));
            }

            rs.Put("SENSOR_NAME", row.TitleSensor);
            rs.Put("DATE_MEAS", row.DateMeas);
            if (row.Freq_MHz.HasValue)
                rs.Put("S_FREQ_MHZ", Math.Round(row.Freq_MHz.Value, 3));
            if (row.BandWidth.HasValue)
                rs.Put("S_BW", Math.Round(row.BandWidth.Value, 3));
            if (row.RadioControlMeasFreq_MHz.HasValue)
                rs.Put("FREQ_MHZ", Math.Round(row.RadioControlMeasFreq_MHz.Value, 3));
            if (row.RadioControlBandWidth_KHz.HasValue)
                rs.Put("BW", Math.Round(row.RadioControlBandWidth_KHz.Value, 3));
            if (row.ProtocolsLinkedWithEmittings != null)
            {
                if (row.ProtocolsLinkedWithEmittings.CurentPower_dBm != null)
                {
                    rs.Put("LEVEL_DBM", Math.Round(row.ProtocolsLinkedWithEmittings.CurentPower_dBm.Value, 1));
                }
            }
            //rs.Put("DESIG_EMISSION", row.DesigEmission);
            rs.Put("GLOBAL_SID", row.GlobalSID);
            rs.Put("CREATED_BY", GetUserFio(IM.ConnectedUser()));
            rs.Update();

            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            //генерация отчета
            var nameFile = selectedPath + $@"\{row.GlobalSID}_{row.StandardName}_{row.Freq_MHz.ToString().Replace(".", "_").Replace(",", "_")}_{row.OwnerName}_{row.Level_dBm.ToString().Replace(".", "_").Replace(",", "_")}_{row.Id.ToString()}.rtf";
            if (isPacketProcess==false)
            {
                nameFile = selectedPath;
            }
            if (System.IO.File.Exists(nameFile))
            {
                System.IO.File.Delete(nameFile);
            }

            RecordPtr recPtr;
            recPtr.Table = "XPROTOCOL_REPORT";
            recPtr.Id = id;
            if ((row.ProtocolsLinkedWithEmittings != null) && (row.ProtocolsLinkedWithEmittings.Levels_dBm != null) && (row.ProtocolsLinkedWithEmittings.SpectrumStartFreq_MHz != null) && (row.ProtocolsLinkedWithEmittings.SpectrumSteps_kHz != null))
            {
                recPtr.PrintRTFReport2(InsertSpectrogram.GetDirTemplates("SHDIR-REP") + @"\REPORT_SIGNALING_SPECTR.IRP", "RUS", nameFile, "", true, false);
                var bm = new System.Drawing.Bitmap(1400, 800);
                buildSpectrogram.CreateBitmapSpectrogram(row, bm, 1300, 700);
                //bm.Save("C:\\Temp\\Res.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                InsertSpectrogram.InsertImageToRtf(nameFile, bm, 16000, 6800);
                bm.Dispose();
                GC.Collect();
            }
            else
            {
                recPtr.PrintRTFReport2(InsertSpectrogram.GetDirTemplates("SHDIR-REP") + @"\REPORT_SIGNALING.IRP", "RUS", nameFile, "", true, false);
            }

            if (isPacketProcess == false)
            {
                System.Diagnostics.Process.Start(nameFile);
            }


            //очистка
            var rsDel = new IMRecordset("XPROTOCOL_REPORT", IMRecordset.Mode.ReadWrite);
            rsDel.Select("ID");
            rsDel.SetWhere("ID", IMRecordset.Operation.Eq, id);
            for (rsDel.Open(); !rsDel.IsEOF(); rsDel.MoveNext())
            {
                rsDel.Delete();
                break;
            }
            if (rsDel.IsOpen())
                rsDel.Close();
            rsDel.Destroy();
        }

        public static string GetUserFio(string login)
        {
            string retVal = "";
            IMRecordset rs = new IMRecordset("EMPLOYEE", IMRecordset.Mode.ReadWrite);
            {
                rs.Select("ID,LASTNAME,FIRSTNAME");
                rs.SetWhere("APP_USER", IMRecordset.Operation.Eq, login);
                rs.Open();
                if (!rs.IsEOF())
                {
                    retVal = string.Format("{0} {1}", rs.GetS("LASTNAME"), rs.GetS("FIRSTNAME"));
                }

                if (rs.IsOpen())
                    rs.Close();
                rs.Destroy();
            }
            return retVal;
        }
        private short? ConvertToShort(string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;

            short result = 0;

            if (short.TryParse(s, out result))
                return result;
            else
                return null;
        }
    }
}
