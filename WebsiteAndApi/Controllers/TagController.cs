using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using DevSpace.Common;
using Newtonsoft.Json;

namespace DevSpace.Api.Controllers {
	public class TagController : ApiController {
		private class JsonTagBinder : JsonBinder<ITag, Tag> { }

		private IDataStore<ITag> _DataStore;
		public TagController( IDataStore<ITag> DataStore ) {
			this._DataStore = DataStore;
		}

		[AllowAnonymous]
		public async Task<HttpResponseMessage> Get() {
			IList<ITag> Tags = await _DataStore.GetAll();

			HttpResponseMessage Response = new HttpResponseMessage( HttpStatusCode.OK );
			Response.Content = new StringContent( await Task.Factory.StartNew( () => JsonConvert.SerializeObject( Tags, Formatting.None ) ) );
			return Response;
		}

		[Authorize]
		public async Task<HttpResponseMessage> Post( [ModelBinder( typeof( JsonTagBinder ) )]ITag newTag ) {
			try {
				if( -1 == newTag.Id ) {
					HttpResponseMessage Response = new HttpResponseMessage( HttpStatusCode.Created );
					Response.Content = new StringContent( await Task.Factory.StartNew( () => JsonConvert.SerializeObject( _DataStore.Add( newTag ).Result, Formatting.None ) ) );
					return Response;
				} else {
					HttpResponseMessage Response = new HttpResponseMessage( HttpStatusCode.NoContent );
					Response.Content = new StringContent( await Task.Factory.StartNew( () => JsonConvert.SerializeObject( _DataStore.Update( newTag ).Result, Formatting.None ) ) );
					return Response;
				}
			} catch {
				return new HttpResponseMessage( HttpStatusCode.InternalServerError );
			}
		}
	}
}
