using BCRM_App.Models.DBModel.Demoquickwin;
using System;

namespace BCRM_App.Areas.Backoffice.Models.Campaign
{
    public class CampaignLoginModel
    {

        public CampaignLoginModel(DemoQuickwin_Campaign_Login Record)
        {
            CampaignId = Record.CampaignId;
            Identity_SRef= Record.Identity_SRef;
            Status= Record.Status;
            Created_DT= Record.Created_DT;
            Updated_DT= Record.Updated_DT;
            Isdelete= Record.Isdelete;
        }
        public int CampaignId { get; set; }
        public string Identity_SRef { get; set; }
        public int Status { get; set; }
        public DateTime Created_DT { get; set; }
        public DateTime Updated_DT { get; set; }
        public bool Isdelete { get; set; }

    }
}
