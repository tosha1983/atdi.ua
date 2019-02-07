using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Adapters;
using COM = Atdi.DataModels.Sdrn.DeviceServer.Commands;
using COMP = Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using COMR = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.Platform.Logging;
using System;
using EN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.Enums;
using MEN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.Enums;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound
{
    /// <summary>
    /// Пример реализации объект аадаптера
    /// </summary>
    public class Adapter : IAdapter
    {
        #region Пример
        private readonly ILogger _logger;
        private readonly AdapterConfig _adapterConfig;

        /// <summary>
        /// Все объекты адаптера создаются через DI-контейнер 
        /// Запрашиваем через конструктор необходимые сервисы
        /// </summary>
        /// <param name="adapterConfig"></param>
        /// <param name="logger"></param>
        public Adapter(AdapterConfig adapterConfig, ILogger logger)
        {
            this._logger = logger;
            this._adapterConfig = adapterConfig;
            FreqArr = new double[TracePoints];
            LevelArr = new float[TracePoints];
            for (int i = 0; i < TracePoints; i++)
            {

                FreqArr[i] = (double)(FreqStart + FreqStep * i);
                LevelArr[i] = -100;

            }
        }

        /// <summary>
        /// Метод будет вызван при инициализации потока воркера адаптера
        /// Адаптеру необходимо зарегестрировать свои обработчики комманд 
        /// </summary>
        /// <param name="host"></param>
        public void Connect(IAdapterHost host)
        {
            try
            {
                Status = bbOpenDevice(ref _Device_ID);
                if (Status != EN.Status.NoError)
                {
                    Debug.Write("Error: Unable to open BB60\n");
                    Debug.Write(bbGetStatusString(Status) + "\n");
                    return;
                }
                else
                {
                    Device_Type = bbGetDeviceName(_Device_ID);
                    Device_SerialNumber = bbGetSerialString(_Device_ID);
                    Device_APIVersion = bbGetAPIString();
                    Device_FirmwareVersion = bbGetFirmwareString(_Device_ID);


                    GetSystemInfo();
                    SetTraceDetectorAndScale();
                    SetFreqCentrSpan();
                    SetRefATT();
                    SetGain();
                    SetRbwVbwSweepTimeRbwType();
                    Status = bbInitiate(_Device_ID, (uint)DeviceMode, 0);
                }
            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            /// включем устройство
            /// иницируем его параметрами сконфигурации
            /// проверяем к чем оно готово

            /// сообщаем инфраструктуре что мы готовы обрабатывать комманду MesureGpsLocationExampleCommand
            /// и при этом возвращать оезультат в типе MesureGpsLocationExampleAdapterResult
            //host.RegisterHandler<COM.MesureGpsLocationExampleCommand, MesureGpsLocationExampleAdapterResult>(MesureGpsLocationExampleCommandHandler);
            host.RegisterHandler<COM.MesureTraceCommand, COMR.MesureTraceResult>(MesureTraceParameterHandler);
        }

        /// <summary>
        /// Метод вызывается контрллером когда необходимо выгрузит адаптер с памяти
        /// </summary>
        public void Disconnect()
        {
            // По идее про SignalHound можно просто забыть 


            /// освобождаем ресурсы и отключаем устройство
        }

        /// <summary>
        ///  типизированный обрабочик конкретной комманды MesureGpsLocation
        /// </summary>
        /// <param name="command"></param>
        /// <param name="context"></param>
        public void MesureGpsLocationExampleCommandHandler(COM.MesureGpsLocationExampleCommand command, IExecutionContext context)
        {
            /// примерный сценарий обрработки комманды адаптером
            /// этот сценарий работает в потоке адаптера
            try
            {

                /// если нужно проводим блокировку комманд который мы не сможем обслужить пока что то меряем в течении выполнени яэтой комманды
                /// и подсказывая этим инфраструктуре что этот устрйостов некоторое время не сможет обрабатываить такие комманды
                context.Lock(CommandType.MesureGpsLocation, CommandType.MesureIQStream);

                // если нужно заблокировать выполняему комманду то достатчоно вызвать метод без параметров и блокируется комманда которая выполняется
                context.Lock();

                // важно: если измерение идет в поток еадаптера то в принцпе в явных блокировках смысла нет - адаптер полностью занят и другие кломаныд обработаить не сможет
                // функции блокировки имеют смысл если мы для измерений создает отдельные потоки а этот освобождаем для прослушивани яследующих комманд

                // сценарйи в данном случаи за разарбочиком адаптера

                // что то меряем

                // пушаем результат
                var result = new MesureGpsLocationExampleAdapterResult(0, CommandResultStatus.Final);
                context.PushResult(result);

                // иногда нужно проверять токен окончания работы комманды
                if (context.Token.IsCancellationRequested)
                {
                    // все нужно остановиться

                    // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать
                    var result2 = new MesureGpsLocationExampleAdapterResult(0, CommandResultStatus.Ragged);
                    context.PushResult(result2);

                    // подтверждаем факт обработки отмены
                    context.Cancel();
                    // освобождаем поток 
                    return;
                }

                // снимаем блокировку с текущей команды
                context.Unlock();

                // что то делаем еще 


                // подтверждаем окончание выполнения комманды 
                // важно: всн ранее устапнволеные в контексте обработки текущей команыд блокировки снимаются автоматически
                context.Finish();
                // дальше кода быть не должно, освобождаем поток
            }
            catch (Exception e)
            {
                // желательно записать влог
                _logger.Exception(Contexts.ThisComponent, e);
                // этот вызов обязательный в случаи обрыва
                context.Abort(e);
                // дальше кода быть не должно, освобождаем поток
            }

        }

        public void MesureTraceParameterHandler(COM.MesureTraceCommand command, IExecutionContext context)
        {
            /// примерный сценарий обрработки комманды адаптером
            /// этот сценарий работает в потоке адаптера
            try
            {

                /// если нужно проводим блокировку комманд который мы не сможем обслужить пока что то меряем в течении выполнени яэтой комманды
                /// и подсказывая этим инфраструктуре что этот устрйостов некоторое время не сможет обрабатываить такие комманды
                context.Lock(CommandType.MesureTrace);

                // если нужно заблокировать выполняему комманду то достатчоно вызвать метод без параметров и блокируется комманда которая выполняется
                context.Lock();

                // важно: если измерение идет в поток еадаптера то в принцпе в явных блокировках смысла нет - адаптер полностью занят и другие кломаныд обработаить не сможет
                // функции блокировки имеют смысл если мы для измерений создает отдельные потоки а этот освобождаем для прослушивани яследующих комманд

                // сценарйи в данном случаи за разарбочиком адаптера

                // что то меряем
                FreqStart = ConvertersITLPs.FreqStart(this, command.Parameter.FreqStart_Hz);
                FreqStop = ConvertersITLPs.FreqStop(this, command.Parameter.FreqStop_Hz);
                Status = bbConfigureCenterSpan(_Device_ID, (double)FreqCentr, (double)FreqSpan);

                Attenuator = ConvertersITLPs.Attenuator(command.Parameter.Att_dB);
                RefLevel = command.Parameter.RefLevel_dBm;
                Status = bbConfigureLevel(_Device_ID, (double)RefLevel, (double)Attenuator);

                Gain = ConvertersITLPs.Gain(command.Parameter.PreAmp_dB);
                Status = bbConfigureGain(_Device_ID, (int)Gain);

                RBW = (decimal)command.Parameter.RBW_Hz;
                VBW = (decimal)command.Parameter.VBW_Hz;
                SweepTime = (decimal)command.Parameter.SweepTime_s;
                Status = bbConfigureSweepCoupling(_Device_ID, (double)RBW, (double)VBW, (double)SweepTime, (uint)RBWShape, (uint)Rejection);
                TraceCountToMeas = command.Parameter.TraceCount;
                TraceCount = 0;
                TracePoints = command.Parameter.TracePoint;

                for (int i = 0; i < TraceCountToMeas; i++)
                {
                    GetTrace();
                    TraceCount++;
                    // пушаем результат
                    var result = new COMR.MesureTraceResult(0, CommandResultStatus.Next);
                    if (TraceCountToMeas == TraceCount)
                    { result = new COMR.MesureTraceResult(0, CommandResultStatus.Final); }
                    result.Freq_Hz = FreqArr;
                    result.Level = LevelArr;
                    result.TimeStamp = 0; //пофиксить

                    context.PushResult(result);

                    // иногда нужно проверять токен окончания работы комманды
                    if (context.Token.IsCancellationRequested)
                    {
                        // все нужно остановиться

                        // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать
                        var result2 = new COMR.MesureTraceResult(0, CommandResultStatus.Ragged);

                        context.PushResult(result2);


                        // подтверждаем факт обработки отмены
                        context.Cancel();
                        // освобождаем поток 
                        return;
                    }

                }

                // снимаем блокировку с текущей команды
                context.Unlock();

                // что то делаем еще 


                // подтверждаем окончание выполнения комманды 
                // важно: всн ранее устапнволеные в контексте обработки текущей команыд блокировки снимаются автоматически
                context.Finish();
                // дальше кода быть не должно, освобождаем поток
            }
            catch (Exception e)
            {
                // желательно записать влог
                _logger.Exception(Contexts.ThisComponent, e);
                // этот вызов обязательный в случаи обрыва
                context.Abort(e);
                // дальше кода быть не должно, освобождаем поток
            }

        }
        #endregion

        ConvertersInputToLocalParameters ConvertersITLPs = new ConvertersInputToLocalParameters();

        #region Param
        public EN.Status Status { get; set; } = EN.Status.NoError;
        public EN.Mode DeviceMode { get; set; } = EN.Mode.Sweeping;

        #region Freqs


        /// <summary>
        /// true = CentrSpan
        /// false = StartStop
        /// </summary>
        public bool Freq_CentrSpan_StartStop
        {
            get { return _Freq_CentrSpan_StartStop; }
            set { _Freq_CentrSpan_StartStop = value; }
        }
        private bool _Freq_CentrSpan_StartStop;



        public decimal FreqCentr
        {
            get { return _FreqCentr; }
            set
            {
                _FreqCentr = value;
                _FreqStart = _FreqCentr - _FreqSpan / 2;
                _FreqStop = _FreqCentr + _FreqSpan / 2;
                _Freq_CentrSpan_StartStop = true;
            }
        }
        private decimal _FreqCentr = 1000000000;//2142400000;

        public decimal FreqSpan
        {
            get { return _FreqSpan; }
            set
            {
                _FreqSpan = value;
                _FreqStart = _FreqCentr - _FreqSpan / 2;
                _FreqStop = _FreqCentr + _FreqSpan / 2;
                Freq_CentrSpan_StartStop = true;
            }
        }
        private decimal _FreqSpan = 20000000;

        public decimal FreqStart
        {
            get { return _FreqStart; }
            set
            {
                _FreqStart = value;
                _FreqCentr = (_FreqStart + _FreqStop) / 2;
                _FreqSpan = _FreqStop - _FreqStart;
                _Freq_CentrSpan_StartStop = false;
            }
        }
        private decimal _FreqStart = 990000000;//2139900000;//1800000000;//2600000000;//2490000000;//

        public decimal FreqStop
        {
            get { return _FreqStop; }
            set
            {
                _FreqStop = value;
                _FreqCentr = (_FreqStart + _FreqStop) / 2;
                _FreqSpan = _FreqStop - _FreqStart;
                _Freq_CentrSpan_StartStop = false;
            }
        }
        private decimal _FreqStop = 1010000000;
        #endregion Freqs

        #region RBW / VBW
        public Enums.Rejection Rejection { get; set; } = Enums.Rejection.SpurReject;
        #region RBW
        public Enums.RBWShape RBWShape { get; set; } = Enums.RBWShape.Shape_FlatTop;
        decimal RBWMax = 10000000;

        public decimal[] RBWArr
        {
            get { return _RBWArr; }
            private set { }
        }
        private decimal[] _RBWArr = new decimal[] { 1, 3, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000, 10000000 };

        public decimal RBW { get; set; } = 10000;

        public int RBWIndex
        {
            get { return _RBWIndex; }
            set
            {
                if (value > -1 && value < RBWArr.Length) { _RBWIndex = value; RBW = RBWArr[_RBWIndex]; }
                else if (value < 0) { _RBWIndex = 0; RBW = RBWArr[_RBWIndex]; }
                else if (value >= RBWArr.Length) { _RBWIndex = RBWArr.Length - 1; RBW = RBWArr[_RBWIndex]; }
                if (AutoVBW) VBWIndex = _RBWIndex;
                if (AutoVBW == false && _VBWIndex > _RBWIndex) VBWIndex = _RBWIndex;
            }
        }
        private int _RBWIndex = 7;
        #endregion RBW

        #region VBW
        decimal VBWMax = 10000000;

        public bool AutoVBW
        {
            get { return _AutoVBW; }
            set { _AutoVBW = value; }
        }
        private bool _AutoVBW = true;
        public decimal[] VBWArr
        {
            get { return _VBWArr; }
            private set { }
        }
        private decimal[] _VBWArr = new decimal[] { 1, 3, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000, 10000000 };

        public decimal VBW
        {
            get { return _VBW; }
            set { _VBW = value; }
        }
        private decimal _VBW = 10000;
        public int VBWIndex
        {
            get { return _VBWIndex; }
            set
            {
                if (value > -1 && value < VBWArr.Length) { _VBWIndex = value; VBW = VBWArr[_VBWIndex]; }
                else if (value < 0) { _VBWIndex = 0; VBW = VBWArr[_VBWIndex]; }
                else if (value >= VBWArr.Length) { _VBWIndex = VBWArr.Length - 1; VBW = RBWArr[_VBWIndex]; }
            }
        }
        private int _VBWIndex = 7;
        #endregion VBW
        #endregion RBW / VBW

        #region Sweep
        public decimal SweepTimeMin = 0.00001m;
        public decimal SweepTimeMax = 1;

        public decimal SweepTime
        {
            get { return _SweepTime; }
            set
            {
                if (value >= SweepTimeMin && value <= SweepTimeMax) _SweepTime = value;
                if (value <= SweepTimeMin) _SweepTime = SweepTimeMin;
                if (value >= SweepTimeMax) _SweepTime = SweepTimeMax;
            }
        }
        private decimal _SweepTime = 0.00001m;
        #endregion

        #region Levels
        /// <summary>
        /// Перегруз прибора
        /// 0 = normal
        /// 1 = RF Overload
        /// </summary>
        public int RFOverload { get; set; } = 0;

        #region уровни отображения
        public decimal RefLevel
        {
            get { return _RefLevel; }
            set
            {
                _RefLevel = value;
                LowestLevel = _RefLevel - Range;
            }
        }
        private decimal _RefLevel = -40;

        public decimal Range
        {
            get { return _Range; }
            set
            {
                _Range = value;
                LowestLevel = _RefLevel - Range;
            }
        }
        private decimal _Range = 100;

        public decimal LowestLevel { get; set; } = -140;
        #endregion


        #region LevelUnit
        public EN.Scale Scale { get; set; } = EN.Scale.LogScale;
        public MEN.LevelUnit LevelUnit { get; set; } = MEN.LevelUnit.dBm;

        public MEN.LevelUnit[] LevelUnits
        {
            get { return _LevelUnits; }
            private set { }
        }
        private readonly MEN.LevelUnit[] _LevelUnits = new MEN.LevelUnit[]
        {
            MEN.LevelUnit.dBm,
            MEN.LevelUnit.dBµV
        };
        #endregion

        #region Gain        
        public Enums.Gain Gain { get; set; } = Enums.Gain.Gain_AUTO;
        public Enums.Gain[] Gains
        {
            get { return _Gains; }
            private set { }
        }
        private readonly Enums.Gain[] _Gains = new Enums.Gain[]
        {
            Enums.Gain.Gain_AUTO,
            Enums.Gain.Gain_0,
            Enums.Gain.Gain_1,
            Enums.Gain.Gain_2,
            Enums.Gain.Gain_3
        };
        #endregion Gain

        #region ATT        
        public Enums.Attenuator Attenuator { get; set; } = Enums.Attenuator.Atten_AUTO;
        public Enums.Attenuator[] Attenuators
        {
            get { return _Attenuators; }
            private set { }
        }
        private readonly Enums.Attenuator[] _Attenuators = new Enums.Attenuator[]
        {
            Enums.Attenuator.Atten_AUTO,
            Enums.Attenuator.Atten_0,
            Enums.Attenuator.Atten_10,
            Enums.Attenuator.Atten_20,
            Enums.Attenuator.Atten_30,
        };
        #endregion ATT
        #endregion

        #region Trace Data
        public bool NewTrace { get; set; } = false;

        public decimal FreqStep { get; set; } = 10000;

        public int TracePoints { get; set; } = 1601;

        public int TraceCountToMeas { get; set; } = 1;
        public int TraceCount { get; set; } = 1;

        public double[] FreqArr { get; set; }
        public float[] LevelArr { get; set; }
        public TracePoint[] Trace1 { get; set; }

        public TracePoint[] Trace1Min { get; set; }


        public float[] RealTimeFrame { get; set; }



        public int RealTimeFrameWidth
        {
            get { return _RealTimeFrameWidth; }
            set { _RealTimeFrameWidth = value; }
        }
        private int _RealTimeFrameWidth = 1;

        public int RealTimeFrameHeight
        {
            get { return _RealTimeFrameHeight; }
            set { _RealTimeFrameHeight = value; }
        }
        private int _RealTimeFrameHeight = 1;

        decimal TraceFreqStart = 0;
        decimal TraceFreqStop = 0;

        #region Trace
        public EN.Detector Detector { get; set; } = EN.Detector.MaxOnly;

        public EN.Detector[] Detectors
        {
            get { return _Detectors; }
            private set { }
        }
        private readonly EN.Detector[] _Detectors = new EN.Detector[]
        {
            EN.Detector.MinOnly,
            EN.Detector.MaxOnly,
            EN.Detector.MinAndMax,
            EN.Detector.Average
        };

        public EN.TraceType TraceType { get; set; } = EN.TraceType.ClearWrite;

        public EN.TraceType[] TraceTypes
        {
            get { return _TraceTypes; }
            private set { }
        }
        private readonly EN.TraceType[] _TraceTypes = new EN.TraceType[]
        {
            EN.TraceType.ClearWrite,
            EN.TraceType.MaxHold,
            EN.TraceType.MinHold,
            EN.TraceType.Average
        };


        public EN.Unit VideoUnit { get; set; } = EN.Unit.Log;
        public EN.Unit[] Units
        {
            get { return _Units; }
            private set { }
        }
        private readonly EN.Unit[] _Units = new EN.Unit[]
        {
            EN.Unit.Log,
            EN.Unit.Voltage,
            EN.Unit.Power,
            EN.Unit.Sample
        };
        #endregion
        #endregion

        #region runs
        //bool _Run;
        //public bool Run
        //{
        //    get { return _Run; }
        //    set
        //    {
        //        _Run = value;
        //        if (Run)
        //        {
        //            GetData = true;
        //            Connect();
        //        }
        //        else if (!Run)
        //        {
        //            Disconnect();
        //        }

        //    }
        //}
        private static bool _IsRuning;
        public bool IsRuning
        {
            get { return _IsRuning; }
            set { _IsRuning = value; }
        }
        public bool _GetData;// = "";
        public bool GetData
        {
            get { return _GetData; }
            set { _GetData = value; }
        }
        private long _LastUpdate;
        public long LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
        #endregion

        #region Device info
        public decimal FreqMin = 9000;
        public decimal FreqMax = 6400000000;

        /// <summary>
        /// температура приемника от нее пляшем с калибровкой
        /// </summary>
        public double Device_BoardTemp
        {
            get { return _Device_BoardTemp; }
            set { _Device_BoardTemp = value; }
        }
        private double _Device_BoardTemp = 10000;
        /// <summary>
        /// последняя температура калибровки
        /// </summary>
        private double Device_LastCalcBoardTemp = 10000;

        public double Device_USBVoltage
        {
            get { return _Device_USBVoltage; }
            set { _Device_USBVoltage = value; }
        }
        private double _Device_USBVoltage = 10000;

        public double Device_USBCurrent
        {
            get { return _Device_USBCurrent; }
            set { _Device_USBCurrent = value; }
        }
        private double _Device_USBCurrent = 10000;

        public int Device_ID
        {
            get { return _Device_ID; }
            set { _Device_ID = value; }
        }
        private int _Device_ID = -1;

        public string Device_Type
        {
            get { return _Device_Type; }
            set { _Device_Type = value; }
        }
        private string _Device_Type = "";

        public string Device_SerialNumber
        {
            get { return _Device_SerialNumber; }
            set { _Device_SerialNumber = value; }
        }
        private string _Device_SerialNumber = "";

        public string Device_FirmwareVersion
        {
            get { return _Device_FirmwareVersion; }
            set { _Device_FirmwareVersion = value; }
        }
        private string _Device_FirmwareVersion = "";

        public string Device_APIVersion
        {
            get { return _Device_APIVersion; }
            set { _Device_APIVersion = value; }
        }
        private string _Device_APIVersion = "";


        #endregion
        #endregion Param

        #region Private Method
        private void GetSystemInfo()
        {
            try
            {
                float temp = 0.0F, voltage = 0.0F, current = 0.0F;
                Status = bbGetDeviceDiagnostics(_Device_ID, ref temp, ref voltage, ref current);
            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetTraceDetectorAndScale()
        {
            try
            {
                Status = bbConfigureAcquisition(_Device_ID, (uint)Detector, (uint)Scale);
            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetFreqCentrSpan()
        {
            try
            {
                Status = bbConfigureCenterSpan(_Device_ID, (double)FreqCentr, (double)FreqSpan);
            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetRefATT()
        {
            try
            {
                Status = bbConfigureLevel(_Device_ID, (double)RefLevel, (double)Attenuator);
            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetGain()
        {
            try
            {
                Status = bbConfigureGain(_Device_ID, (int)Gain);
            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetRbwVbwSweepTimeRbwType()
        {
            try
            {
                Status = bbConfigureSweepCoupling(_Device_ID, (double)RBW, (double)VBW, (double)SweepTime, (uint)RBWShape, (uint)Rejection);

            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetVideoUnits()
        {
            try
            {
                Status = bbConfigureProcUnits(_Device_ID, (uint)VideoUnit);
            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetOverLoad(EN.Status value)
        {
            if (value == EN.Status.ADCOverflow) { RFOverload = 1; }
            else { RFOverload = 0; }
        }

        private void GetTrace()
        {
            //получаем спектр
            uint trace_len = 0;
            double bin_size = 0.0;
            double start_freq = 0.0;
            Status = bbQueryTraceInfo(_Device_ID, ref trace_len, ref bin_size, ref start_freq);
            SetOverLoad(Status);

            //bin_size = Math.Round(bin_size, 8);
            FreqStep = (decimal)bin_size;
            if (Status != EN.Status.DeviceConnectionErr ||
                Status != EN.Status.DeviceInvalidErr ||
                Status != EN.Status.DeviceNotOpenErr ||
                Status != EN.Status.USBTimeoutErr)
            {
                IsRuning = true; LastUpdate = DateTime.Now.Ticks;
            }
            float[] sweep_max, sweep_min;
            sweep_max = new float[trace_len];
            sweep_min = new float[trace_len];
            Status = bbFetchTrace_32f(_Device_ID, unchecked((int)trace_len), sweep_min, sweep_max);
            SetOverLoad(Status);
            SetTraceData((int)trace_len, sweep_min, sweep_max, (decimal)start_freq, (decimal)bin_size);
        }
        private void SetTraceData(int newLength, float[] mintrace, float[] maxtrace, decimal freqStart, decimal step)
        {
            //TracePoint[] temp11 = Trace1;
            //TracePoint[] temp12 = Trace1Min;
            if (maxtrace.Length > 0 && newLength > 0 && step > 0)
            {
                if (TracePoints != newLength || (Math.Abs(TraceFreqStart - (decimal)freqStart) >= (decimal)step))
                {
                    TraceFreqStart = freqStart;
                    TracePoints = newLength;
                    //temp11 = new TracePoint[newLength];
                    //temp12 = new TracePoint[newLength];
                    FreqArr = new double[newLength];
                    LevelArr = new float[newLength];
                    for (int i = 0; i < newLength; i++)
                    {
                        decimal freq = freqStart + step * i;
                        FreqArr[i] = (double)freq;
                        LevelArr[i] = maxtrace[i];
                        //temp11[i] = new TracePoint() { Freq = freq, Level = maxtrace[i] };
                        //temp12[i] = new TracePoint() { Freq = freq, Level = mintrace[i] };
                    }

                }
                #region Trace 1
                if (TraceType == EN.TraceType.ClearWrite)
                {
                    for (int i = 0; i < newLength; i++)
                    {
                        LevelArr[i] = maxtrace[i];
                        //temp11[i].Level = maxtrace[i];
                        //temp12[i].Level = mintrace[i];
                    }
                }
                //else if (TraceType == )//Average
                //{
                //    TracePoint[] temp = new TracePoint[newLength];
                //    for (int i = 0; i < newLength; i++)
                //    {
                //        decimal freq = freqStart + step * i;
                //        temp[i] = new TracePoint() { Freq = freq, Level = maxtrace[i] };
                //    }
                //}
                else if (TraceType == EN.TraceType.MaxHold)
                {
                    for (int i = 0; i < newLength; i++)
                    {
                        if (maxtrace[i] > LevelArr[i]) LevelArr[i] = maxtrace[i];
                        //if (maxtrace[i] > temp11[i].Level) temp11[i].Level = maxtrace[i];
                    }
                }
                else if (TraceType == EN.TraceType.MinHold)
                {
                    for (int i = 0; i < newLength; i++)
                    {
                        if (maxtrace[i] < LevelArr[i]) LevelArr[i] = maxtrace[i];
                        //if (maxtrace[i] < temp11[i].Level) temp11[i].Level = maxtrace[i];
                    }
                }
                #endregion Trace 1

                NewTrace = true;
                //Trace1 = temp11;
                //Trace1Min = temp12;

            }
        }
        #endregion Private Method


        #region Dll import
        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbGetSerialNumberList(int[] devices, ref int deviceCount);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbOpenDeviceBySerialNumber(ref int device, int serialNumber);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbOpenDevice(ref int device);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbCloseDevice(int device);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureAcquisition(int device, uint detector, uint scale);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureCenterSpan(int device, double center, double span);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureLevel(int device, double refLevel, double atten);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureGain(int device, int gain);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureSweepCoupling(int device, double rbw, double vbw, double sweepTime, uint rbwShape, uint rejection);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureProcUnits(int device, uint units);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureIO(int device, uint port1, uint port2);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureDemod(int device, int modType, double freq, float ifBandwidth, float lowPassFreq, float highPassFreq, float fmDeemphasis);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureIQ(int device, int downsampleFactor, double bandwidth);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbInitiate(int device, uint mode, uint flag);

        [DllImport("bb_api", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbFetchTrace_32f(int device, int arraySize, float[] min, float[] max);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbFetchTrace(int device, int arraysize, double[] min, double[] max);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbFetchRealTimeFrame(int device, float[] sweep, float[] frame, float[] alphaFrame);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbFetchAudio(int device, float[] audio);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbGetIQUnpacked(int device, float[] iqData, int iqCount, int[] triggers, int triggerCount, int purge, ref int dataRemaining, ref int sampleLoss, ref int sec, ref int nano);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbQueryTraceInfo(int device, ref uint trace_len, ref double bin_size, ref double start);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbQueryStreamInfo(int device, ref int return_len, ref double bandwidth, ref int samples_per_sec);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbAbort(int device);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbPreset(int device);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbSelfCal(int device);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbSyncCPUtoGPS(int com_port, int baud_rate);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbGetDeviceType(int device, ref int type);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbGetSerialNumber(int device, ref uint serial_number);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbGetFirmwareVersion(int device, ref int version);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbGetDeviceDiagnostics(int device, ref float temperature, ref float usbVoltage, ref float usbCurrent);

        public static string bbGetDeviceName(int device)
        {
            int device_type = -1;
            bbGetDeviceType(device, ref device_type);
            if (device_type == (int)EN.Device.BB60A)
                return "BB60A";
            if (device_type == (int)EN.Device.BB60C)
                return "BB60C";

            return "Unknown device";
        }

        public static string bbGetSerialString(int device)
        {
            uint serial_number = 0;
            if (bbGetSerialNumber(device, ref serial_number) == EN.Status.NoError)
                return serial_number.ToString();
            return "";
        }

        public static string bbGetFirmwareString(int device)
        {
            int firmware_version = 0;
            if (bbGetFirmwareVersion(device, ref firmware_version) == EN.Status.NoError)
                return firmware_version.ToString();

            return "";
        }

        public static string bbGetAPIString()
        {
            IntPtr str_ptr = bbGetAPIVersion();
            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(str_ptr);
        }

        public static string bbGetStatusString(EN.Status status)
        {
            IntPtr str_ptr = bbGetErrorString(status);
            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(str_ptr);
        }

        // Call get_string variants above instead
        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr bbGetAPIVersion();
        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr bbGetErrorString(EN.Status status);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureRealTime(int device,
            double frameScale, int frameRate);
        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbQueryRealTimeInfo(int device,
            ref int frameWidth, ref int frameHeight);
        #region Old
        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbOpenDevice(ref int device);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbCloseDevice(int device);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbConfigureAcquisition(int device,
        //    uint detector, uint scale);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbConfigureCenterSpan(int device,
        //    double center, double span);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbConfigureLevel(int device,
        //    double ref_level, double atten);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbConfigureGain(int device, int gain);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbConfigureSweepCoupling(int device,
        //    double rbw, double vbw, double sweepTime, uint rbw_type, uint rejection);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbConfigureRealTime(int device,
        //    double frameScale, int frameRate);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbConfigureWindow(int device, uint window);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbConfigureProcUnits(int device, uint units);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbConfigureIO(int device,
        //    uint port1, uint port2);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbConfigureDemod(int device,
        //    int mod_type, double freq, float if_bandwidth, float low_pass_freq,
        //    float high_pass_freq, float fm_deemphasis);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbConfigureIQ(int device,
        //    int downsampleFactor, double bandwidth);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbInitiate(int device,
        //    uint mode, uint flag);

        //[DllImport("bb_api", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbFetchTrace_32f(int device,
        //    int arraySize, float[] min, float[] max);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbFetchTrace(int device,
        //    int array_size, double[] min, double[] max);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbFetchAudio(int device, ref float audio);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbFetchRaw(int device,
        //    float[] buffer, int[] triggers);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbFetchRealTimeFrame(int device,
        //    float[] sweep, float[] frame, object magicAHAHAHA_null);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbQueryTraceInfo(int device,
        //    ref uint trace_len, ref double bin_size, ref double start);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbQueryRealTimeInfo(int device,
        //    ref int frameWidth, ref int frameHeight);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbQueryStreamInfo(int device,
        //    ref int return_len, ref double bandwidth, ref int samples_per_sec);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbQueryTimestamp(int device,
        //    ref uint seconds, ref uint nanoseconds);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbAbort(int device);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbPreset(int device);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbSelfCal(int device);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbSyncCPUtoGPS(int com_port, int baud_rate);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbGetDeviceType(int device, ref int type);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbGetSerialNumber(int device, ref uint serial_number);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbGetFirmwareVersion(int device, ref int version);

        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bbStatus bbGetDeviceDiagnostics(int device,
        //    ref float temperature, ref float usbVoltage, ref float usbCurrent);

        //public static string bbGetDeviceName(int device)
        //{
        //    int device_type = -1;
        //    bbGetDeviceType(device, ref device_type);
        //    if (device_type == (int)BB_Device.BB_BB60A)
        //        return "BB60A";
        //    if (device_type == (int)BB_Device.BB_BB60C)
        //        return "BB60C";

        //    return "Unknown device";
        //}

        //public static string bbGetSerialString(int device)
        //{
        //    uint serial_number = 0;
        //    if (bbGetSerialNumber(device, ref serial_number) == bbStatus.bbNoError)
        //        return serial_number.ToString();

        //    return "";
        //}

        //public static string bbGetFirmwareString(int device)
        //{
        //    int firmware_version = 0;
        //    if (bbGetFirmwareVersion(device, ref firmware_version) == bbStatus.bbNoError)
        //        return firmware_version.ToString();

        //    return "";
        //}

        //public static string bbGetAPIString()
        //{
        //    IntPtr str_ptr = bbGetAPIVersion();
        //    return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(str_ptr);
        //}

        //public static string bbGetStatusString(bbStatus status)
        //{
        //    IntPtr str_ptr = bbGetErrorString(status);
        //    return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(str_ptr);
        //}

        //// Call get_string variants above instead
        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //private static extern IntPtr bbGetAPIVersion();
        //[DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        //private static extern IntPtr bbGetErrorString(bbStatus status);
        #endregion





        #endregion
    }
}
