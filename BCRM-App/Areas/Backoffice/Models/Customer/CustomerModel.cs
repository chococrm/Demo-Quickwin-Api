using BCRM.Common.Models.DBModel.CRM;
using BCRM_App.Models.DBModel.Demoquickwin;
using System;
using System.ComponentModel;

namespace BCRM_App.Areas.Backoffice.Models.Customer
{
    public class CustomerModel
    {
        public CustomerModel(DemoQuickwin_Customer_Info customer, DemoQuickwin_Line_Info line)
        {
            LineName = line.LineName;
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            Gender = customer.Gender;
            MobileNo = customer.MobileNo;
            LinePictureUrl = line.LinePictureUrl;

            Address = customer.Address;
            Province = customer.Province;
            District = customer.District;
            SubDistrict = customer.SubDistrict;
            PostalCode = customer.PostalCode;

            DateOfBirth = customer.DateOfBirth;

            Created_DT = customer.Created_DT;
            Register_DT = customer.Updated_DT;
            LastLogin_DT = line.Updated_DT;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string MobileNo { get; set; }
        public string LineName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string SubDistrict { get; set; }
        public string PostalCode { get; set; }
        public DateTime Created_DT { get; set; }
        public DateTime Register_DT { get; set; }
        public DateTime LastLogin_DT { get; set; }
        public string LinePictureUrl { get; set; }
    }
}
