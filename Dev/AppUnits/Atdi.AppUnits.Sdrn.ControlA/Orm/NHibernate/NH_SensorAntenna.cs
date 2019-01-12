using System;
namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class NH_SensorAntenna
    {

        private int m_ID { get; set; }
        private string m_Code { get; set; }
        private Double? m_SlewAng { get; set; }
        private string m_Manufacturer { get; set; }
        private string m_Name { get; set; }
        private string m_TechId { get; set; }
        private string m_AntDir { get; set; }
        private Double? m_HBeamwidth { get; set; }
        private Double? m_VBeamwidth { get; set; }
        private string m_Polarization { get; set; }
        private string m_UseType { get; set; }
        private string m_Category { get; set; }
        private string m_GainType { get; set; }
        private Double? m_GainMax { get; set; }
        private Double? m_LowerFreq { get; set; }
        private Double? m_UpperFreq { get; set; }
        private Double? m_AddLoss { get; set; }
        private Double? m_XPD { get; set; }
        private string m_AntClass { get; set; }
        private string m_Remark { get; set; }
        private string m_CustTxt1 { get; set; }
        private string m_CustData1 { get; set; }
        private Double? m_CustNbr1 { get; set; }
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

        public virtual string Code
        {
			get { return m_Code; }
			set { m_Code = value; }
		}
        public virtual Double? SlewAng
        {
            get { return m_SlewAng; }
            set { m_SlewAng = value; }
        }

        public virtual string Manufacturer
        {
            get { return m_Manufacturer; }
            set { m_Manufacturer = value; }
        }
        public virtual string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public virtual string TechId
        {
            get { return m_TechId; }
            set { m_TechId = value; }
        }

        public virtual string AntDir
        {
            get { return m_AntDir; }
            set { m_AntDir = value; }
        }

        public virtual Double? HBeamwidth
        {
            get { return m_HBeamwidth; }
            set { m_HBeamwidth = value; }
        }


        public virtual Double? VBeamwidth
        {
            get { return m_VBeamwidth; }
            set { m_VBeamwidth = value; }
        }


        public virtual string Polarization
        {
            get { return m_Polarization; }
            set { m_Polarization = value; }
        }

        public virtual string UseType
        {
            get { return m_UseType; }
            set { m_UseType = value; }
        }

        public virtual string Category
        {
            get { return m_Category; }
            set { m_Category = value; }
        }


        public virtual string GainType
        {
            get { return m_GainType; }
            set { m_GainType = value; }
        }

        public virtual Double? GainMax
        {
            get { return m_GainMax; }
            set { m_GainMax = value; }
        }
        public virtual Double? LowerFreq
        {
            get { return m_LowerFreq; }
            set { m_LowerFreq = value; }
        }

        public virtual Double? UpperFreq
        {
            get { return m_UpperFreq; }
            set { m_UpperFreq = value; }
        }

        public virtual Double? AddLoss
        {
            get { return m_AddLoss; }
            set { m_AddLoss = value; }
        }

        public virtual Double? XPD
        {
            get { return m_XPD; }
            set { m_XPD = value; }
        }

        public virtual string AntClass
        {
            get { return m_AntClass; }
            set { m_AntClass = value; }
        }


        public virtual string Remark
        {
            get { return m_Remark; }
            set { m_Remark = value; }
        }

        public virtual string CustTxt1
        {
            get { return m_CustTxt1; }
            set { m_CustTxt1 = value; }
        }

        public virtual string CustData1
        {
            get { return m_CustData1; }
            set { m_CustData1 = value; }
        }

        public virtual Double? CustNbr1
        {
            get { return m_CustNbr1; }
            set { m_CustNbr1 = value; }
        }

        public NH_SensorAntenna()
		{
           
		}

       
    }

}