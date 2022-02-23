using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Mobile_Service.Response
{
    public class changepass_response
    {
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public int data { get; set; }
    }
    
}