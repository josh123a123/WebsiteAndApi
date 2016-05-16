using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using DevSpace.Common;
using Newtonsoft.Json;

namespace DevSpace.Api.Controllers {
	public class SessionController : ApiController {
		private class JsonSessionBinder : JsonBinder<ISession, Session> { }

		private IDataStore<ISession> _DataStore;
		public SessionController( IDataStore<ISession> DataStore ) {
			this._DataStore = DataStore;
		}

		[Authorize]
		[Route( "api/v1/session/user/{Id}" )]
		public async Task<HttpResponseMessage> GetSessionFromUser( int Id ) {
			try {
				HttpResponseMessage Response = new HttpResponseMessage( HttpStatusCode.OK );
				Response.Content = new StringContent( await Task.Factory.StartNew( () => JsonConvert.SerializeObject( _DataStore.Get( "UserId", Id ).Result, Formatting.None ) ) );
				return Response;
			} catch {
				return new HttpResponseMessage( HttpStatusCode.InternalServerError );
			}
		}

		[Authorize]
		public async Task<HttpResponseMessage> Post( [ModelBinder( typeof( JsonSessionBinder ) )]ISession postedSession ) {
			try {
				if( -1 == postedSession.Id ) {
					HttpResponseMessage Response = new HttpResponseMessage( HttpStatusCode.Created );
					Response.Content = new StringContent( await Task.Factory.StartNew( () => JsonConvert.SerializeObject( _DataStore.Add( postedSession ).Result, Formatting.None ) ) );
					return Response;
				} else {
					HttpResponseMessage Response = new HttpResponseMessage( HttpStatusCode.NoContent );
					Response.Content = new StringContent( await Task.Factory.StartNew( () => JsonConvert.SerializeObject( _DataStore.Update( postedSession ).Result, Formatting.None ) ) );
					return Response;
				}
			} catch {
				return new HttpResponseMessage( HttpStatusCode.InternalServerError );
			}
		}

		[Authorize]
		public async Task<HttpResponseMessage> Delete( int Id ) {
			ISession sessionToDelete = await _DataStore.Get( Id );

			if( null == sessionToDelete )
				return new HttpResponseMessage( HttpStatusCode.NotFound );

			if( !sessionToDelete.UserId.Equals( ( Thread.CurrentPrincipal.Identity as DevSpaceIdentity )?.Identity.Id ?? -1 ) )
				return new HttpResponseMessage( HttpStatusCode.Unauthorized );

			try {
				if( await _DataStore.Delete( Id ) )
					return new HttpResponseMessage( HttpStatusCode.NoContent );
				else
					return new HttpResponseMessage( HttpStatusCode.InternalServerError );
			} catch {
				return new HttpResponseMessage( HttpStatusCode.InternalServerError );
			}
		}
	}
}
