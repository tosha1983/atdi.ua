using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Platform.Logging;
using System;

namespace Atdi.WcfServices.Sdrn.Device
{
    internal class BusEventObserver : IBusEventObserver
    {
        private readonly ILogger _logger;

        public BusEventObserver(ILogger logger)
        {
            this._logger = logger;
        }

        public void OnEvent(IBusEvent busEvent)
        {
            switch (busEvent.Level)
            {
                case BusEventLevel.None:
                    _logger.Verbouse(busEvent.Context, busEvent.Text);
                    break;
                case BusEventLevel.Verbouse:
                    _logger.Verbouse(busEvent.Context, busEvent.Text);
                    break;
                case BusEventLevel.Info:
                    _logger.Info(busEvent.Context, busEvent.Text);
                    break;
                case BusEventLevel.Warning:
                    _logger.Warning(busEvent.Context, busEvent.Text);
                    break;
                case BusEventLevel.Trace:
                    _logger.Debug(busEvent.Context, (EventText)busEvent.Text, busEvent.Source);
                    break;
                case BusEventLevel.Debug:
                    _logger.Debug(busEvent.Context, (EventText)busEvent.Text, busEvent.Source);
                    break;
                case BusEventLevel.Error:
                    _logger.Debug(busEvent.Context, (EventText)busEvent.Text, busEvent.Source);
                    break;
                case BusEventLevel.Exception:
                    _logger.Exception(busEvent.Context, new Exception(busEvent.Text), busEvent.Source);
                    break;
                case BusEventLevel.Critical:
                    _logger.Critical(busEvent.Context, new Exception(busEvent.Text), busEvent.Source);
                    break;
            }
        }
    }
}
