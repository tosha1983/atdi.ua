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
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetIcsmAllotmentsByAdmRefIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public List<AssignmentsAllotmentsModel> Read(GetIcsmAllotmentsByAdmRefId criterion)
        {
            var allots = new List<AssignmentsAllotmentsModel>();
            IMRecordset rs = new IMRecordset("FMTV_ASSIGN", IMRecordset.Mode.ReadOnly);
            rs.Select("ID,ADM,NOTICE_TYPE,FRAGMENT,LASTK_REF,ADM_KEY,PLAN_ENTRY,PLAN_TRG_ADM_REF_ID,SFN_IDENT,FREQ,POLARIZATION,REF_PLAN_CFG,SPECT_MASK,TYP_REF_NTWK,A_NAME,ALLOT_AREA");
            rs.SetWhere("ADM_KEY", IMRecordset.Operation.Eq, criterion.Adm_Ref_Id);
            rs.SetWhere("IS_ALLOTM", IMRecordset.Operation.Eq, "Y");
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                var allot = new AssignmentsAllotmentsModel() { Source = AssignmentsAllotmentsSourceType.ICSM, Type = AssignmentsAllotmentsModelType.Allotment };
                allot.Adm = rs.GetS("ADM");
                allot.NoticeType = rs.GetS("NOTICE_TYPE");
                allot.Fragment = rs.GetS("FRAGMENT");
                allot.Action = StringConverter.ConvertToActionType(rs.GetS("LASTK_REF"));
                allot.AdmRefId = rs.GetS("ADM_KEY");
                allot.PlanEntry = StringConverter.ConvertToPlanEntryType(rs.GetS("PLAN_ENTRY"));
                allot.AdmAllotAssociatedId = rs.GetS("PLAN_TRG_ADM_REF_ID");
                allot.SfnId = rs.GetS("SFN_IDENT");
                allot.Freq_MHz = rs.GetD("FREQ");
                allot.Polar = StringConverter.ConvertToPolarType(rs.GetS("POLARIZATION"));
                allot.RefNetworkConfig = StringConverter.ConvertToRefNetworkConfigType(rs.GetS("REF_PLAN_CFG"));
                allot.SpectrumMask = StringConverter.ConvertToSpectrumMaskType(rs.GetS("SPECT_MASK"));
                allot.RefNetwork = StringConverter.ConvertToRefNetworkType(rs.GetS("TYP_REF_NTWK"));
                allot.AllotmentName = rs.GetS("A_NAME");
                if (Int32.TryParse(rs.GetS("ALLOT_AREA").Replace(",", ""), out int ContourId))
                    allot.ContourId = ContourId;
                allots.Add(allot);
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            return allots;
        }
    }
}
