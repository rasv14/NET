using System;
using System.Collections.Generic;
using System.Text;

namespace webapplication.clases
{
   public class SeccionCanvas
    {
      

        public int id { get; set; }
        public int course_id { get; set; }
        public string name { get; set; }

        public string? start_at { get; set; }

        public string? end_at { get; set; }
        public string created_at { get; set; }
        public bool? restrict_enrollments_to_section_dates { get; set; }
        public string? nonxlist_course_id { get; set; }
        public string sis_section_id { get; set; }
        public string sis_course_id { get; set; }
        public string? integration_id { get; set; }
        public int? sis_import_id { get; set; }
        public int? total_students { get; set; }




    }



}



