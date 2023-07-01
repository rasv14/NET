using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace webapplication.Handler
{
    public class HeadersParameters
    {
        [FromHeader]
        [Required]
        public string Authorization { get; set; }
    }
}
