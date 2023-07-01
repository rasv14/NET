using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapplication.clases
{
    public class BodyFormUsuario
    {
       
        public string name { get; set; }
        public string short_name { get; set; }
        public string sortable_name { get; set; }
        public string unique_id { get; set; }
        public string sis_user_id { get; set; }
        public bool? send_confirmation { get; set; }
        public string address { get; set; }

        public bool? terms_of_use { get; set; }

        public bool? skip_registration { get; set; }

        public string authentication_provider_id { get; set; }

        public string? integration_id { get; set; }

        
    }
}
