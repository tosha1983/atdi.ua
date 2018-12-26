using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.MonitoringProcess.ProcessSignal
{
    [Serializable]
    public class TimeStampBlock
    {
        /// <summary>
        /// Indexes with some hit, depend from method find rotation.
        /// </summary>
        public List<int> RotationIndex;
        /// <summary>
        /// Amplitude of indexes with some hit, depend from method find rotation.
        /// </summary>
        public List<double> RotationAmpl;
        /// <summary>
        /// Phase of indexes with some hit, depend from method find rotation.
        /// </summary>
        public List<double> RotationPhase;
        /// <summary>
        /// Parameter of tone for Rotation index
        /// </summary>
        public List<TimeStampToneParameter> TimeStampToneParameters;
        /// <summary>
        /// Start Index of Block relatively start of measurements in IQ array
        /// </summary>
        public int StartIndexOfBlock;
        public double DurationBlockmks;
    }
    [Serializable]
    public class TimeStampToneParameter
    {
        /// <summary>
        /// Frequency of simbol in semples in IQ/2
        /// </summary>
        public double NumberSempleInSimbol;
        /// <summary>
        /// Semple of start new Simbol relatively start block in IQ/2
        /// </summary>
        public double SempleShiftOfSimbol;
        /// <summary>
        /// Penalty - Accuracy of the method in percent
        /// </summary>
        public double Penalty;
        /// <summary>
        /// Number Hit with this tone
        /// </summary>
        public int NumberHit;
    }
}
