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
    public class GetBrificEfhgtByTerrakeyExecutor : IReadQueryExecutor<GetBrificEfhgtByTerrakey, List<short>>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetBrificEfhgtByTerrakeyExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public List<short> Read(GetBrificEfhgtByTerrakey criterion)
        {
            var values = new List<short>();
            IMRecordset rs = new IMRecordset("fmtv_ant_hgt", IMRecordset.Mode.ReadOnly);
            rs.Select("eff_hgt");
            rs.SetWhere("terrakey", IMRecordset.Operation.Eq, criterion.terrakey);

            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                values.Add((short)rs.GetI("eff_hgt"));
            }

            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            return values;
        }
    }
}
