using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Identity
{
	[DataContract(Namespace = CommonSpecification.Namespace)]
	public class ServiceCredential
	{
		/// <summary>
		/// Идентификатор внешнего сервиса
		/// </summary>
		[DataMember]
		public string ServiceId { get; set; }

		/// <summary>
		/// Секретный ключ
		/// </summary>
		[DataMember]
		public string SecretKey { get; set; }
	}
}
