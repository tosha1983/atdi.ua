using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using IC_ES = Atdi.DataModels.Sdrn.Infocenter.Entities;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ProjectManager.Queries
{
    public class GCIDDataModelByParamsExecutor : IReadQueryExecutor<GCIDDataModelByParams, GCIDDataModel>
    {
        private readonly AppComponentConfig _config;
        private readonly InfocenterDataLayer _dataLayer;

        public GCIDDataModelByParamsExecutor(AppComponentConfig config, InfocenterDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public GCIDDataModel Read(GCIDDataModelByParams criterion)
        {
            var query = _dataLayer.GetBuilder<IC_ES.Stations.IGlobalIdentity>()
                .Read()
                .Select(
                     c => c.CreatedDate,
                     c => c.RealGsid,
                     c => c.Standard,
                     c => c.RegionCode,
                     c => c.LicenseGsid
                    )
                    .Filter(c => c.LicenseGsid, DataModels.Api.EntityOrm.WebClient.FilterOperator.Equal, criterion.LicenseGsid)
                    .Filter(c => c.Standard, DataModels.Api.EntityOrm.WebClient.FilterOperator.Equal, criterion.Standard)
                    .Filter(c => c.RegionCode, DataModels.Api.EntityOrm.WebClient.FilterOperator.Equal, criterion.RegionCode)
                    ;

            var reader = _dataLayer.Executor.ExecuteReader(query);
            if (!reader.Read())
            {
                return null;
            }
            return new GCIDDataModel()
            {
                RealGsid = reader.GetValue(c => c.RealGsid),
                Standard = reader.GetValue(c => c.Standard),
                CreatedDate = reader.GetValue(c => c.CreatedDate),
                RegionCode = reader.GetValue(c => c.RegionCode),
                LicenseGsid = reader.GetValue(c => c.LicenseGsid)
            };
        }
    }
}
