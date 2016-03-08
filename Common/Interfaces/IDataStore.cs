using System.Collections.Generic;

namespace DevSpace.Common {
	public interface IDataStore<T> {
		T Get( int Id );
		IList<T> GetAll();
	}
}
