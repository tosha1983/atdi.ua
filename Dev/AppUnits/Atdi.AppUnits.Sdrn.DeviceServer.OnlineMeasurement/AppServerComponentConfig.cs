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

        /// <summary>
        /// Максимально допустимое число точек для массивов Level, Freq
        /// </summary>
        public int? MaxCountPoint { get; set; }


        /// <summary>
        /// Максимальное время ожидания при получении результата
        /// </summary>
        public int maximumDurationMeasLevel_ms { get; set; }


        /// <summary>
        /// Минимально допустимое время,  которое отводится на  получение и отправку одного результата
        /// </summary>
        public int minimumTimeDurationLevel_ms { get; set; }


        [ComponentConfigProperty("Measurement.DebugMode")]
        public bool? MeasurementDebugMode { get; set; }

        /// <summary>
        /// Максимальное число попыток отправки команды на сенсор, когда он занят (DeviceIsBusy)
        /// </summary>
        public int CountLoopDeviceIsBusy { get; set; }

        /// <summary>
        /// Максимальное число попыток отправки команды на сенсор, когда возникает ошибка TimeoutExpired
        /// </summary>
        public int CountLoopTimeoutExpired { get; set; }


    }
}
