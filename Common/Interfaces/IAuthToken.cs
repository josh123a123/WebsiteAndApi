using System;

namespace DevSpace.Common {
	public interface IAuthToken {
		Guid Token { get; }
		int UserId { get; }
		DateTime Expires { get; }

		IAuthToken UpdateToken( Guid newToken );
		IAuthToken UpdateUserId( int newUserId );
		IAuthToken UpdateExpires( DateTime newExpires );
	}
}
