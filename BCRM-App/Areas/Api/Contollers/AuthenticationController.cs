﻿using BCRM.Common.Api;
using BCRM.Common.Api.Response;
using BCRM.Common.Constants;
using BCRM.Common.Factory;
using BCRM.Common.Factory.Entities.Brand;
using BCRM.Common.Filters.Action;
using BCRM.Common.Helpers;
using BCRM.Common.Services;
using BCRM.Common.Services.RemoteInternal.IAM;
using BCRM.IAM.Constants;
using BCRM_App.Constants;
using BCRM_App.Models.DBModel.Demoquickwin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using static BCRM.Common.Services.BCRM_Client_Service_Api.Response;

namespace BCRM_App.Areas.Api.Controllers
{
    public class AuthenticationController : BCRM_Controller
    {
        private readonly IBCRM_Client_Builder _client_Builder;
        private readonly IIAM_Client_Service _iamService;

        public AuthenticationController(ILogger<AuthenticationController> logger, IBCRM_Exception_Factory bcrm_Ex_Factory, IHttpContextAccessor httpContext_Accessor, 
            IBCRM_Client_Builder client_Builder, IIAM_Client_Service iamService
            ) : base(logger, bcrm_Ex_Factory, httpContext_Accessor)
        {
            _client_Builder = client_Builder;
            _iamService = iamService;
        }

        // GET/POST: api/v1/authentication/Callback
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Get | BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Post)]
        [BCRM_Api_Logging(Log_Header: true, Log_Req: true, Log_Req_All_Args: true, Log_Resp: false)]
        public IActionResult Callback()
        {
            try
            {
                Data = "Hello World !!!";

                Status = BCRM_Core_Const.Api.Result_Status.Success;
            }
            catch (Exception ex)
            {
                ApiException = ex;
            }

            return Build_JsonResp();
        }
    }
}