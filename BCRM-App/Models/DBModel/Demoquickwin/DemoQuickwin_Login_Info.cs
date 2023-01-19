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
        public DateTime Created_DT { get; set; }
        public DateTime Updated_DT { get; set; }
    }
}
