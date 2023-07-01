using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapplication.clases
{
    public class SisImportCanvas
    {
        public int id { get; set; }

        public string? created_at { get; set; }

        public int progress { get; set; }

        public string? workflow_state { get; set; }

        public List<List<String>>? processing_warnings { get; set; }
        
        //  public string[]? processing_errors { get; set; }
    }

}
