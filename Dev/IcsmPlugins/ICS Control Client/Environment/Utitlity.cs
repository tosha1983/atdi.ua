using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XICSM.ICSControlClient.Environment
{
    public class Utitlity
    {
        public struct FrequencyRange
        {
            public double MaxValue;
            public double MinValue;
            public double Step;
        }

        public struct LevelRange
        {
            public double MaxValue;
            public double MinValue;
        }

        static public LevelRange CalcLevelRange(double minValue, double maxValue)
        {
            var result = new LevelRange
            {
               MaxValue  = maxValue >= 0 ? Convert.ToDouble(Math.Truncate((decimal)maxValue / 10) * 10 + 10) : Convert.ToDouble(Math.Truncate((decimal)maxValue / 10) * 10),
               MinValue =  minValue >= 0 ? Convert.ToDouble(Math.Truncate((decimal)minValue / 10) * 10) : Convert.ToDouble(Math.Truncate((decimal)minValue / 10) * 10 - 10)
            };

            return result;
        }

        static public FrequencyRange CalcFrequencyRange(double minValue, double maxValue, int maxNumberLine)
        {
            List<decimal> Steps = new List<decimal>();
            long number = 0;
            int Razrad = -6;
            int LastPoint = 10;
            decimal step, fstart, fstop;
            do
            {
                if (LastPoint == 10) { LastPoint = 5; Razrad = Razrad + 1; } else { LastPoint = 10; }
                step = (decimal)(LastPoint * Math.Pow(10, Razrad));
                fstart = Math.Floor((decimal)minValue / step) * step;
                fstop = Math.Ceiling((decimal)maxValue / step) * step;
                number = (long)((fstop - fstart) / step);
            }
            while (number > maxNumberLine);

            Steps.Add(fstart);
            for (int i = 1; number >= i; i++)
            {
                decimal f = fstart + step * i;
                Steps.Add(f);
            }
            return new FrequencyRange
            {
                MinValue = Convert.ToDouble(Steps[0]),
                MaxValue = Convert.ToDouble(Steps[Steps.Count - 1]),
                Step = Convert.ToDouble(step)
            };
        }
    }
}
