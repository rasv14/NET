using System;
using System.Collections.Generic;
using System.Text;

namespace webapplication.clases
{
   public class AssignmentCanvas
    {
        public int id { get; set; }
        public string? description { get; set; }

        public bool? published { get; set; }
         public int? course_id { get; set; }

        public string? name { get; set; }

        public List<OverrideCanvas>? overrides { get; set; }

    }





}



