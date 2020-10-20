using System;
using System.IO;
using System.Xml;


namespace XMLLibrary
{
    public struct XmlReaderStruct
    {
        public string _LicenseOwnerId;
        public string _LicenseProductKey;
        public string _RabbitMQHost;
        public string _RabbitMQVirtualHost;
        public string _RabbitMQUser;
        public string _RabbitMQPassword;
        public string _RabbitMQPort;
        public string _SDRNApiVersion;
        public string _SDRNServerInstance;
        public string _SDRNServerQueueNamePart;
        public string _SDRNDeviceSensorTechId;
        public string _SDRNDeviceExchange;
        public string _SDRNDeviceQueueNamePart;
        public string _SDRNDeviceMessagesBindings;
        public string _SDRNMessageConvertorUseEncryption;
        public string _SDRNMessageConvertorUseCompression;
        public string _DeviceBusSharedSecretKey;
        public string _DeviceBusClient;

        public string _DeviceBusContentType;
        public string _DeviceBusOutboxUseBuffer;
        public string _DeviceBusInboxUseBuffer;
        public string _DeviceBusOutboxBufferFolder;
        public string _DeviceBusInboxBufferFolder;
        public string _DeviceBusOutboxBufferContentType;
        public string _DeviceBusInboxBufferContentType;



        //public string _OwnerId;
        //public string _ProductKey;
        //public string _SensorEquipmentTechId;
        //public string _RabbitHostName;
        //public string _RabbitVirtualHostName;
        //public string _RabbitHostPort;
        //public string _RabbitUserName;
        //public string _RabbitPassword;
        //public string _SensorQueue;
        //public string _SensorConfirmQueue;
        //public string _TaskQueue;
        //public string _ResultQueue;
        //public string _ServerInstance;
    }


    public class XMLReader
    {
        public static XmlReaderStruct GetXmlSettings(string fileName)
        {
            string XmlSetting = "XmlSetting";
            lock (XmlSetting)
            {
                XmlReaderStruct obj = new XmlReaderStruct();
                XmlDocument xmlDoc = new XmlDocument();
                StreamReader xml = new StreamReader(fileName);
                xmlDoc.Load(new StringReader(xml.ReadToEnd()));
                XmlElement root = xmlDoc.DocumentElement;

                XmlNodeList NameClientLibrary = xmlDoc.GetElementsByTagName("LicenseOwnerId");
                if (NameClientLibrary.Count > 0)
                {
                    XmlNode el = NameClientLibrary[0];
                    obj._LicenseOwnerId = el.InnerText;
                }

                XmlNodeList LicenseProductKey = xmlDoc.GetElementsByTagName("LicenseProductKey");
                if (LicenseProductKey.Count > 0)
                {
                    XmlNode el = LicenseProductKey[0];
                    obj._LicenseProductKey = el.InnerText;
                }

                XmlNodeList RabbitMQHost = xmlDoc.GetElementsByTagName("RabbitMQHost");
                if (RabbitMQHost.Count > 0)
                {
                    XmlNode el = RabbitMQHost[0];
                    obj._RabbitMQHost = el.InnerText;
                }

                XmlNodeList RabbitMQVirtualHost = xmlDoc.GetElementsByTagName("RabbitMQVirtualHost");
                if (RabbitMQVirtualHost.Count > 0)
                {
                    XmlNode el = RabbitMQVirtualHost[0];
                    obj._RabbitMQVirtualHost = el.InnerText;
                }

                XmlNodeList RabbitMQUser = xmlDoc.GetElementsByTagName("RabbitMQUser");
                if (RabbitMQUser.Count > 0)
                {
                    XmlNode el = RabbitMQUser[0];
                    obj._RabbitMQUser = el.InnerText;
                }

                XmlNodeList RabbitMQPassword = xmlDoc.GetElementsByTagName("RabbitMQPassword");
                if (RabbitMQPassword.Count > 0)
                {
                    XmlNode el = RabbitMQPassword[0];
                    obj._RabbitMQPassword = el.InnerText;
                }

                XmlNodeList RabbitMQPort = xmlDoc.GetElementsByTagName("RabbitMQPort");
                if (RabbitMQPort.Count > 0)
                {
                    XmlNode el = RabbitMQPort[0];
                    obj._RabbitMQPort = el.InnerText;
                }

                XmlNodeList SDRNApiVersion = xmlDoc.GetElementsByTagName("SDRNApiVersion");
                if (SDRNApiVersion.Count > 0)
                {
                    XmlNode el = SDRNApiVersion[0];
                    obj._SDRNApiVersion = el.InnerText;
                }

                XmlNodeList SDRNServerInstance = xmlDoc.GetElementsByTagName("SDRNServerInstance");
                if (SDRNServerInstance.Count > 0)
                {
                    XmlNode el = SDRNServerInstance[0];
                    obj._SDRNServerInstance = el.InnerText;
                }

                XmlNodeList SDRNServerQueueNamePart = xmlDoc.GetElementsByTagName("SDRNServerQueueNamePart");
                if (SDRNServerQueueNamePart.Count > 0)
                {
                    XmlNode el = SDRNServerQueueNamePart[0];
                    obj._SDRNServerQueueNamePart = el.InnerText;
                }

                XmlNodeList SDRNDeviceSensorTechId = xmlDoc.GetElementsByTagName("SDRNDeviceSensorTechId");
                if (SDRNDeviceSensorTechId.Count > 0)
                {
                    XmlNode el = SDRNDeviceSensorTechId[0];
                    obj._SDRNDeviceSensorTechId = el.InnerText;
                }

                XmlNodeList SDRNDeviceExchange = xmlDoc.GetElementsByTagName("SDRNDeviceExchange");
                if (SDRNDeviceExchange.Count > 0)
                {
                    XmlNode el = SDRNDeviceExchange[0];
                    obj._SDRNDeviceExchange = el.InnerText;
                }

                XmlNodeList SDRNDeviceQueueNamePart = xmlDoc.GetElementsByTagName("SDRNDeviceQueueNamePart");
                if (SDRNDeviceQueueNamePart.Count > 0)
                {
                    XmlNode el = SDRNDeviceQueueNamePart[0];
                    obj._SDRNDeviceQueueNamePart = el.InnerText;
                }

                XmlNodeList SDRNDeviceMessagesBindings = xmlDoc.GetElementsByTagName("SDRNDeviceMessagesBindings");
                if (SDRNDeviceMessagesBindings.Count > 0)
                {
                    XmlNode el = SDRNDeviceMessagesBindings[0];
                    obj._SDRNDeviceMessagesBindings = el.InnerText;
                }

                XmlNodeList SDRNMessageConvertorUseEncryption = xmlDoc.GetElementsByTagName("SDRNMessageConvertorUseEncryption");
                if (SDRNMessageConvertorUseEncryption.Count > 0)
                {
                    XmlNode el = SDRNMessageConvertorUseEncryption[0];
                    obj._SDRNMessageConvertorUseEncryption = el.InnerText;
                }

                XmlNodeList SDRNMessageConvertorUseCompression = xmlDoc.GetElementsByTagName("SDRNMessageConvertorUseCompression");
                if (SDRNMessageConvertorUseCompression.Count > 0)
                {
                    XmlNode el = SDRNMessageConvertorUseCompression[0];
                    obj._SDRNMessageConvertorUseCompression = el.InnerText;
                }

                XmlNodeList DeviceBusSharedSecretKey = xmlDoc.GetElementsByTagName("DeviceBusSharedSecretKey");
                if (DeviceBusSharedSecretKey.Count > 0)
                {
                    XmlNode el = DeviceBusSharedSecretKey[0];
                    obj._DeviceBusSharedSecretKey = el.InnerText;
                }

                XmlNodeList DeviceBusClient = xmlDoc.GetElementsByTagName("DeviceBusClient");
                if (DeviceBusClient.Count > 0)
                {
                    XmlNode el = DeviceBusClient[0];
                    obj._DeviceBusClient = el.InnerText;
                }

                XmlNodeList DeviceBusContentType = xmlDoc.GetElementsByTagName("DeviceBusContentType");
                if (DeviceBusContentType.Count > 0)
                {
                    XmlNode el = DeviceBusContentType[0];
                    obj._DeviceBusContentType = el.InnerText;
                }

                XmlNodeList DeviceBusOutboxUseBuffer = xmlDoc.GetElementsByTagName("DeviceBusOutboxUseBuffer");
                if (DeviceBusOutboxUseBuffer.Count > 0)
                {
                    XmlNode el = DeviceBusOutboxUseBuffer[0];
                    obj._DeviceBusOutboxUseBuffer = el.InnerText;
                }

                XmlNodeList DeviceBusInboxUseBuffer = xmlDoc.GetElementsByTagName("DeviceBusInboxUseBuffer");
                if (DeviceBusInboxUseBuffer.Count > 0)
                {
                    XmlNode el = DeviceBusInboxUseBuffer[0];
                    obj._DeviceBusInboxUseBuffer = el.InnerText;
                }

                XmlNodeList DeviceBusOutboxBufferFolder = xmlDoc.GetElementsByTagName("DeviceBusOutboxBufferFolder");
                if (DeviceBusOutboxBufferFolder.Count > 0)
                {
                    XmlNode el = DeviceBusOutboxBufferFolder[0];
                    obj._DeviceBusOutboxBufferFolder = el.InnerText;
                }

                XmlNodeList DeviceBusInboxBufferFolder = xmlDoc.GetElementsByTagName("DeviceBusInboxBufferFolder");
                if (DeviceBusInboxBufferFolder.Count > 0)
                {
                    XmlNode el = DeviceBusInboxBufferFolder[0];
                    obj._DeviceBusInboxBufferFolder = el.InnerText;
                }

                XmlNodeList DeviceBusOutboxBufferContentType = xmlDoc.GetElementsByTagName("DeviceBusOutboxBufferContentType");
                if (DeviceBusOutboxBufferContentType.Count > 0)
                {
                    XmlNode el = DeviceBusOutboxBufferContentType[0];
                    obj._DeviceBusOutboxBufferContentType = el.InnerText;
                }

                XmlNodeList DeviceBusInboxBufferContentType = xmlDoc.GetElementsByTagName("DeviceBusInboxBufferContentType");
                if (DeviceBusInboxBufferContentType.Count > 0)
                {
                    XmlNode el = DeviceBusInboxBufferContentType[0];
                    obj._DeviceBusInboxBufferContentType = el.InnerText;
                }
                xml.Close();
                return obj;
            }
        }
        //public static XmlReaderStruct GetXmlSettings(string fileName)
        //{
        //    string XmlSetting = "XmlSetting";
        //    lock (XmlSetting) {
        //        XmlReaderStruct obj = new XmlReaderStruct();
        //        XmlDocument xmlDoc = new XmlDocument();
        //        StreamReader xml = new StreamReader(fileName);
        //        xmlDoc.Load(new StringReader(xml.ReadToEnd()));
        //        XmlElement root = xmlDoc.DocumentElement;
        //        XmlNodeList NameClientLibrary = xmlDoc.GetElementsByTagName("OwnerId");
        //        if (NameClientLibrary.Count > 0) {
        //            XmlNode el = NameClientLibrary[0];
        //            obj._OwnerId = el.InnerText;
        //        }
        //        XmlNodeList ConnectionString = xmlDoc.GetElementsByTagName("ProductKey");
        //        if (ConnectionString.Count > 0){
        //            XmlNode el = ConnectionString[0];
        //            obj._ProductKey = el.InnerText;
        //        }
        //        XmlNodeList OleConnectionString = xmlDoc.GetElementsByTagName("SensorEquipmentTechId");
        //        if (OleConnectionString.Count > 0) {
        //            XmlNode el = OleConnectionString[0];
        //            obj._SensorEquipmentTechId = el.InnerText;
        //        }
        //        XmlNodeList TypeRDBMS = xmlDoc.GetElementsByTagName("RabbitHostName");
        //        if (TypeRDBMS.Count > 0) {
        //            XmlNode el = TypeRDBMS[0];
        //            obj._RabbitHostName = el.InnerText;
        //        }
        //        XmlNodeList RabbitVirtualHostName = xmlDoc.GetElementsByTagName("RabbitVirtualHostName");
        //        if (RabbitVirtualHostName.Count > 0)
        //        {
        //            XmlNode el = RabbitVirtualHostName[0];
        //            obj._RabbitVirtualHostName = el.InnerText;
        //        }
        //        XmlNodeList RabbitHostPort = xmlDoc.GetElementsByTagName("RabbitHostPort");
        //        if (RabbitHostPort.Count > 0)
        //        {
        //            XmlNode el = RabbitHostPort[0];
        //            obj._RabbitHostPort = el.InnerText;
        //        }
        //        XmlNodeList TimeExpirationTemp = xmlDoc.GetElementsByTagName("RabbitUserName");
        //        if (TimeExpirationTemp.Count > 0) {
        //            XmlNode el = TimeExpirationTemp[0];
        //            obj._RabbitUserName = el.InnerText;
        //        }
        //        XmlNodeList LocationOrmSchema = xmlDoc.GetElementsByTagName("RabbitPassword");
        //        if (LocationOrmSchema.Count > 0) {
        //            XmlNode el = LocationOrmSchema[0];
        //            obj._RabbitPassword = el.InnerText;
        //        }
        //        XmlNodeList LocationCurrentConfFile = xmlDoc.GetElementsByTagName("SensorQueue");
        //        if (LocationCurrentConfFile.Count > 0) {
        //            XmlNode el = LocationCurrentConfFile[0];
        //            obj._SensorQueue = el.InnerText;
        //        }
        //        XmlNodeList LocationDefaultConfFile = xmlDoc.GetElementsByTagName("SensorConfirmQueue");
        //        if (LocationDefaultConfFile.Count > 0) {
        //            XmlNode el = LocationDefaultConfFile[0];
        //            obj._SensorConfirmQueue = el.InnerText;
        //        }
        //        XmlNodeList LocationLogFile = xmlDoc.GetElementsByTagName("TaskQueue");
        //        if (LocationLogFile.Count > 0) {
        //            XmlNode el = LocationLogFile[0];
        //            obj._TaskQueue = el.InnerText;
        //        }
        //        XmlNodeList NamePlugin = xmlDoc.GetElementsByTagName("ResultQueue");
        //        if (NamePlugin.Count > 0) {
        //            XmlNode el = NamePlugin[0];
        //            obj._ResultQueue = el.InnerText;
        //        }
        //        XmlNodeList ServerInstance = xmlDoc.GetElementsByTagName("ServerInstance");
        //        if (ServerInstance.Count > 0)
        //        {
        //            XmlNode el = ServerInstance[0];
        //            obj._ServerInstance = el.InnerText;
        //        }
        //        xml.Close();
        //        return obj;
        //    }
        //}
    }
}
