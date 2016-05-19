using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using DevSpace.Common;

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

//		public async Task<string> Get( [FromUri] string Email ) {
//			using( SqlConnection Connection = new SqlConnection( RoleEnvironment.GetConfigurationSettingValue( "DatabaseConnectionString" ) ) ) {
//				Connection.Open();

//				Models.Speaker SelectedSpeaker = await Models.Speaker.Select( Email, Connection );

//				if( null == SelectedSpeaker ) {
//					return "Email Not Found";
//				}

//				Guid Token = Guid.NewGuid();

//				using( SqlCommand Command = new SqlCommand() ) {
//					Command.Connection = Connection;
//					Command.CommandText = "INSERT Tokens ( Token, SpeakerId, Expires ) VALUES ( @Token, @SpeakerId, @Expires )";
//					Command.Parameters.Add( "Token", System.Data.SqlDbType.UniqueIdentifier ).Value = Token;
//					Command.Parameters.Add( "SpeakerId", System.Data.SqlDbType.SmallInt ).Value = SelectedSpeaker.Id;
//					Command.Parameters.Add( "Expires", System.Data.SqlDbType.DateTime ).Value = DateTime.UtcNow.AddMinutes( 5 );
//					await Command.ExecuteNonQueryAsync();
//				}

//				MailMessage Mail = new MailMessage( RoleEnvironment.GetConfigurationSettingValue( "SmtpUsername" ), Email );
//				Mail.Subject = "Password Request";
//				Mail.Body = string.Format(
//@"This email is a response to a forgotten password. The link below will force your login into the site. Once you are in, please update your password.

//The link provided is good for one use and will expire in 5 minutes ({0} {1} UTC).

//https://www.devspaceconf.com/login.aspx?force={2}", DateTime.UtcNow.AddMinutes( 5 ).ToShortDateString(), DateTime.UtcNow.AddMinutes( 5 ).ToShortTimeString(), Token.ToString() );

//				NetworkCredential Creds = new NetworkCredential( RoleEnvironment.GetConfigurationSettingValue( "SmtpUsername" ), RoleEnvironment.GetConfigurationSettingValue( "SmtpPassword" ) );

//				SmtpClient Client = new SmtpClient( RoleEnvironment.GetConfigurationSettingValue( "SmtpHost" ), Convert.ToInt32( RoleEnvironment.GetConfigurationSettingValue( "SmtpPort" ) ) );
//				Client.EnableSsl = false;
//				Client.Credentials = Creds;
//				Client.Send( Mail );

//				return "Email Sent";
//			}
//		}
	}
}
