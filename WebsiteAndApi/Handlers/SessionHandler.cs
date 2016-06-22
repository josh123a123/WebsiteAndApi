using System;
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
						// We previously sent expired tokens back with a 401.
						// This was causing errors with password retreval.
						// So, just don't validate an expired token and let
						// the Authorize attributes handle the 401
						return await base.SendAsync( Request, CancelToken );
					}
				}

				HttpResponseMessage Response = await base.SendAsync( Request, CancelToken );

				await Users.UpdateSession( ( Thread.CurrentPrincipal.Identity as DevSpaceIdentity ).Identity );

				return Response;
			}

			return await base.SendAsync( Request, CancelToken );
		}
	}
}
