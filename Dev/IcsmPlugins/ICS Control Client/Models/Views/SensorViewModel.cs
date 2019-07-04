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

        public long Id;

        public string Status;

        public string Name;

        public string Administration;

        public string NetworkId;

        public string Remark;

        public DateTime? BiuseDate;

        public DateTime? EouseDate;

        public Double? Azimuth; //degree

        public Double? Elevation; //degree 

        public Double? AGL;

        public string IdSysARGUS;

        public string TypeSensor;

        public Double? StepMeasTime;

        public Double? RxLoss;

        public Double? OpHHFr;

        public Double? OpHHTo;

        public string OpDays;

        public string CustTxt1;

        public DateTime? CustData1;

        public Double? CustNbr1;

        public DateTime? DateCreated;

        public string CreatedBy;

        public SensorEquip Equipment;

        public SensorAntenna Antenna;

        public SensorLocation[] Locations;
    }
}
