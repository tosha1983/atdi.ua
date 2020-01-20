using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using COM = Atdi.DataModels.Sdrn.DeviceServer.Commands;
using COMR = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Common;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.GPS
{
    /// <summary>
    /// Реализация объекта GPS адаптера
    /// </summary>
    public class GPSAdapter : IAdapter
    {
        private readonly ITimeService _timeService;
        private readonly ILogger _logger;
        private readonly ConfigGPS _config;
        private GNSSReceiverWrapper gnssWrapper;
        private IExecutionContext _executionContextGps;
        private readonly IWorkScheduler _workScheduler;
        private ulong part = 0;
        private COMR.GpsResult resultMember = null;
        /// <summary>
        /// Все объекты адаптера создаются через DI-контейнер 
        /// Запрашиваем через конструктор необходимые сервисы
        /// </summary>
        /// <param name="adapterConfig"></param>
        /// <param name="logger"></param>
        public GPSAdapter(ConfigGPS config,
            IWorkScheduler workScheduler,
            ITimeService timeService,
            ILogger logger)
        {
            this._logger = logger;
            this._config = config;
            this._logger = logger;
            this._timeService = timeService;
            this._workScheduler = workScheduler;
        }

        /// <summary>
        /// Метод будет вызван при инициализации потока воркера адаптера
        /// Адаптеру необходимо зарегестрировать свои обработчики комманд 
        /// </summary>
        /// <param name="host"></param>
        public void Connect(IAdapterHost host)
        {
            try
            {
                OpenGPSDevice();
                host.RegisterHandler<COM.GpsCommand, COMR.GpsResult>(GPSCommandHandler);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
        }

        /// <summary>
        /// Метод вызывается контрллером когда необходимо выгрузит адаптер с памяти
        /// </summary>
        public void Disconnect()
        {
            CloseGPSDevice();
        }


        public void GPSCommandHandler(COM.GpsCommand command, IExecutionContext context)
        {
            try
            {
                if (command.Parameter.GpsMode == COM.Parameters.GpsMode.Start)
                {
                    _executionContextGps = context;
                    resultMember = new COMR.GpsResult(part++, CommandResultStatus.Next);
                }
                else if (command.Parameter.GpsMode == COM.Parameters.GpsMode.Stop)
                {
                    CloseGPSDevice();
                }
            }
            catch (Exception e)
            {
                // желательно записать влог
                _logger.Exception(Contexts.ThisComponent, e);
                // этот вызов обязательный в случаи обрыва
                context.Abort(e);
                throw new InvalidOperationException(Categories.Processing + ":" + e.Message, e);
            }
        }

        private bool OpenGPSDevice()
        {
            bool isSuccess = false;

            this._logger.Verbouse(Contexts.ThisComponent, Categories.Processing, "Try open GPS device...");

            var portSettings = new SerialPortSettings();

            BaudRate baudRate;
            if (Enum.TryParse<BaudRate>(this._config.PortBaudRate, out baudRate))
            {
                portSettings.PortBaudRate = baudRate;
            }
            else
            {
                throw new InvalidOperationException(Categories.Processing + $": Value '{this._config.PortBaudRate}' {Events.FromConfigurationFileNotRecognized}");
            }

            DataBits dataBits;
            if (Enum.TryParse<DataBits>(this._config.PortDataBits, out dataBits))
            {
                portSettings.PortDataBits = dataBits;
            }
            else
            {
                throw new InvalidOperationException(Categories.Processing +  $": Value '{this._config.PortDataBits}' {Events.FromConfigurationFileNotRecognized}");
            }

            System.IO.Ports.Handshake handshake;
            if (Enum.TryParse<System.IO.Ports.Handshake>(this._config.PortHandshake, out handshake))
            {
                portSettings.PortHandshake = handshake;
            }
            else
            {
                throw new InvalidOperationException(Categories.Processing + $": Value '{this._config.PortHandshake}' {Events.FromConfigurationFileNotRecognized}");
            }

            if (!string.IsNullOrEmpty(this._config.PortName))
            {
                portSettings.PortName = this._config.PortName;
            }
            else
            {
                throw new InvalidOperationException(Categories.Processing + $": Value '{this._config.PortName}' {Events.FromConfigurationFileIsNullOrEmpty}");
            }

            System.IO.Ports.StopBits stopBits;
            if (Enum.TryParse<System.IO.Ports.StopBits>(this._config.PortStopBits, out stopBits))
            {
                portSettings.PortStopBits = stopBits;
            }
            else
            {
                throw new InvalidOperationException(Categories.Processing + $": Value '{this._config.PortStopBits}' {Events.FromConfigurationFileNotRecognized}");
            }

            System.IO.Ports.Parity parity;
            if (Enum.TryParse<System.IO.Ports.Parity>(this._config.PortParity, out parity))
            {
                portSettings.PortParity = parity;
            }
            else
            {
                throw new InvalidOperationException(Categories.Processing + $": Value '{this._config.PortParity}' {Events.FromConfigurationFileNotRecognized}");
            }

            gnssWrapper = new GNSSReceiverWrapper(portSettings);

            if (!gnssWrapper.IsOpen)
            {
                try
                {
                    gnssWrapper.Open();
                    if (gnssWrapper.IsOpen)
                    {
                        gnssWrapper.LogEvent += new EventHandler<LogEventArgs>(gnssWrapper_LogEvent);
                        isSuccess = true;
                        this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.OpenDevice);
                    }
                    else
                    {
                        throw new InvalidOperationException($"COM port {portSettings.PortName} is not open");
                    }
                }
                catch (Exception ex)
                {
                    CloseGPSDevice();
                    this._logger.Critical(Contexts.ThisComponent, Categories.Processing, string.Format(Exceptions.UnknownError.ToString(), ex.Message), ex);
                    throw new InvalidOperationException(Categories.Processing + ":" + ex.Message, ex);
                }
            }
            return isSuccess;
        }

        private void gnssWrapper_LogEvent(object sender, LogEventArgs e)
        {
            try
            {
                if ((_executionContextGps != null) && (e != null) && (resultMember != null))
                {
                    var data = e.LogString;
                    if (data != null)
                    {
                        if (!data.EndsWith("\r\n"))
                        {
                            data += "\r\n";
                        }
                        if (!data.StartsWith("$"))
                        {
                            data = data.Insert(0, "$");
                        }
                        NMEASentence result = null;
                        try
                        {
                            result = NMEAParser.Parse(data);
                        }
                        catch (Exception)
                        {
                            result = null;
                        }
                        if (result != null)
                        {
                            if (result is NMEAStandartSentence)
                            {
                                var sentence = (result as NMEAStandartSentence);
                                if (sentence.SentenceID == SentenceIdentifiers.GGA)
                                {
                                    resultMember.Lat = (double)sentence.parameters[1];
                                    if ((string)sentence.parameters[2] == "S")
                                        resultMember.Lat = resultMember.Lat * (-1);
                                    resultMember.Lat = (double)Math.Round(resultMember.Lat.Value, 6);

                                    resultMember.Lon = (double)sentence.parameters[3];
                                    if ((string)sentence.parameters[4] == "W")
                                        resultMember.Lon = resultMember.Lon * (-1);
                                    resultMember.Lon = (double)Math.Round(resultMember.Lon.Value, 6);
                                    resultMember.Asl = (double)Math.Round((double)sentence.parameters[8], 2);
                                }
                                if ((sentence.TalkerID == TalkerIdentifiers.GP) && (sentence.SentenceID == SentenceIdentifiers.RMC))
                                {
                                    if (sentence.parameters[2] != null)
                                    {
                                        resultMember.Lat = Convert.ToDouble(sentence.parameters[2]);
                                    }
                                    if (sentence.parameters[4] != null)
                                    {
                                        resultMember.Lon = Convert.ToDouble(sentence.parameters[4]);
                                    }

                                    if (this._config.EnabledPPS)
                                    {
                                        gnssWrapper.port.SetUTCTime(((DateTime)sentence.parameters[0]).Ticks);
                                        this._timeService.TimeCorrection = gnssWrapper.port.OffsetToAvged;
                                        var currValueTicks = WinAPITime.GetTimeStamp() + this._timeService.TimeCorrection;
                                        if ((currValueTicks > DateTime.MaxValue.Ticks) || (currValueTicks < DateTime.MinValue.Ticks))
                                        {
                                            this._logger.Error(Contexts.ThisComponent, Categories.Processing, string.Format($"Ticks must be between DateTime.MinValue.Ticks and DateTime.MaxValue.Ticks. Parameter name:  TimeCorrection = {currValueTicks}"));
                                            this._timeService.TimeCorrection = ((DateTime)sentence.parameters[0]).Ticks - WinAPITime.GetTimeStamp();
                                        }
                                        if (gnssWrapper.port.OffsetToAvged==0)
                                        {
                                            this._timeService.TimeCorrection = ((DateTime)sentence.parameters[0]).Ticks - WinAPITime.GetTimeStamp();
                                        }
                                    }
                                    else
                                    {
                                        this._timeService.TimeCorrection = ((DateTime)sentence.parameters[0]).Ticks - WinAPITime.GetTimeStamp();
                                    }
                                    resultMember.TimeCorrection = this._timeService.TimeCorrection;
                                }
                                if ((resultMember.Lon != null) && (resultMember.Lat != null) && (resultMember.Asl != null) && (resultMember.TimeCorrection != null))
                                {
                                    _executionContextGps.PushResult(resultMember);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.Exception(Contexts.ThisComponent, Categories.Processing, string.Format(Exceptions.LogEventError.ToString(), ex.Message), ex);
            }
        }

        void CloseGPSDevice()
        {
            if (gnssWrapper.IsOpen)
            {
                try
                {
                    gnssWrapper.Close();
                    this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.CloseDevice);
                }
                catch (Exception ex)
                {
                    this._logger.Critical(Contexts.ThisComponent, Categories.Processing, string.Format(Exceptions.UnknownError.ToString(), ex.Message));
                    throw new InvalidOperationException(Categories.Processing + ":" + ex.Message, ex);
                }
            }
        }
    }
}
