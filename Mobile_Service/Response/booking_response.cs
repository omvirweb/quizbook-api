using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Mobile_Service.get_peram;

namespace Mobile_Service.Response
{
    public class booking_response
    {       
            [DataMember]
            public int status { get; set; }
            [DataMember]
            public string message { get; set; }
            [DataMember]
            public booking_peram[] data { get; set; }
        
        
    }
}