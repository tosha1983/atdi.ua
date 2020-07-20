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
    public class GetIcsmAssigmentsBySfnIdExecutor : IReadQueryExecutor<GetIcsmAssigmentsBySfnId, List<AssignmentsAllotmentsModel>>
    {
        private readonly IObjectReader _objectReader;
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly DataMaper _mapper;

        public GetIcsmAssigmentsBySfnIdExecutor(IObjectReader objectReader, AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _objectReader = objectReader;
            _config = config;
            _dataLayer = dataLayer;
            _mapper = new DataMaper(_objectReader);
        }
        public List<AssignmentsAllotmentsModel> Read(GetIcsmAssigmentsBySfnId criterion)
        {
            var assigns = new List<AssignmentsAllotmentsModel>();
            IMRecordset rs = new IMRecordset("FMTV_ASSIGN", IMRecordset.Mode.ReadOnly);
            rs.Select(_mapper.SelectStatementIcsmAssignment);
            rs.SetWhere("ALLOTM_SFN_IDENT", IMRecordset.Operation.Eq, criterion.SfnId);
            rs.SetWhere("IS_ALLOTM", IMRecordset.Operation.Eq, "N");
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                var assign = new AssignmentsAllotmentsModel() { Source = AssignmentsAllotmentsSourceType.ICSM, Type = AssignmentsAllotmentsModelType.Assignment };
                _mapper.GetIcsmAssignment(assign, rs);
                assigns.Add(assign);
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            IMRecordset rs2 = new IMRecordset("FMTV_ASSIGN", IMRecordset.Mode.ReadOnly);
            rs2.Select(_mapper.SelectStatementIcsmAssignment);
            rs2.SetWhere("SFN_IDENT", IMRecordset.Operation.Eq, criterion.SfnId);
            rs2.SetWhere("IS_ALLOTM", IMRecordset.Operation.Eq, "N");
            for (rs2.Open(); !rs2.IsEOF(); rs2.MoveNext())
            {
                var assign = new AssignmentsAllotmentsModel() { Source = AssignmentsAllotmentsSourceType.ICSM, Type = AssignmentsAllotmentsModelType.Assignment };
                _mapper.GetIcsmAssignment(assign, rs2);
                assigns.Add(assign);
            }
            if (rs2.IsOpen())
                rs2.Close();
            rs2.Destroy();

            return assigns;
        }
    }
}
