using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;

namespace DevSpace.Api {
	public class JsonBinder<I, C> : IModelBinder where C : I {
		public bool BindModel( HttpActionContext actionContext, ModelBindingContext bindingContext ) {
			HttpContent content = actionContext.Request.Content;
			string json = content.ReadAsStringAsync().Result;
			I obj = JsonConvert.DeserializeObject<C>( json );
			bindingContext.Model = obj;
			return true;
		}
	}
}