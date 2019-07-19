using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Globalization;
using Newtonsoft.Json;

namespace Atdi.Tools.Sdrn.Monitoring
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string TAG_CONFIG = "Config";
        public const string TAG_LOG_EVENTS = "LogEvents";
        public Dictionary<string, string> endpointUrls = new Dictionary<string, string>();
        public MainWindow()
        {
            foreach (var keySetting in ConfigurationManager.AppSettings.Keys)
            {
                var key = keySetting.ToString();
                if (key.StartsWith("Endpoint"))
                {
                    endpointUrls[key] = ConfigurationManager.AppSettings[key];
                }
            }
            InitializeComponent();

            //System.Windows.MessageBox.Show("!");

            // тут нужно пройтись по каждому ендпоинту, изучить его возможности и сформировать дереов
            //  в корне вывести  Key и Адрес в следующем формате"Endpoint1(http://localhost:15030/appserver/v1)"
            // потом для каждой точки, проверить вызовы
            //  - 1  api/Host/Info - это чтение конфигурации  сервера , если есть ответ создать ноду с именем Config - на ее клик выводить коно с тексовым описанием конфигурации - форма и на ней слошной этит бокс
            //  - 2. проверим api/SdrnServer/Config - если есть ответ - добавить ноду с кепшеном SDRN Srever (InsanceName) - InsanceName взять из вернутого объекта . Также добавить под ноды описывающие остальную информацию - номер лицензии,ю дата оончания
            //  - 3. проверим наличие api/orm/Config, если ест ьответ  создать ноду ORM в ее детализацию вывести ноды с полученнго объекта
            //  - 4. проверит наличие сущности api/orm/metadata/entity/Atdi.DataModels.Sdrns.Server.Entities.Monitoring/LogEvent - если есть ответ , то добавляем ноду Log Events при клик ена которую открывать окнос таблицей лога

            foreach (var endpointUrl in endpointUrls)
            {
                var twEndPointItem = new TreeViewItem();
                twEndPointItem.Header = endpointUrl.Key + "(" + endpointUrl.Value + ")";
                twEndPointItem.Tag = endpointUrl.Key;
                twEndPointItem.IsExpanded = true;

                using (var wc = new HttpClient())
                {
                    // 1.
                    var responseHost = wc.GetAsync(endpointUrl.Value + "/api/Host/Info").Result;
                    if (responseHost.StatusCode == HttpStatusCode.OK)
                    {
                        var twConfigItem = new TreeViewItem();
                        twConfigItem.Header = "Config";
                        twConfigItem.Tag = TAG_CONFIG;
                        twConfigItem.IsExpanded = true;
                        twEndPointItem.Items.Add(twConfigItem);
                    }
                    // 2.
                    var responseSdrnConfig = wc.GetAsync(endpointUrl.Value + "/api/SdrnServer/Config").Result;
                    if (responseSdrnConfig.StatusCode == HttpStatusCode.OK)
                    {
                        var twConfigItem = new TreeViewItem();
                        twConfigItem.IsExpanded = true;
                        var config = JsonConvert.DeserializeObject<SdrnServerConfigRequestResult>(responseSdrnConfig.Content.ReadAsStringAsync().Result);
                        twConfigItem.Items.Add(new TreeViewItem() { Header = "ServerInstance: " + config.ServerInstance, IsExpanded = true });
                        twConfigItem.Items.Add(new TreeViewItem() { Header = "LicenseNumber: " + config.LicenseNumber, IsExpanded = true });
                        twConfigItem.Items.Add(new TreeViewItem() { Header = "LicenseDateStop: " + config.LicenseDateStop.ToString(), IsExpanded = true });
                        twConfigItem.Items.Add(new TreeViewItem() { Header = "ServerRoles: " + config.ServerRoles, IsExpanded = true });
                        twConfigItem.Items.Add(new TreeViewItem() { Header = "MasterServerInstance: " + config.MasterServerInstance, IsExpanded = true });
                        twConfigItem.Header = "SDRN Serever (" + config.ServerInstance + ")";
                        twEndPointItem.Items.Add(twConfigItem);
                    }
                    // 3.
                    var responseSdrnDeviceConfig = wc.GetAsync(endpointUrl.Value + "/api/SdrnDeviceServer/Config").Result;
                    if (responseSdrnDeviceConfig.StatusCode == HttpStatusCode.OK)
                    {
                        var twConfigItem = new TreeViewItem();
                        twConfigItem.IsExpanded = true;
                        var config = JsonConvert.DeserializeObject<DeviceServerConfigRequestResult>(responseSdrnDeviceConfig.Content.ReadAsStringAsync().Result);
                        twConfigItem.Items.Add(new TreeViewItem() { Header = "ServerInstance: " + config.ServerInstance, IsExpanded = true });
                        twConfigItem.Items.Add(new TreeViewItem() { Header = "SensorName: " + config.SensorName, IsExpanded = true });
                        twConfigItem.Items.Add(new TreeViewItem() { Header = "SensorTechId: " + config.SensorTechId, IsExpanded = true });
                        twConfigItem.Items.Add(new TreeViewItem() { Header = "LicenseNumber: " + config.LicenseNumber, IsExpanded = true });
                        twConfigItem.Items.Add(new TreeViewItem() { Header = "LicenseStartDate: " + config.LicenseStartDate.ToString(), IsExpanded = true });
                        twConfigItem.Items.Add(new TreeViewItem() { Header = "LicenseDateStop: " + config.LicenseDateStop, IsExpanded = true });
                        twConfigItem.Header = "SDRN Device Serever (" + config.ServerInstance + ")";
                        twEndPointItem.Items.Add(twConfigItem);
                    }
                    // 4.
                    var responseSdrnDeviceWcfConfig = wc.GetAsync(endpointUrl.Value + "/api/SdrnDeviceWcfService/Config").Result;
                    if (responseSdrnDeviceWcfConfig.StatusCode == HttpStatusCode.OK)
                    {
                        var twConfigItem = new TreeViewItem();
                        twConfigItem.IsExpanded = true;
                        var config = JsonConvert.DeserializeObject<DeviceWcfServerConfigRequestResult>(responseSdrnDeviceWcfConfig.Content.ReadAsStringAsync().Result);
                        twConfigItem.Items.Add(new TreeViewItem() { Header = "Instance: " + config.Instance, IsExpanded = true });
                        twConfigItem.Items.Add(new TreeViewItem() { Header = "SdrnServerInstance: " + config.SdrnServerInstance, IsExpanded = true });
                        twConfigItem.Items.Add(new TreeViewItem() { Header = "LicenseNumber: " + config.LicenseNumber, IsExpanded = true });
                        twConfigItem.Items.Add(new TreeViewItem() { Header = "LicenseStartDate: " + config.LicenseStartDate, IsExpanded = true });
                        twConfigItem.Items.Add(new TreeViewItem() { Header = "LicenseStopDate: " + config.LicenseStopDate.ToString(), IsExpanded = true });
                        if (config.AllowedSensors.Count > 0)
                        {
                            var twConfigSensorItem = new TreeViewItem() { Header = "AllowedSensors:", IsExpanded = true };
                            foreach (var sensor in config.AllowedSensors)
                            {
                                twConfigSensorItem.Items.Add(new TreeViewItem() { Header = sensor.Key + ": " + sensor.Value, IsExpanded = true });
                            }
                            twConfigItem.Items.Add(twConfigSensorItem);
                        }
                        twConfigItem.Header = "SDRN Device WCF Serever (" + config.Instance + ")";
                        twEndPointItem.Items.Add(twConfigItem);
                    }
                    // 5.
                    var responseOrm = wc.GetAsync(endpointUrl.Value + "/api/orm/Config").Result;
                    if (responseOrm.StatusCode == HttpStatusCode.OK)
                    {
                        var twOrmItem = new TreeViewItem();
                        twOrmItem.Header = "Orm";
                        twOrmItem.IsExpanded = true;
                        var config = JsonConvert.DeserializeObject<OrmConfigRequestResult>(responseOrm.Content.ReadAsStringAsync().Result);
                        twOrmItem.Items.Add(new TreeViewItem() { Header = "Name: " + config.Name, IsExpanded = true });
                        twOrmItem.Items.Add(new TreeViewItem() { Header = "Version: " + config.Version, IsExpanded = true });
                        twOrmItem.Items.Add(new TreeViewItem() { Header = "Namespace: " + config.Namespace, IsExpanded = true });
                        twEndPointItem.Items.Add(twOrmItem);
                    }
                    // 6.
                    var responseLog = wc.GetAsync(endpointUrl.Value + "/api/orm/metadata/entity/Atdi.DataModels.Sdrns.Server.Entities.Monitoring/LogEvent").Result;
                    if (responseLog.StatusCode == HttpStatusCode.OK)
                    {
                        var twLogItem = new TreeViewItem();
                        twLogItem.Header = "Log Events";
                        twLogItem.Tag = TAG_LOG_EVENTS;
                        twLogItem.IsExpanded = true;
                        twEndPointItem.Items.Add(twLogItem);
                    }
                }
                mainTree.Items.Add(twEndPointItem);
            }
        }
        private void mainTree_SelectedItemChange(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem)
            {
                this.HideAllElements();
                TreeViewItem item = e.NewValue as TreeViewItem;
                if (item.Tag != null && !string.IsNullOrEmpty(item.Tag.ToString()))
                {
                    if (item.Tag.ToString() == TAG_CONFIG)
                        this.ShowConfig((item.Parent as TreeViewItem).Tag.ToString());
                    if (item.Tag.ToString() == TAG_LOG_EVENTS)
                        this.ShowLogEvents((item.Parent as TreeViewItem).Tag.ToString());
                }
            }
        }
        private void HideAllElements()
        {
            configTree.Visibility = Visibility.Hidden;
            gridLogEvents.Visibility = Visibility.Hidden;
        }
        private void ShowConfig(string endpointKey)
        {
            configTree.Items.Clear();
            using (var wc = new HttpClient())
            {
                var response = wc.GetAsync(endpointUrls[endpointKey] + "/api/Host/Info").Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var config = JsonConvert.DeserializeObject<HostInfo>(response.Content.ReadAsStringAsync().Result);
                    var instanceServerItem = new TreeViewItem();
                    instanceServerItem.Header = config.Instance;
                    instanceServerItem.IsExpanded = true;
                    foreach (var component in config.Components)
                    {
                        //instanceItem.Items.Add(new TreeViewItem() { Header = "Instance: " + component.Instance, FontSize = 15, IsExpanded = true });
                        var inctanceItem = new TreeViewItem() { Header = "Instance: " + component.Instance, IsExpanded = true };
                        inctanceItem.Items.Add(new TreeViewItem() { Header = "Type: " + component.Type, IsExpanded = true });
                        inctanceItem.Items.Add(new TreeViewItem() { Header = "Assembly: " + component.Assembly, IsExpanded = true });
                        if (component.Parameters != null && component.Parameters.Length > 0)
                        {
                            var componentParametersItem = new TreeViewItem() { Header = "Parameters:", IsExpanded = true };
                            foreach (ComponentConfigParameter parameter in component.Parameters)
                            {
                                componentParametersItem.Items.Add(new TreeViewItem() { Header = parameter.Name + ": " + parameter.Value, IsExpanded = true });
                            }
                            inctanceItem.Items.Add(componentParametersItem);
                        }
                        instanceServerItem.Items.Add(inctanceItem);
                    }
                    configTree.Items.Add(instanceServerItem);
                    configTree.Visibility = Visibility.Visible;
                }
            }
        }
        private void ShowLogEvents(string endpointKey)
        {






            gridLogEvents.Visibility = Visibility.Visible;
        }
    }
    public class OrmConfigRequestResult
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Namespace { get; set; }
    }
    public class SdrnServerConfigRequestResult
    {
        public string ServerInstance { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseDateStop { get; set; }
        public string ServerRoles { get; set; }
        public string MasterServerInstance { get; set; }
    }
    public class DeviceServerConfigRequestResult
    {
        public string ServerInstance { get; set; }
        public string SensorName { get; set; }
        public string SensorTechId { get; set; }
        public string LicenseNumber { get; set; } 
        public DateTime LicenseDateStop { get; set; } 
        public DateTime LicenseStartDate { get; set; }
    }
    public class DeviceWcfServerConfigRequestResult
    {
        public string Instance { get; set; } 
        public string SdrnServerInstance { get; set; } 
        public string LicenseNumber { get; set; }
        public DateTime LicenseStopDate { get; set; }
        public DateTime LicenseStartDate { get; set; }
        public Dictionary<string, string> AllowedSensors { get; set; } 
    }
}


//    //var json = wc.DownloadString(endpointUrl.Value + "/api/orm/data/SDRN_Server_DB/Atdi.DataModels.Sdrns.Server.Entities/Emitting?select=Id,StartFrequency_MHz,StopFrequency_MHz,CurentPower_dBm");
//var json = wc.DownloadString("http://localhost:15030/appserver/v1/api/orm/data/SDRN_Server_DB/Atdi.DataModels.Sdrns.Server.Entities/Emitting?select=Id,StartFrequency_MHz,StopFrequency_MHz,CurentPower_dBm,ReferenceLevel_dBm,MeanDeviationFromReference,TriggerDeviationFromReference,RollOffFactor,StandardBW,LevelsDistributionLvl,LevelsDistributionCount,SensorId,StationID,StationTableName,Loss_dB,Freq_kHz");
//var json = wc.DownloadString("http://localhost:15030/appserver/v1/api/orm/data/SDRN_Server_DB/Atdi.DataModels.Sdrns.Server.Entities/Emitting?select=Id,StartFrequency_MHz,StopFrequency_MHz,CurentPower_dBm");

//var json = wc.DownloadString("http://localhost:15030/appserver/v1/api/orm/data/SDRN_Server_DB/Atdi.DataModels.Sdrns.Server.Entities/Emitting?select=Id,StartFrequency_MHz,StopFrequency_MHz,CurentPower_dBm");


//var emitting = JsonConvert.DeserializeObject<DataSetResult>(json);
//MessageBox.Show("!");
