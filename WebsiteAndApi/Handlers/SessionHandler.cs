using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using DevSpace.Common;
using DevSpace.Database;

namespace DevSpace.Api.Handlers {
	class SessionHandler : DelegatingHandler {
		private Guid GetSessionToken( HttpRequestMessage Request ) {
			if( ( null != Request.Headers.GetCookies() ) && ( 0 < Request.Headers.GetCookies().Count ) ) {
				foreach( CookieHeaderValue HeaderValue in Request.Headers.GetCookies() ) {
					foreach( CookieState State in HeaderValue.Cookies ) {
						if( State.Name == "SessionToken" ) {
							Guid SessionToken = Guid.Empty;
							if( Guid.TryParse( State.Value, out SessionToken ) ) {
								return SessionToken;
							}
						}
					}
				}
			}

			return Guid.Empty;
		}

		protected async override Task<HttpResponseMessage> SendAsync( HttpRequestMessage Request, CancellationToken CancelToken ) {
			Guid SessionToken = GetSessionToken( Request );
			if( !SessionToken.Equals( Guid.Empty ) ) {
				UserDataStore Users = new UserDataStore();
				IUser FoundUser = ( await Users.Get( "SessionToken", SessionToken ) ).FirstOrDefault();
				if( null != FoundUser ) {
					if( DateTime.UtcNow < FoundUser.SessionExpires ) {
						Thread.CurrentPrincipal = new GenericPrincipal( new DevSpaceIdentity( FoundUser ), null );
						Request.GetRequestContext().Principal = Thread.CurrentPrincipal;
					} else {
						// return 401
						HttpResponseMessage Response401 = new HttpResponseMessage( System.Net.HttpStatusCode.Unauthorized );
						Response401.Headers.Add( "Access-Control-Allow-Origin", Request.Headers.GetValues( "Origin" ).First() );
						Response401.Headers.Add( "Access-Control-Allow-Credentials", "true" );
						return Response401;
					}
				}

				HttpResponseMessage Response = await base.SendAsync( Request, CancelToken );

				await Users.Update( FoundUser.UpdateSessionExpires( DateTime.UtcNow.AddMinutes( 20 ) ) );

				return Response;
			}

			return await base.SendAsync( Request, CancelToken );
		}
	}
}
