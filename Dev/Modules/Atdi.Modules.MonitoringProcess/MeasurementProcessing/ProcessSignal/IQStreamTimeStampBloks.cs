using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.SDR.Server.MeasurementProcessing.SingleHound.ProcessSignal
{
    /// <summary>
    /// Class represents parameters of timestamps for localization the source of emission.
    /// </summary>
    public class IQStreamTimeStampBloks
    {
        /// <summary>
        /// Guid
        /// </summary>
        public Guid guid;
        /// <summary>
        /// Block with TimeStamp
        /// </summary>
        public List<TimeStampBlock> TimeStampBlocks;
        /// <summary>
        /// 
        /// </summary>
        public List<int> IndexPPS;
        /// <summary>
        /// Date of Measurements 
        /// </summary>
        public DateTime TimeMeas;
        /// <summary>
        /// Speed of digitization of a signal
        /// </summary>
        public int SemplePerSecond;
        /// <summary>
        /// Total duration IQStream in mks
        /// </summary>
        public double TotalDurationIQStreammks;
        /// <summary>
        /// Index of IQ Stream with start signal flange 
        /// </summary>
        public int[] IndexStartFlange;
        /// <summary>
        /// Index of IQ Stream with start signal flange 
        /// </summary>
        public int[] IndexStopFlange;
        /// <summary>
        /// Name Measurements Station 
        /// </summary>
        public string MeasStationName;
        /// <summary>
        /// Method for time division
        /// </summary>
        public enum MethodForTimeDivision {ChangeOfFlanks, TriggerLevel, EqualTimeIntervals}
        /// <summary>
        /// Method for select critical point
        /// </summary>
        public enum MethodForSelectCriticalPoint { PhaseRotation, MaxLevel, SpeedChangeIQ, TransitionByZero }
        /// <summary>
        /// Method for calc freq from critical point
        /// </summary>
        public enum MethodForCalcFreqFromCriticalPoint {SingleToneByBlock, MultyToneByBlock}
        /// <summary>
        /// Method for time division
        /// </summary>
        public MethodForTimeDivision methodForTimeDivision;
        /// <summary>
        /// Method for select critical point
        /// </summary>
        public MethodForSelectCriticalPoint methodForSelectCriticalPoint;
        /// <summary>
        /// Method for calc freq from critical point
        /// </summary>
        public MethodForCalcFreqFromCriticalPoint methodForCalcFreqFromCriticalPoint;
        /// <summary>
        /// Longitude
        /// </summary>
        public double LonDEC;
        /// <summary>
        /// Latitude 
        /// </summary>
        public double LatDEC;
        /// <summary>
        /// Altitude 
        /// </summary>
        public double Altm;
    }
}
