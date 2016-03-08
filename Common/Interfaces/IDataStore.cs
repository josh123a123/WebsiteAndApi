using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSpace.Common {
	public interface IDataStore<T> {
		Task<T> Get( int Id );
		Task<IList<T>> GetAll();
	}
}
