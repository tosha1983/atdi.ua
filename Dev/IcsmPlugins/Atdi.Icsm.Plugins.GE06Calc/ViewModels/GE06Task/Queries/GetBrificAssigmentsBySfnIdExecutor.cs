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
    public class GetBrificAssigmentsBySfnIdExecutor : IReadQueryExecutor<GetBrificAssigmentsBySfnId, List<AssignmentsAllotmentsModel>>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetBrificAssigmentsBySfnIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public List<AssignmentsAllotmentsModel> Read(GetBrificAssigmentsBySfnId criterion)
        {
            var assigns = new List<AssignmentsAllotmentsModel>();
            IMRecordset rs = new IMRecordset("fmtv_terra", IMRecordset.Mode.ReadOnly);
            rs.Select("terrakey,adm,notice_typ,fragment,intent,adm_ref_id,plan_entry,assgn_code,assoc_allot_id,assoc_allot_sfn_id,sfn_id,freq_assgn,polar,erp_h_dbw,erp_v_dbw,ref_plan_cfg,tran_sys,rx_mode,spect_mask,long_dec,lat_dec,site_alt,site_name,ant_dir,hgt_agl,eff_hgtmax,adm_ref_id,freq_assgn");
            rs.SetWhere("sfn_id", IMRecordset.Operation.Eq, criterion.SfnId);
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                var assign = new AssignmentsAllotmentsModel() { Source = AssignmentsAllotmentsSourceType.Brific, Type = AssignmentsAllotmentsModelType.Assignment };

                assign.Adm = rs.GetS("adm");
                assign.NoticeType = rs.GetS("notice_typ");
                assign.Fragment = rs.GetS("fragment");
                assign.Action = StringConverter.ConvertToActionType(rs.GetS("intent"));
                assign.AdmRefId = rs.GetS("adm_ref_id");
                assign.PlanEntry = StringConverter.ConvertToPlanEntryType(rs.GetS("plan_entry"));
                assign.AssignmentCode = StringConverter.ConvertToAssignmentCodeType(rs.GetS("assgn_code"));
                assign.AdmAllotAssociatedId = rs.GetS("assoc_allot_id");
                assign.SfnAllotAssociatedId = rs.GetS("assoc_allot_sfn_id");
                assign.SfnId = rs.GetS("sfn_id");
                assign.Freq_MHz = rs.GetD("freq_assgn");
                assign.Polar = StringConverter.ConvertToPolarType(rs.GetS("polar"));
                assign.ErpH_dBW = (float)rs.GetD("erp_h_dbw");
                assign.ErpV_dBW = (float)rs.GetD("erp_v_dbw");
                assign.RefNetworkConfig = StringConverter.ConvertToRefNetworkConfigType(rs.GetS("ref_plan_cfg"));
                assign.SystemVariation = StringConverter.ConvertToSystemVariationType(rs.GetS("tran_sys"));
                assign.RxMode = StringConverter.ConvertToRxModeType(rs.GetS("rx_mode"));
                assign.SpectrumMask = StringConverter.ConvertToSpectrumMaskType(rs.GetS("spect_mask"));
                assign.Lon_Dec = rs.GetD("long_dec");
                assign.Lat_Dec = rs.GetD("lat_dec");
                assign.Alt_m = (short)rs.GetI("site_alt");
                assign.Name = rs.GetS("site_name");
                assign.Direction = StringConverter.ConvertToAntennaDirectionType(rs.GetS("ant_dir"));
                assign.AglHeight_m = (short)rs.GetI("hgt_agl");
                assign.MaxEffHeight_m = rs.GetI("eff_hgtmax");
                assign.TargetAdmRefId = rs.GetS("adm_ref_id");
                assign.TargetFreq_MHz = rs.GetD("freq_assgn");
                assign.TargetLon_Dec = rs.GetD("long_dec");
                assign.TargetLat_Dec = rs.GetD("lat_dec");
                assigns.Add(assign);
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            return assigns;
        }
    }
}