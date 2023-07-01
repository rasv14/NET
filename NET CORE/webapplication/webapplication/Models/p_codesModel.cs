
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace webapplication.Models
{
    public class p_codesModel
    {
        public string p_code { get; set; }

    }


    public class RootObject
    {
        public List<p_codesModel> Codes { get; set; }


    }

   

    //public class RootObject
    //{
    //    public List<Person> People { get; set; }
    //}


}
