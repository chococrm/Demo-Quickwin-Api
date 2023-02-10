using System;
using System.Collections.Generic;

#nullable disable

namespace BCRM_App.Models.DBModel.Demoquickwin
{
    public partial class DemoQuickwin_Campaign_Login
    {
        public int LogId { get; set; }
        public int CampaignId { get; set; }
        public string Identity_SRef { get; set; }
        public int Status { get; set; }
        public DateTime Created_DT { get; set; }
        public DateTime Updated_DT { get; set; }
        public bool Isdelete { get; set; }
    }
}
