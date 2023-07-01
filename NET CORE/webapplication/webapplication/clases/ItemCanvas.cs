using System;
using System.Collections.Generic;
using System.Text;

namespace webapplication.clases
{
   public class ItemCanvas
    {
        public int id { get; set; }
        public string title { get; set; }
        public int position { get; set; }
        public int indent { get; set; }
        public bool quiz_lti { get; set; }
        public string type { get; set; }
        public int module_id { get; set; }
        public string html_url { get; set; }
        public string page_url { get; set; }
        public string url { get; set; }
        public bool published { get; set; }

       public  int? content_id { get; set; }
        public string? external_url { get; set; }
    }
}
