using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class localatdi_station_owner : PropertyChangedBase
    {
        public int id
        {
            get { return _id; }
            set { _id = value; /*OnPropertyChanged("id");*/ }
        }
        private int _id = 0;

        public string address
        {
            get { return _address; }
            set { _address = value; OnPropertyChanged("address"); }
        }
        private string _address = "";

        public string code
        {
            get { return _code; }
            set { _code = value; OnPropertyChanged("code"); }
        }
        private string _code = "";

        public string okpo
        {
            get { return _okpo; }
            set { _okpo = value; OnPropertyChanged("okpo"); }
        }
        private string _okpo = "";

        public string name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("name"); }
        }
        private string _name = "";

        public string zip
        {
            get { return _zip; }
            set { _zip = value; OnPropertyChanged("zip"); }
        }
        private string _zip = "";
    }
}
