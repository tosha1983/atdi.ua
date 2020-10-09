using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
    [Serializable]
    public sealed class VoidResult
    {
		public static readonly VoidResult Instance = new VoidResult(); 
	}
}
