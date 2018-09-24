using System.Collections.Generic;

namespace Atdi.AppServer.AppService.SdrnsControllerv2_0
{
    /// <summary>
    /// 
    /// </summary>
    public class ClassSDRResults
    {
        public YXbsResMeas meas_res { get; set; }
        //public YXbsStationmeas stat_meas { get; set; }
        public List<YXbsResLocSensorMeas> loc_sensorM { get; set; }
        public List<YXbsResLevmeasonline> level_meas_onl_res { get; set; }
        public List<YXbsResLevels> ResLevels { get; set; }

        
        public ClassSDRResults()
        {
            meas_res = new YXbsResMeas();
            //stat_meas = new YXbsStationmeas();
            loc_sensorM = new List<YXbsResLocSensorMeas>();
            ResLevels = new  List<YXbsResLevels>();
            level_meas_onl_res = new List<YXbsResLevmeasonline>();
        }
    }
}
