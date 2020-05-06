using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Api.EntityOrm.WebClient
{
	[Serializable]
	public class EntityOrmWebApiException : ApplicationException
	{
		public readonly HttpStatusCode StatusCode;

		public readonly string RequestUri;

		public WebApiServerException ServerException;

		public EntityOrmWebApiException(HttpStatusCode statusCode, string requestUri, string message, WebApiServerException exception) : base(message)
		{
			this.StatusCode = statusCode;
			this.RequestUri = requestUri;
			this.ServerException = exception;
		}

		public override string ToString()
		{
			if (this.ServerException == null)
			{
				return $"{StatusCode}: {this.Message} RequestUri='{RequestUri}'";
			}
			
			return $"{StatusCode}: {this.Message} RequestUri='{RequestUri}'{Environment.NewLine} -> {this.ServerException}";
		}
	}

	[Serializable]
	public class WebApiServerException
	{
		public string Message;

		public string ExceptionMessage;

		public string ExceptionType;

		public string StackTrace;

		public WebApiServerException InnerException;

		public override string ToString()
		{
			if (string.IsNullOrEmpty(ExceptionMessage))
			{
				if (InnerException == null)
				{
					return $"{Message}";
				}
				return $"{Message}{Environment.NewLine} -> {this.InnerException}";
			}
			else
			{
				if (InnerException == null)
				{
					return $"{Message} {ExceptionMessage}";
				}
				return $"{Message} {ExceptionMessage}{Environment.NewLine} -> {this.InnerException}";
			}
			
		}
	}
}
