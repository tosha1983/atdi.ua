using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Atdi.DataModels.Sdrn.DeviceServer.Adapters.Config
{
    public class ThisAdapterConfig
    {
        public AdapterMainConfig Main = new AdapterMainConfig();
        private string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private string FilePath;


        public bool GetThisAdapterConfig(string FileName)
        {
            bool res = false;
            this.FilePath = AppPath + "/" + FileName;
            if (File.Exists(FilePath))
            {
                MainConfig_ReadXml();
                res = true;
            }
            else
            {
                Main.Serialize(FilePath);
            }
            return res;
        }
        public void SetThisAdapterConfig(AdapterMainConfig config, string FileName)
        {
            Main = config;
            this.FilePath = AppPath + "/" + FileName;

            Main.Serialize(FilePath);
        }
        public void Save()
        {
            Main.Serialize(FilePath);
        }
        private void MainConfig_ReadXml()
        {
            XmlSerializer ser1 = new XmlSerializer(typeof(AdapterMainConfig));
            TextReader reader2 = new StreamReader(FilePath);
            Main = ser1.Deserialize(reader2) as AdapterMainConfig;
            reader2.Close();
        }
    }
}
