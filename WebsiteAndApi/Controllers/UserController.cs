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
using Newtonsoft.Json.Linq;
using Scrypt;

namespace DevSpace.Api.Controllers {
	public class JsonUserBinder : IModelBinder {
		public bool BindModel( HttpActionContext actionContext, ModelBindingContext bindingContext ) {
			HttpContent content = actionContext.Request.Content;
			string json = content.ReadAsStringAsync().Result;
			IUser obj = JsonConvert.DeserializeObject<User>( json );

			// The PasswordHash isn't in the DataContract
			JObject raw = JsonConvert.DeserializeObject<JObject>( json );
			obj = obj.UpdatePasswordHash( raw["PasswordHash"]?.ToString() );

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

		[Authorize]
		public async Task<HttpResponseMessage> Get( int Id ) {
			IUser ExistingUser = await _DataStore.Get( Id );

			if( null == ExistingUser )
				return new HttpResponseMessage( HttpStatusCode.NotFound );

			if( Thread.CurrentPrincipal?.Identity?.Name.Equals( ExistingUser.EmailAddress, System.StringComparison.InvariantCultureIgnoreCase ) ?? false ) {
				HttpResponseMessage Response = new HttpResponseMessage( HttpStatusCode.OK );
				Response.Content = new StringContent( await Task.Factory.StartNew( () => JsonConvert.SerializeObject( ExistingUser, Formatting.None ) ) );
				return Response;
			} else {
				return new HttpResponseMessage( HttpStatusCode.Unauthorized );
			}
		}

		[AllowAnonymous]
		public async Task<HttpResponseMessage> Post( [ModelBinder( typeof( JsonUserBinder ) )]IUser NewUser ) {
			if( null == NewUser )
				return new HttpResponseMessage( HttpStatusCode.BadRequest );

			ScryptEncoder Hasher = new ScryptEncoder();
			IUser ExistingRecord = await _DataStore.Get( NewUser.Id );

			// Register new profile
			if( null == ExistingRecord ) {
				await _DataStore.Add( NewUser.UpdatePasswordHash( Hasher.Encode( string.Format( "{0}:{1}", NewUser.EmailAddress, NewUser.PasswordHash ) ) ) );
				return new HttpResponseMessage( HttpStatusCode.Created );
			}

			// You can only post a new profile if you're not authenticated
			if( !Thread.CurrentPrincipal?.Identity?.IsAuthenticated ?? true ) {
				return new HttpResponseMessage( HttpStatusCode.Unauthorized );
			}

			// You can only update yourself
			if( !( Thread.CurrentPrincipal.Identity as DevSpaceIdentity ).Identity.Id.Equals( NewUser.Id ) ) {
				return new HttpResponseMessage( HttpStatusCode.Unauthorized );
			}

			if( !string.IsNullOrWhiteSpace( NewUser.PasswordHash ) ) {
				NewUser = NewUser.UpdatePasswordHash( Hasher.Encode( string.Format( "{0}:{1}", NewUser.EmailAddress, NewUser.PasswordHash ) ) );
			}

			await _DataStore.Update( NewUser );
			return new HttpResponseMessage( HttpStatusCode.OK );
		}
	}
}
