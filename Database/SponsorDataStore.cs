using System.Collections.Generic;
using DevSpace.Common;

namespace DevSpace.Database {
	public class SponsorDataStore : IDataStore<ISponsor> {
		public ISponsor Get( int Id ) {
			return null;
		}

		public IList<ISponsor> GetAll() {
			return new List<ISponsor>();
		}
	}
}
