using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using Atdi.DataModels.EntityOrm;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.DependencyInjection;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    /// <summary>
    /// Воркер, выполняющий запуск GPS девайса
    /// </summary>
    public class GPSWorker : ITaskWorker<GPSTask, BaseContext, SingletonTaskWorkerLifetime>
    {
        private readonly ILogger _logger;
        private readonly ITimeService _timeService;
        private readonly IController _controller;
        private IServicesResolver _resolver;
        private IServicesContainer _servicesContainer;
        private ConfigProcessing  _configProcessing;
        

        public GPSWorker(
            ConfigProcessing configProcessing,
            IController controller,
            IServicesResolver resolver,
            IServicesContainer servicesContainer,
            ITimeService timeService, ILogger logger)
        {
            this._logger = logger;
            this._timeService = timeService;
            this._controller = controller;
            this._resolver = resolver;
            this._servicesContainer = servicesContainer;
            this._configProcessing = configProcessing;
        }

        public void Run(ITaskContext<GPSTask, BaseContext> context)
        {
            try
            {
                _logger.Verbouse(Contexts.GPSWorker, Categories.Processing, Events.StartGPSWorker.With(context.Task.Id));
                this._resolver = this._servicesContainer.GetResolver<IServicesResolver>();
                var baseContext = this._resolver.Resolve(typeof(MainProcess)) as MainProcess;

                //////////////////////////////////////////////
                // 
                // Отправка команды в контроллер GPS
                //
                //////////////////////////////////////////////
                var gpsParameter = new GpsParameter();
                gpsParameter.GpsMode = GpsMode.Start;
                var gpsDevice = new GpsCommand(gpsParameter)
                {
                    Options = CommandOption.PutInQueue
                };

                this._controller.SendCommand<GpsResult>(context, gpsDevice,
                (
                      ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                ) =>
                {
                      taskContext.SetEvent<ExceptionProcessGPS>(new ExceptionProcessGPS(failureReason, ex));
                });

                while (true)
                {
                    if (context.Token.IsCancellationRequested)
                    {
                        context.Cancel();
                        break;
                    }

                    //////////////////////////////////////////////
                    // 
                    // Получение очередного  результат 
                    //
                    //
                    //////////////////////////////////////////////
                    GpsResult gpsResult = null;
                    bool isWait = context.WaitEvent<GpsResult>(out gpsResult, this._configProcessing.DurationWaitingRceivingGPSCoord);
                    if (isWait)
                    {
                        baseContext.Asl = gpsResult.Asl.Value;
                        baseContext.Lon = gpsResult.Lon.Value;
                        baseContext.Lon = gpsResult.Lat.Value;
                        _logger.Info(Contexts.GPSWorker, Categories.Processing, $" New coordinates Lon: {baseContext.Lon}, Lat: {baseContext.Lat}, Asl : {baseContext.Asl}");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error(Contexts.GPSWorker, Categories.Processing, Exceptions.UnknownErrorGPSWorker, e.Message);
                context.Abort(e);
            }
        }
    }
}
