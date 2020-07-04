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
    public class GetBrificAssigmentsByTargetExecutor : IReadQueryExecutor<GetBrificAssigmentsByTarget, List<AssignmentsAllotmentsModel>>
    {
        private readonly IObjectReader _objectReader;
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly DataMaper _mapper;

        public GetBrificAssigmentsByTargetExecutor(IObjectReader objectReader, AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _objectReader = objectReader;
            _config = config;
            _dataLayer = dataLayer;
            _mapper = new DataMaper(_objectReader);
        }
        public List<AssignmentsAllotmentsModel> Read(GetBrificAssigmentsByTarget criterion)
        {
            var assigns = new List<AssignmentsAllotmentsModel>();
            IMRecordset rs = new IMRecordset("fmtv_terra", IMRecordset.Mode.ReadOnly);
            rs.Select(_mapper.SelectStatementBrificAssignment);
            rs.SetWhere("adm_ref_id", IMRecordset.Operation.Eq, criterion.target.AdmRefId);
            rs.SetWhere("freq_assgn", IMRecordset.Operation.Eq, criterion.target.Freq_MHz);
            rs.SetWhere("long_dec", IMRecordset.Operation.Eq, criterion.target.Lon_Dec);
            rs.SetWhere("lat_dec", IMRecordset.Operation.Eq, criterion.target.Lat_Dec);
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                var assign = new AssignmentsAllotmentsModel() { Source = AssignmentsAllotmentsSourceType.ICSM, Type = AssignmentsAllotmentsModelType.Assignment };
                _mapper.GetBrificAssignment(assign, rs);
                assigns.Add(assign);
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            return assigns;
        }
    }
}
