using System;
namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class NH_MeasTaskSDR
    {

   
        private int? m_id { get; set; }
        private int? m_MeasTaskId { get; set; }
        private int? m_MeasSubTaskId { get; set; }
        private int? m_MeasSubTaskStationId { get; set; }
        private int? m_SensorId { get; set; }
        private string m_MeasDataType { get; set; }
        private string m_TypeM { get; set; }
        private int? m_SwNumber { get; set; }
        private DateTime? m_Time_start { get; set; }
        private DateTime? m_Time_stop { get; set; }
        private string m_prio { get; set; }
        private string m_status { get; set; }
        private int? m_PerInterval { get; set; }
        
        
        
        public virtual int? ID
		{
			get { return m_id; }
			set { m_id = value; }
		}

        public virtual int? MeasTaskId
        {
            get { return m_MeasTaskId; }
            set { m_MeasTaskId = value; }
        }

        public virtual int? MeasSubTaskId
        {
            get { return m_MeasSubTaskId; }
            set { m_MeasSubTaskId = value; }
        }

        public virtual int? MeasSubTaskStationId
        {
            get { return m_MeasSubTaskStationId; }
            set { m_MeasSubTaskStationId = value; }
        }

        public virtual int? SensorId
        {
            get { return m_SensorId; }
            set { m_SensorId = value; }
        }


        public virtual string MeasDataType
        {
            get { return m_MeasDataType; }
            set { m_MeasDataType = value; }
		}

    
        public virtual string TypeM
        {
            get { return m_TypeM; }
            set { m_TypeM = value; }
        }

        
        public virtual int? SwNumber
        {
            get { return m_SwNumber; }
            set { m_SwNumber = value; }
		}

        public virtual DateTime? Time_start
        {
			get { return m_Time_start; }
            set { m_Time_start = value; }
		}

        public virtual DateTime? Time_stop
        {
            get { return m_Time_stop; }
            set { m_Time_stop = value; }
		}

        public virtual string prio
        {
			get { return m_prio; }
            set { m_prio = value; }
		}

        public virtual string status
        {
            get { return m_status; }
            set { m_status = value; }
		}

        public virtual int? PerInterval
        {
            get { return m_PerInterval; }
            set { m_PerInterval = value; }
		}
        

        public NH_MeasTaskSDR()
		{
           
		}

       
    }

}