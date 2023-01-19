using System;
using System.Collections.Generic;

#nullable disable

namespace BCRM_App.Models.DBModel.Demoquickwin
{
    public partial class DemoQuickwin_Login_Info
    {
        public int AccountId { get; set; }
        public int? Status { get; set; }
        public string IAM_OAuth_TX_Ref { get; set; }
        public string Line_OAuth_State { get; set; }
        public string Access_Token { get; set; }
        public string Payload { get; set; }
        public string Identity_SRef { get; set; }
        public string Code { get; set; }
        public string UtmSource { get; set; }
        public string UtmMedium { get; set; }
        public string UtmCampaign { get; set; }
        public string UtmTerm { get; set; }
        public string UtmContent { get; set; }
        public int? CampaignId { get; set; }
        public string MobileNo { get; set; }
        public string IdCard { get; set; }
        public string Ref3 { get; set; }
        public string Ref4 { get; set; }
        public string Ref5 { get; set; }
        public DateTime Created_DT { get; set; }
        public DateTime Updated_DT { get; set; }
        public bool? IsResend { get; set; }
    }
}
