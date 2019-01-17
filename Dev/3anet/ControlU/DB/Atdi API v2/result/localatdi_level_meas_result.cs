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

        ///// <summary>
        ///// non
        ///// </summary>
        //public decimal latitude
        //{
        //    get { return _latitude; }
        //    set { _latitude = value; OnPropertyChanged("latitude"); }
        //}
        //private decimal _latitude = 0;

        ///// <summary>
        ///// non
        ///// </summary>
        //public decimal longitude
        //{
        //    get { return _longitude; }
        //    set { _longitude = value; OnPropertyChanged("longitude"); }
        //}
        //private decimal _longitude = 0;

        ///// <summary>
        ///// GeneralResult.StationSysInfo.Location.ASL
        ///// </summary>
        //public decimal sea_altitude
        //{
        //    get { return _sea_altitude; }
        //    set { _sea_altitude = value; OnPropertyChanged("meas_altitude"); }
        //}
        //private decimal _sea_altitude = -100000;

        ///// <summary>
        ///// GeneralResult.StationSysInfo.Location.AGL
        ///// </summary>
        //public decimal ground_altitude
        //{
        //    get { return _ground_altitude; }
        //    set { _ground_altitude = value; OnPropertyChanged("ground_altitude"); }
        //}
        //private decimal _ground_altitude = -100000;


        public localatdi_geo_location location
        {
            get { return _location; }
            set { _location = value; /*OnPropertyChanged("location");*/ }
        }
        private localatdi_geo_location _location = new localatdi_geo_location() { };
    }
}
