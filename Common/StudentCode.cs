using System.Runtime.Serialization;

namespace DevSpace.Common {
	[DataContract]
	public class StudentCode : IStudentCode {
		private StudentCode() { }

		[DataMember]public int Id { get; private set; }
		[DataMember]public string Email { get; private set; }
		[DataMember]public string Code { get; private set; }
	}
}
