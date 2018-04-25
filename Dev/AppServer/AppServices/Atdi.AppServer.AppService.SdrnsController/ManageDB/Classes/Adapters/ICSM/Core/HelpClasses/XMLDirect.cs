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
    [Serializable]
    public struct XMLObj
    {
       public string _NameClientLibrary;
       public string _SecondaryConnectionString;
       public string _MainConnectionString;
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

    [Serializable]
    public class BaseXMLDirect
    {
        public static string file_name_current = AppDomain.CurrentDomain.BaseDirectory + @"\DataDriver\SettingDataDriver.xml";
        public static string file_name_default = AppDomain.CurrentDomain.BaseDirectory + @"\DataDriver\SettingDefDataDriver.xml";
        public static string file_name_email_event = AppDomain.CurrentDomain.BaseDirectory + @"\DataDriver\SettingTemplateEmail.xml";
        public static string file_name_setting_radiotech = AppDomain.CurrentDomain.BaseDirectory + @"\DataDriver\SettingTemplateRadioTech.xml";
        public static string pathtoapp = AppDomain.CurrentDomain.BaseDirectory + @"\DataDriver";
        public static string pathto_LogFile = AppDomain.CurrentDomain.BaseDirectory + @"\DataDriver\LogDataDriver.txt";

        
        /// <summary>
        /// Создание заголовка XML
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="rootElem"></param>
        /// <param name="nameSpaceURI"></param>
        /// <returns></returns>
        private static XmlDocument NewXML(string prefix, string rootElem, string nameSpaceURI)
        {
                string XmlSetting = "XmlSetting";
              lock (XmlSetting)
              {
                  XmlDocument retXML = new XmlDocument();
                  // Добавляем шапку XML
                  XmlDeclaration xmldecl = retXML.CreateXmlDeclaration("1.0", "UTF-8", null);
                  retXML.AppendChild(retXML.CreateElement(prefix, rootElem, nameSpaceURI));
                  XmlElement xmlRoot = retXML.DocumentElement; //Главный элемент
                  retXML.InsertBefore(xmldecl, xmlRoot);
                  return retXML;
              }
        }

        /// <summary>
        /// Сохранение конфигурационных даных в  XML файл
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="FileName"></param>
        public static void SaveXmlDocument(XMLObj obj)
        {
            string XmlSetting = "XmlSetting";
             lock (XmlSetting)
             {
                 XmlDocument xmlDoc = NewXML("", "PluginConfiguration", "");
                 XmlElement xmlRoot = xmlDoc.DocumentElement;
                 XmlElement Elem = xmlDoc.CreateElement("", "RootElement", "");



                 XmlElement NameClientLibrary = xmlDoc.CreateElement("", "NameClientLibrary", "");
                 XmlText value_NameClientLibrary = xmlDoc.CreateTextNode(obj._NameClientLibrary);
                 NameClientLibrary.AppendChild(value_NameClientLibrary);
                 Elem.AppendChild(NameClientLibrary);


                 XmlElement MainConnectionString = xmlDoc.CreateElement("", "MainConnectionString", "");
                 XmlText value_MainConnectionString = xmlDoc.CreateTextNode(obj._MainConnectionString);
                 MainConnectionString.AppendChild(value_MainConnectionString);
                 Elem.AppendChild(MainConnectionString);

                 XmlElement SecondaryConnectionString = xmlDoc.CreateElement("", "SecondaryConnectionString", "");
                 XmlText value_SecondaryConnectionString = xmlDoc.CreateTextNode(obj._SecondaryConnectionString);
                 SecondaryConnectionString.AppendChild(value_SecondaryConnectionString);
                 Elem.AppendChild(SecondaryConnectionString);

                 XmlElement TypeRDBMS = xmlDoc.CreateElement("", "TypeRDBMS", "");
                 XmlText value_TypeRDBMS = xmlDoc.CreateTextNode(obj._TypeRDBMS);
                 TypeRDBMS.AppendChild(value_TypeRDBMS);
                 Elem.AppendChild(TypeRDBMS);

                 XmlElement LocationOrmSchema = xmlDoc.CreateElement("", "LocationOrmSchema", "");
                 XmlText value_LocationOrmSchema = xmlDoc.CreateTextNode(obj._ORMSchema);
                 LocationOrmSchema.AppendChild(value_LocationOrmSchema);
                 Elem.AppendChild(LocationOrmSchema);

                 XmlElement LocationCurrentConfFile = xmlDoc.CreateElement("", "LocationCurrentConfFile", "");
                 XmlText value_LocationCurrentConfFile = xmlDoc.CreateTextNode(obj._XMLSetting);
                 LocationCurrentConfFile.AppendChild(value_LocationCurrentConfFile);
                 Elem.AppendChild(LocationCurrentConfFile);

                 XmlElement LocationDefaultConfFile = xmlDoc.CreateElement("", "LocationDefaultConfFile", "");
                 XmlText value_LocationDefaultConfFile = xmlDoc.CreateTextNode(obj._DefaultXMLSetting);
                 LocationDefaultConfFile.AppendChild(value_LocationDefaultConfFile);
                 Elem.AppendChild(LocationDefaultConfFile);

                 XmlElement LocationLogFile = xmlDoc.CreateElement("", "LocationLogFile", "");
                 XmlText value_LocationLogFile = xmlDoc.CreateTextNode(obj._NameLogFile);
                 LocationLogFile.AppendChild(value_LocationLogFile);
                 Elem.AppendChild(LocationLogFile);

                 XmlElement NamePlugin = xmlDoc.CreateElement("", "NamePlugin", "");
                 XmlText value_NamePlugin = xmlDoc.CreateTextNode(obj._NamePlugin);
                 NamePlugin.AppendChild(value_NamePlugin);
                 Elem.AppendChild(NamePlugin);

                 XmlElement LocationPlugin = xmlDoc.CreateElement("", "LocationPlugin", "");
                 XmlText value_LocationPlugin = xmlDoc.CreateTextNode(obj._LocationPlugin);
                 LocationPlugin.AppendChild(value_LocationPlugin);
                 Elem.AppendChild(LocationPlugin);

                 XmlElement StrUploadPath = xmlDoc.CreateElement("", "StrUploadPath", "");
                 XmlText value_StrUploadPath = xmlDoc.CreateTextNode(obj._StrUploadPath);
                 StrUploadPath.AppendChild(value_StrUploadPath);
                 Elem.AppendChild(StrUploadPath);

                 XmlElement StrUploadServerPath = xmlDoc.CreateElement("", "StrUploadServerPath", "");
                 XmlText value_StrUploadServerPath = xmlDoc.CreateTextNode(obj._StrUploadServerPath);
                 StrUploadServerPath.AppendChild(value_StrUploadServerPath);
                 Elem.AppendChild(StrUploadServerPath);

                 

                 // Создаем атрибут
                 Elem.SetAttribute("ListPlugins", "SettingFileSaveListPlugins");

                 xmlRoot.AppendChild(Elem);

                 xmlDoc.Save(file_name_current);
             }
        }

       
        /// <summary>
        /// Чтение перечня сохраненных в XML файле наименований плагинов
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static List<string> GetXmlDocument(string FileName)
        {
            string XmlSetting = "XmlSetting";
            lock (XmlSetting)
            {
                List<string> mass_guid = new List<string>();
                mass_guid.Clear();

                XmlTextReader rdrXml = new XmlTextReader(FileName);

                do
                {
                    switch (rdrXml.NodeType)
                    {
                        case XmlNodeType.Text:
                            mass_guid.Add(rdrXml.Value.ToString());
                            break;
                    }
                } while (rdrXml.Read());

                return mass_guid;
            }
          
        }

        public static XMLObj GetXmlSettings(string FileName)
        {
            string XmlSetting = "XmlSetting";
            lock (XmlSetting)
            {
                XMLObj obj = new XMLObj();
                XmlDocument xmlDoc = new XmlDocument();
                StreamReader xml = new StreamReader(FileName);
                xmlDoc.Load(new StringReader(xml.ReadToEnd()));
                XmlElement root = xmlDoc.DocumentElement;

                XmlNodeList NameClientLibrary = xmlDoc.GetElementsByTagName("NameClientLibrary");
                if (NameClientLibrary.Count > 0)
                {
                    XmlNode el = NameClientLibrary[0];
                    obj._NameClientLibrary = el.InnerText;
                }

                XmlNodeList MainConnectionString = xmlDoc.GetElementsByTagName("MainConnectionString");
                if (MainConnectionString.Count > 0)
                {
                    XmlNode el = MainConnectionString[0];
                    obj._MainConnectionString = el.InnerText;
                }

                XmlNodeList SecondaryConnectionString = xmlDoc.GetElementsByTagName("SecondaryConnectionString");
                if (SecondaryConnectionString.Count > 0)
                {
                    XmlNode el = SecondaryConnectionString[0];
                    obj._SecondaryConnectionString = el.InnerText;
                }

                XmlNodeList TypeRDBMS = xmlDoc.GetElementsByTagName("TypeRDBMS");
                if (TypeRDBMS.Count > 0)
                {
                    XmlNode el = TypeRDBMS[0];
                    obj._TypeRDBMS = el.InnerText;
                }

                XmlNodeList LocationOrmSchema = xmlDoc.GetElementsByTagName("LocationOrmSchema");
                if (LocationOrmSchema.Count > 0)
                {
                    XmlNode el = LocationOrmSchema[0];
                    obj._ORMSchema = el.InnerText;
                }

                XmlNodeList LocationCurrentConfFile = xmlDoc.GetElementsByTagName("LocationCurrentConfFile");
                if (LocationCurrentConfFile.Count > 0)
                {
                    XmlNode el = LocationCurrentConfFile[0];
                    obj._XMLSetting = el.InnerText;
                }

                XmlNodeList LocationDefaultConfFile = xmlDoc.GetElementsByTagName("LocationDefaultConfFile");
                if (LocationDefaultConfFile.Count > 0)
                {
                    XmlNode el = LocationDefaultConfFile[0];
                    obj._DefaultXMLSetting = el.InnerText;
                }

                XmlNodeList LocationLogFile = xmlDoc.GetElementsByTagName("LocationLogFile");
                if (LocationLogFile.Count > 0)
                {
                    XmlNode el = LocationLogFile[0];
                    obj._NameLogFile = el.InnerText;
                }

                XmlNodeList NamePlugin = xmlDoc.GetElementsByTagName("NamePlugin");
                if (NamePlugin.Count > 0)
                {
                    XmlNode el = NamePlugin[0];
                    obj._NamePlugin = el.InnerText;
                }

                XmlNodeList LocationPlugin = xmlDoc.GetElementsByTagName("LocationPlugin");
                if (LocationPlugin.Count > 0)
                {
                    XmlNode el = LocationPlugin[0];
                    obj._LocationPlugin = el.InnerText;
                }


                XmlNodeList StrUploadPath = xmlDoc.GetElementsByTagName("StrUploadPath");
                if (StrUploadPath.Count > 0)
                {
                    XmlNode el = StrUploadPath[0];
                    obj._StrUploadPath = el.InnerText;
                }

                XmlNodeList StrUploadServerPath = xmlDoc.GetElementsByTagName("StrUploadServerPath");
                if (StrUploadServerPath.Count > 0)
                {
                    XmlNode el = StrUploadServerPath[0];
                    obj._StrUploadServerPath = el.InnerText;
                }

                xml.Close();

                return obj;
            }
        
        }

        /// <summary>
        /// Парсер файла с данными для отправки уведомлений по почте
        /// </summary>
        /// <param name="XML"></param>
        /// <returns></returns>
        static public List<TemplateLetter> ParseXMLEmailSettings(string XML)
        {
            List<TemplateLetter> Template_List = new List<TemplateLetter>();
            try
            {
                XDocument xdoc = new XDocument();
                TemplateLetter Template = new TemplateLetter();
                using (StreamReader reader = new StreamReader(XML, Encoding.UTF8))
                {
                    xdoc = XDocument.Parse(reader.ReadToEnd());
                    IEnumerable<XElement> WKFSettingGroup = xdoc.Element("SettingGroupObject").Element("Params").Element("ObjectDataGroup").Element("AdminSetting").Elements("Category");


                    foreach (XElement xel in WKFSettingGroup.Elements())
                    {


                        if ((xel.Name.ToString() == "Group") || (xel.Name.LocalName == "Group"))
                        {
                            IEnumerable<XElement> ellis_items_menu = xel.Elements("ItemMenu").Elements();
                            List<SMTPSetting> Temp = new List<SMTPSetting>();
                            SMTPSetting tempAdd = new SMTPSetting();
                            foreach (XElement xel_item in ellis_items_menu)
                            {

                                if ((xel_item.Name.ToString() == "Port") || (xel_item.Name.LocalName == "Port"))
                                {
                                    int Val = 0;
                                    int.TryParse(xel_item.Value.ToString(), out Val);
                                    tempAdd.Port = Val;
                                }
                                if ((xel_item.Name.ToString() == "TimeOut") || (xel_item.Name.LocalName == "TimeOut"))
                                {
                                    int Val = 0;
                                    int.TryParse(xel_item.Value.ToString(), out Val);
                                    tempAdd.TimeOut = Val;
                                }
                                if ((xel_item.Name.ToString() == "SMTP") || (xel_item.Name.LocalName == "SMTP"))
                                {
                                    tempAdd.SMTP = xel_item.Value.ToString();
                                }
                                if ((xel_item.Name.ToString() == "Login") || (xel_item.Name.LocalName == "Login"))
                                {
                                    tempAdd.Login = xel_item.Value.ToString();
                                }
                                if ((xel_item.Name.ToString() == "Password") || (xel_item.Name.LocalName == "Password"))
                                {
                                    tempAdd.Password = xel_item.Value.ToString();


                                    if (!Temp.Contains(tempAdd))
                                    {
                                        Temp.Add(tempAdd);
                                        tempAdd = new SMTPSetting();
                                    }

                                }
                            }
                            Template.Props = new List<SMTPSetting>();
                            Template.Props = Temp;
                            Template_List.Add(Template);
                            Template = new TemplateLetter();

                        }



                    }
                }

                Template = new TemplateLetter();
                xdoc = new XDocument();
                using (StreamReader reader = new StreamReader(XML, Encoding.UTF8))
                {
                    /////
                    xdoc = XDocument.Parse(reader.ReadToEnd());
                    IEnumerable<XElement> WKFSettingAdminEmail = xdoc.Element("SettingGroupObject").Element("Params").Element("ObjectDataGroup").Element("AdminControl").Elements("Category");


                    foreach (XElement xel in WKFSettingAdminEmail.Elements())
                    {

                        if ((xel.Name.ToString() == "Group") || (xel.Name.LocalName == "Group"))
                        {
                            IEnumerable<XElement> ellis_items_menu = xel.Elements("ItemMenu").Elements();
                            List<PropertiesAdmin> Temp = new List<PropertiesAdmin>();
                            PropertiesAdmin tempAdd = new PropertiesAdmin();
                            foreach (XElement xel_item in ellis_items_menu)
                            {

                                if ((xel_item.Name.ToString() == "ID") || (xel_item.Name.LocalName == "ID"))
                                {
                                    tempAdd.ID = xel_item.Value.ToString();
                                }
                                if ((xel_item.Name.ToString() == "Descr") || (xel_item.Name.LocalName == "Descr"))
                                {
                                    tempAdd.Descr = xel_item.Value.ToString();
                                }
                                if ((xel_item.Name.ToString() == "Email") || (xel_item.Name.LocalName == "Email"))
                                {
                                    tempAdd.Email = xel_item.Value.ToString();
                                }
                                if ((xel_item.Name.ToString() == "Subject") || (xel_item.Name.LocalName == "Subject"))
                                {
                                    tempAdd.Subject = xel_item.Value.ToString();
                                }
                                if ((xel_item.Name.ToString() == "Body") || (xel_item.Name.LocalName == "Body"))
                                {
                                    tempAdd.Body = xel_item.Value.ToString();


                                    if (!Temp.Contains(tempAdd))
                                    {
                                        Temp.Add(tempAdd);
                                        tempAdd = new PropertiesAdmin();
                                    }

                                }
                            }
                            Template.Admins = new List<PropertiesAdmin>();
                            if (Template_List.Count() > 0) { Template_List[0].Admins = Temp; }

                        }



                    }

                }
            }
            catch (Exception)
            {
                
            }

            return Template_List;

        }

        static public List<RadioTechSettings> ParseXMLSettingsRadioTech(string XML)
        {
            List<RadioTechSettings> Template_List = new List<RadioTechSettings>();
            try
            {
                XDocument xdoc = new XDocument();
                TemplateLetter Template = new TemplateLetter();
               
                using (StreamReader reader = new StreamReader(XML, Encoding.UTF8))
                {
                    /////
                    xdoc = XDocument.Parse(reader.ReadToEnd());
                    IEnumerable<XElement> WKFSettingAdminEmail = xdoc.Element("SettingGroupObject").Element("Params").Element("ObjectDataGroup").Element("AdminControl").Elements("Category");


                    foreach (XElement xel in WKFSettingAdminEmail.Elements())
                    {

                        if ((xel.Name.ToString() == "Group") || (xel.Name.LocalName == "Group"))
                        {
                            IEnumerable<XElement> ellis_items_menu = xel.Elements("ItemMenu").Elements();
                            RadioTechSettings tempAdd = new RadioTechSettings();
                            foreach (XElement xel_item in ellis_items_menu)
                            {

                                if ((xel_item.Name.ToString() == "NUM_TECH") || (xel_item.Name.LocalName == "NUM_TECH"))
                                {
                                    if (xel_item.Value.ToString().Trim() != "")
                                    {
                                        tempAdd.NumTech = int.Parse(xel_item.Value);
                                    }
                                }
                                if ((xel_item.Name.ToString() == "IT_MENU") || (xel_item.Name.LocalName == "IT_MENU"))
                                {
                                    tempAdd.IT_MENU = xel_item.Value.ToString();
                                }
                                if ((xel_item.Name.ToString() == "TABLE") || (xel_item.Name.LocalName == "TABLE"))
                                {
                                    tempAdd.TABLE = xel_item.Value.ToString();
                                }
                                if ((xel_item.Name.ToString() == "PAGE_L1") || (xel_item.Name.LocalName == "PAGE_L1"))
                                {
                                    tempAdd.PAGE_L1 = xel_item.Value.ToString();
                                }
                                if ((xel_item.Name.ToString() == "PAGE_L2") || (xel_item.Name.LocalName == "PAGE_L2"))
                                {
                                    tempAdd.PAGE_L2 = xel_item.Value.ToString();
                                }
                                if ((xel_item.Name.ToString() == "PAGE_L3") || (xel_item.Name.LocalName == "PAGE_L3"))
                                {
                                    tempAdd.PAGE_L3 = xel_item.Value.ToString();
                                }
                                if ((xel_item.Name.ToString() == "RADIOTECH") || (xel_item.Name.LocalName == "RADIOTECH"))
                                {
                                    tempAdd.RadioTech = xel_item.Value.ToString();

                                }
                                if ((xel_item.Name.ToString() == "WORKFLOW") || (xel_item.Name.LocalName == "WORKFLOW"))
                                {
                                    if (xel_item.Value.ToString().Trim() != "")
                                    {
                                        tempAdd.WorkFlow = int.Parse(xel_item.Value);
                                    }
                                }
                                if ((xel_item.Name.ToString() == "SUBTYPE1") || (xel_item.Name.LocalName == "SUBTYPE1"))
                                {
                                    if (xel_item.Value.ToString().Trim() != "")
                                    {
                                        tempAdd.SubType  = int.Parse(xel_item.Value);

                                        Template_List.Add(tempAdd);
                                        tempAdd = new RadioTechSettings();
                                        
                                    }
                                   
                                }

                            }

                        }

                    }

                }
            }
            catch (Exception)
            {

            }

            return Template_List;

        }

        /// <summary>
        /// Получить настройки отправки уведомлений по почте
        /// </summary>
        /// <returns></returns>
        static public List<TemplateLetter> GetSettingEmailNotification()
        {
            List<TemplateLetter> Letter = ParseXMLEmailSettings(file_name_email_event);
            return Letter;
        }

        /// <summary>
        /// Получить настройки перечня радиотехнологий
        /// </summary>
        /// <returns></returns>
        static public List<RadioTechSettings> GetSettingRadioTech()
        {
            List<RadioTechSettings> SettingRadioTech = ParseXMLSettingsRadioTech(file_name_setting_radiotech);
            return SettingRadioTech;
        }

        


    }


    public sealed class TemplateLetter
    {
        public string ID { get; set; }
        public string TypeApplication { get; set; }
        public List<SMTPSetting> Props { get; set; }
        public List<PropertiesAdmin> Admins { get; set; }


        public TemplateLetter()
        {
            Props = new List<SMTPSetting>();
            Admins = new List<PropertiesAdmin>();
        }
    }

    public sealed class PropertiesAdmin
    {
        public string ID { get; set; }
        public string Descr { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }


    public sealed class SMTPSetting
    {
        public int Port { get; set; }
        public int TimeOut { get; set; }
        public string SMTP { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public sealed class RadioTechSettings
    {
        public int    NumTech { get; set; }
        public string IT_MENU { get; set; }
        public string TABLE { get; set; }
        public string PAGE_L1 { get; set; }
        public string PAGE_L2 { get; set; }
        public string PAGE_L3 { get; set; }
        public string RadioTech { get; set; }
        public int    WorkFlow { get; set; }
        public int    SubType { get; set; }
    }


    /// <summary>
    /// Вспомогательный класс
    /// </summary>
    public class cbItem
    {
        public string status;
        public string email;
        public int owner_id;
        public string radiotech;
        public string val;
        public string key;
        public string description;

        public override string ToString()
        {
            return val;
        }
    }


}
