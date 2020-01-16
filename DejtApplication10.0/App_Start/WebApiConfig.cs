using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DejtApplication10._0
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();


            // kontroller endast
            // För att hantera routes som `/api/VarApi`
            config.Routes.MapHttpRoute(
                name: "ControllerOnly",
                routeTemplate: "api/{controller}"
            );


            // kontroller med ID
            // För att hantera routes som `/api/VarApi/1`
            config.Routes.MapHttpRoute(
                name: "ControllerAndId",
                routeTemplate: "api/{controller}/{id}",
                defaults: null,
                constraints: new { id = @"^\d+$" } // Only integers 
            );

            // kontrollers med Actions
            // För att hantera routes som `/api/VarApi/sök`
            config.Routes.MapHttpRoute(
                name: "ControllerAndAction",
                routeTemplate: "api/{controller}/{action}"
            );
        }
    }
}
