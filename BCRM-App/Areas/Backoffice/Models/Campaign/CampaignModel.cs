using BCRM_App.Models.DBModel.Demoquickwin;
using System;
using System.Collections.Generic;

namespace BCRM_App.Areas.Backoffice.Models.Campaign
{
    public class CampaignModel
    {
        public CampaignModel(DemoQuickwin_Campaign campaign) 
        {
            CampaignId = campaign.CampaignId;
            Path = campaign.Path;
            Description = campaign.Description;
            Created_DT = campaign.Created_DT;
            Updated_DT = campaign.Updated_DT;
            Status = campaign.Status;
            Field = campaign.Field;
        }
        public int? CampaignId { get;set; }
        public string Path { get; set; }
        public string Description { get;set;}
        public DateTime Created_DT { get;set; }
        public DateTime Updated_DT { get;set; }
        public int? Status { get;set; }
        public int Field { get; set; }
        List<CampaignLoginModel> LoginRecords { get; set; }

    }

    public class Req_Campaign_List
    {
        public List<Data_Filter_Wrp> Filters { get; set; }
        public List<Data_Ordering_Wrp> Ordering { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 9999;
        public int Category_Id { get; set; }
    }

}
