using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace XICSM.ICSControlClient.Models
{
    public class GroupeEmissionProtocolDataFilter
    {
        public string Owner { get; set; }
        public string DateMeasDay { get; set; }
        public string DateMeasMonth { get; set; }
        public string DateMeasYear { get; set; }
        public string Standard { get; set; }
        public string FreqStart { get; set; }
        public string FreqStop { get; set; }
        public double? Probability { get; set; }
        public string Province { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string PermissionNumber { get; set; }
        public DateTime? PermissionStart { get; set; }
        public DateTime? PermissionStop { get; set; }
    }
}
