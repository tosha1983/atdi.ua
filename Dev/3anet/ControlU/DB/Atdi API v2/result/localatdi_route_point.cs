using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class localatdi_route_point : PropertyChangedBase
    {
        /// <summary>
        /// начинаем с 0
        /// </summary>
        public int route_id
        {
            get { return _route_id; }
            set { _route_id = value; /*OnPropertyChanged("route_id");*/ }
        }
        private int _route_id = 0;

        public bool saved_in_db
        {
            get { return _saved_in_db; }
            set { _saved_in_db = value; /*OnPropertyChanged("saved_in_db");*/ }
        }
        private bool _saved_in_db = false;

        //public decimal agl
        //{
        //    get { return _agl; }
        //    set { _agl = value; /*OnPropertyChanged("agl");*/ }
        //}
        //private decimal _agl = -100000;

        //public decimal asl
        //{
        //    get { return _asl; }
        //    set { _asl = value; /*OnPropertyChanged("asl");*/ }
        //}
        //private decimal _asl = -100000;

        //public decimal latitude
        //{
        //    get { return _latitude; }
        //    set { _latitude = value; /*OnPropertyChanged("latitude");*/ }
        //}
        //private decimal _latitude = 0;

        //public decimal longitude
        //{
        //    get { return _longitude; }
        //    set { _longitude = value; /*OnPropertyChanged("longitude");*/ }
        //}
        //private decimal _longitude = 0;

        /// <summary>
        /// enum Atdi.DataModels.Sdrns.PointStayType обычно InMove
        /// </summary>
        public int point_stay_type
        {
            get { return _point_stay_type; }
            set { _point_stay_type = value; /*OnPropertyChanged("point_stay_type");*/ }
        }
        private int _point_stay_type = 0;

        public DateTime start_time
        {
            get { return _start_time; }
            set { _start_time = value; /*OnPropertyChanged("start_time");*/ }
        }
        private DateTime _start_time = DateTime.MinValue;

        public DateTime finish_time
        {
            get { return _finish_time; }
            set { _finish_time = value; /*OnPropertyChanged("finish_time");*/ }
        }
        private DateTime _finish_time = DateTime.MinValue;

        public localatdi_geo_location location
        {
            get { return _location; }
            set { _location = value; /*OnPropertyChanged("location");*/ }
        }
        private localatdi_geo_location _location = new localatdi_geo_location() { };
    }
}
