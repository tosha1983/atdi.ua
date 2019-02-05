using System;
using NMEA;
using System.Linq;
using Atdi.AppUnits.Sdrn.ControlA.Bus;
using Atdi.AppUnits.Sdrn.ControlA.ManageDB;
using Atdi.AppUnits.Sdrn.ControlA;

namespace GPS
{
    public static class ClassGetCoordinate
    {
        public static double Lat = 1E-99;
        public static double Lon = 1E-99;
        public static double Asl = 1E-99;

        public static bool TryGetLocation(string data)
        {
            try
            {
                var result = NMEAParser.Parse(data);
                if (result is NMEAStandartSentence)
                {
                    var sentence = (result as NMEAStandartSentence);
                    if (sentence.SentenceID == SentenceIdentifiers.GGA)
                    {
                        Lat = (double)sentence.parameters[1];
                        if ((string)sentence.parameters[2] == "S")
                            Lat = Lat * (-1);
                        Lat = Math.Round(Lat, 6);

                        Lon = (double)sentence.parameters[3];
                        if ((string)sentence.parameters[4] == "W")
                            Lon = Lon * (-1);
                        Lon = Math.Round(Lon, 6);
                        Asl = Math.Round((double)sentence.parameters[8], 2);
                        // Если координаты заполнены, тогда включаем механизм записи и отправки координат в SDRNS.
                        if ((Lat != 1E-99) && (Lon != 1E-99) && (Asl != 1E-99))
                        {
                            var sensorDBExtension = new SensorDb();
                            var sensorCurr = sensorDBExtension.GetCurrentSensor();
                            var sensor = sensorDBExtension.LoadSensorFromDB(sensorCurr.Name, sensorCurr.Equipment.TechId);
                            var lSensorLocations = sensorCurr.Locations.ToList().FindAll(t => Math.Abs(t.Lon.GetValueOrDefault() - Lon) <= ConfigParameters.LonDelta && Math.Abs(t.Lat.GetValueOrDefault() - Lat) <= ConfigParameters.LatDelta && t.Status != AllStatusLocation.Z.ToString());
                            if (lSensorLocations.Count == 0)
                            {
                                if (sensor != null)
                                {
                                    sensorDBExtension.AddNewLocations(sensor.ID, Lon, Lat, Asl, DateTime.Now, DateTime.Now, AllStatusLocation.A);
                                    sensorCurr = sensorDBExtension.GetCurrentSensor();
                                    Launcher._messagePublisher.Send("UpdateSensorLocation", sensorCurr);
                                }
                            }
                           
                            var nHL = sensorDBExtension.LoadSensorLocationsFromDB((sensorCurr).Id.Value);
                            if (nHL != null)
                            {
                                if (nHL.Count > 0)
                                {
                                    //выставляем статус Z для координат, которіе были отправлены на SDRNS
                                    foreach (NH_SensorLocation z_o in nHL)
                                    {
                                        if (sensorCurr.Locations != null)
                                        {
                                            if (sensorCurr.Locations.Count() > 0)
                                            {
                                                if (sensorCurr.Locations.ToList().FindAll(t => Math.Abs(t.Lon.GetValueOrDefault() - z_o.Lon.GetValueOrDefault()) <= ConfigParameters.LonDelta && Math.Abs(t.Lat.GetValueOrDefault() - z_o.Lat.GetValueOrDefault()) <= ConfigParameters.LatDelta && t.Status != AllStatusLocation.Z.ToString()) != null)
                                                    sensorDBExtension.CloseOldSensorLocation(z_o);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return true;
                    }
                }
            }
            catch (Exception)
            { 
                return false;
            }
            return false;
        }
    }
}
