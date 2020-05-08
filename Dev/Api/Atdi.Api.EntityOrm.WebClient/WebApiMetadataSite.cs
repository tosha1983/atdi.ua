using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EntityOrm.WebClient
{
	internal sealed class WebApiMetadataSite : IMetadataSite
	{
		private readonly WebApiDataLayer _dataLayer;
		private readonly WebApiHttpClient _httpClient;

		private const string MetadataEntityUri = "/api/orm/metadata/entity/";

		public WebApiMetadataSite(WebApiDataLayer dataLayer, WebApiEndpoint endpoint)
		{
			_dataLayer = dataLayer;
			_httpClient = new WebApiHttpClient(endpoint);
		}

		public EntityMetadata GetEntityMetadata(string qualifiedName)
		{
			var decodedName = WebApiUtility.DecodeEntityQualifiedName(qualifiedName);
			var uri = WebApiUtility.CombineUri(MetadataEntityUri, $"{decodedName.Namespace}/{decodedName.Name}");
			var response = _httpClient.Get(uri);
			var result = response.Decode<EntityMetadata>();
			return result;
		}

		public EntityMetadata GetEntityMetadata<TModel>()
		{
			var decodedName = WebApiUtility.DecodeEntityQualifiedName<TModel>();
			var uri = WebApiUtility.CombineUri(MetadataEntityUri, $"{decodedName.Namespace}/{decodedName.Name}");
			var response = _httpClient.Get(uri);
			var result = response.Decode<EntityMetadata>();
			return result;
		}
	}
}
