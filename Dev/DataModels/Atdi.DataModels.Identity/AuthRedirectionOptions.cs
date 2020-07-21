﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Identity
{
	[DataContract(Namespace = CommonSpecification.Namespace)]
	public class AuthRedirectionOptions
	{
		[DataMember]
		public string AuthService;

		[DataMember]
		public string ReturnUrl;
	}
}