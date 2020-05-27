using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager;
using System.Globalization;
using OrmCs;
using ICSM;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Adapters
{
    public class MobStationsDataAdapter : DataAdapter<YMobStationT, IcsmMobStation, MobStationsDataAdapter>
    {
        public string Standard;
        public string StatusForActiveStation;
        public string StatusForNotActiveStation;
        public int IdentifierStation;
        public string TableName;
        public void Refresh()
        {

            var icsmMobStation = new IcsmMobStation();
            //icsmMobStation.CallSign
            //icsmMobStation.CreatedDate
            //icsmMobStation.ExternalCode
            //icsmMobStation.ExternalSource


            IMRecordset rs = new IMRecordset(TableName, IMRecordset.Mode.ReadOnly);
            rs.Select("Position.NAME,Position.REMARK,Position.LATITUDE,Position.LONGITUDE,BW,Owner.NAME,STANDARD,RadioSystem.DESCRIPTION,Position.PROVINCE,DESIG_EMISSION,NAME,Owner.CODE,STATUS");
            rs.SetWhere("ID", IMRecordset.Operation.Eq, IdentifierStation);
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                
               
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            //this.Source = areas.ToArray();
        }


        protected override Func<YMobStationT, IcsmMobStation> GetMapper()
        {
            return source => new IcsmMobStation
            {
                
            };
        }
        
    }
}
