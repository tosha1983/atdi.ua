using System;
namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class NH_MeasSDRSOParam
    {

        
        private int? m_id { get; set; }
        private double? m_LevelMinOccup { get; set; }
        private string m_TypeSO { get; set; }
        private int? m_NChenal { get; set; }
        private int? m_ID_MeasTaskSDR { get; set; }


        public virtual int? ID
		{
			get { return m_id; }
			set { m_id = value; }
		}


        public virtual double? LevelMinOccup
        {
            get { return m_LevelMinOccup; }
            set { m_LevelMinOccup = value; }
		}

        public virtual string TypeSO
        {
            get { return m_TypeSO; }
            set { m_TypeSO = value; }
        }

        public virtual int? NChenal
        {
            get { return m_NChenal; }
            set { m_NChenal = value; }
        }

     
        public virtual int? ID_MeasTaskSDR
        {
			get { return m_ID_MeasTaskSDR; }
			set { m_ID_MeasTaskSDR = value; }
		}

        public NH_MeasSDRSOParam()
		{
           
		}

       
    }

}