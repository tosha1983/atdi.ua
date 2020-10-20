using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Atdi.Test.Api.Sdrn.Device.BusControllerAPI2_0;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Api.Sdrn.Device.BusController;

using DM = Atdi.DataModels.Sdrns.Device;
using System.Threading;
using System.Diagnostics;

namespace ControlU.DB
{
    public class MyATDI
    {
        public string NameSensor = "Test_Sensor";
        public bool RabbitIsUsed = false;
        IMessagePublisher Publisher;
        IMessageDispatcher Dispatcher;
        IBusGateFactory GateFactory;
        IBusGate Gate;
        
        public void ClosConnection()
        {
            if (RabbitIsUsed)
            {
                //Publisher.Dispose();
                Dispatcher.Deactivate();
                Dispatcher.Dispose();
                Gate.Dispose();
            }
        }
        public void Initialize()
        {
            try
            {
                GateFactory = BusGateFactory.Create();

                var gateConfig = CreateConfig(GateFactory);
                IBusEventObserver busObserver = new MyEventObserver(); // инстанцирование своего объекта
                Gate = GateFactory.CreateGate("MainGate", gateConfig, busObserver);// один на все время           


                Dispatcher = Gate.CreateDispatcher("main");
                Dispatcher.RegistryHandler(new SendMeasTaskHandler(Gate));
                Dispatcher.RegistryHandler(new SendCommandHandler(Gate));
                Dispatcher.RegistryHandler(new SendRegistrationResultHandler(Gate));
                Dispatcher.RegistryHandler(new SendSensorUpdatingResultHandler(Gate));
                Dispatcher.Activate();
                //Publisher = Gate.CreatePublisher("main");
                RabbitIsUsed = true;
            }
            catch (Exception exp)
            {
                RabbitIsUsed = false;
                //MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "MyATDI", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            finally
            {
                try
                {
                    if (RabbitIsUsed)
                    {
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message =
                            ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("MyATDI_ICSCConnected").ToString();
                        }));
                    }
                    else
                    {
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message =
                            ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("MyATDI_ICSCNotConnected").ToString();
                        }));
                    }
                }
                catch { }
            }
        }
        public bool RegisterSensor(string EquipmentTechId)
        {
            bool res = false;
            if (RabbitIsUsed)
            {
                //Пример регистрации сенсора
                // в параметре Name нужно указать Instance лицензии (т.е он должен совпадать с Instance лицензии)
                // в параметре TechId - произвольное значение
                var sensor = new DM.Sensor
                {
                    Antenna = new DM.SensorAntenna
                    {
                        AddLoss = 0
                    },
                    Equipment = new DM.SensorEquipment
                    {
                        TechId = EquipmentTechId
                    },
                    Name = NameSensor,
                    
                };
                try
                {
                    using (Publisher = Gate.CreatePublisher("main"))
                    {
                        var msgTokenRegisterSensor = Publisher.Send("RegisterSensor", sensor);
                        res = true;
                    }
                }
                catch { res = false; }
                finally
                {
                    try
                    {
                        if (res)
                        {
                            // дополнительная обработка после успешной 
                            // отправки сообщения
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {

                                ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = //"Register";
                                ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("SettingsControl_SensorRegistered").ToString().
                                Replace("*SensorName*", EquipmentTechId);// "отправлено";
                            }));
                            // нодо чето запилить из-разряда дата время последней отправки результатов в самих результатах
                            // не критично, хай буде}
                        }
                        else
                        {
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = //"Not Register";
                                ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("SettingsControl_SensorNotRegistered").ToString().
                                Replace("*SensorName*", EquipmentTechId); //"не отправлено";
                            }));
                        }
                    }
                    catch (Exception exp)
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "MyATDI", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }
                }
            }
            return res;
        }
        public void UpdateSensor()
        {
            if (RabbitIsUsed)
            {
                //Пример обновления сенсора
                // в параметре Name нужно указать Instance лицензии (т.е он должен совпадать с Instance лицензии)
                // в параметре TechId - произвольное значение 
                var updatesensor = new DM.Sensor
                {
                    Antenna = new DM.SensorAntenna
                    {
                        AddLoss = 10,
                        Category = "Cat"
                    },
                    Equipment = new DM.SensorEquipment
                    {
                        TechId = App.Sett.ATDIConnection_Settings.Selected.SDRNDeviceSensorTechId
                    },
                    Name = NameSensor

                };
                using (Publisher = Gate.CreatePublisher("main"))
                {
                    try
                    {
                        var msgTokenUpdateSensor = Publisher.Send("UpdateSensor", updatesensor);
                    }
                    catch { }
                }

            }
        }
        public void SendResult(DM.MeasResults res, ref DB.localatdi_result_state_data resinfo)
        {
            if (RabbitIsUsed)
            {
                Debug.WriteLine("буду отправлять");
                bool sended = false;

                try
                {
                    resinfo.DeliveryConfirmation = 0;
                    resinfo.ResultSended = res.Measured;
                    using (Publisher = Gate.CreatePublisher("main"))
                    {
                        IMessageToken msgTokenSendMeasResults = Publisher.Send("SendMeasResults", res);

                        resinfo.DeliveryConfirmation = 1;
                        resinfo.ResultSended = res.Measured;
                    }
                    sended = true;
                }
                catch (Exception exp)
                {
                    sended = false;
                }
                finally
                {
                    try
                    {
                        if (sended)
                        {
                            // дополнительная обработка после успешной 
                            // отправки сообщения
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {

                                ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message =
                                ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("MyATDI_ResultSended").ToString().
                                Replace("*ResultNumber*", res.ResultId).Replace("*TaskNumber*", res.TaskId);// "отправлено";
                            }));
                            // нодо чето запилить из-разряда дата время последней отправки результатов в самих результатах
                            // не критично, хай буде}
                        }
                        else
                        {
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message =
                                ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("MyATDI_ResultNotSent").ToString().
                                Replace("*ResultNumber*", res.ResultId).Replace("*TaskNumber*", res.TaskId); //"не отправлено";
                            }));
                        }
                    }
                    catch (Exception exp)
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "MyATDI", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }
                }
                Debug.WriteLine("Как бы отправил");
            }
        }
        public void GetMeasTask()
        {
            if (RabbitIsUsed)
            {
                //Пример отправки результатов
                var measResult = new DM.MeasTask
                {
                    TaskId = "SomeTask"
                };
                var msgTokenSendMeasResults = Publisher.Send("SendMeasResults", measResult);
            }
        }

        class MyEventObserver : IBusEventObserver
        {
            public void OnEvent(IBusEvent busEvent)
            {
                Debug.WriteLine("Gete:" + $"{busEvent.Created} :  Bus event - {busEvent.Level} - {busEvent.Source} - {busEvent.Context} - '{busEvent.Text}' - '{busEvent.Source}'  - Code:'{busEvent.Code}'");
            }
        }

        void Test1()
        {
            Console.WriteLine($"Press [Enter] to start testing ...");
            Console.ReadLine();

            var gateFactory = BusGateFactory.Create();
            var gate = CreateGate(gateFactory);// один на все время

            Dispatcher = gate.CreateDispatcher("main");
            Dispatcher.RegistryHandler(new SendMeasTaskHandler(gate));
            Dispatcher.RegistryHandler(new SendCommandHandler(gate));
            Dispatcher.RegistryHandler(new SendRegistrationResultHandler(gate));
            Dispatcher.RegistryHandler(new SendSensorUpdatingResultHandler(gate));
            Dispatcher.Activate();



            Publisher = gate.CreatePublisher("main");

            #region RegisterSensor
            //Пример регистрации сенсора
            // в параметре Name нужно указать Instance лицензии (т.е он должен совпадать с Instance лицензии)
            // в параметре TechId - произвольное значение
            var sensor = new DM.Sensor
            {
                Antenna = new DM.SensorAntenna
                {
                    AddLoss = 0
                },
                Equipment = new DM.SensorEquipment
                {
                    TechId = "SomeSensor2"
                },
                Name = NameSensor//"SENSOR-DBD12-A00-1280"

            };
            var msgTokenRegisterSensor = Publisher.Send("RegisterSensor", sensor);
            #endregion RegisterSensor
            #region UpdateSensor
            //Пример обновления сенсора
            // в параметре Name нужно указать Instance лицензии (т.е он должен совпадать с Instance лицензии)
            // в параметре TechId - произвольное значение 
            //
            var updatesensor = new DM.Sensor
            {
                Antenna = new DM.SensorAntenna
                {
                    AddLoss = 10,
                    Category = "Cat"
                },
                Equipment = new DM.SensorEquipment
                {
                    TechId = "SomeSensor2"
                },
                Name = NameSensor//"SENSOR-DBD12-A00-1280"

            };

            var msgTokenUpdateSensor = Publisher.Send("UpdateSensor", updatesensor);
            #endregion UpdateSensor

            #region SendMeasResults
            //Пример отправки результатов
            var measResult = new DM.MeasResults
            {
                TaskId = "SomeTask"
            };
            var msgTokenSendMeasResults = Publisher.Send("SendMeasResults", measResult);
            #endregion SendMeasResults

            #region
            //Пример отправки объекта Entity
            var entity = new DM.Entity
            {
                Content = new byte[2000],   // содержимое сообщения
                ContentType = "ContentType",//тип содержимого    
                Description = "Descr",      //вспомогательное описание
                Encoding = "UTF-8",         //кодировка
                EntityId = "IDENT01",       // идентификатор Entity
                EOF = false,                // признак последней части сообщения
                HashAlgorithm = "RSA",      // алгоритм шифрования
                HashCode = "Code",          // некоторый HashCode
                Name = "Name",              // Наименование объекта
                ParentId = "ParentId",      // id группы сообщений (для отслеживания все частей)
                ParentType = "ParentType",  // тип группы сообщений (для отслеживания все частей)
                PartIndex = 1 //номер части сообщения 
            };
            var msgTokenSendentity = Publisher.Send("SendEntity", measResult);

            //Пример отправки объекта EntityPart
            var pertEntity = new DM.EntityPart
            {
                Content = new byte[2000],  // содержимое сообщения
                EntityId = "EntityId",     // идентификатор Entity
                PartIndex = 2,             //номер части сообщения 
                EOF = false                // признак последней части сообщения
            };
            var msgTokenSendEntityPart = Publisher.Send("SendEntityPart", pertEntity);
            #endregion
            Publisher.Dispose();

            //********** ВАЖНО ****************
            // деактивацию и закрытие диспетчера (прослушивателя сообщений на шине) следует выполнять только при завершении работы приложения иначе сообщения из очереди не извлекаются
            //т.е. фрагмент кода, который представлен ниже работать не будет если по ходу выполнения программы будет:
            //dispatcher.RegistryHandler(new SendMeasTaskHandler(gate));
            //dispatcher.RegistryHandler(new SendCommandHandler(gate));
            //dispatcher.RegistryHandler(new SendRegistrationResultHandler(gate));
            //dispatcher.RegistryHandler(new SendSensorUpdatingResultHandler(gate));
            //dispatcher.Deactivate();
            //dispatcher.Dispose();
            //********** ВАЖНО ****************
            Console.ReadLine();
        }

        private IBusGate CreateGate(IBusGateFactory gateFactory)
        {
            var gateConfig = CreateConfig(gateFactory);
            var gate = gateFactory.CreateGate("MainGate", gateConfig);
            return gate;
        }

        private IBusGateConfig CreateConfig(IBusGateFactory gateFactory)
        {
            var config = gateFactory.CreateConfig();

            string rootPath = System.IO.Path.GetPathRoot(Environment.CurrentDirectory);
            config["License.FileName"] = "license.lic";
            config["License.OwnerId"] = App.Sett.ATDIConnection_Settings.Selected.LicenseOwnerId;// "OID-BD12-A00-N00";//  OwnerId лицензии
            config["License.ProductKey"] = App.Sett.ATDIConnection_Settings.Selected.LicenseProductKey;// "0ZB0-DVZR-ATI1-WIHB-NC1B";// ProductKey лицензии

            config["RabbitMQ.Host"] = App.Sett.ATDIConnection_Settings.Selected.RabbitMQHost;// "109.237.91.29";
            config["RabbitMQ.VirtualHost"] = App.Sett.ATDIConnection_Settings.Selected.RabbitMQVirtualHost;// "177-A-SBD12-A00-1589.DevicesBus";
            config["RabbitMQ.User"] = App.Sett.ATDIConnection_Settings.Selected.RabbitMQUser;//"SDR_Client"; 
            config["RabbitMQ.Password"] = App.Sett.ATDIConnection_Settings.Selected.RabbitMQPassword;//"32Xr567";
            config["RabbitMQ.Port"] = App.Sett.ATDIConnection_Settings.Selected.RabbitMQPort;//"5674"; 

            config["SDRN.ApiVersion"] = App.Sett.ATDIConnection_Settings.Selected.SDRNApiVersion;//"2.0"; // версия АПИ - не менять
            config["SDRN.Server.Instance"] = App.Sett.ATDIConnection_Settings.Selected.SDRNServerInstance;//"SDRNSV-SBD12-A00-1589"; 
            config["SDRN.Server.QueueNamePart"] = App.Sett.ATDIConnection_Settings.Selected.SDRNServerQueueNamePart;//"Q.SDRN.Server";
            config["SDRN.Device.SensorTechId"] = App.Sett.ATDIConnection_Settings.Selected.SDRNDeviceSensorTechId;//"MMS-09";
            config["SDRN.Device.Exchange"] = App.Sett.ATDIConnection_Settings.Selected.SDRNDeviceExchange;//"EX.SDRN.Device";
            config["SDRN.Device.QueueNamePart"] = App.Sett.ATDIConnection_Settings.Selected.SDRNDeviceQueueNamePart;//"Q.SDRN.Device";
            config["SDRN.Device.MessagesBindings"] = App.Sett.ATDIConnection_Settings.Selected.SDRNDeviceMessagesBindings;//"{messageType=RegisterSensor, routingKey=#01};{messageType=SendCommandResult, routingKey=#02};{messageType=SendMeasResults, routingKey=#03};{messageType=SendEntity, routingKey=#04};{messageType=SendEntityPart, routingKey=#05};{messageType=UpdateSensor, routingKey=#06}";
            config["SDRN.MessageConvertor.UseEncryption"] = App.Sett.ATDIConnection_Settings.Selected.SDRNMessageConvertorUseEncryption;//"false";
            config["SDRN.MessageConvertor.UseCompression"] = App.Sett.ATDIConnection_Settings.Selected.SDRNMessageConvertorUseCompression;//"false";

            config["DeviceBus.SharedSecretKey"] = App.Sett.ATDIConnection_Settings.Selected.DeviceBusSharedSecretKey;//"sbfsbfufbsb";
            config["DeviceBus.Client"] = App.Sett.ATDIConnection_Settings.Selected.DeviceBusClient;//"Test Client 1";



            config["DeviceBus.ContentType"] = App.Sett.ATDIConnection_Settings.Selected.DeviceBusContentType;//"Json";
            config["DeviceBus.Outbox.UseBuffer"] = App.Sett.ATDIConnection_Settings.Selected.DeviceBusOutboxUseBuffer;//"FileSystem";
            config["DeviceBus.Inbox.UseBuffer"] = App.Sett.ATDIConnection_Settings.Selected.DeviceBusInboxUseBuffer;//"FileSystem";           
            if (!System.IO.Directory.Exists(App.Sett.ATDIConnection_Settings.Selected.DeviceBusOutboxBufferFolder))
            {
                System.IO.Directory.CreateDirectory(App.Sett.ATDIConnection_Settings.Selected.DeviceBusOutboxBufferFolder);
            }
            config["DeviceBus.Outbox.Buffer.Folder"] = App.Sett.ATDIConnection_Settings.Selected.DeviceBusOutboxBufferFolder;//"c:\\ATDI\\APIServices\\OutBox";
            if (!System.IO.Directory.Exists(App.Sett.ATDIConnection_Settings.Selected.DeviceBusInboxBufferFolder))
            {
                System.IO.Directory.CreateDirectory(App.Sett.ATDIConnection_Settings.Selected.DeviceBusInboxBufferFolder);
            }
            config["DeviceBus.Inbox.Buffer.Folder"] = App.Sett.ATDIConnection_Settings.Selected.DeviceBusInboxBufferFolder;//"c:\\ATDI\\APIServices\\InBox";
            config["DeviceBus.Outbox.Buffer.ContentType"] = App.Sett.ATDIConnection_Settings.Selected.DeviceBusOutboxBufferContentType;//"Binary";
            config["DeviceBus.Inbox.Buffer.ContentType"] = App.Sett.ATDIConnection_Settings.Selected.DeviceBusInboxBufferContentType;//"Binary";

            //string rootpath = System.IO.Path.GetPathRoot(Environment.CurrentDirectory);
            //if (!System.IO.Directory.Exists(rootpath + "ATDI\\APIServices\\OutBox"))
            //{
            //    System.IO.Directory.CreateDirectory(rootpath + "ATDI\\APIServices\\OutBox");
            //}
            //if (!System.IO.Directory.Exists(rootpath + "ATDI\\APIServices\\InBox"))
            //{
            //    System.IO.Directory.CreateDirectory(rootpath + "ATDI\\APIServices\\InBox");
            //}


            ////config["License.FileName"] = "license.lic"; //"LIC-DBD12-A00-187.SENSOR-DBD12-A00-1280.lic";  // наименование файла лицензии
            ////config["License.OwnerId"] = ControlU.MainWindow.db_v2.ATDIConnectionData_Selsected.owner_id;// "OID-BD12-A00-N00";//  OwnerId лицензии
            ////config["License.ProductKey"] = ControlU.MainWindow.db_v2.ATDIConnectionData_Selsected.product_key;// "0ZB0-DVZR-ATI1-WIHB-NC1B";// ProductKey лицензии
            ////config["RabbitMQ.Host"] = ControlU.MainWindow.db_v2.ATDIConnectionData_Selsected.rabbit_host_name;// "109.237.91.29";//  IP RabbitMQ
            ////config["RabbitMQ.VirtualHost"] = ControlU.MainWindow.db_v2.ATDIConnectionData_Selsected.rabbit_virtual_host_name;// Прописать в xml настройки
            ////config["RabbitMQ.Port"] = ControlU.MainWindow.db_v2.ATDIConnectionData_Selsected.rabbit_host_port;// Прописать в xml настройки
            ////config["RabbitMQ.User"] = ControlU.MainWindow.db_v2.ATDIConnectionData_Selsected.rabbit_user_name;// "SDR_Client";//  User Name RabbitMQ
            ////config["RabbitMQ.Password"] = ControlU.MainWindow.db_v2.ATDIConnectionData_Selsected.rabbit_password;// "32Xr567";//  Password RabbitMQ
            ////config["SDRN.Device.SensorTechId"] = ControlU.MainWindow.db_v2.ATDIConnectionData_Selsected.sensor_equipment_tech_id;// "SomeSensor"; // здесь произвольное наименование  SensorTechId


            ////config["SDRN.ApiVersion"] = "2.0"; // версия АПИ - не менять
            ////config["SDRN.Server.Instance"] = ControlU.MainWindow.db_v2.ATDIConnectionData_Selsected.server_instance;// "ServerSDRN01"; // здесь имя сервера SDRN (т.е. серверов может быть много) (не менять, а вот фиг)
            ////config["SDRN.Server.QueueNamePart"] = "Q.SDRN.Server";   //- это часть наименования конечной очереди в которую будут приходить сообщения от сенсора сервису SDRN (путь следования такой  EX.SDRN.Device.[v2.0] ->Q.SDRN.Server.[ServerSDRN01].[#01].[v2.0])  - не изменять
            ////config["SDRN.Device.Exchange"] = "EX.SDRN.Device"; // здесь часть имени точки обмена EX.SDRN.Device.[v2.0] - не изменять
            ////config["SDRN.Device.QueueNamePart"] = "Q.SDRN.Device"; // здесь часть имени конечной очереди в которую вервис SDRN отправляет данные сенсору (путь следования такой EX.SDRN.Server.[v2.0] ->  Q.SDRN.Device.[SENSOR-DBD12-A00-1280].[SomeSensor].[v2.0]) - не изменять
            ////config["SDRN.Device.MessagesBindings"] = "{messageType=RegisterSensor, routingKey=#01};{messageType=SendCommandResult, routingKey=#02};{messageType=SendMeasResults, routingKey=#03};{messageType=SendEntity, routingKey=#04};{messageType=SendEntityPart, routingKey=#05};{messageType=UpdateSensor, routingKey=#06}"; // здесь наименования подочередей для обработки сообщений конкретного типа (фактически создаются очереди Q.SDRN.Server.[ServerSDRN01].[#01].[v2.0], Q.SDRN.Server.[ServerSDRN01].[#02].[v2.0], Q.SDRN.Server.[ServerSDRN01].[#03].[v2.0], Q.SDRN.Server.[ServerSDRN01].[#04].[v2.0], Q.SDRN.Server.[ServerSDRN01].[#05].[v2.0], Q.SDRN.Server.[ServerSDRN01].[#06].[v2.0]) - не изменять
            //////config["DeviceBus.Outbox.ContentType"] = "Json";
            //////config["DeviceBus.Outbox.UseBuffer"] = "FileSystem";
            //////config["DeviceBus.Outbox.Buffer.Folder"] = "C:\\Temp\\DeviceBus\\";
            //////config["DeviceBus.Outbox.Buffer.ContentType"] = "Binary";

            ////// Шифрование и компресия сообщений
            ////config["SDRN.MessageConvertor.UseEncryption"] = "false"; //"false";// "true"; //признак шифрования сообщения (на данный момент пока не внедрено в сервис, но в конфигурации должно быть)
            ////config["SDRN.MessageConvertor.UseСompression"] = "false"; //"false";//"true"; //признак упаковки сообщения (на данный момент пока не внедрено в сервис, но в конфигурации должно быть)

            return config;
        }       
    }
    public class LocalMeasSdrTask_v2
    {
        public bool Saved = false;
        public string Command = "";
        public localatdi_meas_task MeasTask;
        public string Errors = ""; 
    }
}
