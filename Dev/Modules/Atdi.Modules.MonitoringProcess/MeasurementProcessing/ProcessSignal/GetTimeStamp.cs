﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Modules.MonitoringProcess.SingleHound;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Atdi.Modules.MonitoringProcess.ProcessSignal
{
    [Serializable]
    public class GetTimeStamp
    {
        #region parameters 
        public double TimeReceivingSec;
        public double MinDurationSignalForAnalizemks;
        public IQStreamTimeStampBloks IQStreamTimeStampBloks;
        public BlockOfSignal TestBlock;
        public List<int> RotationIndexTest;
        public enum TypeTechnology {GSM, UMTS, CDMA, LTE, PMR, Ununknown}
        public TypeTechnology SignalTechnology;
        #endregion
        #region LoadRaw
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
        private void SaveFileForAnalize(ReceivedIQStream IQStream, int PointStart, int PointStop)
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

        public GetTimeStamp(ReceivedIQStream IQStream, int semple_per_second, double spankHz, TypeTechnology _SignalTechnology = TypeTechnology.Ununknown)
        {
            SignalTechnology =_SignalTechnology;
            // Константы
            MinDurationSignalForAnalizemks = 50000.0/(spankHz);
            bool FilteringForFindSignalAndPause = true; 
            // конец констант
            DateTime TimeMeas = DateTime.Now; // КОСТЫЛЬ
            //считывание объекта с файла используется для тестов
            //if (IQStream is null)
            //{
            //    IQStream = (DeserializeObject("C:\\projects\\Monitoring projects\\SDR\\DataSignal\\GSM_900_2.bin") as ReceivedIQStream);
            //}
            TimeReceivingSec = IQStream.durationReceiving_sec;
            IQStream.CalcAmpl(true);

            //Установка констант
            IQStreamTimeStampBloks.MethodForTimeDivision methodForTimeDivision;
            IQStreamTimeStampBloks.MethodForSelectCriticalPoint methodForSelectCriticalPoint;
            IQStreamTimeStampBloks.MethodForCalcFreqFromCriticalPoint methodForCalcFreqFromCriticalPoint;
            bool CalcFreqTone;
            switch (SignalTechnology)
            {
                case TypeTechnology.GSM:
                    methodForTimeDivision = IQStreamTimeStampBloks.MethodForTimeDivision.ChangeOfFlanks;
                    methodForSelectCriticalPoint = IQStreamTimeStampBloks.MethodForSelectCriticalPoint.PhaseRotation;
                    methodForCalcFreqFromCriticalPoint = IQStreamTimeStampBloks.MethodForCalcFreqFromCriticalPoint.SingleToneByBlock;
                    CalcFreqTone = true;
                    break;
                case TypeTechnology.PMR:
                    methodForTimeDivision = IQStreamTimeStampBloks.MethodForTimeDivision.TriggerLevel;
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

            IQStreamStartAndStopFlange IQstreamSignalFlange = new IQStreamStartAndStopFlange(semple_per_second);
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
                    { IQStreamTimeStampBloks.IndexPPS.Add((IQStream.triggers[i][j] + i * IQStream.iq_samples[0].Length)/2);}else{break;}
                }
            }
            IQStreamTimeStampBloks.SemplePerSecond = semple_per_second;
            IQStreamTimeStampBloks.TimeMeas = TimeMeas;
            IQStreamTimeStampBloks.TotalDurationIQStreammks = TimeReceivingSec*1000000.0;
            IQStreamTimeStampBloks.IndexStartFlange = IQstreamSignalFlange.IndexStartFlange;
            IQStreamTimeStampBloks.IndexStopFlange = IQstreamSignalFlange.IndexStopFlange;
            IQStreamTimeStampBloks.methodForTimeDivision = methodForTimeDivision;
            IQStreamTimeStampBloks.methodForSelectCriticalPoint = methodForSelectCriticalPoint;
            IQStreamTimeStampBloks.methodForCalcFreqFromCriticalPoint = methodForCalcFreqFromCriticalPoint;
            IQStreamTimeStampBloks.TimeStampBlocks = new List<TimeStampBlock>();


            //Нужно только для тестов
            RotationIndexTest = new List<int>();
            List<double> RotationPhaseTest = new List<double>();
            List<double> RotationAmplTest = new List<double>();
            TestBlock = IQstreamSignalFlange.BlockOfSignals[0];
            PhaseProcess.RotationCalculation(methodForSelectCriticalPoint, IQstreamSignalFlange.BlockOfSignals[0], out RotationIndexTest, out RotationAmplTest, out RotationPhaseTest);
            //Penalty = 0;
            //Нужно только для тестов




        List<double> RotationAmpl = new List<double>();
            List<double> RotationPhase = new List<double>();
            for (int i = 0; i < IQstreamSignalFlange.BlockOfSignals.Count; i++) 
            {
                if (IQstreamSignalFlange.BlockOfSignals[i].Durationmks > MinDurationSignalForAnalizemks)
                {
                    int p = IQstreamSignalFlange.BlockOfSignals[i].IQStream.Length;
                    List <int>RotationIndex = new List<int>();
                    PhaseProcess.RotationCalculation(methodForSelectCriticalPoint, IQstreamSignalFlange.BlockOfSignals[i], out RotationIndex, out RotationAmpl, out RotationPhase);
                    TimeStampBlock timeStampBlock = new TimeStampBlock();
                    if (RotationIndex.Count > 10)
                    {
                        PhaseProcess.CalcTimestampBloks(ref RotationIndex, semple_per_second, spankHz, IQstreamSignalFlange.BlockOfSignals[i].Durationmks, IQstreamSignalFlange.BlockOfSignals[i].StartIndexIQ, methodForCalcFreqFromCriticalPoint, CalcFreqTone, out timeStampBlock);
                        timeStampBlock.RotationAmpl = RotationAmpl;
                        timeStampBlock.RotationPhase = RotationPhase;
                        IQStreamTimeStampBloks.TimeStampBlocks.Add(timeStampBlock);
                    }
                }
            }
        }
    }
}
