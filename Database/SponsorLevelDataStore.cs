using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DevSpace.Common;

namespace DevSpace.Database {
	public class SponsorLevelDataStore : IDataStore<ISponsorLevel> {
		private static Dictionary<int, ISponsorLevel> Cache = new Dictionary<int, ISponsorLevel>();

		internal static async Task FillCache() {
			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( "SELECT * FROM SponsorLevels", connection ) ) {
					using( SqlDataReader dataReader = await command.ExecuteReaderAsync() ) {
						while( await dataReader.ReadAsync() ) {
							ISponsorLevel newLevel = new Models.SponsorLevelModel( dataReader );
							Cache.Add( newLevel.Id, newLevel );
						}
					}
				}
			}
		}

		public async Task<ISponsorLevel> Get( int Id ) {
			if( !Cache.ContainsKey( Id ) ) {
				Cache.Clear();
				await FillCache();
			}

			return Cache[Id];
		}

		public async Task<IList<ISponsorLevel>> GetAll() {
			throw new NotImplementedException();
		}

		public async Task<IList<ISponsorLevel>> Get( string Field, object Value ) {
			throw new NotImplementedException();
		}

		public Task<ISponsorLevel> Add( ISponsorLevel ItemToAdd ) {
			throw new NotImplementedException();
		}

		public Task<ISponsorLevel> Update( ISponsorLevel ItemToUpdate ) {
			throw new NotImplementedException();
		}
	}
}
