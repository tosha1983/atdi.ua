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
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
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
        private IList _currentProtocols;
        private GroupeEmissionProtocolDataFilter _dataFilter;
        private bool _isEnabledPrintAllCommand = false;

        private DataSynchronizationProcessProtocolDataAdapter _protocols;

        public WpfCommand FilterApplyCommand { get; set; }
        public WpfCommand PrintSelectedCommand { get; set; }
        public WpfCommand PrintAllCommand { get; set; }

        public DataSynchronizationProcessProtocolDataAdapter Protocols => this._protocols;

        public GroupeEmissionWithStationProtocolModelViewModel()
        {
            this._protocols = new DataSynchronizationProcessProtocolDataAdapter();
            this._dataFilter = new GroupeEmissionProtocolDataFilter();
            this.FilterApplyCommand = new WpfCommand(this.OnFilterApplyCommand);
            this.PrintSelectedCommand = new WpfCommand(this.OnPrintSelectedCommand);
            this.PrintAllCommand = new WpfCommand(this.OnPrintAllCommand);
            IsEnabledPrintAllCommand = false;
            this.ReloadData();
        }
        public IList CurrentProtocols
        {
            get => this._currentProtocols;
            set
            {   
                this._currentProtocols = value;
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
            var sdrProtocols = SVC.SdrnsControllerWcfClient.GetProtocols();
            this._protocols.Source = sdrProtocols;
        }
        private void CheckEnablePrintCommand()
        {
            if (this._dataFilter.Freq_MHz.HasValue || (this._dataFilter.DateMeasYear.HasValue && this._dataFilter.DateMeasMonth.HasValue))
                IsEnabledPrintAllCommand = true;
            else
                IsEnabledPrintAllCommand = false;
        }
        private void OnFilterApplyCommand(object parameter)
        {
            var sdrProtocols = SVC.SdrnsControllerWcfClient.GetProtocolsByParameters(null,
                this._dataFilter.CreatedBy,
                this._dataFilter.DateCreated,
                null,
                null,
                this._dataFilter.DateMeasDay,
                this._dataFilter.DateMeasMonth,
                this._dataFilter.DateMeasYear,
                this._dataFilter.Freq_MHz,
                this._dataFilter.Probability,
                this._dataFilter.Standard,
                this._dataFilter.Province,
                this._dataFilter.Owner,
                this._dataFilter.PermissionNumber,
                this._dataFilter.PermissionStart,
                this._dataFilter.PermissionStop);
            this._protocols.Source = sdrProtocols;
        }
        private void OnPrintAllCommand(object parameter)
        {
            if ((_dataFilter.DateMeasYear.HasValue && _dataFilter.DateMeasMonth.HasValue) || _dataFilter.Freq_MHz.HasValue)
            {
                if (this._protocols.Source != null && this._protocols.Source.Length > 0)
                {
                    FRM.FolderBrowserDialog folderDialog = new FRM.FolderBrowserDialog();
                    if (folderDialog.ShowDialog() == FRM.DialogResult.OK)
                    {
                        if (!string.IsNullOrEmpty(folderDialog.SelectedPath))
                        {
                            foreach (var row in this._protocols.Source)
                            {
                                PrintRow(Mappers.Map(row), folderDialog.SelectedPath);
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
            FRM.FolderBrowserDialog folderDialog = new FRM.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == FRM.DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(folderDialog.SelectedPath))
                {
                    foreach (DataSynchronizationProcessProtocolsViewModel row in this._currentProtocols)
                    {
                        PrintRow(row, folderDialog.SelectedPath);
                    }
                    MessageBox.Show("Процедура формирования отчетов успешно завершена!");
                }
            }
        }

        private void PrintRow(DataSynchronizationProcessProtocolsViewModel row, string selectedPath)
        {
            var buildSpectrogram = new BuildSpectrogram();
            // заполненеие таблицы XPROTOCOL_REPORT
            IMRecordset rs = new IMRecordset("XPROTOCOL_REPORT", IMRecordset.Mode.ReadWrite);
            rs.Select("ID,DATE_CREATED,STANDARD_NAME,OWNER_NAME,PERMISSION_NUMBER,PERMISSION_START,PERMISSION_STOP,ADDRESS,LONGITUDE,LATITUDE,SENSOR_LON,SENSOR_LAT,SENSOR_NAME,DATE_MEAS,S_FREQ_MHZ,S_BW,FREQ_MHZ,BW,LEVEL_DBM,DESIG_EMISSION,GLOBAL_SID,CREATED_BY");
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
            rs.Put("ADDRESS", row.Address);
            if (row.Location != null)
            {
                rs.Put("LONGITUDE", ConvertCoordinates.DecToDmsToString(row.Location.Longitude, Coordinates.EnumCoordLine.Lon));
                rs.Put("LATITUDE", ConvertCoordinates.DecToDmsToString(row.Location.Latitude, Coordinates.EnumCoordLine.Lat));
            }
            if ((row.Locations != null) && (row.Locations.Length>0))
            {
                var lstSensorLocation = row.Locations.ToList();
                var orderSensorLocation = lstSensorLocation.OrderByDescending(c => c.DataCreated);
                lstSensorLocation = orderSensorLocation.ToList();
                var actualSensorLocation = lstSensorLocation[0];
                if (actualSensorLocation.Lon != null)
                {
                    rs.Put("SENSOR_LON", ConvertCoordinates.DecToDmsToString(actualSensorLocation.Lon.Value, Coordinates.EnumCoordLine.Lon));
                }
                if (actualSensorLocation.Lat != null)
                {
                    rs.Put("SENSOR_LAT", ConvertCoordinates.DecToDmsToString(actualSensorLocation.Lat.Value, Coordinates.EnumCoordLine.Lat));
                }
            }
            rs.Put("SENSOR_NAME", row.SensorName);
            rs.Put("DATE_MEAS", row.DateMeas);
            rs.Put("S_FREQ_MHZ", Math.Round(row.Freq_MHz,3));
            if (row.BandWidth != null)
            {
                rs.Put("S_BW", Math.Round(row.BandWidth.Value, 3));
            }
            if (row.RadioControlMeasFreq_MHz != null)
            {
                rs.Put("FREQ_MHZ", Math.Round(row.RadioControlMeasFreq_MHz.Value, 3));
            }
            if (row.RadioControlBandWidth != null)
            {
                rs.Put("BW", Math.Round(row.RadioControlBandWidth.Value, 3));
            }
            if (row.ProtocolsLinkedWithEmittings != null)
            {
                if (row.ProtocolsLinkedWithEmittings.CurentPower_dBm != null)
                {
                    rs.Put("LEVEL_DBM", Math.Round(row.ProtocolsLinkedWithEmittings.CurentPower_dBm.Value, 1));
                }
            }
            rs.Put("DESIG_EMISSION", row.DesigEmission);
            rs.Put("GLOBAL_SID", row.GlobalSID);
            rs.Put("CREATED_BY", GetUserFio(IM.ConnectedUser()));
            rs.Update();

            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            //генерация отчета
            var nameFile = selectedPath + $@"\{row.GlobalSID}_{row.StandardName}_{row.Freq_MHz.ToString().Replace(".","_").Replace(",","_")}_{row.OwnerName}_{row.Level_dBm.ToString().Replace(".", "_").Replace(",", "_")}_{row.Id.ToString()}.rtf";
            RecordPtr recPtr;
            recPtr.Table = "XPROTOCOL_REPORT";
            recPtr.Id = id;
            recPtr.PrintRTFReport2(InsertSpectrogram.GetDirTemplates("SHDIR-REP") + @"\REPORT_SIGNALING.IRP", "RUS", nameFile, "", true, false);
            var bm = new System.Drawing.Bitmap(1200, 600);
            buildSpectrogram.CreateBitmapSpectrogram(row, bm, 1200, 600);
            InsertSpectrogram.InsertImageToRtf(nameFile, bm, 17000, 8000);
            bm.Dispose();
            GC.Collect();

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
    }
}
