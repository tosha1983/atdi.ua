using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.Licensing
{
    public class VerificationResult
    {
        public string LicenseNumber { get; set; }

        public string OwnerName { get; set; }

        public DateTime StopDate { get; set; }

        public int Count { get; set; }

        public string Instance { get; set; }

        public DateTime StartDate { get; set; }
    }

    public static class VerificationResultExtention
    {
        public static void Verify(this VerificationResult result)
        {
            if (result == null)
            {
                throw new Exception("License data is not found");
            }
            var date = DateTime.Now;

            if (result.StartDate > date)
            {
                throw new Exception("The time to start using the license has not yet come");
            }
            if (result.StopDate <= date)
            {
                throw new Exception("License was expired");
            }

        }
    }
}
