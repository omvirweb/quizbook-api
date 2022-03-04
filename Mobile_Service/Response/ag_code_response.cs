using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Mobile_Service.Response
{
    public class ag_code_response
    {
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public ag_codes[] data { get; set; }
        [DataMember]
        public decimal balance { get; set; }
        [DataMember]
        public decimal client_pl { get; set; }
        [DataMember]
        public decimal current_bal { get; set; }
    }

    public class ag_codes
    {
        public int code { get; set; }
        public bool is_used { get; set; }
        public int pin { get; set; }
    }
}