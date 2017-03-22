using System;

namespace DevSpace.Common {
	public interface IUser {
		int Id { get; }
		string DisplayName { get; }
		string EmailAddress { get; }
		string Bio { get; }
		string Twitter { get; }
		byte Permissions { get; }
		string PasswordHash { get; }
		string Website { get; }
		int GithubId { get; }
		Guid SessionToken { get; }
		DateTime SessionExpires { get; }

		IUser UpdateId( int newId );
		IUser UpdateDisplayName( string newDisplayName );
		IUser UpdateEmailAddress( string newEmailAddress );
		IUser UpdateBio( string newBio );
		IUser UpdateTwitter( string newTwitter );
		IUser UpdatePermissions( byte newPermissions );
		IUser UpdatePasswordHash( string newPasswordHash );
		IUser UpdateWebsite( string newWebsite );
		IUser UpdateGithubId( int newGithubId );
		IUser UpdateSessionToken( Guid newSessionToken );
		IUser UpdateSessionExpires( DateTime newSessionExpires );
	}
}
