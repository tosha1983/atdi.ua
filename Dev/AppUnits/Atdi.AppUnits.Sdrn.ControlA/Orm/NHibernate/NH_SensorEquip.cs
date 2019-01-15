using System;
namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class NH_SensorEquip
    {

        private int m_ID { get; set; }
        private string m_Code { get; set; }
        private string m_Manufacturer { get; set; }
        private string m_Name { get; set; }
        private string m_Family { get; set; }
        private string m_TechId { get; set; }
        private string m_Version { get; set; }
        private Double? m_LowerFreq { get; set; }
        private Double? m_UpperFreq { get; set; }
        private Double? m_RBWMin { get; set; }
        private Double? m_RBWMax { get; set; }
        private Double? m_VBWMin { get; set; }
        private Double? m_VBWMax { get; set; }
        private Boolean? m_Mobility { get; set; }
        private Double? m_FFTPointMax { get; set; }
        private Double? m_RefLeveldBm { get; set; }
        private string m_OperationMode { get; set; }
        private string m_Type { get; set; }
        private string m_EquipClass { get; set; }
        private Double? m_TuningStep { get; set; }
        private string m_UseType { get; set; }
        private string m_Category { get; set; }
        private string m_Remark { get; set; }
        private string m_CustTxt1 { get; set; }
        private DateTime? m_CustData1 { get; set; }
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


        public virtual string Family
        {
            get { return m_Family; }
            set { m_Family = value; }
        }

        public virtual string TechId
        {
            get { return m_TechId; }
            set { m_TechId = value; }
        }

        public virtual string Version
        {
            get { return m_Version; }
            set { m_Version = value; }
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

        public virtual Double? RBWMin
        {
            get { return m_RBWMin; }
            set { m_RBWMin = value; }
        }

        public virtual Double? RBWMax
        {
            get { return m_RBWMax; }
            set { m_RBWMax = value; }
        }

        public virtual Double? VBWMin
        {
            get { return m_VBWMin; }
            set { m_VBWMin = value; }
        }

        public virtual Double? VBWMax
        {
            get { return m_VBWMax; }
            set { m_VBWMax = value; }
        }

        public virtual Boolean? Mobility
        {
            get { return m_Mobility; }
            set { m_Mobility = value; }
        }

        public virtual Double? FFTPointMax
        {
            get { return m_FFTPointMax; }
            set { m_FFTPointMax = value; }
        }

        public virtual Double? RefLeveldBm
        {
            get { return m_RefLeveldBm; }
            set { m_RefLeveldBm = value; }
        }

        public virtual string OperationMode
        {
            get { return m_OperationMode; }
            set { m_OperationMode = value; }
        }


        public virtual string Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        public virtual string EquipClass
        {
            get { return m_EquipClass; }
            set { m_EquipClass = value; }
        }

        public virtual Double? TuningStep
        {
            get { return m_TuningStep; }
            set { m_TuningStep = value; }
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

        
        public NH_SensorEquip()
		{
           
		}

       
    }

}