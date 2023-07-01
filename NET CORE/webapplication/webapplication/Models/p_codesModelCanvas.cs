
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace webapplication.Models
{
    public class p_codesModelCanvas
    {
        public string p_sis { get; set; }
        public string p_fecha_migracion { get; set; }
    }


    public class RootObjectCanvas
    {
        public List<p_codesModelCanvas> Codes { get; set; }


    }

   

    //public class RootObject
    //{
    //    public List<Person> People { get; set; }
    //}


}
