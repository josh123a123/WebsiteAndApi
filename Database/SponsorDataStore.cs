using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DevSpace.Common;

namespace DevSpace.Database {
	public class SponsorDataStore : IDataStore<ISponsor> {
		public async Task<ISponsor> Get( int Id ) {
			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( "SELECT * FROM Sponsors WHERE Id = @Id", connection ) ) {
					command.Parameters.Add( "Id", SqlDbType.Int ).Value = Id;

					using( SqlDataReader dataReader = await command.ExecuteReaderAsync() ) {
						if( await dataReader.ReadAsync() ) {
							return new Models.SponsorModel( dataReader );
						}
					}
				}
			}

			return null;
		}

		public async Task<IList<ISponsor>> GetAll() {
			List<ISponsor> returnList = new List<ISponsor>();

			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( "SELECT * FROM Sponsors", connection ) ) {
					using( SqlDataReader dataReader = await command.ExecuteReaderAsync() ) {
						while( await dataReader.ReadAsync() ) {
							returnList.Add( new Models.SponsorModel( dataReader ) );
						}
					}
				}
			}

			return returnList;
		}
	}
}
