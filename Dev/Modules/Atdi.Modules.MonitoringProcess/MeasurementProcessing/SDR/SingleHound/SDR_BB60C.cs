//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;
//using System.Drawing.Drawing2D;
//using System.IO;
//using System.IO.Compression;
////using Atdi.AppServer.Contracts.Sdrns;
//using System.Threading.Tasks;
//using Atdi.Modules.MonitoringProcess.SingleHound.ProcessSignal;
//using Atdi.Modules.MonitoringProcess;

//namespace Atdi.Modules.MonitoringProcess.SingleHound
//{

//    public partial class SDR_BB60C
//    {

//        //параметры класса
//        #region parameters
//        public int count = 0;
//        public System.Timers.Timer tm_curr = new System.Timers.Timer();
//        public int TimeOnlineCalculation = 0;
//        public int IdResult = 0; // идентификатор для MeasSDRResults
//        public string error = null; // описание ошибки
//        public int count_critical_err = 0; // количество критических ошибок в работе устройства
//        public bbStatus status = bbStatus.bbNoError;
//        public int N;
//        //public int TypeFunction; //1,2
//        public bool isAbort;
//        public bool isError;
//        private int sw_time;
//        private Atdi.AppServer.Contracts.Sdrns.SpectrumScanType Type_of_m = Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.Sweep;
//        public int id_dev = 999; //ID признак выключенного устройства.
//        private Double f_min = 103;//MHz
//        private Double f_max = 105;//MHz
//        private Double RBW = 10000;//Hz
//        private Double VBW = 10000;//Hz
//        private Double Time_of_m = 0.001; // sek
//        private Double ref_level_dbm = -30;
//        private Double bin_size;//
//        private Double start_freq;//
//        private List<Double> List_freq_CH = new List<Double>();//MHz // перечень частот для измерения SO
//        private Double BW_CH;//kHz для измерения  SO
//        private Double Level_min_occup = -100; // dBm минимальный уровень для расчета занятости канала
//        private Atdi.AppServer.Contracts.Sdrns.SpectrumOccupationType Type_of_SO = Atdi.AppServer.Contracts.Sdrns.SpectrumOccupationType.FreqChannelOccupation; // Тип занятости канала
//        private uint trace_len;//
//        private uint sweepsize;//
//        private int frameWidth;//
//        private int frameHeight;//
//        private int n_in_chenal;
//        public int IdLastMeasTask;  //идентификатор таска который последним был на обработке
//        public List<FSemples> F_semples = new List<FSemples>();
//        public MeasSdrResults MEAS_SDR_RESULTS = new MeasSdrResults();
//        private uint DETECT_TYPE = bb_api.BB_AVERAGE;
//        private Double atten = bb_api.BB_AUTO_ATTEN;
//        private int gain_SDR = bb_api.BB_AUTO_GAIN;
//        public bool isOnlineCalc = false;
//        public bool isTaskFinished = false;
//        private uint rbwShape = bb_api.BB_NUTALL;
//        #endregion
//        public SDR_BB60C()
//        {
//            tm_curr.Interval = 1000;
//            tm_curr.Elapsed += Tm_curr_Elapsed;
//        }
//        private void Tm_curr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
//        {
//            TimeOnlineCalculation++;
//        }
//        #region initialization
//        /// <summary>
//        /// Инициализация сенсора;
//        /// </summary>
//        public bool initiation_SDR()
//        {
//            bool done = false;
//            try
//            {
//                if (id_dev == 999)
//                    status = bb_api.bbOpenDevice(ref id_dev);
//                if (status != bbStatus.bbNoError) { error = "Error of initialization"; } else { done = true; }
//            }
//            catch (Exception ex)
//            { //GlobalInit.log.Trace("initiation_SDR:" + ex.Message); 
//            }
//            return done;
//        }
//        /// <summary>
//        /// Калибровка сенсора
//        /// </summary>
//        public bool calibration()
//        {
//            bool done = false;
//            try
//            {
//                status = bb_api.bbSelfCal(id_dev);
//                if (status != bbStatus.bbNoError) { error = "Error of calibration"; } else { done = true; }
//            }
//            catch (Exception ex)
//            { //GlobalInit.log.Trace("calibration:" + ex.Message); 
//            }
//            return done;
//        }
//        public void Close_dev()
//        {
//            try
//            {
//                bb_api.bbCloseDevice(id_dev);
//                id_dev = 999;
//            }
//            catch (Exception ex)
//            { //GlobalInit.log.Trace("Close_dev" + ex.Message); 
//            }
//        }
//        public void ResetDevice()
//        {
//            try
//            {
//                bb_api.OffUsbDevice();
//                System.Threading.Thread.Sleep(2000);
//                bb_api.OnUsbDevice();
//                System.Threading.Thread.Sleep(5000);
//                id_dev = 999;
//            }
//            catch (Exception ex)
//            { //GlobalInit.log.Trace("Close_dev" + ex.Message); 
//            }
//        }
//        #endregion
//        #region SetConfigurationForMeasurements
//        /// <summary>
//        /// MainFunctionForPutConfiguration
//        /// </summary>
//        /// <param name="taskSDR"></param>
//        public bool PutConfiguration(ref SDRParameters sDRParameters)
//        {
//            bool done = false;
//            try
//            {
//                if (ValidationTaskForSDR(ref sDRParameters))
//                {
//                    SetParameterForMeasurements(ref sDRParameters);// установка параметров для класса
//                    // настройка конфигурации оборудованию
//                    if (sDRParameters.TypeSpectrumScan == Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.RealTime)
//                    { // RealTimeMeasurement
//                        put_config_for_RT();
//                    }
//                    else
//                    { // SweepMeasurement
//                        put_config_for_sweep();
//                    }
//                }
//                done = true;
//            }
//            catch (Exception ex)
//            { //GlobalInit.log.Trace("PutConfiguration" + ex.Message); 
//            }
//            return done;
//        }
//        /// <summary>
//        /// Установка констант для начала роботы
//        /// </summary>
//        /// <param name="sDRParameters">параметры для инициализации роботы SDR</param>
//        private void SetParameterForMeasurements(ref SDRParameters sDRParameters)
//        {
//            if (sDRParameters.DetectTypeSDR == Atdi.AppServer.Contracts.Sdrns.DetectingType.Avarage) { DETECT_TYPE = bb_api.BB_AVERAGE; } else { DETECT_TYPE = bb_api.BB_MIN_AND_MAX; }
//            // Установка частот 
//            f_max = sDRParameters.MaxFreq_MHz;
//            f_min = sDRParameters.MinFreq_MHz;
//            // Доп параметры
//            Type_of_m = sDRParameters.TypeSpectrumScan;
//            ref_level_dbm = sDRParameters.RefLevel_dBm;
//            switch (sDRParameters.RfAttenuationSDR) { case 0: atten = 0; break; case 10: atten = 10; break; case 20: atten = 20; break; case 30: atten = 30; break; default: atten = bb_api.BB_AUTO_ATTEN; break; }
//            switch (sDRParameters.RfAttenuationSDR) { case 0: gain_SDR = 0; break; case 10: gain_SDR = 10; break; case 20: gain_SDR = 20; break; default: atten = bb_api.BB_AUTO_GAIN; break; }
//            RBW = sDRParameters.RBW_Hz;
//            VBW = sDRParameters.VBW_Hz;
//            Time_of_m = sDRParameters.MeasTime_Sec;
//            sw_time = sDRParameters.SwNumber;
//            BW_CH = sDRParameters.StepSO_kHz; // обязательный параметер для SO (типа ширина канала или шаг сетки частот)
//            Type_of_SO = sDRParameters.Type_of_SO;
//        }
//        /// <summary>
//        /// Настройка физических параметров SDR для sweep
//        /// </summary>
//        private void put_config_for_sweep()
//        {
//            try
//            {
//                bb_api.bbConfigureAcquisition(id_dev, DETECT_TYPE, bb_api.BB_LOG_SCALE);
//                bb_api.bbConfigureCenterSpan(id_dev, (f_max * 1000000 + f_min * 1000000) / 2, f_max * 1000000 - f_min * 1000000);
//                bb_api.bbConfigureLevel(id_dev, ref_level_dbm, atten);
//                bb_api.bbConfigureGain(id_dev, gain_SDR);
//                bb_api.bbConfigureSweepCoupling(id_dev, RBW, VBW, Time_of_m, rbwShape, bb_api.BB_NO_SPUR_REJECT);
//                bb_api.bbConfigureProcUnits(id_dev, bb_api.BB_LOG);
//                status = bb_api.bbInitiate(id_dev, bb_api.BB_SWEEPING, 0);
//                if (status != bbStatus.bbNoError) { return; }
//                trace_len = 0;
//                bin_size = 0.0;
//                start_freq = 0.0;
//                status = bb_api.bbQueryTraceInfo(id_dev, ref trace_len, ref bin_size, ref start_freq);
//                // if (status != bbStatus.bbNoError) GlobalInit.log.Trace("Status BB60C:" + status);
//                if (status != bbStatus.bbNoError) { return; }
//            }
//            catch (Exception ex)
//            { //GlobalInit.log.Trace("put_config_for_sweep" + ex.Message); 
//            }
//        }
//        /// <summary>
//        /// Настройка физических параметров SDR для RT
//        /// </summary>
//        private void put_config_for_RT()
//        {
//            try
//            {
//                bb_api.bbConfigureAcquisition(id_dev, DETECT_TYPE, bb_api.BB_LOG_SCALE);  //
//                bb_api.bbConfigureCenterSpan(id_dev, (f_max * 1000000 + f_min * 1000000) / 2, f_max * 1000000 - f_min * 1000000);           //
//                bb_api.bbConfigureLevel(id_dev, ref_level_dbm, atten);           //
//                bb_api.bbConfigureGain(id_dev, gain_SDR);                            //
//                bb_api.bbConfigureSweepCoupling(id_dev, RBW, VBW, Time_of_m, rbwShape, bb_api.BB_NO_SPUR_REJECT); //
//                bb_api.bbConfigureRealTime(id_dev, 100.0, 30);//
//                status = bb_api.bbInitiate(id_dev, bb_api.BB_REAL_TIME, 0); //
//                if (status != bbStatus.bbNoError) { return; }
//                sweepsize = 0;
//                bin_size = 0.0;
//                start_freq = 0.0;
//                status = bb_api.bbQueryTraceInfo(id_dev, ref sweepsize, ref bin_size, ref start_freq);
//                frameWidth = 0;
//                frameHeight = 0;
//                status = bb_api.bbQueryRealTimeInfo(id_dev, ref frameWidth, ref frameHeight);
//                //if (status != bbStatus.bbNoError) GlobalInit.log.Trace("Status BB60C:" + status);
//            }
//            catch (Exception ex)
//            { //GlobalInit.log.Trace("put_config_for_RT" + ex.Message); 
//            }
//        }
//        #endregion
//        #region ValidationTaskForMeasurements

//        /// <summary>
//        /// Валидация коректности параметров перед конфигурацией измерения
//        /// </summary>
//        /// <returns></returns>
//        private bool ValidationTaskForSDR(ref SDRParameters SDRParameters)
//        {
//            // пока еще не написана данная функция
//            return true;
//        }
//        #endregion
//        #region Location
//        public MeasSdrLoc get_loc_from_sensor() // функция должна возращать текущие координаты сенсора пока с костылем
//        {
//            MeasSdrLoc loc = new MeasSdrLoc();
//            //////
//            loc.Lon = 30; // костыль
//            loc.Lat = 50; // костыль
//            loc.ASL = 150; // костыль
//            return (loc);
//        }
//        #endregion
//        #region Measurements
//        private void Real_time(int n_RT)
//        {
//            try
//            {
//                F_semples.Clear();
//                float[] sweep, frame, arr;
//                sweep = new float[sweepsize];
//                frame = new float[frameWidth * frameHeight];
//                arr = new float[sweepsize];
//                for (int j = 0; j < n_RT; j++)
//                {
//                    bb_api.bbFetchRealTimeFrame(id_dev, sweep, frame);
//                    for (int i = 0; i < sweepsize; i++)
//                    {
//                        arr[i] = ((j) * arr[i] + sweep[i]) / (j + 1);
//                    }
//                }
//                for (int i = 0; i < sweepsize; i++)
//                {
//                    FSemples semple = new FSemples();
//                    semple.Freq = (float)((start_freq + i * bin_size) / 1000000.0);
//                    semple.LeveldBm = arr[i];
//                    semple.Id = i;
//                    F_semples.Add(semple);
//                }
//            }
//            catch (Exception ex)
//            { //GlobalInit.log.Trace("Real_time" + ex.Message); 
//            }
//        }
//        private void Real_time_MAX_HOLD(int n_RT)
//        {
//            try
//            {
//                F_semples.Clear();
//                float[] sweep, frame, arr;
//                sweep = new float[sweepsize];
//                frame = new float[frameWidth * frameHeight];
//                arr = new float[sweepsize];
//                for (int j = 0; j < n_RT; j++)
//                {
//                    bb_api.bbFetchRealTimeFrame(id_dev, sweep, frame);
//                    for (int i = 0; i < sweepsize; i++)
//                    {
//                        if (arr[i] < sweep[i]) { arr[i] = sweep[i]; }
//                    }
//                }
//                for (int i = 0; i < sweepsize; i++)
//                {
//                    FSemples semple = new FSemples();
//                    semple.Freq = (float)((start_freq + i * bin_size) / 1000000.0);
//                    semple.LeveldBm = arr[i];
//                    semple.Id = i;
//                    F_semples.Add(semple);
//                }
//            }
//            catch (Exception ex)
//            { //GlobalInit.log.Trace("Real_time" + ex.Message); 
//            }
//        }
//        private void Sweep(int n_sweep)
//        {
//            try
//            {
//                F_semples.Clear();
//                float[] sweep_max, sweep_min, arr;
//                sweep_max = new float[trace_len];
//                sweep_min = new float[trace_len];
//                arr = new float[trace_len];
//                for (int j = 0; j < n_sweep; j++)
//                {
//                    status = bb_api.bbFetchTrace_32f(id_dev, unchecked((int)trace_len), sweep_min, sweep_max);
//                    if (status != bbStatus.bbNoError) { return; }
//                    for (int i = 0; i < trace_len; i++)
//                    {
//                        arr[i] = ((j) * arr[i] + sweep_max[i]) / (j + 1);
//                    }
//                }

//                for (int i = 0; i < trace_len; i++)
//                {
//                    FSemples semple = new FSemples();
//                    semple.Freq = (float)((start_freq + i * bin_size) / 1000000.0);
//                    semple.LeveldBm = arr[i];
//                    semple.Id = i;
//                    F_semples.Add(semple);
//                }
//            }
//            catch (Exception ex)
//            { //GlobalInit.log.Trace("Sweep" + ex.Message); 
//            }

//        }
//        private void Sweep_MAX_HOLD(int n_sweep)
//        {
//            try
//            {
//                F_semples.Clear();
//                float[] sweep_max, sweep_min, arr;
//                sweep_max = new float[trace_len];
//                sweep_min = new float[trace_len];
//                arr = new float[trace_len];
//                for (int j = 0; j < n_sweep; j++)
//                {
//                    status = bb_api.bbFetchTrace_32f(id_dev, unchecked((int)trace_len), sweep_min, sweep_max);
//                    if (status != bbStatus.bbNoError) { return; }
//                    if (j == 0)
//                    {
//                        for (int i = 0; i < trace_len; i++)
//                        {
//                            arr[i] = sweep_max[i];
//                        }
//                    }
//                    else
//                    {
//                        for (int i = 0; i < trace_len; i++)
//                        {
//                            if (arr[i] < sweep_max[i]) { arr[i] = sweep_max[i]; }
//                        }
//                    }
//                }

//                for (int i = 0; i < trace_len; i++)
//                {
//                    FSemples semple = new FSemples();
//                    semple.Freq = (float)((start_freq + i * bin_size) / 1000000.0);
//                    semple.LeveldBm = arr[i];
//                    semple.Id = i;
//                    F_semples.Add(semple);
//                }
//            }
//            catch (Exception ex)
//            { //GlobalInit.log.Trace("Sweep" + ex.Message); 
//            }
//        }
//        #endregion
//        #region MeasurementsProcessing
//        /// <summary>
//        /// MainFunctionForMeasurementsProccessing
//        /// </summary>
//        /// <param name="taskSDR"></param>
//        /// <param name="sensor"></param>
//        /// <param name="LastSDRresult"></param>
//        public void process_meas_BB60C(ref MeasSdrTask taskSDR, Atdi.AppServer.Contracts.Sdrns.Sensor sensor = null, MeasSdrResults LastSDRresult = null) // функция для расчета 
//        {
//            //try
//            //{
//            // выполнение функкций ////////////////////////////////////////
//            // заполнение базовых вещей в measResult
//            MEAS_SDR_RESULTS.MeasSubTaskId = taskSDR.MeasSubTaskId;
//            MEAS_SDR_RESULTS.MeasSubTaskStationId = taskSDR.MeasSubTaskStationId;
//            MEAS_SDR_RESULTS.MeasTaskId = taskSDR.MeasTaskId;
//            MEAS_SDR_RESULTS.DataMeas = DateTime.Now;
//            MEAS_SDR_RESULTS.MeasDataType = taskSDR.MeasDataType;
//            MEAS_SDR_RESULTS.SwNumber = sw_time;
//            MEAS_SDR_RESULTS.status = "N";
//            if (sensor != null)
//            {
//                MEAS_SDR_RESULTS.SensorId = sensor.Id;
//            }
//            // сканирование спектра 
//            if (taskSDR.MeasDataType == Atdi.AppServer.Contracts.Sdrns.MeasurementType.Level)
//            {
//                // измерение уровня сигнала.

//                if (taskSDR.status == "O")
//                {// измерения online 
//                    MeasScanOnline(sensor, LastSDRresult);
//                }
//                else
//                { // измерения 
//                    MeasScan(sensor, LastSDRresult);
//                }

//            }
//            //занатие полос частот и каналов
//            if (taskSDR.MeasDataType == Atdi.AppServer.Contracts.Sdrns.MeasurementType.SpectrumOccupation)
//            {
//                MeasSO(ref taskSDR, sensor, LastSDRresult);
//            }
//            if (taskSDR.MeasDataType == Atdi.AppServer.Contracts.Sdrns.MeasurementType.BandwidthMeas)
//            {
//                MeasBW(ref taskSDR, sensor, LastSDRresult);
//            }

//            //}
//            //catch (Exception ex) { GlobalInit.log.Trace("process_meas_BB60C" + ex.Message); }
//            return;
//        }
//        public void CalcFsFormLevel(ref List<FSemples> F_Sem, Sensor sensor = null)
//        {
//            try
//            {
//                Double ANT_VAL = 3;
//                if (sensor != null)
//                {
//                    Double Rx = 0;
//                    if (sensor.RxLoss != null) { Rx = sensor.RxLoss.Value; }
//                    Double Gain = 3;
//                    if (sensor.Antenna != null)
//                    {
//                        Gain = sensor.Antenna.GainMax; // Пока костыль, но мы его изменим
//                    }
//                    ANT_VAL = Gain - Rx;
//                }
//                for (int i = 0; F_Sem.Count - 1 >= i; i++)
//                {
//                    F_Sem[i].LeveldBmkVm = (float)(77.2 + 20 * Math.Log10(F_Sem[i].Freq) + F_Sem[i].LeveldBm - ANT_VAL);
//                }
//            }
//            catch (Exception ex)
//            { //GlobalInit.log.Trace("calc_FS" + ex.Message); 
//            }
//            return;
//        }
//        /// <summary>
//        /// Производит непосредственное сканирование уровня сигнала в устанавленном диапазоне
//        /// </summary>
//        private void GetLevel()
//        {
//            try
//            {
//                if (Type_of_m == Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.Sweep)
//                {
//                    Sweep(sw_time);
//                    if (status != bbStatus.bbNoError) { error = "Error of SW"; Close_dev(); return; }
//                }
//                else if (Type_of_m == Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.RealTime)
//                {
//                    Real_time(sw_time);
//                    if (status != bbStatus.bbNoError) { error = "Error of RT"; Close_dev(); return; }
//                }
//            }
//            catch (Exception ex)
//            { //GlobalInit.log.Trace("GetLevel" + ex.Message); 
//            }
//            return;
//        }
//        #endregion
//        #region MeasurementsProcessingSO
//        /// <summary>
//        /// Функция измерения. Изменяет Spectrum Ocupation c агрегированием результатов измерений. 
//        /// </summary>
//        /// <param name="taskSDR">MeasTask на измерение</param>
//        /// <param name="sensor">Sensor</param>
//        /// <param name="LastSDRresult">Result last meas for agregation</param>
//        private void MeasSO(ref MeasSdrTask taskSDR, Atdi.AppServer.Contracts.Sdrns.Sensor sensor = null, MeasSdrResults LastSDRresult = null)
//        {
//            try
//            {
//                MEAS_SDR_RESULTS.Freqs = null;
//                MEAS_SDR_RESULTS.Level = null;
//                if ((taskSDR.MeasSDRSOParam.TypeSO == Atdi.AppServer.Contracts.Sdrns.SpectrumOccupationType.FreqBandwidthOccupation) || (taskSDR.MeasSDRSOParam.TypeSO == Atdi.AppServer.Contracts.Sdrns.SpectrumOccupationType.FreqChannelOccupation))
//                {

//                    // вот собственно само измерение
//                    List<FSemples> F_ch_res_ = new List<FSemples>();
//                    List<FSemples> F_ch_res = new List<FSemples>();

//                    // сохраняем предыдущий результат если это не первый замер
//                    if (LastSDRresult != null) { if (LastSDRresult.FSemples != null) { F_ch_res = LastSDRresult.FSemples.ToList(); count = LastSDRresult.NN; } } else { count = 0; }

//                    // замер 
//                    if (Type_of_m == Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.Sweep) { Sweep(sw_time); if (status != bbStatus.bbNoError) { error = "Error of SW"; return; } }
//                    else if (Type_of_m == Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.RealTime) { Real_time(sw_time); if (status != bbStatus.bbNoError) { error = "Error of RT"; return; } }
//                    // замер выполнен он находится в F_semple
//                    // дополняем замер значениями SO и прочим теперь значения красивые по микроканальчикам
//                    for (int i = 0; i < F_semples.Count - 1; i++)
//                    {
//                        F_semples[i].LevelMaxdBm = F_semples[i].LeveldBm;
//                        F_semples[i].LevelMindBm = F_semples[i].LeveldBm;
//                        if (F_semples[i].LeveldBm > Level_min_occup)
//                        { F_semples[i].OcupationPt = 100; }
//                        else { F_semples[i].OcupationPt = 0; }
//                    }
//                    // Вот и дополнили значениями SO и прочим теперь значения красивые по микроканальчикам
//                    // Вычисляем занятость для данного замера по каналам 
//                    List<FSemples> F_ch_res_temp = new List<FSemples>(); // здест будут храниться замеры приведенные к каналу
//                    int start = 0;
//                    for (int i = 0; i <= List_freq_CH.Count - 1; i++) // Цикл по каналам
//                    {
//                        FSemples F_SO = new FSemples(); // здесь будет храниться один замер приведенный к каналу
//                        int sempl_in_freq = 0; //количество замеров идущие в один канал 
//                        for (int j = start; j <= F_semples.Count - 1; j++) // цикл по замерам по канальчикам
//                        {
//                            if (List_freq_CH[i] + BW_CH / 2000 < F_semples[j].Freq) { start = j; break; }
//                            if ((List_freq_CH[i] - BW_CH / 2000 <= F_semples[j].Freq) && (List_freq_CH[i] + BW_CH / 2000 > F_semples[j].Freq)) // проверка на попадание в диапазон частот
//                            {
//                                sempl_in_freq = sempl_in_freq + 1;
//                                if (sempl_in_freq == 1)// заполняем первое попадание как есть
//                                {
//                                    F_SO.Freq = (float)List_freq_CH[i];
//                                    F_SO.LeveldBm = F_semples[j].LeveldBm;
//                                    if (Type_of_SO == Atdi.AppServer.Contracts.Sdrns.SpectrumOccupationType.FreqBandwidthOccupation) // частотная занятость
//                                    {
//                                        if (F_semples[j].LeveldBm > Level_min_occup + 10 * Math.Log10(RBW / (BW_CH * 1000)))
//                                        { F_SO.OcupationPt = 100; }
//                                    }
//                                }
//                                else // накапливаем уровень синнала
//                                {
//                                    F_SO.LeveldBm = (float)(Math.Pow(10, F_SO.LeveldBm / 10) + Math.Pow(10, F_semples[j].LeveldBm / 10));
//                                    F_SO.LeveldBm = (float)(10 * Math.Log10(F_SO.LeveldBm));
//                                    if (Type_of_SO == Atdi.AppServer.Contracts.Sdrns.SpectrumOccupationType.FreqBandwidthOccupation) // частотная занятость //накапливаем
//                                    {
//                                        if (F_semples[j].LeveldBm > Level_min_occup + 10 * Math.Log10(RBW / (BW_CH * 1000)))
//                                        { F_SO.OcupationPt = F_SO.OcupationPt + 100; }
//                                    }
//                                }
//                            }
//                        }
//                        if (Type_of_SO == Atdi.AppServer.Contracts.Sdrns.SpectrumOccupationType.FreqBandwidthOccupation) { F_SO.OcupationPt = F_SO.OcupationPt / sempl_in_freq; }
//                        if (Type_of_SO == Atdi.AppServer.Contracts.Sdrns.SpectrumOccupationType.FreqChannelOccupation) { if (F_SO.LeveldBm > Level_min_occup) { F_SO.OcupationPt = 100; } }
//                        F_SO.LevelMaxdBm = F_SO.LeveldBm;
//                        F_SO.LevelMindBm = F_SO.LeveldBm;
//                        //F_SO на данный момент готов
//                        F_ch_res_temp.Add(F_SO); // добавляем во временный масив данные.
//                    }
//                    // данные единичного замера приведенного к каналам находятся здесь F_ch_res_temp    
//                    // Собираем статистику  в F_ch_res
//                    if (count == 0)
//                    {
//                        F_ch_res_ = F_ch_res_temp;
//                    }
//                    else
//                    {
//                        for (int i = 0; i < F_ch_res.Count; i++)
//                        {
//                            FSemples Semple = new FSemples();
//                            Semple.Freq = F_ch_res[i].Freq;
//                            Semple.LeveldBm = (float)(10 * Math.Log10((count * Math.Pow(10, F_ch_res[i].LeveldBm / 10) + Math.Pow(10, F_ch_res_temp[i].LeveldBm / 10)) / (count + 1))); // изменение 19.01.2018 Максим
//                            Semple.OcupationPt = (count * F_ch_res[i].OcupationPt + F_ch_res_temp[i].OcupationPt) / (count + 1);
//                            if (F_ch_res[i].LevelMaxdBm < F_ch_res_temp[i].LevelMaxdBm) { Semple.LevelMaxdBm = F_ch_res_temp[i].LevelMaxdBm; } else { Semple.LevelMaxdBm = F_ch_res[i].LevelMaxdBm; }
//                            if (F_ch_res[i].LevelMindBm > F_ch_res_temp[i].LevelMindBm) { Semple.LevelMindBm = F_ch_res_temp[i].LevelMindBm; } else { Semple.LevelMindBm = F_ch_res[i].LevelMindBm; }
//                            F_ch_res_.Add(Semple);
//                        }
//                    }

//                    // в данной точке результат находится в переменой F_ch_res и в count мы его должны показать/запомнить.  
//                    // кстати это происходит у нас циклически
//                    CalcFsFormLevel(ref F_ch_res_, sensor);
//                    MEAS_SDR_RESULTS.FSemples = (new List<FSemples>()).ToArray();
//                    MEAS_SDR_RESULTS.FSemples = F_ch_res_.ToArray();
//                    MEAS_SDR_RESULTS.NN = count + 1; // костыль пока признак 0 
//                }
//            }
//            catch (Exception ex)
//            { //GlobalInit.log.Trace("MeasSO" + ex.Message); 
//            }
//        }
//        #endregion
//        #region MeasurementsProcessingLevel
//        /// <summary>
//        /// Функция измерения изменяет уровень и только. 
//        /// </summary>
//        /// <param name="sensor"></param>
//        private void MeasScan(Atdi.AppServer.Contracts.Sdrns.Sensor sensor = null, MeasSdrResults LastSDRresult = null)
//        {
//            try
//            {
//                GetLevel();
//                if (status != bbStatus.bbNoError) { error = "Measurements Erorr"; return; }
//                CalcFsFormLevel(ref F_semples, sensor);
//                MEAS_SDR_RESULTS.FSemples = F_semples.ToArray();// запоминание результатов 
//                MEAS_SDR_RESULTS.Freqs = null;
//                MEAS_SDR_RESULTS.Level = null;
//                if (LastSDRresult == null) { MEAS_SDR_RESULTS.NN = 1; } else { MEAS_SDR_RESULTS.NN = LastSDRresult.NN + 1; }
//            }
//            catch (Exception ex)
//            { //GlobalInit.log.Trace("MeasScan" + ex.Message); 
//            }
//        }
//        /// <summary>
//        ///  Функция измерения изменяет уровень online. 
//        /// </summary>
//        /// <param name="sensor"></param>
//        private void MeasScanOnline(Atdi.AppServer.Contracts.Sdrns.Sensor sensor = null, MeasSdrResults LastSDRresult = null)
//        {
//            try
//            {
//                GetLevel();
//                if (status != bbStatus.bbNoError) { error = "Measurements Erorr"; return; }
//                CalcFsFormLevel(ref F_semples, sensor);
//                MEAS_SDR_RESULTS.FSemples = null;
//                List<float> Levels = new List<float>();
//                List<float> Freqs = new List<float>();
//                if (LastSDRresult == null)
//                {
//                    MEAS_SDR_RESULTS.NN = 1;
//                    for (int i = 0; i < F_semples.Count - 1; i++)
//                    {
//                        Levels.Add(F_semples[i].LeveldBmkVm);
//                        Freqs.Add(F_semples[i].Freq);
//                    }
//                    MEAS_SDR_RESULTS.Freqs = Freqs.ToArray(); // результат измерений частоты
//                    MEAS_SDR_RESULTS.Level = Levels.ToArray(); // результат измерений уровни 
//                }
//                else
//                {
//                    MEAS_SDR_RESULTS.NN = LastSDRresult.NN + 1;
//                    for (int i = 0; i < F_semples.Count - 1; i++)
//                    {
//                        Levels.Add(F_semples[i].LeveldBmkVm);
//                    }
//                    MEAS_SDR_RESULTS.Freqs = null; // результат измерений частоты
//                    MEAS_SDR_RESULTS.Level = Levels.ToArray(); // результат измерений уровни 
//                }
//            }
//            catch (Exception ex)
//            { //GlobalInit.log.Trace("MeasScanOnline" + ex.Message); 
//            }
//        }
//        #endregion
//        #region MeasurementsProcessingBW
//        /// <summary>
//        /// Функция измерения. Изменяет Bandwidth c улучшением результата. 
//        /// </summary>
//        /// <param name="taskSDR">MeasTask на измерение</param>
//        /// <param name="sensor">Sensor</param>
//        /// <param name="LastSDRresult">Result last meas for agregation</param>
//        private void MeasBW(ref MeasSdrTask taskSDR, Atdi.AppServer.Contracts.Sdrns.Sensor sensor = null, MeasSdrResults LastSDRresult = null)
//        {
//            try
//            {
//                // const 
//                BandwidthEstimation.BandwidthEstimationType bandwidthEstimationType = BandwidthEstimation.BandwidthEstimationType.xFromCentr;
//                double X_Beta = 25;
//                // end const

//                F_semples.Clear(); // очистка старых данных
//                // получение потока данных
//                if (Type_of_m == Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.Sweep) { Sweep_MAX_HOLD(sw_time); if (status != bbStatus.bbNoError) { error = "Error of SW"; return; } }
//                else if (Type_of_m == Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.RealTime) { Real_time_MAX_HOLD(sw_time); if (status != bbStatus.bbNoError) { error = "Error of RT"; return; } }
//                // измерение произведено и находится в F_Semple 
//                double[] SpecrtumArrdBm = new double[F_semples.Count];
//                for (int i = 0; i < F_semples.Count; i++)
//                {
//                    SpecrtumArrdBm[i] = F_semples[i].LeveldBm;
//                }
//                // Расчет BW
//                MeasSdrBandwidthResults measSdrBandwidthResults = BandwidthEstimation.GetBandwidthPoint(SpecrtumArrdBm, bandwidthEstimationType, X_Beta);
//                measSdrBandwidthResults.BandwidthkHz = bin_size * (measSdrBandwidthResults.T2 - measSdrBandwidthResults.T1) / 1000.0;
//                // заполнение результатов по сути
//                MEAS_SDR_RESULTS.ResultsBandwidth = measSdrBandwidthResults;
//                MEAS_SDR_RESULTS.FSemples = F_semples.ToArray();
//            }
//            catch (Exception ex)
//            { //GlobalInit.log.Trace("MeasBW" + ex.Message); 
//            }
//        }
//        #endregion








//    }
//}

