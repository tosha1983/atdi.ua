using System;
using System.IO;
using System.Xml;


namespace XMLLibrary
{
    public struct XmlReaderStruct
    {
        public string _OwnerId;
        public string _ProductKey;
        public string _SensorEquipmentTechId;
        public string _RabbitHostName;
        public string _RabbitVirtualHostName;
        public string _RabbitHostPort;
        public string _RabbitUserName;
        public string _RabbitPassword;
        public string _SensorQueue;
        public string _SensorConfirmQueue;
        public string _TaskQueue;
        public string _ResultQueue;
    }


    public class XMLReader
    {
        public static XmlReaderStruct GetXmlSettings(string fileName)
        {
            string XmlSetting = "XmlSetting";
            lock (XmlSetting) {
                XmlReaderStruct obj = new XmlReaderStruct();
                XmlDocument xmlDoc = new XmlDocument();
                StreamReader xml = new StreamReader(fileName);
                xmlDoc.Load(new StringReader(xml.ReadToEnd()));
                XmlElement root = xmlDoc.DocumentElement;
                XmlNodeList NameClientLibrary = xmlDoc.GetElementsByTagName("OwnerId");
                if (NameClientLibrary.Count > 0) {
                    XmlNode el = NameClientLibrary[0];
                    obj._OwnerId = el.InnerText;
                }
                XmlNodeList ConnectionString = xmlDoc.GetElementsByTagName("ProductKey");
                if (ConnectionString.Count > 0){
                    XmlNode el = ConnectionString[0];
                    obj._ProductKey = el.InnerText;
                }
                XmlNodeList OleConnectionString = xmlDoc.GetElementsByTagName("SensorEquipmentTechId");
                if (OleConnectionString.Count > 0) {
                    XmlNode el = OleConnectionString[0];
                    obj._SensorEquipmentTechId = el.InnerText;
                }
                XmlNodeList TypeRDBMS = xmlDoc.GetElementsByTagName("RabbitHostName");
                if (TypeRDBMS.Count > 0) {
                    XmlNode el = TypeRDBMS[0];
                    obj._RabbitHostName = el.InnerText;
                }
                XmlNodeList RabbitVirtualHostName = xmlDoc.GetElementsByTagName("RabbitVirtualHostName");
                if (RabbitVirtualHostName.Count > 0)
                {
                    XmlNode el = RabbitVirtualHostName[0];
                    obj._RabbitVirtualHostName = el.InnerText;
                }
                XmlNodeList RabbitHostPort = xmlDoc.GetElementsByTagName("RabbitHostPort");
                if (RabbitHostPort.Count > 0)
                {
                    XmlNode el = RabbitHostPort[0];
                    obj._RabbitHostPort = el.InnerText;
                }
                XmlNodeList TimeExpirationTemp = xmlDoc.GetElementsByTagName("RabbitUserName");
                if (TimeExpirationTemp.Count > 0) {
                    XmlNode el = TimeExpirationTemp[0];
                    obj._RabbitUserName = el.InnerText;
                }
                XmlNodeList LocationOrmSchema = xmlDoc.GetElementsByTagName("RabbitPassword");
                if (LocationOrmSchema.Count > 0) {
                    XmlNode el = LocationOrmSchema[0];
                    obj._RabbitPassword = el.InnerText;
                }
                XmlNodeList LocationCurrentConfFile = xmlDoc.GetElementsByTagName("SensorQueue");
                if (LocationCurrentConfFile.Count > 0) {
                    XmlNode el = LocationCurrentConfFile[0];
                    obj._SensorQueue = el.InnerText;
                }
                XmlNodeList LocationDefaultConfFile = xmlDoc.GetElementsByTagName("SensorConfirmQueue");
                if (LocationDefaultConfFile.Count > 0) {
                    XmlNode el = LocationDefaultConfFile[0];
                    obj._SensorConfirmQueue = el.InnerText;
                }
                XmlNodeList LocationLogFile = xmlDoc.GetElementsByTagName("TaskQueue");
                if (LocationLogFile.Count > 0) {
                    XmlNode el = LocationLogFile[0];
                    obj._TaskQueue = el.InnerText;
                }
                XmlNodeList NamePlugin = xmlDoc.GetElementsByTagName("ResultQueue");
                if (NamePlugin.Count > 0) {
                    XmlNode el = NamePlugin[0];
                    obj._ResultQueue = el.InnerText;
                }
                xml.Close();
                return obj;
            }
        }
    }
}
