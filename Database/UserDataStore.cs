using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DevSpace.Common;

namespace DevSpace.Database {
	public class UserDataStore : IDataStore<IUser> {
		public async Task<IUser> Add( IUser ItemToAdd ) {
			throw new NotImplementedException();
		}

		public async Task<IUser> Get( int Id ) {
			throw new NotImplementedException();
		}

		public async Task<IList<IUser>> Get( string Field, object Value ) {
			List<IUser> returnList = new List<IUser>();

			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( string.Format( "SELECT * FROM Users WHERE {0} = @value", Field ), connection ) ) {
					command.Parameters.AddWithValue( "value", Value );

					using( SqlDataReader dataReader = await command.ExecuteReaderAsync() ) {
						while( await dataReader.ReadAsync() ) {
							returnList.Add( new Models.UserModel( dataReader ) );
						}
					}
				}
			}

			return returnList;
		}

		public async Task<IList<IUser>> GetAll() {
			throw new NotImplementedException();
		}

		public async Task<IUser> Update( IUser ItemToUpdate ) {
			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( "UPDATE Users SET = @DisplayName, EmailAddress = @EmailAddress, Bio = @Bio, Permissions = @Permissions, PasswordHash = @PasswordHash, Website = @Website, SessionToken = @SessionToken, SessionExprires = @SessionExpires WHERE Id = @Id", connection ) ) {
					command.Parameters.Add( "Id", SqlDbType.Int ).Value = ItemToUpdate.Id;
					command.Parameters.Add( "DisplayName", SqlDbType.VarChar ).Value = ItemToUpdate.DisplayName;
					command.Parameters.Add( "EmailAddress", SqlDbType.VarChar ).Value = ItemToUpdate.EmailAddress;
					command.Parameters.Add( "Bio", SqlDbType.VarChar ).Value = ItemToUpdate.Bio;
					command.Parameters.Add( "Permissions", SqlDbType.TinyInt ).Value = ItemToUpdate.Permissions;
					command.Parameters.Add( "PasswordHash", SqlDbType.VarChar ).Value = ItemToUpdate.PasswordHash;
					command.Parameters.Add( "Website", SqlDbType.VarChar ).Value = ItemToUpdate.Website;
					command.Parameters.Add( "SessionToken", SqlDbType.UniqueIdentifier ).Value = ItemToUpdate.SessionToken;
					command.Parameters.Add( "SessionExpires", SqlDbType.DateTime ).Value = ItemToUpdate.SessionExpires;

					await command.ExecuteNonQueryAsync();
				}
			}

			return ItemToUpdate;
		}
	}
}
