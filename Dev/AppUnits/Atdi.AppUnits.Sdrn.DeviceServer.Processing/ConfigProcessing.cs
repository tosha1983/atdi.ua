using System;
using Atdi.Platform.AppComponent;
using System.Collections.Generic;



namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public class ConfigProcessing 
    {
        /// <summary>
        /// Время в миллисекундах задержки потока обработки отложенных задач
        /// DeferredTaskWorker - время задержки потока 
        /// </summary>
        public int DurationWaitingEventWithTask { get; set; }

        /// <summary>
        /// Время в минутах, которое определяет какие задачи должны запускаться
        /// Если время до запуска задачи меньше времени MaxDurationBeforeStartTimeTask, то задача сразу запускается,
        /// Если время до запуска больше MaxDurationBeforeStartTimeTask, то задача попадает в очередь отложенных задач
        /// </summary>
        public int MaxDurationBeforeStartTimeTask { get; set; }

        /// <summary>
        /// Время в миллисекундах  задержки перед отправкой результата в шину
        /// </summary>
        public int DurationForSendResult { get; set; }

        /// <summary>
        /// Время в миллисекундах  ожидания сообщения типа SensorRegistrationResult в воркере RegisterSensorTaskWorker
        /// </summary>
        public int MaxTimeOutReceiveSensorRegistrationResult { get; set; }

        /// <summary>
        /// Время в миллисекундах  ожидания сообщения типа GpsResult в воркере GPSWorker
        /// </summary>
        public int DurationWaitingRceivingGPSCoord { get; set; }

        /// <summary>
        /// Время в миллисекундах  ожидания сообщения типа SpectrumOcupationResult в воркере SOTaskWorker
        /// </summary>
        public int maximumTimeForWaitingResultSO { get; set; }

        /// <summary>
        /// Специальный коэффициент для расчета времени приостановки потока, выполняющего измерение SO 
        /// ДЛя огибок типа CommandFailureReason.NotFoundConvertor, CommandFailureReason.NotFoundDevice
        /// </summary>
        public int SOKoeffWaitingDevice { get; set; }

        /// <summary>
        /// Погрешность для долготы
        /// </summary>
        [ComponentConfigProperty("LonDelta.double")]
        public double LonDelta { get; set; }

        /// <summary>
        /// Погрешность для широты
        /// </summary>
        [ComponentConfigProperty("LatDelta.double")]
        public double LatDelta { get; set; }
        /// <summary>
        /// Периодичность в миллисекундах  отправки сообщений о координатах сенсора в SDRNS
        /// </summary>
        public int PeriodSendCoordinatesToSDRNS { get; set; }

        /// <summary>
        /// Период в миллисекундах проверки наличия в БД новых тасков
        /// </summary>
        public int DurationWaitingCheckNewTasks { get; set; }

        /// <summary>
        /// период в миллисекундах временной приостановки потока выполнения задачи (для статуса MEAStASK.STATUS='F')
        /// </summary>
        public int SleepTimePeriodForWaitingStartingMeas_ms { get; set; }

        /// <summary>
        /// период в миллисекундах временной приостановки потока выполнения задачи отправки уведомления об активности сенсора
        /// </summary>
        public int SleepTimePeriodSendActivitySensor_ms { get; set; }

        /// <summary>
        /// период в миллисекундах временной приостановки потока выполняющего "перевод" таска со статуса F в А 
        /// </summary>
        public int SleepTimeForUpdateContextSOTask_ms { get; set; }

    }
}
