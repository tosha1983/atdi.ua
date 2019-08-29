using System;
using Atdi.Platform.AppComponent;
using System.Collections.Generic;



namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement
{
    public class AppServerComponentConfig
    {

        /// <summary>
        /// URL конечной точки вебсокета который сервер будет открывать 
        /// для клиента для организаици онлайн измерений.
        /// Этот URL будет передан клиенту для подключения
        /// </summary>
        [ComponentConfigProperty("WebSocket.PublicUrl")]
        public string WebSocketPublicUrl { get; set; }

        /// <summary>
        /// Локальный порт вебксокета, на котором сервер устройств ожидает пакеты 
        /// согласно The WebSocket Protocol RFC6455
        /// </summary>
        [ComponentConfigProperty("WebSocket.LocalPort")]
        public int WebSocketLocalPort { get; set; }

        [ComponentConfigProperty("WebSocket.BufferSize")]
        public int? WebSocketBufferSize { get; set; }

    }
}
