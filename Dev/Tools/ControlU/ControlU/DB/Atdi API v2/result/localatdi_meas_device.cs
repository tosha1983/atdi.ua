using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class localatdi_meas_device : PropertyChangedBase
    {
        public string manufacture
        {
            get { return _manufacture; }
            set { _manufacture = value; OnPropertyChanged("manufacture"); }
        }
        private string _manufacture = "";

        public string model
        {
            get { return _model; }
            set { _model = value; OnPropertyChanged("model"); }
        }
        private string _model = "";

        public string sn
        {
            get { return _sn; }
            set { _sn = value; OnPropertyChanged("sn"); }
        }
        private string _sn = "";

        public string[] antenna
        {
            get { return _antenna; }
            set { _antenna = value; OnPropertyChanged("antenna"); }
        }
        private string[] _antenna = new string[] { };
    }
}
