using BCRM.Common.Api;
using BCRM.Common.Api.Response;
using BCRM.Common.Constants;
using BCRM.Common.Factory;
using BCRM.Common.Factory.Entities.Brand;
using BCRM.Common.Filters.Action;
using BCRM.Common.Helpers;
using BCRM.Common.Services;
using BCRM.Common.Services.RemoteInternal.IAM;
using BCRM.Common.Services.RemoteInternal.IAM.Model;
using BCRM.IAM.Constants;
using BCRM_App.Areas.Api.Models;
using BCRM_App.Constants;
using BCRM_App.Models.DBModel.Demoquickwin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static BCRM.Common.Services.BCRM_Client_Service_Api.Response;

namespace BCRM_App.Areas.Api.Controllers
{
    public class AuthenticationController : BCRM_Controller
    {
        private readonly IBCRM_Client_Builder _client_Builder;
        private readonly IIAM_Client_Service _iamService;

        public AuthenticationController(
            ILogger<AuthenticationController> logger,
            IBCRM_Exception_Factory bcrm_Ex_Factory,
            IHttpContextAccessor httpContext_Accessor,
            IBCRM_Client_Builder client_Builder,
            IIAM_Client_Service iamService
            ) : base(logger, bcrm_Ex_Factory, httpContext_Accessor)
        {
            _client_Builder = client_Builder;
            _iamService = iamService;
        }

        // GET: api/v1/authentication/CreateState
        [AllowAnonymous]
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Get)]
        public IActionResult CreateState()
        {
            DateTime txTimeStamp = DateTime.Now;

            try
            {
                string redirectUrl = "";
                string lineState = StringHelper.Instance.RandomString(16);

                IBCRM_Client client = _client_Builder.Set_Service(BCRM_Client_Const.Service.IAM).Set_Token(BCRM.Common.Configs.BCRM_Config.Platform.App.App_Token).Client();
                client.Set_Header("bcrm-app-secret", BCRM.Common.Configs.BCRM_Config.Platform.App.App_Secret);

                var payload = new
                {
                    BCRM.Common.Configs.BCRM_Config.Platform.App.App_Id,
                    CCConstant.IAM.Provider_Ref,
                    State = lineState,
                    CCConstant.IAM.Callback_Url
                };

                string errorMessage = "";

                bool isSuccess = client.Request_v2(null, BCRM_IAM_Const.Api.Url.OAuth.Request, payload, HttpMethod.Post,
                (respData) =>
                {
                    var resp_Wrp = respData as BCRM_Client_Resp;
                    BCRM_Api_Response resp = resp_Wrp.Response;

                    if (resp.Status == BCRM_Core_Const.Api.Result_Status.Success)
                    {
                        try
                        {
                            JObject data = (JObject)resp.Data;
                            redirectUrl = (string)data["url"];

                            using (BCRM_81_Entities bcrmContext = new BCRM_Brand_Entities_Factory<BCRM_81_Entities>().Create(BCRM.Common.Configs.BCRM_Config.Platform.Brand_Ref))
                            {
                                // Login Info

                                DemoQuickwin_Login_Info loginInfo = new DemoQuickwin_Login_Info
                                {
                                    Status = CCConstant.Line.Status.Line_Request,
                                    IAM_OAuth_TX_Ref = (string)data["txreference"],
                                    Line_OAuth_State = lineState,
                                    Created_DT = txTimeStamp,
                                    Updated_DT = txTimeStamp,
                                };

                                bcrmContext.DemoQuickwin_Login_Infos.Add(loginInfo);
                                bcrmContext.SaveChanges();
                            }

                            Status = BCRM_Core_Const.Api.Result_Status.Success;
                        }
                        catch (Exception)
                        {

                        }
                    }

                    return true;
                },
                (respData) =>
                {
                    errorMessage = respData.Response.Error.System;

                    return false;
                });

                if (isSuccess == false)
                    throw _bcrm_Ex_Factory.Build(1000400, errorMessage);

                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                ApiException = ex;
            }

            return Build_JsonResp();
        }

        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Get | BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Post)]
        [BCRM_Api_Logging(Log_Header: true, Log_Req: true, Log_Req_All_Args: true, Log_Resp: false)]
        public async Task<IActionResult> Callback([FromQuery] Req_Authentication_Callback req)
        {

            DateTime txTimeStamp = DateTime.Now;

            try
            {
                if (req == null) return Build_BadRequest();

                if (req.Status != BCRM_Core_Const.Api.Result_Status.Success) return Build_BadRequest();

                // Verify App_Id and Provider_Ref
                if (req.App_Id != BCRM.Common.Configs.BCRM_Config.Platform.App.App_Id) throw _bcrm_Ex_Factory.Build(1000401, "Invalid Callback Information", "ข้อมูล CallBack ไม่ถูกต้อง");

                if (req.Provider_Ref != CCConstant.IAM.Provider_Ref) throw _bcrm_Ex_Factory.Build(1000401, "Invalid Callback Information", "ข้อมูล CallBack ไม่ถูกต้อง");

                // Query Thanachart Line - Info using IAM_TX_Ref and Line_OAuth_State                
                string iamOAuthTXRef = req.TX_Reference;
                string lineOAuthState = req.State;

                // Line data
                dynamic lineData = JsonConvert.DeserializeObject(req.Payload);

                string lineId = lineData.line.user_id;
                string lineName = lineData.line.name;
                string linePictureUrl = lineData.line.picture_url;

                string iamToken = req.Access_Token; // IAM - Token
                string scope = CCConstant.Scopes.Desc.BCRM_Register;
                string redirectUrl = "";

                Dictionary<string, string> payload = new Dictionary<string, string>();

                using (BCRM_81_Entities bcrmContext = await new BCRM_Brand_Entities_Factory<BCRM_81_Entities>().CreateAsync(BCRM.Common.Configs.BCRM_Config.Platform.Brand_Ref))
                {
                    #region Update Login Info

                    DemoQuickwin_Login_Info loginInfo = bcrmContext.DemoQuickwin_Login_Infos.FirstOrDefault(o => o.IAM_OAuth_TX_Ref == iamOAuthTXRef && o.Line_OAuth_State == lineOAuthState);

                    if (loginInfo == null)
                    {
                        throw _bcrm_Ex_Factory.Build(1000401, "Invalid Login State (Request Information not found)", "ข้อมูลการ Login ไม่ถูกต้อง");
                    }

                    loginInfo.Access_Token = lineData.line.access_token;
                    loginInfo.Payload = req.Payload;
                    loginInfo.Identity_SRef = req.Identity_SRef;
                    loginInfo.Updated_DT = txTimeStamp;

                    bcrmContext.SaveChanges();

                    #endregion

                    #region Add/Update Line info

                    DemoQuickwin_Line_Info lineInfo = bcrmContext.DemoQuickwin_Line_Infos.FirstOrDefault(o => o.Identity_SRef == req.Identity_SRef);
                    if (lineInfo == null)
                    {
                        lineInfo = new DemoQuickwin_Line_Info
                        {
                            Identity_SRef = req.Identity_SRef,
                            Status = CCConstant.Line.RegisterStatus.NonRegister,
                            LineId = lineId,
                            LineName = lineName,
                            LinePictureUrl = linePictureUrl,
                            Created_DT = txTimeStamp,
                            Updated_DT = txTimeStamp
                        };
                        bcrmContext.DemoQuickwin_Line_Infos.Add(lineInfo);
                        bcrmContext.SaveChanges();
                    }
                    else if (lineInfo != null)
                    {
                        lineInfo.LineId = lineId;
                        lineInfo.LineName = lineName;
                        lineInfo.LinePictureUrl = linePictureUrl;
                        lineInfo.Updated_DT = txTimeStamp;
                        bcrmContext.SaveChanges();
                    }

                    #endregion

                    #region Add Customer row 

                    DemoQuickwin_Customer_Info customerInfo = bcrmContext.DemoQuickwin_Customer_Infos.FirstOrDefault(o => o.Identity_SRef == req.Identity_SRef);
                    if (customerInfo == null)
                    {
                        // add customer row
                        customerInfo = new DemoQuickwin_Customer_Info
                        {
                            Identity_SRef = req.Identity_SRef,
                            Status = CCConstant.Customer.Status.NonRegistered,
                            Created_DT = txTimeStamp,
                            Updated_DT = txTimeStamp
                        };
                        bcrmContext.DemoQuickwin_Customer_Infos.Add(customerInfo);
                        bcrmContext.SaveChanges();
                    }

                    #endregion

                    #region check scope & redirect path

                    string redirect = "";

                    if (customerInfo.Status == CCConstant.Customer.Status.Registered)
                    {
                        scope = CCConstant.Scopes.Desc.BCRM_App;
                        redirect = CCConstant.Redirect.Home;
                    }
                    else
                    {
                        redirect = CCConstant.Redirect.Privacy;
                    }

                    redirectUrl = $"{CCConstant.Line.FrontEndUrl}{redirect}";

                    /*payload["Identity_SRef"] = req.Identity_SRef;*/

                    #endregion

                    IAM_Response iamResponse = await _iamService.TokenExchangeAsync(iamToken, CCConstant.App.Brand_Ref, scope, payload);

                    if (iamResponse.Success == false)
                    {
                        throw _bcrm_Ex_Factory.Build(1000401, iamResponse.ResponseError.Response.Error.System);
                    }

                    IAM_Token_Exchange_Resp tokenExchange = (IAM_Token_Exchange_Resp)iamResponse.ResponseSuccess;

                    redirectUrl = $"{redirectUrl}?token={tokenExchange.Access_Token}";

                    Status = BCRM_Core_Const.Api.Result_Status.Success;

                    return Redirect(redirectUrl);
                }
            }
            catch (Exception ex)
            {
                ApiException = ex;
            }

            return Build_JsonResp();
        }

    }
}