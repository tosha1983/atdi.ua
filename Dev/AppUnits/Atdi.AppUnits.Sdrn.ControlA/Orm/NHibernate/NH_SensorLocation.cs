using System;
namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class NH_SensorLocation
    {
        private int m_ID { get; set; }
        private DateTime? m_DataFrom { get; set; }
        private DateTime? m_DataTo { get; set; }
        private DateTime? m_DataCreated { get; set; }
        private string m_Status { get; set; }
        private Double? m_Lon { get; set; }
        private Double? m_Lat { get; set; }
        private Double? m_ASL { get; set; }
        private int? m_SensorID { get; set; }

        public virtual int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public virtual int? SensorID
        {
            get { return m_SensorID; }
            set { m_SensorID = value; }
        }

        public virtual DateTime? DataFrom
        {
			get { return m_DataFrom; }
			set { m_DataFrom = value; }
		}

        public virtual DateTime? DataTo
        {
			get { return m_DataTo; }
			set { m_DataTo = value; }
		}
     
        public virtual DateTime? DataCreated
        {
			get { return m_DataCreated; }
			set { m_DataCreated = value; }
		}

        public virtual string Status
        {
			get { return m_Status; }
			set { m_Status = value; }
		}

        public virtual Double? Lon
        {
            get { return m_Lon; }
            set { m_Lon = value; }
        }

        public virtual Double? Lat
        {
            get { return m_Lat; }
            set { m_Lat = value; }
        }

        public virtual Double? ASL
        {
            get { return m_ASL; }
            set { m_ASL = value; }
        }

        public NH_SensorLocation()
		{
           
		}

       
    }

}