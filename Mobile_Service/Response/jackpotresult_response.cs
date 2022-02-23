using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Mobile_Service.Response
{
    public class jackpotresult_response
    {
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public jackpotank[] data { get; set; }
    }
    public class jackpotank
    {
        public string company { get; set; }
        public string dayopen { get; set; }
        public string dayclose { get; set; }
        public string nightopen { get; set; }
        public string nightclose { get; set; }
    }

}