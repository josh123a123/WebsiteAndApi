using System;

namespace DevSpace.Common {
	public interface IRoom {
		int Id { get; }
		string DisplayName { get; }

		IRoom UpdateId( int newId );
		IRoom UpdateDisplayName( string newDisplayName );
	}
}
