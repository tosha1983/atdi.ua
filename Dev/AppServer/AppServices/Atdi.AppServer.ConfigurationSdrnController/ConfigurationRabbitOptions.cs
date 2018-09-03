using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RabbitMQ;
using RabbitMQ.Client;
using Atdi.DataModels.Sdrns.Device;
using RabbitMQ.Client.Events;
using Castle.Windsor;

namespace Atdi.AppServer.ConfigurationSdrnController
{
    public class ConfigurationRabbitOptions : IDisposable
    {
        private bool disposedValue = false; 
        public ConnectionFactory factory { get; set; }
        public IConnection _connection { get; set; }
        public static Dictionary<IModel,RabbitOptions> listRabbitOptions { get; set; }
        public List<ConcumerDescribe> listConcumerDescribe { get; set; }
        public Task[] tasks { get; set; }
        public string RabbitHostName { get; set; }
        public string RabbitUserName { get; set; }
        public string RabbitPassword { get; set; }
        public string NameServer { get; set; }
        public string ExchangePointFromDevices { get; set; }
        public string ExchangePointFromServer { get; set; }
        public string StartNameQueueServer { get; set; }
        public string StartNameQueueDevice { get; set; }
        public string ConcumerDescribe { get; set; }
        public static string apiVersion  { get; set; }
    private IWindsorContainer _container { get; set; }
        private ILogger _logger { get; set; }


        public ConfigurationRabbitOptions(IWindsorContainer container, ILogger logger)
        {
            try
            {
                _logger = logger;
                _container = container;
                listConcumerDescribe = new List<AppServer.ConfigurationSdrnController.ConcumerDescribe>();
                listRabbitOptions = new Dictionary<IModel, RabbitOptions>();
                RabbitHostName = ConfigurationManager.AppSettings["RabbitHostName"];
                RabbitUserName = ConfigurationManager.AppSettings["RabbitUserName"];
                RabbitPassword = ConfigurationManager.AppSettings["RabbitPassword"];
                NameServer = ConfigurationManager.AppSettings["NameServer"];
                ExchangePointFromDevices = ConfigurationManager.AppSettings["ExchangePointFromDevices"];
                ExchangePointFromServer = ConfigurationManager.AppSettings["ExchangePointFromServer"];
                StartNameQueueServer = ConfigurationManager.AppSettings["StartNameQueueServer"];
                StartNameQueueDevice = ConfigurationManager.AppSettings["StartNameQueueDevice"];
                ConcumerDescribe = ConfigurationManager.AppSettings["ConcumerDescribe"];
                apiVersion = ConfigurationManager.AppSettings["ApiVersion"];
                string[] val = ConcumerDescribe.Split(new char[] { '}' });
                foreach (string v in val)
                {
                    string k = v.Replace("{", "").Replace("}", "");
                    string[] vl = k.Split(new char[] { ';' });
                    ConcumerDescribe descr = new AppServer.ConfigurationSdrnController.ConcumerDescribe();
                    for (int x = 0; x < vl.Length; x++)
                    {
                        if (x == 0)
                        {
                            descr.NameConcumer = vl[x].Replace("Name=", "").Replace("Name =", "").TrimEnd().TrimStart();
                        }
                        if (x == 1)
                        {
                            string concumerCount = vl[x].Replace("ConcumerCount=", "").Replace("ConcumerCount =", "").TrimEnd().TrimStart();
                            int concumerCountValue = 1;
                            int.TryParse(concumerCount, out concumerCountValue);
                            descr.ConcumerCount = concumerCountValue;
                            listConcumerDescribe.Add(descr);
                        }

                    }
                }
                tasks = new Task[listConcumerDescribe.Count];
                factory = new ConnectionFactory() { HostName = RabbitHostName, UserName = RabbitUserName, Password = RabbitPassword };
                _connection = factory.CreateConnection($"SDRN service (Activate) #{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        public void CreateChannelsAndQueues(List<Sensor> listSensors)
        {
            try
            {
                if (listConcumerDescribe != null)
                {
                    Dictionary<IModel, RabbitOptions> dictionary = new Dictionary<IModel, RabbitOptions>();
                    foreach (ConcumerDescribe d in listConcumerDescribe)
                    {
                        var channel = _connection.CreateModel();
                        channel.ExchangeDeclare(exchange: ExchangePointFromDevices + ".[" + apiVersion + "]", type: "direct", durable: true);
                        var queueName = $"{StartNameQueueServer}.[{this.NameServer}].[{d.NameConcumer}].[{apiVersion}]";
                        var routingKey = $"{StartNameQueueServer}.[{this.NameServer}].[{d.NameConcumer}]";
                        dictionary.Add(channel, new RabbitOptions(StartNameQueueServer, routingKey, queueName, d.NameConcumer));
                        listRabbitOptions.Add(channel, new RabbitOptions(StartNameQueueServer, routingKey, queueName, d.NameConcumer));
                    }
                    int i = 0;
                    foreach (KeyValuePair<IModel, RabbitOptions> lo in dictionary)
                    {
                        QueueDeclareConcumer(StartNameQueueServer, lo.Value.nameConcumer, lo.Key, ExchangePointFromDevices + ".[" + apiVersion + "]");
                        tasks[i] = Task.Run(() => new System.Threading.Thread(() => this.CreateConsumer(lo.Key, i, dictionary[lo.Key].queue, "Concumer" + i.ToString(), StartNameQueueDevice, ExchangePointFromServer + ".[" + apiVersion + "]", _connection, ExchangePointFromServer, StartNameQueueDevice)).Start());
                        i++;
                    }
                    Task.WaitAll(tasks);
                }

                if (listSensors != null)
                {
                    Dictionary<IModel, RabbitOptions> dictionary = new Dictionary<IModel, RabbitOptions>();
                    foreach (Sensor sensor in listSensors)
                    {
                        var channel = _connection.CreateModel();
                        channel.ExchangeDeclare(exchange: ExchangePointFromServer + ".[" + apiVersion + "]", type: "direct", durable: true);
                        var queueName = $"{StartNameQueueDevice}.[{sensor.Name}].[{sensor.Equipment.TechId}].[{apiVersion}]";
                        var routingKey = $"{StartNameQueueDevice}.[{sensor.Name}].[{sensor.Equipment.TechId}]";
                        dictionary.Add(channel, new RabbitOptions(StartNameQueueDevice, routingKey, queueName, sensor.Name, sensor.Equipment.TechId));
                        listRabbitOptions.Add(channel, new RabbitOptions(StartNameQueueDevice, routingKey, queueName, sensor.Name, sensor.Equipment.TechId));
                    }


                    foreach (KeyValuePair<IModel, RabbitOptions> lo in dictionary)
                    {
                        QueueDeclareDevice(StartNameQueueDevice, lo.Value.NameSensor, lo.Value.TechId, lo.Key, ExchangePointFromServer + ".[" + apiVersion + "]");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

        }

        public void QueueDeclareConcumer(string ExchangePoint, string ConsumersQueue, IModel channel, string Point)
        {
            var queueName = $"{ExchangePoint}.[{this.NameServer}].[{ConsumersQueue}].[{apiVersion}]";
            var routingKey = $"{ExchangePoint}.[{this.NameServer}].[{ConsumersQueue}]";
            channel.QueueDeclare(
                  queue: queueName,
                  durable: true,
                  exclusive: false,
                  autoDelete: false,
                  arguments: null);
            channel.QueueBind(queueName, Point, routingKey);
            
        }

        public static void QueueDeclareDevice(string ExchangePoint, string sensorName, string techId, IModel channel, string Point)
        {
            var queueName = $"{ExchangePoint}.[{sensorName}].[{techId}].[{apiVersion}]";
            var routingKey = $"{ExchangePoint}.[{sensorName}].[{techId}]";
            channel.QueueDeclare(
                  queue: queueName,
                  durable: true,
                  exclusive: false,
                  autoDelete: false,
                  arguments: null);
            channel.QueueBind(queueName, Point, routingKey);
        }


        public void UnBindConcumers()
        {
            try
            {
                foreach (KeyValuePair<IModel, RabbitOptions> opt in listRabbitOptions)
                {
                    opt.Key.QueueUnbind(opt.Value.queue, opt.Value.exchange, opt.Value.routingKey);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }


        public void TryToCloseRabbitObjects()
        {
            if (this._connection != null)
            {
                if (listRabbitOptions != null && listRabbitOptions.Count > 0)
                {
                    foreach (KeyValuePair<IModel, RabbitOptions> ch in listRabbitOptions)
                    {
                        ch.Key.Close();
                        ch.Key.Dispose();
                    }
                    listRabbitOptions.Clear();
                    listRabbitOptions = null;
                }
                foreach (Task t in tasks)
                {
                    t.Dispose();
                }
                tasks = null;
                listConcumerDescribe.Clear();
                listConcumerDescribe = null;
                _connection.Close();
                _connection.Dispose();

            }
        }


        public virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.TryToCloseRabbitObjects();
                }
                disposedValue = true;
            }
        }


        void CreateConsumer(RabbitMQ.Client.IModel channel, int index, string QueueName, string Name, string StartName, string ExchangePoint, IConnection connection, string ExchangePointFromServer, string StartNameQueueDevice)
        {
            try
            {
                Concumer consumer = new Concumer();
                var rabbitConsumer = new EventingBasicConsumer(channel);
                rabbitConsumer.Received += (model, ea) =>
                {
                    try
                    {
                        consumer.HandleMessage(channel, ea, _container, StartName,  ExchangePoint, connection, ExchangePointFromServer, StartNameQueueDevice);
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e.Message);
                    }

                };
                channel.BasicConsume(QueueName, false, Name, rabbitConsumer);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

      
    }
}
