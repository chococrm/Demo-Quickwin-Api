using BCRM.Common.Constants;

namespace BCRM_App.Constants
{
    public static partial class CCConstant
    {
        public static class App
        {
            public const int Brand_Id = 14;
            public const string Brand_Ref = "BLU95KI5B1HY";
            public const string App_Id = "ASN9VDZS2D3D";
            public const string App_Secret = "0D5FDF3DBF3B0A72B0459CD76ACFB4C24EF32427D6CD6CB576E2A2FC4DF1594078C2D79F7B25133C94AA144CA9470F7D50841A7E66B61CAA7EBD3522CDBDFB0B";
            public const string Provider_Ref_Login_Ext_Cred = "PVSXtcDblgF5AS";
        }
        public class Common
        {
            public const string EmailRegex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            public const string MobileNoRegex = "^[0-9]{10}$";
            public const string IdCardRegex = "^[0-9]{13}$";
            public const string CarRegistrationRegex_Front = "^[ก-ฮ0-9]{1,3}$";
            public const string CarRegistrationRegex_Back = "^[0-9]{1,4}$";
            public const string DateOfBirthFormat = "dd/MM/yyyy";
        }
        public static class Customer
        {
            public static class Gender
            {
                public const string Male = "M";
                public const string Female = "F";
                public const string Unknown = "N";

                public static class Desc
                {
                    public const string Male = "Male";
                    public const string Female = "Female";
                    public const string Unknown = "Unknown";
                }

                public static string Get_Desc(string Gender)
                {
                    switch (Gender)
                    {
                        case Male:
                            return Desc.Male;
                        case Female:
                            return Desc.Female;
                        case Unknown:
                            return Desc.Unknown;
                        default:
                            return Desc.Unknown;
                    }
                }
            }
            public static class Status
            {
                public const int NonRegistered = 0;
                public const int Registered = 1;
                public const int KYC_IdCard = 2;
            }
        }

        public class Database
        {
            public class ConnectionString
            {
                public static string Core = "";
            }
        }

        public class IAM
        {
            public const string Provider_Ref = "PVvrhoRoFWJcSU";
            public const string Callback_Url = "https://dev-bcrm-demo-api.azurewebsites.net/api/v1/authentication/Callback";
        }

        public class Line
        {
            public const string FrontEndUrl = "https://dev-bcrm-thanachart.azurewebsites.net";

            public class Redirect
            {
                public const string NotFound = "/not-found";
                public const string Success = "/success";
                public const string Register = "/register";
            }

            public class Status
            {
                public const int Line_Request = 1;
                public const int Line_Callback = 2;
                public const int Logined = 10;
                public const int Logout = 40;
                public const int Failed = 41;
            }
            public static class RegisterStatus
            {
                public const int NonRegister = 1;
                public const int Registered = 2;
            }
        }

        public static class RouteData
        {
            public const string IdentityId = BCRM_Core_Const.Api.RouteData.Key.Api_Token_IdentityId;
            public const string Identity_SRef = BCRM_Core_Const.Api.RouteData.Key.Api_Token_Identity_SRef;

            public class Line
            {
                public const string LineId = "BCRM_Api_LineId";
                public const string Linename = "BCRM_Api_LineName";
                public const string PictureUrl = "BCRM_Api_PictureUrl";
            }
        }

        public static class Scopes
        {
            public const int BCRM_Register = 0;
            public const int BCRM_App = 1;

            public class Desc
            {
                public const string BCRM_Register = "BCRM_Register";
                public const string BCRM_App = "BCRM_App";
            }
        }
    }
}