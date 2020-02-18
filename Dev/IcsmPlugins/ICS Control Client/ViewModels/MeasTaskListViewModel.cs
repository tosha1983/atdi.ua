using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Models;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;
using XICSM.ICSControlClient.Models.WcfDataApadters;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using CS = XICSM.ICSControlClient.WpfControls.Charts;
using MP = XICSM.ICSControlClient.WpfControls.Maps;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Windows;
using FRM = System.Windows.Forms;
using FM = XICSM.ICSControlClient.Forms;
using ICSM;
using System.Windows.Controls;
using INP = System.Windows.Input;
using System.Collections;

namespace XICSM.ICSControlClient.ViewModels
{
    public class MeasTaskListViewModel : WpfViewModelBase
    {
        public FM.MeasTaskListForm _form;
        private MeasTask[] _taskData;
        private MeasTaskShortDataAdapter _tasks;
        public MeasTaskShortDataAdapter Tasks => this._tasks;
        public WpfCommand ContinueCommand { get; set; }
        public WpfCommand CancelCommand { get; set; }
        public MeasTaskListViewModel(MeasTask[] taskData)
        {
            this._taskData = taskData;
            this._tasks = new MeasTaskShortDataAdapter();

            this.ContinueCommand = new WpfCommand(this.OnContinueCommand);
            this.CancelCommand = new WpfCommand(this.OnCancelCommand);
            this.ReloadData();
        }
        private void ReloadData()
        {
            this._tasks.Source = this._taskData;
        }
        private void OnContinueCommand(object parameter)
        {
            _form.IsPresOK = true;
            _form.Close();
        }
        private void OnCancelCommand(object parameter)
        {
            _form.Close();
        }
    }
}
