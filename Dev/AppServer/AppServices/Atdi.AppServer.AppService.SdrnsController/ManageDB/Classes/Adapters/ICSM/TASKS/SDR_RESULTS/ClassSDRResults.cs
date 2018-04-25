using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using OrmCs;
using Atdi.SDNRS.AppServer.BusManager;
using CoreICSM.Logs;

namespace Atdi.SDNRS.AppServer.ManageDB.Adapters
{
    /// <summary>
    /// 
    /// </summary>
    public class ClassSDRResults: IDisposable
    {
        public YXbsMeasurementres meas_res { get; set; }
        //public YXbsStationmeas stat_meas { get; set; }
        public List<YXbsLocationsensorm> loc_sensorM { get; set; }
        public List<YXbsFrequencymeas> freq_meas { get; set; }

        public List<YXbsLevelmeasres> level_meas_res { get; set; }
        public List<YXbsLevelmeasonlres> level_meas_onl_res { get; set; }
        public List<YXbsSpectoccupmeas> spect_occup_meas { get; set; }


        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ClassSDRResults()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public ClassSDRResults()
        {
            meas_res = new YXbsMeasurementres();
            //stat_meas = new YXbsStationmeas();
            loc_sensorM = new List<YXbsLocationsensorm>();
            freq_meas = new List<YXbsFrequencymeas>();
            level_meas_res = new List<YXbsLevelmeasres>();
            spect_occup_meas = new List<YXbsSpectoccupmeas>();
            level_meas_onl_res = new List<YXbsLevelmeasonlres>();
        }
    }
}
