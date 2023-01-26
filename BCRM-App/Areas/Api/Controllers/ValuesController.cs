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
using System.Linq.Expressions;

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
                        FirstName= LoginInfo.FirstName,
                        LastName= LoginInfo.LastName,
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
                            DateOfBirth = dateOfBirth,

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
    }
}
