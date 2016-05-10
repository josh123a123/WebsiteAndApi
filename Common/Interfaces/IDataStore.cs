using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSpace.Common {
	public interface IDataStore<T> {
		Task<T> Get( int Id );
		Task<IList<T>> GetAll();
		Task<IList<T>> Get( string Field, object Value );
		Task<T> Add( T ItemToAdd );
		Task<T> Update( T ItemToUpdate );
	}
}
