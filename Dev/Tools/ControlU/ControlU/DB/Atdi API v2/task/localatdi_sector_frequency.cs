using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class localatdi_sector_frequency : PropertyChangedBase
    {
        public int id
        {
            get { return _id; }
            set { _id = value; /*OnPropertyChanged("id");*/ }
        }
        private int _id = 0;

        public int id_plan
        {
            get { return _id_plan; }
            set { _id_plan = value; /*OnPropertyChanged("id_plan");*/ }
        }
        private int _id_plan = 0;

        public int channel_number
        {
            get { return _channel_number; }
            set { _channel_number = value; /*OnPropertyChanged("channel_number");*/ }
        }
        private int _channel_number = 0;

        public decimal frequency
        {
            get { return _frequency; }
            set { _frequency = value; /*OnPropertyChanged("frequency");*/ }
        }
        private decimal _frequency = 0;

       

        
    }
}
