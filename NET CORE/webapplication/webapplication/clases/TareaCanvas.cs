using System;
using System.Collections.Generic;
using System.Text;

namespace webapplication.clases
{
   public class TareaCanvas
    {
        public int id { get; set; }
    
        public string? title { get; set; }
        public string? last_reply_at { get; set; }
        public string? created_at { get; set; }
        public string? posted_at { get; set; }

        public int? assignment_id { get; set; }

        public string? user_name { get; set; }

        public bool? published { get; set; }
        public bool? can_unpublish { get; set; }
        public bool? locked { get; set; }

        public AuthorCanvas? author { get; set; }

        public AssignmentCanvas? assignment { get; set; }

        public string? html_url { get; set; }

        public string? url { get; set; }

    }


  
   



}



