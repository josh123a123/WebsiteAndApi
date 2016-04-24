using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
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

		[AllowAnonymous]
		public async Task<HttpResponseMessage> Post( [FromBody]string value ) {
			//return new HttpResponseMessage( HttpStatusCode.NotImplemented );

			// Check for .edu email
			if( string.IsNullOrWhiteSpace( value ) )
				return new HttpResponseMessage( HttpStatusCode.BadRequest );

			if( !value.Trim().EndsWith( ".edu", StringComparison.InvariantCultureIgnoreCase ) )
				return new HttpResponseMessage( HttpStatusCode.BadRequest );

			// Check DataStore for existing code
			IList<IStudentCode> ExistingCodes = await _DataStore.Get( "Email", value );

			//	If exists, resent existing code
			if( ExistingCodes.Count > 0 )
				SendEmail( ExistingCodes[0] );

			// Generate Code
			string Code = BitConverter.ToString( Guid.NewGuid().ToByteArray() ).Replace( "-", "" ).Substring( 0, 16 );
			while( 1 == ( await _DataStore.Get( "Code", Code ) ).Count )
				Code = BitConverter.ToString( Guid.NewGuid().ToByteArray() ).Replace( "-", "" ).Substring( 0, 16 );

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
					code = Code,
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

			using( HttpWebResponse EventBriteResponse = await EventBriteRequest.GetResponseAsync() as HttpWebResponse ) {
				using( StreamReader Reader = new StreamReader( EventBriteResponse.GetResponseStream() ) ) {
					string response = Reader.ReadToEnd();
					value = response;
				}
			}

			return new HttpResponseMessage( HttpStatusCode.NotImplemented );
			// Store Code in DataStore
			// Email Code
		}

		private void SendEmail( IStudentCode studentCode ) {
			throw new NotImplementedException();
		}
	}
}
