using System;
using System.Data.SqlClient;
using System.Reflection;
using DevSpace.Common;

namespace DevSpace.Database.Models {
	public class TimeSlotModel : ITimeSlot {
		private TimeSlotModel() { }

		internal TimeSlotModel( SqlDataReader dataReader ) {
			for( int lcv = 0; lcv < dataReader.FieldCount; ++lcv ) {
				GetType().GetProperty( dataReader.GetName( lcv ), BindingFlags.Instance | BindingFlags.Public )?.SetValue( this, dataReader.GetValue( lcv ) );
			}
		}

		#region ITimeSlot
		public int Id { get; internal set; }
		public DateTime StartTime { get; internal set; }
		public DateTime EndTime { get; internal set; }

		public ITimeSlot UpdateId( int newId ) {
			return new TimeSlotModel {
				Id = newId,
				StartTime = this.StartTime,
				EndTime = this.EndTime
			};
		}

		public ITimeSlot UpdateStartTime( DateTime newStartTime ) {
			return new TimeSlotModel {
				Id = this.Id,
				StartTime = newStartTime,
				EndTime = this.EndTime
			};
		}

		public ITimeSlot UpdateEndTime( DateTime newEndTime ) {
			return new TimeSlotModel {
				Id = this.Id,
				StartTime = this.StartTime,
				EndTime = newEndTime
			};
		}
		#endregion
	}
}
