using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Cqrs;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager;
using Atdi.Contracts.Sdrn.DeepServices.RadioSystem;
using Atdi.Platform.Logging;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ProjectManager.Queries
{
    public class MobStationsLoadExecutor : IReadQueryExecutor<MobStationsLoadModelByParams, IcsmMobStation[]>
    {
        private readonly AppComponentConfig _config;
        private MobStationsDataAdapter MobStationsDataAdapter;
        private readonly IObjectReader _objectReader;
        public readonly ISignalService _signalService;
        private readonly ILogger _logger;


        public MobStationsLoadExecutor(AppComponentConfig config,
            ISignalService signalService,
            ILogger logger,
            IObjectReader objectReader)
        {
            this._logger = logger;
            this._config = config;
            this._signalService = signalService;
            this._objectReader = objectReader;
        }
        public IcsmMobStation[] Read(MobStationsLoadModelByParams criterion)
        {
            MobStationsDataAdapter = new MobStationsDataAdapter(criterion, this._logger, this._signalService, this._objectReader);
            return MobStationsDataAdapter.LoadStations();
        }
    }
}
