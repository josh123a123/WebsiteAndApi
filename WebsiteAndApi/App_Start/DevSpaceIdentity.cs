using System;
using System.Security.Principal;
using DevSpace.Common;

namespace DevSpace.Api {
	public class DevSpaceIdentity : IIdentity {
		public DevSpaceIdentity( IUser identity ) {
			if( null == identity )
				throw new ArgumentNullException( "identity" );

			this.Identity = identity;
		}
		public IUser Identity { get; internal set; }

		#region IIdentity
		public string AuthenticationType { get { return "Basic Authentication"; } }
		public bool IsAuthenticated { get { return true; } }
		public string Name { get { return Identity.EmailAddress; } }
		#endregion
	}
}