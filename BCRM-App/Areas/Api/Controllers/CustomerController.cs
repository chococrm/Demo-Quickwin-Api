using Microsoft.AspNetCore.Mvc;
using BCRM.Common.Api;
using BCRM.Common.Constants;
using BCRM.Common.Context;
using BCRM.Common.Factory;
using BCRM.Common.Filters.Action;
using BCRM.Common.Models.Common.Token;
using BCRM.Common.Services;
using BCRM.Common.Services.CRM;
using BCRM.Common.Services.CRM.Model;
using BCRM.CRM.Constants;
using BCRM_App.Areas.Api.Models.Customer;
using BCRM_App.Constants;
using BCRM_App.Filters;
using BCRM_App.Helpers;
using Medallion.Threading.SqlServer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BCRM.IAM.Constants;
using Microsoft.AspNetCore.Authorization;
using BCRM.Common.Services.RemoteInternal.IAM;
using BCRM.Common.Services.RemoteInternal.IAM.Model;
using BCRM_App.Models.DBModel.Demoquickwin;
using BCRM.Common.Factory.Entities.Brand;
using BCRM.Common.Models.DBModel.DG;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using BCRM.Common.Helpers;
using BCRM_App.Areas.Api.Models;
using static BCRM.Common.Services.BCRM_Client_Service_Api.Response;
using Org.BouncyCastle.Ocsp;
using System.Linq.Expressions;
using DaaS.Authenticate;
using BCRM_App.Areas.Api.Models.Campaign;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MJ1.Helpers;
using static BCRM.Common.Constants.PDPA.BCRM_PDPA_Const.Article;
using static BCRM_App.Constants.CCConstant;
using BCRM_App.Areas.Backoffice.Models.Campaign;

namespace BCRM_App.Areas.Api.Controllers
{
    public class CustomerController : BCRM_Controller
    {
        private readonly IIAM_Client_Service _iamService;
        private readonly IBCRM_IdentityContext _identityContext;
        private readonly BCRM_81_Entities _bcrmContext;

        public CustomerController(
            BCRM_81_Entities bcrmContext,
            ILogger<CustomerController> logger,
            IBCRM_Exception_Factory bcrm_Ex_Factory,
            IHttpContextAccessor httpContext_Accessor,
            IBCRM_IdentityContext identityContext,
            IIAM_Client_Service iamService
            ) : base(logger, bcrm_Ex_Factory, httpContext_Accessor)
        {
            _bcrmContext = bcrmContext;
            _iamService = iamService;
            _identityContext = identityContext;
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

        #region Get infos

        //GET : api/v1/Customer/GetCampaigns
        [ApiAuthorize(CCConstant.App.Brand_Ref, "")]
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Get)]
        public IActionResult GetCampaigns()
        {/* Dictionary<string, string> filter, Dictionary<string, string> order,*/
            try
            {
                var query = (from campaign in _bcrmContext.DemoQuickwin_Campaigns
                             where (campaign.Status == 1)
                             select new CampaignModel(campaign)).ToList();

                List<CampaignModel> campaigns = new List<CampaignModel>();
                campaigns = query.ToList();
                Data = new
                {
                    Datas = campaigns,
                };
                Status = BCRM_Core_Const.Api.Result_Status.Success;
            }
            catch (Exception ex)
            {
                ApiException = ex;
            }
            return Build_JsonResp();
        }

        //GET : api/v1/Customer/GetCampaignDetails
        [ApiAuthorize(CCConstant.App.Brand_Ref, "")]
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Post)]
        public IActionResult GetCampaignDetails([FromBody] Req_Campaign_Detail req)
        {/* Dictionary<string, string> filter, Dictionary<string, string> order,*/
            try
            {
                /*DemoQuickwin_Campaign campaign = _bcrmContext.DemoQuickwin_Campaigns.FirstOrDefault(o=>o.CampaignId == );*/
                var query = (from campaign in _bcrmContext.DemoQuickwin_Campaigns
                             where (campaign.Status == 1)
                             && (campaign.CampaignId == req.CampaignId)
                             select new CampaignModel(campaign)).ToList();

                List<CampaignModel> campaigns = new List<CampaignModel>();
                campaigns = query.ToList();
                Data = new
                {
                    Datas = campaigns,
                };
                Status = BCRM_Core_Const.Api.Result_Status.Success;
            }
            catch (Exception ex)
            {
                ApiException = ex;
            }
            return Build_JsonResp();
        }

        // POST :api/v1/Customer/GetLinePic
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Get)]
        [ApiAuthorize(CCConstant.App.Brand_Ref, ""/*CCConstant.Scopes.Desc.BCRM_Register*/)]
        public IActionResult GetLinePic()
        {
            try
            {
                DemoQuickwin_Line_Info customerLineInfo = _bcrmContext.DemoQuickwin_Line_Infos.FirstOrDefault(o => o.Identity_SRef == _identityContext.Current.Identity_SRef);
                if (customerLineInfo == null)
                {
                    throw _bcrm_Ex_Factory.Build(1000400, "No customer infomation founded", "ไม่พบข้อมูลลูกค้าในฐานข้อมูล");
                }
                Data = customerLineInfo.LinePictureUrl;
            }
            catch (Exception ex)
            {
                ApiException = ex;
            }
            Status = BCRM_Core_Const.Api.Result_Status.Success;
            return Build_JsonResp();
        }

        #endregion

        #region Register

        //POST :api/v1/Customer/RegisterCampaign
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Post)]
        [ApiAuthorize(CCConstant.App.Brand_Ref, CCConstant.Scopes.Desc.BCRM_App)]
        [BCRM_Api_Logging(Log_Header: true, Log_Req: true, Log_Resp: true, Req_Keys: new string[] { "req" })]
        public IActionResult RegisterCampaign([FromBody] Req_Campaign_Register req)
        {
            int status = CCConstant.Customer.Status.NonRegistered;
            DateTime txTimeStamp = DateTime.Now;
            try
            {
                #region Check Parameter

                if (!ModelState.IsValid)
                    throw _bcrm_Ex_Factory.Build(1000400, "Please complete the information", "กรุณากรอกข้อมูลให้ครบถ้วน");
                if (req.CampaignId <= 0)
                    throw _bcrm_Ex_Factory.Build(1000400, "Invalid Campaign Infomation", "ข้อมูล Campaign ไม่ถูกต้อง");
                DemoQuickwin_Campaign campaign = _bcrmContext.DemoQuickwin_Campaigns.FirstOrDefault(o => o.CampaignId == req.CampaignId);
                if (campaign == null)
                {
                    throw _bcrm_Ex_Factory.Build(1000400, "Can not find the campaign", "ไม่พบแคมเปญในฐานข้อมูล");
                }
                else
                {
                    switch (campaign.Field)
                    {
                        // no info need
                        case CCConstant.Campaign.Field.NoInfoNeed:
                            break;
                        // LastFour of IdCard needed
                        case CCConstant.Campaign.Field.LastFourInfoNeed:
                            if (req.LastFour == null || req.LastFour == "")
                            {
                                throw _bcrm_Ex_Factory.Build(1000400, "Please complete the information", "กรุณากรอกข้อมูลให้ครบถ้วน");
                            }
                            break;
                    }
                }

                #endregion

                #region Process with data

                switch (campaign.Field)
                {
                    // no info need
                    case CCConstant.Campaign.Field.NoInfoNeed:
                        status = CCConstant.Customer.Status.Registered;
                        break;
                    // Check LastFour of IdCard
                    case CCConstant.Campaign.Field.LastFourInfoNeed:
                        try
                        {

                            DemoQuickwin_Customer_Info customerInfo = _bcrmContext.DemoQuickwin_Customer_Infos.FirstOrDefault(o => o.Identity_SRef == _identityContext.Current.Identity_SRef);
                            if (customerInfo == null)
                            {
                                throw _bcrm_Ex_Factory.Build(1000400, "No customer founded", "ไม่พบลูกค้าในฐานข้อมูล");
                            }
                            if (customerInfo.Status == 0)
                                throw _bcrm_Ex_Factory.Build(1000400, "No customer infomation founded", "ไม่พบข้อมูลลูกค้าในฐานข้อมูล");
                            string lastFour = customerInfo.IdCard.Substring(customerInfo.IdCard.Length - 4);
                            if (lastFour == null)
                                throw _bcrm_Ex_Factory.Build(1000400, "Error acccured, Information not found", "เกิดข้อผิดพลาดไม่พบข้อมูลเลขบัตรประชาชน");
                            if (req.LastFour != lastFour)
                            {
                                throw _bcrm_Ex_Factory.Build(1000400, "Information not correct", "ข้อมูลเลขบัตรประชาชนไม่ถูกต้อง");
                            }
                            status = CCConstant.Customer.Status.Registered;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        break;
                }

                #endregion

                #region Add customer info in DB record

                try
                {
/*                    var query = (from loginInfoQuery in _bcrmContext.DemoQuickwin_Campaign_Logins
                                 where ((loginInfoQuery.Identity_SRef == _identityContext.Current.Identity_SRef)
                                 &&(loginInfoQuery.CampaignId == req.CampaignId))
                                 orderby loginInfoQuery.LogId descending
                                 select new CampaignModel(campaign)).ToList();
*/
                    DemoQuickwin_Campaign_Login loginInfo = _bcrmContext.DemoQuickwin_Campaign_Logins
                        .Where(o => o.Identity_SRef == _identityContext.Current.Identity_SRef && o.CampaignId == req.CampaignId)
                        .OrderByDescending(o => o.LogId)
                        .FirstOrDefault();

                    if (loginInfo == null)
                    {
                        loginInfo = new DemoQuickwin_Campaign_Login
                        {
                            Identity_SRef = _identityContext.Current.Identity_SRef,
                            CampaignId = req.CampaignId,
                            Status = status,
                            Created_DT = txTimeStamp,
                            Updated_DT = txTimeStamp,
                            Isdelete = false
                        };
                        _bcrmContext.DemoQuickwin_Campaign_Logins.Add(loginInfo);
                        _bcrmContext.SaveChanges();
                    }
                    else if (loginInfo != null)
                    {
                        if (loginInfo.Isdelete == false)
                        {
                            loginInfo.Status = 0;
                            Status = BCRM_Core_Const.Api.Result_Status.Fail;
                            throw _bcrm_Ex_Factory.Build(1000400, "Customer already registered", "ลุกค้าได้ทำการสมัครเรียบร้อยแล้ว");
                        }
                        else if (loginInfo.Isdelete == true)
                        {
                            DemoQuickwin_Campaign_Login newLoginInfo = new DemoQuickwin_Campaign_Login
                            {
                                Identity_SRef = _identityContext.Current.Identity_SRef,
                                CampaignId = req.CampaignId,
                                Status = status,
                                Created_DT = txTimeStamp,
                                Updated_DT = txTimeStamp,
                                Isdelete = false
                            };
                            _bcrmContext.DemoQuickwin_Campaign_Logins.Add(newLoginInfo);
                            _bcrmContext.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                #endregion

                Status = BCRM_Core_Const.Api.Result_Status.Success;
            }
            catch (Exception ex)
            {
                ApiException = ex;
            }
            return Build_JsonResp();
        }

        //POST :api/v1/Customer/RegisterCustomer
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Post)]
        [ApiAuthorize(CCConstant.App.Brand_Ref, CCConstant.Scopes.Desc.BCRM_Register)]
        [BCRM_Api_Logging(Log_Header: true, Log_Req: true, Log_Resp: true, Req_Keys: new string[] { "req" })]
        public async Task<IActionResult> RegisterCustomer([FromBody] Req_Customer_Register req)
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

                #region update customer info in DB record

                DemoQuickwin_Customer_Info CustomerInfo = _bcrmContext.DemoQuickwin_Customer_Infos.FirstOrDefault(o => o.Identity_SRef == _identityContext.Current.Identity_SRef);
                if (CustomerInfo != null)
                {
                    CustomerInfo.Status = CCConstant.Customer.Status.Registered;

                    CustomerInfo.FirstName = req.FirstName;
                    CustomerInfo.LastName = req.LastName;
                    CustomerInfo.Gender = req.Gender;
                    CustomerInfo.IdCard = req.IdCard;
                    CustomerInfo.MobileNo = req.MobileNo;
                    CustomerInfo.DateOfBirth = dateOfBirth;
                    CustomerInfo.Address = req.Address;
                    CustomerInfo.ProvinceId = req.ProvinceId;
                    CustomerInfo.Province = req.Province;
                    CustomerInfo.DistrictId = req.DistrictId;
                    CustomerInfo.District = req.District;
                    CustomerInfo.SubDistrictId = req.SubDistrictId;
                    CustomerInfo.SubDistrict = req.SubDistrict;
                    CustomerInfo.PostalCode = req.PostalCode;
                    CustomerInfo.Updated_DT = txTimeStamp;
                    _bcrmContext.SaveChanges();
                }
                else
                {
                    Status = BCRM_Core_Const.Api.Result_Status.Fail;
                    throw _bcrm_Ex_Factory.Build(1000400, "Error acccured", "เกิดข้อผิดพลาด");
                }

                #endregion

                #region Token exchange

                string scope = CCConstant.Scopes.Desc.BCRM_App;
                Dictionary<string, string> payload = new Dictionary<string, string>();
                IAM_Response iamResponse = await _iamService.TokenExchangeAsync(_identityContext.IAM_Token, CCConstant.App.Brand_Ref, scope, payload);

                if (iamResponse.Success == false)
                {
                    throw _bcrm_Ex_Factory.Build(1000401, iamResponse.ResponseError.Response.Error.System);
                }

                IAM_Token_Exchange_Resp tokenExchange = (IAM_Token_Exchange_Resp)iamResponse.ResponseSuccess;

                #endregion

                string redirect = CCConstant.Redirect.Home;
                string redirectUrl = $"{CCConstant.Line.FrontEndUrl}{redirect}";
                redirectUrl = $"{redirectUrl}?token={tokenExchange.Access_Token}";

                Status = BCRM_Core_Const.Api.Result_Status.Success;
                Data = tokenExchange.Access_Token;
                //return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                ApiException = ex;
            }

            return Build_JsonResp();
        }

        #endregion

        #region Checking

        // POST :api/v1/Customer/CheckIdCard
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Post)]
        [ApiAuthorize(CCConstant.App.Brand_Ref, CCConstant.Scopes.Desc.BCRM_Register)]
        [BCRM_Api_Logging(Log_Header: true, Log_Req: true, Log_Req_All_Args: true, Log_Resp: false)]
        public IActionResult CheckIdCard([FromBody] Req_Customer_IdCard req)
        {
            DateTime txTimeStamp = DateTime.Now;
            try
            {
                DemoQuickwin_Customer_Info customerInfo = _bcrmContext.DemoQuickwin_Customer_Infos.FirstOrDefault(o => o.Identity_SRef == _identityContext.Current.Identity_SRef);
                if (customerInfo == null)
                {
                    throw _bcrm_Ex_Factory.Build(1000400, "No customer founded", "ไม่พบลูกค้าในฐานข้อมูล");
                }
                try
                {
                    if (customerInfo.Status == 0)
                        throw _bcrm_Ex_Factory.Build(1000400, "No customer infomation founded", "ไม่พบข้อมูลลูกค้าในฐานข้อมูล");
                    string lastFour = customerInfo.IdCard.Substring(customerInfo.IdCard.Length - 4);
                    if (lastFour == null)
                        throw _bcrm_Ex_Factory.Build(1000400, "Error acccured, Information not found", "เกิดข้อผิดพลาดไม่พบข้อมูลเลขบัตรประชาชน");
                    if (req.LastFour != lastFour)
                    {
                        throw _bcrm_Ex_Factory.Build(1000400, "Information not correct", "ข้อมูลเลขบัตรประชาชนไม่ถูกต้อง");
                    }
                    else
                    {
                        customerInfo.Status = CCConstant.Customer.Status.KYC_IdCard;
                        customerInfo.Updated_DT = txTimeStamp;
                        _bcrmContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }


                /*                string redirectUrl = CCConstant.Line.FrontEndUrl;
                                Dictionary<string, string> payload = new Dictionary<string, string>();

                                #region change token

                                IAM_Response iamResponse = await _iamService.TokenExchangeAsync(_identityContext.IAM_Token, CCConstant.App.Brand_Ref, CCConstant.Scopes.Desc.BCRM_App, payload);

                                if (iamResponse.Success == false)
                                {
                                    throw _bcrm_Ex_Factory.Build(1000401, iamResponse.ResponseError.Response.Error.System);
                                }

                                IAM_Token_Exchange_Resp tokenExchange = (IAM_Token_Exchange_Resp)iamResponse.ResponseSuccess;

                                #endregion

                                redirectUrl = $"{redirectUrl}?token={tokenExchange.Access_Token}";
                                Status = BCRM_Core_Const.Api.Result_Status.Success;
                                return Redirect(CCConstant.Line.FrontEndUrl);
                */
            }
            catch (Exception ex)
            {
                ApiException = ex;
            }
            Status = BCRM_Core_Const.Api.Result_Status.Success;
            return Build_JsonResp();
        }

        // POST :api/v1/Customer/CheckCampaignRegistered
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Post)]
        [ApiAuthorize(CCConstant.App.Brand_Ref, "")]
        [BCRM_Api_Logging(Log_Header: true, Log_Req: true, Log_Req_All_Args: true, Log_Resp: false)]
        public IActionResult CheckCampaignRegistered([FromBody] Req_Campaign_Detail req)
        {
            DateTime txTimeStamp = DateTime.Now;
            try
            {
                DemoQuickwin_Campaign_Login campaignLogin = _bcrmContext.DemoQuickwin_Campaign_Logins
                       .Where(o => o.Identity_SRef == _identityContext.Current.Identity_SRef && o.CampaignId == req.CampaignId)
                       .OrderByDescending(o => o.LogId)
                       .FirstOrDefault();

                if (campaignLogin == null)
                {
                    Data = "Non Registered";
                }
                else if (campaignLogin != null)
                {
                    Data = "Registered";
                }
            }
            catch (Exception ex)
            {
                ApiException = ex;
            }
            Status = BCRM_Core_Const.Api.Result_Status.Success;
            return Build_JsonResp();
        }

        #endregion
    }
}
