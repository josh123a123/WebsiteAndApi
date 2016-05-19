namespace DevSpace.Common {
	public interface ITag {
		int Id { get; }
		string Text { get; }

		ITag UpdateId( int newId );
		ITag UpdateText( string newText );
	}
}
