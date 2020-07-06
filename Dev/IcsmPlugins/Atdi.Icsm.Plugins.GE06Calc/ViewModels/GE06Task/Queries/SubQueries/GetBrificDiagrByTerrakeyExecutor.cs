using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using ICSM;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Queries
{
    public class GetBrificDiagrByTerrakeyExecutor : IReadQueryExecutor<GetBrificDiagrByTerrakey, List<float>>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetBrificDiagrByTerrakeyExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public List<float> Read(GetBrificDiagrByTerrakey criterion)
        {
            var values = new List<float>();
            IMRecordset rs = new IMRecordset("fmtv_ant_diag", IMRecordset.Mode.ReadOnly);
            rs.Select("attn");
            rs.SetWhere("terrakey", IMRecordset.Operation.Eq, criterion.terrakey);
            rs.SetWhere("polar", IMRecordset.Operation.Eq, criterion.polar);

            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                values.Add((float)rs.GetD("attn"));
            }

            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            return values;
        }
    }
}
