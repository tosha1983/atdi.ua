using System;


namespace Atdi.WcfServices.Sdrn.Server
{

    public class LevelMeasurementsCarForSO 
    {
        public DateTime? TimeOfMeasurements; // из class LevelMeasurementsCar
        public decimal? CentralFrequency; // из class LevelMeasurementsCar
        public double? BW; // из class LevelMeasurementsCar если значение <= 0, то вытаскивать из MeasurementsParameterGeneral причем = SpecrumSteps*(T2-T1);
        public long? Idstation; // из class ResultsMeasurementsStation
        public long? Id; // из class ResultsMeasurementsStation
        public string globalSid;
    }

}
