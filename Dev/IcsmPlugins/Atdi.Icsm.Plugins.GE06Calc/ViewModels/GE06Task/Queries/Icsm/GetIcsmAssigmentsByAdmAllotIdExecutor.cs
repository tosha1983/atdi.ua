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
    public class GetIcsmAssigmentsByAdmAllotIdExecutor : IReadQueryExecutor<GetIcsmAssigmentsByAdmAllotId, List<AssignmentsAllotmentsModel>>
    {
        private readonly IObjectReader _objectReader;
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly DataMaper _mapper;

        public GetIcsmAssigmentsByAdmAllotIdExecutor(IObjectReader objectReader, AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _objectReader = objectReader;
            _config = config;
            _dataLayer = dataLayer;
            _mapper = new DataMaper(_objectReader);
        }
        public List<AssignmentsAllotmentsModel> Read(GetIcsmAssigmentsByAdmAllotId criterion)
        {
            var assigns = new List<AssignmentsAllotmentsModel>();
            IMRecordset rs = new IMRecordset("FMTV_ASSIGN", IMRecordset.Mode.ReadOnly);
            rs.Select(_mapper.SelectStatementIcsmAssignment);
            rs.SetWhere("ALLOTM_ADM_KEY", IMRecordset.Operation.Eq, criterion.Adm_Allot_Id);
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

            return assigns;
        }
    }
}
