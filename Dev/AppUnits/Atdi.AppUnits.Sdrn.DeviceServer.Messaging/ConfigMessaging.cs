using System;
using Atdi.Platform.AppComponent;
using System.Collections.Generic;



namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging
{
    public class ConfigMessaging
    {
        /// <summary>
        /// 
        /// </summary>
        public bool CompareTraceJustWithRefLevels { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool AutoDivisionEmitting { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool FiltrationTrace { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ComponentConfigProperty("DifferenceMaxMax.double")]
        public double DifferenceMaxMax { get; set; }
        /// <summary>
        ///   допустимое превышение реферативного уровня по умолчанию 10
        /// </summary>
        [ComponentConfigProperty("allowableExcess_dB.double")]
        public double allowableExcess_dB { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SignalizationNCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SignalizationNChenal { get; set; }

        /// <summary>
        /// проводить корреляционный анализ между излучениями по умолчанию лож
        /// </summary>
        public bool? CorrelationAnalize { get; set; }
        /// <summary>
        ///  коєфициент кореляции при котором обединяем излучения по умолчанию 0.7
        /// </summary>
        [ComponentConfigProperty("CorrelationFactor.double")]
        public double? CorrelationFactor { get; set; }
        /// <summary>
        ///  проверять совподение частоты сигнала с частотой канала по умолчанию лож
        /// </summary>
        public bool? CheckFreqChannel { get; set; }
        /// <summary>
        ///  true значит надо анализировать согласно существующим частото каналам  по умолчанию false
        /// </summary>
        public bool? AnalyzeByChannel { get; set; }
        /// <summary>
        ///  true значит надо выполнять вложенное вокфлоу по SysInfo по умолчанию false 
        /// </summary>
        public bool? AnalyzeSysInfoEmission { get; set; }
        /// <summary>
        ///  true значит надо выполнять вложенное вокфлоу по BW по умолчанию false 
        /// </summary>
        public bool? DetailedMeasurementsBWEmission { get; set; }
        /// <summary>
        ///  ожидаемій стандарт сигналов (GSM or LTE or UMTS) по умолчанию null
        /// </summary>
        public string Standard { get; set; }
        /// <summary>
        ///  уровень шума на входе приемника по умолчанию измерение автошума = -999 используется в  namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements строка 61 (в хендлере)
        /// </summary>
        [ComponentConfigProperty("triggerLevel_dBm_Hz.double")]
        public double? triggerLevel_dBm_Hz { get; set; }
        /// <summary>
        /// минимальное количество точек для нахождения излучения Необходимо для исключения случайных выбросов спектров по умолчанию 10
        /// </summary>
        public int? NumberPointForChangeExcess { get; set; }
        /// <summary>
        ///  характеризует размер окна для обработки и хранения спектра излучения по умолчанию 1.1
        /// </summary>
        [ComponentConfigProperty("windowBW.double")]
        public double? windowBW { get; set; }
        /// <summary>
        ///  используется для нахождения BW методом ndBDown по умолчанию 25
        /// </summary>
        [ComponentConfigProperty("DiffLevelForCalcBW.double")]
        public double? DiffLevelForCalcBW { get; set; }
        /// <summary>
        /// уровень ndBDown для поиска конца излучения метод используется если нельзя определить BW класическим методом по умолчанию 25
        /// </summary>
        [ComponentConfigProperty("nDbLevel_dB.double")]
        public double? nDbLevel_dB { get; set; }
        /// <summary>
        ///  количество точек которые игнорируются при поиске концов излучения метод используется если нельзя определить BW класическим методом по умолчанию 1
        /// </summary>
        public int? NumberIgnoredPoints { get; set; }
        /// <summary>
        /// минимально допустимое превышение уровня шума максимумом излучения метод используется если нельзя определить BW класическим методом по умолчанию 5
        /// </summary>
        [ComponentConfigProperty("MinExcessNoseLevel_dB.double")]
        public double? MinExcessNoseLevel_dB { get; set; }
        /// <summary>
        ///  время для группировки записей workTime по умолчанию 60
        /// </summary>
        public int? TimeBetweenWorkTimes_sec { get; set; }
        /// <summary>
        ///  принцип объединения спектра 0 - Best Emmiting (ClearWrite), 1 - MaxHold, 2 - Avarage по умолчанию 0
        /// </summary>
        public int? TypeJoinSpectrum { get; set; }
        /// <summary>
        /// определяет насколько процентов должно совпадать излучение если BW определен по умолчанию 70
        /// </summary>
        [ComponentConfigProperty("CrossingBWPercentageForGoodSignals.double")]
        public double? CrossingBWPercentageForGoodSignals { get; set; }
        /// <summary>
        ///  определяет насколько процентов должно совпадать излучение если BW не определен по умолчанию 40
        /// </summary>
        [ComponentConfigProperty("CrossingBWPercentageForBadSignals.double")]
        public double? CrossingBWPercentageForBadSignals { get; set; } 
        /// <summary>
        /// 
        /// </summary>
        [ComponentConfigProperty("PercentForCalcNoise.double")]
        public double PercentForCalcNoise { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ComponentConfigProperty("MaxFreqDeviation.double")]
        public double? MaxFreqDeviation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ComponentConfigProperty("CheckLevelChannel.double")]
        public bool? CheckLevelChannel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ComponentConfigProperty("MinPointForDetailBW.double")]
        public int? MinPointForDetailBW { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? CorrelationAdaptation { get; set; } // true означает что можно адаптировать коэфициент корреляции. Устанавливается когда первоначально коэфициент корреляции 0.99 и выше.
        /// <summary>
        /// 
        /// </summary>
        public int? MaxNumberEmitingOnFreq { get; set; } // брать из файла конфигурации.
        /// <summary>
        /// 
        /// </summary>
        [ComponentConfigProperty("MinCoeffCorrelation.double")]
        public double? MinCoeffCorrelation { get; set; } // брать из файла конфигурации.
        /// <summary>
        /// 
        /// </summary>
        public bool? UkraineNationalMonitoring { get; set; } // признак что делается все для Украины

        [ComponentConfigProperty("HealthJob.StartDelay")]
        public int? HealthJobStartDelay { get; set; }

        [ComponentConfigProperty("HealthJob.RepeatDelay")]
        public int? HealthJobRepeatDelay { get; set; }
	}
}
