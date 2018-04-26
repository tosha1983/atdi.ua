using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XICSM.ICSControlClient.Metadata
{
    public class FreqPlanChan
    {
        public static readonly string TableName = "FREQ_PLAN_CHAN";

        public static class Fields
        {
            public static readonly string Freq = "FREQ";
            public static readonly string PlanId = "PLAN_ID";
        }
    }
}
