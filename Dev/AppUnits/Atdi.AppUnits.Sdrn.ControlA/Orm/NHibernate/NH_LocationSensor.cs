using System;
namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class NH_LocationSensor
    {
     
        
        private int? m_ID { get; set; }
        private double? m_Lon { get; set; }
        private double? m_Lat { get; set; }
        private double? m_ASL { get; set; }
        private int? m_ID_MeasSDRResults { get; set; }

        

        public virtual int? ID
        {
			get { return m_ID; }
			set { m_ID = value; }
		}

     
        public virtual double? Lon
        {
            get { return m_Lon; }
            set { m_Lon = value; }
		}

        public virtual double? Lat
        {
            get { return m_Lat; }
            set { m_Lat = value; }
        }

        public virtual double? ASL
        {
            get { return m_ASL; }
            set { m_ASL = value; }
        }

      
        public virtual int? ID_MeasSDRResults
        {
			get { return m_ID_MeasSDRResults; }
			set { m_ID_MeasSDRResults = value; }
		}

              

        public NH_LocationSensor()
		{
           
		}

       
    }

}