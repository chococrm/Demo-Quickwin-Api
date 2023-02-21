using BCRM.Common.Api;
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
using BCRM_App.Areas.Api.Models.Customer;
using BCRM.Common.Constants;
using BCRM.Common.Filters.Action;
using BCRM_App.Areas.Backoffice.Models;
using BCRM_App.Areas.Backoffice.Models.Customer;
using BCRM_App.Constants;
using BCRM_App.Filters;
using System.Linq;
using BCRM_App.Areas.Backoffice.Models.Authentication;
using BCRM_App.Areas.Backoffice.Models.Campaign;
using BCRM_App.Areas.Api.Models.Campaign;
using static BCRM.Common.Constants.BCRM_Core_Const.Api.Filter;

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
        public IActionResult GetCampaigns(string search, int page = 1, int count = 10, DateTime? startDate = null, DateTime? finishDate = null, int? status = null)
        {/* Dictionary<string, string> filter, Dictionary<string, string> order,*/
            try
            {
                List<CampaignModel> query;
                if (status.HasValue == false)
                {
                    query = (from campaign in _bcrmContext.DemoQuickwin_Campaigns
                             where ((startDate == null || campaign.Updated_DT.Date >= startDate.Value.Date)
                                   && (finishDate == null || campaign.Updated_DT.Date <= finishDate.Value.Date))
                                   && (string.IsNullOrEmpty(search)
                                   || campaign.Description.Contains(search))
                             select new CampaignModel(campaign)).ToList();
                }
                else
                {
                    query = (from campaign in _bcrmContext.DemoQuickwin_Campaigns
                             where ((startDate == null || campaign.Updated_DT.Date >= startDate.Value.Date)
                                   && (finishDate == null || campaign.Updated_DT.Date <= finishDate.Value.Date))
                                   && (string.IsNullOrEmpty(search)
                                   && campaign.Status == status.Value
                                   || campaign.Description.Contains(search))
                             select new CampaignModel(campaign)).ToList();
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

        //GET : api/v1/Visualize/GetCampaigns
        [ApiAuthorize(CCConstant.App.Brand_Ref, "bcrm-bo-cust")]
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Get)]
        public IActionResult GetCampaigns(Req_Campaign_List req)
        {/* Dictionary<string, string> filter, Dictionary<string, string> order,*/
            try
            {
                IQueryable<DemoQuickwin_Campaign> query = _bcrmContext.DemoQuickwin_Campaigns.Where(o => o.CampaignId != null) as IQueryable<DemoQuickwin_Campaign>;

                int totalRecords = query.Count();
                // Total page calculation
                decimal pages = (Math.Ceiling((decimal)totalRecords / (decimal)req.Limit));
                pages = (pages == 0) ? 1 : pages;
                int totalPage = (int)pages;

                // Throw bad request if page to show > total page
                if (req.Page > totalPage) ModelState.AddModelError(nameof(req.Page), "Page to show > Total page");

                if (!ModelState.IsValid)
                {
                    return Build_BadRequest();
                }

                // Page calculation (cal from page) - Start from 0 
                int start = (req.Limit * (req.Page - 1));

                int totalFilteredRecords = query.Count();

                // Filter
                if (req.Filters != null)
                {
                    req.Filters = req.Filters.OrderBy(o => o.Seq).ToList();

                    foreach (Data_Filter_Wrp filter in req.Filters)
                    {
                        String key = filter.Key;
                        String search = filter.Value;
                        Int32.TryParse(search, out int searchInt);

                        if (String.Compare(key, "Status", true) == 0)
                        {
                            query = query.Where(o => Int64.Parse(search) == o.Status);
                            _logger.LogInformation("Filtering - Key:" + key + ", Value:" + search);

                        }
                    }

                    totalFilteredRecords = query.Count();

                    List<DemoQuickwin_Campaign> campaigns = query.ToList();
                    List<DemoQuickwin_Campaign> lists = new List<DemoQuickwin_Campaign>();
                    foreach (DemoQuickwin_Campaign row in campaigns)
                    {
                        lists.Add(row);
                    }
                    Status = BCRM_Core_Const.Api.Result_Status.Success;
                    Data = new
                    {
                        total_record = totalRecords,
                        filtered_record = totalFilteredRecords,
                        total_page = totalPage,
                        current_page = req.Page,
                        payload = lists
                    };
                }


            }
            catch (Exception ex)
            {
                ApiException = ex;
            }
            return Build_JsonResp();
        }

        //POST : api/v1/Visualize/GetCampaignDetails
        [ApiAuthorize(CCConstant.App.Brand_Ref, "bcrm-bo-cust")]
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Post)]
        public IActionResult GetCampaignDetails([FromBody] Req_Campaign_Detail req)
        {/* Dictionary<string, string> filter, Dictionary<string, string> order,*/
            try
            {
                var query = (from campaign in _bcrmContext.DemoQuickwin_Campaigns
                             where (campaign.CampaignId == req.CampaignId)
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

        //GET : api/v1/Visualize/GetCampaigns
        [ApiAuthorize(CCConstant.App.Brand_Ref, "bcrm-bo-cust")]
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Get)]
        public IActionResult GetCampaignLoginRecords(string search, int page = 1, int count = 10, DateTime? startDate = null, DateTime? finishDate = null, int? status = null)
        {
            try
            {
                List<CampaignLoginModel> query;
                if (status.HasValue == false)
                {
                    query = (from campaign in _bcrmContext.DemoQuickwin_Campaign_Logins

                             where ((startDate == null || campaign.Updated_DT.Date >= startDate.Value.Date)
                                   && (finishDate == null || campaign.Updated_DT.Date <= finishDate.Value.Date))
                                   && (string.IsNullOrEmpty(search)
                                   || (campaign.Identity_SRef == search))
                             select new CampaignLoginModel(campaign)).ToList();
                }
                else
                {
                    query = (from campaign in _bcrmContext.DemoQuickwin_Campaign_Logins

                             where ((startDate == null || campaign.Updated_DT.Date >= startDate.Value.Date)
                                   && (finishDate == null || campaign.Updated_DT.Date <= finishDate.Value.Date))
                                   && (string.IsNullOrEmpty(search)
                                   && (campaign.Status == status.Value)
                                   || (campaign.Identity_SRef == search))
                             select new CampaignLoginModel(campaign)).ToList();
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

                List<CampaignLoginModel> _logins = new List<CampaignLoginModel>();
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
                throw ex;
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
                if ((campaignUpdate.Status.HasValue == false)
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
                        Status = campaignUpdate.Status.Value,
                        Field = campaignUpdate.Field.Value,
                        Created_DT = txTimeStamp,
                        Updated_DT = txTimeStamp
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
                    || (campaignUpdate.Status.HasValue == false)
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
                    campaign.Status = campaignUpdate.Status.Value;
                    campaign.Field = campaignUpdate.Field.Value;
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

        //POST : api/v1/Visualize/UpdateCampaignStatus
        [ApiAuthorize(CCConstant.App.Brand_Ref, "bcrm-bo-cust")]
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Post)]
        public IActionResult UpdateCampaignStatus([FromBody] Req_Update_Campaign campaignUpdate)
        {
            DateTime txTimeStamp = DateTime.Now;
            try
            {
                if (campaignUpdate == null)
                {
                    throw _bcrm_Ex_Factory.Build(1000400, "No updating infomation found", "การส่งข้อมูลไม่ถูกต้อง ไม่พบข้อมูล");
                }
                if ((campaignUpdate.CampaignId.HasValue == false)
                    || (campaignUpdate.Status.HasValue == false))
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
                    campaign.Status = campaignUpdate.Status.Value;
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

        //POST : api/v1/Visualize/UpdateCampaign
        [ApiAuthorize(CCConstant.App.Brand_Ref, "bcrm-bo-cust")]
        [BCRM_AcceptVerb(BCRM_Core_Const.Api.Filter.BCRM_HttpMethods.Post)]
        public IActionResult ResetStatusCustomer([FromBody] Req_Customer_IdentitySRef req)
        {
            DateTime txTimeStamp = DateTime.Now;
            try
            {
                DemoQuickwin_Customer_Info customer = _bcrmContext.DemoQuickwin_Customer_Infos.FirstOrDefault(o => o.Identity_SRef == req.Identity_SRef);
                if (customer == null)
                {
                    throw _bcrm_Ex_Factory.Build(1000400, "No customer infomation found", "ไม่พบข้อมูลลูกค้าในฐานข้อมูล");
                }
                customer.Status = CCConstant.Customer.Status.NonRegistered;
                _bcrmContext.SaveChanges();
                Status = BCRM_Core_Const.Api.Result_Status.Success;
                Data = customer;
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


