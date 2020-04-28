using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeepServices
{
    public interface IExampleDeepService : IDeepService
	{
	    float[] CalcSomething(float[] ar1, float[] ar2, int len);

	    double[] CalcSomething(double[] ar1, double[] ar2, int len);
	}
}
