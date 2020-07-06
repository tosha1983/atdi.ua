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
    public class DataMaper
    {
        private readonly IObjectReader _objectReader;
        public DataMaper(IObjectReader objectReader)
        {
            _objectReader = objectReader;
        }
        public string SelectStatementIcsmAll = "ID,ADM,IS_ALLOTM,NOTICE_TYPE,FRAGMENT,LASTK_REF,ADM_KEY,PLAN_ENTRY,ASSGN_CODE,PLAN_TRG_ADM_REF_ID,SFN_IDENT,ALLOTM_SFN_IDENT,FREQ,POLARIZATION,ERP_H,ERP_V,REF_PLAN_CFG,ADM_KEY,TVSYS_CODE,RX_MODE,SPECT_MASK,LONGITUDE,LATITUDE,SITE_ALT,SITE_NAME,ANT_DIR,AGL,EFHGT_MAX,EFHGT,ATTN_H,ATTN_V,TYP_REF_NTWK,A_NAME,ALLOT_AREA,CLASS,DIGITAL";

        public string SelectStatementIcsmAllotment = "ID,ADM,IS_ALLOTM,NOTICE_TYPE,FRAGMENT,LASTK_REF,ADM_KEY,PLAN_ENTRY,PLAN_TRG_ADM_REF_ID,SFN_IDENT,FREQ,POLARIZATION,REF_PLAN_CFG,SPECT_MASK,TYP_REF_NTWK,A_NAME,ALLOT_AREA";
        public void GetIcsmAllotment(AssignmentsAllotmentsModel allot, IMRecordset rs)
        {
            allot.Adm = rs.GetS("ADM");
            allot.NoticeType = rs.GetS("NOTICE_TYPE");
            allot.Fragment = rs.GetS("FRAGMENT");
            allot.Action = StringConverter.ConvertToActionType(rs.GetS("LASTK_REF"));
            allot.AdmRefId = rs.GetS("ADM_KEY");
            allot.PlanEntry = StringConverter.ConvertToPlanEntryType(rs.GetI("PLAN_ENTRY"));
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
            allot.Сontur = _objectReader.Read<AreaPoint[]>().By(new GetIcsmAreaPointByContourId { ContourId = allot.ContourId });
        }

        public string SelectStatementIcsmAssignment = "ID,ADM,IS_ALLOTM,NOTICE_TYPE,FRAGMENT,LASTK_REF,ADM_KEY,PLAN_ENTRY,ASSGN_CODE,PLAN_TRG_ADM_REF_ID,SFN_IDENT,ALLOTM_SFN_IDENT,FREQ,POLARIZATION,ERP_H,ERP_V,REF_PLAN_CFG,ADM_KEY,TVSYS_CODE,RX_MODE,SPECT_MASK,LONGITUDE,LATITUDE,SITE_ALT,SITE_NAME,ANT_DIR,AGL,EFHGT_MAX,EFHGT,ATTN_H,ATTN_V,CLASS,DIGITAL";
        public void GetIcsmAssignment(AssignmentsAllotmentsModel assign, IMRecordset rs)
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
            assign.SystemVariation = StringConverter.ConvertToSystemVariationType(rs.GetS("TVSYS_CODE"));
            assign.RxMode = StringConverter.ConvertToRxModeType(rs.GetS("RX_MODE"));
            assign.SpectrumMask = StringConverter.ConvertToSpectrumMaskType(rs.GetS("SPECT_MASK"));
            assign.Lon_Dec = rs.GetD("LONGITUDE");
            assign.Lat_Dec = rs.GetD("LATITUDE");
            assign.Alt_m = (short)rs.GetD("SITE_ALT");
            assign.Name = rs.GetS("SITE_NAME");
            assign.Direction = StringConverter.ConvertToAntennaDirectionType(rs.GetS("ANT_DIR"));
            assign.AglHeight_m = (short)rs.GetD("AGL");
            assign.MaxEffHeight_m = rs.GetI("EFHGT_MAX");
            assign.EffHeight_m = StringConverter.ConvertToEffHeight(rs.GetS("EFHGT"));
            assign.DiagrH = StringConverter.ConvertToDiagrH(rs.GetS("ATTN_H"));
            assign.DiagrV = StringConverter.ConvertToDiagrV(rs.GetS("ATTN_V"));
            assign.TargetLon_Dec = rs.GetD("LONGITUDE");
            assign.TargetLat_Dec = rs.GetD("LATITUDE");
            assign.StnClass = rs.GetS("CLASS");
            assign.IsDigital = rs.GetD("DIGITAL") == 1 ? true : false;
        }

        public string SelectStatementBrificAllotment = "terrakey,adm,notice_typ,fragment,intent,adm_ref_id,plan_entry,allot_name,sfn_id,freq_assgn,polar,ref_plan_cfg,spect_mask,typ_ref_netwk";
        public void GetBrificAllotment(AssignmentsAllotmentsModel allot, IMRecordset rs)
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
            allot.ContourId = _objectReader.Read<int>().By(new GeBrificCounturIdByTerrakey { terrakey = rs.GetI("terrakey") });
            allot.Сontur = _objectReader.Read<AreaPoint[]>().By(new GetBrificAreaPointBySubAreaKey { SubAreaKey = allot.ContourId });
        }

        public string SelectStatementBrificAssignment = "terrakey,adm,notice_typ,fragment,intent,adm_ref_id,plan_entry,assgn_code,assoc_allot_id,assoc_allot_sfn_id,sfn_id,freq_assgn,polar,erp_h_dbw,erp_v_dbw,ref_plan_cfg,tran_sys,rx_mode,spect_mask,long_dec,lat_dec,site_alt,site_name,ant_dir,hgt_agl,eff_hgtmax,adm_ref_id,freq_assgn,stn_cls,is_digital";
        public void GetBrificAssignment(AssignmentsAllotmentsModel assign, IMRecordset rs)
        {
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
            assign.EffHeight_m = _objectReader.Read<List<short>>().By(new GetBrificEfhgtByTerrakey { terrakey = rs.GetI("terrakey") }).ToArray();
            assign.DiagrH = _objectReader.Read<List<float>>().By(new GetBrificDiagrByTerrakey { terrakey = rs.GetI("terrakey"), polar = "H" }).ToArray();
            assign.DiagrV = _objectReader.Read<List<float>>().By(new GetBrificDiagrByTerrakey { terrakey = rs.GetI("terrakey"), polar = "V" }).ToArray();
            assign.TargetAdmRefId = rs.GetS("adm_ref_id");
            assign.TargetFreq_MHz = rs.GetD("freq_assgn");
            assign.TargetLon_Dec = rs.GetD("long_dec");
            assign.TargetLat_Dec = rs.GetD("lat_dec");
            assign.StnClass = rs.GetS("stn_cls");
            assign.IsDigital = bool.Parse(rs.GetS("is_digital"));
        }
    }
}
