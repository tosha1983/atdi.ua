using System.Collections.Generic;


namespace Atdi.AppServer.AppService.SdrnsControllerv2_0
{
    public class ClassTasks
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

     
        public ClassTasks()
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


