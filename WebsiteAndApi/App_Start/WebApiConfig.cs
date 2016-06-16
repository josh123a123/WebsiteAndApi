using System.Web.Http;

namespace DevSpace {
	public static class WebApiConfig {
		public static void Register( HttpConfiguration config ) {
			// Web API configuration and services
			config.DependencyResolver = new DependencyInjector();

			config.MessageHandlers.Add( new Api.Handlers.BasicAuthHandler() );
			config.MessageHandlers.Add( new Api.Handlers.SessionHandler() );
			config.MessageHandlers.Add( new Api.Handlers.ForcedAuthHandler() );

			// Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/v1/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
		}
	}
}
