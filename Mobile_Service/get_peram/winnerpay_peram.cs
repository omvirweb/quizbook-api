using Mobile_Service.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Mobile_Service.get_peram
{
    public class winnerpay_peram
    {
        [DataMember]
        public string barcode { get; set; }
        [DataMember]
        public string agent { get; set; }
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public long[] win_ank_list { get; set; }
    }

}