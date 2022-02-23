using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Mobile_Service.Response
{
    public class bookschapter_response
    {
        [DataMember]
        public int status { get; set; }

        [DataMember]
        public string message { get; set; }

        [DataMember]
        public List<data_win_bought> data_win { get; set; }

        [DataMember]
        public List<data_win_bought> data_bought { get; set; }
    }

    public class data_win_bought
    {
        public long win_id { get; set; }
        public string company { get; set; }
        public string session { get; set; }
        public string ank { get; set; }
        public decimal amount { get; set; }
        public string pdf_url { get; set; }
    }
}