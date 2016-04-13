using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DevSpace.Common;

namespace DevSpace.Database {
	public class StudentCodeDataStore : IDataStore<IStudentCode> {
		public async Task<IStudentCode> Get( int Id ) {
			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( "SELECT * FROM StudentCodes WHERE Id = @Id", connection ) ) {
					command.Parameters.Add( "Id", SqlDbType.Int ).Value = Id;

					using( SqlDataReader dataReader = await command.ExecuteReaderAsync() ) {
						if( await dataReader.ReadAsync() ) {
							return new Models.StudentCodeModel( dataReader );
						}
					}
				}
			}

			return null;
		}

		public async Task<IList<IStudentCode>> GetAll() {
			throw new NotImplementedException();
		}

		public async Task<IList<IStudentCode>> Get( string Field, object Value ) {
			List<IStudentCode> returnList = new List<IStudentCode>();

			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( string.Format( "SELECT * FROM Sponsors WHERE {0} = @value", Field ), connection ) ) {
					command.Parameters.AddWithValue( Field, Value );

					using( SqlDataReader dataReader = await command.ExecuteReaderAsync() ) {
						while( await dataReader.ReadAsync() ) {
							returnList.Add( new Models.StudentCodeModel( dataReader ) );
						}
					}
				}
			}

			return returnList;
		}
	}
}
