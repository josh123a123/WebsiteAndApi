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
				IList<IUser> FoundUsers = await Users.Get( "SessionToken", SessionToken );
				if( 0 != FoundUsers.Count ) {
					if( DateTime.UtcNow < FoundUsers[0].SessionExpires ) {
						GenericIdentity UserIdentity = new GenericIdentity( FoundUsers[0].EmailAddress );
						GenericPrincipal UserPrincipal = new GenericPrincipal( UserIdentity, null );
						Thread.CurrentPrincipal = UserPrincipal;
						Request.GetRequestContext().Principal = UserPrincipal;
					} else {
						// return 401
						HttpResponseMessage Response401 = new HttpResponseMessage( System.Net.HttpStatusCode.Unauthorized );
						Response401.Headers.Add( "Access-Control-Allow-Origin", Request.Headers.GetValues( "Origin" ).First() );
						Response401.Headers.Add( "Access-Control-Allow-Credentials", "true" );
						return Response401;
					}
				}

				HttpResponseMessage Response = await base.SendAsync( Request, CancelToken );

				IUser UpdatedUser = FoundUsers[0].UpdateSessionExpires( DateTime.UtcNow.AddMinutes( 20 ) );
				await Users.Update( UpdatedUser );

				return Response;
			}

			return await base.SendAsync( Request, CancelToken );
		}
	}
}
