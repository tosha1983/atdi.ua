using System;
namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class NH_SensorEquipSensitivity
    {

        private int m_ID { get; set; }
        private Double? m_Freq { get; set; }
        private Double? m_KTBF { get; set; }
        private Double? m_NoiseF { get; set; }
        private Double? m_FreqStability { get; set; }
        private Double? m_AddLoss { get; set; }
        private int? m_SensorEquip_ID { get; set; }



        public virtual int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public virtual Double? Freq
        {
            get { return m_Freq; }
            set { m_Freq = value; }
        }

        public virtual Double? KTBF
        {
            get { return m_KTBF; }
            set { m_KTBF = value; }
        }

        public virtual Double? NoiseF
        {
            get { return m_NoiseF; }
            set { m_NoiseF = value; }
        }

        public virtual Double? FreqStability
        {
            get { return m_FreqStability; }
            set { m_FreqStability = value; }
        }

        public virtual Double? AddLoss
        {
            get { return m_AddLoss; }
            set { m_AddLoss = value; }
        }

        public virtual int? SensorEquip_ID
        {
            get { return m_SensorEquip_ID; }
            set { m_SensorEquip_ID = value; }
        }

        public NH_SensorEquipSensitivity()
		{
           
		}

       
    }

}