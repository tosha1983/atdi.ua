using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using IC_ES = Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ProjectManager.Queries
{
    public class MobStationsLoadExecutor : IReadQueryExecutor<MobStationsLoadModelByParams, IcsmMobStation[]>
    {
        private readonly AppComponentConfig _config;
        private MobStationsDataAdapter MobStationsDataAdapter;
        public readonly IObjectReader _objectReader;


        public MobStationsLoadExecutor(AppComponentConfig config, IObjectReader objectReader)
        {
            this._config = config;
            this._objectReader = objectReader;
        }
        public IcsmMobStation[] Read(MobStationsLoadModelByParams criterion)
        {
            MobStationsDataAdapter = new MobStationsDataAdapter(criterion, this._objectReader);
            return MobStationsDataAdapter.LoadStations();
        }
    }
}
