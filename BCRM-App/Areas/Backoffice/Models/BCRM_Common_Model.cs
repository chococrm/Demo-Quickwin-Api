using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCRM_App.Areas.Backoffice.Models
{
    public class Data_Filter_Wrp
    {
        public int Seq { get; set; }
        public String Key { get; set; }
        public String Value { get; set; }
    }

    public class Data_Ordering_Wrp
    {
        public int Seq { get; set; }
        public String Key { get; set; }
        public String Direction { get; set; }
    }
}
