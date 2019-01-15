using System;
namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class NH_MeasSDRFreqParam
    {


        private int? m_id { get; set; }
        private string m_Mode { get; set; }
        private double? m_RgL { get; set; }
        private double? m_RgU { get; set; }
        private double? m_Step { get; set; }
        private int? m_id_meas_task_sdr { get; set; }



        public virtual int? ID
		{
			get { return m_id; }
			set { m_id = value; }
		}

        public virtual string Mode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
		}

        public virtual double? RgL
        {
            get { return m_RgL; }
            set { m_RgL = value; }
		}

        public virtual double? RgU
        {
            get { return m_RgU; }
            set { m_RgU = value; }
        }

        public virtual double? Step
        {
            get { return m_Step; }
            set { m_Step = value; }
        }

        public virtual int? id_meas_task_sdr
        {
			get { return m_id_meas_task_sdr; }
			set { m_id_meas_task_sdr = value; }
		}

        public NH_MeasSDRFreqParam()
		{
           
		}

       
    }

}