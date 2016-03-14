using System.Configuration;
using System.Reflection;
using System.Text;

namespace DevSpace.Database {
	internal static class Settings {
		static Settings() {
			foreach( PropertyInfo property in typeof( Settings ).GetProperties( BindingFlags.Static ) ) {
				property.SetValue( null, ConfigurationManager.AppSettings[property.Name] );
			}	
		}

		public static string Server {
			get;
			private set;
		}

		public static string Database {
			get;
			private set;
		}

		public static string Trusted_Connection {
			get;
			private set;
		}

		public static string UserID {
			get;
			private set;
		}

		public static string Password {
			get;
			private set;
		}

		public static string Encrypt {
			get;
			private set;
		}

		public static string ConnectionTimeout {
			get;
			private set;
		}

		public static string MultipleActiveResultSets {
			get;
			private set;
		}

		public static string ConnectionString {
			get {
				if( string.IsNullOrWhiteSpace( Server ) ) {
					foreach( PropertyInfo property in typeof( Settings ).GetProperties( BindingFlags.Static | BindingFlags.Public ) ) {
						property.SetValue( null, ConfigurationManager.AppSettings[property.Name] );
					}
				}

				StringBuilder builder = new StringBuilder();
				builder.AppendFormat( "Server={0};Database={1};", Server, Database );

				if( "TRUE".Equals( Trusted_Connection.ToUpper() ) ) {
					builder.Append( "Trusted_Connection=True;" );
				} else {
					builder.AppendFormat( "User ID={0};Password={1};Trusted_Connection=False;", UserID, Password );
				}

				builder.AppendFormat( "Encrypt={0};Connection Timeout={1};MultipleActiveResultSets={2}", Encrypt, ConnectionTimeout, MultipleActiveResultSets );
				return builder.ToString();
			}
		}
	}
}
