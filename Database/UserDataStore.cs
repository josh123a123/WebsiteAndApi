using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DevSpace.Common;

namespace DevSpace.Database {
	public class UserDataStore : IDataStore<IUser> {
		public async Task<IUser> Add( IUser ItemToAdd ) {
			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( "INSERT Users ( DisplayName, EmailAddress, Bio, Permissions, PasswordHash, Twitter, Website, SessionToken, SessionExpires ) VALUES ( @DisplayName, @EmailAddress, @Bio, @Permissions, @PasswordHash, @Twitter, @Website, @SessionToken, @SessionExpires ); SELECT SCOPE_IDENTITY();", connection ) ) {
					command.Parameters.Add( "DisplayName", SqlDbType.VarChar ).Value = ItemToAdd.DisplayName;
					command.Parameters.Add( "EmailAddress", SqlDbType.VarChar ).Value = ItemToAdd.EmailAddress;

					if( string.IsNullOrWhiteSpace( ItemToAdd.Bio ) )
						command.Parameters.Add( "Bio", SqlDbType.VarChar ).Value = DBNull.Value;
					else
						command.Parameters.Add( "Bio", SqlDbType.VarChar ).Value = ItemToAdd.Bio;

					command.Parameters.Add( "Permissions", SqlDbType.TinyInt ).Value = ItemToAdd.Permissions;
					command.Parameters.Add( "PasswordHash", SqlDbType.VarChar ).Value = ItemToAdd.PasswordHash;

					if( string.IsNullOrWhiteSpace( ItemToAdd.Twitter ) )
						command.Parameters.Add( "Twitter", SqlDbType.VarChar ).Value = DBNull.Value;
					else
						command.Parameters.Add( "Twitter", SqlDbType.VarChar ).Value = ItemToAdd.Twitter;

					if( string.IsNullOrWhiteSpace( ItemToAdd.Website ) )
						command.Parameters.Add( "Website", SqlDbType.VarChar ).Value = DBNull.Value;
					else
						command.Parameters.Add( "Website", SqlDbType.VarChar ).Value = ItemToAdd.Website;

					if( Guid.Empty.Equals( ItemToAdd.SessionToken ) )
						command.Parameters.Add( "SessionToken", SqlDbType.UniqueIdentifier ).Value = DBNull.Value;
					else
						command.Parameters.Add( "SessionToken", SqlDbType.UniqueIdentifier ).Value = ItemToAdd.SessionToken;

					if( DateTime.MinValue.Equals( ItemToAdd.SessionExpires ) )
						command.Parameters.Add( "SessionExpires", SqlDbType.DateTime ).Value = DBNull.Value;
					else
						command.Parameters.Add( "SessionExpires", SqlDbType.DateTime ).Value = ItemToAdd.SessionExpires;


					return ItemToAdd.UpdateId( Convert.ToInt32( await command.ExecuteScalarAsync() ) );
				}
			}
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

				using( SqlCommand command = new SqlCommand( "UPDATE Users SET DisplayName = @DisplayName, EmailAddress = @EmailAddress, Bio = @Bio, Permissions = @Permissions, PasswordHash = @PasswordHash, Twitter = @Twitter, Website = @Website, SessionToken = @SessionToken, SessionExprires = @SessionExpires WHERE Id = @Id", connection ) ) {
					command.Parameters.Add( "Id", SqlDbType.Int ).Value = ItemToUpdate.Id;
					command.Parameters.Add( "DisplayName", SqlDbType.VarChar ).Value = ItemToUpdate.DisplayName;
					command.Parameters.Add( "EmailAddress", SqlDbType.VarChar ).Value = ItemToUpdate.EmailAddress;

					if( string.IsNullOrWhiteSpace( ItemToUpdate.Bio ) )
						command.Parameters.Add( "Bio", SqlDbType.VarChar ).Value = DBNull.Value;
					else
						command.Parameters.Add( "Bio", SqlDbType.VarChar ).Value = ItemToUpdate.Bio;

					command.Parameters.Add( "Permissions", SqlDbType.TinyInt ).Value = ItemToUpdate.Permissions;
					command.Parameters.Add( "PasswordHash", SqlDbType.VarChar ).Value = ItemToUpdate.PasswordHash;

					if( string.IsNullOrWhiteSpace( ItemToUpdate.Twitter ) )
						command.Parameters.Add( "Twitter", SqlDbType.VarChar ).Value = DBNull.Value;
					else
						command.Parameters.Add( "Twitter", SqlDbType.VarChar ).Value = ItemToUpdate.Twitter;

					if( string.IsNullOrWhiteSpace( ItemToUpdate.Website ) )
						command.Parameters.Add( "Website", SqlDbType.VarChar ).Value = DBNull.Value;
					else
						command.Parameters.Add( "Website", SqlDbType.VarChar ).Value = ItemToUpdate.Website;

					if(  Guid.Empty.Equals( ItemToUpdate.SessionToken ) )
						command.Parameters.Add( "SessionToken", SqlDbType.UniqueIdentifier ).Value = DBNull.Value;
					else
						command.Parameters.Add( "SessionToken", SqlDbType.UniqueIdentifier ).Value = ItemToUpdate.SessionToken;

					if( DateTime.MinValue.Equals( ItemToUpdate.SessionExpires ) )
						command.Parameters.Add( "SessionExpires", SqlDbType.DateTime ).Value = DBNull.Value;
					else
						command.Parameters.Add( "SessionExpires", SqlDbType.DateTime ).Value = ItemToUpdate.SessionExpires;

					await command.ExecuteNonQueryAsync();
				}
			}

			return ItemToUpdate;
		}
	}
}
