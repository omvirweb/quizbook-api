using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Mobile_Service.Response
{
    public class cust_login_response
    {
        [DataMember]
        public int status { get; set; }

        [DataMember]
        public string message { get; set; }

        [DataMember]
        public decimal balance { get; set; }

        [DataMember]
        public decimal use_free { get; set; }

        [DataMember]
        public decimal current { get; set; }

        [DataMember]
        public string lookingfor { get; set; }

        [DataMember]
        public int code { get; set; }
    }        
}