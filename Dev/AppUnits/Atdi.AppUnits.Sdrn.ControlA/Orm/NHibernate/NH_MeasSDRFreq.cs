using System;
namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class NH_MeasSDRFreq
    {
     
        
        private int? m_id { get; set; }
        private double? m_Freq { get; set; }
        private int? m_ID_MeasSDRFreqParam { get; set; }



        public virtual int? ID
		{
			get { return m_id; }
			set { m_id = value; }
		}

        public virtual double? Freq
        {
            get { return m_Freq; }
            set { m_Freq = value; }
		}
     

        public virtual int? ID_MeasSDRFreqParam
        {
            get { return m_ID_MeasSDRFreqParam; }
            set { m_ID_MeasSDRFreqParam = value; }
		}

        public NH_MeasSDRFreq()
		{
           
		}

       
    }

}