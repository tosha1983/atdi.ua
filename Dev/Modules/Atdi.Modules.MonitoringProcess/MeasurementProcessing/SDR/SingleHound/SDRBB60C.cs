using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Atdi.AppServer.Contracts.Sdrns;
using Atdi.Modules.MonitoringProcess;
using Atdi.Modules.MonitoringProcess.ProcessSignal;

namespace Atdi.Modules.MonitoringProcess.SingleHound
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
        private int return_len; // IQ
        private int samples_per_sec; // IQ
        private double bandwidth; // IQ
        private int downsampleFactor; // IQ



        private bbStatus status = bbStatus.bbNoError;

        private SpectrScanType TypeSpectrumScan = SpectrScanType.Sweep;
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
        private int LastPPSTime_sec;
        private int LastPPSTime_nano = -1;

        #region IQStream
        unsafe public bool GetIQStream(ref ReceivedIQStream receivedIQStream, double durationReceiving_sec = -1, bool AfterPPS = false, bool JustWithSignal = false)
        {
            bool done = false;
            try
            {
                // константа
                int max_count = 500000;
                // конец констант

                int NumberPass = 0;
                if (durationReceiving_sec < 0) { NumberPass = 1; } else { NumberPass = (int)Math.Ceiling(durationReceiving_sec * samples_per_sec / return_len); }
                receivedIQStream = new ReceivedIQStream();
                receivedIQStream.TimeMeasStart = DateTime.Now;
                receivedIQStream.iq_samples = new List<float[]>();
                receivedIQStream.triggers = new List<int[]>();
                receivedIQStream.dataRemainings = new List<int>();
                receivedIQStream.sampleLosses = new List<int>();
                receivedIQStream.iqSeces = new List<int>();
                receivedIQStream.iqNanos = new List<int>();
                List<float[]> IQData = new List<float[]>();
                List<int []> TrData = new List<int[]>();
                List<int> dataRemainings = new List<int>();
                List<int> sampleLosses = new List<int>();
                List<int> iqSeces = new List<int>();
                List<int> iqNanos = new List<int>();
                for (int i = 0; i < NumberPass; i++)
                {
                    float[] iqSamplesX = new float[return_len * 2];
                    int[] triggersX = new int[71];
                    IQData.Add(iqSamplesX);
                    TrData.Add(triggersX);
                    dataRemainings.Add(-1);
                    sampleLosses.Add(-1);
                    iqSeces.Add(-1);
                    iqNanos.Add(-1);
                }
                int dataRemaining = 0, sampleLoss = 0, iqSec = 0, iqNano = 0; 
                int count = 0; int number_bloks = 0;
                for (int i = 0; i < NumberPass; i++)
                {
                    bb_api.bbGetIQUnpacked(id_dev, IQData[i], return_len, TrData[i], 71, 1,
                            ref dataRemaining, ref sampleLoss, ref iqSec, ref iqNano);
                    
                    dataRemainings[i] = dataRemaining;
                    sampleLosses[i] = sampleLoss;
                    iqSeces[i] = iqSec;
                    iqNanos[i] = iqNano;
                    if (TrData[i][0] != 0)
                    {
                        AfterPPS = false;
                        LastPPSTime_sec = iqSec;
                        LastPPSTime_nano = iqNano + TrData[i][0]*(downsampleFactor * 25);
                        //if (JustWithSignal)
                        //{max_count = NumberPass; count = 0;}
                    }
                    else
                    {
                        if (LastPPSTime_nano != -1)
                        {// В данном случае у нас был PPS ранее
                            if (iqSec - LastPPSTime_sec < 10)
                            {// еще не успело сильно растроиться время
                                TrData[i][0] = -(int)((iqNano - LastPPSTime_nano) / (downsampleFactor * 25));
                                if (TrData[i][0] > 0) { TrData[i][0] = (- 1000000000/ (downsampleFactor * 25)) + TrData[i][0];}
                            }
                        }
                    }
                    if (AfterPPS)
                    {
                        i--;
                    }
                    //else
                    //{
                    //    if (TrData[i][0] == 0)
                    //    {
                    //        i--;
                    //    }
                    //}
                    if ((JustWithSignal)&&(!AfterPPS))
                    {
                        // Константы
                        double noise = 0.00001; // уровень шума в mW^2
                        double SN = 10; // превышение шума в разах 
                        // Конец констант 
                        bool signal = false;
                        int step = (int)(IQData[i].Length / 1000);
                        if (step < 1) { step = 1; }
                        double TrigerLevel = noise * SN;
                        for (int j = 0; IQData[i].Length - 6 > j; j = j + step)
                        {
                            if ((IQData[i][j] >= TrigerLevel) || (IQData[i][j + 1] >= TrigerLevel))
                            {
                                if ((IQData[i][j + 2] >= TrigerLevel) || (IQData[i][j + 3] >= TrigerLevel))
                                {
                                    if ((IQData[i][j + 4] >= TrigerLevel) || (IQData[i][j + 5] >= TrigerLevel))
                                    {
                                        signal = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!signal)
                        {
                            i--;
                        }
                    }
                        count++;
                    number_bloks = i+1;
                    if (count >= max_count) { break;}
                    
                }
                for (int i = 0; i < number_bloks; i++)
                {
                    float[] iq_sample = new float[return_len * 2];
                    int[] trigger = new int[80];
                    for (int j = 0; j < return_len * 2; j++)
                    {
                        iq_sample[j] = IQData[i][j];
                    }
                    for (int j = 0; j < 10; j++)
                    {
                        trigger[j] = TrData[i][j];
                        if (trigger[j] == 0) { break; }
                    }
                    receivedIQStream.iq_samples.Add(iq_sample);
                    receivedIQStream.triggers.Add(trigger);
                    receivedIQStream.dataRemainings.Add(dataRemainings[i]);
                    receivedIQStream.sampleLosses.Add(sampleLosses[i]);
                    receivedIQStream.iqSeces.Add(iqSeces[i]);
                    receivedIQStream.iqNanos.Add(iqNanos[i]);

                }
                receivedIQStream.durationReceiving_sec = durationReceiving_sec;
                done = true;
            }
            catch
            {
                receivedIQStream = null;
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
            if (TypeSpectrumScan == SpectrScanType.Sweep)
            {
                if (traceType == TraceType.Average) { Trace = SweepAvarageTrace(sw_time); }
                if (traceType == TraceType.MaxHold) { Trace = SweepMaxHoldTrace(sw_time); }
            }
            if (TypeSpectrumScan == SpectrScanType.RealTime)
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
                    if ((sDRParameters.MeasurementType == MeasType.IQReceive)||(sDRParameters.MeasurementType == MeasType.Timetimestamp)) // КОСТЫЛЬ пока не будет изменен контракт
                    {
                        done = put_config_for_IQ();
                    }
                    else {
                        if (sDRParameters.TypeSpectrumScan == SpectrScanType.RealTime)
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
            if (sDRParameters.DetectTypeSDR == DetectType.Avarage) { DETECT_TYPE = bb_api.BB_AVERAGE; } else { DETECT_TYPE = bb_api.BB_MIN_AND_MAX; }
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
                case MeasType.Level:
                    traceType = TraceType.Average;
                    break;
                case MeasType.BandwidthMeas:
                    traceType = TraceType.MaxHold;
                    break;
                case MeasType.SpectrumOccupation:
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
                downsampleFactor = getDownsampleFactor(f_max * 1000 - f_min * 1000); //Коэфициент прореживания IQ потока
                bb_api.bbConfigureLevel(id_dev, ref_level_dbm, atten);
                bb_api.bbConfigureGain(id_dev, gain_SDR);
                bb_api.bbConfigureCenterSpan(id_dev, (f_max * 1000000 + f_min * 1000000) / 2, f_max * 1000000 - f_min * 1000000);
                bb_api.bbConfigureIQ(id_dev, downsampleFactor, f_max * 1000000 - f_min * 1000000);
                bb_api.bbConfigureIO(id_dev, 0, bb_api.BB_PORT2_IN_TRIGGER_RISING_EDGE);
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
        private int getDownsampleFactor(double BW_kHz)// подлежит Тестированию и уточнению
        {
            if (BW_kHz >= 200) { return 1; }
            if (BW_kHz >= 50) { return 2; }
            if (BW_kHz >= 20) { return 4; }
            return 8;
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
        public SDRLoc GetSDRLocation() // функция должна возращать текущие координаты сенсора пока с костылем
        {
            SDRLoc loc = new SDRLoc();
            //////
            loc.Lon = 30; // костыль
            loc.Lat = 50; // костыль
            loc.ASL = 150; // костыль
            return (loc);
        }
        #endregion
    }
}
