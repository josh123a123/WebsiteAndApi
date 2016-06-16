using System;
using System.Runtime.Serialization;

namespace DevSpace.Common {
	[DataContract]
	public class AuthToken : IAuthToken {
		private AuthToken() { }

		// Needed for Session
		internal AuthToken( IAuthToken Token ) {
			this.Token = Token.Token;
			this.UserId = Token.UserId;
			this.Expires = Token.Expires;
		}

		#region IToken
		[DataMember]
		public Guid Token { get; private set; }
		[DataMember]
		public int UserId { get; private set; }
		[DataMember]
		public DateTime Expires { get; private set; }

		public IAuthToken UpdateToken( Guid newToken ) {
			return new AuthToken {
				UserId = this.UserId,
				Token = newToken,
				Expires = this.Expires
			};
		}

		public IAuthToken UpdateUserId( int newUserId ) {
			return new AuthToken {
				UserId = newUserId,
				Token = this.Token,
				Expires = this.Expires
			};
		}

		public IAuthToken UpdateExpires( DateTime newExpires ) {
			return new AuthToken {
				UserId = this.UserId,
				Token = this.Token,
				Expires = newExpires
			};
		}
		#endregion
	}
}
