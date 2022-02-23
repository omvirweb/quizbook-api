using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Mobile_Service.Response
{
    public class code_response
    {
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public codes[] data { get; set; }
    }

    public class codes
    {        
        public int code { get; set; }                
        public bool is_used { get; set; }
        public int pin { get; set; }
    }
}