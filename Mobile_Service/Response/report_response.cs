using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Mobile_Service.Response
{
    public class report_response
    {
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public report[] data { get; set; }
    }
    public class report
    {
        public string collectdate { get; set; }
        public decimal totalcollect { get; set; }
        public decimal commission { get; set; }
        public decimal commamt { get; set; }
        public decimal winamt { get; set; }
        public decimal finalbalance { get; set; }
    }
}