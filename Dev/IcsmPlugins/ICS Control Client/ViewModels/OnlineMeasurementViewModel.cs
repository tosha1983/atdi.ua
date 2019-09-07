using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Models;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;
using XICSM.ICSControlClient.Models.WcfDataApadters;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using CS = XICSM.ICSControlClient.WpfControls.Charts;
using MP = XICSM.ICSControlClient.WpfControls.Maps;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Windows;
using FRM = System.Windows.Forms;
using FM = XICSM.ICSControlClient.Forms;
using ICSM;
using System.Windows.Controls;
using INP = System.Windows.Input;
using System.Collections;
using System.Globalization;
using System.Timers;
using XICSM.ICSControlClient.Forms;
using System.Threading;
using XICSM.ICSControlClient.OnlineMeasurement;
using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;

namespace XICSM.ICSControlClient.ViewModels
{
    public class OnlineMeasurementViewModel : WpfViewModelBase, IDisposable, IWebSocketMessageHandler
    {
        [Flags]
        private enum MeasProcessStatus
        {
            Disconnected = 0,
            Initiation = 1,
            SensorReady = 2,
            Connected = 4, 
            WaiteSensor = 8,
            IncomingData = 16,
            Cancellation = 32,
            Closing = 64
        }

        private enum MeasurementStatus
        {
            Undefined = 0, // сокет закрыт
            ReadyToRun  = 1, // сокет открыт, измерения не происходят
            TaskSent = 2, // состояние при котором клиант отправил таск но еще не получил параметры обратно
            ReceivedParameters = 3, // получены параметры результатов
            ReadyToAccept = 5, // клиент отпраивл сообщение о готовности принимать результаты
            IncomingData = 6, // поступают данные
            SensorCancellation = 7, // от сервера пришло сообщение об отмене процесса измерения
            Error = 8, // состояние ошибки
            Cancellation = 9
        }

        private readonly SensorViewModel _sensor;
        private OnlienrMeasParametersViewModel _measParameters;
        private MeasProcessStatus _processStatus;
        private MeasurementStatus _measStatus;
        private TimeSpan _measurementPeriod;
        private SDR.OnlineMeasurementInitiationResult _initOptions;
        private SDR.SensorAvailabilityDescriptor _serverDescriptor;
        private WebSocketContext _webSocketContext;
        private WebSocketClient _webSocket;
        private CS.ChartOption _measChartOption;
        private string _logRecords;

        private int _attempts = 20;

        public OnlineMeasurementViewModel(ShortSensorViewModel sensor)
        {
            this._sensor = Mappers.Map(DataStore.GetStore().GetSensorById(sensor.Id));
            this._measParameters = new OnlienrMeasParametersViewModel(this)
            {
                TraceType = TraceType.ClearWhrite,
                TraceCount = 1,
                DetectorType = DetectorType.MaxPeak,
                CurrentStatus = "Disconnected",
                ConnectButtonText = "Connect with Sensor",
                RunButtonText = "Run Online Measurement",
                OnlineMeasType = OnlineMeasType.Level,
                SweepTime_s = -1,
                Att_dB = -1,
                PreAmp_dB = -1,
                RefLevel_dBm = 1000000000,
                FreqStart_MHz = 935,
                FreqStop_MHz = 960
            };

            if (_sensor.Equipment != null)
            {
                if (_sensor.Equipment.LowerFreq.HasValue)
                {
                    this._measParameters.FreqStart_MHz = _sensor.Equipment.LowerFreq.Value;
                }
                if (_sensor.Equipment.UpperFreq.HasValue)
                {
                    this._measParameters.FreqStop_MHz = _sensor.Equipment.UpperFreq.Value;
                }
            }
            this._measParameters.RBW_kHz = 1000.0 * (this._measParameters.FreqStop_MHz - this._measParameters.FreqStart_MHz) / 2000.0;

            this._processStatus = MeasProcessStatus.Disconnected;
            this._measStatus = MeasurementStatus.Undefined;
            this._measChartOption = this.GetDefaultMeasChartOption();
            this._measurementPeriod = new TimeSpan(0, 10, 0);
            this.InitCommands();
        }

        

        public SensorViewModel Sensor
        {
            get => this._sensor;
           // set => this.Set(ref this._sensor, value, () => { });
        }

        public OnlienrMeasParametersViewModel MeasParameters
        {
            get => this._measParameters;
            set => this.Set(ref this._measParameters, value, () => { });
        }

        public CS.ChartOption MeasChartOption
        {
            get => this._measChartOption;
            set => this.Set(ref this._measChartOption, value);
        }

        public string CurrentStatus
        {
            get => this._measParameters.CurrentStatus;
        }

        public string LogRecords
        {
            get => this._logRecords;
            set => this.Set(ref this._logRecords, value);
        }

        public void Dispose()
        {
            if ((this._processStatus & MeasProcessStatus.Connected) == MeasProcessStatus.Connected)
            {
                this.CloseCeonnectionProcess();
            }
        }

        #region Commands

        private void InitCommands()
        {
            this.ConnectDisconnect = new WpfCommand(this.ConnectDisconnectCommandHandler);
            this.RunCancel = new WpfCommand(this.RunCancelCommandHandler);
        }

        public WpfCommand ConnectDisconnect { get; set; }

        public WpfCommand RunCancel { get; set; }

        private void ConnectDisconnectCommandHandler(object parameter)
        {
            if (this._processStatus == MeasProcessStatus.Disconnected)
            {
                this.InitiateMeasurement();
            }
            else if ((this._processStatus & MeasProcessStatus.Connected) == MeasProcessStatus.Connected)
            {
                this.CloseCeonnection();
            }
            else
            {
                this.WriteToLog($"ERROR: Incorrect status (ConnectDisconnectCommandHandler)");
            }
        }

        private void RunCancelCommandHandler(object parameter)
        {
            if (this._measStatus == MeasurementStatus.ReadyToRun)
            {
                this.RunMeasurement();
            }
            else if (this._measStatus == MeasurementStatus.ReadyToAccept || this._measStatus == MeasurementStatus.IncomingData )
            {
                this.CancelMeasurement();
            }
            else
            {
                this.WriteToLog($"ERROR: Incorrect status (RunCancelCommandHandler)");
            }
        }

        private void RunMeasurement()
        {
            _measParameters.IsEnabledRunButton = false;
            Task.Run((Action)this.RunMeasurementProcess);
        }
        private void RunMeasurementProcess()
        {
            try
            {
                this.WriteToLog($"Run a measurement task");

                UIContext(() =>
                {
                    _measParameters.CurrentStatus = "Running";
                    _measParameters.IsReadOnlyProperties = true;
                });

                if (_webSocket == null)
                {
                    throw new InvalidOperationException("The WebSocket was not opened");
                }
                if (_webSocketContext == null)
                {
                    throw new InvalidOperationException("The WebSocket Context was not created");
                }
                if (_serverDescriptor == null)
                {
                    throw new InvalidOperationException("The Server Descriptor Context was not initialized");
                }

                // валидируем состояние переменных
                try
                {
                    _measParameters.ValidateStateModel();
                }
                catch (Exception ex)
                {
                    _measParameters.IsReadOnlyProperties = false;
                    _measParameters.IsEnabledRunButton = true;
                    UIContext(() =>
                    {
                        _measParameters.CurrentStatus = "Invalid input value";
                        _measParameters.IsReadOnlyProperties = false;
                    });
                    MessageBox.Show(ex.Message, "Run Measurement Process", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                this.WriteToLog($"Validated state");

                // пакуем параметры и создаем таск
                var clientTask = new ClientMeasTaskData
                {
                    SensorToken = _serverDescriptor.SensorToken,
                    Att_dB = _measParameters.Att_dB,
                    DetectorType = _measParameters.DetectorType,
                    FreqStart_MHz = _measParameters.FreqStart_MHz,
                    FreqStop_MHz = _measParameters.FreqStop_MHz,
                    OnlineMeasType = _measParameters.OnlineMeasType,
                    PreAmp_dB = _measParameters.PreAmp_dB,
                    RBW_kHz =_measParameters.RBW_kHz,
                    RefLevel_dBm = _measParameters.RefLevel_dBm,
                    SweepTime_s = _measParameters.SweepTime_s,
                    TraceCount = _measParameters.TraceCount,
                    TraceType = _measParameters.TraceType
                };
                _measStatus = MeasurementStatus.TaskSent;
                _webSocketContext.SendAsJson(OnlineMeasMessageKind.ClientTaskRegistration, clientTask);

                this.WriteToLog($"Sent task to the sensor");

                // засыпаем на минуту, если сервре не ответил, генерируем ошибку
                Thread.Sleep(60 * 1000);
                if (this._measStatus == MeasurementStatus.TaskSent)
                {
                    throw new InvalidOperationException("Expired response time with measurement parameters from the sensor");
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RunMeasurementProcess: {ex.ToString()}");
                this.WriteToLog($"ERROR '{ex.ToString()}'");

                UIContext(() =>
                {
                    _measParameters.CurrentStatus = "Has error";
                });
                this._measStatus = MeasurementStatus.Error;

                MessageBox.Show(ex.Message, "Run Measurement Process", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelMeasurement()
        {
            _measParameters.IsEnabledRunButton = false;
            Task.Run((Action)this.CancelMeasurementProcess);
        }
        private void CancelMeasurementProcess()
        {
            try
            {
                this.WriteToLog($"Cancel measurement process");

                if (_webSocket == null)
                {
                    return;
                }
                if (_webSocketContext == null)
                {
                    return;
                }
                if (_serverDescriptor == null)
                {
                    return;
                }

                UIContext(() =>
                {
                    _measParameters.CurrentStatus = "Cancelling";
                    _measParameters.IsReadOnlyProperties = true;
                });
                Thread.Sleep(1000);

                // пакуем параметры и создаем таск
                var clientCancellationData = new ClientTaskCancellationData
                {
                    SensorToken = _serverDescriptor.SensorToken
                };

                while(_measStatus != MeasurementStatus.Cancellation)
                {
                    _measStatus = MeasurementStatus.Cancellation;
                    Thread.SpinWait(10);
                }

                _webSocketContext.SendAsJson(OnlineMeasMessageKind.ClientTaskCancellation, clientCancellationData);

                this.WriteToLog($"Sent task cancellation data to the sensor");

                // засыпаем 3 секунды и меняем статус
                Thread.Sleep(3 * 1000);
                if (this._measStatus == MeasurementStatus.Cancellation)
                {
                    this._measStatus = MeasurementStatus.ReadyToRun;
                }

                UIContext(() =>
                {
                    _measParameters.CurrentStatus = "Connected";
                    // позволим пользователю передавать такс сенсору
                    _measParameters.RunButtonText = "Run Online Measurement";
                    _measParameters.IsEnabledRunButton = true;
                    // делаем парамтеры таска достпными для заполнения пользоватлеме
                    _measParameters.IsReadOnlyProperties = false;
                });
                Thread.Sleep(300);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"InitiateMeasurementProcess: {ex.ToString()}");
                this.WriteToLog($"ERROR '{ex.ToString()}'");

                UIContext(() =>
                {
                    _measParameters.CurrentStatus = "Has error";
                });
                this._measStatus = MeasurementStatus.Error;
            }
        }
        #endregion
        private void CloseCeonnection()
        {
            _measParameters.IsEnabledConnectButton = false;
            Task.Run((Action)this.CloseCeonnectionProcess);
        }
        private void CloseCeonnectionProcess()
        {
            try
            {
                this.WriteToLog($"Close connection process ...");

                this._processStatus = MeasProcessStatus.Closing;
                UIContext(() =>
                {
                    _measParameters.CurrentStatus = "Closing connection";
                });

                // тут анализируем состояние измерения, если идет. отправляем запро сн аотмену 
                if (this._measStatus != MeasurementStatus.Undefined || this._measStatus == MeasurementStatus.SensorCancellation)
                {
                    this.CancelMeasurementProcess();
                }


                // тут закрываем веб сокет 
                this._webSocketContext = null;
                if (this._webSocket != null)
                {
                    this._webSocket.Close();
                    this._webSocket.Dispose();
                    this._webSocket = null;

                    this.WriteToLog($"Closed WebSocket with the sensor");
                }
                this._serverDescriptor = null;
                this._initOptions = null;

                // В этой точке считаем что с сенсором соединение удачно закрыто
                this._processStatus = MeasProcessStatus.Disconnected;
                this._measStatus = MeasurementStatus.Undefined;

                UIContext(() =>
                {
                    _measParameters.CurrentStatus = "Disconnected";

                    // меняем назначение кнопки - теперь она будет иницировать полностью новый процесс измерения.
                    _measParameters.ConnectButtonText = "Connect with Sensor";
                    _measParameters.IsEnabledConnectButton = true;

                    //  пользователю меняем назначение этой кнопки
                    _measParameters.IsEnabledRunButton = false;
                    _measParameters.RunButtonText = "Run Online Measurement";

                    // делаем парамтеры таска не достпными для заполнения пользоватлеме
                    _measParameters.IsReadOnlyProperties = true;
                });

                this.WriteToLog($"Closed connection");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CloseCeonnectionProcess: {ex.ToString()}");
                this.WriteToLog($"ERROR '{ex.ToString()}'");
            }
        }
        private void InitiateMeasurement()
        {
            _measParameters.IsEnabledConnectButton = false;
            Task.Run((Action)this.InitiateMeasurementProcess);
        }

        private void InitiateMeasurementProcess()
        {
            try
            {
                this._measStatus = MeasurementStatus.Undefined;
                this._processStatus = MeasProcessStatus.Initiation;

                this.WriteToLog("Initiating Online Measurement on SDRN Server");

                UIContext(() =>
                {
                    _measParameters.CurrentStatus = "Initiating on SDRN Server";
                });
                

                // тут отправляем запрос иницировани яонлайн измерения
                var initOptions = SVC.SdrnsControllerWcfClient.InitOnlineMeasurement(this._sensor.Id, this._measurementPeriod);
                this.WriteToLog($"Called 'InitOnlineMeasurement': Allowed = '{initOptions.Allowed}', Message = '{initOptions.Message}'");

                if (!initOptions.Allowed)
                {
                    throw new InvalidOperationException($"Sensor not available. Reason: {initOptions.Message}");
                }

                this.WriteToLog("Waiting sensor ...");

                this._initOptions = initOptions;
                this._processStatus = MeasProcessStatus.WaiteSensor;

                this._serverDescriptor = null;
                for (int i = 0; i < _attempts; i++)
                {
                    UIContext(() =>
                    {
                        _measParameters.CurrentStatus = $"Waiting ({1} attempt out of {_attempts} )...";
                        this.WriteToLog($" - {1} attempt out of {_attempts} ...");
                    });

                    // засыпаем на 3 секунды
                    Thread.Sleep(3000);

                    var descriptor = SVC.SdrnsControllerWcfClient.GetSensorAvailabilityForOnlineMesurement(this._initOptions.ServerToken);
                    this.WriteToLog($"Called 'GetSensorAvailabilityForOnlineMesurement': Status = '{descriptor.Status}', Message = '{descriptor.Message}'");

                    if ( descriptor.Status == SDR.OnlineMeasurementStatus.Initiation || descriptor.Status == SDR.OnlineMeasurementStatus.WaitSensor)
                    {
                        continue;
                    }
                    this._serverDescriptor = descriptor;
                    break;
                }
                if (this._serverDescriptor == null)
                {
                    // отмегнеям измерение так как не дождалдись его результатов - пока метода нет - нужно будет добавить
                    throw new InvalidOperationException($"The sensor is not responding");
                }
                if (this._serverDescriptor.Status == SDR.OnlineMeasurementStatus.CanceledBySensor|| this._serverDescriptor.Status == SDR.OnlineMeasurementStatus.DeniedBySensor)
                {
                    throw new InvalidOperationException($"Sensor denied access. Reason: {_serverDescriptor.Message}");
                }
                if (this._serverDescriptor.Status == SDR.OnlineMeasurementStatus.CanceledByServer || this._serverDescriptor.Status == SDR.OnlineMeasurementStatus.DeniedByServer)
                {
                    throw new InvalidOperationException($"SDRN Server denied access. Reason: {_serverDescriptor.Message}");
                }
                if (this._serverDescriptor.Status == SDR.OnlineMeasurementStatus.CanceledByClient)
                {
                    throw new InvalidOperationException($"SDRN Server denied access. Reason: {_serverDescriptor.Message}");
                }
                if (this._serverDescriptor.Status != SDR.OnlineMeasurementStatus.SonsorReady)
                {
                    throw new InvalidOperationException($"SDRN Server denied access. Reason: {_serverDescriptor.Message}");
                }

                this.WriteToLog($"Received response from sensor '{_sensor.Name}': Status = '{this._serverDescriptor.Status}', WebSocketUrl = '{this._serverDescriptor.WebSocketUrl}'");

                this._processStatus = MeasProcessStatus.SensorReady;
                UIContext(() =>
                {
                    _measParameters.CurrentStatus = "Sensor is ready";
                });
                // небольшая пауза что бі  пользователь увидел что сенсор доступен
                Thread.Sleep(3000);


                UIContext(() =>
                {
                    _measParameters.CurrentStatus = "Connecting to Sensor";
                });

                // Открываем соединение с сенсором через вебсокет
                _webSocket = new WebSocketClient(
                        new Uri(this._serverDescriptor.WebSocketUrl),
                        new WebSocketHandler(this)
                    );

                this.WriteToLog($"Opening a websocket with a sensor '{_sensor.Name}': Url = '{this._serverDescriptor.WebSocketUrl}'");
                _webSocketContext = _webSocket.Connect();

                // В этой точке считаем что с сенсором открыто соединение
                this._processStatus = MeasProcessStatus.Connected;
                this._measStatus = MeasurementStatus.ReadyToRun;

                this.WriteToLog($"Opening socket with sensor '{_sensor.Name}' was successful sensor");

                UIContext(() =>
                {
                    _measParameters.CurrentStatus = "Connected";
                    // меняем назначение кнопки - теперь она будет закрывать соединение с сенсором.
                    _measParameters.ConnectButtonText = "Disconnect sensor";
                    _measParameters.IsEnabledConnectButton = true;
                    // позволим пользователю передавать такс сенсору
                    _measParameters.IsEnabledRunButton = true;
                    // делаем парамтеры таска достпными для заполнения пользоватлеме
                    _measParameters.IsReadOnlyProperties = false;
                });

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"InitiateMeasurementProcess: {ex.ToString()}");
                this.WriteToLog($"ERROR '{ex.ToString()}'");

                this._processStatus = MeasProcessStatus.Disconnected;
                this._measStatus = MeasurementStatus.Undefined;
                this._initOptions = null;
                this._serverDescriptor = null;
                this._webSocketContext = null;
                if (this._webSocket != null)
                {
                    this._webSocket.Dispose();
                    this._webSocket = null;
                }

                UIContext(() =>
                {
                    _measParameters.CurrentStatus = "Disconnected";

                    // меняем назначение кнопки - теперь она будет иницировать полностью новый процесс измерения.
                    _measParameters.ConnectButtonText = "Connect with Sensor";
                    _measParameters.IsEnabledConnectButton = true;

                    //  пользователю меняем назначение этой кнопки
                    _measParameters.IsEnabledRunButton = false;
                    _measParameters.RunButtonText = "Run Online Measurement";

                    // делаем парамтеры таска не достпными для заполнения пользоватлеме
                    _measParameters.IsReadOnlyProperties = true;

                    MessageBox.Show(ex.Message, "Initiate Measurement Process", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private void UIContext(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        public void OnDeviceServerParameters(MessageContainer data, WebSocketContext context)
        {
            try
            {
                this.WriteToLog($"Received sensor parameters");

                this._measStatus = MeasurementStatus.ReceivedParameters;

                UIContext(() =>
                {
                    _measParameters.CurrentStatus = "Parameters Received";
                });

                // Пользователь должен увидеть что сенсор прислал параметры
                Thread.Sleep(3000);

                if (_webSocket == null)
                {
                    throw new InvalidOperationException("The WebSocket was not opened");
                }
                if (_webSocketContext == null)
                {
                    throw new InvalidOperationException("The WebSocket Context was not created");
                }
                if (_serverDescriptor == null)
                {
                    throw new InvalidOperationException("The Server Descriptor Context was not initialized");
                }

                // рапаковываем параметры
                if (_measParameters.OnlineMeasType == OnlineMeasType.Level)
                {
                    var parameters = data.GetData<DeviceServerParametersDataLevel>();
                    if (parameters == null)
                    {
                        throw new InvalidOperationException($"Invalid parameters received from the sensor");
                    }
                    if (parameters.isChanged_Att_dB)
                    {
                        _measParameters.Att_dB = parameters.Att_dB;
                    }
                    if (parameters.isChanged_PreAmp_dB)
                    {
                        _measParameters.PreAmp_dB = parameters.PreAmp_dB;
                    }
                    if (parameters.isChanged_RBW_kHz)
                    {
                        _measParameters.RBW_kHz = parameters.RBW_kHz;
                    }
                    if (parameters.isChanged_RefLevel_dBm)
                    {
                        _measParameters.RefLevel_dBm = parameters.RefLevel_dBm;
                    }

                    if (parameters.Freq_Hz != null && parameters.Freq_Hz.Length > 0)
                    {
                        _measParameters.FreqStart_MHz = parameters.Freq_Hz[0];
                        _measParameters.FreqStop_MHz = parameters.Freq_Hz[parameters.Freq_Hz.Length - 1];
                        _measParameters.Freq_Hz = parameters.Freq_Hz;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Incorrect parameters received from the device: no frequency");
                    }
                    var p = _measParameters;
                    this.MeasParameters = null;
                    this.MeasParameters = p;
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported the Online Meas Type '{_measParameters.OnlineMeasType}'");
                }

                // готовим интерфейс к приему данных
                UIContext(() =>
                {
                    this.PrepareUIToAcceptResults();
                });

                // отправляем запрос устройству что мы готов ыпринимать результаты 
                _measStatus = MeasurementStatus.ReadyToAccept;
                var clientReadyData = new ClientReadyData
                {
                    SensorToken = _serverDescriptor.SensorToken
                };
                _webSocketContext.SendAsJson(OnlineMeasMessageKind.ClientReadyTakeMeasResult, clientReadyData);

                UIContext(() =>
                {
                    _measParameters.RunButtonText = "Stop Online Measurement";
                    _measParameters.IsEnabledRunButton = true;
                });

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnDeviceServerParameters: {ex.ToString()}");
                this.WriteToLog($"ERROR '{ex.ToString()}'");

                UIContext(() =>
                {
                    _measParameters.CurrentStatus = "Has error";
                });
                this._measStatus = MeasurementStatus.Error;
                MessageBox.Show(ex.Message, "Parameters received from the sensor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private CS.ChartOption GetDefaultMeasChartOption()
        {
            return new CS.ChartOption
            {
                ChartType = CS.ChartType.Line,
                XLabel = "Freq (Mhz)",
                XMin = 900,
                XMax = 960,
                XTick = 5,
                XInnerTickCount = 5,
                YLabel = "Level (dBm)",
                YMin = -120,
                YMax = -10,
                YTick = 10,
                YInnerTickCount = 5,
                Title = "Online Measurements"
            };
        }

        private void PrepareUIToAcceptResults()
        {
            var option = GetDefaultMeasChartOption();
            option.XMin = _measParameters.FreqStart_MHz; //900,
            option.XMax = _measParameters.FreqStop_MHz; // 960,

            this.MeasChartOption = option;
        }
        private void AcceptNextResults(DeviceServerResultLevel serverResult)
        {
            var option = GetDefaultMeasChartOption();
            option.XMin = _measParameters.FreqStart_MHz; //900,
            option.XMax = _measParameters.FreqStop_MHz;
            var delta = DateTime.Now - serverResult.Time;
            option.LeftTitle = $"{serverResult.Time.Hour:D2}:{serverResult.Time.Minute:D2}:{serverResult.Time.Second:D2}.{serverResult.Time.Millisecond:D3} ({delta.TotalMilliseconds}ms)";
            option.Title = $"Online Measurements  -  {serverResult.Index}";
            option.RightTitle = (serverResult.Overload ? "Overload" : "");

            var count = _measParameters.Freq_Hz.Length;
            var points = new Point[count];

            var maxX = default(double);
            var minX = default(double);

            var maxY = default(double);
            var minY = default(double);

            for (int i = 0; i < count; i++)
            {
                var level = serverResult.Level[i];
                var valX = _measParameters.Freq_Hz[i];
                var valY = level;

                var point = new Point
                {
                    X = valX,
                    Y = valY
                };
                if (i == 0)
                {
                    maxX = valX;
                    minX = valX;
                    maxY = valY;
                    minY = valY;
                }
                else
                {
                    if (maxX < valX)
                        maxX = valX;
                    if (minX > valX)
                        minX = valX;

                    if (maxY < valY)
                        maxY = valY;
                    if (minY > valY)
                        minY = valY;
                }
                points[i] = point;
            }

            var preparedDataY = Environment.Utitlity.CalcLevelRange(minY, maxY);
            option.YTick = 10;
            option.YMax = preparedDataY.MaxValue;
            option.YMin = preparedDataY.MinValue;

            //var preparedDataX = Environment.Utitlity.CalcFrequencyRange(minX, maxX, 8);
            //option.XTick = preparedDataX.Step;
            //option.XMin = preparedDataX.MinValue;
            //option.XMax = preparedDataX.MaxValue;

            var preparedDataX = Environment.Utitlity.CalcLevelRange(minX - 5, maxX + 5);
            option.XTick = 50;
            option.XMin = preparedDataX.MinValue;
            option.XMax = preparedDataX.MaxValue;

            option.Points = points;

            this.MeasChartOption = option;
        }

        public void OnDeviceServerMeasResult(MessageContainer data, WebSocketContext context)
        {
            try
            {
                if (this._measStatus != MeasurementStatus.ReadyToAccept
                    && this._measStatus != MeasurementStatus.IncomingData)
                {
                    // при таком состоянии нам данные уже не интересны 
                    // и просто игнорим этот поток данных
                    return;
                }

                this._measStatus = MeasurementStatus.IncomingData;

                if (_measParameters.OnlineMeasType == OnlineMeasType.Level)
                {
                    var measResult = data.GetData<DeviceServerResultLevel>();
                    if (measResult == null)
                    {
                        throw new InvalidOperationException($"Invalid meas result received from the sensor");
                    }
                    UIContext(() =>
                    {
                        _measParameters.CurrentStatus = $"Incoming Data: {measResult.Index}";
                        this.AcceptNextResults(measResult);
                    });
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported the Online Meas Type '{_measParameters.OnlineMeasType}'");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnDeviceServerCancellation: {ex.ToString()}");
                this.WriteToLog($"ERROR '{ex.ToString()}'");

                UIContext(() =>
                {
                    _measParameters.CurrentStatus = "Has error";
                });
                this._measStatus = MeasurementStatus.Error;
            }
        }

        public void OnDeviceServerCancellation(MessageContainer data, WebSocketContext context)
        {
            try
            {
                this.WriteToLog($"Received sensor cancellation message");

                this._measStatus = MeasurementStatus.SensorCancellation;

                UIContext(() =>
                {
                    _measParameters.CurrentStatus = "Sensor Cancellation";
                });

                // Пользователь должен увидеть что сенсор прислал параметры
                Thread.Sleep(3000);

                if (_serverDescriptor == null)
                {
                    throw new InvalidOperationException("The Server Descriptor Context was not initialized");
                }

                // рапаковываем параметры
                var sensorCancellationData = data.GetData<DeviceServerCancellationData>();
                if (sensorCancellationData == null)
                {
                    throw new InvalidOperationException($"Invalid Cancellation Data received from the sensor");
                }

                // все что нам остается, это закрыть соединение
                this.CloseCeonnection();


                MessageBox.Show(sensorCancellationData.Message, "Sensor Cancellation Measurement Process", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnDeviceServerCancellation: {ex.ToString()}");
                this.WriteToLog($"ERROR '{ex.ToString()}'");

                UIContext(() =>
                {
                    _measParameters.CurrentStatus = "Has error";
                });
                this._measStatus = MeasurementStatus.Error;
                MessageBox.Show(ex.Message, "Sensor Cancellation Measurement Process", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void WriteToLog(string eventData)
        {
            var now = DateTime.Now;

            UIContext(() =>
            {
                
                this.LogRecords = $"{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}.{now.Millisecond:D3}: {eventData} \r\n{this.LogRecords}";
            });
        }
    }
}
