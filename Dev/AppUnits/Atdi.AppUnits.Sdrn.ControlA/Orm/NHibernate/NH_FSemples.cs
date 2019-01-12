using System;
namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class NH_FSemples
    {
        
        private int? m_ID { get; set; }
        private double? m_Freq { get; set; }
        private double? m_LeveldBm { get; set; }
        private double? m_LeveldBmkVm { get; set; }
        private double? m_LevelMindBm { get; set; }
        private double? m_LevelMaxdBm { get; set; }
        private double? m_OcupationPt { get; set; }
        private int? m_ID_MeasSDRResults { get; set; }



        public virtual int? ID
        {
			get { return m_ID; }
			set { m_ID = value; }
		}

        public virtual double? Freq
        {
			get { return m_Freq; }
            set { m_Freq = value; }
		}

        public virtual double? LeveldBm
        {
			get { return m_LeveldBm; }
            set { m_LeveldBm = value; }
		}

        public virtual double? LeveldBmkVm
        {
			get { return m_LeveldBmkVm; }
            set { m_LeveldBmkVm = value; }
		}

        public virtual double? LevelMindBm
        {
            get { return m_LevelMindBm; }
            set { m_LevelMindBm = value; }
        }

        public virtual double? LevelMaxdBm
        {
            get { return m_LevelMaxdBm; }
            set { m_LevelMaxdBm = value; }
        }

        public virtual double? OcupationPt
        {
            get { return m_OcupationPt; }
            set { m_OcupationPt = value; }
        }

     
        public virtual int? ID_MeasSDRResults
        {
            get { return m_ID_MeasSDRResults; }
            set { m_ID_MeasSDRResults = value; }
        }

        public NH_FSemples()
		{
           
		}

       
    }

}