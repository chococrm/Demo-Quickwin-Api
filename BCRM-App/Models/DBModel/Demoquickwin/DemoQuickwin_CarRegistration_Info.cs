using System;
using System.Collections.Generic;

#nullable disable

namespace BCRM_App.Models.DBModel.Demoquickwin
{
    public partial class DemoQuickwin_CarRegistration_Info
    {
        public int PlateId { get; set; }
        public string Identity_SRef { get; set; }
        public int CampaignId { get; set; }
        public string CarRegistration_Front { get; set; }
        public string CarRegistration_Back { get; set; }
        public int? CarRegistrationProvinceId { get; set; }
        public string CarRegistrationProvince { get; set; }
        public DateTime Created_DT { get; set; }
        public DateTime Updated_DT { get; set; }
    }
}
