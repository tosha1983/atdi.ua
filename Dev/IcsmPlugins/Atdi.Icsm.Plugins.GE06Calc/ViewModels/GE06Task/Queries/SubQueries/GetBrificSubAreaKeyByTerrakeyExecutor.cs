using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using ICSM;
using Atdi.DataModels.Sdrn.DeepServices.GN06;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Queries.SubQueries
{
    public class GetBrificSubAreaKeyByTerrakeyExecutor : IReadQueryExecutor<GetBrificSubAreaKeyByTerrakey, int>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetBrificSubAreaKeyByTerrakeyExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public int Read(GetBrificSubAreaKeyByTerrakey criterion)
        {
            IMRecordset rs = new IMRecordset("ge06_allot_sub_area_xref", IMRecordset.Mode.ReadOnly);
            rs.Select("ge06_sub_area_key");
            rs.SetWhere("terrakey", IMRecordset.Operation.Eq, criterion.terrakey);
            int subAreaKey = 0;
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                subAreaKey = rs.GetI("ge06_sub_area_key");
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            return subAreaKey;
        }
    }
}
