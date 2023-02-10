using System;

namespace BCRM_App.Areas.Api.Models.Campaign
{
    public class Req_Campaign_Register
    {
        public int CampaignId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string MobileNo { get; set; }
        public string LastFour { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public int? ProvinceId { get; set; }
        public string Province { get; set; }
        public int? DistrictId { get; set; }
        public string District { get; set; }
        public int? SubDistrictId { get; set; }
        public string SubDistrict { get; set; }
        public string PostalCode { get; set; }
    }

    public class Req_Get_Plate
    {
        public int CampaignId;
    }

       public class Req_Update_Campaign
    {
        public int? CampaignId { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public DateTime Created_DT { get; set; }
        public DateTime Updated_DT { get; set; }
        public bool? IsClosed { get; set; }
        public int? Field { get; set; }

    }
}
