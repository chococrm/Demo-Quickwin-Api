using Microsoft.AspNetCore.Mvc;
using BCRM.Common.Api;
using BCRM.Common.Api.Response;
using BCRM.Common.Constants;
using BCRM.Common.Factory;
using BCRM.Common.Filters.Action;
using BCRM.IAM.Constants;
using Microsoft.AspNetCore.Authorization;
using BCRM.Common.Services.RemoteInternal.IAM;
using BCRM.Common.Services.RemoteInternal.IAM.Model;
using System.Threading.Tasks;
using BCRM_App.Models.DBModel.Demoquickwin;
using BCRM.Common.Factory.Entities.Brand;
using BCRM_App.Areas.Api.Models.Customer;
using BCRM.Common.Models.DBModel.DG;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using BCRM.Common.Helpers;
using BCRM.Common.Services;
using BCRM_App.Areas.Api.Models;
using BCRM_App.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using static BCRM.Common.Services.BCRM_Client_Service_Api.Response;
using Org.BouncyCastle.Ocsp;
using BCRM_App.Filters;
using System.Globalization;

namespace BCRM_App.Areas.Api.Controllers
{
    public class ValuesController : BCRM_Controller
    {
        private readonly IBCRM_Client_Builder _client_Builder;
        private readonly IIAM_Client_Service _iamService;

        public ValuesController(
            ILogger<ValuesController> logger,
            IBCRM_Exception_Factory bcrm_Ex_Factory,
            IHttpContextAccessor httpContext_Accessor,
            IBCRM_Client_Builder client_Builder,
            IIAM_Client_Service iamService
            ) : base(logger, bcrm_Ex_Factory, httpContext_Accessor)
        {
            _client_Builder = client_Builder;
            _iamService = iamService;
        }

        private bool CheckMissingAddress(Req_Customer_Register req)
        {
            if (string.IsNullOrEmpty(req.Address) == true) return true;

            if (req.ProvinceId.HasValue == false) return true;

            if (string.IsNullOrEmpty(req.Province) == true) return true;

            if (req.DistrictId.HasValue == false) return true;

            if (string.IsNullOrEmpty(req.District) == true) return true;

            if (req.SubDistrictId.HasValue == false) return true;

            if (string.IsNullOrEmpty(req.SubDistrict) == true) return true;

            if (string.IsNullOrEmpty(req.PostalCode) == true) return true;

            return false;
        }

        [AllowAnonymous]
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Get)]
        public IActionResult GetMobileNo(int CustomerId)
        {
            try
            {
                using (BCRM_81_Entities bcrmContext = new BCRM_Brand_Entities_Factory<BCRM_81_Entities>().Create(BCRM.Common.Configs.BCRM_Config.Platform.Brand_Ref))
                {
                    DemoQuickwin_Customer_Info LoginInfo = bcrmContext.DemoQuickwin_Customer_Infos.FirstOrDefault(o => o.CustomerId == CustomerId);
                    if (LoginInfo == null)
                    {
                        throw _bcrm_Ex_Factory.Build(1000400, "No customer founded", "ไม่พบลูกค้าในฐานข้อมูล");
                    }
                    Resp_Customer_GetCustomerMobileNo resp = new Resp_Customer_GetCustomerMobileNo
                    {
                        MobileNo = LoginInfo.MobileNo
                    };

                    Data = resp;

                    Status = BCRM_Core_Const.Api.Result_Status.Success;
                }
            }
            catch (Exception ex)
            {
                ApiException = ex;
            }

            return Build_JsonResp();
        }

        [AllowAnonymous]
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Get)]
        public IActionResult GetIdcard(int CustomerId)
        {
            try
            {
                using (BCRM_81_Entities bcrmContext = new BCRM_Brand_Entities_Factory<BCRM_81_Entities>().Create(BCRM.Common.Configs.BCRM_Config.Platform.Brand_Ref))
                {
                    DemoQuickwin_Customer_Info LoginInfo = bcrmContext.DemoQuickwin_Customer_Infos.FirstOrDefault(o => o.CustomerId == CustomerId);
                    if (LoginInfo == null)
                    {
                        throw _bcrm_Ex_Factory.Build(1000400, "No customer founded", "ไม่พบลูกค้าในฐานข้อมูล");
                    }
                    string lastFour = LoginInfo.IdCard.Substring(LoginInfo.IdCard.Length - 4);
                    Resp_Customer_GetCustomerIdCard resp = new Resp_Customer_GetCustomerIdCard
                    {
                        IdCard = lastFour
                    };

                    Data = resp;

                    Status = BCRM_Core_Const.Api.Result_Status.Success;
                }
            }
            catch (Exception ex)
            {
                ApiException = ex;
            }

            return Build_JsonResp();
        }

        [AllowAnonymous]
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Get)]
        public IActionResult CreateState_Ref()
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

                                DemoQuickwin_Login_Info lineInfo = new DemoQuickwin_Login_Info
                                {
                                    Status = CCConstant.Line.Status.Line_Request,
                                    IAM_OAuth_TX_Ref = (string)data["txreference"],
                                    Line_OAuth_State = lineState,
                                    Created_DT = txTimeStamp,
                                    Updated_DT = txTimeStamp,
                                };

                                bcrmContext.DemoQuickwin_Login_Infos.Add(lineInfo);
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
                    throw _bcrm_Ex_Factory.Build(1000400, errorMessage/*"Failed to use Request_V2", "ไม่่สามารถยิง Request_V2"*/);

                return Redirect(/*redirectUrl*/"https://localhost:44306");
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
            /*int logId = 0;*/

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

                /*string lineId = lineData.line.user_id;
                string lineName = lineData.line.name;
                string linePictureUrl = lineData.line.picture_url;*/

                string iamToken = req.Access_Token; // IAM - Token
                string scope = CCConstant.Scopes.Desc.BCRM_Register;
                string redirectUrl = "";

                Dictionary<string, string> payload = new Dictionary<string, string>();

                using (BCRM_81_Entities bcrmContext = await new BCRM_Brand_Entities_Factory<BCRM_81_Entities>().CreateAsync(BCRM.Common.Configs.BCRM_Config.Platform.Brand_Ref))
                {
                    #region Update Line Info

                    // Search from existing Line UserId

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

                    #region Old CDP

                    // await SendDataToDaas(bcrmContext, loginInfo, lineInfo, campaignLogin, req.Identity_SRef, lineOAuthState);

                    #endregion

                    #region New CDP
                    /*
                                        bool isSendingToCDP = false;

                                        if (loginInfo.CampaignId.Value == CCConstant.Campaign.Id.Seventh)
                                        {
                                            if (lineInfo.SeventhCampaignStatus.Value == CCConstant_BCRM_App.Authentication.Line.RegisterStatus.NonRegister)
                                            {
                                                isSendingToCDP = true;
                                            }
                                        }
                                        else if (loginInfo.CampaignId.Value == CCConstant.Campaign.Id.Ninth)
                                        {
                                            if (lineInfo.NinthCampaignStatus.Value == CCConstant_BCRM_App.Authentication.Line.RegisterStatus.NonRegister)
                                            {
                                                isSendingToCDP = true;
                                            }
                                        }
                                        else if (loginInfo.CampaignId.Value > CCConstant.Campaign.Id.Twelveth)
                                        {
                                            if (campaign.IsDuplicate == null || campaign.IsDuplicate == false)
                                            {
                                                if (campaignLogin.Status == CCConstant_BCRM_App.Authentication.Line.RegisterStatus.NonRegister)
                                                {
                                                    isSendingToCDP = true;
                                                }
                                            }
                                            else if (campaign.IsDuplicate == true)
                                            {
                                                isSendingToCDP = true;
                                            }
                                        }

                                        if (isSendingToCDP == true)
                                        {
                                            NewCDP_Customer customer = new NewCDP_Customer
                                            {
                                                UserRef = req.Identity_SRef,
                                                Mobile = loginInfo.MobileNo,
                                                LinePictureProfile = lineInfo.LinePictureUrl,
                                                LineId = lineInfo.LineId,
                                                LineName = lineInfo.LineName,
                                                RegisterDate = lineInfo.Created_DT,
                                                PDPA_Consent = true,
                                                PDPA_Line = true,
                                                Created_DT = lineInfo.Created_DT,
                                                Updated_DT = txTimeStamp,
                                            };

                                            NewCDP_CampaignUserStatus campaignUserStatus = new NewCDP_CampaignUserStatus
                                            {
                                                CampaignID = loginInfo.CampaignId.Value,
                                                UserRef = req.Identity_SRef,
                                                Campaign_status = CCConstant.ThirdParty.CDP.Status.Login,
                                                Created_DT = txTimeStamp,
                                                Updated_DT = txTimeStamp,
                                                Round = 1
                                            };

                                            NewCDP_Log_Information logInformation = new NewCDP_Log_Information
                                            {
                                                Identity_SRef = req.Identity_SRef,
                                                CampaignId = loginInfo.CampaignId.Value,
                                            };

                                            await _newCdpService.SendDataToCDP(bcrmContext, customer, campaignUserStatus, logInformation);
                                        }
                    */
                    #endregion

                    #region Check Redirect

                    redirectUrl = $"{CCConstant.Line.FrontEndUrl}";

                    #endregion

                    IAM_Response iamResponse = await _iamService.TokenExchangeAsync(iamToken, CCConstant.App.Brand_Ref, scope, payload);

                    if (iamResponse.Success == false)
                    {
                        throw _bcrm_Ex_Factory.Build(1000401, iamResponse.ResponseError.Response.Error.System);
                    }

                    IAM_Token_Exchange_Resp tokenExchange = (IAM_Token_Exchange_Resp)iamResponse.ResponseSuccess;

                    redirectUrl = $"{redirectUrl}?token={tokenExchange.Access_Token}";

                    Status = BCRM_Core_Const.Api.Result_Status.Success;

                    return Redirect("https://localhost:44306" /*redirectUrl*/);
                }
            }
            catch (Exception ex)
            {
                ApiException = ex;
            }

            return Build_JsonResp();
        }



        [AllowAnonymous]
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Post)]
        [ApiAuthorize(CCConstant.App.Brand_Ref, CCConstant.Scopes.Desc.BCRM_Register)]
        public async Task<IActionResult> Register([FromBody] Req_Customer_Register req)
        {
            DateTime txTimeStamp = DateTime.Now;
            try
            {
                #region Check Parameter

                if (!ModelState.IsValid)
                    throw _bcrm_Ex_Factory.Build(1000400, "Please complete the information", "กรุณากรอกข้อมูลให้ครบถ้วน");
                Regex idCardRegex = new Regex(CCConstant.Common.IdCardRegex);
                if (idCardRegex.IsMatch(req.IdCard.Trim()) == false)
                    throw _bcrm_Ex_Factory.Build(1000400, "Invalid Id card", "ข้อมูลหมายเลขบัตรประชาชนไม่ถูกต้อง");
                Regex mobileNoRegex = new Regex(CCConstant.Common.MobileNoRegex);
                if (mobileNoRegex.IsMatch(req.MobileNo.Trim()) == false)
                    throw _bcrm_Ex_Factory.Build(1000400, "Invalid MobileNo", "ข้อมูลเบอร์โทรศัพท์ไม่ถูกต้อง");

                DateTime dateOfBirth;
                try
                {
                    string separator = "/";
                    string newDate = req.DateOfBirth;
                    string[] arr = req.DateOfBirth.Split(separator);
                    int newYear = int.Parse(arr[2]);

                    if (newYear >= 2400)
                    {
                        newYear -= 543;
                        newDate = $"{arr[0]}{separator}{arr[1]}{separator}{newYear}";
                    }

                    dateOfBirth = DateTime.ParseExact(newDate, CCConstant.Common.DateOfBirthFormat, CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    throw _bcrm_Ex_Factory.Build(1000400, "Invalid Date of Birth", "ข้อมูลวันเกิดไม่ถูกต้อง");
                }
                if (CheckMissingAddress(req) == true)
                    throw _bcrm_Ex_Factory.Build(1000400, "Please complete your address information", "กรุณากรอกข้อมูลที่อยู่ให้ครบถ้วน");

                #endregion

                #region add customer info to DB

                using (BCRM_81_Entities bcrmContext = await new BCRM_Brand_Entities_Factory<BCRM_81_Entities>().CreateAsync(BCRM.Common.Configs.BCRM_Config.Platform.Brand_Ref))
                {
                    DemoQuickwin_Customer_Info loginInfo = bcrmContext.DemoQuickwin_Customer_Infos.FirstOrDefault(o => o.IdCard == req.IdCard && o.MobileNo == req.MobileNo);
                    if (loginInfo == null)
                    {
                        DemoQuickwin_Customer_Info CustomerInfo = new DemoQuickwin_Customer_Info
                        {
                            CustomerId = 0,
                            Status = CCConstant.Customer.Status.Registered,

                            FirstName = req.FirstName,
                            LastName = req.LastName,
                            Gender = req.Gender,
                            IdCard = req.IdCard,
                            MobileNo = req.MobileNo,
                            DateOfBirth = req.DateOfBirth,

                            Address = req.Address,
                            ProvinceId = req.ProvinceId,
                            Province = req.Province,
                            DistrictId = req.DistrictId,
                            District = req.District,
                            SubDistrictId = req.SubDistrictId,
                            SubDistrict = req.SubDistrict,
                            PostalCode = req.PostalCode,

                            Created_DT = txTimeStamp,
                            Updated_DT = txTimeStamp
                        };
                        bcrmContext.DemoQuickwin_Customer_Infos.Add(CustomerInfo);
                        bcrmContext.SaveChanges();
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                ApiException = ex;
            }

            return Build_JsonResp();
        }

        /*// GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }*/
    }
}
