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
    public class GetBrificAllotmentsByTargetExecutor : IReadQueryExecutor<GetBrificAllotmentsByTarget, List<AssignmentsAllotmentsModel>>
    {
        private readonly IObjectReader _objectReader;
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly DataMaper _mapper;

        public GetBrificAllotmentsByTargetExecutor(IObjectReader objectReader, AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _objectReader = objectReader;
            _config = config;
            _dataLayer = dataLayer;
            _mapper = new DataMaper(_objectReader);
        }
        public List<AssignmentsAllotmentsModel> Read(GetBrificAllotmentsByTarget criterion)
        {
            var allots = new List<AssignmentsAllotmentsModel>();
            IMRecordset rs = new IMRecordset("ge06_allot_terra", IMRecordset.Mode.ReadOnly);
            rs.Select(_mapper.SelectStatementBrificAllotment);
            rs.SetWhere("adm_ref_id", IMRecordset.Operation.Eq, criterion.PlanAssignNo);
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                var allot = new AssignmentsAllotmentsModel() { Source = AssignmentsAllotmentsSourceType.Brific, Type = AssignmentsAllotmentsModelType.Allotment };
                _mapper.GetBrificAllotment(allot, rs);
                allots.Add(allot);
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            return allots;
        }
    }
}
