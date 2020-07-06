using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.GN06;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task
{
    public class AssignmentsAllotmentsModel
    {
        public string Id { get; set; }
        public AssignmentsAllotmentsSourceType Source { get; set; }
        public AssignmentsAllotmentsModelType Type { get; set; }
        public string ViewName { get; set; }

        // AdministrativeData
        public string Adm { get; set; }
        public string NoticeType { get; set; }
        public string Fragment { get; set; }
        public ActionType Action { get; set; }
        public string AdmRefId { get; set; }
        public string StnClass { get; set; }
        public bool? IsDigital { get; set; }

        // DigitalPlanEntryParameters
        public PlanEntryType PlanEntry { get; set; }
        public AssignmentCodeType AssignmentCode { get; set; }
        public string AdmAllotAssociatedId { get; set; }
        public string SfnAllotAssociatedId { get; set; }
        public string SfnId { get; set; }

        // BroadcastingAssignmentEmissionCharacteristics
        public double Freq_MHz { get; set; }
        public PolarType Polar { get; set; }
        public float ErpH_dBW { get; set; }
        public float ErpV_dBW { get; set; }
        public RefNetworkConfigType RefNetworkConfig { get; set; }
        public SystemVariationType SystemVariation { get; set; } 
        public RxModeType RxMode { get; set; }
        public SpectrumMaskType SpectrumMask { get; set; }

        // BroadcastingAllotmentEmissionCharacteristics
        //public double Freq_MHz { get; set; }
        //public PolarType Polar { get; set; }
        //public RefNetworkConfigType RefNetworkConfig { get; set; }
        //public SpectrumMaskType SpectrumMask { get; set; }
        public RefNetworkType RefNetwork { get; set; }

        // BroadcastingAllotmentEmissionCharacteristics
        public double Lon_Dec { get; set; }
        public double Lat_Dec { get; set; }
        public short Alt_m { get; set; }
        public string Name { get; set; }

        // AllotmentParameters
        public string AllotmentName { get; set; } 
        public int ContourId { get; set; }
        public AreaPoint[] Contur { get; set; }

        // AntennaCharacteristics
        public AntennaDirectionType Direction { get; set; } 
        public short AglHeight_m { get; set; }  //  min = 0 max = 800
        public int MaxEffHeight_m { get; set; }  //  max from ArrEff_hgtAz_m
        public short[] EffHeight_m { get; set; } // в масиве 36 первый соответсвует нулевому азимуту елементов min = -3000 max = 3000
        public float[] DiagrH { get; set; }  // for polar H, M В масиве 36 елементов первый соответсвует нулевому азимуту min = 0 max = 40 
        public float[] DiagrV { get; set; }  // for polar V, M В масиве 36 елементов первый соответсвует нулевому азимуту min = 0 max = 40

        // BroadcastingAssignmentTarget
        public string TargetAdmRefId { get; set; } 
        public double TargetFreq_MHz { get; set; } 
        public double TargetLon_Dec { get; set; } 
        public double TargetLat_Dec { get; set; } 
    }
    public enum AssignmentsAllotmentsModelType
    {
        Allotment,
        Assignment
    }
    public enum AssignmentsAllotmentsSourceType
    {
        ICSM,
        Brific
    }
}
