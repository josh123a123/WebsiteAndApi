using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DevSpace.Common;

namespace DevSpace.Database {
	public class TagDataStore : IDataStore<ITag> {
		public async Task<ITag> Add( ITag ItemToAdd ) {
			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( "INSERT Tags ( Text ) VALUES ( @Text ); SELECT SCOPE_IDENTITY();", connection ) ) {
					command.Parameters.Add( "Text", SqlDbType.VarChar ).Value = ItemToAdd.Text;

					int newId = Convert.ToInt32( await command.ExecuteScalarAsync() );
					return ItemToAdd.UpdateId( newId );
				}
			}
		}

		public async Task<ITag> Get( int Id ) {
			throw new NotImplementedException();
		}

		public async Task<IList<ITag>> Get( string Field, object Value ) {
			throw new NotImplementedException();
		}

		public async Task<IList<ITag>> GetAll() {
			List<ITag> returnList = new List<ITag>();

			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( "SELECT * FROM Tags", connection ) ) {
					using( SqlDataReader dataReader = await command.ExecuteReaderAsync() ) {
						while( await dataReader.ReadAsync() ) {
							returnList.Add( new Models.TagModel( dataReader ) );
						}
					}
				}
			}

			return returnList;
		}

		public async Task<ITag> Update( ITag ItemToUpdate ) {
			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( "UPDATE Tags SET Text = @Text WHERE Id = @Id;", connection ) ) {
					command.Parameters.Add( "Id", SqlDbType.Int ).Value = ItemToUpdate.Id;
					command.Parameters.Add( "Text", SqlDbType.VarChar ).Value = ItemToUpdate.Text;
					await command.ExecuteNonQueryAsync();

					return ItemToUpdate;
				}
			}
		}
	}
}
