using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer
{
	public class CalcResultEvent
	{
		public CalcResultEventLevel Level;

		public string Context;

		public string Message { get; set; }
	}

	public class CalcResultEvent<TData> : CalcResultEvent
	{
		public TData Data;
	}

	public enum CalcResultEventLevel
	{
		Info = 0,
		Warning = 1,
		Error = 2
	}

	
}
