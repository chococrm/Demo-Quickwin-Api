using BCRM.Common.Filters.Authorize;
using BCRM_App.Constants;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace BCRM_App.Filters
{
    public class ApiAuthorize : BCRM_Authorize
    {
        public ApiAuthorize(string brandRef, string scopes)
            : base(brandRef, scopes)
        {
            
        }

        public override void ValidateExtraPayloads(AuthorizationFilterContext filterContext, JwtSecurityToken jwt_RawToken, Dictionary<string, object> dictExtraPayloads)
        {
            try
            {
                /*AddRouteData(filterContext, CCConstant.RouteData.Line.LineId, (string)dictExtraPayloads["line_id"]);
                AddRouteData(filterContext, CCConstant.RouteData.Line.LineOAuthState, (string)dictExtraPayloads["line_oauth_state"]);
                AddRouteData(filterContext, CCConstant.RouteData.Line.Linename, (string)dictExtraPayloads["line_name"]);
                AddRouteData(filterContext, CCConstant.RouteData.Line.PictureUrl, (string)dictExtraPayloads["line_picture_url"]);*/
                AddRouteData(filterContext, CCConstant.RouteData.Identity_SRef, (string)dictExtraPayloads["Identity_SRef"]);
            }
            catch (Exception)
            {
                
            }
        }
    }
}
