using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevSpace.Common;
using DevSpace.Database;
using Scrypt;

namespace DevSpace.Api.Handlers {
	class BasicAuthHandler : DelegatingHandler {
		protected async override Task<HttpResponseMessage> SendAsync( HttpRequestMessage Request, CancellationToken CancelToken ) {
			if( null != Request.Headers.Authorization ) {
				// IF we have a "Basic" Authorization header, we need to login
				// ELSE we need to let the SessionToken cookie have a wack at it, so we do nothing
				if( Request.Headers.Authorization.Scheme.ToUpper() == "BASIC" ) {
					string authData = Encoding.UTF8.GetString( Convert.FromBase64String( Request.Headers.Authorization.Parameter ) );

					// Do Basic Auth
					UserDataStore Users = new UserDataStore();
					IUser FoundUser = ( await Users.Get( "EmailAddress", authData.Substring( 0, authData.IndexOf( ':' ) ) ) ).FirstOrDefault();

					// EmailAddress is a unique key, so we can only find one
					if( null != FoundUser ) {
						ScryptEncoder scryptEncoder = new ScryptEncoder();

						// IF Basic Auth Succeeds
						if( scryptEncoder.Compare( Encoding.UTF8.GetString( Convert.FromBase64String( Request.Headers.Authorization.Parameter ) ), FoundUser.PasswordHash ) ) {
							// Set Principle
							Thread.CurrentPrincipal = new GenericPrincipal( new DevSpaceIdentity( FoundUser ), null );
							Request.GetRequestContext().Principal = Thread.CurrentPrincipal;
						}
						// ELSE Basic Auth Fails
						else {
							// return 401
							return new HttpResponseMessage( System.Net.HttpStatusCode.Unauthorized );
						}
					}

					// Complete the request. We'll set the login cookie on the way out
					HttpResponseMessage Response = await base.SendAsync( Request, CancelToken );

					CookieHeaderValue SessionCookie = new CookieHeaderValue( "SessionToken", ( await Users.CreateSession( ( Thread.CurrentPrincipal.Identity as DevSpaceIdentity ).Identity ) ).SessionToken.ToString() );
#if DEBUG == false
					SessionCookie.Secure = true;
#endif
					SessionCookie.HttpOnly = true;
					Response.Headers.AddCookies( new CookieHeaderValue[] { SessionCookie } );

					return Response;
				}
			}

			return await base.SendAsync( Request, CancelToken );
		}
	}
}
