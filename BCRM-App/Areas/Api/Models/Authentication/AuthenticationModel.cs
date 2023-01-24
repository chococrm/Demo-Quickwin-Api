namespace BCRM_App.Areas.Api.Models
{
    public class Req_Authentication_Callback
    {
        public string Status { get; set; }

        public string App_Id { get; set; }

        public string Provider_Ref { get; set; }

        public string Identity_SRef { get; set; }

        public int OAuth_Type { get; set; }

        public int OAuth_Method { get; set; }

        public string TX_Reference { get; set; }

        public string State { get; set; }

        public string Access_Token { get; set; }

        public int Access_Token_Exp_Time { get; set; }

        public string Refresh_Token { get; set; }

        public int Refresh_Token_Exp_Time { get; set; }

        public string Payload { get; set; }
    }
}
