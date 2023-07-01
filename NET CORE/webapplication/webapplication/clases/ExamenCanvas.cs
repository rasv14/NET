using System;
using System.Collections.Generic;
using System.Text;

namespace webapplication.clases
{
   public class ExamenCanvas
    {
        public int id { get; set; }
        public string? title { get; set; }

        public string? html_url { get; set; }

        public string? mobile_url { get; set; }

        public string? quiz_type { get; set; }

        public bool? published { get; set; }
        public bool? unpublishable { get; set; }

        public bool? locked_for_user { get; set; }


        public string? lock_explanation { get; set; }

        public string? hide_results { get; set; }

        //     public List<OverrideCanvas>? overrides { get; set; }

        public int? assignment_id { get; set; }

        public List<All_Dates_Examen>? all_dates { get; set; }

    }


    public class All_Dates_Examen
    {
        public string? due_at { get; set; }
        public string? unlock_at { get; set; }

        public string? lock_at { get; set; }

        public string? title { get; set; }

        public string? set_type { get; set; }

        public int? set_id { get; set; }

    }


    




}



