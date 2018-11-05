using System;
using System.Collections.Generic;
using Atdi.Oracle.DataAccess;


namespace Atdi.SDNRS.AppServer.ManageDB.Adapters
{
    /// <summary>
    /// 
    /// </summary>
    public class ClassSDRResults: IDisposable
    {
        public YXbsResMeas meas_res { get; set; }
        //public YXbsStationmeas stat_meas { get; set; }
        public List<YXbsResLocSensorMeas> loc_sensorM { get; set; }
        public List<YXbsResLevels> resLevels { get; set; }
        public List<YXbsResLevmeasonline> level_meas_onl_res { get; set; }
        public List<YXbsResmeasstation> XbsResmeasstation { get; set; }
        public List<YXbsLinkResSensor> XbsLinkResSensor { get; set; }
        public string SensorName { get; set; }
        public List<YXbsResStGeneral> XbsResGeneral { get; set; }
        public List<YXbsResStLevelCar> XbsResLevelMeas { get; set; }
        public List<YXbsResStMaskElm> XbsResmaskBw { get; set; }
        public List<YXbsResStLevelsSpect> XbsLevelSpecrum { get; set; }

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
            meas_res = new YXbsResMeas();
            //stat_meas = new YXbsStationmeas();
            loc_sensorM = new List<YXbsResLocSensorMeas>();
            resLevels = new List<YXbsResLevels>();
            level_meas_onl_res = new List<YXbsResLevmeasonline>();
            XbsResmeasstation = new List<YXbsResmeasstation>();
            XbsResGeneral = new List<YXbsResStGeneral>();
            XbsResLevelMeas = new List<YXbsResStLevelCar>();
            XbsResmaskBw = new List<YXbsResStMaskElm>();
            XbsLevelSpecrum = new List<YXbsResStLevelsSpect>();
            XbsLinkResSensor = new List<YXbsLinkResSensor>();
            SensorName = "";
    }

       
    }
}
