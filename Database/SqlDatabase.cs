using System;
using System.Data.SqlClient;
using DevSpace.Common;

namespace DevSpace.Database {
	public class SqlDatabase : IDatabase {
		private SqlConnection Connection;

		private bool ConnectToMaster() {
			try {
				SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder( Settings.ConnectionString );
				builder.InitialCatalog = "master";

				Connection = new SqlConnection( builder.ConnectionString );
				Connection.Open();
				return true;
			} catch( Exception Ex ) {
				return false;
			}
		}

		private bool CreateDatabase() {
			try {
				SqlCommand Command = new SqlCommand();
				Command.Connection = Connection;
				Command.CommandText = "CREATE DATABASE " + Settings.Database + ";";
				Command.ExecuteNonQuery();
				return true;
			} catch( Exception Ex ) {
				return false;
			}
		}

		private bool CreateVersionInfo() {
			try {
				SqlCommand Command = new SqlCommand();
				Command.Connection = Connection;
				Command.CommandText =
@"CREATE TABLE VersionInfo (
	DbVersion	VARCHAR(16)	NOT NULL,

	CONSTRAINT VersionInfo_PK PRIMARY KEY ( DbVersion )
);

INSERT VersionInfo ( DbVersion ) VALUES ( '00.00.00.0000' );";
				Command.ExecuteNonQuery();
				return true;
			} catch( Exception Ex ) {
				return false;
			}
		}

		private bool ConnectToDb() {
			try {
				Connection = new SqlConnection( Settings.ConnectionString );
				Connection.Open();
				return true;
			} catch( Exception Ex ) {
				return false;
			}
		}

		private string GetDatabaseVersion() {
			try {
				SqlCommand Command = new SqlCommand();
				Command.Connection = Connection;
				Command.CommandText = "SELECT DbVersion FROM VersionInfo;";
				return Command.ExecuteScalar()?.ToString();
			} catch( Exception Ex ) {
				return string.Empty;
			}
		}

		private string GetUpgradeScript( string DatabaseVersion ) {
			switch( DatabaseVersion ) {
				case "": // Because string.Empty is not a constant...
				case "00.00.00.0000":
					return
@"CREATE TABLE SponsorLevels (
	Id					INT			IDENTITY(1,1)	NOT NULL,
	DisplayOrder		INT							NOT NULL,
	DisplayName			VARCHAR(16)					NOT NULL,
	DisplayInEmails		BIT							NOT NULL,
	DisplayInSidebar	BIT							NOT NULL,

	CONSTRAINT SponsorLevels_PK PRIMARY KEY NONCLUSTERED ( Id ),
	CONSTRAINT SponsorLevels_CI UNIQUE CLUSTERED ( DisplayOrder )
);

CREATE TABLE Sponsors (
	Id				INT				IDENTITY(1,1)	NOT NULL,
	DisplayName		VARCHAR(16)						NOT NULL,
	Level			INT								NOT NULL,
	LogoSmall		VARCHAR(64)						NOT NULL,
	LogoLarge		VARCHAR(64)						NOT NULL,

	CONSTRAINT Sponsor_PK PRIMARY KEY ( Id ),
	CONSTRAINT Sponsors_SponsorLevels_FK FOREIGN KEY ( Level ) REFERENCES SponsorLevels ( Id )
);

UPDATE VersionInfo SET DbVersion = '01.00.00.0000';";

				default:
					return string.Empty;
			}
		}

		private bool RunUpgradeScript( string UpgradeScript ) {
			try {
				SqlCommand Command = new SqlCommand();
				Command.Connection = Connection;
				Command.CommandText = UpgradeScript;
				Command.ExecuteNonQuery();
				return true;
			} catch( Exception Ex ) {
				return false;
			}
		}

		public void Initialize() {
			// Yes, this thing in a little nasty...

			if( !ConnectToDb() ) {
				if( !ConnectToMaster() )
					throw new Exception( "Database not found." );

				if( !CreateDatabase() )
					throw new Exception( "Could not create database" );

				// At this point, this is a connection to master;
				Connection.Close();

				if( !ConnectToDb() )
					throw new Exception( "Could not connect to database" );

				if( !CreateVersionInfo() )
					throw new Exception( "Could not create version information" );
			}

			do {
				string DatabaseVersion = GetDatabaseVersion();
				if( string.IsNullOrWhiteSpace( DatabaseVersion ) )
					throw new Exception( "Could not get current database version" );

				string UpgradeScript = GetUpgradeScript( DatabaseVersion );
				if( string.IsNullOrWhiteSpace( UpgradeScript ) )
					break;

				if( !RunUpgradeScript( UpgradeScript ) )
					throw new Exception( "Error running upgrade script" );
			} while( true );

			Connection.Close();
		}
	}
}
