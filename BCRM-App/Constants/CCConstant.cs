using BCRM.Common.Constants;

namespace BCRM_App.Constants
{
    public static partial class CCConstant
    {
        public class Common
        {
            public const string EmailRegex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            public const string MobileNoRegex = "^[0-9]{10}$";
            public const string IdCardRegex = "^[0-9]{13}$";
            public const string CarRegistrationRegex_Front = "^[ก-ฮ0-9]{1,3}$";
            public const string CarRegistrationRegex_Back = "^[0-9]{1,4}$";
            public const string DateOfBirthFormat = "dd/MM/yyyy";
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
            public const string Callback_Url = "https://dev-bcrm-thanachart-api.azurewebsites.net/api/v1/authentication/Callback";
        }

        public class Line
        {
            public class Status
            {
                public const int Line_Request = 1;
                public const int Line_Callback = 2;
                public const int Logined = 10;
                public const int Logout = 40;
                public const int Failed = 41;
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