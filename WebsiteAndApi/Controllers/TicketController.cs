using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using DevSpace.Common;

namespace DevSpace.Api.Controllers {
	public class TicketController : ApiController {
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

			// Call EventBrite to create code
			//24347789895

			// NOTE: I removed this code, which had some things I didn't even want in the commit log, removed
			// This is because I have to switch back to master for a sponsor.
			// Don't forget to re-add this from the notepad on your desktop.

			return new HttpResponseMessage( HttpStatusCode.NotImplemented );
			// Store Code in DataStore
			// Email Code
		}

		private void SendEmail( IStudentCode studentCode ) {
			throw new NotImplementedException();
		}
	}
}
