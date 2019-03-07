using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using System.Linq;
using Atdi.DataModels.EntityOrm;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.DependencyInjection;
using Atdi.Contracts.Api.Sdrn.MessageBus;

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
        private readonly IBusGate _busGate;
        private readonly IRepository<DM.Sensor, int?> _repositorySensor;

        public GPSWorker(
            ConfigProcessing configProcessing,
            IController controller,
            IServicesResolver resolver,
            IBusGate busGate,
            IServicesContainer servicesContainer,
            IRepository<DM.Sensor, int?> repositorySensor,
            ITimeService timeService, ILogger logger)
        {
            this._logger = logger;
            this._timeService = timeService;
            this._controller = controller;
            this._resolver = resolver;
            this._busGate = busGate;
            this._servicesContainer = servicesContainer;
            this._configProcessing = configProcessing;
            this._repositorySensor = repositorySensor;
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
                        baseContext.Lat = gpsResult.Lat.Value;

                        var sensors = this._repositorySensor.LoadAllObjects();
                        if ((sensors != null) && (sensors.Length > 0))
                        {
                            var sensorCurr = sensors[0];
                            var listSensorLocations = sensorCurr.Locations.ToList();
                            var lSensorLocations = listSensorLocations.FindAll(t => Math.Abs(t.Lon - baseContext.Lon) <= this._configProcessing.LonDelta && Math.Abs(t.Lat - baseContext.Lat) <= this._configProcessing.LatDelta && t.Status != "Z");
                            if (lSensorLocations.Count > 0)
                            {
                                lSensorLocations.OrderByDescending(x => x.Created);
                                var sensorLocation = new DM.SensorLocation[lSensorLocations.Count + 1];
                                if (lSensorLocations.Count > 1)
                                {
                                    for (int i = 0; i < lSensorLocations.Count; i++)
                                    {
                                        sensorLocation[i] = sensorCurr.Locations[i];
                                        if (lSensorLocations.FindAll(t => Math.Abs(t.Lon - lSensorLocations[i].Lon) <= this._configProcessing.LonDelta && Math.Abs(t.Lat - lSensorLocations[i].Lat) <= this._configProcessing.LatDelta && t.Status != "Z") != null)
                                        {
                                            sensorLocation[i].Status = "Z";
                                        }
                                    }
                                }

                                var location = new DM.SensorLocation()
                                {
                                    ASL = baseContext.Asl,
                                    Lon = baseContext.Lon,
                                    Lat = baseContext.Lat,
                                    Status = "A",
                                    Created = DateTime.Now
                                };

                                sensorLocation[lSensorLocations.Count] = location;
                                this._repositorySensor.Update(sensorCurr);

                                DM.DeviceCommandResult deviceCommandResult = new DM.DeviceCommandResult();
                                deviceCommandResult.CommandId = "UpdateSensorLocation";
                                deviceCommandResult.CustDate1 = DateTime.Now;
                                deviceCommandResult.CustTxt1 = $"{location.Lon}|{location.Lat}|{location.ASL}";

                                var publisher = this._busGate.CreatePublisher("main");
                                publisher.Send<DM.DeviceCommandResult>("UpdateSensorLocation", deviceCommandResult);
                                publisher.Dispose();
                            }
                        }

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

