using System;
namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class NH_MeasSDRParam
    {


        private int? m_ID { get; set; }
        private double? m_RBW { get; set; }
        private double? m_VBW { get; set; }
        private double? m_MeasTime { get; set; }
        private double? m_ref_level_dbm { get; set; }
        private string m_DetectTypeSDR { get; set; }
        private int? m_PreamplificationSDR { get; set; }
        private int? m_RfAttenuationSDR { get; set; }
        private int? m_ID_MeasTaskSDR { get; set; }



        public virtual int? ID
		{
			get { return m_ID; }
			set { m_ID = value; }
		}


        public virtual double? RBW
        {
            get { return m_RBW; }
            set { m_RBW = value; }
		}

        public virtual double? VBW
        {
            get { return m_VBW; }
            set { m_VBW = value; }
        }

        public virtual double? MeasTime
        {
            get { return m_MeasTime; }
            set { m_MeasTime = value; }
        }

        public virtual double? ref_level_dbm
        {
            get { return m_ref_level_dbm; }
            set { m_ref_level_dbm = value; }
        }

        public virtual string DetectTypeSDR
        {
            get { return m_DetectTypeSDR; }
            set { m_DetectTypeSDR = value; }
        }

        public virtual int? PreamplificationSDR
        {
            get { return m_PreamplificationSDR; }
            set { m_PreamplificationSDR = value; }
        }

        public virtual int? RfAttenuationSDR
        {
            get { return m_RfAttenuationSDR; }
            set { m_RfAttenuationSDR = value; }
        }
        
        public virtual int? ID_MeasTaskSDR
        {
			get { return m_ID_MeasTaskSDR; }
			set { m_ID_MeasTaskSDR = value; }
		}

        public NH_MeasSDRParam()
		{
           
		}

       
    }

}