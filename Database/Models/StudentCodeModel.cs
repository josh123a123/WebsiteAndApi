using System.Data.SqlClient;
using System.Reflection;
using DevSpace.Common;

namespace DevSpace.Database.Models {
	internal class StudentCodeModel : IStudentCode {
		internal StudentCodeModel() {
		}

		internal StudentCodeModel( SqlDataReader dataReader ) {
			for( int lcv = 0; lcv < dataReader.FieldCount; ++lcv ) {
				GetType().GetProperty( dataReader.GetName( lcv ), BindingFlags.Instance | BindingFlags.Public )?.SetValue( this, dataReader.GetValue( lcv ) );
			}
		}

		public string Email { get; internal set; }
		public string Code { get; internal set; }
		public int Id { get; internal set; }
	}
}
