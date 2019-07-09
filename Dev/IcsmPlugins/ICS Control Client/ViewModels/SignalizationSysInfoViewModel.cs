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

namespace XICSM.ICSControlClient.ViewModels
{
    public class SignalizationSysInfoViewModel : WpfViewModelBase
    {
        long _measResultId;
        double _freq_MHz;
        private SignSysInfoViewModel _currentSysInfo;
        private SignSysInfoDataAdapter _sysInfos;
        private EmittingWorkTimeDataAdapter _sysInfoWorkTimes;
        public SignSysInfoDataAdapter SysInfos => this._sysInfos;
        public EmittingWorkTimeDataAdapter SysInfoWorkTimes => this._sysInfoWorkTimes;
        public SignalizationSysInfoViewModel(long measResultId, double freq_MHz)
        {
            this._measResultId = measResultId;
            this._sysInfos = new SignSysInfoDataAdapter();
            this._sysInfoWorkTimes = new EmittingWorkTimeDataAdapter();
            this._freq_MHz = freq_MHz;
            this.ReloadData();
        }
        public SignSysInfoViewModel CurrentSysInfo
        {
            get => this._currentSysInfo;
            set => this.Set(ref this._currentSysInfo, value, () => { ReloadWorkTime(); });
        }
        private void ReloadData()
        {
            var sysInfo = SVC.SdrnsControllerWcfClient.GetSignalingSysInfos(this._measResultId, this._freq_MHz);
            this._sysInfos.Source = sysInfo;
        }
        private void ReloadWorkTime()
        {
            this._sysInfoWorkTimes.Source = _currentSysInfo.WorkTimes;
        }
    }
}
