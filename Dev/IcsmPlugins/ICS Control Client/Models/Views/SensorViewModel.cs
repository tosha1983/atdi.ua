using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.Models.Views
{
    public class SensorViewModel
    {

        public long Id { get; set; }

        public string Status { get; set; }

        public string Name { get; set; }

        public string Administration { get; set; }

        public string NetworkId { get; set; }

        public string Remark { get; set; }

        public DateTime? BiuseDate { get; set; }

        public DateTime? EouseDate { get; set; }

        public Double? Azimuth { get; set; }//degree

        public Double? Elevation { get; set; } //degree 

        public Double? AGL { get; set; }

        public string IdSysARGUS { get; set; }

        public string TypeSensor { get; set; }

        public Double? StepMeasTime { get; set; }

        public Double? RxLoss { get; set; }

        public Double? OpHHFr { get; set; }

        public Double? OpHHTo { get; set; }

        public string OpDays { get; set; }

        public string CustTxt1 { get; set; }

        public DateTime? CustData1 { get; set; }

        public Double? CustNbr1 { get; set; }

        public DateTime? DateCreated { get; set; }

        public string CreatedBy { get; set; }

        public SensorEquip Equipment { get; set; }

        public SensorAntenna Antenna { get; set; }

        public SensorLocation[] Locations { get; set; }
    }
}
