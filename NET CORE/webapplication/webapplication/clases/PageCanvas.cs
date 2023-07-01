using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapplication.clases
{
    public class PageCanvas
    {
        public int page_id { get; set; }
        public string title { get; set; }
        public string create_at { get; set; }
        public string url { get; set; }
        public string? editing_roles { get; set; }
        public bool? published { get; set; }
        public string html_url { get; set; }
        public string? update_at { get; set; }
        //public string page_url { get; set; }
       
    }
}
