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
    public class GetBrificCounturIdByTerrakeyExecutor : IReadQueryExecutor<GetBrificCounturIdByTerrakey, int>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetBrificCounturIdByTerrakeyExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public int Read(GetBrificCounturIdByTerrakey criterion)
        {
            int countourId = 0;
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

            IMRecordset rsadm = new IMRecordset("ge06_sub_area_adm", IMRecordset.Mode.ReadOnly);
            rsadm.Select("contour_id");
            rsadm.SetWhere("ge06_sub_area_key", IMRecordset.Operation.Eq, subAreaKey);
            for (rsadm.Open(); !rsadm.IsEOF(); rsadm.MoveNext())
            {
                countourId = rsadm.GetI("contour_id");
            }
            if (rsadm.IsOpen())
                rsadm.Close();
            rsadm.Destroy();

            return countourId;
        }
    }
}
