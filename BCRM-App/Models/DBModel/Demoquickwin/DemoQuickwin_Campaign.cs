using System;
using System.Collections.Generic;

#nullable disable

namespace BCRM_App.Models.DBModel.Demoquickwin
{
    public partial class DemoQuickwin_Campaign
    {
        public int CampaignId { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public DateTime Created_DT { get; set; }
        public DateTime Updated_DT { get; set; }
        public int Field { get; set; }
        public int Status { get; set; }
    }
}
