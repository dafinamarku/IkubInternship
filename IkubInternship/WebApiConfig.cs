using IkubInternship.RepositoryContracts;
using IkubInternship.RepositoryLayer;
using IkubInternship.ServiceContracts;
using IkubInternship.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Unity;
using Unity.AspNet.Mvc;

namespace IkubInternship
{
  public class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      

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