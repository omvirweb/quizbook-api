using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Mobile_Service.Response
{
    public class login_response
    {
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public log[] data { get; set; }
    }
    public class log
    {
        public string name { get; set; }
        public string type { get; set; }
    }
    public enum responsestatuscode
    {
        success,
        failure,
        nodatafound
    };
}