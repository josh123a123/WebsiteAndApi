using System;
using System.Data.SqlClient;
using System.Reflection;
using DevSpace.Common;

namespace DevSpace.Database.Models {
	internal class UserModel : IUser {
		private UserModel() {}

		internal UserModel( SqlDataReader dataReader ) {
			for( int lcv = 0; lcv < dataReader.FieldCount; ++lcv ) {
				GetType().GetProperty( dataReader.GetName( lcv ), BindingFlags.Instance | BindingFlags.Public )?.SetValue( this, dataReader.GetValue( lcv ) );
			}
		}

		public string Bio { get; internal set; }
		public string DisplayName { get; internal set; }
		public string EmailAddress { get; internal set; }
		public int Id { get; internal set; }
		public string PasswordHash { get; internal set; }
		public byte Permissions { get; internal set; }
		public string Website { get; internal set; }
		public Guid SessionToken { get; internal set; }
		public DateTime SessionExpires { get; internal set; }

		public IUser UpdateId( int newId ) {
			UserModel newUser = Clone();
			newUser.Id = newId;
			return newUser;
		}

		public IUser UpdateDisplayName( string newDisplayName ) {
			UserModel newUser = Clone();
			newUser.DisplayName = newDisplayName;
			return newUser;
		}

		public IUser UpdateEmailAddress( string newEmailAddress ) {
			UserModel newUser = Clone();
			newUser.EmailAddress = newEmailAddress;
			return newUser;
		}

		public IUser UpdateBio( string newBio ) {
			UserModel newUser = Clone();
			newUser.Bio = newBio;
			return newUser;
		}

		public IUser UpdatePermissions( byte newPermissions ) {
			UserModel newUser = Clone();
			newUser.Permissions = newPermissions;
			return newUser;
		}

		public IUser UpdatePasswordHash( string newPasswordHash ) {
			UserModel newUser = Clone();
			newUser.PasswordHash = newPasswordHash;
			return newUser;
		}

		public IUser UpdateWebsite( string newWebsite ) {
			UserModel newUser = Clone();
			newUser.Website = newWebsite;
			return newUser;
		}

		public IUser UpdateSessionToken( Guid newSessionToken ) {
			UserModel newUser = Clone();
			newUser.SessionToken = newSessionToken;
			return newUser;
		}

		public IUser UpdateSessionExpires( DateTime newSessionExpires ) {
			UserModel newUser = Clone();
			newUser.SessionExpires = newSessionExpires;
			return newUser;
		}

		private UserModel Clone() {
			UserModel cloned = new UserModel();
			cloned.Id = this.Id;
			cloned.DisplayName = string.Copy( this.DisplayName );
			cloned.EmailAddress = string.Copy( this.EmailAddress );
			cloned.Bio = string.Copy( this.Bio );
			cloned.Permissions = this.Permissions;
			cloned.PasswordHash = string.Copy( this.PasswordHash );
			cloned.Website = string.Copy( this.Website );
			cloned.SessionToken = this.SessionToken;
			cloned.SessionExpires = this.SessionExpires;
			return cloned;
		}
	}
}
