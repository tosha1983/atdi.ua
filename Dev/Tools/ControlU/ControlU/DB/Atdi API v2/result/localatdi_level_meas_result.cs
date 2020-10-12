using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class localatdi_level_meas_result : PropertyChangedBase
    {
        /// <summary>
        /// добавлена ли эта строка в результаты
        /// </summary>        
        public bool saved_in_result
        {
            get { return _saved_in_result; }
            set { _saved_in_result = value;/* OnPropertyChanged("saved_in_measurement"); */}
        }
        private bool _saved_in_result = false;

        [PgName("saved_in_db")]
        public bool saved_in_db
        {
            get { return _saved_in_db; }
            set { _saved_in_db = value; /*OnPropertyChanged("saved_in_db"); */}
        }
        private bool _saved_in_db = false;

        public decimal difference_time_stamp_ns
        {
            get { return _difference_time_stamp_ns; }
            set { _difference_time_stamp_ns = value; /*OnPropertyChanged("difference_time_stamp_ns");*/ }
        }
        private decimal _difference_time_stamp_ns = -1;

        public double level_dbm
        {
            get { return _level_dbm; }
            set { _level_dbm = value; /*OnPropertyChanged("level_dbm");*/ }
        }
        private double _level_dbm = -1000;

        public double level_dbmkvm
        {
            get { return _level_dbmkvm; }
            set { _level_dbmkvm = value; /*OnPropertyChanged("level_dbmkvm");*/ }
        }
        private double _level_dbmkvm = -1000;

        public DateTime measurement_time
        {
            get { return _measurement_time; }
            set { _measurement_time = value; /*OnPropertyChanged("measurement_time");*/ }
        }
        private DateTime _measurement_time = DateTime.MinValue;

        public localatdi_geo_location location
        {
            get { return _location; }
            set { _location = value; /*OnPropertyChanged("location");*/ }
        }
        private localatdi_geo_location _location = new localatdi_geo_location() { };
    }
}
