using System;

namespace DevSpace.Common {
	public interface ITimeSlot {
		int Id { get; }
		DateTime StartTime { get; }
		DateTime EndTime { get; }

		ITimeSlot UpdateId( int newId );
		ITimeSlot UpdateStartTime( DateTime newStartTime );
		ITimeSlot UpdateEndTime( DateTime newEndTime );
	}
}
