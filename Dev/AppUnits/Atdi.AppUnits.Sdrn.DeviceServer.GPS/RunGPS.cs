using System;
using Atdi.Platform.Logging;
using Atdi.Platform.DependencyInjection;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Contracts.Sdrn.DeviceServer.GPS;

namespace Atdi.AppUnits.Sdrn.DeviceServer.GPS
{
    public class RunGPS : IGpsDevice
    {
        private ConfigGPS _config;
        private GNSSReceiverWrapper gnssWrapper;
        private ILogger _logger;
        private IServicesResolver _resolver;
        private IServicesContainer _servicesContainer;
        public RunGPS(
            ConfigGPS config,
            ILogger logger,
            IServicesResolver resolver,
            IServicesContainer servicesContainer
            )
        {
            this._config = config;
            this._logger = logger;
            this._resolver = resolver;
            this._servicesContainer = servicesContainer;
        }


        void IGpsDevice.Run()
        {
            ////////////////////////////////////////////////////////////////////////
            // 
            //
            // получение с DI - контейнера экземпляра глобального процесса MainProcess
            //
            ////////////////////////////////////////////////////////////////////////
            this._resolver = this._servicesContainer.GetResolver<IServicesResolver>();
            var baseContext = this._resolver.Resolve(typeof(MainProcess)) as MainProcess;

            ////////////////////////////////////////////////
            //
            // здесь пока заглушка
            //
            //
            ////////////////////////////////////////////////
            baseContext.Asl = 120;
            baseContext.Lon = 30;
            baseContext.Lat = 50;


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
                var baseContext = this._resolver.Resolve(typeof(MainProcess)) as MainProcess;
                var data = e.LogString;
                var result = NMEAParser.Parse(data);
                if (result is NMEAStandartSentence)
                {
                    var sentence = (result as NMEAStandartSentence);
                    if (sentence.SentenceID == SentenceIdentifiers.GGA)
                    {
                        baseContext.Lat = (float)sentence.parameters[1];
                        if ((string)sentence.parameters[2] == "S")
                            baseContext.Lat = baseContext.Lat * (-1);
                        baseContext.Lat = (float)Math.Round(baseContext.Lat, 6);

                        baseContext.Lon = (float)sentence.parameters[3];
                        if ((string)sentence.parameters[4] == "W")
                            baseContext.Lon = baseContext.Lon * (-1);
                        baseContext.Lon = (float)Math.Round(baseContext.Lon, 6);
                        baseContext.Asl = (float)Math.Round((float)sentence.parameters[8], 2);
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(Contexts.ThisComponent, Categories.Processing, string.Format(Exceptions.LogEventError.ToString(), ex.Message));
            }
        }

        void IGpsDevice.Stop()
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
