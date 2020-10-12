using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class localatdi_station_sector : PropertyChangedBase
    {
        public string sector_id
        {
            get { return _sector_id; }
            set { _sector_id = value; /*OnPropertyChanged("sector_id");*/ }
        }
        private string _sector_id = "";

        public decimal agl
        {
            get { return _agl; }
            set { _agl = value; OnPropertyChanged("agl"); }
        }
        private decimal _agl = 0;

        public decimal azimuth
        {
            get { return _azimuth; }
            set { _azimuth = value; OnPropertyChanged("azimuth"); }
        }
        private decimal _azimuth = 0;

        /// <summary>
        /// BW_kHz
        /// </summary>
        public decimal bw
        {
            get { return _bw; }
            set { _bw = value; OnPropertyChanged("bw"); }
        }
        private decimal _bw = 0;

        /// <summary>
        /// eirp_dbm
        /// </summary>
        public decimal eirp
        {
            get { return _eirp; }
            set { _eirp = value; OnPropertyChanged("eirp"); }
        }
        private decimal _eirp = 0;

        public string class_emission
        {
            get { return _class_emission; }
            set { _class_emission = value; OnPropertyChanged("class_emission"); }
        }
        private string _class_emission = "";

        public localatdi_sector_frequency[] frequencies
        {
            get { return _frequencies; }
            set { _frequencies = value; OnPropertyChanged("frequencies"); }
        }
        private localatdi_sector_frequency[] _frequencies = new localatdi_sector_frequency[] { };

        public localatdi_elements_mask[] bw_mask
        {
            get { return _bw_mask; }
            set { _bw_mask = value; OnPropertyChanged("frequencies"); }
        }
        private localatdi_elements_mask[] _bw_mask = new localatdi_elements_mask[] { };
    }
}
