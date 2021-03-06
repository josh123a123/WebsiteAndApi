﻿using System;
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
		public bool? Accepted { get; internal set; }
		public int Id { get; internal set; }
		public string Notes { get; internal set; }
		public ImmutableList<ITag> Tags { get; private set; }
		public string Title { get; internal set; }
		public int UserId { get; internal set; }
		public int SessionLength { get; internal set; }

		public int TimeSlotId { get; internal set; }
		public ITimeSlot TimeSlot { get; internal set; }

		public int RoomId { get; internal set; }
		public IRoom Room { get; internal set; }

		public ISession UpdateAbstract( string value ) {
			SessionModel newSession = this.Clone();
			newSession.Abstract = value;
			return newSession;
		}

		public ISession UpdateAccepted( bool? value ) {
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

		public ISession UpdateSessionLength( int value ) {
			SessionModel newSession = this.Clone();
			newSession.SessionLength = value;
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

		public ISession UpdateTimeSlot( ITimeSlot value ) {
			SessionModel newSession = this.Clone();
			if( null == value ) {
				newSession.TimeSlotId = 0;
				newSession.TimeSlot = null;
			} else {
				newSession.TimeSlotId = value.Id;
				newSession.TimeSlot = new TimeSlotModel( value );
			}
			return newSession;
		}

		public ISession UpdateRoom( IRoom value ) {
			SessionModel newSession = this.Clone();
			if( null == value ) {
				newSession.RoomId = 0;
				newSession.Room = null;
			} else {
				newSession.RoomId = value.Id;
				newSession.Room = new RoomModel( value );
			}
			return newSession;
		}
		#endregion

		private SessionModel Clone() {
			SessionModel cloned = new SessionModel {
				Id = this.Id,
				UserId = this.UserId,
				Title = string.Copy( this.Title ),
				Abstract = string.Copy( this.Abstract ),
				SessionLength = this.SessionLength,
				Accepted = this.Accepted,
				Tags = this.Tags?.ToImmutableList(),
				TimeSlotId = this.TimeSlotId,
				RoomId = this.RoomId
			};

			if( null != this.TimeSlot )
				cloned.TimeSlot = new TimeSlotModel( this.TimeSlot );

			if( null != this.Room )
				cloned.Room = new RoomModel( this.Room );

			if( !string.IsNullOrWhiteSpace( cloned.Notes ) )
				cloned.Notes = string.Copy( this.Notes );

			return cloned;
		}
	}
}
