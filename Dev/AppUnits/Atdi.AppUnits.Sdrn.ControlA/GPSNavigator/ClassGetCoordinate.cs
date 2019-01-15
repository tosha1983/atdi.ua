using System;
using NMEA;
using Atdi.AppServer.Contracts.Sdrns;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppUnits.Sdrn.ControlA;
using System.Xml.Serialization;
using Atdi.Modules.Licensing;
using XMLLibrary;
using Atdi.AppUnits.Sdrn.ControlA.Handlers;

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
                //Lat = 1E-99; Lon = 1E-99; Asl = 1E-99;
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
                        SensorDBExtension svr = new SensorDBExtension();
                        // Если координаты заполнены, тогда включаем механизм записи и отправки координат в SDRNS.
                        if ((Lat != 1E-99) && (Lon != 1E-99) && (Asl != 1E-99))
                        {
                            SensorDBExtension sensorDBExtension = new SensorDBExtension();
                            Sensor se_curr = null;
                            XmlSerializer sersens = new XmlSerializer(typeof(Sensor));
                            var readersenss = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\sensor.xml");
                            object obj = sersens.Deserialize(readersenss);
                            if (obj != null)
                            {
                                se_curr = obj as Sensor;


                                var licenseDeviceFileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\License\" + ConfigurationManager.AppSettings["LicenseDevice.FileName"].ToString();
                                if (System.IO.File.Exists(licenseDeviceFileName))
                                {
                                    var productKey = Atdi.Platform.Cryptography.Encryptor.DecryptStringAES(ConfigurationManager.AppSettings["License.ProductKey"].ToString(), "Atdi.WcfServices.Sdrn.Device");
                                    var ownerId = Atdi.Platform.Cryptography.Encryptor.DecryptStringAES(ConfigurationManager.AppSettings["License.OwnerId"].ToString(), "Atdi.WcfServices.Sdrn.Device");
                                    var verificationData = new VerificationData
                                    {
                                        OwnerId = ownerId,
                                        ProductName = "ICS Control Device",
                                        ProductKey = productKey,
                                        LicenseType = "DeviceLicense",
                                        Date = DateTime.Now
                                    };
                                    var licenseBody = System.IO.File.ReadAllBytes(licenseDeviceFileName);
                                    var verResult = LicenseVerifier.Verify(verificationData, licenseBody);
                                    if (verResult != null)
                                    {
                                        if (!string.IsNullOrEmpty(verResult.Instance))
                                        {
                                            se_curr.Name = verResult.Instance;
                                            if (se_curr.Locations != null)
                                            {
                                                NH_Sensor sens_ = sensorDBExtension.LoadSensorFromDB(se_curr.Name, se_curr.Equipment.TechId);
                                                List<SensorLocation> L_sensor_locations = se_curr.Locations.ToList().FindAll(t => Math.Abs(t.Lon.GetValueOrDefault() - Lon) <= BaseXMLConfiguration.xml_conf._Lon_Delta && Math.Abs(t.Lat.GetValueOrDefault() - Lat) <= BaseXMLConfiguration.xml_conf._Lat_Delta && t.Status != AllStatusLocation.Z.ToString());
                                                if (L_sensor_locations.Count == 0)
                                                {
                                                    if (sens_ != null)
                                                    {
                                                        SensorDBExtension opt_DB = new SensorDBExtension();
                                                        List<NH_SensorLocation> NH_L = opt_DB.LoadSensorLocationsFromDB((se_curr).Id.Value);
                                                        if (NH_L != null)
                                                        {
                                                            if (NH_L.Count > 0)
                                                            {
                                                                //выставляем статус Z для координат, которіе были отправлены на SDRNS
                                                                foreach (NH_SensorLocation z_o in NH_L)
                                                                {
                                                                    if (se_curr.Locations != null)
                                                                    {
                                                                        if (se_curr.Locations.Count() > 0)
                                                                        {
                                                                            if (se_curr.Locations.ToList().FindAll(t => Math.Abs(t.Lon.GetValueOrDefault() - z_o.Lon.GetValueOrDefault()) <= BaseXMLConfiguration.xml_conf._Lon_Delta && Math.Abs(t.Lat.GetValueOrDefault() - z_o.Lat.GetValueOrDefault()) <= BaseXMLConfiguration.xml_conf._Lat_Delta && t.Status != AllStatusLocation.Z.ToString()) != null)
                                                                                opt_DB.CloseOldSensorLocation(z_o);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                sensorDBExtension.AddNewLocations(sens_.ID, Lon, Lat, Asl, DateTime.Now, DateTime.Now, AllStatusLocation.A);
                                                BusManager._messagePublisher.Send("UpdateSensorLocation", se_curr);
                                            }
                                        }
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { 
                return false;
            }
            return false;
        }
    }
}
