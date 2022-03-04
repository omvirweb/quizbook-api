using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Mobile_Service.get_peram
{
    public class cust_code_bal_peram
    {
        [DataMember]
        public int code { get; set; }

        [DataMember]
        public decimal balance { get; set; }

        [DataMember]
        public string type { get; set; }
    }
}