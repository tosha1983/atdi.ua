using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class localatdi_bandwidth_meas_result : PropertyChangedBase
    {
        /// <summary>
        /// Bandwidth_kHz
        /// </summary>
        public decimal bandwidth
        {
            get { return _bandwidth; }
            set { _bandwidth = value; /*OnPropertyChanged("bandwidth");*/ }
        }
        private decimal _bandwidth = 0;

        /// <summary>
        /// MarkerIndex
        /// </summary>
        public int m_index
        {
            get { return _m_index; }
            set { _m_index = value; /*OnPropertyChanged("m_index");*/ }
        }
        private int _m_index = 0;

        public int t1_index
        {
            get { return _t1_index; }
            set { _t1_index = value; /*OnPropertyChanged("t1_index");*/ }
        }
        private int _t1_index = 0;

        public int t2_index
        {
            get { return _t2_index; }
            set { _t2_index = value; /*OnPropertyChanged("t2_index");*/ }
        }
        private int _t2_index = 0;

        public int trace_count
        {
            get { return _trace_count; }
            set { _trace_count = value; /*OnPropertyChanged("trace_count");*/ }
        }
        private int _trace_count = 0;

        /// <summary>
        /// СorrectnessEstimations 0 = false, 1 = true дальше поглядим
        /// </summary>
        public int correctness_estimations
        {
            get { return _correctness_estimations; }
            set { _correctness_estimations = value; /*OnPropertyChanged("correctness_estimations");*/ }
        }
        private int _correctness_estimations = 0;
    }
}
