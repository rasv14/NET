using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapplication.clases
{
    public class EnrollmentCanvas
    {
        public int id { get; set; }
        public int user_id { get; set; }
       
        public int course_id { get; set; }
        public string? type { get; set; }

        public string? role { get; set; }
        
        public string created_at { get; set; }

        public int? course_section_id { get; set; }

        public string? sis_course_id { get; set; }

        public string? sis_section_id { get; set; }

        public string? sis_user_id { get; set; }




        



    }

   
}
