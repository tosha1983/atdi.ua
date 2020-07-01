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
    public class GetAssignmentByIcsmIdExecutor : IReadQueryExecutor<GetAssignmentByIcsmId, AssignmentsAllotmentsModel>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetAssignmentByIcsmIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public AssignmentsAllotmentsModel Read(GetAssignmentByIcsmId criterion)
        {
            var assign = new AssignmentsAllotmentsModel() { Source = AssignmentsAllotmentsSourceType.ICSM, Type = AssignmentsAllotmentsModelType.Assignment };
            IMRecordset rs = new IMRecordset("FMTV_ASSIGN", IMRecordset.Mode.ReadOnly);
            rs.Select("ID,ADM,NOTICE_TYPE,FRAGMENT,LASTK_REF,ADM_KEY,PLAN_ENTRY,ASSGN_CODE,PLAN_TRG_ADM_REF_ID,SFN_IDENT,ALLOTM_SFN_IDENT,FREQ,POLARIZATION,ERP_H,ERP_V,REF_PLAN_CFG,ADM_KEY");
            rs.SetWhere("ID", IMRecordset.Operation.Eq, criterion.Id);
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                assign.Adm = rs.GetS("ADM");
                assign.NoticeType = rs.GetS("NOTICE_TYPE");
                assign.Fragment = rs.GetS("FRAGMENT");
                assign.Action = StringConverter.ConvertToActionType(rs.GetS("LASTK_REF"));
                assign.AdmRefId = rs.GetS("ADM_KEY");
                assign.PlanEntry = StringConverter.ConvertToPlanEntryType(rs.GetI("PLAN_ENTRY"));
                assign.AssignmentCode = StringConverter.ConvertToAssignmentCodeType(rs.GetS("ASSGN_CODE"));
                assign.AdmAllotAssociatedId = rs.GetS("PLAN_TRG_ADM_REF_ID");
                assign.SfnAllotAssociatedId = rs.GetS("SFN_IDENT");
                assign.SfnId = rs.GetS("ALLOTM_SFN_IDENT");
                assign.Freq_MHz = rs.GetD("FREQ");
                assign.ErpH_dBW = (float)rs.GetD("ERP_H");
                assign.ErpV_dBW = (float)rs.GetD("ERP_V");
                assign.RefNetworkConfig = StringConverter.ConvertToRefNetworkConfigType(rs.GetS("REF_PLAN_CFG"));
                assign.AdmRefId = rs.GetS("ADM_KEY");
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            return assign;
        }
    }
}
