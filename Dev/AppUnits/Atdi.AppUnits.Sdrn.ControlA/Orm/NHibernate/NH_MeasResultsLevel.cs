using System;
namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class NH_MeasResultsLevel
    {
     
        
        private int? m_id { get; set; }
        private double? m_Level { get; set; }
        private int? m_ID_NH_MeasSDRResults { get; set; }



        public virtual int? ID
		{
			get { return m_id; }
			set { m_id = value; }
		}

        public virtual double? Level
        {
            get { return m_Level; }
            set { m_Level = value; }
		}
     

        public virtual int? ID_NH_MeasSDRResults
        {
            get { return m_ID_NH_MeasSDRResults; }
            set { m_ID_NH_MeasSDRResults = value; }
		}

        public NH_MeasResultsLevel()
		{
           
		}

       
    }

}