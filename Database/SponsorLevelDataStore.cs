using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DevSpace.Common;

namespace DevSpace.Database {
	public class SponsorLevelDataStore : IDataStore<ISponsorLevel> {
		public async Task<ISponsorLevel> Get( int Id ) {
			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				using( SqlCommand command = new SqlCommand( "SELECT * FROM SponsorLevels WHERE Id = @Id", connection ) ) {
					command.Parameters.Add( "Id", SqlDbType.Int ).Value = Id;

					using( SqlDataReader dataReader = await command.ExecuteReaderAsync() ) {
						if( await dataReader.ReadAsync() ) {
							return null;
						}
					}
				}
			}

			return null;
		}

		public async Task<IList<ISponsorLevel>> GetAll() {
			List<ISponsorLevel> returnList = new List<ISponsorLevel>();

			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				using( SqlCommand command = new SqlCommand( "SELECT * FROM SponsorLevels", connection ) ) {
					using( SqlDataReader dataReader = await command.ExecuteReaderAsync() ) {
						while( await dataReader.ReadAsync() ) {
							returnList.Add( new Models.SponsorLevelModel( dataReader ) );
						}
					}
				}
			}

			return returnList;
		}
	}
}
