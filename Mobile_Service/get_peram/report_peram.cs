using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Mobile_Service.get_peram
{
    public class report_peram
    {
        [DataMember]
        public string agent { get; set; }
        [DataMember]
        public string from { get; set; }
        [DataMember]
        public string to { get; set; }
    }
}