using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Mobile_Service.get_peram
{
    public class code_peram
    {
        [DataMember]
        public int code { get; set; }

        [DataMember]
        public string agentcode { get; set; }

        [DataMember]
        public int pin { get; set; }

        [DataMember]
        public int cid { get; set; }

        [DataMember]
        public string lookfor { get; set; }
    }
}