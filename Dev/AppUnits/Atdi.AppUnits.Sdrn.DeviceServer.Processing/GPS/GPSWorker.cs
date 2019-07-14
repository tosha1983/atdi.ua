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
using Atdi.Contracts.Api.Sdrn.MessageBus;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    /// <summary>
    /// Воркер, выполняющий запуск GPS девайса
    /// </summary>
    public class GPSWorker : ITaskWorker<GPSTask, DispatchProcess, SingletonTaskWorkerLifetime>
    {
        private readonly ILogger _logger;
        private readonly ITimeService _timeService;
        private readonly IController _controller;
        private ConfigProcessing _configProcessing;
        private readonly IBusGate _busGate;
        private readonly IRepository<DM.Sensor, long?> _repositorySensor;

        public GPSWorker(
            ConfigProcessing configProcessing,
            IController controller,
            IBusGate busGate,
            IRepository<DM.Sensor, long?> repositorySensor,
            ITimeService timeService, ILogger logger)
        {
            this._logger = logger;
            this._timeService = timeService;
            this._controller = controller;
            this._busGate = busGate;
            this._configProcessing = configProcessing;
            this._repositorySensor = repositorySensor;
        }

        public void Run(ITaskContext<GPSTask, DispatchProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.GPSWorker, Categories.Processing, Events.StartGPSWorker.With(context.Task.Id));

                //////////////////////////////////////////////
                // 
                // Задаем координаты сенсора по умолчанию 
                //
                //////////////////////////////////////////////

                context.Process.Asl = this._configProcessing.AslDefault;
                context.Process.Lon = this._configProcessing.LonDelta;
                context.Process.Lat = this._configProcessing.LatDefault;

                //////////////////////////////////////////////
                // 
                // Отправка команды в контроллер GPS
                //
                //////////////////////////////////////////////


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
                    System.Threading.Thread.Sleep(this._configProcessing.PeriodSendCoordinatesToSDRNS);
                    var gpsParameter = new GpsParameter();
                    gpsParameter.GpsMode = GpsMode.Start;
                    var gpsDevice = new GpsCommand(gpsParameter);

                    this._controller.SendCommand<GpsResult>(context, gpsDevice,
                    (
                          ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                    ) =>
                    {
                        taskContext.SetEvent<ExceptionProcessGPS>(new ExceptionProcessGPS(failureReason, ex));
                    });

                    GpsResult gpsResult = null;
                    bool isWait = context.WaitEvent<GpsResult>(out gpsResult, this._configProcessing.DurationWaitingRceivingGPSCoord);
                    if (isWait)
                    {
                        context.Process.Asl = gpsResult.Asl.Value;
                        context.Process.Lon = gpsResult.Lon.Value;
                        context.Process.Lat = gpsResult.Lat.Value;

                        var sensors = this._repositorySensor.LoadAllObjects();
                        if ((sensors != null) && (sensors.Length > 0))
                        {
                            var sensorCurr = sensors[0];
                            var listSensorLocations = sensorCurr.Locations.ToList();
                            var lSensorLocations = listSensorLocations.FindAll(t => Math.Abs(t.Lon - context.Process.Lon) <= this._configProcessing.LonDelta && Math.Abs(t.Lat - context.Process.Lat) <= this._configProcessing.LatDelta && Math.Abs(t.ASL.Value - context.Process.Asl) <= this._configProcessing.AslDelta && t.Status != "Z");
                            if (lSensorLocations.Count == 0)
                            {
                                lSensorLocations.OrderByDescending(x => x.Created);
                                var mass = lSensorLocations.ToArray();
                                var sensorLocation = new DM.SensorLocation[mass.Length + 1];
                                if (mass.Length >= 1)
                                {
                                    for (int i = 0; i < mass.Length; i++)
                                    {
                                        sensorLocation[i] = sensorCurr.Locations[i];
                                        sensorLocation[i].Status = "Z";
                                    }
                                }

                                var location = new DM.SensorLocation()
                                {
                                    ASL = context.Process.Asl,
                                    Lon = context.Process.Lon,
                                    Lat = context.Process.Lat,
                                    Status = "A",
                                    Created = DateTime.Now,
                                    From = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1),
                                    To = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59)
                                };

                                sensorLocation[mass.Length] = location;
                                sensorCurr.Locations = sensorLocation;
                                this._repositorySensor.Update(sensorCurr);

                                DM.DeviceCommandResult deviceCommandResult = new DM.DeviceCommandResult();
                                deviceCommandResult.CommandId = "UpdateSensorLocation";
                                deviceCommandResult.CustDate1 = DateTime.Now;
                                deviceCommandResult.CustTxt1 = $"{location.Lon}|{location.Lat}|{location.ASL}";
                                deviceCommandResult.Status = "A";
                                deviceCommandResult.CustNbr1 = 0;

                                var publisher = this._busGate.CreatePublisher("main");
                                publisher.Send<DM.DeviceCommandResult>("SendCommandResult", deviceCommandResult);
                                publisher.Dispose();
                            }
                        }

                        _logger.Info(Contexts.GPSWorker, Categories.Processing, $" New coordinates Lon: {context.Process.Lon}, Lat: {context.Process.Lat}, Asl : {context.Process.Asl}");
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

