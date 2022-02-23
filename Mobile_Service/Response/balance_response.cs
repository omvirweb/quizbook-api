using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Mobile_Service.Response
{
    public class balance_response
    {
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public balance[] data { get; set; }
    }
    public class balance
    {
        public decimal amount { get; set; }
        public decimal limit { get; set; }
       
    }
}