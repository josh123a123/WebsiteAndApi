using System;
using System.Net;
using System.Net.Http;
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
			return new HttpResponseMessage( HttpStatusCode.NotImplemented );

			// Check for .edu email
			if( string.IsNullOrWhiteSpace( value ) )
				return new HttpResponseMessage( HttpStatusCode.BadRequest );

			if( !value.Trim().EndsWith( ".edu", StringComparison.InvariantCultureIgnoreCase ) )
				return new HttpResponseMessage( HttpStatusCode.BadRequest );

			// Check DataStore for existing code
			//	If exists, resent existing code
			// Call EventBrite to create code
			// Store Code in DataStore
			// Email Code
		}
	}
}
