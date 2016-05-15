using System.Runtime.Serialization;

namespace DevSpace.Common {
	[DataContract]
	public class Tag : ITag {
		private Tag() {}

		#region ITag
		[DataMember]public int Id { get; private set; }
		[DataMember]public string Text { get; private set; }

		public ITag UpdateId( int newId ) {
			return new Tag {
				Id = newId,
				Text = string.Copy( this.Text )
			};
		}

		public ITag UpdateText( string newText ) {
			return new Tag {
				Id = this.Id,
				Text = string.Copy( newText )
			};
		}
		#endregion
	}
}
