﻿using System.Web.Http;
using System.Web.Http.Cors;

namespace ticket_vendor_demo
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Enable CORS
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}