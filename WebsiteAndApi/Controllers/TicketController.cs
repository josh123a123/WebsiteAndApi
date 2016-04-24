using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using DevSpace.Common;
using Newtonsoft.Json;

namespace DevSpace.Api.Controllers {
	public class TicketController : ApiController {
		private class AccessCode {
			public string code { get; set; }
			public string[] ticket_ids { get; set; }
			public int quantity_available { get; set; }
		}

		private class EventBriteApiObject {
			public AccessCode access_code { get; set; }
		}

		private IDataStore<IStudentCode> _DataStore;
		public TicketController( IDataStore<IStudentCode> DataStore ) {
			this._DataStore = DataStore;
		}

		private class MutableStudentCode : IStudentCode {
			public string Code { get; set; }
			public string Email { get; set; }
			public int Id { get; set; }
		};

		[AllowAnonymous]
		public async Task<HttpResponseMessage> Post( [FromBody]IStudentCode value ) {
			MutableStudentCode NewStudentCode = value as MutableStudentCode;
			if( null == NewStudentCode ) return new HttpResponseMessage( HttpStatusCode.BadRequest );

			// Check for .edu email
			if( string.IsNullOrWhiteSpace( NewStudentCode.Email ) )
				return new HttpResponseMessage( HttpStatusCode.BadRequest );

			if( !NewStudentCode.Email.Trim().EndsWith( ".edu", StringComparison.InvariantCultureIgnoreCase ) )
				return new HttpResponseMessage( HttpStatusCode.BadRequest );

			// Check DataStore for existing code
			IList<IStudentCode> ExistingCodes = await _DataStore.Get( "Email", NewStudentCode.Email );

			//	If exists, resent existing code
			if( ExistingCodes.Count > 0 )
				SendEmail( ExistingCodes[0] );

			// Generate Code
			NewStudentCode.Code = BitConverter.ToString( Guid.NewGuid().ToByteArray() ).Replace( "-", "" ).Substring( 0, 16 );
			while( 1 == ( await _DataStore.Get( "Code", NewStudentCode.Code ) ).Count )
				NewStudentCode.Code = BitConverter.ToString( Guid.NewGuid().ToByteArray() ).Replace( "-", "" ).Substring( 0, 16 );

			// Call EventBrite to create code
#if DEBUG
			string EventBriteApiKey = Environment.GetEnvironmentVariable( "EVENTBRITEAPIKEY", EnvironmentVariableTarget.Machine );
			string EventBriteEventId = Environment.GetEnvironmentVariable( "EVENTBRITEEVENTID", EnvironmentVariableTarget.Machine );
			string EventBriteTicketId = Environment.GetEnvironmentVariable( "EVENTBRITETICKETID", EnvironmentVariableTarget.Machine );
#else
			string EventBriteApiKey = ConfigurationManager.AppSettings["EventBriteApiKey"];
			string EventBriteEventId = ConfigurationManager.AppSettings["EventBriteEventId"];
			string EventBriteTicketId = ConfigurationManager.AppSettings["EventBriteTicketId"];
#endif
			EventBriteApiObject JsonObject = new EventBriteApiObject {
				access_code =  new AccessCode {
					code = NewStudentCode.Code,
					ticket_ids = new string[] { EventBriteTicketId },
					quantity_available = 1
				}
			};

			string Uri = string.Format( "https://www.eventbriteapi.com/v3/events/{0}/access_codes/", EventBriteEventId );

			HttpWebRequest EventBriteRequest = HttpWebRequest.Create( Uri ) as HttpWebRequest;
			EventBriteRequest.Headers.Add( "Authorization", string.Format( "Bearer {0}", EventBriteApiKey ) );
			EventBriteRequest.ContentType = "application/json";
			EventBriteRequest.Accept = "application/json";
			EventBriteRequest.Method = "POST";

			byte[] RequestBytes = Encoding.UTF8.GetBytes( JsonConvert.SerializeObject( JsonObject, Formatting.None ) );
			EventBriteRequest.ContentLength = RequestBytes.Length;
			using( Stream RequestStream = EventBriteRequest.GetRequestStream() )
				RequestStream.Write( RequestBytes, 0, RequestBytes.Length );

			try {
				using( HttpWebResponse EventBriteResponse = await EventBriteRequest.GetResponseAsync() as HttpWebResponse );
			} catch( WebException ) {
				return new HttpResponseMessage( HttpStatusCode.BadGateway );
			} catch( Exception ) {
				return new HttpResponseMessage( HttpStatusCode.InternalServerError );
			}

			// Store Code in DataStore
			NewStudentCode.Id = ( await _DataStore.Add( NewStudentCode ) ).Id;

			// Email Code
			SendEmail( NewStudentCode );

			return new HttpResponseMessage( HttpStatusCode.Created );
		}

		private void SendEmail( IStudentCode studentCode ) {
			MailMessage Mail = new MailMessage( RoleEnvironment.GetConfigurationSettingValue( "SmtpUsername" ), studentCode.Email );
			Mail.Subject = "Student Ticket Code";
			Mail.Body = string.Format(
@"This email is a response to a forgotten password. The link below will force your login into the site. Once you are in, please update your password.

The link provided is good for one use and will expire in 5 minutes ({0} {1} UTC).

https://www.devspaceconf.com/login.aspx?force={2}", DateTime.UtcNow.AddMinutes( 5 ).ToShortDateString(), DateTime.UtcNow.AddMinutes( 5 ).ToShortTimeString(), Token.ToString() );

			NetworkCredential Creds = new NetworkCredential( RoleEnvironment.GetConfigurationSettingValue( "SmtpUsername" ), RoleEnvironment.GetConfigurationSettingValue( "SmtpPassword" ) );

			SmtpClient Client = new SmtpClient( RoleEnvironment.GetConfigurationSettingValue( "SmtpHost" ), Convert.ToInt32( RoleEnvironment.GetConfigurationSettingValue( "SmtpPort" ) ) );
			Client.EnableSsl = false;
			Client.Credentials = Creds;
			Client.Send( Mail );
		}
	}
}
