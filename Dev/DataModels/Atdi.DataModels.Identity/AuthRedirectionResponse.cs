using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Identity
{
	[DataContract(Namespace = CommonSpecification.Namespace)]
	public class AuthRedirectionResponse
	{
		[DataMember]
		public string Url { get; set; }
	}
}
