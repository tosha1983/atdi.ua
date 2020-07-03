using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.Platform.Cqrs;
using ICSM;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Queries
{
    public class GetAssignmentAllotmentByIcsmIdExecutor : IReadQueryExecutor<GetAssignmentAllotmentByIcsmId, AssignmentsAllotmentsModel>
    {
        private readonly IObjectReader _objectReader;
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetAssignmentAllotmentByIcsmIdExecutor(IObjectReader objectReader, AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _objectReader = objectReader;
            _config = config;
            _dataLayer = dataLayer;
        }
        public AssignmentsAllotmentsModel Read(GetAssignmentAllotmentByIcsmId criterion)
        {
            var allotAssign = new AssignmentsAllotmentsModel() { Source = AssignmentsAllotmentsSourceType.ICSM };
            IMRecordset rs = new IMRecordset("FMTV_ASSIGN", IMRecordset.Mode.ReadOnly);
            rs.Select("ID,IS_ALLOTM,ADM,NOTICE_TYPE,FRAGMENT,LASTK_REF,ADM_KEY,PLAN_ENTRY,ASSGN_CODE,PLAN_TRG_ADM_REF_ID,SFN_IDENT,ALLOTM_SFN_IDENT,FREQ,POLARIZATION,ERP_H,ERP_V,REF_PLAN_CFG,ADM_KEY,TVSYS_CODE,RX_MODE,SPECT_MASK,LONGITUDE,LATITUDE,SITE_ALT,SITE_NAME,ANT_DIR,AGL,EFHGT_MAX,EFHGT,ATTN_H,ATTN_V,TYP_REF_NTWK,A_NAME,ALLOT_AREA");
            rs.SetWhere("ID", IMRecordset.Operation.Eq, criterion.Id);
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                if (rs.GetS("IS_ALLOTM") == "Y")
                {
                    allotAssign.Type = AssignmentsAllotmentsModelType.Allotment;
                    allotAssign.Adm = rs.GetS("ADM");
                    allotAssign.NoticeType = rs.GetS("NOTICE_TYPE");
                    allotAssign.Fragment = rs.GetS("FRAGMENT");
                    allotAssign.Action = StringConverter.ConvertToActionType(rs.GetS("LASTK_REF"));
                    allotAssign.AdmRefId = rs.GetS("ADM_KEY");
                    allotAssign.PlanEntry = StringConverter.ConvertToPlanEntryType(rs.GetI("PLAN_ENTRY"));
                    allotAssign.AdmAllotAssociatedId = rs.GetS("PLAN_TRG_ADM_REF_ID");
                    allotAssign.SfnId = rs.GetS("SFN_IDENT");
                    allotAssign.Freq_MHz = rs.GetD("FREQ");
                    allotAssign.Polar = StringConverter.ConvertToPolarType(rs.GetS("POLARIZATION"));
                    allotAssign.RefNetworkConfig = StringConverter.ConvertToRefNetworkConfigType(rs.GetS("REF_PLAN_CFG"));
                    allotAssign.SpectrumMask = StringConverter.ConvertToSpectrumMaskType(rs.GetS("SPECT_MASK"));
                    allotAssign.RefNetwork = StringConverter.ConvertToRefNetworkType(rs.GetS("TYP_REF_NTWK"));
                    allotAssign.AllotmentName = rs.GetS("A_NAME");
                    if (Int32.TryParse(rs.GetS("ALLOT_AREA").Replace(",", ""), out int ContourId))
                        allotAssign.ContourId = ContourId;
                    allotAssign.Сontur = _objectReader.Read<AreaPoint[]>().By(new GetAreaPointBySubAreaKey { SubAreaKey = allotAssign.ContourId });
                }
                else
                {
                    allotAssign.Type = AssignmentsAllotmentsModelType.Assignment;
                    allotAssign.Adm = rs.GetS("ADM");
                    allotAssign.NoticeType = rs.GetS("NOTICE_TYPE");
                    allotAssign.Fragment = rs.GetS("FRAGMENT");
                    allotAssign.Action = StringConverter.ConvertToActionType(rs.GetS("LASTK_REF"));
                    allotAssign.AdmRefId = rs.GetS("ADM_KEY");
                    allotAssign.PlanEntry = StringConverter.ConvertToPlanEntryType(rs.GetI("PLAN_ENTRY"));
                    allotAssign.AssignmentCode = StringConverter.ConvertToAssignmentCodeType(rs.GetS("ASSGN_CODE"));
                    allotAssign.AdmAllotAssociatedId = rs.GetS("PLAN_TRG_ADM_REF_ID");
                    allotAssign.SfnAllotAssociatedId = rs.GetS("SFN_IDENT");
                    allotAssign.SfnId = rs.GetS("ALLOTM_SFN_IDENT");
                    allotAssign.Freq_MHz = rs.GetD("FREQ");
                    allotAssign.ErpH_dBW = (float)rs.GetD("ERP_H");
                    allotAssign.ErpV_dBW = (float)rs.GetD("ERP_V");
                    allotAssign.RefNetworkConfig = StringConverter.ConvertToRefNetworkConfigType(rs.GetS("REF_PLAN_CFG"));
                    allotAssign.SystemVariation = StringConverter.ConvertToSystemVariationType(rs.GetS("TVSYS_CODE"));
                    allotAssign.RxMode = StringConverter.ConvertToRxModeType(rs.GetS("RX_MODE"));
                    allotAssign.SpectrumMask = StringConverter.ConvertToSpectrumMaskType(rs.GetS("SPECT_MASK"));
                    allotAssign.Lon_Dec = rs.GetD("LONGITUDE");
                    allotAssign.Lat_Dec = rs.GetD("LATITUDE");
                    allotAssign.Alt_m = (short)rs.GetD("SITE_ALT");
                    allotAssign.Name = rs.GetS("SITE_NAME");
                    allotAssign.Direction = StringConverter.ConvertToAntennaDirectionType(rs.GetS("ANT_DIR"));
                    allotAssign.AglHeight_m = (short)rs.GetD("AGL");
                    allotAssign.MaxEffHeight_m = rs.GetI("EFHGT_MAX");
                    allotAssign.EffHeight_m = StringConverter.ConvertToEffHeight(rs.GetS("EFHGT"));
                    allotAssign.DiagrH = StringConverter.ConvertToDiagrH(rs.GetS("ATTN_H"));
                    allotAssign.DiagrV = StringConverter.ConvertToDiagrV(rs.GetS("ATTN_V"));
                    allotAssign.TargetLon_Dec = rs.GetD("LONGITUDE");
                    allotAssign.TargetLat_Dec = rs.GetD("LATITUDE");
                }
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            return allotAssign;
        }
    }
}
