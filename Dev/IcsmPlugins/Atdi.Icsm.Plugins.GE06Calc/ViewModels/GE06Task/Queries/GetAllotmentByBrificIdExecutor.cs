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
    public class GetAllotmentByBrificIdExecutor : IReadQueryExecutor<GetAllotmentByBrificId, AssignmentsAllotmentsModel>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetAllotmentByBrificIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public AssignmentsAllotmentsModel Read(GetAllotmentByBrificId criterion)
        {
            var allot = new AssignmentsAllotmentsModel() { Source = AssignmentsAllotmentsSourceType.Brific, Type = AssignmentsAllotmentsModelType.Allotment };
            IMRecordset rs = new IMRecordset("ge06_allot_terra", IMRecordset.Mode.ReadOnly);
            rs.Select("terrakey,adm,notice_typ,fragment,intent,adm_ref_id,plan_entry,allot_name,sfn_id,freq_assgn,polar,ref_plan_cfg,spect_mask,typ_ref_netwk");
            rs.SetWhere("terrakey", IMRecordset.Operation.Eq, criterion.Id);
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                allot.Adm = rs.GetS("adm");
                allot.NoticeType = rs.GetS("notice_typ");
                allot.Fragment = rs.GetS("fragment");
                allot.Action = StringConverter.ConvertToActionType(rs.GetS("intent"));
                allot.AdmRefId = rs.GetS("adm_ref_id");
                allot.PlanEntry = StringConverter.ConvertToPlanEntryType(rs.GetS("plan_entry"));
                allot.AdmAllotAssociatedId = rs.GetS("allot_name");
                allot.SfnId = rs.GetS("sfn_id");
                allot.Freq_MHz = rs.GetD("freq_assgn");
                allot.Polar = StringConverter.ConvertToPolarType(rs.GetS("polar"));
                allot.RefNetworkConfig = StringConverter.ConvertToRefNetworkConfigType(rs.GetS("ref_plan_cfg"));
                allot.SpectrumMask = StringConverter.ConvertToSpectrumMaskType(rs.GetS("spect_mask"));
                allot.RefNetwork = StringConverter.ConvertToRefNetworkType(rs.GetS("typ_ref_netwk"));
                allot.AllotmentName = rs.GetS("allot_name");
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();
            return allot;
        }
    }
}
