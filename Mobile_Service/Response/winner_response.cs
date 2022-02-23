using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Mobile_Service.Response
{
    public class winner_response
    {
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public List<winank> data { get; set; }
    }
    public class winank
    {
        public long win_id { get; set; }
        public string company { get; set; }
        public string session { get; set; }
        public string ank { get; set; }
        public decimal amount { get; set; }
        public decimal rate { get; set; }
        public decimal winamt { get; set; }
    }
}