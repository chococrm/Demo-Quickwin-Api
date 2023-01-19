using BCRM.Common.Filters.Authorize;
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
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
