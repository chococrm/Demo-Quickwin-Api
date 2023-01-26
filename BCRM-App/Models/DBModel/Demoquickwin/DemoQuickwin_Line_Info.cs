using System;
using System.Collections.Generic;

#nullable disable

namespace BCRM_App.Models.DBModel.Demoquickwin
{
    public partial class DemoQuickwin_Line_Info
    {
        public int AccountId { get; set; }
        public string LineId { get; set; }
        public string LineName { get; set; }
        public string LinePictureUrl { get; set; }
        public string Identity_SRef { get; set; }
        public int Status { get; set; }
        public DateTime Created_DT { get; set; }
        public DateTime Updated_DT { get; set; }
    }
}
