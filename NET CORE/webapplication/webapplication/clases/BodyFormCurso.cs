using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapplication.clases
{
    public class BodyFormCurso
    {
        public int account_id { get; set; }
        public string name { get; set; }
        public string course_code { get; set; }
        public bool is_public_to_auth_users { get; set; }
        public bool public_syllabus { get; set; }
        public bool public_syllabus_to_auth { get; set; }
        public string sis_course_id { get; set; }
        public string default_view { get; set; }

        public string? term_id { get; set; }
    }
}
