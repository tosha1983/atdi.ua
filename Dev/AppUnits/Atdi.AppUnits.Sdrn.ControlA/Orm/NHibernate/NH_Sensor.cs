using System;
namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class NH_Sensor
    {

        private int m_ID { get; set; }
        private int? m_SensorIdentifier_Id { get; set; }
        private string m_Status { get; set; }
        private string m_Name { get; set; }
        private string m_Administration { get; set; }
        private string m_NetworkId { get; set; }
        private string m_Remark { get; set; }
        private DateTime? m_BiuseDate { get; set; }
        private DateTime? m_EouseDate { get; set; }
        private Double? m_Azimuth { get; set; }
        private Double? m_Elevation { get; set; }
        private Double? m_AGL { get; set; }
        private string m_IdSysARGUS { get; set; }
        private string m_TypeSensor { get; set; }
        private Double? m_StepMeasTime { get; set; }
        private Double? m_RxLoss { get; set; }
        private Double? m_OpHHFr { get; set; }
        private Double? m_OpHHTo { get; set; }
        private string m_OpDays { get; set; }
        private string m_CustTxt1 { get; set; }
        private DateTime? m_CustData1 { get; set; }
        private Double? m_CustNbr1 { get; set; }
        private DateTime? m_DateCreated { get; set; }
        private string m_CreatedBy { get; set; }



        public virtual int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public virtual int? SensorIdentifier_Id
        {
            get { return m_SensorIdentifier_Id; }
            set { m_SensorIdentifier_Id = value; }
        }

        public virtual string Status
        {
            get { return m_Status; }
            set { m_Status = value; }
        }


        public virtual string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public virtual string Administration
        {
            get { return m_Administration; }
            set { m_Administration = value; }
        }


        public virtual string NetworkId
        {
            get { return m_NetworkId; }
            set { m_NetworkId = value; }
        }

        public virtual string Remark
        {
            get { return m_Remark; }
            set { m_Remark = value; }
        }

        public virtual DateTime? BiuseDate
        {
            get { return m_BiuseDate; }
            set { m_BiuseDate = value; }
        }

        public virtual DateTime? EouseDate
        {
            get { return m_EouseDate; }
            set { m_EouseDate = value; }
        }

        public virtual Double? Azimuth
        {
            get { return m_Azimuth; }
            set { m_Azimuth = value; }
        }

        public virtual Double? Elevation
        {
            get { return m_Elevation; }
            set { m_Elevation = value; }
        }

        public virtual Double? AGL
        {
            get { return m_AGL; }
            set { m_AGL = value; }
        }


        public virtual string IdSysARGUS
        {
            get { return m_IdSysARGUS; }
            set { m_IdSysARGUS = value; }
        }


        public virtual string TypeSensor
        {
            get { return m_TypeSensor; }
            set { m_TypeSensor = value; }
        }

        public virtual Double? StepMeasTime
        {
            get { return m_StepMeasTime; }
            set { m_StepMeasTime = value; }
        }

        public virtual Double? RxLoss
        {
            get { return m_RxLoss; }
            set { m_RxLoss = value; }
        }

        public virtual Double? OpHHFr
        {
            get { return m_OpHHFr; }
            set { m_OpHHFr = value; }
        }


        public virtual Double? OpHHTo
        {
            get { return m_OpHHTo; }
            set { m_OpHHTo = value; }
        }


        public virtual string OpDays
        {
            get { return m_OpDays; }
            set { m_OpDays = value; }
        }

        public virtual string CustTxt1
        {
            get { return m_CustTxt1; }
            set { m_CustTxt1 = value; }
        }

        public virtual DateTime? CustData1
        {
            get { return m_CustData1; }
            set { m_CustData1 = value; }
        }

        public virtual Double? CustNbr1
        {
            get { return m_CustNbr1; }
            set { m_CustNbr1 = value; }
        }

        public virtual DateTime? DateCreated
        {
            get { return m_DateCreated; }
            set { m_DateCreated = value; }
        }
        public virtual string CreatedBy
        {
            get { return m_CreatedBy; }
            set { m_CreatedBy = value; }
        }

        

        public NH_Sensor()
		{
           
		}

       
    }

}