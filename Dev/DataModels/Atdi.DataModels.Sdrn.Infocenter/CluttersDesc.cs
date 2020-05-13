using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter
{
	public class CluttersDesc
	{
		public string Name;

		public string Note;

		public CluttersDescClutter[] Clutters;

		public CluttersDescFreq[] Frequencies;
	}
}
