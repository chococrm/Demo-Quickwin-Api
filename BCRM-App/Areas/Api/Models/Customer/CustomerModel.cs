using System;

namespace BCRM_App.Areas.Api.Models.Customer
{
    public class Req_Customer_Register
    {
        public int? Status { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? SubDistrictId { get; set; }
        public string Identity_SRef { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string IdCard { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string SubDistrict { get; set; }
        public string PostalCode { get; set; }
        public string DateOfBirth { get; set; }
    }

    public class Resp_Customer_GetCustomerMobileNo
    {
        public string FirstName;
        public string LastName;
        public string MobileNo;
    }

    public class Resp_Customer_GetCustomerIdCard
    {
        public string FirstName;
        public string LastName;
        public string IdCard;
    }
}
