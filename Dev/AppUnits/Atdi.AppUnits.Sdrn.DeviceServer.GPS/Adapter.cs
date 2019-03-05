using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using COM = Atdi.DataModels.Sdrn.DeviceServer.Commands;
using COMR = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.Platform.DependencyInjection;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;




namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.GPS
{
    /// <summary>
    /// Реализация объекта адаптера
    /// </summary>
    public class Adapter : IAdapter
    {
        private readonly ILogger _logger;
        private readonly ConfigGPS _config;
        private GNSSReceiverWrapper gnssWrapper;
        private IServicesResolver _resolver;
        private IServicesContainer _servicesContainer;
        private IExecutionContext _executionContextGps;
        private ulong _counter = 0;
        /// <summary>
        /// Все объекты адаптера создаются через DI-контейнер 
        /// Запрашиваем через конструктор необходимые сервисы
        /// </summary>
        /// <param name="adapterConfig"></param>
        /// <param name="logger"></param>
        public Adapter(ConfigGPS config,
            IServicesResolver resolver,
            IServicesContainer servicesContainer,
            ILogger logger)
        {
            this._logger = logger;
            this._config = config;
            this._logger = logger;
            this._resolver = resolver;
            this._servicesContainer = servicesContainer;
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
                Start();
                host.RegisterHandler<COM.GpsCommand, COMR.GpsResult>(MesureTraceCommandHandler);
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
            try
            {
                Stop();
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
        }


        public void MesureTraceCommandHandler(COM.GpsCommand command, IExecutionContext context)
        {
            try
            {
                if (command.Parameter.GpsMode == COM.Parameters.GpsMode.Run)
                {
                    _executionContextGps = context;
                }
            }
            catch (Exception e)
            {
                // желательно записать влог
                _logger.Exception(Contexts.ThisComponent, e);
                // этот вызов обязательный в случаи обрыва
                context.Abort(e);
                // дальше кода быть не должно, освобождаем поток
            }
        }

        private void Start()
        {
            SerialPortSettings portSettings = new SerialPortSettings();

            BaudRate baudRate;
            if (Enum.TryParse<BaudRate>(this._config.PortBaudRate, out baudRate))
            {
                portSettings.PortBaudRate = baudRate;
            }

            DataBits dataBits;
            if (Enum.TryParse<DataBits>(this._config.PortDataBits, out dataBits))
            {
                portSettings.PortDataBits = dataBits;
            }

            System.IO.Ports.Handshake handshake;
            if (Enum.TryParse<System.IO.Ports.Handshake>(this._config.PortHandshake, out handshake))
            {
                portSettings.PortHandshake = handshake;
            }

            portSettings.PortName = this._config.PortName;

            System.IO.Ports.StopBits stopBits;
            if (Enum.TryParse<System.IO.Ports.StopBits>(this._config.PortStopBits, out stopBits))
            {
                portSettings.PortStopBits = stopBits;
            }

            System.IO.Ports.Parity parity;
            if (Enum.TryParse<System.IO.Ports.Parity>(this._config.PortParity, out parity))
            {
                portSettings.PortParity = parity;
            }

            gnssWrapper = new GNSSReceiverWrapper(portSettings);
            gnssWrapper.LogEvent += new EventHandler<LogEventArgs>(gnssWrapper_LogEvent);
            if (gnssWrapper.IsOpen)
            {
                try
                {
                    gnssWrapper.Close();
                    this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.CloseDevice);
                }
                catch (Exception ex)
                {
                    this._logger.Error(Contexts.ThisComponent, Categories.Processing, string.Format(Exceptions.UnknownError.ToString(), ex.Message));
                }
            }
            else
            {
                try
                {
                    gnssWrapper.Open();
                    this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.OpenDevice);
                }
                catch (Exception ex)
                {
                    this._logger.Error(Contexts.ThisComponent, Categories.Processing, string.Format(Exceptions.UnknownError.ToString(), ex.Message));
                }
            }
        }

        private void gnssWrapper_LogEvent(object sender, LogEventArgs e)
        {
            try
            {
                if (_executionContextGps != null)
                {
                    var resultMember = new COMR.GpsResult(this._counter++, CommandResultStatus.Next);
                    var data = e.LogString;
                    var result = NMEAParser.Parse(data);
                    if (result is NMEAStandartSentence)
                    {
                        var sentence = (result as NMEAStandartSentence);
                        if (sentence.SentenceID == SentenceIdentifiers.GGA)
                        {
                            if (_executionContextGps.Token.IsCancellationRequested)
                            {
                                _executionContextGps.Cancel();
                                return;
                            }

                            resultMember.Lat = (float)sentence.parameters[1];
                            if ((string)sentence.parameters[2] == "S")
                                resultMember.Lat = resultMember.Lat * (-1);
                            resultMember.Lat = (float)Math.Round(resultMember.Lat.Value, 6);

                            resultMember.Lon = (float)sentence.parameters[3];
                            if ((string)sentence.parameters[4] == "W")
                                resultMember.Lon = resultMember.Lon * (-1);
                            resultMember.Lon = (float)Math.Round(resultMember.Lon.Value, 6);
                            resultMember.Asl = (float)Math.Round((float)sentence.parameters[8], 2);

                            _executionContextGps.PushResult(resultMember);

                            _executionContextGps.Finish();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                 this._logger.Error(Contexts.ThisComponent, Categories.Processing, string.Format(Exceptions.LogEventError.ToString(), ex.Message));
            }
        }

        void Stop()
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
                    this._logger.Error(Contexts.ThisComponent, Categories.Processing, string.Format(Exceptions.UnknownError.ToString(), ex.Message));
                }
            }
        }
    }
}
