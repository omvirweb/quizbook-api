using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Mobile_Service.get_peram
{
    public class changepass_peram
    {
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public string oldpassword { get; set; }
        [DataMember]
        public string newpassword { get; set; }
    }
}