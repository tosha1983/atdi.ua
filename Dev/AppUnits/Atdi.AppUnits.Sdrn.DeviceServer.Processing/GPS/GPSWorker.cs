using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using Atdi.DataModels.EntityOrm;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.Sdrn.DeviceServer.GPS;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public class GPSWorker : ITaskWorker<GPSTask, BaseContext, SingletonTaskWorkerLifetime>
    {
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly IRepository<DM.Sensor, int?> _repositorySensor;
        private readonly IRepository<TaskParameters, int?> _repositoryTaskParameters;
        private readonly IController _controller;
        private readonly ILogger _logger;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly IGpsDevice _gpsDevice;

        public GPSWorker(IProcessingDispatcher processingDispatcher,
            IRepository<DM.Sensor, int?> repositorySensor,
            IRepository<TaskParameters, int?> repositoryTaskParameters,
            IController controller,
            ITaskStarter taskStarter,
            IGpsDevice gpsDevice,
            ITimeService timeService, ILogger logger)
        {
            this._controller = controller;
            this._logger = logger;
            this._repositorySensor = repositorySensor;
            this._timeService = timeService;
            this._processingDispatcher = processingDispatcher;
            this._repositoryTaskParameters = repositoryTaskParameters;
            this._taskStarter = taskStarter;
            this._gpsDevice = gpsDevice;
        }

        public void Run(ITaskContext<GPSTask, BaseContext> context)
        {
            try
            {
                _logger.Verbouse(Contexts.GPSWorker, Categories.Processing, Events.StartGPSWorker.With(context.Task.Id));
                this._gpsDevice.Run();
                //context.Finish();
            }
            catch (Exception e)
            {
                _logger.Error(Contexts.GPSWorker, Categories.Processing, Exceptions.UnknownErrorGPSWorker, e.Message);
                context.Abort(e);
            }
        }
    }
}
