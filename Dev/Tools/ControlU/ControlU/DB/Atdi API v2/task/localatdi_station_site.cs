using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class localatdi_station_site : PropertyChangedBase
    {
        public string address
        {
            get { return _address; }
            set { _address = value; OnPropertyChanged("address"); }
        }
        private string _address = "";

        public string region
        {
            get { return _region; }
            set { _region = value; OnPropertyChanged("region"); }
        }
        private string _region = "";

        //public decimal latitude
        //{
        //    get { return _latitude; }
        //    set { _latitude = value; OnPropertyChanged("latitude"); }
        //}
        //private decimal _latitude = 0;

        //public decimal longitude
        //{
        //    get { return _longitude; }
        //    set { _longitude = value; OnPropertyChanged("longitude"); }
        //}
        //private decimal _longitude = 0;

        public localatdi_geo_location location
        {
            get { return _location; }
            set { _location = value; OnPropertyChanged("location"); }
        }
        private localatdi_geo_location _location = new localatdi_geo_location() { };
    }
}
