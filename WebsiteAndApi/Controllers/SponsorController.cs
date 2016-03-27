using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DevSpace.Common;
using Newtonsoft.Json;

namespace DevSpace.Api.Controllers {
	public class SponsorController : ApiController {
		private IDataStore<ISponsor> _DataStore;
		public SponsorController( IDataStore<ISponsor> DataStore ) {
			this._DataStore = DataStore;
		}

		[AllowAnonymous]
		public async Task<HttpResponseMessage> Get() {
			try {
				HttpResponseMessage response = new HttpResponseMessage( HttpStatusCode.OK );
				string val = JsonConvert.SerializeObject( await _DataStore.GetAll() );
				response.Content = new StringContent( val );
				return response;
			} catch( NotImplementedException ) {
				return new HttpResponseMessage( HttpStatusCode.NotImplemented );
			} catch {
				return new HttpResponseMessage( HttpStatusCode.InternalServerError );
			}
		}

		[AllowAnonymous]
		public async Task<HttpResponseMessage> Get( int id ) {
			try {
				ISponsor sponsor = await _DataStore.Get( id );
				if( null == sponsor ) return new HttpResponseMessage( HttpStatusCode.NotFound );

				HttpResponseMessage response = new HttpResponseMessage( HttpStatusCode.OK );
				response.Content = new StringContent( JsonConvert.SerializeObject( sponsor ) );
				return response;
			} catch( NotImplementedException ) {
				return new HttpResponseMessage( HttpStatusCode.NotImplemented );
			} catch {
				return new HttpResponseMessage( HttpStatusCode.InternalServerError );
			}
		}

		//public void Post( [FromBody]string value ) {
		//}

		//public void Put( int id, [FromBody]string value ) {
		//}

		//public void Delete( int id ) {
		//}
	}
}
