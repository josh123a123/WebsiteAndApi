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

				using( SqlCommand command = new SqlCommand( string.Format( "SELECT * FROM StudentCodes WHERE {0} = @value", Field ), connection ) ) {
					command.Parameters.AddWithValue( "value", Value );

					using( SqlDataReader dataReader = await command.ExecuteReaderAsync() ) {
						while( await dataReader.ReadAsync() ) {
							returnList.Add( new Models.StudentCodeModel( dataReader ) );
						}
					}
				}
			}

			return returnList;
		}

		public async Task<IStudentCode> Add( IStudentCode ItemToAdd ) {
			if( string.IsNullOrWhiteSpace( ItemToAdd.Code ) ) return null;
			if( string.IsNullOrWhiteSpace( ItemToAdd.Email ) ) return null;

			Models.StudentCodeModel returnValue = new Models.StudentCodeModel {
				Email = ItemToAdd.Email,
				Code = ItemToAdd.Code
			};

			using( SqlConnection connection = new SqlConnection( Settings.ConnectionString ) ) {
				connection.Open();

				using( SqlCommand command = new SqlCommand( "INSERT StudentCodes ( Email, Code ) VALUES ( @Email, @Code ); SELECT SCOPE_IDENTITY();", connection ) ) {
					command.Parameters.Add( "Code", SqlDbType.VarChar ).Value = ItemToAdd.Code;
					command.Parameters.Add( "Email", SqlDbType.VarChar ).Value = ItemToAdd.Email;
					returnValue.Id = Convert.ToInt32( await command.ExecuteScalarAsync() );
				}
			}

			return returnValue;
		}

		public Task<IStudentCode> Update( IStudentCode ItemToUpdate ) {
			throw new NotImplementedException();
		}
	}
}
