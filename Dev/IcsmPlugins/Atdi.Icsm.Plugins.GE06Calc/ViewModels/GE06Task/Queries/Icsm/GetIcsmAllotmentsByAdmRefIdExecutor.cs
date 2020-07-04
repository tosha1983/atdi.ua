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
    public class GetIcsmAllotmentsByAdmRefIdExecutor : IReadQueryExecutor<GetIcsmAllotmentsByAdmRefId, List<AssignmentsAllotmentsModel>>
    {
        private readonly IObjectReader _objectReader;
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly DataMaper _mapper;

        public GetIcsmAllotmentsByAdmRefIdExecutor(IObjectReader objectReader, AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _objectReader = objectReader;
            _config = config;
            _dataLayer = dataLayer;
            _mapper = new DataMaper(_objectReader);
        }
        public List<AssignmentsAllotmentsModel> Read(GetIcsmAllotmentsByAdmRefId criterion)
        {
            var allots = new List<AssignmentsAllotmentsModel>();
            IMRecordset rs = new IMRecordset("FMTV_ASSIGN", IMRecordset.Mode.ReadOnly);
            rs.Select(_mapper.SelectStatementIcsmAllotment);
            rs.SetWhere("ADM_KEY", IMRecordset.Operation.Eq, criterion.Adm_Ref_Id);
            rs.SetWhere("IS_ALLOTM", IMRecordset.Operation.Eq, "Y");
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                var allot = new AssignmentsAllotmentsModel() { Source = AssignmentsAllotmentsSourceType.ICSM, Type = AssignmentsAllotmentsModelType.Allotment };
                _mapper.GetIcsmAllotment(allot, rs);
                allots.Add(allot);
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            return allots;
        }
    }
}
