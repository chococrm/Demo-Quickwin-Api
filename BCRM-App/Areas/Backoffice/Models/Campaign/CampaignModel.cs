using BCRM_App.Models.DBModel.Demoquickwin;
using System;

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
            IsClosed = campaign.IsClosed;
        }
        public int? CampaignId { get;set; }
        public string Path { get; set; }
        public string Description { get;set;}
        public DateTime Created_DT { get;set; }
        public DateTime Updated_DT { get;set; }
        public bool? IsClosed { get;set; }

    }
}
