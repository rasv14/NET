using System;
using System.Collections.Generic;
using System.Text;

namespace webapplication.clases { 
    public class ModuloCanvas
    {
        public int id { get; set; }
        public string name { get; set; }
        public int position { get; set; }
        public string? unlock_at { get; set; }
        public bool require_sequential_progress { get; set; }
        public bool publish_final_grade { get; set; }
        public List<int> prerequisite_module_ids { get; set; }
        public string state { get; set; }
        public string completed_at { get; set; }
        public bool published { get; set; }
        public int items_count { get; set; }
        public string items_url { get; set; }

        public List<ItemCanvas>? items { get; set; }

    }
}
