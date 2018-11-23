using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.SDR.Server.MeasurementProcessing.SingleHound;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Atdi.SDR.Server.MeasurementProcessing.SingleHound.ProcessSignal
{
    class MainProcessReceveIQStreamAndGetTimeStamp
    {
        #region parameters 
        SDR_BB60C SDR;
        public Double TimeReceivingSec;
        public Double MinDurationSignalForAnalizemks;
        public IQStreamTimeStampBloks IQStreamTimeStampBloks;
        public enum TypeTechnology { GSM, UMTS, CDMA, LTE, PMR, Ununknown}
        public TypeTechnology SignalTechnology;
        #endregion
        #region SaveLoadRaw
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="File"></param>
        /// <param name="obj"></param>
        public void SerializeObject(string File, object obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(File, FileMode.OpenOrCreate);
            formatter.Serialize(fs, obj);
            fs.Close();
            fs.Dispose();
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        public object DeserializeObject(string File)
        {
            object obj = null;
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(File, FileMode.OpenOrCreate);
            obj = formatter.Deserialize(fs);
            fs.Close();
            fs.Dispose();
            return obj;
        }
        private void SaveFileForAnalize(ReceiveIQStream IQStream, int PointStart, int PointStop)
        {
            int steps = 0;
            StreamWriter sw = new StreamWriter("C:\\TEMP\\points.csv");
            for (int i = 0; i < IQStream.Ampl.Count - 1; i++)
            {
                for (int j = 0; j < IQStream.Ampl[i].Length - 1; j++)
                {
                    if ((steps > PointStart) && (steps < PointStop))
                    {
                        string ss = steps.ToString() + ";" + IQStream.Ampl[i][j].ToString("N9", null);
                        sw.WriteLine(ss);
                    }
                    else if (steps > PointStop)
                    {
                        sw.Close();
                        return;
                    }
                    steps++;

                }
            }
        }
        #endregion

        public MainProcessReceveIQStreamAndGetTimeStamp(double _TimeReceivingSec, Double freqMHz, Double spankHz, TypeTechnology _SignalTechnology = TypeTechnology.Ununknown)
        {
            SignalTechnology =_SignalTechnology; 
            // Константы
            MinDurationSignalForAnalizemks = 100000.0/(spankHz);
            bool FilteringForFindSignalAndPause = true; 
            // конец констант
            TimeReceivingSec = _TimeReceivingSec; 
            SDR = new SDR_BB60C();
            SDR.initiation_SDR();
            SDR.calibration();
            SetConfigurationForReceivIQStream SDRConfig = new SetConfigurationForReceivIQStream(ref SDR, freqMHz, spankHz);
            // костыль 
            SDRConfig.samples_per_sec = 40000000;
            DateTime TimeMeas = DateTime.Now; // КОСТЫЛЬ
            //ReceiveIQStream IQStream = new ReceiveIQStream(SDR.id_dev, SDRConfig.return_len, SDRConfig.samples_per_sec, TimeReceivingSec);
            //запись объекта в файл
            //SerializeObject("C:\\Temp\\REsult4.bin", IQStream);
            //считывание объекта с файла
            ReceiveIQStream IQStream = (DeserializeObject("C:\\projects\\Monitoring projects\\SDR\\DataSignal\\GSM 900\\REsult2.bin") as ReceiveIQStream);
            IQStream.CalcAmpl();
            //SaveFileForAnalize(IQStream, 9600, 10600);

            // Константы и их установка
            IQStreamTimeStampBloks.MethodForTimeDivision methodForTimeDivision;
            IQStreamTimeStampBloks.MethodForSelectCriticalPoint methodForSelectCriticalPoint;
            IQStreamTimeStampBloks.MethodForCalcFreqFromCriticalPoint methodForCalcFreqFromCriticalPoint;
            bool CalcFreqTone;



            switch (SignalTechnology)
            {

                case TypeTechnology.GSM:
                case TypeTechnology.PMR:
                    methodForTimeDivision = IQStreamTimeStampBloks.MethodForTimeDivision.ChangeOfFlanks;
                    methodForSelectCriticalPoint = IQStreamTimeStampBloks.MethodForSelectCriticalPoint.PhaseRotation;
                    methodForCalcFreqFromCriticalPoint = IQStreamTimeStampBloks.MethodForCalcFreqFromCriticalPoint.SingleToneByBlock;
                    CalcFreqTone = true;
                    break;
                case TypeTechnology.CDMA:
                case TypeTechnology.UMTS:
                case TypeTechnology.LTE:
                case TypeTechnology.Ununknown:
                default:
                    methodForTimeDivision = IQStreamTimeStampBloks.MethodForTimeDivision.EqualTimeIntervals;
                    methodForSelectCriticalPoint = IQStreamTimeStampBloks.MethodForSelectCriticalPoint.SpeedChangeIQ;
                    methodForCalcFreqFromCriticalPoint = IQStreamTimeStampBloks.MethodForCalcFreqFromCriticalPoint.MultyToneByBlock;
                    CalcFreqTone = false;

                    MinDurationSignalForAnalizemks = Math.Max(TimeReceivingSec * 1000000 / 50, MinDurationSignalForAnalizemks);
                    break;
            }
            // конец констант

            IQStreamStartAndStopFlange IQstreamSignalFlange = new IQStreamStartAndStopFlange(SDRConfig);
            IQstreamSignalFlange.IQStreamStartAndStopFlangeCalc(methodForTimeDivision, IQStream, MinDurationSignalForAnalizemks, FilteringForFindSignalAndPause, spankHz); 
            IQstreamSignalFlange.CalcDurationSignalPause();
            IQstreamSignalFlange.CreateBlockSignal(IQStream, MinDurationSignalForAnalizemks);
            // Присвоение данных результату
            IQStreamTimeStampBloks = new IQStreamTimeStampBloks();
            IQStreamTimeStampBloks.guid = Guid.NewGuid();//КОСТЫЛЬ
            IQStreamTimeStampBloks.IndexPPS = new List<int>();
            // Заполнение PPS
            for (int i = 0; i < IQStream.triggers.Count; i++)
            {
                for (int j = 0; j < IQStream.triggers[i].Length; j++)
                {
                    if (IQStream.triggers[i][j] != 0)
                    { IQStreamTimeStampBloks.IndexPPS.Add(IQStream.triggers[i][j] + i * IQStream.iq_samples[0].Length);}else{break;}
                }
            }
            IQStreamTimeStampBloks.SemplePerSecond = SDRConfig.samples_per_sec;
            IQStreamTimeStampBloks.TimeMeas = TimeMeas;
            IQStreamTimeStampBloks.TotalDurationIQStreammks = TimeReceivingSec*1000000.0;
            IQStreamTimeStampBloks.IndexStartFlange = IQstreamSignalFlange.IndexStartFlange;
            IQStreamTimeStampBloks.IndexStopFlange = IQstreamSignalFlange.IndexStopFlange;
            IQStreamTimeStampBloks.methodForTimeDivision = methodForTimeDivision;
            IQStreamTimeStampBloks.methodForSelectCriticalPoint = methodForSelectCriticalPoint;
            IQStreamTimeStampBloks.methodForCalcFreqFromCriticalPoint = methodForCalcFreqFromCriticalPoint;
            IQStreamTimeStampBloks.TimeStampBlocks = new List<TimeStampBlock>();


            //Нужно только для тестов
            //RotationIndexTest = new List<int>(); RotationPhaseTest = new List<double>();
            //Test_block = IQstreamSignalFlange.BlockOfSignals[6];
            //PhaseProcess.RotationCalculation(methodForSelectCriticalPoint, IQstreamSignalFlange.BlockOfSignals[6], out RotationIndexTest, out RotationAmpl);
            //Penalty = 0;
            //Нужно только для тестов

            List<double> RotationAmpl = new List<double>();
            for (int i = 0; i < IQstreamSignalFlange.BlockOfSignals.Count; i++) 
            {
                if (IQstreamSignalFlange.BlockOfSignals[i].Durationmks > MinDurationSignalForAnalizemks)
                {
                    int p = IQstreamSignalFlange.BlockOfSignals[i].IQStream.Length;
                    List <int>RotationIndex = new List<int>();
                    List<Double> RotationPhase = new List<double>();
                    PhaseProcess.RotationCalculation(methodForSelectCriticalPoint, IQstreamSignalFlange.BlockOfSignals[i], out RotationIndex, out RotationAmpl);
                    TimeStampBlock timeStampBlock = new TimeStampBlock();
                    if (RotationIndex.Count > 10)
                    {
                        PhaseProcess.CalcTimestampBloks(ref RotationIndex, SDRConfig.samples_per_sec, spankHz, IQstreamSignalFlange.BlockOfSignals[i].Durationmks, IQstreamSignalFlange.BlockOfSignals[i].StartIndexIQ, methodForCalcFreqFromCriticalPoint, CalcFreqTone, out timeStampBlock);
                        timeStampBlock.RotationAmpl = RotationAmpl;
                        IQStreamTimeStampBloks.TimeStampBlocks.Add(timeStampBlock);
                    }
                }
             }
        }
    }
}
