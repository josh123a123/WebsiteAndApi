using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using DevSpace.Common;
using DevSpace.Database;

namespace DevSpace.Api.Controllers {
	[AllowAnonymous]
	public class LoginController : ApiController {
		private IDataStore<IUser> _DataStore;
		public LoginController( IDataStore<IUser> DataStore ) {
			this._DataStore = DataStore;
		}

		public async Task<int> Get() {
			if( Thread.CurrentPrincipal?.Identity?.IsAuthenticated ?? false ) {
				IList<IUser> Users = await _DataStore.Get( "EmailAddress", Thread.CurrentPrincipal.Identity.Name );
				if( 0 < Users.Count )
					return Users[0].Id;
			}

			return -1;
		}

		public async Task<string> Get( [FromUri] string Email ) {
			IUser SelectedUser = ( await _DataStore.Get( "Email", Email ) ).FirstOrDefault();

			if( null == SelectedUser ) {
				return "Email Not Found";
			}

			AuthTokenDataStore TokenStore = new AuthTokenDataStore();
			IAuthToken Token = await TokenStore.Create( SelectedUser.Id );

			Email Mail = new Email( SelectedUser.EmailAddress, SelectedUser.DisplayName );
			Mail.Subject = "Password Request";
			Mail.Body = string.Format(
@"This email is a response to a forgotten password. The link below will force your login into the site. Once you are in, please update your password.

The link provided is good for one use and will expire in 5 minutes ({0} {1} UTC).

https://www.devspaceconf.com/login.aspx?force={2}", DateTime.UtcNow.AddMinutes( 5 ).ToShortDateString(), DateTime.UtcNow.AddMinutes( 5 ).ToShortTimeString(), Token.ToString() );

			Mail.Send();
			return "Email Sent";
		}
	}
}
