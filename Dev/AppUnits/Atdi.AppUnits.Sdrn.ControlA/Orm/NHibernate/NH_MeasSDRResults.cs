using System;
namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class NH_MeasSDRResults
    {

        private int? m_id { get; set; }
        private int? m_MeasSubTaskId { get; set; }
        private int? m_MeasSubTaskStationId { get; set; }
        private int? m_MeasTaskId { get; set; }
        private int? m_SensorId { get; set; }
        private DateTime? m_DataMeas { get; set; }
        private string m_status { get; set; }
        private int? m_SwNumber { get; set; }
        private int? m_NN { get; set; }
        private string m_MeasDataType { get; set; }
        private int? m_isSend { get; set; }


        public virtual int? ID
		{
			get { return m_id; }
			set { m_id = value; }
		}

        public virtual int? isSend
        {
            get { return m_isSend; }
            set { m_isSend = value; }
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

        public virtual int? MeasTaskId
        {
            get { return m_MeasTaskId; }
            set { m_MeasTaskId = value; }
        }

        public virtual int? SensorId
        {
            get { return m_SensorId; }
            set { m_SensorId = value; }
        }


        public virtual DateTime? DataMeas
        {
			get { return m_DataMeas; }
            set { m_DataMeas = value; }
		}

        public virtual string status
        {
			get { return m_status; }
            set { m_status = value; }
		}

        public virtual int? NN
        {
            get { return m_NN; }
            set { m_NN = value; }
        }

        public virtual int? SwNumber
        {
            get { return m_SwNumber; }
            set { m_SwNumber = value; }
        }

        public virtual string MeasDataType
        {
            get { return m_MeasDataType; }
            set { m_MeasDataType = value; }
        }

        public NH_MeasSDRResults()
		{
           
		}

       
    }

}