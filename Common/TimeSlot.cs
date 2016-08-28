using System;
using System.Runtime.Serialization;

namespace DevSpace.Common {
	[DataContract]
	public class TimeSlot : ITimeSlot {
		private TimeSlot() { }

		// Needed for Session
		internal TimeSlot( ITimeSlot timeSlot ) {
			this.Id = timeSlot.Id;
			this.StartTime = timeSlot.StartTime;
			this.EndTime = timeSlot.EndTime;
		}

		#region ITimeSlot
		[DataMember]	public int Id { get; private set; }
		[DataMember]	public DateTime StartTime { get; private set; }
		[DataMember]	public DateTime EndTime { get; private set; }

		public ITimeSlot UpdateId( int newId ) {
			return new TimeSlot {
				Id = newId,
				StartTime = this.StartTime,
				EndTime = this.EndTime
			};
		}

		public ITimeSlot UpdateStartTime( DateTime newStartTime ) {
			return new TimeSlot {
				Id = this.Id,
				StartTime = newStartTime,
				EndTime = this.EndTime
			};
		}

		public ITimeSlot UpdateEndTime( DateTime newEndTime ) {
			return new TimeSlot {
				Id = this.Id,
				StartTime = this.StartTime,
				EndTime = newEndTime
			};
		}
		#endregion
	}
}
