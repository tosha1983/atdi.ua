using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Adapters;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.EN;

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
        }

        /// <summary>
        /// Метод будет вызван при инициализации потока воркера адаптера
        /// Адаптеру необходимо зарегестрировать свои обработчики комманд 
        /// </summary>
        /// <param name="host"></param>
        public void Connect(IAdapterHost host)
        {
            /// включем устройство
            /// иницируем его параметрами сконфигурации
            /// проверяем к чем оно готово

            /// сообщаем инфраструктуре что мы готовы обрабатывать комманду MesureGpsLocationExampleCommand
            /// и при этом возвращать оезультат в типе MesureGpsLocationExampleAdapterResult
            host.RegisterHandler<MesureGpsLocationExampleCommand, MesureGpsLocationExampleAdapterResult>(MesureGpsLocationExampleCommandHandler);
        }

        /// <summary>
        /// Метод вызывается контрллером когда необходимо выгрузит адаптер с памяти
        /// </summary>
        public void Disconnect()
        {
            /// освобождаем ресурсы и отключаем устройство
        }

        /// <summary>
        ///  типизированный обрабочик конкретной комманды MesureGpsLocation
        /// </summary>
        /// <param name="command"></param>
        /// <param name="context"></param>
        public void MesureGpsLocationExampleCommandHandler(MesureGpsLocationExampleCommand command, IExecutionContext context)
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
        #endregion

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


        
        private decimal FreqCentr
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

        private decimal FreqSpan
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

        private decimal FreqStart
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
        public EN.Rejection Rejection { get; set; } = EN.Rejection.SpurReject;
        #region RBW
        public EN.RBWShape RBWShape { get; set; } = EN.RBWShape.Shape_FlatTop;
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
        //public EN.Scale Scale { get; set; } = EN.Scale.LogScale;
        //public LevelUnit LevelUnit { get; set; } = LevelUnit.dBm;

        //public LevelUnit[] LevelUnits
        //{
        //    get { return _LevelUnits; }
        //    private set { }
        //}
        //private readonly LevelUnit[] _LevelUnits = new LevelUnit[]
        //{
        //    LevelUnit.dBm,
        //    LevelUnit.dBµV
        //};
        #endregion

        #region Gain        
        public EN.Gain Gain { get; set; } = EN.Gain.Gain_AUTO;
        public EN.Gain[] Gains
        {
            get { return _Gains; }
            private set { }
        }
        private readonly EN.Gain[] _Gains = new EN.Gain[]
        {
            EN.Gain.Gain_AUTO,
            EN.Gain.Gain_0,
            EN.Gain.Gain_1,
            EN.Gain.Gain_2,
            EN.Gain.Gain_3
        };
        #endregion Gain

        #region ATT        
        public EN.Attenuator Attenuator { get; set; } = EN.Attenuator.Atten_AUTO;
        public EN.Attenuator[] Attenuators
        {
            get { return _Attenuators; }
            private set { }
        }
        private readonly EN.Attenuator[] _Attenuators = new EN.Attenuator[]
        {
            EN.Attenuator.Atten_AUTO,
            EN.Attenuator.Atten_0,
            EN.Attenuator.Atten_10,
            EN.Attenuator.Atten_20,
            EN.Attenuator.Atten_30,
        };
        #endregion ATT
        #endregion
        #endregion
    }
}
