using System;
using System.Collections.Generic;
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
		/// <summary>User's Primary Email Address</summary>
		private string UserEmail { get; set; }

		/// <summary>Argon2i Hash of User's Password</summary>
		private string Argon2Password { get; set; }

		private ScryptEncoder _Hasher = null;
		private ScryptEncoder Hasher {
			get {
				if( null == _Hasher )
					_Hasher = new ScryptEncoder();

				return _Hasher;
			}
		}

		private void GetCredentials( string EncodedBasicAuth ) {
			byte[] HeaderBytes = Convert.FromBase64String( EncodedBasicAuth );

			string RawBasicAuth = Encoding.UTF8.GetString( HeaderBytes );
			UserEmail = RawBasicAuth.Substring( 0, RawBasicAuth.IndexOf( ':' ) );

			// This has the effect of using the email as a salt for the password.
			Argon2Password = Hasher.Encode( RawBasicAuth );

			// The RawBasicAuth has the password in clear text
			// Thus, we want to get rid of it as soon as possible.
			RawBasicAuth = null;

			// The HeaderBytes also has the raw password.
			// Thus, clear it out, now that we're done with it
			for( int index = 0; index < HeaderBytes.Length; ++index )
				HeaderBytes[index] = 0;
		}

		protected async override Task<HttpResponseMessage> SendAsync( HttpRequestMessage Request, CancellationToken CancelToken ) {
			if( null != Request.Headers.Authorization ) {
				// IF we have a "Basic" Authorization header, we need to login
				// ELSE we need to let the SessionToken cookie have a wack at it, so we do nothing
				if( Request.Headers.Authorization.Scheme.ToUpper() == "BASIC" ) {
					GetCredentials( Request.Headers.Authorization.Parameter );

					// Do Basic Auth
					UserDataStore Users = new UserDataStore();
					IList<IUser> FoundUsers = await Users.Get( "EmailAddress", UserEmail );

					// EmailAddress is a unique key, so we can only find one
					if( 0 != FoundUsers.Count ) {
						// IF Basic Auth Succeeds
						if( Hasher.Compare( Encoding.UTF8.GetString( Convert.FromBase64String( Request.Headers.Authorization.Parameter ) ), FoundUsers[0].PasswordHash ) ) {
							GenericIdentity UserIdentity = new GenericIdentity( UserEmail );
							GenericPrincipal UserPrincipal = new GenericPrincipal( UserIdentity, null );

							// Set Principle
							Thread.CurrentPrincipal = UserPrincipal;
							Request.GetRequestContext().Principal = UserPrincipal;
						}
						// ELSE Basic Auth Fails
						else {
							// return 401
							//Access-Control-Allow-Origin: https://www.devspaceconf.com
							//Access-Control-Allow-Credentials: true

							HttpResponseMessage Response401 = new HttpResponseMessage( System.Net.HttpStatusCode.Unauthorized );
							Response401.Headers.Add( "Access-Control-Allow-Origin", Request.Headers.GetValues( "Origin" ).First() );
							Response401.Headers.Add( "Access-Control-Allow-Credentials", "true" );
							return Response401;
						}
					}

					// Complete the request. We'll set the login cookie on the way out
					HttpResponseMessage Response = await base.SendAsync( Request, CancelToken );

					IUser UpdatedUser = FoundUsers[0].UpdateSessionToken( Guid.NewGuid() ).UpdateSessionExpires( DateTime.UtcNow.AddMinutes( 20 ) );
					await Users.Update( UpdatedUser );

					CookieHeaderValue SessionCookie = new CookieHeaderValue( "SessionToken", UpdatedUser.SessionToken.ToString() );
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
