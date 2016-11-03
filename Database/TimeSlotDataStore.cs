using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DevSpace.Common;

namespace DevSpace.Database {
	public class TimeSlotDataStore : IDataStore<ITimeSlot> {
		public async Task<ITimeSlot> Add( ITimeSlot ItemToAdd ) {
			throw new NotImplementedException();
		}

		public Task<bool> Delete( int Id ) {
			throw new NotImplementedException();
		}

		public async Task<ITimeSlot> Get( int Id ) {
			try {
				using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
					connection.Open();

					using( SqlCommand command = new SqlCommand( "SELECT * FROM TimeSlots WHERE Id = @Id", connection ) ) {
						command.Parameters.Add( "Id", SqlDbType.Int ).Value = Id;
						using( SqlDataReader dataReader = await command.ExecuteReaderAsync().ConfigureAwait( false ) ) {
							if( await dataReader.ReadAsync() ) {
								return new Models.TimeSlotModel( dataReader );
							}
						}
					}
				}
			} catch( Exception ex ) {
				return null;
			}
			return null;
		}

		public async Task<IList<ITimeSlot>> Get( string Field, object Value ) {
			throw new NotImplementedException();
		}

		public async Task<IList<ITimeSlot>> GetAll() {
			List<ITimeSlot> returnList = new List<ITimeSlot>();

			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( "SELECT * FROM TimeSlots", connection ) ) {
					using( SqlDataReader dataReader = await command.ExecuteReaderAsync() ) {
						while( await dataReader.ReadAsync() ) {
							returnList.Add( new Models.TimeSlotModel( dataReader ) );
						}
					}
				}
			}

			return returnList;
		}

		public async Task<ITimeSlot> Update( ITimeSlot ItemToUpdate ) {
			throw new NotImplementedException();
		}
	}
}
