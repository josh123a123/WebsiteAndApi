using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DevSpace.Common;

namespace DevSpace.Database {
	public class RoomDataStore : IDataStore<IRoom> {
		public async Task<IRoom> Add( IRoom ItemToAdd ) {
			throw new NotImplementedException();
		}

		public Task<bool> Delete( int Id ) {
			throw new NotImplementedException();
		}

		public async Task<IRoom> Get( int Id ) {
			try {
				using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
					connection.Open();

					using( SqlCommand command = new SqlCommand( "SELECT * FROM Rooms WHERE Id = @Id", connection ) ) {
						command.Parameters.Add( "Id", SqlDbType.Int ).Value = Id;
						using( SqlDataReader dataReader = await command.ExecuteReaderAsync().ConfigureAwait( false ) ) {
							if( await dataReader.ReadAsync() ) {
								return new Models.RoomModel( dataReader );
							}
						}
					}
				}
			} catch( Exception ex ) {
				return null;
			}
			return null;
		}

		public async Task<IList<IRoom>> Get( string Field, object Value ) {
			throw new NotImplementedException();
		}

		public async Task<IList<IRoom>> GetAll() {
			List<IRoom> returnList = new List<IRoom>();

			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( "SELECT * FROM Rooms", connection ) ) {
					using( SqlDataReader dataReader = await command.ExecuteReaderAsync() ) {
						while( await dataReader.ReadAsync() ) {
							returnList.Add( new Models.RoomModel( dataReader ) );
						}
					}
				}
			}

			return returnList;
		}

		public async Task<IRoom> Update( IRoom ItemToUpdate ) {
			throw new NotImplementedException();
		}
	}
}
