using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class localatdi_license_info : PropertyChangedBase
    {
        public int icsm_id
        {
            get { return _icsm_id; }
            set { _icsm_id = value; /*OnPropertyChanged("icsm_id");*/ }
        }
        private int _icsm_id = 0;

        public string name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("name"); }
        }
        private string _name = "";

        public DateTime start_date
        {
            get { return _start_date; }
            set { _start_date = value; OnPropertyChanged("start_date"); }
        }
        private DateTime _start_date = DateTime.MinValue;

        public DateTime close_date
        {
            get { return _close_date; }
            set { _close_date = value; OnPropertyChanged("close_date"); }
        }
        private DateTime _close_date = DateTime.MinValue;

        public DateTime end_date
        {
            get { return _end_date; }
            set { _end_date = value; OnPropertyChanged("end_date"); }
        }
        private DateTime _end_date = DateTime.MinValue;
    }
}
