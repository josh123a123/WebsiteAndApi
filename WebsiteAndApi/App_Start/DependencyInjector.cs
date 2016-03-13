using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using DevSpace.Api.Controllers;

namespace DevSpace {
	internal class DependencyInjector : IDependencyResolver {
		public object GetService( Type serviceType ) {
			switch( serviceType.Name ) {
				case nameof( SponsorController ):
					return new SponsorController( new Database.SponsorDataStore() );
			}

			return null;
		}

		public IEnumerable<object> GetServices( Type serviceType ) {
			return new List<object>();
		}

		public IDependencyScope BeginScope() {
			return this;
		}

		public void Dispose() {
		}
	}
}