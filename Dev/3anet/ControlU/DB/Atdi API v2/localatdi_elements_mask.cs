using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class localatdi_elements_mask : PropertyChangedBase
    {
        public decimal bw
        {
            get { return _bw; }
            set { _bw = value; /*OnPropertyChanged("bw");*/ }
        }
        private decimal _bw = 0;

        public decimal level
        {
            get { return _level; }
            set { _level = value; /*OnPropertyChanged("level");*/ }
        }
        private decimal _level = 0;
    }
}
