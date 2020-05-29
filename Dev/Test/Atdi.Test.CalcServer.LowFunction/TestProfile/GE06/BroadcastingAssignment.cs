using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.CalcServer.LowFunction
{
    public class BroadcastingAllotment
    {
        public AdministrativeData Administrative;
        public TargetBroadcastingAssignment TargetBroadcastingAssignment; //M for Action = MODIFY
        public EmissionCharacteristicsAlotment EmissionCharacteristicsAlotment;
        public AllotmentParameters AllotmentParameters;
        public DigitalPlanEntryParameters DigitalPlanEntryParameters;
    }
    public class BroadcastingAssignment
    {
        public AdministrativeData Administrative;
        public TargetBroadcastingAssignment TargetBroadcastingAssignment; //M for Action = MODIFY
        public EmissionCharacteristics EmissionCharacteristics;
        public SiteParameters SiteParameters;
        public AntennaCharacteristics AntennaCharacteristics;
        public DigitalPlanEntryParameters DigitalPlanEntryParameters;
    }
    public class AdministrativeData
    {
        public string Adm; // M
        public string Notice_type; // M
        public string Fragment; // O
        public ActionType Action; //M
        public string Adm_ref_id; //M ADM_KEY ICSM 
    }
    public class DigitalPlanEntryParameters
    {
        public Plan_entryType Plan_entry; //M
        public Assgn_codeType Assgn_code; //M
        public string Associated_adm_allot_id;//O
        public string Associated_allot_sfn_id;//O
        public string Sfn_id;//O
    }
    public class EmissionCharacteristics
    {
        public double Freq_assgn_MHz; //M есть жесткое ограничение данного поля. смотри соглашение женева 06 Максим
        public PolarType Polar; //M
        public float Erp_h_dBW; // max 53
        public float Erp_v_dBW; // max 53
        public Ref_plan_cfgType Ref_plan_cfg;//  Ref_plan_cfg of (Sys_var and Rx_mode) is M
        public Sys_varType Sys_var; // 
        public Rx_modeType Rx_mode; // 
        public Spect_maskType Spect_mask; //M
    }
    public class EmissionCharacteristicsAlotment
    {
        public double Freq_assgn_MHz; //M есть жесткое ограничение данного поля. смотри соглашение женева 06 Максим
        public PolarType Polar; //M
        public Ref_plan_cfgType Ref_plan_cfg;//  Ref_plan_cfg of (Sys_var and Rx_mode) is M
        public Spect_maskType Spect_mask; //M
        public Typ_ref_netwkType Typ_Ref_Netwk; //M

    }
    public class SiteParameters
    {
        public double Lon_DEC; //M
        public double Lat_DEC; //M
        public int site_alt_m; // M min = -1000 max = 8850 
        public string Site_name;//M
    }
    public class AllotmentParameters
    {
        public string Allot_name;//M
        public int Contour_id;//M
    }
    public class AreaPoint
    {
        public double Lon_DEC;
        public double Lat_DEC;
    }
    public class AntennaCharacteristics
    {
        public Ant_dirType ant_dir; //M
        public int hgt_agl_m; // M min = 0 max = 800
        public int eff_hgtmax_m; // M max from ArrEff_hgtAz_m
        public int[] ArrEff_hgtAz_m; // в масиве 36 первый соответсвует нулевому азимуту елементов min = -3000 max = 3000
        public float[] ArrAttnAnt_Diagr_H;  //M for polar H, M В масиве 36 елементов первый соответсвует нулевому азимуту min = 0 max = 40 
        public float[] ArrAttnAnt_Diagr_V;  //M for polar V, M В масиве 36 елементов первый соответсвует нулевому азимуту min = 0 max = 40
    }
    public class TargetBroadcastingAssignment
    {
        public string Adm_ref_id; // M
        public double Freq_assgn; // M
        public double Lon_DEC; // M
        public double Lat_DEC; // M
    }
    public enum ActionType
    {
        ADD,
        MODIFY
    }
    public enum Assgn_codeType
    {
        U = 0, // unknown
        L, // linked with a SFN or an allotment, 
        C, // converted, 
        S // standalone
    }
    public enum Plan_entryType
    {
        Unknown = 0,
        SingleAssignment = 1,
        SFN = 2,
        Allotment = 3,
        AllotmentWithLinkedAssignmentAndSFN_id = 4, 
        AllotmentWithSingleLinkedAssignmentAndNoSFN_id = 5
    }
    public enum Ref_plan_cfgType
    {
        Unknown = 0,
        RPC1 = 1,
        RPC2 = 2,
        RPC3 = 3,
        RPC4 = 4,
        RPC5 = 5
    }
    public enum Sys_varType
    {
        Unknown = 0,
        A1, A2, A3, A5, A7,
        B1, B2, B3, B5, B7,
        C1, C2, C3, C5, C7,
        D1, D2, D3, D5, D7,
        E1, E2, E3, E5, E7,
        F1, F2, F3, F5, F7
    }
    public enum Rx_modeType
    {
        Unknown = 0,
        FX,
        PO,
        PI,
        MO
    }
    public enum Spect_maskType
    {
        N,
        S
    }
    public enum Ant_dirType
    {
        D,
        ND
    }
    public enum PolarType
    {
        H,
        V,
        M
    }
    public enum Typ_ref_netwkType
    {
        RN1 = 1,
        RN2 = 2,
        RN3 = 3,
        RN4 = 4
    }


}
