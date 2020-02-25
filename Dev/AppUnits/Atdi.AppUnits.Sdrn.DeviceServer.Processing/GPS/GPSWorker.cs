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
using System.Collections.Generic;


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
        private readonly DM.Sensor _sensor;
        private readonly IRepository<DM.DeviceCommandResult, string> _repositoryDeviceCommandResult;


        public GPSWorker(
            ConfigProcessing configProcessing,
            IController controller,
            IBusGate busGate,
            IRepository<DM.DeviceCommandResult, string> repositoryDeviceCommandResult,
            ITimeService timeService, ILogger logger)
        {
            this._repositoryDeviceCommandResult = repositoryDeviceCommandResult;
            this._logger = logger;
            this._timeService = timeService;
            this._controller = controller;
            this._busGate = busGate;
            this._configProcessing = configProcessing;
            _sensor = new DM.Sensor();
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
                context.Process.Lon = this._configProcessing.LonDefault;
                context.Process.Lat = this._configProcessing.LatDefault;

                if (this._configProcessing.EnableGPS == true)
                {
                    //////////////////////////////////////////////
                    // 
                    // Отправка команды в контроллер GPS
                    //
                    //////////////////////////////////////////////
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
                        if ((context.Task.Lon != null) && (context.Task.Asl != null) && (context.Task.Lat != null))
                        {

                            if (_sensor != null)
                            {
                                if (_sensor.Locations == null)
                                {
                                    _sensor.Locations = new DM.SensorLocation[1];
                                    _sensor.Locations[0] = new DM.SensorLocation()
                                    {
                                        Status = StatusTask.A.ToString(),
                                        Created = DateTime.Now,
                                        From = DateTime.Now,
                                        To = DateTime.Now
                                    };
                                }

                                var listSensorLocations = _sensor.Locations.ToList();
                                var lSensorLocations = listSensorLocations.FindAll(t => Math.Abs(t.Lon - context.Process.Lon) <= this._configProcessing.LonDelta && Math.Abs(t.Lat - context.Process.Lat) <= this._configProcessing.LatDelta && Math.Abs(t.ASL.Value - context.Process.Asl) <= this._configProcessing.AslDelta && t.Status != StatusTask.Z.ToString());
                                if (lSensorLocations.Count == 0)
                                {
                                    lSensorLocations.OrderByDescending(x => x.Created);
                                    var mass = lSensorLocations.ToArray();
                                    var sensorLocation = new DM.SensorLocation[mass.Length + 1];
                                    if (mass.Length >= 1)
                                    {
                                        for (int i = 0; i < mass.Length; i++)
                                        {
                                            sensorLocation[i] = _sensor.Locations[i];
                                            sensorLocation[i].Status = StatusTask.Z.ToString();
                                        }
                                    }

                                    var location = new DM.SensorLocation()
                                    {
                                        ASL = context.Process.Asl,
                                        Lon = context.Process.Lon,
                                        Lat = context.Process.Lat,
                                        Status = "A",
                                        Created = DateTime.Now,
                                        From = DateTime.Now,
                                        To = DateTime.Now
                                    };

                                    sensorLocation[mass.Length] = location;
                                    _sensor.Locations = sensorLocation;


                                    DM.DeviceCommandResult deviceCommandResult = new DM.DeviceCommandResult();
                                    deviceCommandResult.CommandId = "UpdateSensorLocation";
                                    deviceCommandResult.CustDate1 = DateTime.Now;
                                    deviceCommandResult.CustTxt1 = $"{location.Lon}|{location.Lat}|{location.ASL}";
                                    deviceCommandResult.Status = StatusTask.A.ToString();
                                    deviceCommandResult.CustNbr1 = 0;

                                    this._repositoryDeviceCommandResult.Create(deviceCommandResult);
                                }
                            }
                            _logger.Info(Contexts.GPSWorker, Categories.Processing, $" New coordinates Lon: {context.Process.Lon}, Lat: {context.Process.Lat}, Asl : {context.Process.Asl}");
                        }
                    }
                }
                else
                {
                    context.Finish();
                }
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.GPSWorker, Categories.Processing, Exceptions.UnknownErrorGPSWorker, e);
                context.Abort(e);
            }
        }
    }
}

