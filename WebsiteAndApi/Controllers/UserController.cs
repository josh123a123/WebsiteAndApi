using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using DevSpace.Common;
using Newtonsoft.Json;
using Scrypt;

namespace DevSpace.Api.Controllers {
	public class JsonUserBinder : IModelBinder {
		public bool BindModel( HttpActionContext actionContext, ModelBindingContext bindingContext ) {
			HttpContent content = actionContext.Request.Content;
			string json = content.ReadAsStringAsync().Result;
			IStudentCode obj = JsonConvert.DeserializeObject<StudentCode>( json );
			bindingContext.Model = obj;
			return true;
		}
	}

	[AllowAnonymous]
	public class UserController : ApiController {
		private IDataStore<IUser> _DataStore;
		public UserController( IDataStore<IUser> DataStore ) {
			this._DataStore = DataStore;
		}

		[AllowAnonymous]
		public async Task<HttpResponseMessage> Post( [ModelBinder( typeof( JsonUserBinder ) )]IUser NewUser ) {
			if( null == NewUser )
				return new HttpResponseMessage( HttpStatusCode.BadRequest );

			ScryptEncoder Hasher = new ScryptEncoder();
			IUser ExistingRecord = ( await _DataStore.Get( "EmailAddress", NewUser.EmailAddress ) ).FirstOrDefault();

			// Register new profile
			if( null == ExistingRecord ) {
				await _DataStore.Add( NewUser.UpdatePasswordHash( Hasher.Encode( string.Format( "{0}:{1}", NewUser.EmailAddress, NewUser.PasswordHash ) ) ) );
				return new HttpResponseMessage( HttpStatusCode.Created );
			}

			// You can only update yourself
			if( !Thread.CurrentPrincipal?.Identity?.IsAuthenticated ?? true ) {
				return new HttpResponseMessage( HttpStatusCode.Unauthorized );
			}

			if( !Thread.CurrentPrincipal.Identity.Name.ToUpper().Equals( NewUser.EmailAddress.ToUpper() ) ) {
				return new HttpResponseMessage( HttpStatusCode.Unauthorized );
			}

			if( !string.IsNullOrWhiteSpace( NewUser.PasswordHash ) ) {
				NewUser.UpdatePasswordHash( Hasher.Encode( string.Format( "{0}:{1}", NewUser.EmailAddress, NewUser.PasswordHash ) ) );
			}

			await _DataStore.Update( NewUser );
			return new HttpResponseMessage( HttpStatusCode.OK );
		}
	}
}
