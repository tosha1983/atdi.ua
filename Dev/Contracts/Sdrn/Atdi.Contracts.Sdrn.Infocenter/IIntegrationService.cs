using Atdi.DataModels.Sdrn.Infocenter.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.Infocenter
{
	public interface IIntegrationService
	{
		long Start(string source, string objectName);

		void Finish(long token, IntegrationStatusCode status, string statusNote, string total);

		void Finish<TSyncKey>(long token, IntegrationStatusCode status, string statusNote, string total, TSyncKey syncKey);
	}
}
