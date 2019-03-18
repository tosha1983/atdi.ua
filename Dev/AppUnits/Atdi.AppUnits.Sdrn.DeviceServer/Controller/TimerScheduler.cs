using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    class TimerScheduler : IDisposable
    {
        public delegate void TimerCallback();

        class CallbackSlot
        {
            public TimerCallback Callback { get; set; }

            public long TimeStamp { get; set; }

            public long Delay { get; set; }

            public long RealDelay { get; set; }

            public CallbackSlot Next { get; set; }
        }

        private Thread _processThread;
        private readonly long _timeStamp;
        private readonly CommandType _commandType;
        private readonly ITimeService _timeService;
        private readonly ILogger _logger;
        private CallbackSlot _currentSlot;
        private EventWaitHandle _slotWaiter;
        private SpinLock _spinLock;

        public TimerScheduler(CommandType commandType, ITimeService timeService, ILogger logger)
        {
            this._commandType = commandType;
            this._timeService = timeService;
            this._logger = logger;

            this._timeStamp = this._timeService.TimeStamp.Milliseconds;
            this._slotWaiter = new AutoResetEvent(false);
            this._spinLock = new SpinLock();
            this._processThread = new Thread(this.Process)
            {
                IsBackground = true,
            };
            _processThread.Priority = ThreadPriority.Highest;
            _processThread.Start();
            
        }

        private void Process()
        {
            try
            {
                while (true)
                {
                    var slot = this._currentSlot;

                    while (slot == null)
                    {
                        this._slotWaiter.WaitOne();
                        slot = this._currentSlot;
                    }

                    var timeStamp = default(long);
                    var delay = default(long);
                    bool lockTaken = false;

                    while (true)
                    {
                        timeStamp = this._timeService.TimeStamp.Milliseconds;
                        delay = slot.RealDelay - (timeStamp - this._timeStamp);

                        if (delay <= 0f)
                            break;

                        if (delay < 1f)
                        {
                            Thread.SpinWait(10);
                        }
                        else if (delay < 5f)
                        {
                            Thread.SpinWait(20);
                        }
                        else
                        {
                            if (!this._slotWaiter.WaitOne((int)delay))
                            {
                                slot = this._currentSlot;
                            }
                        }

                    }

                    // вызываем в этом потоке - считается что код будет быстрым
                    try
                    {
                        slot.Callback();
                    }
                    catch (Exception e)
                    {
                        this._logger.Exception(Contexts.TimerScheduler, Categories.Callbacking, Events.ErrorOccurredDuringCallbackCall.With(_commandType), e);
                    }

                    lockTaken = false;
                    _spinLock.Enter(ref lockTaken);
                    try
                    {
                        // Тут важно не перекрыть значение, которое возможно установили в паралельном потоке
                        if (this._currentSlot == slot)
                        {
                            this._currentSlot = slot.Next;
                        }
                        else
                        {
                            var delSlot = this._currentSlot;
                            while (delSlot != null)
                            {
                                if (delSlot.Next == slot)
                                {
                                    delSlot.Next = slot.Next;
                                    break;
                                }
                                delSlot = delSlot.Next;
                            }
                        }
                    }
                    finally
                    {
                        if (lockTaken)
                        {
                            _spinLock.Exit();
                        }
                    }

                }
            }
            catch (ThreadAbortException e)
            {
                Thread.ResetAbort();

                this._logger.Exception(Contexts.TimerScheduler, Categories.Processing, Events.AbortSchedulerThread.With(_commandType), e);
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.TimerScheduler, Categories.Processing, Events.ProcessingSchedulerError.With(_commandType), e);
            }
        }

        public void StartAt(long timestamp, long delay, TimerCallback callback)
        {
            var slot = new CallbackSlot
            {
                Callback = callback,
                TimeStamp = timestamp,
                Delay = delay
            };

            /// 1000 --- 1050 --- 50  = 1050-1000 + 50
            /// 200 --- 50 -- 400 = 50 - 200 + 400 = 250
            /// 
            slot.RealDelay = delay + (timestamp - this._timeStamp);


            // вводим щадящую блокировочку
            bool lockTaken = false;
            _spinLock.Enter(ref lockTaken);

            try
            {
                if (this._currentSlot == null)
                {
                    this._currentSlot = slot;
                    this._slotWaiter.Set();
                }
                else
                {
                    var movedSlot = this._currentSlot;
                    var prevSlot = default(CallbackSlot);

                    while (movedSlot != null)
                    {
                        if (movedSlot.RealDelay > slot.RealDelay)
                        {
                            slot.Next = movedSlot;
                            movedSlot = null;
                            if (prevSlot != null)
                            {
                                prevSlot.Next = slot;
                            }
                            else
                            {
                                this._currentSlot = slot;
                                this._slotWaiter.Set();
                            }
                        }
                        else
                        {
                            prevSlot = movedSlot;
                            movedSlot = movedSlot.Next;
                            if (movedSlot == null)
                            {
                                prevSlot.Next = slot;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (lockTaken)
                {
                    _spinLock.Exit();
                }
            }
        }

        public void Dispose()
        {
            try
            {
                if (this._processThread != null)
                {
                    this._processThread.Abort();

                    this._processThread = null;
                }

                if (this._slotWaiter != null)
                {
                    this._slotWaiter.Dispose();
                    this._slotWaiter = null;
                }
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.TimerScheduler, Categories.Disposing, e);
            }
        }
    }
}
