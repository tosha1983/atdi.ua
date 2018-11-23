using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDR.Server.MeasurementProcessing;

namespace Atdi.SDR.Server.MeasurementProcessing.SingleHound
{
    public class SDRBB60C : ISDR
    {
        #region parameter
        SDRState sDRstate = SDRState.NeedInitialization;
        private int id_dev = 999; //ID признак выключенного устройства.
        private int LastTaskId; //идентификатор таска который последним был на обработке
        TraceType traceType;

        public uint TraceSize; //
        private int frameWidth;//
        private int frameHeight;//
        private int sw_time;//
        private Double bin_size;//
        private Double start_freq;//
        private int return_len; //IQ
        private int samples_per_sec; //IQ
        private double bandwidth; // IQ



        private bbStatus status = bbStatus.bbNoError;

        private Atdi.AppServer.Contracts.Sdrns.SpectrumScanType TypeSpectrumScan = Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.Sweep;
        private Double f_min = 103;//MHz
        private Double f_max = 105;//MHz
        private Double RBW = 10000;//Hz
        private Double VBW = 10000;//Hz
        private Double Time_of_m = 0.001; // sek
        private Double ref_level_dbm = -30;
        private Double BW_CH;//kHz для измерения  SO
        private uint DETECT_TYPE = bb_api.BB_AVERAGE;
        private Double atten = bb_api.BB_AUTO_ATTEN;
        private int gain_SDR = bb_api.BB_AUTO_GAIN;
        private uint rbwShape = bb_api.BB_NUTALL;
        #endregion

        #region LowMethods
        public bool Calibration()
        {
            bool done = false;
            try
            {
                status = bb_api.bbSelfCal(id_dev);
                if (status != bbStatus.bbNoError) {} else { done = true; sDRstate = SDRState.ReadyForMeasurements; }
            }
            catch {}
            return done;
        }
        public void Close()
        {
            try
            {
                bb_api.bbCloseDevice(id_dev);
                sDRstate = SDRState.NeedInitialization;
            }
            catch {}
        }
        public bool Initiation()
        {
            bool done = false;
            try
            {
                if (id_dev == 999)
                    status = bb_api.bbOpenDevice(ref id_dev);
                if (status != bbStatus.bbNoError) {} else { done = true; sDRstate = SDRState.ReadyForMeasurements; }
            }
            catch { }
            return done;
        }
        public bool ResetDevice()
        {
            bool done = false;
            try
            {
                bb_api.OffUsbDevice();
                System.Threading.Thread.Sleep(2000);
                bb_api.OnUsbDevice();
                System.Threading.Thread.Sleep(5000);
                id_dev = 999;
                done = true;
                sDRstate = SDRState.NeedInitialization;
            }
            catch {}
            return done;
        }
        #endregion

        public int GetLastTaskId()
        {
            return LastTaskId;
        }
        public SDRState GetSDRState()
        {
            return sDRstate;
        }

        #region IQStream
        public bool GetIQStream(ref float[] iq_sample, ref int[] trigger)
        {
            //(int id_dev, int return_len, int samples_per_sec, Double TimeReceivingSec)
            bool done = false;
            try
            {
                iq_sample = new float[return_len * 2];
                trigger = new int[80];
                bb_api.bbFetchRaw(id_dev, iq_sample, trigger);
                done = true;
            }
            catch
            {
                iq_sample = null;
                trigger = null;
            }
            return done;
        }
        #endregion

        #region Trace 
        public SDRTraceParameters GetSDRTraceParameters()
        {
            SDRTraceParameters sDRTraceParameters = new SDRTraceParameters();
            sDRTraceParameters.StartFreq_Hz = start_freq;
            sDRTraceParameters.StepFreq_Hz = bin_size;
            sDRTraceParameters.TraceSize = TraceSize;
            sDRTraceParameters.FrameHeight = frameHeight;
            sDRTraceParameters.FrameWidth = frameWidth;
            return sDRTraceParameters;
        }
        public float[] GetTrace(int TraceCount = 1)
        {
            sw_time = TraceCount;
            float[] Trace = null;
            if (TypeSpectrumScan == SpectrumScanType.Sweep)
            {
                if (traceType == TraceType.Average) { Trace = SweepAvarageTrace(sw_time); }
                if (traceType == TraceType.MaxHold) { Trace = SweepMaxHoldTrace(sw_time); }
            }
            if (TypeSpectrumScan == SpectrumScanType.RealTime)
            {
                if (traceType == TraceType.Average) { Trace = RealTimeAvarageTrace(sw_time); }
                if (traceType == TraceType.MaxHold) { Trace = RealTimeMaxHoldTrace(sw_time); }
            }
            return Trace;
        }
        private float[] RealTimeAvarageTrace(int n_RT)
        {
            float[] arr = null;  
            try
            {
                float[] sweep, frame;
                sweep = new float[TraceSize];
                frame = new float[frameWidth * frameHeight];
                arr = new float[TraceSize];
                for (int j = 0; j < n_RT; j++)
                {
                    bb_api.bbFetchRealTimeFrame(id_dev, sweep, frame);
                    for (int i = 0; i < TraceSize; i++)
                    {
                        arr[i] = ((j) * arr[i] + sweep[i]) / (j + 1);
                    }
                }
            }
            catch  { arr = null; }
            return arr;
        }
        private float[] RealTimeMaxHoldTrace(int n_RT)
        {
            float[] arr = null;
            try
            {
                float[] sweep, frame;
                sweep = new float[TraceSize];
                frame = new float[frameWidth * frameHeight];
                arr = new float[TraceSize];
                for (int j = 0; j < n_RT; j++)
                {
                    bb_api.bbFetchRealTimeFrame(id_dev, sweep, frame);
                    for (int i = 0; i < TraceSize; i++)
                    {
                        if (arr[i] < sweep[i]) { arr[i] = sweep[i]; }
                    }
                }
            }
            catch { arr = null; }
            return arr;
        }
        private float[] SweepAvarageTrace(int n_sweep)
        {
            float[] arr = null;
            try
            {
                float[] sweep_max, sweep_min;
                sweep_max = new float[TraceSize];
                sweep_min = new float[TraceSize];
                arr = new float[TraceSize];
                for (int j = 0; j < n_sweep; j++)
                {
                    status = bb_api.bbFetchTrace_32f(id_dev, unchecked((int)TraceSize), sweep_min, sweep_max);
                    if (j == 0)
                    {
                        for (int i = 0; i < TraceSize; i++)
                        {
                            arr[i] = sweep_max[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < TraceSize; i++)
                        {
                             arr[i] = (float)(10*Math.Log10((Math.Pow(10, arr[i]/10)*i + Math.Pow(10, sweep_max[i]/10))/(i+1.0))); // НЕ ПРОВЕРЕННО
                        }
                    }
                }
            }
            catch { arr = null; }
            return arr;

        }
        private float[] SweepMaxHoldTrace(int n_sweep)
        {
            float[] arr = null;
            try
            {
                float[] sweep_max, sweep_min;
                sweep_max = new float[TraceSize];
                sweep_min = new float[TraceSize];
                arr = new float[TraceSize];
                for (int j = 0; j < n_sweep; j++)
                {
                    status = bb_api.bbFetchTrace_32f(id_dev, unchecked((int)TraceSize), sweep_min, sweep_max);
                    if (j == 0)
                    {
                        for (int i = 0; i < TraceSize; i++)
                        {
                            arr[i] = sweep_max[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < TraceSize; i++)
                        {
                            if (arr[i] < sweep_max[i]) { arr[i] = sweep_max[i]; }
                        }
                    }
                }
            }
            catch { arr = null; }
            return arr;
        }
        #endregion

        #region SetConfigurationForMeasurements
        public bool SetConfiguration (SDRParameters sDRParameters)
        {
            bool done = false;
            try
            {
                if (ValidationTaskForSDR(ref sDRParameters))
                {
                    SetParameterForMeasurements(ref sDRParameters);// установка параметров для класса

                    // настройка конфигурации оборудования
                    if (sDRParameters.MeasurementType == MeasurementType.SoundID) // КОСТЫЛЬ пока не будет изменен контракт
                    {
                        done = put_config_for_IQ();
                    }
                    else {
                        if (sDRParameters.TypeSpectrumScan == Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.RealTime)
                        { // RealTimeMeasurement
                            done = put_config_for_RT();
                        }
                        else
                        { // SweepMeasurement
                            done = put_config_for_sweep();
                        }
                    }
                    
                }
            }
            catch {}
            return done;
        }
        /// <summary>
        /// Установка констант для начала роботы
        /// </summary>
        /// <param name="sDRParameters">параметры для инициализации роботы SDR</param>
        private void SetParameterForMeasurements(ref SDRParameters sDRParameters)
        {
            if (sDRParameters.DetectTypeSDR == Atdi.AppServer.Contracts.Sdrns.DetectingType.Avarage) { DETECT_TYPE = bb_api.BB_AVERAGE; } else { DETECT_TYPE = bb_api.BB_MIN_AND_MAX; }
            // Установка частот 
            f_max = sDRParameters.MaxFreq_MHz;
            f_min = sDRParameters.MinFreq_MHz;
            // Доп параметры
            TypeSpectrumScan = sDRParameters.TypeSpectrumScan;
            ref_level_dbm = sDRParameters.RefLevel_dBm;
            switch (sDRParameters.RfAttenuationSDR) { case 0: atten = 0; break; case 10: atten = 10; break; case 20: atten = 20; break; case 30: atten = 30; break; default: atten = bb_api.BB_AUTO_ATTEN; break; }
            switch (sDRParameters.PreamplificationSDR) { case 0: gain_SDR = 0; break; case 10: gain_SDR = 10; break; case 20: gain_SDR = 20; break; default: atten = bb_api.BB_AUTO_GAIN; break; }
            RBW = sDRParameters.RBW_Hz;
            VBW = sDRParameters.VBW_Hz;
            Time_of_m = sDRParameters.MeasTime_Sec;
            sw_time = sDRParameters.SwNumber;
            BW_CH = sDRParameters.StepSO_kHz; // обязательный параметер для SO (типа ширина канала или шаг сетки частот)
            LastTaskId = sDRParameters.TaskId;
            switch (sDRParameters.MeasurementType)
            {
                case MeasurementType.Level:
                    traceType = TraceType.Average;
                    break;
                case MeasurementType.BandwidthMeas:
                    traceType = TraceType.MaxHold;
                    break;
                case MeasurementType.SpectrumOccupation:
                    traceType = TraceType.Average;
                    break;
                default:
                    traceType = TraceType.Average;
                    break;
            }
        }
        /// <summary>
        /// Настройка физических параметров SDR для sweep
        /// </summary>
        private bool put_config_for_sweep()
        {
            bool done = false;
            try
            {
                bb_api.bbConfigureAcquisition(id_dev, DETECT_TYPE, bb_api.BB_LOG_SCALE);
                bb_api.bbConfigureCenterSpan(id_dev, (f_max * 1000000 + f_min * 1000000) / 2, f_max * 1000000 - f_min * 1000000);
                bb_api.bbConfigureLevel(id_dev, ref_level_dbm, atten);
                bb_api.bbConfigureGain(id_dev, gain_SDR);
                bb_api.bbConfigureSweepCoupling(id_dev, RBW, VBW, Time_of_m, rbwShape, bb_api.BB_NO_SPUR_REJECT);
                bb_api.bbConfigureProcUnits(id_dev, bb_api.BB_LOG);
                status = bb_api.bbInitiate(id_dev, bb_api.BB_SWEEPING, 0);
                if (status != bbStatus.bbNoError) { return false; }
                TraceSize = 0;
                bin_size = 0.0;
                start_freq = 0.0;
                status = bb_api.bbQueryTraceInfo(id_dev, ref TraceSize, ref bin_size, ref start_freq);
                if (status != bbStatus.bbNoError) { return false; }
                double a = 1;
                while (!((bin_size * 0.5 < Math.Min(RBW, VBW)) && (Math.Min(RBW, VBW) < bin_size * 1.5)))
                {
                    if (Math.Min(RBW, VBW) > bin_size * 1.5) { a = a * 1.7;}
                    if (Math.Min(RBW, VBW) < bin_size * 0.5) { a = a / 1.7; }
                    bb_api.bbConfigureSweepCoupling(id_dev, a*RBW, a*VBW, Time_of_m, rbwShape, bb_api.BB_NO_SPUR_REJECT);
                    status = bb_api.bbInitiate(id_dev, bb_api.BB_SWEEPING, 0);
                    status = bb_api.bbQueryTraceInfo(id_dev, ref TraceSize, ref bin_size, ref start_freq);
                    if (status != bbStatus.bbNoError) { return false; }
                }
                done = true;
            }
            catch {}
            return done;
        }
        /// <summary>
        /// Настройка физических параметров SDR для RT
        /// </summary>
        private bool put_config_for_RT()
        {
            bool done = false;
            try
            {
                bb_api.bbConfigureAcquisition(id_dev, DETECT_TYPE, bb_api.BB_LOG_SCALE);  //
                bb_api.bbConfigureCenterSpan(id_dev, (f_max * 1000000 + f_min * 1000000) / 2, f_max * 1000000 - f_min * 1000000);           //
                bb_api.bbConfigureLevel(id_dev, ref_level_dbm, atten);           //
                bb_api.bbConfigureGain(id_dev, gain_SDR);                            //
                bb_api.bbConfigureSweepCoupling(id_dev, RBW, VBW, Time_of_m, rbwShape, bb_api.BB_NO_SPUR_REJECT); //
                bb_api.bbConfigureRealTime(id_dev, 100.0, 30);//
                status = bb_api.bbInitiate(id_dev, bb_api.BB_REAL_TIME, 0); //
                if (status != bbStatus.bbNoError) { return false; }
                TraceSize = 0;
                bin_size = 0.0;
                start_freq = 0.0;
                status = bb_api.bbQueryTraceInfo(id_dev, ref TraceSize, ref bin_size, ref start_freq);
                frameWidth = 0;
                frameHeight = 0;
                status = bb_api.bbQueryRealTimeInfo(id_dev, ref frameWidth, ref frameHeight);
                if (status != bbStatus.bbNoError) { return false; }
                done = true;
            }
            catch {}
            return done;
        }
        private bool put_config_for_IQ()
        {
            bool done = false;
            try
            {
                // пока параметры будут константами временное решение для тестирования
                int downsampleFactor = 1; //Коэфициент прореживания IQ потока
                bb_api.bbConfigureLevel(id_dev, ref_level_dbm, atten);
                bb_api.bbConfigureGain(id_dev, gain_SDR);
                bb_api.bbConfigureCenterSpan(id_dev, (f_max * 1000000 + f_min * 1000000) / 2, f_max * 1000000 - f_min * 1000000);
                bb_api.bbConfigureIQ(id_dev, downsampleFactor, f_max * 1000000 - f_min * 1000000);
                status = bb_api.bbInitiate(id_dev, bb_api.BB_STREAMING, bb_api.BB_STREAM_IQ);
                if (status != bbStatus.bbNoError) { return false; } //Выход с ошибкой 
                return_len = 0; samples_per_sec = 0; bandwidth = 0.0;
                bb_api.bbQueryStreamInfo(id_dev, ref return_len, ref bandwidth, ref samples_per_sec);
                done = true;
            }
            catch { }
            return done;
        }
        public bool ChangeSpan(double MinFrequency_MHz, double MaxFrequency_MHz)
        {
            bool done = false;
            try
            {
                this.f_min = MinFrequency_MHz;
                this.f_max = MaxFrequency_MHz;
                bb_api.bbConfigureCenterSpan(id_dev, (f_max * 1000000 + f_min * 1000000) / 2, f_max * 1000000 - f_min * 1000000);
                status = bb_api.bbQueryTraceInfo(id_dev, ref TraceSize, ref bin_size, ref start_freq);
                if (status != bbStatus.bbNoError) { return false; }
            }
            catch {}
            return done;
        }
        public bool ChangeSweepCoupling(double RBW_Hz, double VBW_Hz)
        {
            bool done = false;
            try
            {
                this.RBW = RBW_Hz;
                this.VBW = VBW_Hz;
                bb_api.bbConfigureSweepCoupling(id_dev, RBW, VBW, Time_of_m, rbwShape, bb_api.BB_NO_SPUR_REJECT);
                status = bb_api.bbQueryTraceInfo(id_dev, ref TraceSize, ref bin_size, ref start_freq);
                if (status != bbStatus.bbNoError) { return false; }
            }
            catch { }
            return done;
        }
        public bool ChangeSweepGain(int Gain)
        {
            bool done = false;
            if (Gain > 0) { Gain = (int)(10 * Math.Round(Gain / 10.0)); }
            try
            {
                if (Gain > 30) { this.gain_SDR = 30; }
                else if (Gain < 0) { this.gain_SDR = bb_api.BB_AUTO_GAIN; }
                else { this.gain_SDR = Gain;}
                bb_api.bbConfigureGain(id_dev, gain_SDR);
                status = bb_api.bbQueryTraceInfo(id_dev, ref TraceSize, ref bin_size, ref start_freq);
                if (status != bbStatus.bbNoError) { return false; }
            }
            catch { }
            return done;
        }
        public bool ChangeSweepAtt(double Att)
        {
            bool done = false;
            if (Att > 0) { Att = (int)(10 * Math.Round(Att / 10.0)); }
            try
            {
                if (Att > 30) { this.atten = 30; }
                else if (Att < 0) { this.atten = bb_api.BB_AUTO_ATTEN; }
                else { this.atten = Att; }
                bb_api.bbConfigureLevel(id_dev, ref_level_dbm, atten);
                status = bb_api.bbQueryTraceInfo(id_dev, ref TraceSize, ref bin_size, ref start_freq);
                if (status != bbStatus.bbNoError) { return false; }
            }
            catch { }
            return done;
            throw new NotImplementedException();
            
        }
        public bool ChangeSweepRefLevel(double RefLevel_dBm)
        {
            RefLevel_dBm = (int)(10 * Math.Round(RefLevel_dBm / 10.0)); 
            bool done = false;
            try
            {
                if (RefLevel_dBm > 20) { this.ref_level_dbm = 20; }
                else if (RefLevel_dBm < -130) { this.ref_level_dbm = -130; }
                else { this.ref_level_dbm = RefLevel_dBm; }
                bb_api.bbConfigureLevel(id_dev, ref_level_dbm, atten);
                status = bb_api.bbQueryTraceInfo(id_dev, ref TraceSize, ref bin_size, ref start_freq);
                if (status != bbStatus.bbNoError) { return false; }
            }
            catch { }
            return done;
            throw new NotImplementedException();
        }
        #endregion

        #region ValidationTaskForMeasurements
        /// <summary>
        /// Валидация коректности параметров перед конфигурацией измерения
        /// </summary>
        /// <returns></returns>
        private bool ValidationTaskForSDR(ref SDRParameters SDRParameters)
        {
            // пока еще не написана данная функция
            return true;
        }
        #endregion

        #region Location
        public MeasSdrLoc GetSDRLocation() // функция должна возращать текущие координаты сенсора пока с костылем
        {
            MeasSdrLoc loc = new MeasSdrLoc();
            //////
            loc.Lon = 30; // костыль
            loc.Lat = 50; // костыль
            loc.ASL = 150; // костыль
            return (loc);
        }

        
        #endregion
    }
}
