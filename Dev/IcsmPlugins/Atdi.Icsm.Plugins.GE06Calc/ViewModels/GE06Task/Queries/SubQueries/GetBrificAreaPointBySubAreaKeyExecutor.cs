using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using ICSM;
using Atdi.DataModels.Sdrn.DeepServices.GN06;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Queries
{
    public class GetAreaPointBySubAreaKeyExecutor : IReadQueryExecutor<GetAreaPointBySubAreaKey, AreaPoint[]>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetAreaPointBySubAreaKeyExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public AreaPoint[] Read(GetAreaPointBySubAreaKey criterion)
        {
            var values = new List<AreaPoint>();
            IMRecordset rs = new IMRecordset("ge06_sub_area_pt", IMRecordset.Mode.ReadOnly);
            rs.Select("long_dec,lat_dec");
            rs.SetWhere("ge06_sub_area_key", IMRecordset.Operation.Eq, criterion.SubAreaKey);

            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                var areaPoint = new AreaPoint();
                values.Add(new AreaPoint() { Lat_DEC = rs.GetD("lat_dec"), Lon_DEC = rs.GetD("long_dec") });
            }

            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            return values.ToArray();
        }
    }
}
