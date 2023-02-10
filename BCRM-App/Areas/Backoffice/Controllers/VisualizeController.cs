﻿using BCRM.Common.Api;
using BCRM.Common.Context;
using BCRM.Common.Factory;
using BCRM.Common.Services.RemoteInternal.IAM;
using BCRM_App.Areas.Api.Controllers;
using BCRM_App.Models.DBModel.Demoquickwin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using static BCRM.Privilege.Constants.BCRM_PV_Const.Privilege.Verify;
using BCRM.Common.Constants;
using BCRM.Common.Filters.Action;
using BCRM_App.Areas.Backoffice.Models;
using BCRM_App.Areas.Backoffice.Models.Customer;
using BCRM_App.Constants;
using BCRM_App.Filters;
using BCRM_App.Helpers;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;
using BCRM_App.Areas.Backoffice.Models.Authentication;
using BCRM_App.Areas.Backoffice.Models.Campaign;
using MJ1.Helpers;
using BCRM_App.Areas.Api.Models.Campaign;

namespace BCRM_App.Areas.Backoffice.Controllers
{
    public class VisualizeController : BCRM_Controller
    {
        private readonly IIAM_Client_Service _iamService;
        private readonly IBCRM_IdentityContext _identityContext;
        private readonly BCRM_81_Entities _bcrmContext;

        public VisualizeController(
            BCRM_81_Entities bcrmContext,
            ILogger<VisualizeController> logger,
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

        #region API Query get data

        //GET : api/v1/Visualize/GetCampaigns
        [ApiAuthorize(CCConstant.App.Brand_Ref, "bcrm-bo-cust")]
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Get)]
        public IActionResult GetCampaigns(string search, int page = 1, int count = 10, DateTime? startDate = null, DateTime? finishDate = null)
        {/* Dictionary<string, string> filter, Dictionary<string, string> order,*/
            try
            {
                var query = (from campaign in _bcrmContext.DemoQuickwin_Campaigns

                             where ((startDate == null || campaign.Updated_DT.Date >= startDate.Value.Date)
                                   && (finishDate == null || campaign.Updated_DT.Date <= finishDate.Value.Date))
                                   && (string.IsNullOrEmpty(search)
                               || campaign.Description.Contains(search))
                             select new CampaignModel(campaign)).ToList();

                int totalRecord = 0;
                totalRecord = query.Count();
                try
                {
                    int skiped = (page - 1) * 10;

                    query = query.Skip(skiped).Take(count).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                List<CampaignModel> _logins = new List<CampaignModel>();
                _logins = query.ToList();

                Data = new
                {
                    Datas = _logins,
                    Count = totalRecord

                };

                Status = BCRM_Core_Const.Api.Result_Status.Success;

            }
            catch (Exception ex)
            {
                ApiException = ex;
            }
            return Build_JsonResp();
        }

        //GET : api/v1/Visualize/GetCustomers
        [ApiAuthorize(CCConstant.App.Brand_Ref, "bcrm-bo-cust")]
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Get)]
        public IActionResult GetCustomers(string search, int page = 1, int count = 10)
        {/* Dictionary<string, string> filter, Dictionary<string, string> order,*/
            try
            {
                var query = (from customer in _bcrmContext.DemoQuickwin_Customer_Infos
                             join lineAcc in _bcrmContext.DemoQuickwin_Line_Infos
                             on customer.Identity_SRef equals lineAcc.Identity_SRef

                             where (string.IsNullOrEmpty(search)
                               || customer.MobileNo.Contains(search)
                               || lineAcc.LineName.Contains(search)
                               || customer.Province.Contains(search)
                               || customer.District.Contains(search)
                               || customer.SubDistrict.Contains(search)
                               || customer.PostalCode.Contains(search)
                               || customer.FirstName.Contains(search)
                               || customer.LastName.Contains(search)
                               || customer.DateOfBirth.ToString().Contains(search))
                             select new CustomerModel(customer, lineAcc)).ToList();

                int totalRecord = 0;
                totalRecord = query.Count();
                try
                {
                    int skiped = (page - 1) * 10;

                    query = query.Skip(skiped).Take(count).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }



                List<CustomerModel> _customers = new List<CustomerModel>();
                _customers = query.ToList();

                Data = new
                {
                    Datas = _customers,
                    Count = totalRecord

                };

                Status = BCRM_Core_Const.Api.Result_Status.Success;

            }
            catch (Exception ex)
            {
                ApiException = ex;
            }
            return Build_JsonResp();
        }

        //GET : api/v1/Visualize/GetLoginRecords
        [ApiAuthorize(CCConstant.App.Brand_Ref, "bcrm-bo-cust")]
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Get)]
        public IActionResult GetLoginRecords(string search, int page = 1, int count = 10, DateTime? startDate = null, DateTime? finishDate = null, int status = 0)
        {/* Dictionary<string, string> filter, Dictionary<string, string> order,*/
            try
            {
                List<AuthenticationModel> query = null;
                switch (status)
                {
                    case 0:
                        query = (from login in _bcrmContext.DemoQuickwin_Login_Infos

                                 where ((startDate == null || login.Updated_DT.Date >= startDate.Value.Date)
                                       && (finishDate == null || login.Updated_DT.Date <= finishDate.Value.Date))
                                       && (string.IsNullOrEmpty(search))
                                 select new AuthenticationModel(login)).ToList();
                        break;
                    case 1:
                        query = (from login in _bcrmContext.DemoQuickwin_Login_Infos

                                 where ((startDate == null || login.Updated_DT.Date >= startDate.Value.Date)
                                       && (finishDate == null || login.Updated_DT.Date <= finishDate.Value.Date))
                                       && (string.IsNullOrEmpty(search)
                                       && (login.Identity_SRef != null))
                                 select new AuthenticationModel(login)).ToList();
                        break;
                    case 2:
                        query = (from login in _bcrmContext.DemoQuickwin_Login_Infos

                                 where ((startDate == null || login.Updated_DT.Date >= startDate.Value.Date)
                                       && (finishDate == null || login.Updated_DT.Date <= finishDate.Value.Date))
                                       && (string.IsNullOrEmpty(search)
                                       && (login.Identity_SRef == null))
                                 select new AuthenticationModel(login)).ToList();
                        break;
                    default:
                        query = (from login in _bcrmContext.DemoQuickwin_Login_Infos

                                 where ((startDate == null || login.Updated_DT.Date >= startDate.Value.Date)
                                       && (finishDate == null || login.Updated_DT.Date <= finishDate.Value.Date))
                                       && (string.IsNullOrEmpty(search))
                                 select new AuthenticationModel(login)).ToList();
                        break;
                }

                int totalRecord = 0;
                totalRecord = query.Count();
                try
                {
                    int skiped = (page - 1) * 10;

                    query = query.Skip(skiped).Take(count).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }



                List<AuthenticationModel> _logins = new List<AuthenticationModel>();
                _logins = query.ToList();

                Data = new
                {
                    Datas = _logins,
                    Count = totalRecord

                };

                Status = BCRM_Core_Const.Api.Result_Status.Success;

            }
            catch (Exception ex)
            {
                ApiException = ex;
            }
            return Build_JsonResp();
        }

        #endregion

        #region API Query Update/Add data

        //POST : api/v1/Visualize/AddCampaign
        [ApiAuthorize(CCConstant.App.Brand_Ref, "bcrm-bo-cust")]
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Post)]
        public IActionResult AddCampaign([FromBody] Req_Update_Campaign campaignUpdate)
        {
            DateTime txTimeStamp = DateTime.Now;
            try
            {
                #region check params
                if (campaignUpdate == null)
                {
                    throw _bcrm_Ex_Factory.Build(1000400, "No updating infomation found", "การส่งข้อมูลไม่ถูกต้อง ไม่พบข้อมูล");
                }
                if ((campaignUpdate.IsClosed.HasValue == false)
                    || (campaignUpdate.Description == null)
                    || (campaignUpdate.Path == null)
                    || (campaignUpdate.Field.HasValue == false))
                {
                    throw _bcrm_Ex_Factory.Build(1000400, "Updating infomation not valid", "การส่งข้อมูลไม่ถูกต้อง ข้อมูลไม่ครบถ้วน");
                }
                #endregion
                else
                {
                    DemoQuickwin_Campaign campaign = new DemoQuickwin_Campaign
                    {
                        Path = campaignUpdate.Path,
                        Description = campaignUpdate.Description,
                        IsClosed = campaignUpdate.IsClosed.Value,
                        Created_DT= txTimeStamp,
                        Updated_DT  = txTimeStamp
                    };
                    _bcrmContext.DemoQuickwin_Campaigns.Add(campaign);
                    _bcrmContext.SaveChanges();
                    Status = BCRM_Core_Const.Api.Result_Status.Success;
                }

            }
            catch (Exception ex)
            {
                ApiException = ex;
            }

            return Build_JsonResp();
        }

        //POST : api/v1/Visualize/UpdateCampaign
        [ApiAuthorize(CCConstant.App.Brand_Ref, "bcrm-bo-cust")]
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Post)]
        public IActionResult UpdateCampaign([FromBody] Req_Update_Campaign campaignUpdate)
        {
            DateTime txTimeStamp = DateTime.Now;
            try
            {
                if (campaignUpdate == null)
                {
                    throw _bcrm_Ex_Factory.Build(1000400, "No updating infomation found", "การส่งข้อมูลไม่ถูกต้อง ไม่พบข้อมูล");
                }
                if ((campaignUpdate.CampaignId.HasValue == false)
                    || (campaignUpdate.IsClosed.HasValue == false)
                    || (campaignUpdate.Description == null)
                    || (campaignUpdate.Path == null))
                {
                    throw _bcrm_Ex_Factory.Build(1000400, "Updating infomation not valid", "การส่งข้อมูลไม่ถูกต้อง ข้อมูลไม่ครบถ้วน");
                }
                else
                {
                    DemoQuickwin_Campaign campaign = _bcrmContext.DemoQuickwin_Campaigns.FirstOrDefault(o => o.CampaignId == campaignUpdate.CampaignId);
                    if (campaign == null)
                    {
                        throw _bcrm_Ex_Factory.Build(1000400, "Can not find campaign in database", "ไม่พบแคมเปญในฐานข้อมูล");
                    }
                    campaign.Path = campaignUpdate.Path;
                    campaign.Description = campaignUpdate.Description;
                    campaign.IsClosed = campaignUpdate.IsClosed.Value;
                    campaign.Updated_DT = txTimeStamp;
                    _bcrmContext.SaveChanges();
                    Status = BCRM_Core_Const.Api.Result_Status.Success;
                }

            }
            catch (Exception ex)
            {
                ApiException = ex;
            }

            return Build_JsonResp();
        }

        #endregion
    }
}

