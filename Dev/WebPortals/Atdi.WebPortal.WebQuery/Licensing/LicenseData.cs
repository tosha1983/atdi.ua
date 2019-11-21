using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.Licensing
{
    [Serializable]
    public class LicenseData
    {
        public string LicenseNumber { get; set; }

        public string LicenseType { get; set; }

        public string Company { get; set; }

        public string Copyright { get; set; }

        public string OwnerName { get; set; }

        public string OwnerId { get; set; }

        public string ProductName {get; set; }

        public string ProductKey { get; set; }

        public DateTime Created { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime StopDate { get; set; }

        public int Count { get; set; }

        public string Instance { get; set; }
    }

    [Flags]
    public enum LicenseLimitationTerms
    {
        TimePeriod = 1,
        Version = 2,
        Assembly = 4,
        Year = 8,
        Host = 16
    }

    [Serializable]
    public class LicenseData2 : LicenseData
    {
        public LicenseLimitationTerms LimitationTerms { get; set; }

        public string Version { get; set; }

        public ushort Year { get; set; }

        public string AssemblyFullName { get; set; }

        public string HostData { get; set; }
    }
}
