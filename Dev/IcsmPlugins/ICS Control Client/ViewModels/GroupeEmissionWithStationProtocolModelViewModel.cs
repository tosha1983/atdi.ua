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
                    foreach (var row in this._protocols.Source)
                        PrintRow(Mappers.Map(row));

            }
            else
                MessageBox.Show("You must set the filter by date or frequency");
        }
        private void OnPrintSelectedCommand(object parameter)
        {
            foreach (DataSynchronizationProcessProtocolsViewModel row in this._currentProtocols)
            {
                PrintRow(row);
            }
        }
        private void PrintRow(DataSynchronizationProcessProtocolsViewModel row)
        {

        }
    }
}
