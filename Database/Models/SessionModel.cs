using System;
using System.Collections.Immutable;
using System.Data.SqlClient;
using System.Reflection;
using DevSpace.Common;

namespace DevSpace.Database.Models {
	public class SessionModel : ISession {
		private SessionModel() {
			Tags = ImmutableList<ITag>.Empty;
		}

		internal SessionModel( SqlDataReader dataReader ) : this() {
			for( int lcv = 0; lcv < dataReader.FieldCount; ++lcv ) {
				PropertyInfo property = GetType().GetProperty( dataReader.GetName( lcv ), BindingFlags.Instance | BindingFlags.Public );
				if( null == property )
					continue;

				object value = dataReader.GetValue( lcv );
				if( DBNull.Value == value )
					value = null;

				property.SetValue( this, value );
			}
		}

		#region ISession
		public string Abstract { get; internal set; }
		public bool Accepted { get; internal set; }
		public int Id { get; internal set; }
		public string Notes { get; internal set; }
		public ImmutableList<ITag> Tags { get; private set; }
		public string Title { get; internal set; }
		public int UserId { get; internal set; }

		public ISession UpdateAbstract( string value ) {
			SessionModel newSession = this.Clone();
			newSession.Abstract = value;
			return newSession;
		}

		public ISession UpdateAccepted( bool value ) {
			SessionModel newSession = this.Clone();
			newSession.Accepted = value;
			return newSession;
		}

		public ISession UpdateId( int value ) {
			SessionModel newSession = this.Clone();
			newSession.Id = value;
			return newSession;
		}

		public ISession UpdateNotes( string value ) {
			SessionModel newSession = this.Clone();
			newSession.Notes = value;
			return newSession;
		}

		public ISession UpdateTitle( string value ) {
			SessionModel newSession = this.Clone();
			newSession.Title = value;
			return newSession;
		}

		public ISession UpdateUserId( int value ) {
			SessionModel newSession = this.Clone();
			newSession.UserId = value;
			return newSession;
		}

		public ISession AddTag( ITag value ) {
			SessionModel newSession = this.Clone();
			newSession.Tags = this.Tags.Add( value );
			return newSession;
		}

		public ISession RemoveTag( ITag value ) {
			SessionModel newSession = this.Clone();
			newSession.Tags = this.Tags.Remove( value );
			return newSession;
		}
		#endregion

		private SessionModel Clone() {
			SessionModel cloned = new SessionModel {
				Id = this.Id,
				UserId = this.UserId,
				Title = string.Copy( this.Title ),
				Abstract = string.Copy( this.Abstract ),
				Accepted = this.Accepted,
				Tags = this.Tags?.ToImmutableList()
			};

			if( !string.IsNullOrWhiteSpace( cloned.Notes ) )
				cloned.Notes = string.Copy( this.Notes );

			return cloned;
		}
	}
}
