using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace EgoraMap
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Добавляем поддержку CORS
            config.EnableCors();
            // Конфигурация и службы веб-API

            // Маршруты веб-API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
