using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Mobile_Service.Response
{
    public class company_response
    {
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public comp[] data { get; set; }
    }
    public class comp
    {
        public int cid { get; set; }
        public string cname { get; set; }
        public string C_Guj { get; set; }
    }
    
}