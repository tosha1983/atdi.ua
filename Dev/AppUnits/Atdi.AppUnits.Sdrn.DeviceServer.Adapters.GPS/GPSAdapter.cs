using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using COM = Atdi.DataModels.Sdrn.DeviceServer.Commands;
using COMR = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.Platform.DependencyInjection;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.GPS
{
    /// <summary>
    /// Реализация объекта GPS адаптера
    /// </summary>
    public class GPSAdapter : IAdapter
    {
        private readonly ILogger _logger;
        private readonly ConfigGPS _config;
        private GNSSReceiverWrapper gnssWrapper;
        private IExecutionContext _executionContextGps;
        private readonly IWorkScheduler _workScheduler;
        private ulong part = 0;
        /// <summary>
        /// Все объекты адаптера создаются через DI-контейнер 
        /// Запрашиваем через конструктор необходимые сервисы
        /// </summary>
        /// <param name="adapterConfig"></param>
        /// <param name="logger"></param>
        public GPSAdapter(ConfigGPS config,
            IWorkScheduler workScheduler,
            ILogger logger)
        {
            this._logger = logger;
            this._config = config;
            this._logger = logger;
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

        private void OpenGPSDevice()
        {
            this._logger.Verbouse(Contexts.ThisComponent, Categories.Processing, "Try open GPS device...");

            var portSettings = new SerialPortSettings();

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
            if (!gnssWrapper.IsOpen)
            {
                try
                {
                    gnssWrapper.Open();
                    this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.OpenDevice);
                }
                catch (Exception ex)
                {
                    this._logger.Critical(Contexts.ThisComponent, Categories.Processing, string.Format(Exceptions.UnknownError.ToString(), ex.Message), ex);
                    throw new InvalidOperationException(Categories.Processing+":"+ex.Message, ex);
                }
            }
        }

        private void gnssWrapper_LogEvent(object sender, LogEventArgs e)
        {
            try
            {
                if ((_executionContextGps != null) && (e!=null))
                {
                    var resultMember = new COMR.GpsResult(part++, CommandResultStatus.Final);
                    var data = e.LogString;
                    if (data != null)
                    {
                        var result = NMEAParser.Parse(data);
                        if (result != null)
                        {
                            if (result is NMEAStandartSentence)
                            {
                                var sentence = (result as NMEAStandartSentence);
                                if (sentence.SentenceID == SentenceIdentifiers.GGA)
                                {

                                    if (_executionContextGps.Token.IsCancellationRequested)
                                    {
                                        _executionContextGps.Cancel();
                                        CloseGPSDevice();
                                        return;
                                    }

                                    resultMember.Lat = (double)sentence.parameters[1];
                                    if ((string)sentence.parameters[2] == "S")
                                        resultMember.Lat = resultMember.Lat * (-1);
                                    resultMember.Lat = (double)Math.Round(resultMember.Lat.Value, 6);

                                    resultMember.Lon = (double)sentence.parameters[3];
                                    if ((string)sentence.parameters[4] == "W")
                                        resultMember.Lon = resultMember.Lon * (-1);
                                    resultMember.Lon = (double)Math.Round(resultMember.Lon.Value, 6);
                                    resultMember.Asl = (double)Math.Round((double)sentence.parameters[8], 2);

                                    _executionContextGps.PushResult(resultMember);

                                    // контекст не освобождаем, т.к. в GPSWorker ожидаем отправленные координаты с этого контекста
                                    //_executionContextGps.Finish();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                 this._logger.Error(Contexts.ThisComponent, Categories.Processing, string.Format(Exceptions.LogEventError.ToString(), ex.Message));
            }
        }

        void CloseGPSDevice()
        {
            if (_executionContextGps != null)
            {
                _executionContextGps.Finish();
            }

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
