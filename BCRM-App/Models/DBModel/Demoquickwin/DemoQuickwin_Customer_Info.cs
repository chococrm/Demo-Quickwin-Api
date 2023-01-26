using System;
using System.Collections.Generic;

#nullable disable

namespace BCRM_App.Models.DBModel.Demoquickwin
{
    public partial class DemoQuickwin_Customer_Info
    {
        public int CustomerId { get; set; }
        public string Identity_SRef { get; set; }
        public int Status { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string MobileNo { get; set; }
        public string IdCard { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public int? ProvinceId { get; set; }
        public string Province { get; set; }
        public int? DistrictId { get; set; }
        public string District { get; set; }
        public int? SubDistrictId { get; set; }
        public string SubDistrict { get; set; }
        public string PostalCode { get; set; }
        public DateTime? Created_DT { get; set; }
        public DateTime? Updated_DT { get; set; }
    }
}
