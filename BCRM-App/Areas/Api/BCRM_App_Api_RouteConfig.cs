using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace BCRM_App.Areas.Api
{
    public class BCRM_App_Api_RouteConfig
    {
        public static IEndpointRouteBuilder Config(IEndpointRouteBuilder endpoints)
        {
            // Area - Api
            endpoints.MapAreaControllerRoute(
                name: "Api - Default",
                areaName: "Api",
                pattern: "Api/v{version:apiVersion}/{controller}/{action}/{id?}",
                defaults: new { area = "Api" }
            );

            return endpoints;
        }
    }
}
