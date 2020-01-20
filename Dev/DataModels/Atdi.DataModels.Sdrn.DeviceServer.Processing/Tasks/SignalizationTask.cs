using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class SignalizationTask : MeasurementTaskBase
    {
        public MeasResults MeasResults; //  результат измерения
        public DateTime? LastTimeSend = null;
        public int CounterCallSignaling = 0;
        public MesureTraceDeviceProperties mesureTraceDeviceProperties;
        public ReferenceLevels ReferenceLevels;
        public TaskParameters[] taskParametersForBW;

        //сырой поток излучений после трейса
        public Emitting[] EmittingsRaw;
        // обработанные излучения которые нелзя выдавать в результаты
        public Emitting[] EmittingsTemp;
        // резулььтаты измерений излучений сделанные отдельно и детально
        public Emitting[] EmittingsDetailed;
        //результирующее излучание которое пойдет в результат
        public Emitting[] EmittingsSummary;
        public double NoiseLevel_dBm = -100; //константа пока
        public long maximumTimeForWaitingResultSignalization; // (максимальное время ожидания результата)
        public Func<TaskParameters, MesureTraceParameter> actionConvertBW = null;
        public Func<TaskParameters, MesureSystemInfoParameter[]> actionConvertSysInfo = null;
        public long durationForSendResultBandWidth;  // в миллисекундах из файла конфигурации - время в течении которого мы посылаем один результат (10 мин по умолчанию)
        public long durationForMeasBW_ms; // в миллисекундах из файла конфигурации - максимальное время, выделяемое для выполнения измерения BW
        public long durationForMeasSysInfo_ms; // в миллисекундах из файла конфигурации - максимальное время, выделяемое для выполнения измерения SysInfo
        public long durationForSendResultSysInfo;
        public long durationForSendResultSignaling;  // в миллисекундах из файла конфигурации - время в течении которого мы посылаем один результат (10 мин по умолчанию)
        public float[] templevel; // временный массив

    }
   
}
