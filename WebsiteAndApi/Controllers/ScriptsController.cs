using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace DevSpace.Api.Controllers {
	[AllowAnonymous]
	public class ScriptsController : ApiController {
		public HttpResponseMessage Get( string name ) {
			HttpResponseMessage response;
			string content = CreateJS( name );

			// Yes, we want to explicitly check for null
			// Sometimes, we want to send back an empty file
			// For example, if they don't use Google Analytics
			if( null == content ) {
				response = Request.CreateResponse( HttpStatusCode.NotFound );
			} else {
				response = Request.CreateResponse( HttpStatusCode.OK );
				response.Content = new StringContent( content, Encoding.UTF8, "text/javascript" );
			}

			return response;
		}

		private string CreateJS( string name ) {
			switch( name?.ToUpper() ) {
				case "ANALYTICS":
					return CreateAnalyticsJs();
			}

			return string.Empty;
		}

		private string CreateAnalyticsJs() {
			if( bool.Parse( ConfigurationManager.AppSettings["UseGoogleAnalytics"] ?? bool.FalseString ) ) {
				return string.Format(
					"(function(i,s,o,g,r,a,m){{i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){{(i[r].q=i[r].q||[]).push(arguments)}},i[r].l=1*new Date();a=s.createElement(o),m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)}})(window,document,'script','https://www.google-analytics.com/analytics.js','ga');ga('create','{0}','auto');ga('send','pageview');",
					ConfigurationManager.AppSettings["GoogleAnalyticsId"] ?? string.Empty
				);
			}

			return string.Empty;
		}
	}
}
