using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Windows.Forms;



namespace XMLLibrary
{
    public struct XMLConfiguration
    {
        public string _MainRabbitMQServices;
        public string _Lon_Delta;
        public string _Lat_Delta;
        public string _TimeExpirationTask;
        public string _TimeExpirationTemp;
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
        public string _CheckActivitySensor;
        public string _MaxTimeNotActivateStatusSensor; //в секундах
        public string _RescanActivitySensor;
        public string _TimeUpdateMeasTaskStatus;
        public string _TimeUpdateMeasResult;
        public string _TimeArchiveResult;
        



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
        public int _RescanActivitySensor;
        public int _CheckActivitySensor;
        public int _MaxTimeNotActivateStatusSensor; //в секундах
        public int _TimeUpdateMeasTaskStatus;
        public int _TimeUpdateMeasResult;
        public int _TimeArchiveResult;
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
            int.TryParse(xml_configuration._ClassSensorSubmitToDB.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._ClassSensorSubmitToDB);
            int.TryParse(xml_configuration._DefaultValueMinTimeInterval.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._DefaultValueMinTimeInterval);
            int.TryParse(xml_configuration._ReloadStart.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._ReloadStart);
            int.TryParse(xml_configuration._ScanDataSensor.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._ScanDataSensor);
            int.TryParse(xml_configuration._ScanMeasTasks.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._ScanMeasTasks);
            int.TryParse(xml_configuration._ShedulerSensorSubmitLstQueues.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._ShedulerSensorSubmitLstQueues);
            int.TryParse(xml_configuration._TimeExpirationTask.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._TimeExpirationTask);
            int.TryParse(xml_configuration._TimeExpirationTemp.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._TimeExpirationTemp);
            double.TryParse(xml_configuration._MEAS_SDR_PARAM_RBW.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._MEAS_SDR_PARAM_RBW);
            double.TryParse(xml_configuration._MEAS_SDR_PARAM_VBW.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._MEAS_SDR_PARAM_VBW);
            double.TryParse(xml_configuration._MEAS_SDR_PARAM_ref_level_dbm.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._MEAS_SDR_PARAM_ref_level_dbm);
            double.TryParse(xml_configuration._MEAS_SDR_PARAM_Time_of_m.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._MEAS_SDR_PARAM_Time_of_m);
            xml_conf._MEAS_Type_of_m = xml_configuration._MEAS_Type_of_m;
            xml_conf._MEAS_TypeFunction = xml_configuration._MEAS_TypeFunction;
            int.TryParse(xml_configuration._MEAS_sw_time.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._MEAS_sw_time);
            int.TryParse(xml_configuration._TimerSendMeaskTaskToSDR.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._TimerSendMeaskTaskToSDR);
            int.TryParse(xml_configuration._CheckActivitySensor.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._CheckActivitySensor);
            int.TryParse(xml_configuration._MaxTimeNotActivateStatusSensor.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._MaxTimeNotActivateStatusSensor);
            int.TryParse(xml_configuration._RescanActivitySensor.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._RescanActivitySensor);
            int.TryParse(xml_configuration._TimeUpdateMeasTaskStatus.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._TimeUpdateMeasTaskStatus);
            int.TryParse(xml_configuration._TimeUpdateMeasResult.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._TimeUpdateMeasResult);
            int.TryParse(xml_configuration._TimeArchiveResult.Trim().ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out xml_conf._TimeArchiveResult);

            
        }
        public static XMLConfiguration GetXmlSettings(string FileName)
        {
            string XmlSetting = "XmlSetting";
            lock (XmlSetting) {
                XMLConfiguration obj = new XMLConfiguration();
                XmlDocument xmlDoc = new XmlDocument();
                StreamReader xml = new StreamReader(FileName);
                xmlDoc.Load(new StringReader(xml.ReadToEnd()));
                XmlElement root = xmlDoc.DocumentElement;
                XmlNodeList NameClientLibrary = xmlDoc.GetElementsByTagName("MainRabbitMQServices");
                if (NameClientLibrary.Count > 0) {
                    XmlNode el = NameClientLibrary[0];
                    obj._MainRabbitMQServices = el.InnerText;
                }
                XmlNodeList ConnectionString = xmlDoc.GetElementsByTagName("Lon_Delta");
                if (ConnectionString.Count > 0){
                    XmlNode el = ConnectionString[0];
                    obj._Lon_Delta = el.InnerText;
                }
                XmlNodeList OleConnectionString = xmlDoc.GetElementsByTagName("Lat_Delta");
                if (OleConnectionString.Count > 0) {
                    XmlNode el = OleConnectionString[0];
                    obj._Lat_Delta = el.InnerText;
                }
                XmlNodeList TypeRDBMS = xmlDoc.GetElementsByTagName("TimeExpirationTask");
                if (TypeRDBMS.Count > 0) {
                    XmlNode el = TypeRDBMS[0];
                    obj._TimeExpirationTask = el.InnerText;
                }
                XmlNodeList TimeExpirationTemp = xmlDoc.GetElementsByTagName("TimeExpirationTemp");
                if (TimeExpirationTemp.Count > 0) {
                    XmlNode el = TimeExpirationTemp[0];
                    obj._TimeExpirationTemp = el.InnerText;
                }
                XmlNodeList LocationOrmSchema = xmlDoc.GetElementsByTagName("ReloadStart");
                if (LocationOrmSchema.Count > 0) {
                    XmlNode el = LocationOrmSchema[0];
                    obj._ReloadStart = el.InnerText;
                }
                XmlNodeList LocationCurrentConfFile = xmlDoc.GetElementsByTagName("ScanDataSensor");
                if (LocationCurrentConfFile.Count > 0) {
                    XmlNode el = LocationCurrentConfFile[0];
                    obj._ScanDataSensor = el.InnerText;
                }
                XmlNodeList LocationDefaultConfFile = xmlDoc.GetElementsByTagName("ClassSensorSubmitToDB");
                if (LocationDefaultConfFile.Count > 0) {
                    XmlNode el = LocationDefaultConfFile[0];
                    obj._ClassSensorSubmitToDB = el.InnerText;
                }
                XmlNodeList LocationLogFile = xmlDoc.GetElementsByTagName("ScanMeasTasks");
                if (LocationLogFile.Count > 0) {
                    XmlNode el = LocationLogFile[0];
                    obj._ScanMeasTasks = el.InnerText;
                }
                XmlNodeList NamePlugin = xmlDoc.GetElementsByTagName("ShedulerSensorSubmitLstQueues");
                if (NamePlugin.Count > 0) {
                    XmlNode el = NamePlugin[0];
                    obj._ShedulerSensorSubmitLstQueues = el.InnerText;
                }
                XmlNodeList LocationPlugin = xmlDoc.GetElementsByTagName("DefaultValueMinTimeInterval");
                if (LocationPlugin.Count > 0) {
                    XmlNode el = LocationPlugin[0];
                    obj._DefaultValueMinTimeInterval = el.InnerText;
                }

                XmlNodeList MEAS_SDR_PARAM_Time_of_m = xmlDoc.GetElementsByTagName("MEAS_SDR_PARAM_Time_of_m");
                if (MEAS_SDR_PARAM_Time_of_m.Count > 0) {
                    XmlNode el = MEAS_SDR_PARAM_Time_of_m[0];
                    obj._MEAS_SDR_PARAM_Time_of_m = el.InnerText;
                }

                XmlNodeList MEAS_SDR_PARAM_RBW = xmlDoc.GetElementsByTagName("MEAS_SDR_PARAM_RBW");
                if (MEAS_SDR_PARAM_RBW.Count > 0) {
                    XmlNode el = MEAS_SDR_PARAM_RBW[0];
                    obj._MEAS_SDR_PARAM_RBW = el.InnerText;
                }

                XmlNodeList MEAS_SDR_PARAM_VBW = xmlDoc.GetElementsByTagName("MEAS_SDR_PARAM_VBW");
                if (MEAS_SDR_PARAM_VBW.Count > 0) {
                    XmlNode el = MEAS_SDR_PARAM_VBW[0];
                    obj._MEAS_SDR_PARAM_VBW = el.InnerText;
                }

                XmlNodeList MEAS_SDR_PARAM_ref_level_dbm = xmlDoc.GetElementsByTagName("MEAS_SDR_PARAM_ref_level_dbm");
                if (MEAS_SDR_PARAM_ref_level_dbm.Count > 0) {
                    XmlNode el = MEAS_SDR_PARAM_ref_level_dbm[0];
                    obj._MEAS_SDR_PARAM_ref_level_dbm = el.InnerText;
                }

                XmlNodeList MEAS_TypeFunction = xmlDoc.GetElementsByTagName("MEAS_TypeFunction");
                if (MEAS_TypeFunction.Count > 0) {
                    XmlNode el = MEAS_TypeFunction[0];
                    obj._MEAS_TypeFunction = el.InnerText;
                }

                XmlNodeList MEAS_Type_of_m = xmlDoc.GetElementsByTagName("MEAS_Type_of_m");
                if (MEAS_Type_of_m.Count > 0) {
                    XmlNode el = MEAS_Type_of_m[0];
                    obj._MEAS_Type_of_m = el.InnerText;
                }

                XmlNodeList MEAS_sw_time = xmlDoc.GetElementsByTagName("MEAS_sw_time");
                if (MEAS_sw_time.Count > 0) {
                    XmlNode el = MEAS_sw_time[0];
                    obj._MEAS_sw_time = el.InnerText;
                }

                XmlNodeList TimerSendMeaskTaskToSDR = xmlDoc.GetElementsByTagName("TimerSendMeaskTaskToSDR");
                if (TimerSendMeaskTaskToSDR.Count > 0) {
                    XmlNode el = TimerSendMeaskTaskToSDR[0];
                    obj._TimerSendMeaskTaskToSDR = el.InnerText;
                }

                XmlNodeList CheckActivitySensor = xmlDoc.GetElementsByTagName("CheckActivitySensor");
                if (CheckActivitySensor.Count > 0)
                {
                    XmlNode el = CheckActivitySensor[0];
                    obj._CheckActivitySensor = el.InnerText;
                }

                XmlNodeList _MaxTimeNotActivateStatusSensor = xmlDoc.GetElementsByTagName("MaxTimeNotActivateStatusSensor");
                if (_MaxTimeNotActivateStatusSensor.Count > 0)
                {
                    XmlNode el = _MaxTimeNotActivateStatusSensor[0];
                    obj._MaxTimeNotActivateStatusSensor = el.InnerText;
                }

                XmlNodeList RescanActivitySensor = xmlDoc.GetElementsByTagName("RescanActivitySensor");
                if (RescanActivitySensor.Count > 0)
                {
                    XmlNode el = RescanActivitySensor[0];
                    obj._RescanActivitySensor = el.InnerText;
                }

                XmlNodeList TimeUpdateMeasTaskStatus = xmlDoc.GetElementsByTagName("TimeUpdateMeasTaskStatus");
                if (TimeUpdateMeasTaskStatus.Count > 0)
                {
                    XmlNode el = TimeUpdateMeasTaskStatus[0];
                    obj._TimeUpdateMeasTaskStatus = el.InnerText;
                }

                XmlNodeList TimeUpdateMeasResult = xmlDoc.GetElementsByTagName("TimeUpdateMeasResult");
                if (TimeUpdateMeasResult.Count > 0)
                {
                    XmlNode el = TimeUpdateMeasResult[0];
                    obj._TimeUpdateMeasResult = el.InnerText;
                }

                XmlNodeList TimeArchiveResult = xmlDoc.GetElementsByTagName("TimeArchiveResult");
                if (TimeArchiveResult.Count > 0)
                {
                    XmlNode el = TimeArchiveResult[0];
                    obj._TimeArchiveResult = el.InnerText;
                }
                


                xml.Close();
                return obj;
            }
        }
    }
}
