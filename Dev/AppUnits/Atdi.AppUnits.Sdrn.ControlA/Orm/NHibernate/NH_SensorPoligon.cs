using System;
namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class NH_SensorPoligon
    {
     
        
        private int? m_ID { get; set; }
        private double? m_Lon { get; set; }
        private double? m_Lat { get; set; }
        private int? m_SensorID { get; set; }



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
   
      
        public virtual int? SensorID
        {
			get { return m_SensorID; }
			set { m_SensorID = value; }
		}


        public NH_SensorPoligon()
		{
           
		}

       
    }

}