using System;
namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class NH_AntennaPattern
    {

        private int m_ID { get; set; }
        private int? m_SensorAntenna_ID { get; set; }
        private Double m_Freq { get; set; }
        private Double m_Gain { get; set; }
        private string m_DiagA { get; set; }
        private string m_DiagH { get; set; }
        private string m_DiagV { get; set; }


        public virtual int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public virtual int? SensorAntenna_ID
        {
            get { return m_SensorAntenna_ID; }
            set { m_SensorAntenna_ID = value; }
        }
      

        public virtual Double Freq
        {
			get { return m_Freq; }
			set { m_Freq = value; }
		}
        public virtual Double Gain
        {
            get { return m_Gain; }
            set { m_Gain = value; }
        }

        public virtual string DiagA
        {
            get { return m_DiagA; }
            set { m_DiagA = value; }
        }

        public virtual string DiagH
        {
            get { return m_DiagH; }
            set { m_DiagH = value; }
        }

        public virtual string DiagV
        {
            get { return m_DiagV; }
            set { m_DiagV = value; }
        }
      
        public NH_AntennaPattern()
		{
           
		}

       
    }

}