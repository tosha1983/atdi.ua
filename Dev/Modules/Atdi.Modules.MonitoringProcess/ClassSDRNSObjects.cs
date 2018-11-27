using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB60C_
{
    // Meas resules
    public class MEAS_RESULTS
    {
        public int ID;
        public int ID_MEAS;
        public double FREQ;
        public double ANT_VAL;
        public DateTime TIME_MEAS;
        public int DATE_RANK;
        public int NN;
        public string STATUS;
        public List<MEAS_RESULTS_FO> meas_results_fo;
        public List<MEAS_RESULTS_ST> meas_results_st;
        public List<MEAS_RESULTS_AM> meas_results_am;
        public List<MEAS_RESULTS_FM> meas_results_fm;
        public List<MEAS_RESULTS_BW> meas_results_bw;
        public List<MEAS_RESULTS_P> meas_results_p;
        public List<MEAS_RESULTS_LOC> meas_results_loc;
        public List<MEAS_RESULTS_TEXTR> meas_results_textr;
        public List<MEAS_RESULTS_LOC_ST> meas_results_loc_st;
        public List<MEAS_RESULTS_IMA> meas_results_ima;
        public List<MEAS_RESULTS_SAT> meas_results_sat;
        public List<MEAS_RESULTS_LV> meas_results_lv;
        public List<MEAS_RESULTS_M_VINFO> meas_results_m_vinfo;

        public MEAS_RESULTS()
        {
            meas_results_fo = new List<MEAS_RESULTS_FO>();
            meas_results_st = new List<MEAS_RESULTS_ST>();
            meas_results_am = new List<MEAS_RESULTS_AM>();
            meas_results_fm = new List<MEAS_RESULTS_FM>();
            meas_results_bw = new List<MEAS_RESULTS_BW>();
            meas_results_p = new List<MEAS_RESULTS_P>();
            meas_results_loc = new List<MEAS_RESULTS_LOC>();
            meas_results_textr = new List<MEAS_RESULTS_TEXTR>();
            meas_results_loc_st = new List<MEAS_RESULTS_LOC_ST>();
            meas_results_ima = new List<MEAS_RESULTS_IMA>();
            meas_results_sat = new List<MEAS_RESULTS_SAT>();
            meas_results_lv = new List<MEAS_RESULTS_LV>();
            meas_results_m_vinfo = new List<MEAS_RESULTS_M_VINFO>();
        }
    }
    public class MEAS_RESULTS_FO
    {
        public int ID;
        public double VALUE;
        public double STD_DEV;
        public double V_MIN;
        public double V_MAX;
        public double LIMIT;
        public double OCCUPANCY;
    }
    public class MEAS_RESULTS_ST
    {
        public string IDENT;
    }
    public class MEAS_RESULTS_AM
    {
        public int ID;
        public double VALUE;
        public double STD_DEV;
        public double V_MIN;
        public double V_MAX;
        public double LIMIT;
        public double OCCUPANCY;
    }
    public class MEAS_RESULTS_FM
    {
        public int ID;
        public double VALUE;
        public double STD_DEV;
        public double V_MIN;
        public double V_MAX;
        public double LIMIT;
        public double OCCUPANCY;
    }
    public class MEAS_RESULTS_BW
    {
        public int ID;
        public double VALUE;
        public double STD_DEV;
        public double V_MIN;
        public double V_MAX;
        public double LIMIT;
        public double OCCUPANCY;
    }
    public class MEAS_RESULTS_P
    {
        public int ID;
        public double VALUE;
        public double STD_DEV;
        public double V_MIN;
        public double V_MAX;
        public double LIMIT;
        public double OCCUPANCY;
    }
    public class MEAS_RESULTS_LOC
    {
        public int ID;
        public double LONGITUDE;
        public double LATITUDE;
        public double RADIUS;
        public double P_LONGI;
        public double P_LATI;
        public double P_DIFF;
    }
    public class MEAS_RESULTS_TEXTR
    {
        public int ID;
        public string PICODE;
        public string PROGRAM;
        public string SOUNDID;
        public string TXT_RES;
        public string P_PICODE;
        public string P_PROGRAM;
    }
    public class MEAS_RESULTS_LOC_ST
    {
        public int ID;
        public double LON;
        public double LAT;
        public double ASL;

    }
    public class MEAS_RESULTS_IMA
    {
        public int ID;
        public double MD_FREQ_I;
        public double MD_FREQ_A;
        public double MD_FREQ_B;
        public double MD_FREQ_C;
        public string MD_FORM;
    }
    public class MEAS_RESULTS_SAT
    {
        public int ID;
        public double VALUE;
        public double STD_DEV;
        public double V_MIN;
        public double V_MAX;
        public double LIMIT;
        public double OCCUPANCY;
    }
    public class MEAS_RESULTS_LV
    {
        public int ID;
        public double VALUE;
        public double STD_DEV;
        public double V_MIN;
        public double V_MAX;
        public double LIMIT;
        public double OCCUPANCY;
        public double P_MIN;
        public double P_MAX;
        public double P_DIFF;
        public string TYPE_LV;
    }
    public class MEAS_RESULTS_M_VINFO
    {
        public int ID;
        public string MD_TX_NAME;
        public string MD_TX_SERVICE;
        public string MD_TX_SIGNATURE;
        public string MD_TX_CALL_SIGN;
        public string MD_TX_LICENSEE;
        public double BER;
        public double BER_LIM;
    }

}

   
