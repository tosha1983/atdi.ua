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

        public GPSWorker(
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
        }

        public void Run(ITaskContext<GPSTask, BaseContext> context)
        {
            try
            {
                _logger.Verbouse(Contexts.GPSWorker, Categories.Processing, Events.StartGPSWorker.With(context.Task.Id));
                this._resolver = this._servicesContainer.GetResolver<IServicesResolver>();
                var baseContext = this._resolver.Resolve(typeof(MainProcess)) as MainProcess;
                while (true)
                {
                    if (context.Token.IsCancellationRequested)
                    {
                        context.Cancel();
                        break;
                    }

                    //////////////////////////////////////////////
                    // 
                    // Отправка команды в контроллер (причем context уже содержит информацию о сообщение с шины RabbitMq)
                    //
                    //////////////////////////////////////////////
                    var gpsParameter = new GpsParameter();
                    gpsParameter.GpsMode = GpsMode.Run;
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
                    //////////////////////////////////////////////
                    // 
                    // Получение очередного  результат 
                    //
                    //
                    //////////////////////////////////////////////
                    GpsResult gpsResult = null;
                    bool isDown = context.WaitEvent<GpsResult>(out gpsResult, 2000);
                    if (isDown == false) // таймут - результатов нет
                    {
                        var error = new ExceptionProcessGPS();
                        if (context.WaitEvent<ExceptionProcessGPS>(out error, 1) == true)
                        {
                            // 
                            context.Cancel();
                            break;
                        }
                    }
                    else
                    {
                        baseContext.Asl = gpsResult.Asl.Value;
                        baseContext.Lon = gpsResult.Lon.Value;
                        baseContext.Lon = gpsResult.Lat.Value;
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
