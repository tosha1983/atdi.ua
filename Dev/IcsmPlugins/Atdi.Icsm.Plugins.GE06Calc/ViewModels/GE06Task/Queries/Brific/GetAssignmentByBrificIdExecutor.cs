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
    public class GetAssignmentByBrificIdExecutor : IReadQueryExecutor<GetAssignmentByBrificId, AssignmentsAllotmentsModel>
    {
        private readonly IObjectReader _objectReader;
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly DataMaper _mapper;

        public GetAssignmentByBrificIdExecutor(IObjectReader objectReader, AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _objectReader = objectReader;
            _config = config;
            _dataLayer = dataLayer;
            _mapper = new DataMaper(_objectReader);
        }
        public AssignmentsAllotmentsModel Read(GetAssignmentByBrificId criterion)
        {
            var assign = new AssignmentsAllotmentsModel() { Source = AssignmentsAllotmentsSourceType.Brific, Type = AssignmentsAllotmentsModelType.Assignment };
            IMRecordset rs = new IMRecordset("fmtv_terra", IMRecordset.Mode.ReadOnly);
            rs.Select(_mapper.SelectStatementBrificAssignment);
            rs.SetWhere("terrakey", IMRecordset.Operation.Eq, criterion.Id);
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                _mapper.GetBrificAssignment(assign, rs);
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            return assign;
        }
    }
}
