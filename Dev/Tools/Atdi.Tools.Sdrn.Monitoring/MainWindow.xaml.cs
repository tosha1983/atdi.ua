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
using System.Threading;
using SWF = System.Windows.Forms;

namespace Atdi.Tools.Sdrn.Monitoring
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string TAG_CONFIG = "Config";
        const string TAG_LOG_EVENTS = "LogEvents";
        const string TAG_STATISTICS = "Statistics";
        const string TAG_LOG_HEALTH = "HealthLog";

		Dictionary<string, TreeViewItem> statEntryTreeDic = new Dictionary<string, TreeViewItem>();
        Dictionary<string, TreeViewItem> statCounterTreeDic = new Dictionary<string, TreeViewItem>();
        List<StatisticEntryRecord> statEntrysList = new List<StatisticEntryRecord>();
        List<StatisticCounterRecord> statCurrCounterList = new List<StatisticCounterRecord>();
        List<StatisticCounterRecord> statCounterList = new List<StatisticCounterRecord>();
        public Dictionary<string, string> endpointUrls = new Dictionary<string, string>();

        private readonly Dictionary<string, string> _namespaces = new Dictionary<string, string>();

        public MainWindow()
        {
            //Thread.Sleep(5000);
            InitializeComponent();
            InitializeDataGrids();
            Task.Run((Action)LoadEnvironmentDescription);
        }

        private void UIContext(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);

            
        }

        private void LoadEnvironmentDescription()
        {
            int count = 0;

            try
            {
                UIContext(() =>
                {
                    this.Title = "SDRN Servers Monitoring: Endpoints Loading ...";
                });

                foreach (var keySetting in ConfigurationManager.AppSettings.Keys)
                {
                    var key = keySetting.ToString();
                    if (key.StartsWith("Endpoint"))
                    {
                        endpointUrls[key] = ConfigurationManager.AppSettings[key];
                    }
                }

                

                foreach (var endpointUrl in endpointUrls)
                {
                    TreeViewItem twEndPointItem = null;

                    UIContext(() =>
                    {
                        twEndPointItem = new TreeViewItem();
                        twEndPointItem.Header = endpointUrl.Key + " (" + endpointUrl.Value + ")";
                        twEndPointItem.Tag = endpointUrl.Key;
                        twEndPointItem.FontWeight = FontWeights.Bold;
                        twEndPointItem.IsExpanded = true;
                    });

                    while(twEndPointItem == null)
                    {
                        Thread.Sleep(100);
                    }
                    

                    using (var wc = new HttpClient())
                    {
                        // 1.
                        try
                        {
                            UIContext(() =>
                            {
                                this.Title = $"SDRN Servers Monitoring: Endpoints Loading - Getting from '{endpointUrl.Value + "/api/Host/Info"}'. Please wait a moment ...";
                            });
                            var responseHost = wc.GetAsync(endpointUrl.Value + "/api/Host/Info").Result;
                            if (responseHost.StatusCode == HttpStatusCode.OK)
                            {
                                UIContext(() =>
                                {
                                    var twConfigItem = new TreeViewItem();
                                    twConfigItem.Header = "Config";
                                    twConfigItem.Tag = TAG_CONFIG;
                                    twConfigItem.IsExpanded = true;
                                    twConfigItem.Foreground = Brushes.Green;
                                    twEndPointItem.Items.Add(twConfigItem);
                                });

                                ++count;
                            }
                        }
                        catch (Exception)
                        {
                            UIContext(() =>
                            {
                                twEndPointItem.Header = twEndPointItem.Header + " - unavailable";
                                twEndPointItem.Foreground = Brushes.Gray;
                                mainTree.Items.Add(twEndPointItem);
                            });
                            continue;
                        }

                        // 2.
                        var responseSdrnConfig = wc.GetAsync(endpointUrl.Value + "/api/SdrnServer/Config").Result;
                        if (responseSdrnConfig.StatusCode == HttpStatusCode.OK)
                        {
                            var config = JsonConvert.DeserializeObject<SdrnServerConfigRequestResult>(responseSdrnConfig.Content.ReadAsStringAsync().Result);
                            UIContext(() =>
                            {
                                var twConfigItem = new TreeViewItem();
                                twConfigItem.IsExpanded = true;
                                twConfigItem.Items.Add(new TreeViewItem() { Header = "Instance: " + config.ServerInstance, IsExpanded = true, FontWeight = FontWeights.Normal });
                                twConfigItem.Items.Add(new TreeViewItem() { Header = "License Number: " + config.LicenseNumber, IsExpanded = true, FontWeight = FontWeights.Normal });
                                twConfigItem.Items.Add(new TreeViewItem() { Header = "License StartDate: " + config.LicenseStartDate.ToString(), IsExpanded = true, FontWeight = FontWeights.Normal });
                                twConfigItem.Items.Add(new TreeViewItem() { Header = "License StopDate: " + config.LicenseStopDate.ToString(), IsExpanded = true, FontWeight = FontWeights.Normal });
                                twConfigItem.Items.Add(new TreeViewItem() { Header = "Server Roles: " + DecodeServerRoles(config.ServerRoles), IsExpanded = true, FontWeight = FontWeights.Normal });
                                twConfigItem.Items.Add(new TreeViewItem() { Header = "MasterServer Instance: " + config.MasterServerInstance, IsExpanded = true, FontWeight = FontWeights.Normal });
                                twConfigItem.Header = "SDRN Server (" + config.ServerInstance + ")";
                                twEndPointItem.Items.Add(twConfigItem);
                            });
                        }
                        // 3.
                        var responseSdrnDeviceConfig = wc.GetAsync(endpointUrl.Value + "/api/SdrnDeviceServer/Config").Result;
                        if (responseSdrnDeviceConfig.StatusCode == HttpStatusCode.OK)
                        {
                            var config = JsonConvert.DeserializeObject<DeviceServerConfigRequestResult>(responseSdrnDeviceConfig.Content.ReadAsStringAsync().Result);
                            UIContext(() =>
                            {
                                var twConfigItem = new TreeViewItem();
                                twConfigItem.IsExpanded = true;
                                twConfigItem.Items.Add(new TreeViewItem() { Header = "Instance: " + config.SensorName + " : " + config.SensorTechId, IsExpanded = true, FontWeight = FontWeights.Normal });
                                twConfigItem.Items.Add(new TreeViewItem() { Header = "SdrnServer Instance: " + config.SdrnServerInstance, IsExpanded = true, FontWeight = FontWeights.Normal });
                                twConfigItem.Items.Add(new TreeViewItem() { Header = "Sensor Name: " + config.SensorName, IsExpanded = true, FontWeight = FontWeights.Normal });
                                twConfigItem.Items.Add(new TreeViewItem() { Header = "Sensor TechId: " + config.SensorTechId, IsExpanded = true, FontWeight = FontWeights.Normal });
                                twConfigItem.Items.Add(new TreeViewItem() { Header = "License Number: " + config.LicenseNumber, IsExpanded = true, FontWeight = FontWeights.Normal });
                                twConfigItem.Items.Add(new TreeViewItem() { Header = "License StartDate: " + config.LicenseStartDate.ToString(), IsExpanded = true, FontWeight = FontWeights.Normal });
                                twConfigItem.Items.Add(new TreeViewItem() { Header = "License StopDate: " + config.LicenseStopDate, IsExpanded = true, FontWeight = FontWeights.Normal });
                                twConfigItem.Header = "SDRN Device Server (" + config.SensorName + " : " + config.SensorTechId + ")";
                                twEndPointItem.Items.Add(twConfigItem);
                            });
                        }
                        // 4.
                        var responseSdrnDeviceWcfConfig = wc.GetAsync(endpointUrl.Value + "/api/SdrnDeviceWcfService/Config").Result;
                        if (responseSdrnDeviceWcfConfig.StatusCode == HttpStatusCode.OK)
                        {
                            var config = JsonConvert.DeserializeObject<DeviceWcfServerConfigRequestResult>(responseSdrnDeviceWcfConfig.Content.ReadAsStringAsync().Result);
                            UIContext(() =>
                            {
                                var twConfigItem = new TreeViewItem();
                                twConfigItem.IsExpanded = true;
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
                            });
                        }
                        // 5.
                        string ormNamespace = string.Empty;
                        var responseOrm = wc.GetAsync(endpointUrl.Value + "/api/orm/Config").Result;
                        if (responseOrm.StatusCode == HttpStatusCode.OK)
                        {
                            var config = JsonConvert.DeserializeObject<OrmConfigRequestResult>(responseOrm.Content.ReadAsStringAsync().Result);
                            UIContext(() =>
                            {
                                var twOrmItem = new TreeViewItem();
                                twOrmItem.Header = "ORM";
                                twOrmItem.IsExpanded = true;
                                twOrmItem.Items.Add(new TreeViewItem() { Header = "Name: " + config.Name, IsExpanded = true, FontWeight = FontWeights.Normal });
                                twOrmItem.Items.Add(new TreeViewItem() { Header = "Version: " + config.Version, IsExpanded = true, FontWeight = FontWeights.Normal });
                                twOrmItem.Items.Add(new TreeViewItem() { Header = "Namespace: " + config.Namespace, IsExpanded = true, FontWeight = FontWeights.Normal });
                                twEndPointItem.Items.Add(twOrmItem);
                                
                            });
                            ormNamespace = config.Namespace;
                            _namespaces.Add(endpointUrl.Key, ormNamespace);
                        }
                        // 6.
                        var responseLog = wc.GetAsync(endpointUrl.Value + $"/api/orm/metadata/entity/{ormNamespace}.Monitoring/LogEvent").Result;
                        if (responseLog.StatusCode == HttpStatusCode.OK)
                        {
                            UIContext(() =>
                            {
                                var twLogItem = new TreeViewItem();
                                twLogItem.Header = "Log Events";
                                twLogItem.Tag = TAG_LOG_EVENTS;
                                twLogItem.IsExpanded = true;
                                twLogItem.Foreground = Brushes.Blue;
                                twEndPointItem.Items.Add(twLogItem);

                                var twStatisticItem = new TreeViewItem();
                                twStatisticItem.Header = "Statistics";
                                twStatisticItem.Tag = TAG_STATISTICS;
                                twStatisticItem.IsExpanded = true;
                                twStatisticItem.Foreground = Brushes.Blue;
                                twEndPointItem.Items.Add(twStatisticItem);
                            });
                        }

                        // 7. 
                        var responseHealth = wc.GetAsync(endpointUrl.Value + $"/api/orm/metadata/entity/{ormNamespace}/HealthLog").Result;
                        if (responseHealth.StatusCode == HttpStatusCode.OK)
                        {
	                        UIContext(() =>
	                        {
		                        var twHealthItem = new TreeViewItem();
		                        twHealthItem.Header = "Health Log";
		                        twHealthItem.Tag = TAG_LOG_HEALTH;
		                        twHealthItem.IsExpanded = true;
		                        twHealthItem.Foreground = Brushes.Navy;
		                        twEndPointItem.Items.Add(twHealthItem);
	                        });
                        }
					}

                    UIContext(() => 
                    {
                        mainTree.Items.Add(twEndPointItem);
                    });
                    
                }
            }
            catch (Exception e)
            {
                UIContext(() =>
                {
                    MessageBox.Show(e.Message);
                });
            }

            UIContext(() =>
            {
                this.Title = $"SDRN Servers Monitoring: Available Endpoints - {count}";
            });
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
            try
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
                        if (item.Tag.ToString() == TAG_STATISTICS)
                            this.ShowStatistics((item.Parent as TreeViewItem).Tag.ToString());
                        if (item.Tag.ToString() == TAG_LOG_HEALTH)
	                        this.ShowHealthLog((item.Parent as TreeViewItem).Tag.ToString());
					}
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }
        private void HideAllElements()
        {
            configTree.Visibility = Visibility.Hidden;
            gridLogEvents.Visibility = Visibility.Hidden;
            groupStatistics.Visibility = Visibility.Hidden;
            groupHealthLog.Visibility = Visibility.Hidden;

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
                var ormNamespace = _namespaces[endpointKey];
                var response = wc.GetAsync(endpointUrls[endpointKey] + $"/api/orm/data/Platform/{ormNamespace}.Monitoring/LogEvent?select=Id,Time,Thread,LevelCode,LevelName,Context,Category,Text,Source,Duration,Exception").Result;
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
        private void ShowStatistics(string endpointKey)
        {
            statEntryTreeDic.Clear();
            statCounterTreeDic.Clear();
            statEntryKeysTree.Items.Clear();
            statCounterKeysTree.Items.Clear();
            var ormNamespace = _namespaces[endpointKey];
            using (var wc = new HttpClient())
            {
                var response1 = wc.GetAsync(endpointUrls[endpointKey] + $"/api/orm/data/Platform/{ormNamespace}.Monitoring/StatisticEntryKey?select=Name,Type").Result;
                if (response1.StatusCode == HttpStatusCode.OK)
                {
                    var dicFields = new Dictionary<string, int>();
                    var data = JsonConvert.DeserializeObject<DataSetResult>(response1.Content.ReadAsStringAsync().Result);

                    foreach (var field in data.Fields)
                        dicFields[field.Path] = field.Index;

                    foreach (object[] record in data.Records)
                    {
                        var entryKey = new StatisticEntryKey();
                        entryKey.Name = (string)record[dicFields["Name"]];

                        //if (record[dicFields["Type"]] != null)
                        //{
                        //    var typeJObject = record[dicFields["Type"]] as JObject;
                        //    entryKey.Type = typeJObject.ToObject<Type>();
                        //}

                        this.BuildNodeEntryKeys(entryKey.Name);
                    }
                }

                statEntrysList.Clear();
                var response3 = wc.GetAsync(endpointUrls[endpointKey] + $"/api/orm/data/Platform/{ormNamespace}.Monitoring/StatisticEntryRecord?select=Name,Data").Result;
                if (response3.StatusCode == HttpStatusCode.OK)
                {
                    var dicFields = new Dictionary<string, int>();
                    var data = JsonConvert.DeserializeObject<DataSetResult>(response3.Content.ReadAsStringAsync().Result);

                    foreach (var field in data.Fields)
                        dicFields[field.Path] = field.Index;

                    foreach (object[] record in data.Records)
                    {
                        var statRecord = new StatisticEntryRecord();
                        statRecord.Name = (string)record[dicFields["Name"]];
                        statRecord.Data = record[dicFields["Data"]]?.ToString();
                        statEntrysList.Add(statRecord);
                    }
                    gridEntryRecords.ItemsSource = statEntrysList;
                }

                this.RefreshStatistics(endpointKey);
            }
            groupStatistics.Visibility = Visibility.Visible;
        }
        private void RefreshStatistics(string endpointKey)
        {
            var ormNamespace = _namespaces[endpointKey];
            using (var wc = new HttpClient())
            {
                statCounterKeysTree.Items.Clear();
                statCounterTreeDic.Clear();
                var response2 = wc.GetAsync(endpointUrls[endpointKey] + $"/api/orm/data/Platform/{ormNamespace}.Monitoring/StatisticCounterKey?select=Name,Type,Scale").Result;
                if (response2.StatusCode == HttpStatusCode.OK)
                {
                    var dicFields = new Dictionary<string, int>();
                    var data = JsonConvert.DeserializeObject<DataSetResult>(response2.Content.ReadAsStringAsync().Result);

                    foreach (var field in data.Fields)
                        dicFields[field.Path] = field.Index;

                    foreach (object[] record in data.Records)
                    {
                        var counterKey = new StatisticCounterKey();
                        counterKey.Name = (string)record[dicFields["Name"]];
                        counterKey.Scale = (float)Convert.ToDouble(record[dicFields["Scale"]]);

                        //if (record[dicFields["Type"]] != null)
                        //{
                        //    var typeJObject = record[dicFields["Type"]] as JObject;
                        //    entryKey.Type = typeJObject.ToObject<Type>();
                        //}

                        this.BuildNodeCounterKeys(counterKey.Name);
                    }
                }

                statCurrCounterList.Clear();
                var response4 = wc.GetAsync(endpointUrls[endpointKey] + $"/api/orm/data/Platform/{ormNamespace}.Monitoring/StatisticCurrentCounter?select=Name,Time,Data").Result;
                if (response4.StatusCode == HttpStatusCode.OK)
                {
                    var dicFields = new Dictionary<string, int>();
                    var data = JsonConvert.DeserializeObject<DataSetResult>(response4.Content.ReadAsStringAsync().Result);

                    foreach (var field in data.Fields)
                        dicFields[field.Path] = field.Index;

                    foreach (object[] record in data.Records)
                    {
                        var statRecord = new StatisticCounterRecord();
                        statRecord.Name = (string)record[dicFields["Name"]];
                        statRecord.Data = Convert.ToInt64(record[dicFields["Data"]]);
                        statRecord.Time = (DateTime)record[dicFields["Time"]];
                        statCurrCounterList.Add(statRecord);
                    }
                    gridCurrentCounter.ItemsSource = statCurrCounterList;
                }

                statCounterList.Clear();
                var response5 = wc.GetAsync(endpointUrls[endpointKey] + $"/api/orm/data/Platform/{ormNamespace}.Monitoring/StatisticCounterRecord?select=Name,Time,Data").Result;
                if (response5.StatusCode == HttpStatusCode.OK)
                {
                    var dicFields = new Dictionary<string, int>();
                    var data = JsonConvert.DeserializeObject<DataSetResult>(response5.Content.ReadAsStringAsync().Result);

                    foreach (var field in data.Fields)
                        dicFields[field.Path] = field.Index;

                    foreach (object[] record in data.Records)
                    {
                        var statRecord = new StatisticCounterRecord();
                        statRecord.Name = (string)record[dicFields["Name"]];
                        statRecord.Data = Convert.ToInt64(record[dicFields["Data"]]);
                        statRecord.Time = (DateTime)record[dicFields["Time"]];
                        statCounterList.Add(statRecord);
                    }
                    gridCounterRecords.ItemsSource = statCounterList;
                }
            }
        }
        private void BuildNodeEntryKeys(string node)
        {
            var nodes = node.Split('.');
            string path = "";
            string parentpath = "";

            for (int i = 0; i < nodes.Length; i++)
            {
                path = path + (i == 0 ? "" : ".") + nodes[i];
                if (!statEntryTreeDic.ContainsKey(path))
                {
                    var treeItem = new TreeViewItem() { Header = nodes[i], Tag = path, IsExpanded = true };

                    if (string.IsNullOrEmpty(parentpath))
                        statEntryKeysTree.Items.Add(treeItem);
                    else
                        statEntryTreeDic[parentpath].Items.Add(treeItem);

                    statEntryTreeDic[path] = treeItem;
                }
                parentpath = parentpath + (i == 0 ? "" : ".") + nodes[i];
            }
        }
        private void BuildNodeCounterKeys(string node)
        {
            var nodes = node.Split('.');
            string path = "";
            string parentpath = "";

            for (int i = 0; i < nodes.Length; i++)
            {
                path = path + (i == 0 ? "" : ".") + nodes[i];
                if (!statCounterTreeDic.ContainsKey(path))
                {
                    var treeItem = new TreeViewItem() { Header = nodes[i], Tag = path, IsExpanded = true };

                    if (string.IsNullOrEmpty(parentpath))
                        statCounterKeysTree.Items.Add(treeItem);
                    else
                        statCounterTreeDic[parentpath].Items.Add(treeItem);

                    statCounterTreeDic[path] = treeItem;
                }
                parentpath = parentpath + (i == 0 ? "" : ".") + nodes[i];
            }
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
        private void statEntryKeysTree_SelectedItemChange(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem)
            {
                TreeViewItem item = e.NewValue as TreeViewItem;
                if (item.Tag != null && !string.IsNullOrEmpty(item.Tag.ToString()))
                {
                    gridEntryRecords.ItemsSource = statEntrysList.Where(c => c.Name.StartsWith(item.Tag.ToString()));
                }
            }
        }
        private void statCounterKeysTree_SelectedItemChange(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem)
            {
                TreeViewItem item = e.NewValue as TreeViewItem;
                if (item.Tag != null && !string.IsNullOrEmpty(item.Tag.ToString()))
                {
                    gridCurrentCounter.ItemsSource = statCurrCounterList.Where(c => c.Name.StartsWith(item.Tag.ToString()));
                    gridCounterRecords.ItemsSource = statCounterList.Where(c => c.Name.StartsWith(item.Tag.ToString()));
                }
            }
        }
        private void cmdRefreshStat_Click(object sender, RoutedEventArgs e)
        {
            var item = mainTree.SelectedItem as TreeViewItem;
            if (!string.IsNullOrEmpty((item.Parent as TreeViewItem).Tag.ToString()))
                this.RefreshStatistics((item.Parent as TreeViewItem).Tag.ToString());
        }
        private void cmdRefreshMainTree_Click(object sender, RoutedEventArgs e)
        {
            mainTree.Items.Clear();
            endpointUrls.Clear();
            _namespaces.Clear();
            Task.Run((Action)LoadEnvironmentDescription);
        }

        private void ShowHealthLog(string endpointKey)
		{
			using (var wc = new HttpClient())
			{
				var logeventData = new List<HealthLogResult>();
				var ormNamespace = _namespaces[endpointKey];
				var response = wc.GetAsync(endpointUrls[endpointKey] + $"/api/orm/data/SDRN_Server_DB/{ormNamespace}/HealthLog?select=Id,SenderLogId,SenderTypeCode,SenderTypeName,SenderInstance,SenderHost,SourceTypeCode,SourceTypeName,SourceInstance,SourceTechId,SourceHost,EventCode,EventName,EventNote,DispatchTime,ReceivedTime,ForwardedTime&OrderBy=Id desc&Top=1000").Result;
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var dicFields = new Dictionary<string, int>();
					var log = JsonConvert.DeserializeObject<DataSetResult>(response.Content.ReadAsStringAsync().Result);

					foreach (var field in log.Fields)
						dicFields[field.Path] = field.Index;

					foreach (object[] record in log.Records)
					{
						var healthLogRecord = new HealthLogResult();

						healthLogRecord.Id = Convert.ToInt64(record[dicFields["Id"]]);
						healthLogRecord.DispatchTime = (DateTime)record[dicFields["DispatchTime"]];
						healthLogRecord.ReceivedTime = (DateTime)record[dicFields["ReceivedTime"]];
						var forwardedTime = record[dicFields["ForwardedTime"]];
						if (forwardedTime != null)
						{
							healthLogRecord.ForwardedTime = (DateTime)forwardedTime;
						}
						

						healthLogRecord.SenderLogId = Convert.ToInt64(record[dicFields["SenderLogId"]]);
						healthLogRecord.SenderHost = (string)(record[dicFields["SenderHost"]]);
						healthLogRecord.SenderInstance = (string)record[dicFields["SenderInstance"]];
						healthLogRecord.SenderTypeCode = Convert.ToByte(record[dicFields["SenderTypeCode"]]);
						healthLogRecord.SenderTypeName = (string)record[dicFields["SenderTypeName"]];

						healthLogRecord.SourceHost = (string)(record[dicFields["SourceHost"]]);
						healthLogRecord.SourceInstance = (string)record[dicFields["SourceInstance"]];
						healthLogRecord.SourceTechId = (string)record[dicFields["SourceTechId"]];
						healthLogRecord.SourceTypeCode = Convert.ToByte(record[dicFields["SourceTypeCode"]]);
						healthLogRecord.SourceTypeName = (string)record[dicFields["SourceTypeName"]];

						healthLogRecord.EventCode = Convert.ToByte(record[dicFields["EventCode"]]);
						healthLogRecord.EventName = (string)record[dicFields["EventName"]];
						healthLogRecord.EventNote = (string)record[dicFields["EventNote"]];

						logeventData.Add(healthLogRecord);
					}
					gridHealthLog.ItemsSource = logeventData;
				}
			}
			groupHealthLog.Visibility = Visibility.Visible;
		}

		private void gridHealthLog_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine($"gridHealthLog_SelectionChanged: {e.AddedItems.Count}");
			if (e.AddedItems.Count > 0)
			{
				var healthId = (e.AddedItems[0] as HealthLogResult).Id;
				this.ShowHealthDetail(healthId);
				this.ShowHealthContent(healthId);
			}
			
		}

		private void ShowHealthDetail(long healthId)
		{
			var endpointKey = Convert.ToString(((mainTree.SelectedItem as TreeViewItem).Parent as TreeViewItem).Tag);
			using (var wc = new HttpClient())
			{
				var detailResult = new List<HealthLogDetailResult>();
				var ormNamespace = _namespaces[endpointKey];
				var response = wc.GetAsync(endpointUrls[endpointKey] + $"/api/orm/data/SDRN_Server_DB/{ormNamespace}/HealthLogDetail?select=Id,CreatedDate,Message,Note,Source,ThreadId,SiteTypeCode,SiteTypeName,SiteInstance,SiteHost&OrderBy=Id&Top=1000&filter=HEALTH.Id eq {healthId}").Result;
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var dicFields = new Dictionary<string, int>();
					var log = JsonConvert.DeserializeObject<DataSetResult>(response.Content.ReadAsStringAsync().Result);

					foreach (var field in log.Fields)
						dicFields[field.Path] = field.Index;

					foreach (object[] record in log.Records)
					{
						var healthLogRecord = new HealthLogDetailResult();

						healthLogRecord.Id = Convert.ToInt64(record[dicFields["Id"]]);
						healthLogRecord.CreatedDate = (DateTime)record[dicFields["CreatedDate"]];
						healthLogRecord.Message = (string)record[dicFields["Message"]];
						healthLogRecord.Note = (string)record[dicFields["Note"]];
						healthLogRecord.ThreadId = Convert.ToInt32(record[dicFields["ThreadId"]]);
						healthLogRecord.Source = (string)record[dicFields["Source"]];


						healthLogRecord.SiteHost = (string)(record[dicFields["SiteHost"]]);
						healthLogRecord.SiteInstance = (string)record[dicFields["SiteInstance"]];
						healthLogRecord.SiteTypeCode = Convert.ToByte(record[dicFields["SiteTypeCode"]]);
						healthLogRecord.SiteTypeName = (string)record[dicFields["SiteTypeName"]];

						detailResult.Add(healthLogRecord);
					}
					gridHealthDetail.ItemsSource = detailResult;
				}
			}
			
		}

		private void ShowHealthContent(long healthId)
		{
			var endpointKey = Convert.ToString(((mainTree.SelectedItem as TreeViewItem).Parent as TreeViewItem).Tag);
			using (var wc = new HttpClient())
			{
				
				var ormNamespace = _namespaces[endpointKey];
				var response = wc.GetAsync(endpointUrls[endpointKey] + $"/api/orm/data/SDRN_Server_DB/{ormNamespace}/HealthLogData?select=JsonData&filter=Id eq {healthId}").Result;
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var dicFields = new Dictionary<string, int>();
					var log = JsonConvert.DeserializeObject<DataSetResult>(response.Content.ReadAsStringAsync().Result);

					foreach (var field in log.Fields)
						dicFields[field.Path] = field.Index;

					foreach (object[] record in log.Records)
					{
						textHealthContent.Text = FormatJson((string)record[dicFields["JsonData"]]);
					}
					
				}
			}

		}

		private static string FormatJson(string json)
		{
			dynamic parsedJson = JsonConvert.DeserializeObject(json);
			return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
		}
        void InitializeDataGrids()
        {
            PrepareDataGridMenu(gridLogEvents);
            PrepareDataGridMenu(gridHealthLog);
            PrepareDataGridMenu(gridHealthDetail);
            PrepareDataGridMenu(gridCurrentCounter);
            PrepareDataGridMenu(gridCounterRecords);
            PrepareDataGridMenu(gridEntryRecords);
        }
        void PrepareDataGridMenu(DataGrid grd)
        {
            if (grd.ContextMenu == null)
                grd.ContextMenu = new ContextMenu();

            var itemCSV = new MenuItem() { Header = "Save to CSV", Name = "SaveToCSV" };
            itemCSV.Click += DataGrid_MenuClick_SaveToCSV;
            grd.ContextMenu.Items.Add(itemCSV);
        }
        void DataGrid_MenuClick_SaveToCSV(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            var contextMenu = menuItem.Parent as ContextMenu;
            var grid = contextMenu.PlacementTarget as DataGrid;
            SWF.SaveFileDialog sfd = new SWF.SaveFileDialog()
            {
                Filter = "CSV (*.csv)|*.csv",
                FileName = $"DataGrid_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.csv"
            };
            if (sfd.ShowDialog() == SWF.DialogResult.OK)
            {
                int recCount = grid.Items.Count;
                string[] output = new string[recCount + 1];

                var csvRow = new List<string>();
                var columnsBindName = new List<string>();

                foreach (var column in grid.Columns)
                {
                    var columnName = (((column as DataGridTextColumn).Binding as System.Windows.Data.Binding).Path as PropertyPath).Path;
                    columnsBindName.Add(columnName);
                    csvRow.Add(column.Header.ToString());
                }
                output[0] += string.Join(";", csvRow);

                int i = 1;

                if (grid.SelectedItems.Count > 1)
                    foreach (dynamic row in grid.SelectedItems)
                    {
                        csvRow = new List<string>();

                        foreach (var columnName in columnsBindName)
                        {
                            var cellValue = row.GetType().GetProperty(columnName).GetValue(row, null);
                            csvRow.Add(cellValue == null ? "" : $"\"{cellValue.ToString()}\"");
                        }
                        output[i++] += string.Join(";", csvRow);
                    }
                else
                    foreach (dynamic row in grid.Items)
                    {
                        csvRow = new List<string>();

                        foreach (var columnName in columnsBindName)
                        {
                            var cellValue = row.GetType().GetProperty(columnName).GetValue(row, null);
                            csvRow.Add(cellValue == null ? "" : $"\"{cellValue.ToString()}\"");
                        }
                        output[i++] += string.Join(";", csvRow);
                    }

                System.IO.File.WriteAllLines(sfd.FileName, output, System.Text.Encoding.UTF8);
                System.Windows.MessageBox.Show("Your file was generated and its ready for use.");
            }

        }
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }
    }
}