using Atdi.Common.Extensions;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.DataModels.Identity;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.Identity
{
	public sealed class ServiceTokenProvider : LoggedObject, IServiceTokenProvider
	{
		private const string SecretKey = "11FDED81-70AE-4F39-95F7-AB38EDB20B48";

		public ServiceTokenProvider(ILogger logger) : base(logger)
		{

		}

		public ServiceTokenData DecodeToken(ServiceToken token)
		{
			if (token == null)
			{
				throw new ArgumentNullException(nameof(token));
			}

			if (token.Data == null)
			{
				throw new ArgumentNullException(nameof(token.Data));
			}

			try
			{
				var plainTextData = Encryptor.DecryptStringAES(token.Data, SecretKey);
				var rawData = Convert.FromBase64String(plainTextData);
				return rawData.Deserialize<ServiceTokenData>();
			}
			catch (Exception e)
			{
				this.Logger.Exception(Contexts.IdentityCoreServices, Categories.Handling, e, this);
				throw new InvalidOperationException("Failed to decode the service token", e);
			}
		}

		public ServiceToken EncodeToken(ServiceTokenData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			try
			{
				var rawData = data.Serialize();
				var encryptedData = Encryptor.EncryptStringAES(Convert.ToBase64String(rawData), SecretKey);
				var token = new ServiceToken
				{
					Data = encryptedData
				};

				return token;
			}
			catch (Exception e)
			{
				this.Logger.Exception(Contexts.IdentityCoreServices, Categories.Handling, e, this);
				throw new InvalidOperationException("Failed to encode the service token", e);
			}
		}
	}
}
