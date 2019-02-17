using System;


namespace Atdi.WcfServices.Sdrn.Server
{

    public class LevelMeasurementsCarForSO 
    {
        public DateTime? TimeOfMeasurements; // из class LevelMeasurementsCar
        public decimal? CentralFrequency; // из class LevelMeasurementsCar
        public double? BW; // из class LevelMeasurementsCar если значение <= 0, то вытаскивать из MeasurementsParameterGeneral причем = SpecrumSteps*(T2-T1);
        public int? Idstation; // из class ResultsMeasurementsStation
        public int? Id; // из class ResultsMeasurementsStation
        public string globalSid;
    }

}
