
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace webapplication.Models
{
    public class p_codesModelCanvas_Periodo
    {
        public string p_sis { get; set; }

        public string p_id_canvas { get; set; }
        public string p_fecha_migracion { get; set; }

        public string? p_fecha_inicio { get; set; }

        public string? p_fecha_fin { get; set; }
    }


    public class RootObjectCanvas_Periodo
    {
        public List<p_codesModelCanvas_Periodo> Codes { get; set; }


    }

   

    //public class RootObject
    //{
    //    public List<Person> People { get; set; }
    //}


}
