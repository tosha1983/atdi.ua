using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XICSM.ICSControlClient.Models
{
    public class MeasTask
    {
        public long MeasTaskId;
        public string TaskType;
        public string TaskName;
        public double? FqMin;
        public double? FqMax;
        public DateTime DateStart;
        public DateTime DateStop;
        public DateTime? DateCreated;
        public string CreatedBy;
        public string Status;
        public string SensorIds;
    }
}
