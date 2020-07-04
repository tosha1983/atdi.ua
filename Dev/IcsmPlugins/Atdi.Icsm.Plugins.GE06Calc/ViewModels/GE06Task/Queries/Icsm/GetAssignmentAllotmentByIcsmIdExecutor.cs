using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.Platform.Cqrs;
using ICSM;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Queries
{
    public class GetAssignmentAllotmentByIcsmIdExecutor : IReadQueryExecutor<GetAssignmentAllotmentByIcsmId, AssignmentsAllotmentsModel>
    {
        private readonly IObjectReader _objectReader;
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly DataMaper _mapper;

        public GetAssignmentAllotmentByIcsmIdExecutor(IObjectReader objectReader, AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _objectReader = objectReader;
            _config = config;
            _dataLayer = dataLayer;
            _mapper = new DataMaper(_objectReader);
        }
        public AssignmentsAllotmentsModel Read(GetAssignmentAllotmentByIcsmId criterion)
        {
            var allotAssign = new AssignmentsAllotmentsModel() { Source = AssignmentsAllotmentsSourceType.ICSM };
            IMRecordset rs = new IMRecordset("FMTV_ASSIGN", IMRecordset.Mode.ReadOnly);
            rs.Select(_mapper.SelectStatementIcsmAll);
            rs.SetWhere("ID", IMRecordset.Operation.Eq, criterion.Id);
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                if (rs.GetS("IS_ALLOTM") == "Y")
                {
                    _mapper.GetIcsmAllotment(allotAssign, rs);
                }
                else
                {
                    _mapper.GetIcsmAssignment(allotAssign, rs);
                }
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            return allotAssign;
        }
    }
}
