using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Atdi.AppServer.AppService.WebQueryDataDriver.XMLLibrary
{
    public struct XMLObj
    {
       public string _NameClientLibrary;
       public string _ConnectionString;
       public string _OleConnectionString;
       public string _TypeRDBMS;
       public string _ORMSchema;
       public string _XMLSetting;
       public string _DefaultXMLSetting;
       public string _NamePlugin;
       public string _LocationPlugin;
       public string _NameLogFile;
       public string _StrUploadPath;
       public string _StrUploadServerPath;
    }

    public class BaseXMLDirect
    {
        public static string file_name_current = AppDomain.CurrentDomain.BaseDirectory + @"\DataDriver\SettingDataDriver.xml";
        public static string pathtoapp = AppDomain.CurrentDomain.BaseDirectory + @"\DataDriver";
        public static string pathto_LogFile = AppDomain.CurrentDomain.BaseDirectory + @"\DataDriver\LogDataDriver.txt";
        public static XMLObj GetXmlSettings(string FileName)
        {
            string XmlSetting = "XmlSetting";
            lock (XmlSetting){
                XMLObj obj = new XMLObj();
                XmlDocument xmlDoc = new XmlDocument();
                StreamReader xml = new StreamReader(FileName);
                xmlDoc.Load(new StringReader(xml.ReadToEnd()));
                XmlElement root = xmlDoc.DocumentElement;
                XmlNodeList NameClientLibrary = xmlDoc.GetElementsByTagName("NameClientLibrary");
                if (NameClientLibrary.Count > 0) {
                    XmlNode el = NameClientLibrary[0];
                    obj._NameClientLibrary = el.InnerText;
                }
                XmlNodeList ConnectionString = xmlDoc.GetElementsByTagName("ConnectionString");
                if (ConnectionString.Count > 0) {
                    XmlNode el = ConnectionString[0];
                    obj._ConnectionString = el.InnerText;
                }
                XmlNodeList OleConnectionString = xmlDoc.GetElementsByTagName("OleConnectionString");
                if (OleConnectionString.Count > 0) {
                    XmlNode el = OleConnectionString[0];
                    obj._OleConnectionString = el.InnerText;
                }
                XmlNodeList TypeRDBMS = xmlDoc.GetElementsByTagName("TypeRDBMS");
                if (TypeRDBMS.Count > 0) {
                    XmlNode el = TypeRDBMS[0];
                    obj._TypeRDBMS = el.InnerText;
                }
                XmlNodeList LocationOrmSchema = xmlDoc.GetElementsByTagName("LocationOrmSchema");
                if (LocationOrmSchema.Count > 0) {
                    XmlNode el = LocationOrmSchema[0];
                    obj._ORMSchema = el.InnerText;
                }
                XmlNodeList LocationCurrentConfFile = xmlDoc.GetElementsByTagName("LocationCurrentConfFile");
                if (LocationCurrentConfFile.Count > 0) {
                    XmlNode el = LocationCurrentConfFile[0];
                    obj._XMLSetting = el.InnerText;
                }
                XmlNodeList LocationDefaultConfFile = xmlDoc.GetElementsByTagName("LocationDefaultConfFile");
                if (LocationDefaultConfFile.Count > 0) {
                    XmlNode el = LocationDefaultConfFile[0];
                    obj._DefaultXMLSetting = el.InnerText;
                }
                XmlNodeList LocationLogFile = xmlDoc.GetElementsByTagName("LocationLogFile");
                if (LocationLogFile.Count > 0) {
                    XmlNode el = LocationLogFile[0];
                    obj._NameLogFile = el.InnerText;
                }
                XmlNodeList NamePlugin = xmlDoc.GetElementsByTagName("NamePlugin");
                if (NamePlugin.Count > 0) {
                    XmlNode el = NamePlugin[0];
                    obj._NamePlugin = el.InnerText;
                }
                XmlNodeList LocationPlugin = xmlDoc.GetElementsByTagName("LocationPlugin");
                if (LocationPlugin.Count > 0) {
                    XmlNode el = LocationPlugin[0];
                    obj._LocationPlugin = el.InnerText;
                }
                XmlNodeList StrUploadPath = xmlDoc.GetElementsByTagName("StrUploadPath");
                if (StrUploadPath.Count > 0) {
                    XmlNode el = StrUploadPath[0];
                    obj._StrUploadPath = el.InnerText;
                }
                XmlNodeList StrUploadServerPath = xmlDoc.GetElementsByTagName("StrUploadServerPath");
                if (StrUploadServerPath.Count > 0) {
                    XmlNode el = StrUploadServerPath[0];
                    obj._StrUploadServerPath = el.InnerText;
                }
                xml.Close();
                return obj;
            }
        
        }
    }
    
}
