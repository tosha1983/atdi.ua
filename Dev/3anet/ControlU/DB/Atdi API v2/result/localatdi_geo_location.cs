using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class localatdi_geo_location : PropertyChangedBase
    {
        public double agl
        {
            get { return _agl; }
            set { _agl = value; OnPropertyChanged("agl"); }
        }
        private double _agl = -100000;

        public double asl
        {
            get { return _asl; }
            set { _asl = value; OnPropertyChanged("asl"); }
        }
        private double _asl = -100000;

        public double latitude
        {
            get { return _latitude; }
            set { _latitude = value; OnPropertyChanged("latitude"); }
        }
        private double _latitude = 0;

        public double longitude
        {
            get { return _longitude; }
            set { _longitude = value; OnPropertyChanged("longitude"); }
        }
        private double _longitude = 0;
    }
}
