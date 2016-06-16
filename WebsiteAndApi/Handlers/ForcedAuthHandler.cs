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
	class ForcedAuthHandler : DelegatingHandler {
		protected async override Task<HttpResponseMessage> SendAsync( HttpRequestMessage Request, CancellationToken CancelToken ) {
			if( null != Request.Headers.Authorization ) {
				if( Request.Headers.Authorization.Scheme.ToUpper() == "FORCE" ) {
					Guid Token;
					if( !Guid.TryParse( Request.Headers.Authorization.Parameter, out Token ) ) {
						HttpResponseMessage Response401 = new HttpResponseMessage( System.Net.HttpStatusCode.Unauthorized );
						Response401.Headers.Add( "Access-Control-Allow-Origin", Request.Headers.GetValues( "Origin" ).First() );
						Response401.Headers.Add( "Access-Control-Allow-Credentials", "true" );
						return Response401;
					}

					AuthTokenDataStore Tokens = new AuthTokenDataStore();
					await Tokens.Delete( 0 );
					IAuthToken ValidToken = ( await Tokens.Get( "Token", Token ) ).FirstOrDefault();

					if( null == ValidToken ) {
						HttpResponseMessage Response401 = new HttpResponseMessage( System.Net.HttpStatusCode.Unauthorized );
						Response401.Headers.Add( "Access-Control-Allow-Origin", Request.Headers.GetValues( "Origin" ).First() );
						Response401.Headers.Add( "Access-Control-Allow-Credentials", "true" );
						return Response401;
					}

					UserDataStore Users = new UserDataStore();
					IUser User = await Users.Get( ValidToken.UserId );

					if( null == User ) {
						HttpResponseMessage Response401 = new HttpResponseMessage( System.Net.HttpStatusCode.Unauthorized );
						Response401.Headers.Add( "Access-Control-Allow-Origin", Request.Headers.GetValues( "Origin" ).First() );
						Response401.Headers.Add( "Access-Control-Allow-Credentials", "true" );
						return Response401;
					}

					Thread.CurrentPrincipal = new GenericPrincipal( new DevSpaceIdentity( User ), null );
					Request.GetRequestContext().Principal = Thread.CurrentPrincipal;

					await Tokens.Update( ValidToken.UpdateExpires( DateTime.UtcNow.AddMinutes( -1 ) ) );

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
