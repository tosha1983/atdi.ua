using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.Models.Views
{
    public class MeasTaskDetailStationViewModel
    {
        public long StationId { get; set; }

        public long OwnerId { get; set; }

        public string OwnerName { get; set; }

        public string OwnerOKPO { get; set; }

        public string OwnerZip { get; set; }

        public string OwnerCode { get; set; }

        public string OwnerAddres { get; set; }

        public string GlobalSID { get; set; }

        public double? SiteLon { get; set; }

        public double? SiteLat { get; set; }

        public string SiteAdress { get; set; }

        public string SiteRegion { get; set; }

        public SectorStationForMeas[] Sectors { get; set; }

        public string Status { get; set; }

        public string Standart { get; set; }

        public int? LicenseIcsmId { get; set; }

        public DateTime? LicenseStartDate { get; set; }

        public DateTime? LicenseEndDate { get; set; }

        public DateTime? LicenseCloseDate { get; set; }

        public string LicenseDozvilName { get; set; }
    }
}
