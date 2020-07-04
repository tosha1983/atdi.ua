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
    public class GetAllotmentByBrificIdExecutor : IReadQueryExecutor<GetAllotmentByBrificId, AssignmentsAllotmentsModel>
    {
        private readonly IObjectReader _objectReader;
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly DataMaper _mapper;

        public GetAllotmentByBrificIdExecutor(IObjectReader objectReader, AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _objectReader = objectReader;
            _config = config;
            _dataLayer = dataLayer;
            _mapper = new DataMaper(_objectReader);
        }
        public AssignmentsAllotmentsModel Read(GetAllotmentByBrificId criterion)
        {
            var allot = new AssignmentsAllotmentsModel() { Source = AssignmentsAllotmentsSourceType.Brific, Type = AssignmentsAllotmentsModelType.Allotment };
            IMRecordset rs = new IMRecordset("ge06_allot_terra", IMRecordset.Mode.ReadOnly);
            rs.Select(_mapper.SelectStatementBrificAllotment);
            rs.SetWhere("terrakey", IMRecordset.Operation.Eq, criterion.Id);
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                _mapper.GetBrificAllotment(allot, rs);
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();
            return allot;
        }
    }
}
