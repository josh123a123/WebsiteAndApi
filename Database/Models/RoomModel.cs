using System;
using System.Data.SqlClient;
using System.Reflection;
using DevSpace.Common;

namespace DevSpace.Database.Models {
	public class RoomModel : IRoom {
		private RoomModel() { }

		internal RoomModel( IRoom Room ) {
			this.Id = Room.Id;
			this.DisplayName = string.Copy( Room.DisplayName );
		}

		internal RoomModel( SqlDataReader dataReader ) {
			for( int lcv = 0; lcv < dataReader.FieldCount; ++lcv ) {
				GetType().GetProperty( dataReader.GetName( lcv ), BindingFlags.Instance | BindingFlags.Public )?.SetValue( this, dataReader.GetValue( lcv ) );
			}
		}

		#region IRoom
		public int Id { get; internal set; }
		public string DisplayName { get; internal set; }

		public IRoom UpdateId( int newId ) {
			return new RoomModel {
				Id = newId,
				DisplayName = string.Copy( this.DisplayName )
			};
		}

		public IRoom UpdateDisplayName( string newDisplayName ) {
			return new RoomModel {
				Id = this.Id,
				DisplayName = string.Copy( newDisplayName )
			};
		}
		#endregion
	}
}
