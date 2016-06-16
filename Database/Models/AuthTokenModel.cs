using System;
using System.Data.SqlClient;
using System.Reflection;
using DevSpace.Common;

namespace DevSpace.Database.Models {
	public class AuthTokenModel : IAuthToken {
		private AuthTokenModel() { }

		internal AuthTokenModel( SqlDataReader dataReader ) {
			if( null == dataReader ) return;

			for( int lcv = 0; lcv < dataReader.FieldCount; ++lcv ) {
				GetType().GetProperty( dataReader.GetName( lcv ), BindingFlags.Instance | BindingFlags.Public )?.SetValue( this, dataReader.GetValue( lcv ) );
			}
		}

		#region IToken
		public Guid Token { get; private set; }
		public int UserId { get; private set; }
		public DateTime Expires { get; private set; }

		public IAuthToken UpdateToken( Guid newToken ) {
			return new AuthTokenModel {
				UserId = this.UserId,
				Token = newToken,
				Expires = this.Expires
			};
		}

		public IAuthToken UpdateUserId( int newUserId ) {
			return new AuthTokenModel {
				UserId = newUserId,
				Token = this.Token,
				Expires = this.Expires
			};
		}

		public IAuthToken UpdateExpires( DateTime newExpires ) {
			return new AuthTokenModel {
				UserId = this.UserId,
				Token = this.Token,
				Expires = newExpires
			};
		}
		#endregion
	}
}
