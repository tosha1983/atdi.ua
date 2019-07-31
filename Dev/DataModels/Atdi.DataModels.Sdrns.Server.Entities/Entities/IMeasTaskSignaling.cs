using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IMeasTaskSignaling_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IMeasTaskSignaling: IMeasTaskSignaling_PK
    {
        bool? CompareTraceJustWithRefLevels { get; set; }
        bool? AutoDivisionEmitting { get; set; }
        double? DifferenceMaxMax { get; set; }
        bool? FiltrationTrace { get; set; }
        double? allowableExcess_dB { get; set; }
        int? SignalizationNCount { get; set; }
        int? SignalizationNChenal { get; set; }
        double? InterruptAllowableExcess_dB { get; set; }
        bool? CorrelationAnalize { get; set; }// проводить корреляционный анализ между излучениями по умолчанию лож
        bool? CheckFreqChannel { get; set; }// проверять совподение частоты сигнала с частотой канала по умолчанию лож
        bool? AnalyzeByChannel { get; set; } // true значит надо анализировать согласно существующим частото каналам  по умолчанию false
        bool? AnalyzeSysInfoEmission { get; set; } // true значит надо выполнять вложенное вокфлоу по SysInfo по умолчанию false 
        bool? DetailedMeasurementsBWEmission { get; set; } // true значит надо выполнять вложенное вокфлоу по BW по умолчанию false 
        double? CorrelationFactor { get; set; }// коєфициент кореляции при котором обединяем излучения по умолчанию 0.7
        string Standard { get; set; } // ожидаемій стандарт сигналов (GSM or LTE or UMTS) по умолчанию null
        double? TriggerLevel_dBm_Hz { get; set; } // уровень шума на входе приемника по умолчанию измерение автошума = -999 используется в  namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements строка 61 (в хендлере)
        int? NumberPointForChangeExcess { get; set; } //минимальное количество точек для нахождения излучения Необходимо для исключения случайных выбросов спектров по умолчанию 10
        double? WindowBW { get; set; } // характеризует размер окна для обработки и хранения спектра излучения по умолчанию 1.1
        double? DiffLevelForCalcBW { get; set; } // используется для нахождения BW методом ndBDown по умолчанию 25
        double? NDbLevel_dB { get; set; } //уровень ndBDown для поиска конца излучения метод используется если нельзя определить BW класическим методом по умолчанию 25
        int? NumberIgnoredPoints { get; set; } // количество точек которые игнорируются при поиске концов излучения метод используется если нельзя определить BW класическим методом по умолчанию 1
        double? MinExcessNoseLevel_dB { get; set; } // минимально допустимое превышение уровня шума максимумом излучения метод используется если нельзя определить BW класическим методом по умолчанию 5
        int? TimeBetweenWorkTimes_sec { get; set; } // время для группировки записей workTime по умолчанию 60
        int? TypeJoinSpectrum { get; set; } // принцип объединения спектра 0 - Best Emmiting (ClearWrite), 1 - MaxHold, 2 - Avarage по умолчанию 0
        double? CrossingBWPercentageForGoodSignals { get; set; } //определяет насколько процентов должно совпадать излучение если BW определен по умолчанию 70
        double? CrossingBWPercentageForBadSignals { get; set; } // определяет насколько процентов должно совпадать излучение если BW не определен по умолчанию 40
        IMeasTask MEAS_TASK { get; set; }
    }
}
