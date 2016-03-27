using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DevSpace.Api.Controllers;
using DevSpace.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevSpace.WebsiteAndApi.Test {
	[TestClass]
	public class SponsorControllerTests {
		internal class TestSponsorLevel : ISponsorLevel {
			public bool DisplayInEmails { get; set; }
			public bool DisplayInSidebar { get; set; }
			public string DisplayName { get; set; }
			public int DisplayOrder { get; set; }
			public int Id { get; set; }
		}

		internal class TestSponsor : ISponsor {
			public string DisplayName { get; set; }
			public int Id { get; set; }
			public ISponsorLevel Level { get; set; }
			public string LogoLarge { get; set; }
			public string LogoSmall { get; set; }
			public string Website { get; set; }
		}

		internal class TestDataStore : Common.IDataStore<Common.ISponsor> {
			public delegate Task<ISponsor> GetDelegate( int Id );
			public GetDelegate GetFunction { get; set; }

			public Task<ISponsor> Get( int Id ) {
				return GetFunction( Id );
			}

			public delegate Task<IList<ISponsor>> GetAllDelegate();
			public GetAllDelegate GetAllFunction { get; set; }

			public Task<IList<ISponsor>> GetAll() {
				return GetAllFunction();
			}
		}

		[TestMethod]
		public void GetTest() {
			ISponsor Expected = new TestSponsor {
				Id = 1,
				DisplayName = "Test Sponsor",
				LogoLarge = "Logo Large",
				LogoSmall = "Logo Small",
				Website = "Test Website",
				Level = new TestSponsorLevel {
					Id = 1,
					DisplayName = "Test Sponsor Level",
					DisplayOrder = 1,
					DisplayInEmails = false,
					DisplayInSidebar = false
				}
			};

			TestDataStore testDataStore = new TestDataStore();
			testDataStore.GetFunction = ( id ) => {
				return Task.FromResult<ISponsor>( Expected );
			};

			SponsorController ControllerToTest = new SponsorController( testDataStore );
			HttpResponseMessage Actual = ControllerToTest.Get( 1 ).Result;

			Assert.IsNotNull( Actual );
			Assert.AreEqual( HttpStatusCode.OK, Actual.StatusCode );
			AssertSponorsAreEqual( Expected, Newtonsoft.Json.JsonConvert.DeserializeObject<Common.Sponsor>( Actual.Content.ReadAsStringAsync().Result ) );
		}

		[TestMethod]
		public void GetTest_NotFound() {
			TestDataStore testDataStore = new TestDataStore();
			testDataStore.GetFunction = ( id ) => {
				return Task.FromResult<ISponsor>( null );
			};

			SponsorController ControllerToTest = new SponsorController( testDataStore );
			HttpResponseMessage Actual = ControllerToTest.Get( 1 ).Result;

			Assert.IsNotNull( Actual );
			Assert.AreEqual( HttpStatusCode.NotFound, Actual.StatusCode );
		}

		[TestMethod]
		public void GetAllTest() {
			IList<ISponsor> Expected = new List<ISponsor>( 2 );
			Expected.Add( new TestSponsor {
				Id = 1,
				DisplayName = "Test Sponsor",
				LogoLarge = "Logo Large",
				LogoSmall = "Logo Small",
				Website = "Test Website",
				Level = new TestSponsorLevel {
					Id = 1,
					DisplayName = "Test Sponsor Level",
					DisplayOrder = 1,
					DisplayInEmails = false,
					DisplayInSidebar = false
				},
			} );
			Expected.Add( new TestSponsor {
				Id = 2,
				DisplayName = "Test Sponsor 2",
				LogoLarge = "Logo Large 2",
				LogoSmall = "Logo Small 2",
				Website = "Test Website 2",
				Level = new TestSponsorLevel {
					Id = 1,
					DisplayName = "Test Sponsor Level",
					DisplayOrder = 1,
					DisplayInEmails = false,
					DisplayInSidebar = false
				},
			} );

			TestDataStore testDataStore = new TestDataStore();
			testDataStore.GetAllFunction = () => {
				return Task.FromResult<IList<ISponsor>>( Expected );
			};

			SponsorController ControllerToTest = new SponsorController( testDataStore );
			HttpResponseMessage Actual = ControllerToTest.Get().Result;

			Assert.IsNotNull( Actual );
			Assert.AreEqual( HttpStatusCode.OK, Actual.StatusCode );
			AssertSponorsAreEqual( Expected[0], Newtonsoft.Json.JsonConvert.DeserializeObject<List<Common.Sponsor>>( Actual.Content.ReadAsStringAsync().Result )[0] );
			AssertSponorsAreEqual( Expected[1], Newtonsoft.Json.JsonConvert.DeserializeObject<List<Common.Sponsor>>( Actual.Content.ReadAsStringAsync().Result )[1] );
		}

		[TestMethod]
		public void GetAllTest_NotFound() {
			TestDataStore testDataStore = new TestDataStore();
			testDataStore.GetAllFunction = () => {
				return Task.FromResult<IList<ISponsor>>( new List<ISponsor>() );
			};

			SponsorController ControllerToTest = new SponsorController( testDataStore );
			HttpResponseMessage Actual = ControllerToTest.Get().Result;

			Assert.IsNotNull( Actual );
			Assert.AreEqual( HttpStatusCode.OK, Actual.StatusCode );

			IList<Common.Sponsor> ReturnedList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Common.Sponsor>>( Actual.Content.ReadAsStringAsync().Result );
			Assert.AreEqual( 0, ReturnedList.Count );
		}

		private void AssertSponorsAreEqual( ISponsor Expected, ISponsor Actual ) {
			Assert.AreEqual( Expected.Id, Actual.Id );
			Assert.AreEqual( Expected.DisplayName, Actual.DisplayName );
			Assert.AreEqual( Expected.LogoLarge, Actual.LogoLarge );
			Assert.AreEqual( Expected.LogoSmall, Actual.LogoSmall );
			Assert.AreEqual( Expected.Website, Actual.Website );
			AssertSponorLevelsAreEqual( Expected.Level, Actual.Level );
		}

		private void AssertSponorLevelsAreEqual( ISponsorLevel Expected, ISponsorLevel Actual ) {
			Assert.AreEqual( Expected.Id, Actual.Id );
			Assert.AreEqual( Expected.DisplayName, Actual.DisplayName );
			Assert.AreEqual( Expected.DisplayOrder, Actual.DisplayOrder );
			Assert.AreEqual( Expected.DisplayInEmails, Actual.DisplayInEmails );
			Assert.AreEqual( Expected.DisplayInSidebar, Actual.DisplayInSidebar );
		}
	}
}
