using BCRM_App.Models.DBModel.Demoquickwin;
using Org.BouncyCastle.Ocsp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace BCRM_App.Areas.Backoffice.Models.Authentication
{
    public class AuthenticationModel
    {
        public AuthenticationModel(DemoQuickwin_Login_Info login, DemoQuickwin_Line_Info line)
        {
            LineName = line.LineName;
            LinePictureUrl = line.LinePictureUrl;
            LoginTime = login.Updated_DT;
        }
        public AuthenticationModel(DemoQuickwin_Login_Info login)
        {

            LoginTime = login.Updated_DT;
            try
            {
                dynamic lineData = JsonConvert.DeserializeObject(login.Payload);
                LineName = lineData.line.name;
                LinePictureUrl = lineData.line.picture_url;
            }
            catch
            {

            }

        }
        public string LinePictureUrl { get; set; }
        public string LineName { get; set; }
        public DateTime LoginTime { get; set; }
/*        public string Payload { get; set; }*/

    }
}


