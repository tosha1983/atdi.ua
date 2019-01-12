using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Atdi.AppServer.Contracts.Sdrns;


namespace XMLLibrary
{
    public struct XMLConfiguration
    {
        public string _MainRabbitMQServices;
        public string _Lon_Delta;
        public string _Lat_Delta;
        public string _TimeExpirationTask;
        public string _TimeUpdateSensorLocation;
        public string _ReloadStart;
        public string _ScanDataSensor;
        public string _ClassSensorSubmitToDB;
        public string _ScanMeasTasks;
        public string _ShedulerSensorSubmitLstQueues;
        public string _DefaultValueMinTimeInterval;
        public string _MEAS_SDR_PARAM_Time_of_m;
        public string _MEAS_SDR_PARAM_RBW;
        public string _MEAS_SDR_PARAM_VBW;
        public string _MEAS_SDR_PARAM_ref_level_dbm;
        public string _MEAS_TypeFunction;
        public string _MEAS_Type_of_m;
        public string _MEAS_sw_time;
        public string _TimerSendMeaskTaskToSDR;
        public string _ResendingSensorToSDRNS;
        public string _CheckActivitySensor;
        public string _TimeUpdateMeasTaskStatus;
        public string _TimerSendMeasSDRResults;
        public string _MaxTimeOnlineCalculation;
        public string _TimeArchiveResult;
        public string _TimeExpirationTemp;
        

    }

    public struct XMLConfType
    {
        public string _MainRabbitMQServices;
        public double _Lon_Delta;
        public double _Lat_Delta;
        public int _TimeExpirationTask;
        public int _TimeExpirationTemp;
        public int _ReloadStart;
        public int _ScanDataSensor;
        public int _ClassSensorSubmitToDB;
        public int _ScanMeasTasks;
        public int _ShedulerSensorSubmitLstQueues;
        public int _DefaultValueMinTimeInterval;
        public double _MEAS_SDR_PARAM_Time_of_m;
        public double _MEAS_SDR_PARAM_RBW;
        public double _MEAS_SDR_PARAM_VBW;
        public double _MEAS_SDR_PARAM_ref_level_dbm;
        public string _MEAS_TypeFunction;
        public string _MEAS_Type_of_m;
        public int _MEAS_sw_time;
        public int _TimerSendMeaskTaskToSDR;
        public int _TimeUpdateSensorLocation;
        public int _ResendingSensorToSDRNS;
        public int _CheckActivitySensor;
        public int _TimeUpdateMeasTaskStatus;
        public int _TimerSendMeasSDRResults;
        public int _MaxTimeOnlineCalculation;
        public int _TimeArchiveResult;
    }

    public class BaseXMLConfigurationSensor
    {
        public static Sensor GetXmlSettingsSensor(string FileName)
        {
            Sensor obj = new Sensor();
            try {
                XmlSerializer ser_senss = new XmlSerializer(typeof(Sensor));
                var reader_senss = new System.IO.StreamReader(FileName);
                object obj_x = ser_senss.Deserialize(reader_senss);
                if (obj_x != null) obj = (Sensor)obj_x;
            }
            catch (Exception ex) { }
            return obj;
        }
    }

        /// <summary>
        /// Класс для извлечения конфигурационных данных из XML - файла
        /// </summary>
    public class BaseXMLConfiguration
    {
        public static string file_name_current = AppDomain.CurrentDomain.BaseDirectory + @"\ServerSetting.xml";
        public XMLConfiguration xml_configuration = new XMLConfiguration();
        public static XMLConfType xml_conf = new XMLConfType();

        public BaseXMLConfiguration()
        {
            string decimal_sep = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            xml_configuration = GetXmlSettings(file_name_current);
            xml_conf._MainRabbitMQServices = xml_configuration._MainRabbitMQServices;
            double.TryParse(xml_configuration._Lon_Delta.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._Lon_Delta);
            double.TryParse(xml_configuration._Lat_Delta.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._Lat_Delta);
            int.TryParse(xml_configuration._TimeExpirationTask.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._TimeExpirationTask);
            int.TryParse(xml_configuration._TimeExpirationTemp.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._TimeExpirationTemp);
            int.TryParse(xml_configuration._TimeUpdateSensorLocation.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._TimeUpdateSensorLocation);
            int.TryParse(xml_configuration._ReloadStart.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._ReloadStart);
            double.TryParse(xml_configuration._MEAS_SDR_PARAM_RBW.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._MEAS_SDR_PARAM_RBW);
            double.TryParse(xml_configuration._MEAS_SDR_PARAM_VBW.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._MEAS_SDR_PARAM_VBW);
            double.TryParse(xml_configuration._MEAS_SDR_PARAM_ref_level_dbm.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._MEAS_SDR_PARAM_ref_level_dbm);
            double.TryParse(xml_configuration._MEAS_SDR_PARAM_Time_of_m.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._MEAS_SDR_PARAM_Time_of_m);
            xml_conf._MEAS_Type_of_m = xml_configuration._MEAS_Type_of_m;
            xml_conf._MEAS_TypeFunction = xml_configuration._MEAS_TypeFunction;
            int.TryParse(xml_configuration._MEAS_sw_time.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._MEAS_sw_time);
            int.TryParse(xml_configuration._CheckActivitySensor.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._CheckActivitySensor);
            int.TryParse(xml_configuration._TimerSendMeasSDRResults.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._TimerSendMeasSDRResults);
            int.TryParse(xml_configuration._MaxTimeOnlineCalculation.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._MaxTimeOnlineCalculation);
            int.TryParse(xml_configuration._TimeUpdateMeasTaskStatus.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._TimeUpdateMeasTaskStatus);
        }
        public static XMLConfiguration GetXmlSettings(string FileName)
        {
            string XmlSetting = "XmlSetting";
            lock (XmlSetting)
            {
                XMLConfiguration obj = new XMLConfiguration();
                XmlDocument xmlDoc = new XmlDocument();
                StreamReader xml = new StreamReader(FileName);
                xmlDoc.Load(new StringReader(xml.ReadToEnd()));
                XmlElement root = xmlDoc.DocumentElement;
                XmlNodeList NameClientLibrary = xmlDoc.GetElementsByTagName("MainRabbitMQServices");
                if (NameClientLibrary.Count > 0)
                {
                    XmlNode el = NameClientLibrary[0];
                    obj._MainRabbitMQServices = el.InnerText;
                }
                XmlNodeList ConnectionString = xmlDoc.GetElementsByTagName("Lon_Delta");
                if (ConnectionString.Count > 0)
                {
                    XmlNode el = ConnectionString[0];
                    obj._Lon_Delta = el.InnerText;
                }
                XmlNodeList OleConnectionString = xmlDoc.GetElementsByTagName("Lat_Delta");
                if (OleConnectionString.Count > 0)
                {
                    XmlNode el = OleConnectionString[0];
                    obj._Lat_Delta = el.InnerText;
                }
                XmlNodeList TypeRDBMS = xmlDoc.GetElementsByTagName("TimeExpirationTask");
                if (TypeRDBMS.Count > 0)
                {
                    XmlNode el = TypeRDBMS[0];
                    obj._TimeExpirationTask = el.InnerText;
                }

                XmlNodeList TimeUpdateSensorLocation = xmlDoc.GetElementsByTagName("TimeUpdateSensorLocation");
                if (TimeUpdateSensorLocation.Count > 0)
                {
                    XmlNode el = TimeUpdateSensorLocation[0];
                    obj._TimeUpdateSensorLocation = el.InnerText;
                }

                XmlNodeList LocationOrmSchema = xmlDoc.GetElementsByTagName("ReloadStart");
                if (LocationOrmSchema.Count > 0)
                {
                    XmlNode el = LocationOrmSchema[0];
                    obj._ReloadStart = el.InnerText;
                }
                XmlNodeList LocationCurrentConfFile = xmlDoc.GetElementsByTagName("ScanDataSensor");
                if (LocationCurrentConfFile.Count > 0)
                {
                    XmlNode el = LocationCurrentConfFile[0];
                    obj._ScanDataSensor = el.InnerText;
                }
                XmlNodeList LocationDefaultConfFile = xmlDoc.GetElementsByTagName("ClassSensorSubmitToDB");
                if (LocationDefaultConfFile.Count > 0)
                {
                    XmlNode el = LocationDefaultConfFile[0];
                    obj._ClassSensorSubmitToDB = el.InnerText;
                }
                XmlNodeList LocationLogFile = xmlDoc.GetElementsByTagName("ScanMeasTasks");
                if (LocationLogFile.Count > 0)
                {
                    XmlNode el = LocationLogFile[0];
                    obj._ScanMeasTasks = el.InnerText;
                }
                XmlNodeList NamePlugin = xmlDoc.GetElementsByTagName("ShedulerSensorSubmitLstQueues");
                if (NamePlugin.Count > 0)
                {
                    XmlNode el = NamePlugin[0];
                    obj._ShedulerSensorSubmitLstQueues = el.InnerText;
                }
                XmlNodeList LocationPlugin = xmlDoc.GetElementsByTagName("DefaultValueMinTimeInterval");
                if (LocationPlugin.Count > 0)
                {
                    XmlNode el = LocationPlugin[0];
                    obj._DefaultValueMinTimeInterval = el.InnerText;
                }

                XmlNodeList MEAS_SDR_PARAM_Time_of_m = xmlDoc.GetElementsByTagName("MEAS_SDR_PARAM_Time_of_m");
                if (MEAS_SDR_PARAM_Time_of_m.Count > 0)
                {
                    XmlNode el = MEAS_SDR_PARAM_Time_of_m[0];
                    obj._MEAS_SDR_PARAM_Time_of_m = el.InnerText;
                }

                XmlNodeList MEAS_SDR_PARAM_RBW = xmlDoc.GetElementsByTagName("MEAS_SDR_PARAM_RBW");
                if (MEAS_SDR_PARAM_RBW.Count > 0)
                {
                    XmlNode el = MEAS_SDR_PARAM_RBW[0];
                    obj._MEAS_SDR_PARAM_RBW = el.InnerText;
                }

                XmlNodeList MEAS_SDR_PARAM_VBW = xmlDoc.GetElementsByTagName("MEAS_SDR_PARAM_VBW");
                if (MEAS_SDR_PARAM_VBW.Count > 0)
                {
                    XmlNode el = MEAS_SDR_PARAM_VBW[0];
                    obj._MEAS_SDR_PARAM_VBW = el.InnerText;
                }

                XmlNodeList MEAS_SDR_PARAM_ref_level_dbm = xmlDoc.GetElementsByTagName("MEAS_SDR_PARAM_ref_level_dbm");
                if (MEAS_SDR_PARAM_ref_level_dbm.Count > 0)
                {
                    XmlNode el = MEAS_SDR_PARAM_ref_level_dbm[0];
                    obj._MEAS_SDR_PARAM_ref_level_dbm = el.InnerText;
                }

                XmlNodeList MEAS_TypeFunction = xmlDoc.GetElementsByTagName("MEAS_TypeFunction");
                if (MEAS_TypeFunction.Count > 0)
                {
                    XmlNode el = MEAS_TypeFunction[0];
                    obj._MEAS_TypeFunction = el.InnerText;
                }

                XmlNodeList MEAS_Type_of_m = xmlDoc.GetElementsByTagName("MEAS_Type_of_m");
                if (MEAS_Type_of_m.Count > 0)
                {
                    XmlNode el = MEAS_Type_of_m[0];
                    obj._MEAS_Type_of_m = el.InnerText;
                }

                XmlNodeList MEAS_sw_time = xmlDoc.GetElementsByTagName("MEAS_sw_time");
                if (MEAS_sw_time.Count > 0)
                {
                    XmlNode el = MEAS_sw_time[0];
                    obj._MEAS_sw_time = el.InnerText;
                }

                XmlNodeList TimerSendMeaskTaskToSDR = xmlDoc.GetElementsByTagName("TimerSendMeaskTaskToSDR");
                if (TimerSendMeaskTaskToSDR.Count > 0)
                {
                    XmlNode el = TimerSendMeaskTaskToSDR[0];
                    obj._TimerSendMeaskTaskToSDR = el.InnerText;
                }

                XmlNodeList ResendingSensorToSDRNS = xmlDoc.GetElementsByTagName("ResendingSensorToSDRNS");
                if (ResendingSensorToSDRNS.Count > 0)
                {
                    XmlNode el = ResendingSensorToSDRNS[0];
                    obj._ResendingSensorToSDRNS = el.InnerText;
                }

                XmlNodeList CheckActivitySensor = xmlDoc.GetElementsByTagName("CheckActivitySensor");
                if (CheckActivitySensor.Count > 0)
                {
                    XmlNode el = CheckActivitySensor[0];
                    obj._CheckActivitySensor = el.InnerText;
                }

                XmlNodeList TimeUpdateMeasTaskStatus = xmlDoc.GetElementsByTagName("TimeUpdateMeasTaskStatus");
                if (TimeUpdateMeasTaskStatus.Count > 0)
                {
                    XmlNode el = TimeUpdateMeasTaskStatus[0];
                    obj._TimeUpdateMeasTaskStatus = el.InnerText;
                }

                XmlNodeList TimerSendMeasSDRResults = xmlDoc.GetElementsByTagName("TimerSendMeasSDRResults");
                if (TimerSendMeasSDRResults.Count > 0)
                {
                    XmlNode el = TimerSendMeasSDRResults[0];
                    obj._TimerSendMeasSDRResults = el.InnerText;
                }

                XmlNodeList MaxTimeOnlineCalculation = xmlDoc.GetElementsByTagName("MaxTimeOnlineCalculation");
                if (MaxTimeOnlineCalculation.Count > 0)
                {
                    XmlNode el = MaxTimeOnlineCalculation[0];
                    obj._MaxTimeOnlineCalculation = el.InnerText;
                }

                XmlNodeList TimeArchiveResult = xmlDoc.GetElementsByTagName("TimeArchiveResult");
                if (TimeArchiveResult.Count > 0)
                {
                    XmlNode el = TimeArchiveResult[0];
                    obj._TimeArchiveResult = el.InnerText;
                }

                XmlNodeList TimeExpirationTemp = xmlDoc.GetElementsByTagName("TimeExpirationTemp");
                if (TimeExpirationTemp.Count > 0)
                {
                    XmlNode el = TimeExpirationTemp[0];
                    obj._TimeExpirationTemp = el.InnerText;
                }
                xml.Close();
                return obj;
            }
        }
    }
}
