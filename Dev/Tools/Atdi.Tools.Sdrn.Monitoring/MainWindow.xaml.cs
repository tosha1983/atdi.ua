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
using Newtonsoft.Json.Linq;

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
            try
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

                foreach (var endpointUrl in endpointUrls)
                {
                    var twEndPointItem = new TreeViewItem();
                    twEndPointItem.Header = endpointUrl.Key + "(" + endpointUrl.Value + ")";
                    twEndPointItem.Tag = endpointUrl.Key;
                    twEndPointItem.FontWeight = FontWeights.Bold;
                    twEndPointItem.IsExpanded = true;

                    using (var wc = new HttpClient())
                    {
                        // 1.
                        try
                        {
                            var responseHost = wc.GetAsync(endpointUrl.Value + "/api/Host/Info").Result;
                            if (responseHost.StatusCode == HttpStatusCode.OK)
                            {
                                var twConfigItem = new TreeViewItem();
                                twConfigItem.Header = "Config";
                                twConfigItem.Tag = TAG_CONFIG;
                                twConfigItem.IsExpanded = true;
                                twConfigItem.Foreground = Brushes.Green;
                                twEndPointItem.Items.Add(twConfigItem);
                            }
                        }
                        catch (Exception)
                        {
                            twEndPointItem.Header = twEndPointItem.Header + " - unavailable";
                            twEndPointItem.Foreground = Brushes.Gray;
                            mainTree.Items.Add(twEndPointItem);
                            continue;
                        }

                        // 2.
                        var responseSdrnConfig = wc.GetAsync(endpointUrl.Value + "/api/SdrnServer/Config").Result;
                        if (responseSdrnConfig.StatusCode == HttpStatusCode.OK)
                        {
                            var twConfigItem = new TreeViewItem();
                            twConfigItem.IsExpanded = true;
                            var config = JsonConvert.DeserializeObject<SdrnServerConfigRequestResult>(responseSdrnConfig.Content.ReadAsStringAsync().Result);
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "Instance: " + config.ServerInstance, IsExpanded = true, FontWeight = FontWeights.Normal });
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "License Number: " + config.LicenseNumber, IsExpanded = true, FontWeight = FontWeights.Normal });
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "License StartDate: " + config.LicenseStartDate.ToString(), IsExpanded = true, FontWeight = FontWeights.Normal });
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "License StopDate: " + config.LicenseStopDate.ToString(), IsExpanded = true, FontWeight = FontWeights.Normal });
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "Server Roles: " + DecodeServerRoles(config.ServerRoles), IsExpanded = true, FontWeight = FontWeights.Normal });
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "MasterServer Instance: " + config.MasterServerInstance, IsExpanded = true, FontWeight = FontWeights.Normal });
                            twConfigItem.Header = "SDRN Server (" + config.ServerInstance + ")";
                            twEndPointItem.Items.Add(twConfigItem);
                        }
                        // 3.
                        var responseSdrnDeviceConfig = wc.GetAsync(endpointUrl.Value + "/api/SdrnDeviceServer/Config").Result;
                        if (responseSdrnDeviceConfig.StatusCode == HttpStatusCode.OK)
                        {
                            var twConfigItem = new TreeViewItem();
                            twConfigItem.IsExpanded = true;
                            var config = JsonConvert.DeserializeObject<DeviceServerConfigRequestResult>(responseSdrnDeviceConfig.Content.ReadAsStringAsync().Result);
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "Instance: " + config.SensorName + " : " + config.SensorTechId, IsExpanded = true, FontWeight = FontWeights.Normal });
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "SdrnServer Instance: " + config.SdrnServerInstance, IsExpanded = true, FontWeight = FontWeights.Normal });
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "Sensor Name: " + config.SensorName, IsExpanded = true, FontWeight = FontWeights.Normal });
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "Sensor TechId: " + config.SensorTechId, IsExpanded = true, FontWeight = FontWeights.Normal });
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "License Number: " + config.LicenseNumber, IsExpanded = true, FontWeight = FontWeights.Normal });
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "License StartDate: " + config.LicenseStartDate.ToString(), IsExpanded = true, FontWeight = FontWeights.Normal });
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "License StopDate: " + config.LicenseStopDate, IsExpanded = true, FontWeight = FontWeights.Normal });
                            twConfigItem.Header = "SDRN Device Server (" + config.SensorName + " : " + config.SensorTechId + ")";
                            twEndPointItem.Items.Add(twConfigItem);
                        }
                        // 4.
                        var responseSdrnDeviceWcfConfig = wc.GetAsync(endpointUrl.Value + "/api/SdrnDeviceWcfService/Config").Result;
                        if (responseSdrnDeviceWcfConfig.StatusCode == HttpStatusCode.OK)
                        {
                            var twConfigItem = new TreeViewItem();
                            twConfigItem.IsExpanded = true;
                            var config = JsonConvert.DeserializeObject<DeviceWcfServerConfigRequestResult>(responseSdrnDeviceWcfConfig.Content.ReadAsStringAsync().Result);
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "Instance: " + config.Instance, IsExpanded = true, FontWeight = FontWeights.Normal });
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "SdrnServer Instance: " + config.SdrnServerInstance, IsExpanded = true, FontWeight = FontWeights.Normal });
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "License Number: " + config.LicenseNumber, IsExpanded = true, FontWeight = FontWeights.Normal });
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "License StartDate: " + config.LicenseStartDate, IsExpanded = true, FontWeight = FontWeights.Normal });
                            twConfigItem.Items.Add(new TreeViewItem() { Header = "License StopDate: " + config.LicenseStopDate.ToString(), IsExpanded = true, FontWeight = FontWeights.Normal });
                            if (config.AllowedSensors.Count > 0)
                            {
                                var twConfigSensorItem = new TreeViewItem() { Header = "AllowedSensors:", IsExpanded = true };
                                foreach (var sensor in config.AllowedSensors)
                                {
                                    twConfigSensorItem.Items.Add(new TreeViewItem() { Header = sensor.Key + ": " + sensor.Value, IsExpanded = true, FontWeight = FontWeights.Normal });
                                }
                                twConfigItem.Items.Add(twConfigSensorItem);
                            }
                            twConfigItem.Header = "SDRN Device WCF Service (" + config.Instance + ")";
                            twEndPointItem.Items.Add(twConfigItem);
                        }
                        // 5.
                        var responseOrm = wc.GetAsync(endpointUrl.Value + "/api/orm/Config").Result;
                        if (responseOrm.StatusCode == HttpStatusCode.OK)
                        {
                            var twOrmItem = new TreeViewItem();
                            twOrmItem.Header = "ORM";
                            twOrmItem.IsExpanded = true;
                            var config = JsonConvert.DeserializeObject<OrmConfigRequestResult>(responseOrm.Content.ReadAsStringAsync().Result);
                            twOrmItem.Items.Add(new TreeViewItem() { Header = "Name: " + config.Name, IsExpanded = true, FontWeight = FontWeights.Normal });
                            twOrmItem.Items.Add(new TreeViewItem() { Header = "Version: " + config.Version, IsExpanded = true, FontWeight = FontWeights.Normal });
                            twOrmItem.Items.Add(new TreeViewItem() { Header = "Namespace: " + config.Namespace, IsExpanded = true, FontWeight = FontWeights.Normal });
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
                            twLogItem.Foreground = Brushes.Blue;
                            twEndPointItem.Items.Add(twLogItem);
                        }
                    }
                    mainTree.Items.Add(twEndPointItem);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private string DecodeServerRoles(int serverRoles)
        {
            var roles = new List<string>();
            if ((serverRoles & 1) == 1)
            {
                roles.Add("SDRN Server");
            }
            if ((serverRoles & 2) == 2)
            {
                roles.Add("Master");
            }
            if ((serverRoles & 4) == 4)
            {
                roles.Add("Aggregation");
            }
            return string.Join(", ", roles.ToArray());
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
            using (var wc = new HttpClient())
            {
                var logeventData = new List<LogEventResult>();
                var response = wc.GetAsync(endpointUrls[endpointKey] + "/api/orm/data/Platform/Atdi.DataModels.Sdrns.Server.Entities.Monitoring/LogEvent?select=Id,Time,Thread,LevelCode,LevelName,Context,Category,Text,Source,Duration,Exception").Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var dicFields = new Dictionary<string, int>();
                    var log = JsonConvert.DeserializeObject<DataSetResult>(response.Content.ReadAsStringAsync().Result);

                    foreach (var field in log.Fields)
                        dicFields[field.Path] = field.Index;

                    foreach (object[] record in log.Records)
                    {
                        var logeventDataRecord = new LogEventResult();

                        Guid.TryParse((string)record[dicFields["Id"]], out Guid Id);
                        TimeSpan.TryParse((string)record[dicFields["Duration"]], out TimeSpan Duration);
                        logeventDataRecord.Id = Id;
                        logeventDataRecord.Time = (DateTime)record[dicFields["Time"]];
                        logeventDataRecord.Thread = Convert.ToInt32(record[dicFields["Thread"]]);
                        logeventDataRecord.LevelCode = Convert.ToInt32(record[dicFields["LevelCode"]]);
                        logeventDataRecord.LevelName = (string)record[dicFields["LevelName"]];
                        logeventDataRecord.Context = (string)record[dicFields["Context"]];
                        logeventDataRecord.Category = (string)record[dicFields["Category"]];
                        logeventDataRecord.Text = (string)record[dicFields["Text"]];
                        logeventDataRecord.Source = (string)record[dicFields["Source"]];
                        logeventDataRecord.Duration = Duration;
                        if (record[dicFields["Exception"]] != null)
                        {
                            var exceptionJObject = record[dicFields["Exception"]] as JObject;
                            logeventDataRecord.Exception = exceptionJObject.ToObject<ExceptionData>();
                        }
                        logeventData.Add(logeventDataRecord);
                    }
                    gridLogEvents.ItemsSource = logeventData;
                }
            }
            gridLogEvents.Visibility = Visibility.Visible;
        }

        private void gridLogEvents_DblClick(object sender, MouseButtonEventArgs e)
        {
            var currentItem = gridLogEvents.CurrentItem as LogEventResult;
            if (currentItem.Exception != null)
            {
                var dlgForm = new DetailLog(currentItem.Exception);
                dlgForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("No data found!");
            }
        }
    }
}