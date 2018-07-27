using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrmCs;


namespace Atdi.SDNRS.AppServer.ManageDB.Adapters
{
    public class CLASS_TASKS : IDisposable
    {
        public List<KeyValuePair<YXbsMeassubtask,List<YXbsMeassubtasksta>>> meas_st { get; set; }
        public YXbsMeastask meas_task { get; set; }
        public List<YXbsMeaslocparam> MeasLocParam { get; set; }
        public List<YXbsMeasstation> Stations { get; set; }
        public List<YXbsMeasdtparam> MeasDtParam { get; set; }
        public List<KeyValuePair<YXbsMeasfreqparam, List<YXbsMeasfreq>>> MeasFreqLst_param { get; set; }
        public YXbsMeasother MeasOther { get; set; }
        public YXbsMeastimeparaml MeasTimeParamList { get; set; }
        public List<YXbsStationdatform> XbsStationdatform { get; set; }

        /// <summary>
        /// Деструктор.
        /// </summary>
        ~CLASS_TASKS()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public CLASS_TASKS()
        {
            meas_st = new List<KeyValuePair<YXbsMeassubtask, List<YXbsMeassubtasksta>>>();
            meas_task = new YXbsMeastask();
            MeasOther = new YXbsMeasother();
            MeasLocParam = new List<YXbsMeaslocparam>();
            MeasDtParam = new List<YXbsMeasdtparam>();
            Stations = new List<YXbsMeasstation>();
            MeasFreqLst_param = new List<KeyValuePair<YXbsMeasfreqparam, List<YXbsMeasfreq>>>();
            MeasTimeParamList = new YXbsMeastimeparaml();
            XbsStationdatform = new List<YXbsStationdatform>();
        }
    }

  
}


