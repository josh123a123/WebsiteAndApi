using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DevSpace.Common;

namespace DevSpace.Database {
	public class AuthTokenDataStore : IDataStore<IAuthToken> {
		public async Task<IAuthToken> Add( IAuthToken ItemToAdd ) {
			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( "INSERT Tokens ( Token, UserId, Expires ) VALUES ( @Token, @UserId, @Expires );", connection ) ) {
					command.Parameters.Add( "Token", SqlDbType.UniqueIdentifier ).Value = ItemToAdd.Token;
					command.Parameters.Add( "UserId", SqlDbType.Int ).Value = ItemToAdd.UserId;
					command.Parameters.Add( "Expries", SqlDbType.DateTime ).Value = ItemToAdd.Expires;

					await command.ExecuteNonQueryAsync();
					return ItemToAdd;
				}
			}
		}

		public async Task<bool> Delete( int Id ) {
			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( "DELETE Tokens WHERE Expires < @CurrentTime;", connection ) ) {
					command.Parameters.Add( "CurrentTime", SqlDbType.DateTime ).Value = DateTime.UtcNow;

					return 0 < await command.ExecuteNonQueryAsync();
				}
			}
		}

		public async Task<IAuthToken> Get( int Id ) {
			throw new NotImplementedException();
		}

		public async Task<IList<IAuthToken>> Get( string Field, object Value ) {
			List<IAuthToken> returnList = new List<IAuthToken>();

			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand sessionCommand = new SqlCommand( string.Format( "SELECT * FROM AuthTokens WHERE {0} = @value", Field ), connection ) ) {
					sessionCommand.Parameters.AddWithValue( "value", Value );

					using( SqlDataReader dataReader = await sessionCommand.ExecuteReaderAsync() ) {
						while( await dataReader.ReadAsync() ) {
							returnList.Add( new Models.AuthTokenModel( dataReader ) );
						}
					}
				}
			}

			return returnList;
		}

		public async Task<IList<IAuthToken>> GetAll() {
			List<IAuthToken> returnList = new List<IAuthToken>();

			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( "SELECT * FROM Tokens", connection ) ) {
					using( SqlDataReader dataReader = await command.ExecuteReaderAsync() ) {
						while( await dataReader.ReadAsync() ) {
							returnList.Add( new Models.AuthTokenModel( dataReader ) );
						}
					}
				}
			}

			return returnList;
		}

		public async Task<IAuthToken> Update( IAuthToken ItemToUpdate ) {
			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( "UPDATE AuthTokens SET USerId = @UserId, @Expires = @Expires WHERE Token = @Token;", connection ) ) {
					command.Parameters.Add( "Token", SqlDbType.UniqueIdentifier ).Value = ItemToUpdate.Token;
					command.Parameters.Add( "UserId", SqlDbType.Int ).Value = ItemToUpdate.UserId;
					command.Parameters.Add( "Expires", SqlDbType.DateTime ).Value = ItemToUpdate.Expires;
					await command.ExecuteNonQueryAsync();

					return ItemToUpdate;
				}
			}
		}
	}
}
